using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOperations
{
    abstract class IO_Operations
    {
        public abstract (byte[][] R, byte[][] G, byte[][] B) ReadImage();
    }

    class IO_bmp : IO_Operations
    {
        Bitmap image;
        int Width;
        int Height;
        public byte[][] R;
        public byte[][] G;
        public byte[][] B;
        public IO_bmp(Bitmap image)
        {
            this.image = image;
        }
        public override (byte[][] R, byte[][] G, byte[][] B) ReadImage()
        {
            Width = image.Width;
            Height = image.Height;

            R = new byte[Width][];
            G = new byte[Width][];
            B = new byte[Width][];

            for (int i = 0; i < Width; i++)
            {
                R[i] = new byte[Height];
                G[i] = new byte[Height];
                B[i] = new byte[Height];
            }
            unsafe
            {
                BitmapData bitmapData = image.LockBits
                    (
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadWrite,
                    image.PixelFormat
                    );
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int currentWidthInPixels = 0;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        currentWidthInPixels = x / bytesPerPixel;
                        R[currentWidthInPixels][y] = currentLine[x];
                        G[currentWidthInPixels][y] = currentLine[x + 1];
                        B[currentWidthInPixels][y] = currentLine[x + 2];
                    }
                }
                image.UnlockBits(bitmapData);
            }
            return (R, G, B);
        }
    }
}
