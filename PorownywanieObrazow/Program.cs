using ImageOperations;
using System.Drawing;

//email: dobija.bartosz@gmail.com
//github: https://github.com/BartShoot/Porownywanie-Obrazow
class Porownywanie_Obrazow
{
    static void Main()
    {
        Bitmap testImage = new Bitmap("D:/noise/zupa.jpg");
        Bitmap testImage2 = new Bitmap("D:/noise/zupa3.jpg");
        ImageContainer obraz1 = new ImageContainer(testImage);
        ImageContainer obraz2 = new ImageContainer(testImage);
        Operations op = new Operations();
        for (int i = 0; i < 5; i++)
        {
            double roznica = op.CompareHistogram(obraz1, obraz2, i);
            Console.WriteLine(roznica);
        }
        
        op.DrawHistogramPlot(obraz1, "D:/noise/histogram.jpg");
    }
}
