using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;

namespace RockCollect
{
    public class RockListToShapeFile
    {
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: RockListToShapeFile.exe <rocklist path>");
                return 1;
            }

            try
            {
                Convert(args[0]);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                return 1;
            }

            return 0;
        }

        public static void Convert(string path, bool overwrite = false)
        {
            
            string shpPath = Path.ChangeExtension(path, ".shp");

            Console.WriteLine(string.Format("Converting rock list {0} to shape file {1}", path, shpPath));

            if (File.Exists(shpPath))
            {
                if (!overwrite) throw new Exception(string.Format("Not overwriting existing {0}", shpPath));

                File.Delete(shpPath);

                string shxPath = Path.ChangeExtension(path, ".shx");
                if (File.Exists(shxPath))
                {
                    File.Delete(shxPath);
                }

                string dbfPath = Path.ChangeExtension(path, ".dbf");
                if (File.Exists(dbfPath))
                {
                    File.Delete(dbfPath);
                }
            }

            const string expectedColumns = "id, tileR, tileC, shaX, shaY, rockX, rockY, tileShaX, tileShaY, shaArea, shaLen, rockWidth, rockHeight, score, gradMean, Compact, Exent, Class, gamma";

            var rocks = new List<RockDetector.OUTROCK>();
            bool foundColumnHeader = false;
            bool foundGsd = false;
            float gsd = 0.0f;
            int lineNumber = 0;

            foreach (string line in File.ReadLines(path))
            {
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line)) continue;

                if (!foundColumnHeader && line.TrimStart().StartsWith("version"))
                {
                    continue;
                }

                if (!foundColumnHeader && line.TrimStart().StartsWith("%"))
                {
                    if (!foundGsd && line.Contains("GSD_resolution"))
                    {
                        string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                        {
                            throw new Exception(string.Format(
                                "Invalid GSD_resolution format at line {0}: expected whitespace followed by a number",
                                lineNumber));
                        }

                        string valueStr = parts[parts.Length - 1];
                        if (!float.TryParse(valueStr, out gsd))
                        {
                            throw new Exception(string.Format("Failed to parse GSD_resolution at line {0}: \"{1}\"",
                                                              lineNumber, valueStr));
                        }

                        foundGsd = true;
                    }
                    continue;
                }

                if (!foundColumnHeader)
                {
                    if (line != expectedColumns)
                    {
                        throw new Exception(string.Format("Column names mismatch at line {0}.\nExpected: {1}\nGot: {2}",
                                                          lineNumber, expectedColumns, line));
                    }

                    if (!foundGsd)
                    {
                        throw new Exception("GSD_resolution header not found in file");
                    }

                    foundColumnHeader = true;
                    continue;
                }

                string[] values = line.Split(',');
                if (values.Length != 19)
                {
                    throw new Exception(string.Format("Expected 19 values at line {0}, got {1}",
                                                      lineNumber, values.Length));
                }

                try
                {
                    var rock = new RockDetector.OUTROCK
                    {
                        id = int.Parse(values[0].Trim()),
                        tileR = int.Parse(values[1].Trim()),
                        tileC = int.Parse(values[2].Trim()),
                        shaX = float.Parse(values[3].Trim()),
                        shaY = float.Parse(values[4].Trim()),
                        rockX = float.Parse(values[5].Trim()),
                        rockY = float.Parse(values[6].Trim()),
                        tileShaX = float.Parse(values[7].Trim()),
                        tileShaY = float.Parse(values[8].Trim()),
                        shaArea = int.Parse(values[9].Trim()),
                        shaLen = float.Parse(values[10].Trim()),
                        rockWidth = float.Parse(values[11].Trim()),
                        rockHeight = float.Parse(values[12].Trim()),
                        score = float.Parse(values[13].Trim()),
                        gradMean = float.Parse(values[14].Trim()),
                        compact = float.Parse(values[15].Trim()),
                        extent = float.Parse(values[16].Trim()),
                        Class = int.Parse(values[17].Trim()),
                        gamma = float.Parse(values[18].Trim())
                    };
                    rocks.Add(rock);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error parsing line {0}: {1}", lineNumber, ex.Message));
                }
            }

            if (gsd < RockDetector.MIN_VALID_GSD || gsd > RockDetector.MAX_VALID_GSD)
            {
                throw new Exception(string.Format("Invalid ground sampling distance {0} not in range {1} to {2}",
                                                  gsd, RockDetector.MIN_VALID_GSD, RockDetector.MAX_VALID_GSD));
            }

            Console.WriteLine(string.Format("Loaded rock list {0} with {1} rocks, gsd={2}", path, rocks.Count(), gsd));

            if (!foundColumnHeader)
            {
                throw new Exception("No column header found in file");
            }

            if (rocks.Count() == 0)
            {
                throw new Exception("Empty rocklist");
            }

            //now write an ESRI shape file containing all the rocks and their metadata
            //the specifics here mimic the functionality of original matlab script rocklist2shapefileNOMAP.m
            //by Marshall Trautman
            //the original script is attached to https://github.com/nasa-jpl/ROSCO/issues/7

            var fields = new List<DbfField>();
            var idField = fields.AddNumericInt32Field("id");
            var tileRField = fields.AddNumericInt32Field("tileR");
            var tileCField = fields.AddNumericInt32Field("tileC");
            var shaXField = fields.AddFloatField("shaX");
            var shaYField = fields.AddFloatField("shaY");
            var rockXField = fields.AddFloatField("rockX");
            var rockYField = fields.AddFloatField("rockY");
            var tileShaXField = fields.AddFloatField("tileShaX");
            var tileShaYField = fields.AddFloatField("tileShaY");
            var shaAreaField = fields.AddNumericInt32Field("shaArea");
            var shaLenField = fields.AddFloatField("shaLen");
            var rockWidthField = fields.AddFloatField("rockWidth");
            var rockHeightField = fields.AddFloatField("rockHeight");
            var scoreField = fields.AddFloatField("score");
            var gradMeanField = fields.AddFloatField("gradMean");
            var compactField = fields.AddFloatField("Compact");
            var extentField = fields.AddFloatField("Extent");
            var classField = fields.AddNumericInt32Field("Class");
            var gammaField = fields.AddFloatField("gamma");
            var diamMField = fields.AddFloatField("DiamM");
            var radiusField = fields.AddFloatField("Radius");
            var radiusMField = fields.AddFloatField("RadiusM");

            Console.WriteLine(string.Format("Saving shape file {0}...", shpPath));

            var options = new ShapefileWriterOptions(ShapeType.Polygon, fields.ToArray());
            using (var writer = Shapefile.OpenWrite(shpPath, options))
            {
                foreach (RockDetector.OUTROCK rock in rocks)
                {
                    const int numSides = 18;
                    var coords = new Coordinate[numSides + 1];
                    double radius = rock.rockWidth / 2.0;
                    for (int i = 0; i <= numSides; i++)
                    {
                        //the negative y coordinate here replicates the functionality of original matlab code
                        //in readrockList.m by Marshall Trautman
                        double angle = i < numSides ? (2.0 * Math.PI * i / numSides) : 0;
                        double x = rock.rockX + radius * Math.Cos(angle);
                        double y = -rock.rockY + radius * Math.Sin(angle);
                        coords[i] = new Coordinate(x, y);
                    }
                    writer.Geometry = new Polygon(new LinearRing(coords));

                    idField.NumericValue = rock.id;
                    tileRField.NumericValue = rock.tileR;
                    tileCField.NumericValue = rock.tileC;
                    shaXField.NumericValue = rock.shaX;
                    shaYField.NumericValue = rock.shaY;
                    rockXField.NumericValue = rock.rockX;
                    rockYField.NumericValue = rock.rockY;
                    tileShaXField.NumericValue = rock.tileShaX;
                    tileShaYField.NumericValue = rock.tileShaY;
                    shaAreaField.NumericValue = rock.shaArea;
                    shaLenField.NumericValue = rock.shaLen;
                    rockWidthField.NumericValue = rock.rockWidth;
                    rockHeightField.NumericValue = rock.rockHeight;
                    scoreField.NumericValue = rock.score;
                    gradMeanField.NumericValue = rock.gradMean;
                    compactField.NumericValue = rock.compact;
                    extentField.NumericValue = rock.extent;
                    classField.NumericValue = rock.Class;
                    gammaField.NumericValue = rock.gamma;
                    diamMField.NumericValue = gsd * rock.rockWidth;
                    radiusField.NumericValue = rock.rockWidth / 2;
                    radiusMField.NumericValue = gsd * rock.rockWidth / 2;

                    writer.Write();
                }
            }

            Console.WriteLine(string.Format("Saved {0} rocks to shape file {1}", rocks.Count(), shpPath));
        }
    }
}
