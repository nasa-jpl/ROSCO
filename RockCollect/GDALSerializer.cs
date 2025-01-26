using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSGeo.GDAL;

namespace RockCollect
{
    class GDALSerializer
    {
        static object gdalLockObj = new object();
        static Dictionary<string, Tuple<string, bool>> extensionToGdalDriver;
        static Dictionary<Type, DataType> systemTypeToGdalType;
        static Dictionary<DataType, Type> gdalTypeToSystemType;
        static string filePickerString;

        static GDALSerializer()
        {
            lock (gdalLockObj)
            {
                GdalConfiguration.ConfigureGdal();
                GdalConfiguration.ConfigureOgr();

                // Specify mapping from extension to gdal driver type
                // and whether or not the file needs to be written using
                // CreateCopy from memory.
                // Lots more file types available if built with gdal
                // http://www.gdal.org/formats_list.html
                extensionToGdalDriver = new Dictionary<string, Tuple<string, bool>>();
                extensionToGdalDriver.Add(".pgm", new Tuple<string, bool>("PNM", true));
                extensionToGdalDriver.Add(".bmp", new Tuple<string, bool>("BMP", true));
                extensionToGdalDriver.Add(".jpg", new Tuple<string, bool>("JPEG", true));
                extensionToGdalDriver.Add(".jp2", new Tuple<string, bool>("JPEG2000", true));
                extensionToGdalDriver.Add(".png", new Tuple<string, bool>("PNG", true));
                extensionToGdalDriver.Add(".xml", new Tuple<string, bool>("PDS4", true)); 
                extensionToGdalDriver.Add(".tif", new Tuple<string, bool>("GTiff", true));

                filePickerString = "Portable GrayMap (*.pgm)|*.pgm|" +                     
                    "JPEG (*.jpg)|*.jpg|" +
                    "JPEG2000 (*.jp2)|*.jp2|" +
                    "PNG (*.png)|*.png|" +
                    "GeoTIFF (*.tif)|*.tif|" +
                    "PDS4 (*.xml)|*.xml|" +
                    "Bitmap (*.bmp)|*.bmp|" +
                    "All files (*.*)|*.*";

                // Native to gdal type conversion
                systemTypeToGdalType = new Dictionary<Type, DataType>();
                systemTypeToGdalType.Add(typeof(byte), DataType.GDT_Byte);
                systemTypeToGdalType.Add(typeof(float), DataType.GDT_Float32);
                systemTypeToGdalType.Add(typeof(double), DataType.GDT_Float64);
                systemTypeToGdalType.Add(typeof(short), DataType.GDT_Int16);
                systemTypeToGdalType.Add(typeof(int), DataType.GDT_Int32);
                systemTypeToGdalType.Add(typeof(ushort), DataType.GDT_UInt16);
                systemTypeToGdalType.Add(typeof(uint), DataType.GDT_UInt32);

                gdalTypeToSystemType = new Dictionary<DataType, Type>();
                gdalTypeToSystemType.Add(DataType.GDT_Byte, typeof(byte));
                gdalTypeToSystemType.Add(DataType.GDT_Float32, typeof(float));
                gdalTypeToSystemType.Add(DataType.GDT_Float64, typeof(double));
                gdalTypeToSystemType.Add(DataType.GDT_Int16, typeof(short));
                gdalTypeToSystemType.Add(DataType.GDT_Int32, typeof(int));
                gdalTypeToSystemType.Add(DataType.GDT_UInt16, typeof(ushort));
                gdalTypeToSystemType.Add(DataType.GDT_UInt32, typeof(uint));
            }
        }

        static public string GetFilePickerFilter()
        {
            return filePickerString;
        }

        static public bool LoadMetadata(string path, out int widthPixels, out int heightPixels, out int bands, out Type[] bandDataType)
        {
            bool result = false;
            lock (gdalLockObj)
            {
                using (Dataset dataset = Gdal.Open(path, Access.GA_ReadOnly))
                {
                    widthPixels = dataset.RasterXSize;
                    heightPixels = dataset.RasterYSize;
                    bands = dataset.RasterCount;
                    bandDataType = new Type[bands];

                    for (int idx = 0; idx < bands; idx++)
                    {
                        using (Band band = dataset.GetRasterBand(idx + 1))
                        {
                            if (!gdalTypeToSystemType.ContainsKey(band.DataType))
                                throw new NotImplementedException(string.Format("data type {0} not supported yet", band.DataType));

                            bandDataType[idx] = gdalTypeToSystemType[band.DataType];
                            result |= true;
                        }
                    }
                }
            }

            return result;
        }

        static public bool Save(Image image, string path, string[] writeOptions)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string fileExt = Path.GetExtension(path).ToLower();
            if (!extensionToGdalDriver.ContainsKey(fileExt))
            {
                throw new NotImplementedException("Unsupported file extension: " + fileExt);
            }

            // Get the gdal driver settings for this extension
            Tuple<string, bool> driverSettings = extensionToGdalDriver[fileExt];

            // Some file types don't support Create so we need to use CreateCopy instead
            // To do this we will first write the rasters to memory using the MEM driver
            string driverName = driverSettings.Item2 ? "MEM" : driverSettings.Item1;
            string[] driverOptions = driverSettings.Item2 ? null : writeOptions;
            Driver driver = Gdal.GetDriverByName(driverName);

            if (image.Bands != 1 && image.Bands != 3)
                throw new NotImplementedException("only support saving 1 or 3 channel images");

            lock (gdalLockObj)
            {
                using (Dataset dataset = driver.Create(path, image.Width, image.Height, image.Bands,
                                                       systemTypeToGdalType[typeof(byte)], driverOptions))
                {
                    for (int b = 0; b < image.Bands; b++)
                    {
                        using (Band band = dataset.GetRasterBand(b + 1))
                        {
                            band.WriteRaster(0, 0, image.Width, image.Height, image.DataByBand[b], image.Width, image.Height, 0, 0);
                            band.FlushCache();
                        }
                    }


                    // If we wrote this raster in memory first
                    if (driverSettings.Item2)
                    {
                        Driver actualDriver = Gdal.GetDriverByName(driverSettings.Item1);
                        using (Dataset actualDataset = actualDriver.CreateCopy(path, dataset, 1, driverOptions, null, null))
                        {
                        }
                    }

                    dataset.FlushCache();
                }
            }

            return true;
        }

        static public Image Load(string path, int firstCol, int firstRow, int width, int height)
        {
            Image image = null;
            lock (gdalLockObj)
            {
                using (Dataset dataset = Gdal.Open(path, Access.GA_ReadOnly))
                {
                    if (dataset.RasterCount != 1 && dataset.RasterCount != 3)
                    {
                        throw new NotImplementedException(string.Format("Only single or 3 channel images supported currently, image has {0}", dataset.RasterCount));
                    }

                    image = new Image(width, height, dataset.RasterCount);
                    for (int idxBand = 0; idxBand < dataset.RasterCount; idxBand++)
                    {
                        using (Band band = dataset.GetRasterBand(idxBand + 1))
                        {
                            if (band.DataType != DataType.GDT_Byte)
                            {
                                throw new NotImplementedException("Only single byte image depths supported currently");
                            }

                            int stride = width; //if not byte type, multiply by pixel data size
                            band.ReadRaster(firstCol, firstRow, width, height, image.DataByBand[idxBand], width, height, 1, stride);

                        }
                    }
                }
            }
            return image;
        }
    }
}
