using PorownywanieObrazow;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;


Bitmap testImage;
ImageProcessor processor = new ImageProcessor(testImage = new Bitmap("D:/noise/zupa.jpg"));
ImageProcessor processor2 = new ImageProcessor(testImage = new Bitmap("D:/noise/zupa.jpg"));
processor.CalculateHistogram();
processor2.CalculateHistogram();
processor.HistogramCompare(processor2);
processor.MakeHistogram();