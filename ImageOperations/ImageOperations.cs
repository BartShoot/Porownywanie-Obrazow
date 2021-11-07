using System.Drawing;
using System.Drawing.Imaging;

namespace ImageOperations
{
    public class ImageContainer
    {
        Bitmap imageToLoad;
        int width;
        int height;
        int bitDepth;
        int amountOfValues;
        int histogramMaxValue;
        int[,] r;
        int[,] g;
        int[,] b;
        int[][] histogramRGB = new int[3][];
        int[][] histogramHSV = new int[3][];
        double[] histogramNormalized;
        double averageHistogramValue;
        public ImageContainer(Bitmap imageToLoad)
        {
            this.imageToLoad = imageToLoad;
            Width = imageToLoad.Width;
            Height = imageToLoad.Height;

            R = new int[Width, Height];
            G = new int[Width, Height];
            B = new int[Width, Height];

            bitDepth = 8;
            AmountOfValues = (int)Math.Pow(2, bitDepth);
            HistogramMaxValue = 0;

            HistogramNormalized=new double[AmountOfValues];
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
                        R[currentWidthInPixels, y] = currentLine[x];
                        G[currentWidthInPixels, y] = currentLine[x + 1];
                        B[currentWidthInPixels, y] = currentLine[x + 2];
                    }
                }
                imageToLoad.UnlockBits(bitmapData);
            }
        }

        public int[][] HistogramRGB { get => histogramRGB; set => histogramRGB = value; }
        public int[][] HistogramHSV { get => histogramHSV; set => histogramHSV = value; }
        public int[,] R { get => r; set => r = value; }
        public int[,] G { get => g; set => g = value; }
        public int[,] B { get => b; set => b = value; }
        public double[] HistogramNormalized { get => histogramNormalized; set => histogramNormalized = value; }
        public int AmountOfValues { get => amountOfValues; set => amountOfValues = value; }
        public double AverageHistogramValue { get => averageHistogramValue; set => averageHistogramValue = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public int HistogramMaxValue { get => histogramMaxValue; set => histogramMaxValue = value; }
    }
    public class Operations
    {
        ///<summary>
        ///1. Correlation ( CV_COMP_CORREL )
        ///<br> 2. Chi-Square ( CV_COMP_CHISQR )</br>
        ///<br> 3. Intersection ( method=CV_COMP_INTERSECT )</br>
        ///<br> 4. Bhattacharyya distance ( CV_COMP_BHATTACHARYYA )</br>
        ///</summary>
        public double CompareHistogram(ImageContainer image1, ImageContainer image2, int method)
        {
            return this.CompareHistogram(image1, image2, method,
            0, 0, image1.Width, image1.Height);
        }

        ///<summary>
        ///1. Correlation ( CV_COMP_CORREL )
        ///<br> 2. Chi-Square ( CV_COMP_CHISQR )</br>
        ///<br> 3. Intersection ( method=CV_COMP_INTERSECT )</br>
        ///<br> 4. Bhattacharyya distance ( CV_COMP_BHATTACHARYYA )</br>
        ///</summary>
        public double CompareHistogram(ImageContainer image1, ImageContainer image2, int method, 
            int startX, int startY, int endX, int endY)
        {
            this.CalculateHistogram(image1, startX, startY, endX, endY);
            this.CalculateHistogram(image2, startX, startY, endX, endY);

            double distanceMetric = 0;

            switch (method)
            {
                case 1:
                    distanceMetric = this.CompareCorrelation(image1, image2);
                    break;
                case 2:
                    distanceMetric = this.CompareChiSquare(image1, image2);
                    break;
                case 3:
                    distanceMetric = this.CompareIntersection(image1, image2);
                    break;
                case 4:
                    distanceMetric = this.CompareBhattacharyya(image1, image2);
                    break;
                default:
                    distanceMetric = 100000;
                    break;
            }

            return distanceMetric;
        }

        private double CompareBhattacharyya(ImageContainer image1, ImageContainer image2)
        {
            this.HistogramAverage(image1);
            this.HistogramAverage(image2);
            double sum = 0;
            for (int i = 0; i < image1.AmountOfValues; i++)
            {
                sum += Math.Sqrt(image1.HistogramNormalized[i] * image2.HistogramNormalized[i]);
            }
            double pom1 = Math.Sqrt(image1.AverageHistogramValue * image2.AverageHistogramValue * image1.AmountOfValues * image1.AmountOfValues);
            return Math.Sqrt(1 - (1 / pom1) * sum);
        }

        private double CompareIntersection(ImageContainer image1, ImageContainer image2)
        {
            double sum = 0;
            for (int i = 0; i < image1.AmountOfValues; i++)
            {
                if (image1.HistogramNormalized[i] > image2.HistogramNormalized[i])
                {
                    sum += image2.HistogramNormalized[i];
                }
                else
                {
                    sum += image1.HistogramNormalized[i];
                }
            }
            return sum;
        }

        private double CompareChiSquare(ImageContainer image1, ImageContainer image2)
        {
            double sum = 0;
            for (int i = 0; i < image1.AmountOfValues; i++)
            {
                sum += Math.Pow(image1.HistogramNormalized[i] - image2.HistogramNormalized[i], 2) / image1.HistogramNormalized[i];
            }
            return sum;
        }

        private double CompareCorrelation(ImageContainer image1, ImageContainer image2)
        {
            this.HistogramAverage(image1);
            this.HistogramAverage(image2);
            double pom1 = 0, pom2 = 0, sum1 = 0, sum2 = 0, sum3 = 0;
            for (int i = 0; i < image1.AmountOfValues; i++)
            {
                pom1 = image1.HistogramNormalized[i] - image1.AverageHistogramValue;
                pom2 = image2.HistogramNormalized[i] - image2.AverageHistogramValue;
                sum1 += pom1 * pom2;
                sum2 += Math.Pow(pom1, 2);
                sum3 += Math.Pow(pom2, 2);
            }
            return sum1 / Math.Sqrt(sum2 * sum3);
        }

        private void HistogramAverage(ImageContainer image)
        {
            double valueSum = 0;
            for (int i = 0; i < image.AmountOfValues; i++)
            {
                valueSum += image.HistogramNormalized[i];
            }
            image.AverageHistogramValue = valueSum / image.AmountOfValues;
        }

        private void CalculateHistogram(ImageContainer image)
        {
            CalculateHistogram(image, 0, 0, image.Width, image.Height);
        }

        private void CalculateHistogram(ImageContainer image, int startX, int startY, int endX, int endY)
        {
            for (int i = startY; i < endX-1; i++)
            {
                for (int j = startX; j < endY-1; j++)
                {
                    int rValue = image.R[i, j],
                        gValue = image.G[i, j],
                        bValue = image.B[i, j];

                    image.HistogramRGB[0][rValue]++;
                    image.HistogramRGB[1][gValue]++;
                    image.HistogramRGB[2][bValue]++;

                    //TODO: HistogramHSV
                    (double hue, double saturation, double value) = Rgb2Hsv(rValue, gValue, bValue);
                    image.HistogramHSV[0][(int)hue]++;
                    image.HistogramHSV[0][(int)saturation]++;
                    image.HistogramHSV[0][(int)value]++;
                }
            }
            for (int i = 0; i < image.AmountOfValues; i++)
            {
                int sum =   image.HistogramRGB[0][i] +
                            image.HistogramRGB[1][i] +
                            image.HistogramRGB[2][i];
                if (image.HistogramMaxValue < sum)
                    image.HistogramMaxValue = sum;
            }
            this.NormalizeHistogram(image, startX, startY, endX, endY);
        }

        private (double hue, double saturation, double value)
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

        private void NormalizeHistogram(ImageContainer image, int startX, int startY, int endX, int endY)
        {
            int imageSizeTimesColors = (endX - startX)*(endY-startY)*3;
            for (int i = 0;i<image.AmountOfValues; i++)
            {
                image.HistogramNormalized[i] = (double)(image.HistogramRGB[0][i] + image.HistogramRGB[1][i] + image.HistogramRGB[2][i]) / (double)imageSizeTimesColors;
            }
        }

        public void DrawHistogramPlot(ImageContainer image, string fileName)
        {
            CalculateHistogram(image);
            int histogramImageWidth = 2000, histogramImageHeight = 1000;
            Bitmap histogramImage = new Bitmap(histogramImageWidth, histogramImageHeight);
            Graphics graphics = Graphics.FromImage(histogramImage);
            Color backgroundColor = Color.White;
            graphics.Clear(backgroundColor);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            double yScale = (double)(histogramImageHeight) / image.HistogramMaxValue,
                    xScale = (double)(histogramImageWidth) / image.AmountOfValues;
            Font font = new Font(FontFamily.GenericSerif, 12);
            Brush brush = new SolidBrush(Color.Black);
            SizeF size = graphics.MeasureString(Convert.ToString(image.HistogramMaxValue), font);
            
            Pen redPen = new Pen(Color.Red, 1);
            Pen greenPen = new Pen(Color.Green, 1);
            Pen bluePen = new Pen(Color.Blue, 1);
            Pen blackPen = new Pen(Color.Black, 1);

            Point point1 = new Point(0, 0);
            Point point2 = new Point(0, 0);
            int x1, x2, y1, y2, xOffset=Convert.ToInt32(size.Width);
            for (int i = 0; i < image.AmountOfValues - 1; i++)
            {
                x1 = i * (int)xScale+ xOffset;
                x2 = (i + 1) * (int)xScale+ xOffset;

                y1 = histogramImageHeight - ((int)(image.HistogramRGB[0][i] * yScale));
                y2 = histogramImageHeight - ((int)(image.HistogramRGB[0][i + 1] * yScale));
                point1 = new Point(x1, y1);
                point2 = new Point(x2, y2);
                graphics.DrawLine(redPen, point1, point2);

                y1 = histogramImageHeight - ((int)(image.HistogramRGB[1][i] * yScale));
                y2 = histogramImageHeight - ((int)(image.HistogramRGB[1][i + 1] * yScale));
                point1 = new Point(x1, y1);
                point2 = new Point(x2, y2);
                graphics.DrawLine(greenPen, point1, point2);

                y1 = histogramImageHeight - ((int)(image.HistogramRGB[2][i] * yScale));
                y2 = histogramImageHeight - ((int)(image.HistogramRGB[2][i + 1] * yScale));
                point1 = new Point(x1, y1);
                point2 = new Point(x2, y2);
                graphics.DrawLine(bluePen, point1, point2);

                int histogramValue = (int)( image.HistogramRGB[0][i] + 
                                            image.HistogramRGB[1][i] + 
                                            image.HistogramRGB[2][i]);

                int histogramValue2 = (int)(image.HistogramRGB[0][i + 1] + 
                                            image.HistogramRGB[1][i + 1] + 
                                            image.HistogramRGB[2][i + 1]);
                y1 = histogramImageHeight - ((int)(histogramValue * yScale));
                y2 = histogramImageHeight - ((int)(histogramValue2 * yScale));
                point1 = new Point(x1, y1);
                point2 = new Point(x2, y2);
                graphics.DrawLine(blackPen, point1, point2);
            }
            for (int i = 0; i < 5; i++)
            {
                if (i == 0) graphics.DrawString("0", font, brush, 0, histogramImageHeight - font.GetHeight());
                else
                {
                    graphics.DrawString(Convert.ToString((5-i)*image.HistogramMaxValue / 4), 
                        font, brush, 0, (i-1)*(histogramImageHeight/4));
                }
                
            }
            
            histogramImage.Save(fileName);
        }

        
    }
}