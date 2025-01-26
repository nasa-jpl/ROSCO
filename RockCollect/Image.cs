using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockCollect
{
    public class Image
    {
        public int Width;
        public int Height;
        public int Bands;
        public byte[][] DataByBand;

        public Image(int width, int height, int bands)
        {
            Width = width;
            Height = height;
            Bands = bands;
            DataByBand = new byte[bands][];
            for(int idx=0; idx < bands; idx++)
            {
                DataByBand[idx] = new byte[width * height];
            }
        }

        public Image CreateFromBand(int band)
        {
            if (band >= Bands)
                throw new NotImplementedException("invalid band");

            Image monoImage = new Image(Width, Height, 1);
            for (int idxPixel = 0; idxPixel < Width * Height; idxPixel++)
            {
                monoImage.DataByBand[0][idxPixel] = DataByBand[band][idxPixel];             
            }
            return monoImage;
        }
        public Bitmap ToBitmap()
        {
            if (DataByBand.Count() == 1)
                return SingleChannelDataToBmp(DataByBand[0], Width, Height);
            else if (DataByBand.Count() == 3)
                return MultiChannelDataToBmp(DataByBand, Width, Height, Bands);
            else
                throw new NotImplementedException("only single band images supported currently");
        }

        private static Bitmap MultiChannelDataToBmp(byte[][] imageData, int cols, int rows, int bands)
        {
            if (bands != 3)
                throw new NotImplementedException("only support 3 channel data");

            Bitmap bmp = new Bitmap(cols, rows, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int numDstBytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] expandedImageData = new byte[numDstBytes];
            for (int idxRow = 0; idxRow < rows; idxRow++)
            {
                for (int idxCol = 0; idxCol < cols; idxCol++)
                {
                    int srcOffset = idxRow * cols + idxCol;
                    int curDstChannel = idxRow * Math.Abs(bmpData.Stride) + idxCol * bands;
                    expandedImageData[curDstChannel] = imageData[2][srcOffset];
                    expandedImageData[curDstChannel + 1] = imageData[1][srcOffset];
                    expandedImageData[curDstChannel + 2] = imageData[0][srcOffset];
                }
            }

            // Get the address of the first line and copy
            IntPtr ptr = bmpData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(expandedImageData, 0, ptr, numDstBytes);

            bmp.UnlockBits(bmpData);

            return bmp;

        }
        private static Bitmap SingleChannelDataToBmp(byte[] imageData, int cols, int rows)
        {
            Bitmap bmp = new Bitmap(cols, rows, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int numDstBytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] expandedImageData = new byte[numDstBytes];
            for (int idxRow = 0; idxRow < rows; idxRow++)
            {
                for (int idxCol = 0; idxCol < cols; idxCol++)
                {
                    int curDstChannel = idxRow * Math.Abs(bmpData.Stride) + idxCol * 3;
                    expandedImageData[curDstChannel] = imageData[idxRow * cols + idxCol];
                    expandedImageData[curDstChannel + 1] = imageData[idxRow * cols + idxCol];
                    expandedImageData[curDstChannel + 2] = imageData[idxRow * cols + idxCol];
                }
            }

            // Get the address of the first line and copy
            IntPtr ptr = bmpData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(expandedImageData, 0, ptr, numDstBytes);

            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}
