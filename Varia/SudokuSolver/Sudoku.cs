using System;
using System.IO;
using System.Windows;

namespace SudokuSolver
{
    class Sudoku
    {
        private Cel[] cells = new Cel[81];

        public Sudoku()
        {
            for (int I = 0; I <= 80; I++)
            {
                cells[I] = new Cel((int)Math.Floor((double)I / 9) + 1, I % 9 + 1);
            }
        }

        public Sudoku copy()
        {
            Sudoku s = new Sudoku();
            for (int I = 0; I <= 80; I++)
            {
                s.SetCel(I, new Cel(cells[I].Row, cells[I].Col));
                s.GetCel(I).Number = cells[I].Number;
                s.GetCel(I).SetIsAllowed(0, false);
                for (int J = 1; J <= 9; J++)
                {
                    s.GetCel(I).SetIsAllowed(J, cells[I].GetIsAllowed(J));
                }
                s.GetCel(I).Fixed = cells[I].Fixed;
                s.GetCel(I).Given = cells[I].Given;
            }
            return s;
        }

        public Cel GetCel(int index)
        {
            return cells[index];
        }

        public void SetCel(int index, Cel value)
        {
            cells[index] = value;
        }

        public void SetFixedCel(int index, int value)
        {
            cells[index].Number = value;
            cells[index].Fixed = true;
        }

        public void ClearFixedCel(int index)
        {
            cells[index].Fixed = false;
            cells[index].Number = 0;
        }

        public void SetGivenCel(int index, int value)
        {
            cells[index].Number = value;
            cells[index].Given = true;
        }

        public void ClearGivenCel(int index)
        {
            cells[index].Given = false;
            cells[index].Number = 0;
        }
        public void Clear()
        {
            for (int I = 0; I <= 80; I++)
            {
                cells[I] = new Cel((int)Math.Floor((double)I / 9) + 1, I % 9 + 1);
            }
        }

        public int TotalFilled()
        {
            int teller = 0;
            for (int I = 0; I <= 80; I++)
            {
                if (cells[I].Number > 0)
                {
                    teller += 1;
                }
            }
            return teller;
        }

        public void UpdateValues()
        {
            //Check the allowed values for each cel
            int r;
            int c;
            int b;
            for (int I = 0; I <= 80; I++)
            {
                if (cells[I].Number == 0)
                {
                    r = cells[I].Row;
                    c = cells[I].Col;
                    b = cells[I].Block;
                    //Step1: allow all values (1 to 9)
                    for (int J = 1; J <= 9; J++)
                    {
                        cells[I].SetIsAllowed(J, true);
                    }
                    //Step 2: Check all cells in the same row, column and block
                    //        if a Cel has a value != 0 then this value is not allowed
                    for (int J = 0; J <= 80; J++)
                    {
                        if (cells[J].Number != 0)
                        {
                            if (cells[J].Row == r | cells[J].Col == c | cells[J].Block == b)
                            {
                                cells[I].SetIsAllowed(cells[J].Number, false);
                            }
                        }
                    }
                }
                else    //Cel has a value so all others are not allowed
                {
                    for (int J = 1; J <= 9; J++)
                    {
                        cells[I].SetIsAllowed(J, false);
                    }
                    cells[I].SetIsAllowed(cells[I].Number, true);
                }
            }
        }

        public void Load(string filename)
        {
            Clear();
            StreamReader myStream = null;
            string S = "";
            try
            {
                myStream = new StreamReader(filename);
                if (myStream != null)
                {
                    //Read the Sudoku data from the file
                    S = myStream.ReadLine();
                    //fill the numbers in the sudoku cells
                    for (int I = 0; I <= 80; I++)
                    {
                        cells[I].Number = int.Parse(S.Substring(I, 1));
                        if (cells[I].Number > 0)
                        {
                            cells[I].Fixed = true;
                            cells[I].Given = true;
                        }
                    }
                    UpdateValues();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Cannot read the Sudoku data. Original error: " + Ex.Message, "SudokuSolver error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (myStream != null) myStream.Close();
            }
        }

        public void Save(string filename)
        {
            StreamWriter myStream = null;
            try
            {
                myStream = new StreamWriter(filename);
                if (myStream != null)
                {
                    //Write the Sudoku data to the file
                    for (int I = 0; I <= 80; I++)
                    {
                        myStream.Write(cells[I].Number);
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Cannot save the Sudoku data. Original error: " + Ex.Message, "SudokuSolver error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (myStream != null) myStream.Close();
            }
        }
    }
}
