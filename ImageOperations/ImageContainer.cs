using PorownywanieObrazow;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageOperations
{
    public class ImageContainer
    {
        Bitmap imageToLoad;
        HistogramData histogram;
        int width;
        int height;
        int bitDepth;
        int amountOfValues;
        int histogramMaxValue;
        int accuracy;
        public byte[][] R;
        public byte[][] G;
        public byte[][] B;
        int[][] histogramRGB = new int[3][];
        int[][] histogramHSV = new int[3][];
        double[] histogramNormalized;
        double averageHistogramValue;
        bool isHistogramCalculated;

        public ImageContainer(Bitmap imageToLoad) :
            this(imageToLoad, 1) { }

        public ImageContainer(Bitmap imageToLoad, int accuracy)
        {
            //klasa abstrakcyjna od odczytu i zapisu plików np IOOperations
            //odczyt zwracający dwuwymiarową tablicy byte
            //zapis zapisuje dane 
            //musi być opcja obsługi różnych formatów bez zamieniania istniejących klas (np ImageSharp) 
            this.imageToLoad = imageToLoad;
            Width = imageToLoad.Width;
            Height = imageToLoad.Height;

            R = new byte[Width][];
            G = new byte[Width][];
            B = new byte[Width][];

            for (int i = 0; i < Width; i++)
            {
                R[i] = new byte[Height];
                G[i] = new byte[Height];
                B[i] = new byte[Height];
            }

            bitDepth = 8;
            this.accuracy = accuracy;
            amountOfValues = (int)Math.Pow(2, bitDepth) / Accuracy;
            HistogramMaxValue = 0;
            IsHistogramCalculated = false;

            HistogramNormalized = new double[AmountOfValues];
            for (int i = 0; i < histogramRGB.Length; i++)
            {
                histogramRGB[i] = new int[AmountOfValues];
            }
            histogramHSV[0] = new int[360];
            histogramHSV[1] = new int[256];
            histogramHSV[2] = new int[256];

            unsafe
            {
                BitmapData bitmapData = imageToLoad.LockBits
                    (
                    new Rectangle(0, 0, imageToLoad.Width, imageToLoad.Height),
                    ImageLockMode.ReadWrite,
                    imageToLoad.PixelFormat
                    );
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(imageToLoad.PixelFormat) / 8;
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
                imageToLoad.UnlockBits(bitmapData);
            }
            Histogram = new HistogramData(this, amountOfValues, accuracy);
        }

        public int[][] HistogramRGB { get => histogramRGB; set => histogramRGB = value; }
        public int[][] HistogramHSV { get => histogramHSV; set => histogramHSV = value; }
        public double[] HistogramNormalized { get => histogramNormalized; set => histogramNormalized = value; }
        public int AmountOfValues { get => amountOfValues; set => amountOfValues = value; }
        public double AverageHistogramValue { get => averageHistogramValue; set => averageHistogramValue = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public int HistogramMaxValue { get => histogramMaxValue; set => histogramMaxValue = value; }
        public bool IsHistogramCalculated { get => isHistogramCalculated; set => isHistogramCalculated = value; }
        public int Accuracy { get => accuracy; set => accuracy = value; }
        internal HistogramData Histogram { get => histogram; set => histogram = value; }
    }
}