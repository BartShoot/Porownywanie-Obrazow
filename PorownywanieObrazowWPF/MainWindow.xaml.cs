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
        ImageContainer imageEdgeDetect;
        Operations operations = new Operations();
        ConvolutionOperations convolutionOperations = new ConvolutionOperations();
        TextBox[,] _textBox;
        public MainWindow()
        {
            InitializeComponent();

            System.IO.Directory.CreateDirectory(@"D:\zdj\matrixop");
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
                if (sender.Equals(LoadImageForEdgeDetect))
                {
                    ImgEdgeDetect.Source = new BitmapImage(new Uri(op.FileName));
                    imageEdgeDetect = new ImageContainer(new System.Drawing.Bitmap(op.FileName));
                }
                if (sender.Equals(Img1Loader))
                {
                    Img1.Source = new BitmapImage(new Uri(op.FileName));
                    image1 = new ImageContainer(new System.Drawing.Bitmap(op.FileName));
                }
                if (sender.Equals(Img2Loader))
                {
                    Img2.Source = new BitmapImage(new Uri(op.FileName));
                    image2 = new ImageContainer(new System.Drawing.Bitmap(op.FileName));
                }
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
                        image1Output.Text += Environment.NewLine + "Wynik porównania histogramu(korelacja):"+Environment.NewLine + histResult;
                    }
                    else
                        MessageBox.Show("Załaduj oba obrazy","Uwaga!",MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 1:
                    if (image1 != null)
                    {
                        var result = operations.EdgeDetect(image1, "edgeDetect.png");
                        MessageBox.Show(Convert.ToString(result.amount));
                        image1Output.Text += Environment.NewLine + "Ilość znalezionych krawędzi:" + Environment.NewLine + result.amount;
                        var path = System.IO.Path.Combine(Environment.CurrentDirectory, result.fileName);
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = new Uri(path);
                        image.EndInit();
                        Img1Result.Source = image;
                    }
                    else
                        MessageBox.Show("Załaduj obraz", "Uwaga!", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                default:
                    MessageBox.Show("oof");
                    break;
            }
        }

        private void RadioButton_Checked3(object sender, RoutedEventArgs e)
        {
            CreateTextBox(3);
        }

        private void RadioButton_Checked5(object sender, RoutedEventArgs e)
        {
            CreateTextBox(5);
        }

        private void RadioButton_Checked7(object sender, RoutedEventArgs e)
        {
            CreateTextBox(7);
        }

        private void RadioButton_Checked9(object sender, RoutedEventArgs e)
        {
            CreateTextBox(9);
        }

        private void CreateTextBox(int N)
        {
            _textBox = new TextBox[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    _textBox[i, j] = new TextBox();
                    _textBox[i, j].FontSize = 20;
                    _textBox[i, j].Width = 60;
                    _textBox[i, j].Height = 60;
                    _textBox[i, j].Margin = new Thickness(100 * i,100 * j, 1, 1);
                    _textBox[i, j].BorderThickness = new Thickness(5);
                    _textBox[i, j].BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    G.Children.Add(_textBox[i, j]);
                    Grid.SetRow(_textBox[i, j], 0);
                }
            }
        }

        private void CalculateMatrix_Click(object sender, RoutedEventArgs e)
        {
            int[][] convertedMatrix;
            convertedMatrix = new int[_textBox.GetLength(0)][];
            for (int i = 0; i < _textBox.GetLength(0); i++)
                convertedMatrix[i] = new int[_textBox.GetLength(0)];

            for (int i = 0; i < _textBox.GetLength(0); i++)
            {
                for (int j = 0; j < _textBox.GetLength(0); j++)
                {
                    convertedMatrix[i][j] = Convert.ToInt32(_textBox[i, j].Text);
                }
            }

            var result = convolutionOperations.MatrixOP(imageEdgeDetect, $"matrixOp{DateTime.Now.Ticks.ToString()}.png", convertedMatrix);
            var path = System.IO.Path.Combine(Environment.CurrentDirectory, result.fileName);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path);
            image.EndInit();
            ImgEdgeDetectResult.Source = image;
        }
    }
}
