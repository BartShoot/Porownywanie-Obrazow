using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOperations
{
    public class MatrixContainer
    {
        public string name;
        public double[][] values;

        MatrixContainer(string name, int n)
        {
            this.name = name;
            values = new double[n][];
            for (int i = 0; i < n; i++)
            {
                values[i] = new double[n];
            }
        }
        public MatrixContainer(string name, int n, double[][] matrix)
        {
            this.name = name;
            values = new double[n][];
            for (int i = 0; i < n; i++)
            {
                values[i] = new double[n];
            }
            for (int i = 0;i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    values[i][j] = matrix[i][j];
                }
            }
        }

        public void SaveToJson()
        {
            string fileName =  Environment.CurrentDirectory + "\\matrixOpPresets"+$"\\{this.name}.json";
            if (System.IO.File.Exists(fileName) == false) 
            {
                var _test = this;
                var jsonFormattedContent = Newtonsoft.Json.JsonConvert.SerializeObject(_test);
                System.IO.File.WriteAllText(fileName, jsonFormattedContent);
            }
        }
    }
}
