using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOperations
{
    public class ConvolutionOperations
    {
        public (double amount, string fileName) EdgeDetect(ImageContainer image, string fileName)
        {
            int[][] matrix = new int[3][]
            {
                new int[] {0, -1, 0 },
                new int[] {-1, 4, -1 },
                new int[] {0, -1, 0 }
            };
            Bitmap edgeDetectImage = new Bitmap(image.Width, image.Height);
            Color color = new Color();
            double edgeDetectAmount = 0;


            int LoadMatrix(int x, int y)
            {
                if (x < 0 || x >= image.Width)
                    return 0;
                if (y < 0 || y >= image.Height)
                    return 0;
                return (image.R[x][y] + image.G[x][y] + image.B[x][y]);
            }

            int matrixSize = matrix[0].Length;
            int[][] imageMatrix = new int[matrixSize][];
            for (int i = 0; i < matrixSize; i++)
                imageMatrix[i] = new int[matrixSize];

            int newPixelValue = 0, loadOffset= (int)(Math.Floor((double)matrixSize / 2));

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    for (int k = 0; k < matrixSize; k++)
                    {
                        for (int l = 0; l < matrixSize; l++)
                        {
                            imageMatrix[k][l] = LoadMatrix(i - loadOffset + k, j - loadOffset + l);
                        }
                    }

                    newPixelValue = 0;

                    for (int k = 0; k < matrixSize; k++)
                    {
                        for (int l = 0; l < matrixSize; l++)
                        {
                            newPixelValue+=imageMatrix[k][l]*matrix[k][l];
                        }
                    }

                    if (newPixelValue < 0)
                        newPixelValue = 0;
                    if (newPixelValue > 255)
                        newPixelValue = 255;

                    if (newPixelValue > 128)
                    {
                        edgeDetectAmount += newPixelValue;
                    }

                    color = Color.FromArgb(255, newPixelValue, newPixelValue, newPixelValue);

                    edgeDetectImage.SetPixel(i, j, color);
                }
            }
            edgeDetectAmount = edgeDetectAmount / (image.Width * image.Height);
            edgeDetectImage.Save(fileName);
            return (edgeDetectAmount, fileName);
        }

        public (double amount, string fileName) MatrixOP(ImageContainer image, string fileName, int[][] matrix)
        {
            Bitmap edgeDetectImage = new Bitmap(image.Width, image.Height);
            Color color = new Color();
            double edgeDetectAmount = 0;


            int LoadMatrix(int x, int y)
            {
                if (x < 0 || x >= image.Width)
                    return 0;
                if (y < 0 || y >= image.Height)
                    return 0;
                return (image.R[x][y] + image.G[x][y] + image.B[x][y]);
            }

            int matrixSize = matrix[0].Length;
            int[][] imageMatrix = new int[matrixSize][];
            for (int i = 0; i < matrixSize; i++)
                imageMatrix[i] = new int[matrixSize];

            int newPixelValue = 0, loadOffset = (int)(Math.Floor((double)matrixSize / 2));

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    for (int k = 0; k < matrixSize; k++)
                    {
                        for (int l = 0; l < matrixSize; l++)
                        {
                            imageMatrix[k][l] = LoadMatrix(i - loadOffset + k, j - loadOffset + l);
                        }
                    }

                    newPixelValue = 0;

                    for (int k = 0; k < matrixSize; k++)
                    {
                        for (int l = 0; l < matrixSize; l++)
                        {
                            newPixelValue += imageMatrix[k][l] * matrix[k][l];
                        }
                    }

                    if (newPixelValue < 0)
                        newPixelValue = 0;
                    if (newPixelValue > 255)
                        newPixelValue = 255;

                    edgeDetectAmount += newPixelValue;

                    color = Color.FromArgb(255, newPixelValue, newPixelValue, newPixelValue);

                    edgeDetectImage.SetPixel(i, j, color);
                }
            }
            edgeDetectAmount = edgeDetectAmount / (image.Width * image.Height);
            edgeDetectImage.Save(fileName);
            return (edgeDetectAmount, fileName);
        }
    }
}
