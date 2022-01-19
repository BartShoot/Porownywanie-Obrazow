using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOperations
{
    public class PresetMatrixArray
    {
        public List<MatrixContainer> PresetMatrix = new List<MatrixContainer>();
        public PresetMatrixArray(string path)
        {
            PresetMatrix.Clear();
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                MatrixContainer container = ReadFromJson(files[i]);
                if (container!=null)
                {
                    PresetMatrix.Add(container);
                }
            }

            //presety
            double[][] matrix =
            {
                new double [] { 0,-1,0},
                new double [] { -1,4,-1},
                new double [] { 0,-1,0}
            };
            MatrixContainer matrixToCheck = new("Wykrywanie krawedzi", 3, matrix);

            bool containsItem = PresetMatrix.Any(item => item.name == matrixToCheck.name);
            if (!containsItem)
            {
                matrixToCheck.SaveToJson();
                PresetMatrix.Add(matrixToCheck);
            }

            matrix = new double[][]
            {
                new double [] { 0,0,0},
                new double [] { 0,1,0},
                new double [] { 0,0,0}
            };
            matrixToCheck = new("Tożsamość", 3, matrix);

            containsItem = PresetMatrix.Any(item => item.name == matrixToCheck.name);
            if (!containsItem)
            {
                matrixToCheck.SaveToJson();
                PresetMatrix.Add(matrixToCheck);
            }

            matrix = new double[][]
            {
                new double [] { -1, -1, -1},
                new double [] { -1, 8, -1},
                new double [] { -1, -1, -1 }
            };
            matrixToCheck = new("Wykrywanie krawędzi 2", 3, matrix);

            containsItem = PresetMatrix.Any(item => item.name == matrixToCheck.name);
            if (!containsItem)
            {
                matrixToCheck.SaveToJson();
                PresetMatrix.Add(matrixToCheck);
            }

            matrix = new double[][]
            {
                new double [] { 0, -1, 0},
                new double [] { -1, 5, -1},
                new double [] { 0, -1, 0 }
            };
            matrixToCheck = new("Wyostrz", 3, matrix);

            containsItem = PresetMatrix.Any(item => item.name == matrixToCheck.name);
            if (!containsItem)
            {
                matrixToCheck.SaveToJson();
                PresetMatrix.Add(matrixToCheck);
            }

            matrix = new double[][]
            {
                new double [] { .1, .1, .1},
                new double [] { .1, .1, .1},
                new double [] { .1, .1, .1 }
            };
            matrixToCheck = new("Box Blur", 3, matrix);

            containsItem = PresetMatrix.Any(item => item.name == matrixToCheck.name);
            if (!containsItem)
            {
                matrixToCheck.SaveToJson();
                PresetMatrix.Add(matrixToCheck);
            }

            matrix = new double[][]
            {
                new double [] { 1.0 / 16, 2.0 / 16, 1.0 / 16},
                new double [] { 2.0 / 16, 4.0 / 16, 2.0 / 16},
                new double [] { 1.0 / 16, 2.0 / 16, 1.0 / 16 }
            };
            matrixToCheck = new("Gaussian Blur", 3, matrix);

            containsItem = PresetMatrix.Any(item => item.name == matrixToCheck.name);
            if (!containsItem)
            {
                matrixToCheck.SaveToJson();
                PresetMatrix.Add(matrixToCheck);
            }

            matrix = new double[][]
            {
                new double [] { 0, 0, 0},
                new double [] { -1, 1, 0},
                new double [] { 0, 0, 0 }
            };
            matrixToCheck = new("Edge Enhance", 3, matrix);

            containsItem = PresetMatrix.Any(item => item.name == matrixToCheck.name);
            if (!containsItem)
            {
                matrixToCheck.SaveToJson();
                PresetMatrix.Add(matrixToCheck);
            }

            matrix = new double[][]
            {
                new double [] { -2, -1, 0},
                new double [] { -1, 1, 1},
                new double [] { 0, 1, 2 }
            };
            matrixToCheck = new("Emboss", 3, matrix);

            containsItem = PresetMatrix.Any(item => item.name == matrixToCheck.name);
            if (!containsItem)
            {
                matrixToCheck.SaveToJson();
                PresetMatrix.Add(matrixToCheck);
            }
        }
        public MatrixContainer? ReadFromJson(string fileName)
        {
            if (System.IO.File.Exists(fileName) != false)
            {
                string plainText = File.ReadAllText(fileName);
                MatrixContainer m = Newtonsoft.Json.JsonConvert.DeserializeObject<MatrixContainer>(plainText);
                return m;
            }
            return null;
        }
    }
}