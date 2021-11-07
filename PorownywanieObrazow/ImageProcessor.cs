/*
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
        bool isHistogramCalculated;
        int bitDepth;
        int amountOfValues;
        int[][] histogram = new int[3][];
        double[] histogramNormalized;
        int histogramMaxValue;
        double averageHistogramValue;
        Color backgroundColor = Color.White;

        public ImageProcessor(Bitmap imageToProcess)
        {
            this.ImageToProcess = imageToProcess;
            histogramMaxValue=0;
            isHistogramCalculated=false;
            bitDepth = 8;
            amountOfValues = (int)Math.Pow(2, bitDepth);
            //dodac wczytanie z pliku do R[,],G[,] itd

            for (int i = 0; i < histogram.Length; i++)
            {
                histogram[i] = new int[amountOfValues];
            }
            histogramNormalized = new double[amountOfValues];
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
                isHistogramCalculated = false;
            }
        }
        
        ///<summary>
        ///Calculate histogram and save to file
        ///</summary>
        public void MakeHistogram()
        {
            if(!isHistogramCalculated)
                CalculateHistogram();
            DrawHistogramPlot("test.png");
        }

        ///<summary>
        ///Only calculate histogram for operations
        ///</summary>
        private void CalculateHistogram()
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
                isHistogramCalculated = true;
            }
        }

        public void HistogramCompare(ImageProcessor imageToCompare)
        {
            if (!isHistogramCalculated) CalculateHistogram();
            int histogramDifference = 0;
            for (int i = 0; i < amountOfValues; i++)
            {
                histogramDifference += Math.Abs(histogram[0][i] - imageToCompare.histogram[0][i]);
                histogramDifference += Math.Abs(histogram[1][i] - imageToCompare.histogram[1][i]);
                histogramDifference += Math.Abs(histogram[2][i] - imageToCompare.histogram[2][i]);
            }
            Console.WriteLine($"Roznica histogramow to: {histogramDifference}");
        }

        ///<summary>
        ///1. Correlation ( CV_COMP_CORREL )
        ///<br> 2. Chi-Square ( CV_COMP_CHISQR )</br>
        ///<br> 3. Intersection ( method=CV_COMP_INTERSECT )</br>
        ///<br> 4. Bhattacharyya distance ( CV_COMP_BHATTACHARYYA )</br>
        ///</summary>
        public void HistogramCompare(ImageProcessor imageToCompare, int methodSelect)
        {
            //https://docs.opencv.org/3.4.15/d8/dc8/tutorial_histogram_comparison.html
            //H1 = this.histogramNormalized, H2 = imageToCompare.histogramNormalized, N = amountOfValues
            double distanceMetric = 0;
            if (!isHistogramCalculated)
            {
                CalculateHistogram();
            }
            if (!imageToCompare.isHistogramCalculated)
            {
                imageToCompare.CalculateHistogram();
            }
            NormalizeHistogram();
            imageToCompare.NormalizeHistogram();

            switch (methodSelect)
            {
                case 1:
                    HistogramAverage();
                    imageToCompare.HistogramAverage();
                    double pom1 = 0, pom2 = 0, sum1=0, sum2=0, sum3=0;
                    for (int i = 0; i < amountOfValues; i++)
                    {
                        pom1 = histogramNormalized[i] - averageHistogramValue;
                        pom2 = imageToCompare.histogramNormalized[i] - imageToCompare.averageHistogramValue;
                        sum1 += pom1 * pom2;
                        sum2 += Math.Pow(pom1, 2);
                        sum3 += Math.Pow(pom2, 2);
                    }
                    distanceMetric = sum1 / Math.Sqrt(sum2 * sum3);
                    Console.WriteLine("Różnica histogramów wersja 1 to: " + distanceMetric);
                    break;
                case 2:
                    double sum = 0;
                    for (int i = 0; i < amountOfValues; i++)
                    {
                        sum += Math.Pow(histogramNormalized[i] - imageToCompare.histogramNormalized[i], 2) / histogramNormalized[i];
                    }
                    distanceMetric = sum;
                    Console.WriteLine("Różnica histogramów wersja 2 to: " + distanceMetric);
                    break;
                case 3:
                    sum = 0;
                    for (int i = 0; i < amountOfValues; i++)
                    {
                        sum += histogramNormalized[i] > imageToCompare.histogramNormalized[i] ? imageToCompare.histogramNormalized[i] : histogramNormalized[i];
                    }
                    distanceMetric = sum;
                    Console.WriteLine("Różnica histogramów wersja 3 to: " + distanceMetric);
                    break;
                case 4:
                    HistogramAverage();
                    imageToCompare.HistogramAverage();
                    sum = 0;
                    for (int i = 0; i < amountOfValues; i++)
                    {
                        sum += Math.Sqrt(histogramNormalized[i] * imageToCompare.histogramNormalized[i]);
                    }
                    pom1 = Math.Sqrt(averageHistogramValue * imageToCompare.averageHistogramValue * amountOfValues * amountOfValues);
                    distanceMetric = Math.Sqrt(1 - (1 / pom1) * sum);
                    Console.WriteLine("Różnica histogramów wersja 4 to: " + distanceMetric);
                    break;
                default:
                    Console.WriteLine("Nie ma takiego numeru");
                    break;
            }
        }

        public void HistogramCompare(ImageProcessor imageToCompare, int methodSelect, Point start, Point end)
        {

        }

        public void HistogramCompare(ImageProcessor imageToCompare, int methodSelect, int startX, int startY, int endX, int endY)
        {
            Point start = new Point(startX, startY);
            Point end = new Point(endX, endY);
            HistogramCompare(imageToCompare, methodSelect, start, end);
        }

        private void NormalizeHistogram()
        {
            int imageSizeTimesColors = imageToProcess.Width*imageToProcess.Height*3;
            for (int j = 0; j < amountOfValues; j++)
            {
                histogramNormalized[j] = (double)(histogram[0][j]+ histogram[1][j]+ histogram[2][j]) / (double)(imageSizeTimesColors);
            }
        }

        public void HistogramRgbToHsv()
        {
            
        }

        public (double hue, double saturation, double value) 
            Rgb2Hsv(double r, double g, double b)
            {
                //based on the source from: https://www.geeksforgeeks.org/program-change-rgb-color-model-hsv-color-model/
                // R, G, B values are divided by 255 
                // to change the range from 0..255 to 0..1 
                r = r / 255.0;
                g = g / 255.0;
                b = b / 255.0;

                // h, s, v = hue, saturation, value 
                double cmax = Math.Max(r, Math.Max(g, b)); // maximum of r, g, b 
                double cmin = Math.Min(r, Math.Min(g, b)); // minimum of r, g, b 
                double diff = cmax - cmin; // diff of cmax and cmin. 
                double h = -1, s = -1;

                // if cmax and cmax are equal then h = 0 
                if (cmax == cmin)
                    h = 0;

                // if cmax equal r then compute h 
                else if (cmax == r)
                    h = (60 * ((g - b) / diff) + 360) % 360;

                // if cmax equal g then compute h 
                else if (cmax == g)
                    h = (60 * ((b - r) / diff) + 120) % 360;

                // if cmax equal b then compute h 
                else if (cmax == b)
                    h = (60 * ((r - g) / diff) + 240) % 360;

                // if cmax equal zero 
                if (cmax == 0)
                    s = 0;
                else
                    s = (diff / cmax) * 100;

                // compute v 
                double v = cmax * 100;
                return (h, s, v);

            }

        private void HistogramAverage()
        {
            double valueSum=0;
            for (int i = 0; i < amountOfValues; i++)
            {
                valueSum += histogramNormalized[i];
            }
            averageHistogramValue = valueSum / amountOfValues;
        }

        private void DrawHistogramPlot(string fileName)
        {
            if (!isHistogramCalculated) CalculateHistogram();
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
            histogramImage.Save(fileName);
        }

        public Bitmap ImageToProcess { get => imageToProcess; set => imageToProcess = value; }
    }
}
*/