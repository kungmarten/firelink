using System;
using System.Threading.Tasks;

namespace KalmanFilters
{
    class Matrix
    {
        private double[] flatmap;
        private int columns;
        private int rows;
        private double[][] matrix;

        // Constructor:
        public Matrix(double[] flatmap, int columns)
        {
            int rowColFactor = flatmap.Length % columns;
            try
            {
                if (rowColFactor != 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("The length of the array ({0}) is not a factor of {1}.", flatmap.Length, columns);
            }

            this.flatmap = flatmap;
            this.columns = columns;
            rows = flatmap.Length / columns;
        }

        public Matrix(double[][] matrix)
        {
            int rowColFactor = flatmap.Length % columns;
            try
            {
                if (rowColFactor != 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("The length of the array ({0}) is not a factor of {1}.", flatmap.Length, columns);
            }

            this.matrix = matrix;
            //this.flatmap = ;
            columns = matrix[0].Length;
            rows = matrix.Length;
        }

        private double[][] MatrixCreate()
        {
            double[][] result = new double[Rows][];
            for (int i = 0; i < Rows; ++i)
                result[i] = new double[Columns];

            return result;
        }

        private static double[][] MatrixCreate(int rows, int columns)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[columns];

            return result;
        }

        private double[][] PopulateMatrix()
        {
            double[][] result = MatrixCreate();

            Parallel.For(0, Rows, i =>
            {
                int k = i * Columns;
                for (int j = 0; j < Columns; j++)
                {
                    result[i][j] = FlatMap[j + k];
                }

            });

            return result;
        }

        public double[] FlatMap
        {
            get { return flatmap; }
        }

        public int Columns
        {
            get { return columns; }
        }

        public int Rows
        {
            get { return rows; }
        }

        public double[][] AsMatrix
        {
            get { return PopulateMatrix(); }
        }

        // Printing method:
        public void ShowMatrix()
        {
            double[][] matrix = PopulateMatrix();

            for (int i = 0; i < matrix.Length; i++)
            {
                Console.Write("Element({0}): ", i);

                for (int j = 0; j < matrix[i].Length; j++)
                {
                    Console.Write("{0}{1}", matrix[i][j], j == (matrix[i].Length - 1) ? "" : " ");
                }
                Console.WriteLine();
            }
        }

        public static double[][] MatrixProduct(Matrix matrixA,
                                    Matrix matrixB)
        {
            int aRows = matrixA.Rows; int aCols = matrixA.Columns;
            int bRows = matrixB.Rows; int bCols = matrixB.Columns;
            if (aCols != bRows)
                throw new Exception("xxxx");

            double[][] result = MatrixCreate(aRows, bCols);

            Parallel.For(0, aRows, i =>
            {
                for (int j = 0; j < bCols; ++j) // each col of B
                    for (int k = 0; k < aCols; ++k) // could use k < bRows
                        result[i][j] += matrixA.AsMatrix[i][k] * matrixB.AsMatrix[k][j];
            }
            );

            return result;
        }
    }
}