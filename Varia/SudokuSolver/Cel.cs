using System;

namespace SudokuSolver
{
    class Cel
    {
        private int my_Index;
        private int my_Row;
        private int my_Column;
        private int my_Block;
        private bool my_Fixed;
        private bool my_given;
        private bool[] my_Values = new bool[10];
        private int my_Number;

        public Cel(int row, int column)
        {
            if (row >= 1 & row <= 9)
            {
                my_Row = row;
            }
            else
            {
                throw new ArgumentException("The Row must be from 1 to 9 in new Cel");
            }
            if (column >= 1 & column <= 9)
            {
                my_Column = column;
            }
            else
            {
                throw new ArgumentException("The Column must be from 1 to 9 in new Cel");
            }
            my_Index = 9 * (row - 1) + column - 1;
            my_Block = GetBlockNr(row, column);
            my_Fixed = false;
            my_given = false;
            my_Values[0] = false;
            for (int I = 1; I <= 9; I++)
            {
                my_Values[I] = true;
            }
            my_Number = 0;
        }

        public int Row
        {
            get { return my_Row; }
            set
            {
                if (value >= 1 & value <= 9)
                {
                    my_Row = value;
                }
                else
                {
                    throw new ArgumentException("The Row must be from 1 to 9 in Cel " + my_Index.ToString());
                }
            }
        }

        public int Col
        {
            get { return my_Column; }
            set
            {
                if (value >= 1 & value <= 9)
                {
                    my_Row = value;
                }
                else
                {
                    throw new ArgumentException("The Column must be from 1 to 9 in Cel " + my_Index.ToString());
                }
            }
        }

        public int Block
        {
            get { return my_Block; }
            set
            {
                if (value >= 1 & value <= 9)
                {
                    my_Block = value;
                }
                else
                {
                    throw new ArgumentException("The Block must be from 1 to 9 in Cel " + my_Index.ToString());
                }
            }
        }

        public bool Fixed
        {
            get { return my_Fixed; }
            set { my_Fixed = value; }
        }

        public bool Given
        {
            get { return my_given; }
            set { my_given = value; }
        }

        public bool GetIsAllowed(int Number)
        {
            if (Number >= 0 & Number <= 9)
            {
                return my_Values[Number];
            }
            else
            {
                return false;
            }
        }

        public void SetIsAllowed(int Number, bool Value)
        {
            if (Number >= 0 & Number <= 9)
            {
                my_Values[Number] = Value;
            }
            else
            {
                throw new ArgumentException("The allowed number must be from 0 to 9 in Cel " + my_Index.ToString());
            }
        }

        public int Number
        {
            get { return my_Number; }
            set
            {
                if (my_Fixed) throw new Exception("The number can not be changed in fixed Cel " + my_Index.ToString());
                if (value >= 0 & value <= 9)
                {
                    my_Number = value;
                }
                else
                {
                    throw new ArgumentException("The number must be from 0 to 9 in Cel " + my_Index.ToString());
                }
            }
        }


        public int[] GetAllowedValues()
        {
            int teller = 0;
            for (int I = 1; I <= 9; I++)
            {
                if (my_Values[I]) teller += 1;
            }
            int[] result = new int[teller];
            teller = 0;
            for (int I = 1; I <= 9; I++)
            {
                if (my_Values[I])
                {
                    result[teller] = I;
                    teller += 1;
                }
            }
            return result;
        }

        public int TotalAllowed()
        {
            int teller = 0;
            for (int I = 1; I <= 9; I++)
            {
                if (my_Values[I]) teller += 1;
            }
            return teller;
        }

        private int GetBlockNr(int row, int col)
        {
            if (row < 4)
            {
                if (col < 4) return 1;
                if (col < 7) return 2;
                if (col < 10) return 3;
            }
            if (row < 7)
            {
                if (col < 4) return 4;
                if (col < 7) return 5;
                if (col < 10) return 6;
            }
            if (row < 10)
            {
                if (col < 4) return 7;
                if (col < 7) return 8;
                if (col < 10) return 9;
            }
            return 0;
        }
    }
}
