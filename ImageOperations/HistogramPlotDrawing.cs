using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOperations
{
    internal class HistogramPlotDrawing
    {
        ImageContainer image;
        string fileName;
        public HistogramPlotDrawing(ImageContainer image, string fileName)
        {
            this.image = image;
            this.fileName = fileName;
        }

        public void DrawHistogramPlot()
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
            Pen gridPen = new Pen(Color.FromArgb(40, Color.Black), 1);

            Point point1 = new Point(0, 0);
            Point point2 = new Point(0, 0);
            int x1, x2, y1, y2, xOffset = Convert.ToInt32(size.Width);
            for (int i = 0; i < image.AmountOfValues - 1; i++)
            {
                x1 = i * (int)xScale + xOffset;
                x2 = (i + 1) * (int)xScale + xOffset;

                if (i % 8 == 0)
                {
                    graphics.DrawLine(gridPen, x1, 0, x1, histogramImageHeight);
                    graphics.DrawString(Convert.ToString(i), font, brush, x1, 0);
                }
                if (i == image.AmountOfValues - 2)
                {
                    graphics.DrawLine(gridPen, x2, 0, x2, histogramImageHeight);
                    graphics.DrawString(Convert.ToString(i + 1), font, brush, x2, 0);
                }

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

                int histogramValue = (int)(image.HistogramRGB[0][i] +
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
                    graphics.DrawString(Convert.ToString((5 - i) * image.HistogramMaxValue / 4),
                        font, brush, 0, (i - 1) * (histogramImageHeight / 4));
                }

            }

            histogramImage.Save(fileName);
        }

        private void CalculateHistogram(ImageContainer image)
        {
            throw new NotImplementedException();
        }
    }
}
