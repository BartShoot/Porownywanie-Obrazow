using PorownywanieObrazow;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

//email: dobija.bartosz@gmail.com
Bitmap testImage;
ImageProcessor processor = new ImageProcessor(testImage = new Bitmap("D:/noise/zupa.jpg"));
ImageProcessor processor2 = new ImageProcessor(testImage = new Bitmap("D:/noise/zupa2.jpg"));

processor.MakeHistogram();
for (int i = 1; i < 5; i++)
{
    processor.HistogramCompare(processor2, i);
}
processor2.HistogramCompare(processor, 1);