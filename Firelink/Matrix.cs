using System;
using System.Threading.Tasks;

namespace Firelink
{
    class Matrix
    {
        //private double[] flatmap;
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

            //this.flatmap = flatmap;
            this.columns = columns;
            rows = flatmap.Length / columns;
            matrix = PopulateMatrix(flatmap);
        }

        public Matrix(double[][] matrix)
        {
            this.matrix = matrix;
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

        private static double[][] Zeroes(int rows, int columns)
        {
            double[][] result = MatrixCreate(rows, columns);
            Parallel.For(0, rows, i =>
            { 
                for (int j = 0; j < columns; j++)
                {
                    result[i][j] = 0.0;
                }
            });

            return result;
        }

        private double[][] PopulateMatrix(double[] flatmap)
        {
            double[][] result = MatrixCreate();

            Parallel.For(0, Rows, i =>
            {
                int k = i * Columns;
                for (int j = 0; j < Columns; j++)
                {
                    result[i][j] = flatmap[j + k];
                }

            });

            return result;
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
            get { return matrix; }
        }

        // Printing method:
        public void ShowMatrix()
        {
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

        // Matrix transpose.
        public Matrix Transpose()
        {
            var transpose = MatrixCreate(Columns, Rows);
            Parallel.For(0, Rows, i =>
            {
                for (int j = 0; j < Columns; j++)
                {
                    transpose[j][i] = matrix[i][j];
                }
            });
            Matrix result = new Matrix(transpose);
            return result;
        }

        // The identiy of the matrix.
        public Matrix Identity()
        {
            if (Columns != Rows)
                throw new Exception("Invalid size of matrix.");
            else
            {
                var identity = MatrixCreate(Columns, Rows);
                Parallel.For(0, Rows, i =>
                {
                    for (int j = 0; j < Columns; j++)
                    {
                            if (i == j)
                                identity[j][i] = 1;
                            else
                                identity[j][i] = 0;
                    } 
                });
                Matrix result = new Matrix(identity);
                return result;
            }
        }

        // Overload the multiplication for matrices.
        public static Matrix operator *(Matrix matrixA,
                                    Matrix matrixB)
        {
            int aRows = matrixA.Rows; int aCols = matrixA.Columns;
            int bRows = matrixB.Rows; int bCols = matrixB.Columns;
            if (aCols != bRows)
                throw new Exception("Columns of matrix A does not match rows of matrix B.");

            double[][] product = MatrixCreate(aRows, bCols);

            Parallel.For(0, aRows, i =>
            {
                for (int j = 0; j < bCols; ++j) // each col of B
                    for (int k = 0; k < aCols; ++k) // could use k < bRows
                        product[i][j] += matrixA.AsMatrix[i][k] * matrixB.AsMatrix[k][j];
            }
            );

            Matrix result = new Matrix(product);

            return result;
        }

        // Overload the addition for matrices.
        public static Matrix operator +(Matrix matrixA,
                                    Matrix matrixB)
        {
            if (matrixA.Columns != matrixB.Columns || matrixA.Rows != matrixB.Rows)
                throw new Exception("Matrices must be of equal dimensions to add.");
            else
            {
                var cols = matrixA.Columns;
                var rows = matrixA.Columns;
                double[][] addition = MatrixCreate(rows, cols);

                Parallel.For(0, rows, i =>
                {
                    for (int j = 0; j < cols; j++) // each col of B
                    {
                        addition[i][j] = matrixA.AsMatrix[i][j] + matrixB.AsMatrix[i][j];
                    } 
                });

                Matrix result = new Matrix(addition);
                return result;
            }
        }

        // Overload the subtraction for matrices.
        public static Matrix operator -(Matrix matrixA,
                                    Matrix matrixB)
        {
            if (matrixA.Columns != matrixB.Columns || matrixA.Rows != matrixB.Rows)
                throw new Exception("Matrices must be of equal dimensions to subtract.");
            else
            {
                var cols = matrixA.Columns;
                var rows = matrixA.Columns;
                double[][] addition = MatrixCreate(rows, cols);

                Parallel.For(0, rows, i =>
                {
                    for (int j = 0; j < cols; j++) // each col of B
                    {
                        addition[i][j] = matrixA.AsMatrix[i][j] - matrixB.AsMatrix[i][j];
                    } 
                });

                Matrix result = new Matrix(addition);
                return result;
            }
        }

        //Computes the upper triangular Cholesky factorization of a positive definite matrix.
        public Matrix Cholesky(double zTol = 1.0e-5)
        {
            var cholesky = Zeroes(Rows, Columns);
            for (int i = 0; i < Rows; i++)
            {
                double S = 0.0;
                for (int k = 0; k < i; k++)
                {
                    S = S + Math.Pow(cholesky[k][i], 2.0);
                }
                double d = AsMatrix[i][i] - S;
                if (Math.Abs(d) < zTol)
                {
                    cholesky[i][i] = 0.0;

                }
                else if (d < 0.0)
                {
                    throw new Exception("Matrix not positive-definite.");
                }
                else
                {
                    cholesky[i][i] = Math.Sqrt(d);
                }
                for (int j = i; j < Rows; j++)
                {
                    S = 0.0;
                    for (int k = 0; k < Rows; k++)
                    {
                        S = S + cholesky[k][i] * cholesky[k][j];
                    }
                    if (Math.Abs(S) < zTol)
                    {
                        S = 0.0;
                    }
                    cholesky[i][j] = (AsMatrix[i][i] - S) / cholesky[i][i];
                }
            }

            Matrix result = new Matrix(cholesky);
            return result;
        }
    }
}