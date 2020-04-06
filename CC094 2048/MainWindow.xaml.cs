using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2048
{
    public partial class MainWindow : Window
    {
        private readonly int RowNum = 4;
        private readonly int ColNum = 4;
        private double CelWidth = 0.0;
        private double CelHeight = 0.0;
        private readonly double LineThickness = 10.0;
        private Cell[,] Cells;
        private bool GameStarted;
        private int Score = 0;
        private readonly Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Line gridLine;
            double FieldWidth = Canvas1.ActualWidth;
            double FieldHeight = Canvas1.ActualHeight;
            Brush LineColor = Brushes.BurlyWood;
            Cells = new Cell[RowNum, ColNum];
            Score = 0;
            CelWidth = (FieldWidth - (ColNum + 1) * LineThickness) / ColNum;
            CelHeight = (FieldHeight - (RowNum + 1) * LineThickness) / RowNum;
            Canvas1.Children.Clear();
            //Draw border of the canvas
            //Top
            gridLine = new Line
            {
                Stroke = LineColor,
                StrokeThickness = LineThickness,
                X1 = 0,
                Y1 = LineThickness / 2,
                X2 = FieldWidth,
                Y2 = LineThickness / 2
            };
            Canvas1.Children.Add(gridLine);
            //Right
            gridLine = new Line
            {
                Stroke = LineColor,
                StrokeThickness = LineThickness,
                X1 = FieldWidth - LineThickness / 2,
                Y1 = LineThickness / 2,
                X2 = FieldWidth - LineThickness / 2,
                Y2 = FieldHeight - LineThickness / 2
            };
            Canvas1.Children.Add(gridLine);
            //Left
            gridLine = new Line
            {
                Stroke = LineColor,
                StrokeThickness = LineThickness,
                X1 = LineThickness / 2,
                Y1 = LineThickness / 2,
                X2 = LineThickness / 2,
                Y2 = FieldHeight - LineThickness / 2
            };
            Canvas1.Children.Add(gridLine);
            //Bottom
            gridLine = new Line
            {
                Stroke = LineColor,
                StrokeThickness = LineThickness,
                X1 = 0,
                Y1 = FieldHeight - LineThickness / 2,
                X2 = FieldWidth,
                Y2 = FieldHeight - LineThickness / 2
            };
            Canvas1.Children.Add(gridLine);
            //Draw Vertical gridlines
            for (int I = 1; I < ColNum; I++)
            {
                gridLine = new Line
                {
                    Stroke = LineColor,
                    StrokeThickness = LineThickness,
                    X1 = (CelWidth + LineThickness) * I + LineThickness / 2,
                    Y1 = LineThickness / 2,
                    X2 = (CelWidth + LineThickness) * I + LineThickness / 2,
                    Y2 = FieldHeight - LineThickness / 2
                };
                Canvas1.Children.Add(gridLine);
            }
            //Draw Horizontal gridlines
            for (int I = 1; I < RowNum; I++)
            {
                gridLine = new Line
                {
                    Stroke = LineColor,
                    StrokeThickness = LineThickness,
                    X1 = LineThickness / 2,
                    Y1 = (CelHeight + LineThickness) * I + LineThickness / 2,
                    X2 = FieldWidth - LineThickness / 2,
                    Y2 = (CelHeight + LineThickness) * I + LineThickness / 2
                };
                Canvas1.Children.Add(gridLine);
            }
            //Make all the cells
            for (int I = 0; I < RowNum; I++)
            {
                for (int J = 0; J < ColNum; J++)
                {
                    Cells[I, J] = CreateCell(I, J, 0, false);
                }
            }
            //Show the initial 2 cells
            GetEmptyCell().Visible = true;
            GetEmptyCell().Visible = true;
            for (int I = 0; I < RowNum; I++)
            {
                for (int J = 0; J < ColNum; J++)
                {
                    Cells[I, J].Draw(Canvas1);
                }
            }
            GameStarted = true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            int movescore;
            int moveCount;
            if (GameStarted)
            {
                movescore = 0;
                moveCount = 0;
                switch (e.Key)
                {
                    case Key.Up:
                    {
                        //Shift all Cells to the Top and collapse equal cells
                        moveCount += MoveUp();
                        movescore += MergeUp();
                        moveCount += MoveUp();
                        break;
                    }
                    case Key.Down:
                    {
                        //Shift all Cells to the Bottom and collapse equal cells
                        moveCount += MoveDown();
                        movescore += MergeDown();
                        moveCount += MoveDown();
                        break;
                    }
                    case Key.Left:
                    {
                        //Shift all Cells to the Left and collapse equal cells
                        moveCount += MoveLeft();
                        movescore += MergeLeft();
                        moveCount += MoveLeft();
                        break;
                    }
                    case Key.Right:
                    {
                        //Shift all Cells to the Right and collapse equal cells
                        moveCount += MoveRight();
                        movescore += MergeRight();
                        moveCount += MoveRight();
                        break;
                    }
                    default:
                    {
                        e.Handled = true;
                        return;
                    }
                }
                //Check for 2048 Game Won
                for (int I = 0; I < RowNum; I++)
                {
                    for (int J = 0; J < ColNum; J++)
                    {
                        if (Cells[I, J].CellValue == 2048)
                        {
                            MessageBox.Show(this, "CONGRATULATIONS! YOU WON.\n" + "Your Score = " + Score.ToString(), "2048", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                    }
                }
                //Show a new Cell and give it the Focus
                if (moveCount + movescore > 0)
                {
                    for (int I = 0; I < RowNum; I++)
                    {
                        for (int J = 0; J < ColNum; J++)
                        {
                            Cells[I, J].SetFocus(false);
                        }
                    }
                    if (EmptyCellCount() > 0)
                    {
                        Cell c = GetEmptyCell();
                        c.Visible = true;
                        c.SetFocus(true);
                    }
                }
                Score += movescore;
                Title = "Score = " + Score.ToString();
                for (int I = 0; I < RowNum; I++)
                {
                    for (int J = 0; J < ColNum; J++)
                    {
                        Cells[I, J].Draw(Canvas1);
                    }
                }
                //Check possible moves
                if (PossibleMoves() == false)
                {
                    MessageBox.Show(this, "  GAME OVER!\n" + "Your Score = " + Score.ToString(), "2048", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    GameStarted = false;
                    return;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GameStarted = false;
            Environment.Exit(0);
        }

        #region "Cell Moves"

        private int MoveUp()
        {
            int result = 0;
            int swapcount;
            do
            {
                swapcount = 0;
                for (int J = 0; J < ColNum; J++)
                { 
                    for (int I = RowNum - 1; I > 0; I--)
                    { 
                        if (Cells[I, J].Visible == true)
                        { 
                            //check the cell above
                            if (Cells[I -1, J].Visible == false) //Empty cell above so move the cell up
                            {
                                SwapCells(Cells[I, J], Cells[I - 1, J]);
                                swapcount += 1;
                                result += 1;
                            }
                        }
                    }                   
                }   
            } while (swapcount > 0);
            return result;
        }

        private int MergeUp()
        {
            int result = 0;
            for (int J = 0; J < ColNum; J++)
            {
                for (int I = 0; I < RowNum - 1; I++)
                {
                    if (Cells[I, J].Visible == true)
                    {
                        //check the cell below
                        if (Cells[I + 1, J].Visible == true)
                        {
                            if (Cells[I, J].CellValue == Cells[I + 1, J].CellValue)
                            {
                                MergeCells(Cells[I + 1, J], Cells[I, J]);
                                result += Cells[I, J].CellValue;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private int MoveDown()
        {
            int result = 0;
            int swapcount;
            do
            {
                swapcount = 0;
                for (int J = 0; J < ColNum; J++)
                {
                    for (int I = 0; I < RowNum - 1; I++)
                    {
                        if (Cells[I, J].Visible == true)
                        {
                            //check the cell below
                            if (Cells[I + 1, J].Visible == false) //Empty cell below so move the cell down
                            {
                                SwapCells(Cells[I, J], Cells[I + 1, J]);
                                swapcount += 1;
                                result += 1;
                            }
                        }
                    }
                }
            } while (swapcount > 0);
            return result;
        }

        private int MergeDown()
        {
            int result = 0;
            for (int J = 0; J < ColNum; J++)
            {
                for (int I = RowNum - 1; I > 0; I--)
                {
                    if (Cells[I, J].Visible == true)
                    {
                        //check the cell above
                        if (Cells[I - 1, J].Visible == true)
                        {
                            if (Cells[I, J].CellValue == Cells[I - 1, J].CellValue)
                            {
                                MergeCells(Cells[I - 1, J], Cells[I, J]);
                                result += Cells[I, J].CellValue;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private int MoveLeft()
        {
            int result = 0;
            int swapcount;
            do
            {
                swapcount = 0;
                for (int I = 0; I < RowNum; I++)
                {
                    for (int J = ColNum - 1; J > 0; J--)
                    {
                        if (Cells[I, J].Visible == true)
                        {
                            //check the cell to the left
                            if (Cells[I, J - 1].Visible == false) //Empty cell to the left so move the cell left
                            {
                                SwapCells(Cells[I, J], Cells[I, J - 1]);
                                swapcount += 1;
                                result += 1;
                            }
                        }
                    }
                }
            } while (swapcount > 0);
            return result;
        }

        private int MergeLeft()
        {
            int result = 0;
            for (int I = 0; I < RowNum; I++)
            {
                for (int J = 0; J < ColNum - 1; J++)
                {
                    if (Cells[I, J].Visible == true)
                    {
                        //check the cell to the right
                        if (Cells[I, J + 1].Visible == true)
                        {
                            if (Cells[I, J].CellValue == Cells[I, J + 1].CellValue)
                            {
                                MergeCells(Cells[I, J + 1], Cells[I, J]);
                                result += Cells[I, J].CellValue;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private int MoveRight()
        {
            int result = 0;
            int swapcount;
            do
            {
                swapcount = 0;
                for (int I = 0; I < RowNum; I++)
                {
                    for (int J = 0; J < ColNum - 1; J++)
                    {
                        if (Cells[I, J].Visible == true)
                        {
                            //check the cell to the right
                            if (Cells[I, J + 1].Visible == false) //Empty cell to the right so move the cell right
                            {
                                SwapCells(Cells[I, J], Cells[I, J + 1]);
                                swapcount += 1;
                                result += 1;
                            }
                        }
                    }
                }
            } while (swapcount > 0);
            return result;
        }

        private int MergeRight()
        {
            int result = 0;
            for (int I = 0; I < RowNum; I++)
            {
                for (int J = ColNum - 1; J > 0; J--)
                {
                    if (Cells[I, J].Visible == true)
                    {
                        //check the cell to the left
                        if (Cells[I, J - 1].Visible == true)
                        {
                            if (Cells[I, J].CellValue == Cells[I, J - 1].CellValue)
                            {
                                MergeCells(Cells[I, J - 1], Cells[I, J]);
                                result += Cells[I, J].CellValue;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private void SwapCells(Cell A, Cell B)
        {
            int value = A.CellValue;
            bool vis = A.Visible;
            Brush celColor = A.BackColor;
            double TxtSize = A.TextSize;
            A.CellValue = B.CellValue;
            A.Visible = B.Visible;
            A.BackColor = B.BackColor;
            A.TextSize = B.TextSize;
            B.CellValue = value;
            B.Visible = vis;
            B.BackColor = celColor;
            B.TextSize = TxtSize;
        }

        private void MergeCells(Cell movingCell, Cell targetCell)
        {
            int value = targetCell.CellValue;
            targetCell.CellValue = 2 * value;
            targetCell.BackColor = GetCellFormat(2 * value).Color;
            targetCell.TextSize = GetCellFormat(2 * value).Size;
            if (Rnd.NextDouble() < 0.9)
            {
                value = 2;
            }
            else
            {
                value = 4;
            }
            movingCell.CellValue = value;
            movingCell.BackColor = GetCellFormat(value).Color;
            movingCell.TextSize = GetCellFormat(value).Size;
            movingCell.Visible = false;
        }

        #endregion 

        private Cell CreateCell(int row, int col, int value, bool visible)
        {
            Cell c = new Cell(CelWidth, CelHeight)
            {
                Top = LineThickness + (CelHeight + LineThickness) * row,
                Left = LineThickness + (CelWidth + LineThickness) * col
            };
            if (value == 0)
            {
                if (Rnd.NextDouble() < 0.9)
                {
                    value = 2;
                }
                else
                {
                    value = 4;
                }
            }
            c.CellValue = value;
            c.Visible = visible;
            c.BackColor = GetCellFormat(value).Color;
            c.TextSize = GetCellFormat(value).Size;
            return c;
        }

        private bool PossibleMoves()
        {
            bool result = false;
            if (EmptyCellCount() > 0) { result = true; }
            //Check for 2 identical cells side by side
            for (int I = 0; I < RowNum; I++)
            {
                for (int J = 0; J < ColNum - 1; J++)
                {
                    if (Cells[I, J].CellValue == Cells[I, J + 1].CellValue) { result = true; }
                }
            }
            //Check for 2 identical cells above each other
            for (int J = 0; J < ColNum - 1; J++)
            {
                for (int I = 0; I < RowNum - 1; I++)
                {
                    if (Cells[I, J].CellValue == Cells[I + 1, J].CellValue) { result = true; }
                }
            }
            return result;
        }

        private Cell GetEmptyCell()
        {
            List<Cell> dummy = new List<Cell>();
            int index;
            for (int I = 0; I < RowNum; I++)
            {
                for (int J = 0; J < ColNum; J++)
                {
                    if (Cells[I, J].Visible == false)
                    {
                        dummy.Add(Cells[I, J]);
                    }
                }
            }
            index = Rnd.Next(dummy.Count);
            return dummy.ElementAt(index);
        }

        private int EmptyCellCount()
        {
            int count = 0;
            for (int I = 0; I < RowNum; I++)
            {
                for (int J = 0; J < ColNum; J++)
                {
                    if (Cells[I, J].Visible == false)
                    {
                        count++;
                    }
                }
            }
            PrintVisibility();
            return count;
        }

        private void PrintVisibility()
        {
            string result = "";
            for (int I = 0; I < RowNum; I++)
            {
                result += "[";
                for (int J = 0; J < ColNum; J++)
                {
                    if (Cells[I, J].Visible == true)
                    {
                        result += "T";
                    }
                    else
                    {
                        result += "F";
                    }
                }
                result += "]";
            }
            Debug.Print(result);
        }

        private CellFormat GetCellFormat(int value)
        {
            CellFormat result;
            switch (value)
            {
                case 2:
                {
                    result.Color = Brushes.LightYellow;
                    result.Size = 72;
                    break;
                }
                case 4:
                {
                    result.Color = Brushes.LightCyan;
                    result.Size = 72;
                    break;
                }
                case 8:
                {
                    result.Color = Brushes.LightGreen;
                    result.Size = 72;
                    break;
                }
                case 16:
                {
                    result.Color = Brushes.Bisque;
                    result.Size = 72;
                    break;
                }
                case 32:
                {
                    result.Color = Brushes.Pink;
                    result.Size = 72;
                    break;
                }
                case 64:
                {
                    result.Color = Brushes.PaleTurquoise;
                    result.Size = 72;
                    break;
                }
                case 128:
                {
                    result.Color = Brushes.Cyan;
                    result.Size = 64;
                    break;
                }
                case 256:
                {
                    result.Color = Brushes.GreenYellow;
                    result.Size = 64;
                    break;
                }
                case 512:
                {
                    result.Color = Brushes.Orange;
                    result.Size = 64;
                    break;
                }
                case 1024:
                {
                    result.Color = Brushes.Red;
                    result.Size = 52;
                    break;
                }
                case 2048:
                {
                    result.Color = Brushes.Purple;
                    result.Size = 52;
                    break;
                }
                default:
                {
                    result.Color = Brushes.Black;
                    result.Size = 8;
                    break;
                }
            }
            return result;
        }
    }

    public struct CellFormat
    {
        public int Size;
        public Brush Color;
    }
}
