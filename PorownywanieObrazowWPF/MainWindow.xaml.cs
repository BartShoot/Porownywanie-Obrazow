using ImageOperations;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PorownywanieObrazowWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageContainer image1;
        ImageContainer image2;
        Operations operations = new Operations();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                Img1.Source = new BitmapImage(new Uri(op.FileName));
                image1 = new ImageContainer(new System.Drawing.Bitmap(op.FileName));
            }         
        }
        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                Img2.Source = new BitmapImage(new Uri(op.FileName));
                image2 = new ImageContainer(new System.Drawing.Bitmap(op.FileName));
            }
        }

        private void Image1FunctionSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int check = Image1FunctionSelect.SelectedIndex;
            switch (check)
            {
                case 0:
                    if (image1 != null && image2 != null)
                    {
                        var histResult= Convert.ToString(operations.CompareHistogram(image1, image2, 1));
                        MessageBox.Show(histResult);
                        image1Output.Text += Environment.NewLine + histResult;
                    }
                    else
                        MessageBox.Show("Załaduj oba obrazy","Uwaga!",MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 1:
                    if (image1 != null)
                    {
                        var result = operations.EdgeDetect(image1, "edgeDetect.png");
                        MessageBox.Show(Convert.ToString(result.amount));
                        var path = System.IO.Path.Combine(Environment.CurrentDirectory, result.fileName);
                        Img1Result.Source = new BitmapImage(new Uri(path));
                    }
                    else
                        MessageBox.Show("Załaduj obraz", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                default:
                    MessageBox.Show("oof");
                    break;
            }
        }
    }
}
