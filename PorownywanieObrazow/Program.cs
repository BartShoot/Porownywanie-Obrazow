using PorownywanieObrazow;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

//email: dobija.bartosz@gmail.com
Bitmap testImage;
ImageProcessor processor = new ImageProcessor(testImage = new Bitmap("C:/zdj/krajobraz.jpg"));
ImageProcessor processor2 = new ImageProcessor(testImage = new Bitmap("C:/zdj/krajobraz2.jpg"));
processor.CalculateHistogram();
processor2.CalculateHistogram();
processor.HistogramCompare(processor2);
processor.MakeHistogram();