using System;

namespace _Matrix
{
    class Matrix : ICloneable 
    {
        private int my_cols;
        private int my_rows;
        private double[,] my_values;
        private static readonly Random Rnd = new Random();
        public delegate double FunctionDelegate(double value);

        #region "Constructors"

        /// <summary>
        /// Make a Matrix with the specified number of rows and columns.
        /// </summary>
        public Matrix(int rows, int columns)
        {
            my_cols = columns;
            my_rows = rows;
            my_values = new double[my_rows, my_cols];
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    my_values[I, J] = 0.0;
                }
            }
        }

        /// <summary>
        /// Make a square Matrix of the specified Size (rows = columns).
        /// </summary>
        public Matrix(int size)
        {
            my_cols = size;
            my_rows = size;
            my_values = new double[my_rows, my_cols];
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    my_values[I, J] = 0.0;
                }
            }
        }

        /// <summary>
        /// Make a Unit Matrix of the specified Size (rows = columns).
        /// </summary>
        public static Matrix Unity(int size)
        {
            Matrix result = new Matrix(size);
            for (int I = 0; I < size; I++)
            {
                result.SetValue(I, I, 1.0);
            }
            return result;
        }

        /// <summary>
        /// Make a single column Matrix from a 1D Array.
        /// </summary>
        public static Matrix FromArray(double[] values)
        {
            if (!values.IsFixedSize)
            {
                throw new Exception("FromArray: The values array must have a fixed size.");
            }
            Matrix result = new Matrix(values.Length,1);
            for (int I = 0; I < values.Length ; I++)
            {
                result.SetValue(I, 0, values[I]);
            }
            return result;
        }

        /// <summary>
        /// Make a Matrix from a 2D Array.
        /// </summary>
        public static Matrix FromArray(double[,] values)
        {
            if (!values.IsFixedSize)
            {
                throw new Exception("FromArray: The values array must have a fixed size.");
            }
            Matrix result = new Matrix(values.GetLength(0),values.GetLength(1));
            for (int I = 0; I < values.GetLength(0); I++)
            {
                for (int J = 0; J < values.GetLength(1); J++)
                {
                    result.SetValue(I, J, values[I,J]);
                }
            }
            return result;
        }

        #endregion

        #region "Properties"

        public int Columns
        {
            get { return my_cols; }
        }

        public int Rows
        {
            get { return my_rows; }
        }

        public double GetValue(int row, int col)
        {
            if (row >= 0 & row < my_rows & col >= 0 & col < my_cols)
            {
                return my_values[row, col];
            }
            else
            {
                throw new IndexOutOfRangeException("Matrix.GetValue(int, int) method was called with an invalid row or column number.");
            }
        }

        public void SetValue(int row, int col, double value)
        {
            if (row >= 0 & row < my_rows & col >= 0 & col < my_cols)
            {
                my_values[row, col] = value;
            }
            else
            {
                throw new IndexOutOfRangeException("Matrix.SetValue(int, int) method was called with an invalid row or column number.");
            }
        }

        #endregion

        #region "Self modification operations"

        /// <summary>
        /// Fills the Matrix with random values between 0 and 1.
        /// </summary>
        public void Randomize()
        {
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    my_values[I, J] = Rnd.NextDouble();
                }
            }
        }

        /// <summary>
        /// Fills the Matrix with random values between lowLimit and highLimit.
        /// </summary>
        /// <param name="lowLimit">inclusive low limit of the values</param>
        /// <param name="highLimit">non-inclusive high limit of the values</param>
        public void Randomize(double lowLimit, double highLimit)
        {
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    my_values[I, J] = lowLimit + (highLimit - lowLimit) * Rnd.NextDouble();
                }
            }
        }

        /// <summary>
        /// Fills the Matrix with random values from a Normal distribution.
        /// </summary>
        /// <param name="mean">The mean of the Normal distribution.</param>
        /// <param name="stdDev">The Standard Deviation of the Normal distribution.</param>
        public void RandomizeNormal(double mean, double stdDev)
        {
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    my_values[I, J] = stdDev * NormalDist.NormalRandom() + mean;
                }
            }
        }

        /// <summary>
        /// Add the scalar value to each element in the Matrix.
        /// </summary>
        public void AddScalar(double scalar)
        {
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    my_values[I, J] += scalar;
                }
            }
        }

        /// <summary>
        /// Multiply each element in the Matrix with the scalar value.
        /// </summary>
        public void MultiplyScalar(double scalar)
        {
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    my_values[I, J] *= scalar;
                }
            }
        }

        /// <summary>
        /// Add each element of the parameter Matrix to the same element in the Matrix.
        /// </summary>
        public void AddMatrix(Matrix C)
        {
            if (my_rows == C.Rows & my_cols == C.Columns )
            {
                for (int I = 0; I < my_rows; I++)
                {
                    for (int J = 0; J < my_cols; J++)
                    {
                        my_values[I, J] += C.GetValue(I,J);
                    }
                }
            }
            else
            {
                throw new Exception("Matrix.AddMatrix(Matrix2): Matrices size Mismatch.");
            }
        }

        /// <summary>
        /// Multiply each element in the Matrix with the same element of the parameter Matrix.
        /// </summary>
        public void MultiplyHadamard(Matrix C)
        {
            if (my_rows == C.Rows & my_cols == C.Columns)
            {
                for (int I = 0; I < my_rows; I++)
                {
                    for (int J = 0; J < my_cols; J++)
                    {
                        my_values[I, J] *= C.GetValue(I, J);
                    }
                }
            }
            else
            {
                throw new Exception("Matrix.MultiplyHadamard(Matrix2): Matrices size Mismatch.");
            }
        }

        /// <summary>
        /// Switch rows and columns of the Matrix
        /// </summary>
        public void Transpose()
        {
            double[,] dummy = new double[my_cols, my_rows];
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    dummy[J, I] = my_values[I, J];
                }
            }
            int dummy2 = my_rows; 
            my_rows = my_cols;
            my_cols = dummy2;
            my_values = dummy;
        }

        /// <summary>
        /// Apply a function to each element in the Matrix.
        /// </summary>
        /// <param name="function">Address of a function that takes a double parameter and returns a double value.</param>
        public void Map(FunctionDelegate function)
        {
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    my_values[I, J] = function(my_values[I, J]);
                }
            }
        }

        #endregion

        #region "New Matrix Operations"

        /// <summary>
        /// Returns a new Matrix by adding the scalar value to each element in the Matrix.
        /// </summary>
        public Matrix ScalarAdd(double scalar)
        {
            Matrix result = new Matrix(my_rows, my_cols);
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    result.SetValue(I,J,my_values[I, J] + scalar);
                }
            }
            return result; 
        }

        /// <summary>
        /// Returns a new Matrix by multiplying each element in the Matrix with the parameter value.
        /// </summary>
        public Matrix ScalarMultiply(double scalar)
        {
            Matrix result = new Matrix(my_rows, my_cols);
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    result.SetValue(I, J, my_values[I, J] * scalar);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a new Matrix by adding each element in the parameter Matrix
        /// <para>to the element at the same position in the Matrix.</para>
        /// </summary>
        public Matrix MatrixAdd(Matrix C)
        {
            if (my_rows == C.Rows & my_cols == C.Columns)
            {
                Matrix result = new Matrix(my_rows, my_cols);
                for (int I = 0; I < my_rows; I++)
                {
                    for (int J = 0; J < my_cols; J++)
                    {
                        result.SetValue(I, J, my_values[I, J] + C.GetValue(I, J));
                    }
                }
                return result;
            }
            else
            {
                throw new Exception("Matrix.MatrixAdd(Matrix2): Matrices size Mismatch.");
            }
        }

        /// <summary>
        /// Returns a new Matrix by multiplying each element in the parameter Matrix
        /// <para>with the element at the same position in the Matrix.</para>
        /// </summary>
        public Matrix HadamardMultiply(Matrix C)
        {
            if (my_rows == C.Rows & my_cols == C.Columns)
            {
                Matrix result = new Matrix(my_rows, my_cols);
                for (int I = 0; I < my_rows; I++)
                {
                    for (int J = 0; J < my_cols; J++)
                    {
                        result.SetValue(I, J, my_values[I, J] * C.GetValue(I, J));
                    }
                }
                return result;
            }
            else
            {
                throw new Exception("Matrix.HadamardMultiply(Matrix2): Matrices size Mismatch.");
            }
        }

        /// <summary>
        /// Returns a new Matrix by Matrix multiplication of the Matrix and the parameter Matrix.
        /// <para>The number of rows of the parameter Matrix must be equal</para>
        /// <para>to the number of columns of the Matrix.</para>
        /// </summary>
        /// <returns>The return Matrix has the number of rows from the Matrix and
        /// <para>the number of columns of the parameter Matrix</para>
        /// </returns>
        public Matrix MatrixMultiply(Matrix C)
        {
            if (my_cols == C.Rows)
            {
                Matrix result = new Matrix(my_rows, C.Columns );
                for (int I = 0; I < my_rows; I++)
                {
                    for (int J = 0; J < C.Columns ; J++)
                    {
                        for (int K = 0; K < my_cols; K++)
                        {
                            result.SetValue(I, J, result.GetValue(I, J) + my_values[I, K] * C.GetValue(K, J));
                        }
                    }
                }
                return result;
            }
            else
            {
                throw new Exception("Matrix.MatrixMultiply(Matrix2): Matrices size Mismatch.");
            }
        }

        /// <summary>
        /// Returns a new Matrix by switching rows and columns of the parameter Matrix.
        /// </summary>
        public Matrix Transpose(Matrix C)
        {
            Matrix result = new Matrix(C.Columns , C.Rows );
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    result.SetValue(J, I, C.GetValue(I, J));
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a new Matrix by Applying a function to each element in the Matrix.
        /// </summary>
        /// <param name="function">Address of a function that takes a double parameter and returns a double value.</param>
        public Matrix MapTo (FunctionDelegate function)
        {
            Matrix result = new Matrix(my_rows, my_cols);
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    result.SetValue(I, J, function(my_values[I, J]));
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a copy of the Matrix without the specified row and column.
        /// </summary>
        public Matrix SubMatrix(int row, int column)
        {
            if (row >= 0 & row < my_rows & column >= 0 & column < my_cols)
            {
                int currentX;
                int currentY;
                Matrix result = new Matrix(my_rows - 1, my_cols - 1);
                currentX = -1;
                for (int I = 0; I < my_rows; I++)
                {
                    if (I != row)
                    {
                        currentX += 1;
                        currentY = -1;
                        for (int J = 0; J < my_cols; J++)
                        {
                            if (J != column)
                            {
                                currentY += 1;
                                result.SetValue(currentX, currentY, my_values[I, J]);
                            }
                        }
                    }
                }
                return result;
            }
            else
            {
                throw new IndexOutOfRangeException("Matrix.SubMatrix(int, int) was called with an invalid row or column number.");
            }
        }

        #endregion

        #region "Static Operators"

        /// <summary>
        /// Returns the parameter Matrix.
        /// </summary>
        public static Matrix operator +(Matrix C)
        {
            return C;
        }

        /// <summary>
        /// Returns the parameter Matrix with each element multiplied by -1.
        /// </summary>
        public static Matrix operator -(Matrix C)
        {
            Matrix result = new Matrix(C.Rows, C.Columns);
            for (int I = 0; I < C.Rows; I++)
            {
                for (int J = 0; J < C.Columns; J++)
                {
                    result.SetValue(I, J, -1 * C.GetValue(I, J));
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a new Matrix by Element wise addition of 2 Matrices.
        /// </summary>
        public static Matrix operator +(Matrix left, Matrix right)
        {
            if (left.Rows == right.Rows & left.Columns == right.Columns )
            {
                Matrix result = new Matrix(left.Rows, left.Columns);
                for (int I = 0; I < left.Rows; I++)
                {
                    for (int J = 0; J < left.Columns; J++)
                    {
                        result.SetValue(I, J, left.GetValue(I, J) + right.GetValue(I, J));
                    }
                }
                return result;
            }
            else
            {
                throw new Exception("Matrix Operator +(Matrix, Matrix) : Matrices size Mismatch.");
            }
        }

        /// <summary>
        /// Returns a new Matrix by Element wise subtraction of right Matrix from left Matrix.
        /// </summary>
        public static Matrix operator -(Matrix left, Matrix right)
        {
            if (left.Rows == right.Rows & left.Columns == right.Columns)
            {
                Matrix result = new Matrix(left.Rows, left.Columns);
                for (int I = 0; I < left.Rows; I++)
                {
                    for (int J = 0; J < left.Columns; J++)
                    {
                        result.SetValue(I, J, left.GetValue(I, J) - right.GetValue(I, J));
                    }
                }
                return result;
            }
            else
            {
                throw new Exception("Matrix Operator -(Matrix, Matrix) : Matrices size Mismatch.");
            }
        }

        /// <summary>
        /// Returns a new Matrix by Matrix multiplication of 2 Matrices. 
        /// <para>The number of columns of the left Matrix must be equal</para>
        /// <para>to the number of rows of the right Matrix.</para>
        /// </summary>
        /// <returns>The return Matrix has the number of rows from the left Matrix and
        /// <para>the number of columns of the right Matrix</para>
        /// </returns>
        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left.Columns == right.Rows)
            {
                Matrix result = new Matrix(left.Rows, right.Columns);
                for (int I = 0; I < left.Rows ; I++)
                {
                    for (int J = 0; J < right.Columns; J++)
                    {
                        for (int K = 0; K < left.Columns; K++)
                        {
                            result.SetValue(I, J, result.GetValue(I, J) + left.GetValue(I, K) * right.GetValue(K, J));
                        }
                    }
                }
                return result;
            }
            else
            {
                throw new Exception("Matrix Operator *(Matrix, Matrix) : Matrices size Mismatch.");
            }
        }

        /// <summary>
        /// Returns True if each element of left Matrix is equal to the element at the same position of right Matrix.
        /// </summary>
        public static bool operator ==(Matrix left, Matrix right)
        {
            if (left.Rows == right.Rows & left.Columns == right.Columns)
            {
                for (int I = 0; I < left.Rows; I++)
                {
                    for (int J = 0; J < left.Columns; J++)
                    {
                        if (left.GetValue(I, J) != right.GetValue(I, J)) { return false; }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns True if at least one element of left Matrix is not equal to the element at the same position of right Matrix.
        /// </summary>
        public static bool operator !=(Matrix left, Matrix right)
        {
            if (left.Rows == right.Rows & left.Columns == right.Columns)
            {
                for (int I = 0; I < left.Rows; I++)
                {
                    for (int J = 0; J < left.Columns; J++)
                    {
                        if (left.GetValue(I, J) != right.GetValue(I, J)) { return true; }
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Compares the Matrix with a given object.
        /// </summary>
        /// <returns>True if Object type = Matrix and both matrices are identical</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Matrix))
            {
                return (Matrix)obj == this;
            }
            else
            {
                return false;
            }
            
        }

        /// <summary>
        /// Returns the base Hashcode.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region "Exports"

        /// <summary>
        /// Converts a 2D Matrix to a 2 dimensional Array.
        /// </summary>
        public double[,] ToArray()
        {
            double[,] result = new double[my_rows, my_cols];
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    result[I, J] = my_values[I,J];
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a single column from a Matrix to a 1 dimensional Array.
        /// </summary>
        public double[] ColToArray(int column)
        {
            if (column >= 0 & column < my_cols )
            {
                double[] result = new double[my_rows];
                for (int I = 0; I < my_rows; I++)
                {
                    result[I] = my_values[I, column];
                }
                return result;
            }
            else
            {
                throw new IndexOutOfRangeException("Matrix.ColToArray(int) was called with an invalid column number."); 
            }
        }

        /// <summary>
        /// Converts a single row from a Matrix to a 1 dimensional Array.
        /// </summary>
        public double[] RowToArray(int row)
        {
            if (row >= 0 & row < my_rows)
            {
                double[] result = new double[my_cols];
                for (int I = 0; I < my_cols; I++)
                {
                    result[I] = my_values[row, I];
                }
                return result;
            }
            else
            {
                throw new IndexOutOfRangeException("Matrix.RowToArray(int) was called with an invalid row number.");
            }
        }

        /// <summary>
        /// Creates a clone of a Matrix to implement ICloneable.
        /// </summary>
        public object Clone()
        {
            Matrix result = new Matrix(my_rows, my_cols);
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    result.SetValue(I, J, my_values[I, J]);
                }
            }
            return result;
        }

        /// <summary>
        /// Creates a deep copy of a Matrix.
        /// </summary>
        public Matrix Copy()
        {
            Matrix result = new Matrix(my_rows, my_cols);
            for (int I = 0; I < my_rows; I++)
            {
                for (int J = 0; J < my_cols; J++)
                {
                    result.SetValue(I, J, my_values[I, J]);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a string description of a Matrix (format= [x,x,x]\n[x,x,x]...)
        /// </summary>
        public override string ToString()
        {
            string temp = "";
            for (int I = 0; I < my_rows; I++)
            {
                temp += "[";
                for (int J = 0; J < my_cols; J++)
                {
                    temp += my_values[I, J].ToString();
                    if (J < my_cols -1) { temp += " , "; }
                }
                temp += "]" + Environment.NewLine;
            }
            return temp;
        }

        /// <summary>
        /// Returns a formatted string description of a Matrix (format= [x,x,x]\n[x,x,x]...)
        /// </summary>
        public string ToString(string format)
        {
            string temp = "";
            for (int I = 0; I < my_rows; I++)
            {
                temp += "[";
                for (int J = 0; J < my_cols; J++)
                {
                    temp += my_values[I, J].ToString(format);
                    if (J < my_cols - 1) { temp += " , "; }
                }
                temp += "]" + Environment.NewLine;
            }
            return temp;
        }

        /// <summary>
        /// Calculates the Determinant of a square Matrix.
        /// </summary>
        /// <returns></returns>
        public double Determinant()
        {
            if (my_rows == my_cols )
            {
                //Convert the Matrix to a 2D array
                double[,] temp = new double[my_rows , my_cols];
                for (int I = 0; I < my_rows; I++)
                {
                    for (int J = 0; J < my_cols; J++)
                    {
                        temp[I, J] = my_values[I, J];
                    }
                }
                //Calculate the Determinant with the Recursive CalcDet function.
                return CalcDet(temp, my_rows);
            }
            else
            {
                throw new FormatException("Matrix.Determinant() can only be used on square Matrices.");
            }
        }

        private double CalcDet(double[,] D, int N)
        {
            if (N <= 0) { return double.NaN; }
            if (D.GetUpperBound(0) != N - 1) { return double.NaN; }
            if (D.GetUpperBound(1) != N - 1) { return double.NaN; }
            if (N == 1) { return D[0, 0]; }
            if (N == 2) { return D[0, 0] * D[1, 1] - D[0, 1] * D[1, 0]; }
            double result = 0;
            double[,] Det = new double[N - 1, N - 1];
            int sign = 1;
            int col;
            //Develop to the index 0 row
            for (int I = 0; I < N; I++)
            {
                //Make (N-1)x(N-1) matrices for each element D(0,I)
                for (int J = 1; J < N; J++)
                {
                    col = 0;
                    for (int K = 0; K < N; K++)
                    {
                        if (K == I) { continue; }
                        Det[J - 1, col] = D[J, K];
                        col += 1;
                    }
                }
                //Calculate the determinants of the sub matrices recursively.
                result += sign * D[0, I] * CalcDet(Det, N - 1);
                sign *= -1;
            }
            return result;
        }

        #endregion

    }

    class NormalDist
    {
        private static bool flag = false;
        private static double  gset = 0;
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Returns a randomly distributed drawing from a standard normal distribution
        /// <para>with: Average = 0 and Standard Deviation = 1.0.</para>
        /// </summary>
        /// <returns></returns>
        public static double NormalRandom()
        {
            double result;
            double fac;
            double rsq;
            double v1;
            double v2;
            if (flag)
            {
                result = gset;
                flag = false;
            }
            else
            {
                do
                {
                    v1 = 2 * Rnd.NextDouble() - 1.0;
                    v2 = 2 * Rnd.NextDouble() - 1.0;
                    rsq = v1 * v1 + v2 * v2;
                } while (rsq > 1.0);
                fac = Math.Sqrt(-2.0 * Math.Log(rsq) / rsq);
                result = v2 * fac;
                gset = v1 * fac;
                flag = true;
            }
            return result;
        }
    }
}
