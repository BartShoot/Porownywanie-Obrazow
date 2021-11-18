using ImageOperations;
using System;
using System.Diagnostics;
using System.Drawing;

//email: dobija.bartosz@gmail.com
//github: https://github.com/BartShoot/Porownywanie-Obrazow
class Porownywanie_Obrazow
{
    static void Main()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Bitmap testImage = new Bitmap("C:/zdj/krajobraz.jpg");
        Bitmap testImage2 = new Bitmap("C:/zdj/krajobraz2.jpg");
        ImageContainer obraz1 = new ImageContainer(testImage);
        //ImageContainer obrazAccu2 = new ImageContainer(testImage, 2);
        //ImageContainer obrazAccu4 = new ImageContainer(testImage, 4);
        ImageContainer obraz2 = new ImageContainer(testImage2);
        Operations op = new Operations();
        sw.Stop();
        Console.WriteLine($"Czas generacji {sw.Elapsed}");
        for (int i = 1; i < 5; i++)
        {
            sw.Restart();
            //double roznica = op.CompareHistogram(obraz1, obraz2, i);
            double roznica = op.CompareHistogram(obraz1, obraz2, i, 0, 0, 500, 500);
            sw.Stop();
            Console.WriteLine((decimal)roznica);
            Console.WriteLine($"Czas liczenia {sw.Elapsed}");
        }
        //Console.WriteLine("Accuracy 1 i accuracy 2");
        //Console.WriteLine(op.CompareHistogram(obraz1, obrazAccu2, 1, 0, 0, 500, 500));
        //Console.WriteLine("Accuracy 2 i accuracy 4");
        //Console.WriteLine(op.CompareHistogram(obrazAccu2, obrazAccu4, 1, 0, 0, 500, 500));
        //Console.WriteLine("Accuracy 1 i accuracy 4");
        //Console.WriteLine(op.CompareHistogram(obraz1, obrazAccu4, 1, 0, 0, 500, 500));

        //op.DrawHistogramPlot(obraz1, "C:/zdj/histogram1.jpg");
        //op.DrawHistogramPlot(obrazAccu2, "C:/zdj/histogram2.jpg");
        //op.DrawHistogramPlot(obrazAccu4, "C:/zdj/histogram3.jpg");
    }
}
