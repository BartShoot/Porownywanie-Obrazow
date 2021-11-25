using ImageOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PorownywanieObrazow
{
    internal class HistogramData
    {
        public int[][] histogramRGB = new int[3][];
        public int[][] histogramHSV = new int[3][];
        public double[] histogramNormalized;
        public double averageHistogramValue;
        public bool isHistogramCalculated;
        public int histogramMaxValue;
        public int amountOfValues;
        HistogramPlotDrawing drawHistogram;
        public HistogramData(ImageContainer image, int amountOfValues, int accuracy)
        {
            this.amountOfValues = amountOfValues;
            isHistogramCalculated = false;
            histogramNormalized = new double[amountOfValues / accuracy];
            for (int i = 0; i < histogramRGB.Length; i++)
            {
                histogramRGB[i] = new int[amountOfValues / accuracy];
            }
            histogramHSV[0] = new int[360];
            histogramHSV[1] = new int[256];
            histogramHSV[2] = new int[256];
            CalculateHistogram(image, 0, 0,image.Width,image.Height);
        }
        public void CalculateHistogram(ImageContainer image, int startX, int startY, int endX, int endY)
        {
            if (image.IsHistogramCalculated)
            {
                return;
            }
            for (int i = startY; i < endX - 1; i++)
            {
                for (int j = startX; j < endY - 1; j++)
                {
                    int rValue = image.R[i][j],
                        gValue = image.G[i][j],
                        bValue = image.B[i][j];

                    image.HistogramRGB[0][rValue / image.Accuracy]++;
                    image.HistogramRGB[1][gValue / image.Accuracy]++;
                    image.HistogramRGB[2][bValue / image.Accuracy]++;

                    //TODO: HistogramHSV
                    //(double hue, double saturation, double value) = Rgb2Hsv(rValue, gValue, bValue);
                    //image.HistogramHSV[0][(int)hue]++;
                    //image.HistogramHSV[0][(int)saturation]++;
                    //image.HistogramHSV[0][(int)value]++;
                }
            }
            for (int i = 0; i < image.AmountOfValues; i++)
            {
                int sum = image.HistogramRGB[0][i] +
                            image.HistogramRGB[1][i] +
                            image.HistogramRGB[2][i];
                if (image.HistogramMaxValue < sum)
                    image.HistogramMaxValue = sum;
            }
            this.NormalizeHistogram(image, startX, startY, endX, endY);
            image.IsHistogramCalculated = true;
        }

        private void NormalizeHistogram(ImageContainer image, int startX, int startY, int endX, int endY)
        {
            int imageSizeTimesColors = (endX - startX) * (endY - startY) * 3;
            double test = 0;
            for (int i = 0; i < image.AmountOfValues; i++)
            {
                image.HistogramNormalized[i] = (double)(image.HistogramRGB[0][i] + image.HistogramRGB[1][i] + image.HistogramRGB[2][i]) / (double)imageSizeTimesColors;
                test += image.HistogramNormalized[i];
            }
        }

        public void DrawHistogramPlot(string fileName)
        {
            if (!isHistogramCalculated)
            {
                throw new InvalidOperationException("Histogram not calculated!");
            }
            drawHistogram = new HistogramPlotDrawing(this, fileName);
        }
    }
}
