using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firelink
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] startMatrix = new double[] { 5.0, 6.0, 7.0, 9.0, 10.0, 1.0, 1.0, 2.0, 1.0, 1.0 };
            Matrix myMatrix = new Matrix(startMatrix, 5);
            Matrix mySecondMatrix = new Matrix(startMatrix.Reverse().ToArray(), 2);

            myMatrix.ShowMatrix();
            mySecondMatrix.ShowMatrix();
            var prodMatrix = myMatrix * mySecondMatrix;
            prodMatrix.ShowMatrix();
            myMatrix.Transpose().ShowMatrix();
            prodMatrix.Identity().Transpose().ShowMatrix();
            Console.ReadKey();
        }
    }
}
