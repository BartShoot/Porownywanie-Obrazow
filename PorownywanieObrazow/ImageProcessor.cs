using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PorownywanieObrazow
{
    internal class ImageProcessor
    {
        System.Drawing.Bitmap imageToProcess;
        int bitDepth;
        int amountOfValues;
        int[][]histogram = new int[3][];
        int histogramMaxValue;
        Color backgroundColor = Color.White;

        public ImageProcessor(Bitmap imageToProcess)
        {
            this.ImageToProcess = imageToProcess;
            histogramMaxValue=0;
            bitDepth = 8;
            amountOfValues = (int)Math.Pow(2, bitDepth);
            for (int i = 0; i < histogram.Length; i++)
            {
                histogram[i] = new int[amountOfValues];
            }
        }

        internal void RandomColorNoise()
        {
            Random rnd = new Random();
            unsafe
            {
                BitmapData bitmapData = imageToProcess.LockBits(new Rectangle(0, 0, imageToProcess.Width, imageToProcess.Height), 
                    ImageLockMode.ReadWrite, imageToProcess.PixelFormat);

                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(imageToProcess.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int oldBlue = currentLine[x];
                        int oldGreen = currentLine[x + 1];
                        int oldRed = currentLine[x + 2];

                        oldRed = rnd.Next(256);
                        oldGreen = rnd.Next(256);
                        oldBlue = rnd.Next(256);

                        currentLine[x] = (byte)oldBlue;
                        currentLine[x + 1] = (byte)oldGreen;
                        currentLine[x + 2] = (byte)oldRed;
                    }
                });
                imageToProcess.UnlockBits(bitmapData);
            }
        }
        
        ///<summary>
        ///Calculate histogram and save to file
        ///</summary>
        public void MakeHistogram()
        {
            CalculateHistogram();
            DrawHistogramPlot();
        }

        ///<summary>
        ///Only calculate histogram for operations
        ///</summary>
        public void CalculateHistogram()
        {
            unsafe
            {
                BitmapData bitmapData = imageToProcess.LockBits(new Rectangle(0, 0, imageToProcess.Width, imageToProcess.Height), 
                                        ImageLockMode.ReadWrite, imageToProcess.PixelFormat);

                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(imageToProcess.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                int[,] sumBlue = new int[heightInPixels, amountOfValues];
                int[,] sumGreen = new int[heightInPixels, amountOfValues];
                int[,] sumRed = new int[heightInPixels, amountOfValues];

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int oldBlue = currentLine[x];
                        int oldGreen = currentLine[x + 1];
                        int oldRed = currentLine[x + 2];

                        sumBlue[y, oldBlue]++;
                        sumGreen[y, oldGreen]++;
                        sumRed[y, oldRed]++;

                        currentLine[x] = (byte)oldBlue;
                        currentLine[x + 1] = (byte)oldGreen;
                        currentLine[x + 2] = (byte)oldRed;
                    }
                });
                imageToProcess.UnlockBits(bitmapData);

                int[] finalBlue = new int[amountOfValues];
                int[] finalGreen = new int[amountOfValues];
                int[] finalRed = new int[amountOfValues];
                for (int i = 0; i < heightInPixels; i++)
                {
                    for (int j = 0; j < amountOfValues; j++)
                    {
                        finalBlue[j] += sumBlue[i, j];
                        finalGreen[j] += sumGreen[i, j];
                        finalRed[j] += sumRed[i, j];
                    }
                }
                for (int i = 0; i < amountOfValues; i++)
                {
                    histogram[0][i] = finalRed[i];
                    histogram[1][i] = finalGreen[i];
                    histogram[2][i] = finalBlue[i];

                    if (histogram[0][i] + histogram[1][i] + histogram[2][i] > histogramMaxValue)
                    {
                        histogramMaxValue = (int)(histogram[0][i] + histogram[1][i] + histogram[2][i]);
                    }
                }
                

                
            }
        }

        public void HistogramCompare(ImageProcessor imageToCompare)
        {
            int histogramDifference = 0;
            for (int i = 0; i < amountOfValues; i++)
            {
                histogramDifference += Math.Abs(histogram[0][i] - imageToCompare.histogram[0][i]);
                histogramDifference += Math.Abs(histogram[1][i] - imageToCompare.histogram[1][i]);
                histogramDifference += Math.Abs(histogram[2][i] - imageToCompare.histogram[2][i]);
            }
            Console.WriteLine($"Roznica histogramow to: {histogramDifference}");
        }

        public void HistogramCompare(ImageProcessor imageToCompare, int methodSelect)
        {
            //https://docs.opencv.org/3.4.15/d8/dc8/tutorial_histogram_comparison.html
            //na wszytkie 4 sposoby
        }

        private void DrawHistogramPlot()
        {
            int histogramImageWidth = 2000, histogramImageHeight = 1000;
            Bitmap histogramImage = new Bitmap(histogramImageWidth, histogramImageHeight);
            Graphics graphics = Graphics.FromImage(histogramImage);
            graphics.Clear(backgroundColor);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            double  yScale = (double)(histogramImageHeight) / histogramMaxValue, 
                    xScale = (double)(histogramImageWidth) / amountOfValues;
            Console.WriteLine($"Skalowanie wysokosci: {yScale}");
            Console.WriteLine($"Skalowanie szerokosci: {xScale}");

            Pen redPen = new Pen(Color.Red, 1);
            Pen greenPen = new Pen(Color.Green, 1);
            Pen bluePen = new Pen(Color.Blue, 1);
            Pen blackPen = new Pen(Color.Black, 1);

            Point point1 = new Point(0, 0);
            Point point2 = new Point(0, 0);

            for (int i = 0; i < amountOfValues - 1; i++)
            {
                point1 = new Point( i * (int)xScale,
                                    histogramImageHeight - ((int)(histogram[0][i] * yScale)));
                point2 = new Point((i + 1) * (int)xScale, 
                                    histogramImageHeight - ((int)(histogram[0][i + 1] * yScale)));
                graphics.DrawLine(redPen, point1, point2);
            
                point1 = new Point( i * (int)xScale,
                                    histogramImageHeight - ((int)(histogram[1][i] * yScale)));
                point2 = new Point((i + 1) * (int)xScale,
                                    histogramImageHeight - ((int)(histogram[1][i + 1] * yScale)));
                graphics.DrawLine(greenPen, point1, point2);
            
                point1 = new Point( i * (int)xScale,
                                    histogramImageHeight - ((int)(histogram[2][i] * yScale)));
                point2 = new Point((i + 1) * (int)xScale,
                                    histogramImageHeight - ((int)(histogram[2][i + 1] * yScale)));
                graphics.DrawLine(bluePen, point1, point2);
            
                int histogramValue = (int)(histogram[0][i] + histogram[1][i] + histogram[2][i]);
                int histogramValue2 = (int)(histogram[0][i + 1] + histogram[1][i + 1] + histogram[2][i + 1]);
                point1 = new Point( i * (int)xScale,
                                    histogramImageHeight - ((int)(histogramValue * yScale)));
                point2 = new Point((i + 1) * (int)xScale,
                                    histogramImageHeight - ((int)(histogramValue2 * yScale)));
                graphics.DrawLine(blackPen, point1, point2);
            }
            histogramImage.Save("histogram.png");
        }

        public Bitmap ImageToProcess { get => imageToProcess; set => imageToProcess = value; }
    }
}
