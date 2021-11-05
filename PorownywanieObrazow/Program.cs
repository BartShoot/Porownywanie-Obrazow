using PorownywanieObrazow;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
//TODO do zrobienia nagłówek i main
//TODO zrobic jako dll
//email: dobija.bartosz@gmail.com
//github: https://github.com/BartShoot/Porownywanie-Obrazow
Bitmap testImage;
ImageProcessor processor = new ImageProcessor(testImage = new Bitmap("C:/zdj/krajobraz.jpg"));
ImageProcessor processor2 = new ImageProcessor(testImage = new Bitmap("C:/zdj/krajobraz2.jpg"));

processor.MakeHistogram();
for (int i = 1; i < 5; i++)
{
    processor.HistogramCompare(processor2, i);
}
processor2.HistogramCompare(processor, 1);