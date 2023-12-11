using System.Windows;
using System.Windows.Media;

namespace Maze_Generator
{
    public partial class MainWindow : Window
    {
        private int WaitTime = 0;
        private double Size = 15.0;
        private int Rows = 0;
        private int Cols = 0;
        private Cell[,] Grid;
        private Cell CurrentCell;
        private Cell NextCell;
        private List<Cell> CellStack;
        private Random Rnd = new Random();
        private bool AllowRandomRemoval = false;
        private bool Rendering;

        public MainWindow()
        {
            InitializeComponent();
            Grid = new Cell[Rows, Cols];
            CellStack = new List<Cell>();
            CurrentCell = new Cell(0, 0, 1);
            NextCell = new Cell(0,0,1);
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Rows = (int)Math.Floor(canvas1.ActualHeight / Size);
            Cols = (int)Math.Floor(canvas1.ActualWidth / Size);
            Grid = new Cell[Rows, Cols];
            CellStack = new List<Cell>();
            for (int I = 0; I < Rows; I++)
            {
                for (int J = 0; J < Cols; J++)
                {
                    Grid[I, J] = new Cell(I, J, Size);
                    Grid[I, J].Draw(canvas1);
                }
            }
            CurrentCell = Grid[0, 0];
            CurrentCell.IsCurrent = true;
            CellStack.Add(CurrentCell);
            StartRender();
        }

        private void StartRender()
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            Rendering = true;
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            //Generate the maze
            if (!Rendering) return;
            Wait();
            CurrentCell.IsVisited = true;
            NextCell = GetUnvisitedNeighbour(CurrentCell);
            if (NextCell != null)
            {
                RemoveWalls(CurrentCell, NextCell);
                CurrentCell.IsCurrent = false;
                CurrentCell = NextCell;
                CellStack.Add(CurrentCell);
            }
            else
            {
                if (CellStack.Count > 0)
                {
                    CurrentCell.IsCurrent = false;
                    CurrentCell = CellStack.Last();
                    CellStack.RemoveAt(CellStack.Count - 1);
                }
                else
                {
                    return;
                }
            }
            CurrentCell.IsCurrent = true;
        }

        private Cell GetUnvisitedNeighbour(Cell c)
        {
            List<Cell> Neighbours = new List<Cell>();
            int index;
            if (c.Col > 0)
            {
                if (!Grid[c.Row, c.Col - 1].IsVisited) Neighbours.Add(Grid[c.Row, c.Col - 1]);
            }
            if (c.Col < Cols - 1)
            {
                if (!Grid[c.Row, c.Col + 1].IsVisited) Neighbours.Add(Grid[c.Row, c.Col + 1]);
            }
            if (c.Row > 0)
            {
                if (!Grid[c.Row - 1, c.Col].IsVisited) Neighbours.Add(Grid[c.Row - 1, c.Col]);
            }
            if (c.Row < Rows - 1)
            {
                if (!Grid[c.Row + 1, c.Col].IsVisited) Neighbours.Add(Grid[c.Row + 1, c.Col]);
            }
            if (Neighbours.Count == 0)
            {
                return null;
            }
            else
            {
                index = Rnd.Next(Neighbours.Count);
                return Neighbours[index];
            }
        }

        private void RemoveWalls(Cell Cell1, Cell Cell2)
        {
            if (Cell1.Row > Cell2.Row)
            {
                Cell1.RemoveTopWall();
                Cell2.RemoveBottomWall();
            }
            else if (Cell1.Row < Cell2.Row)
            {
                Cell1.RemoveBottomWall();
                Cell2.RemoveTopWall();
            }
            else if (Cell1.Col < Cell2.Col)
            {
                Cell1.RemoveRightWall();
                Cell2.RemoveLeftWall();
            }
            else if (Cell1.Col > Cell2.Col)
            {
                Cell1.RemoveLeftWall();
                Cell2.RemoveRightWall();
            }
            if (AllowRandomRemoval)
            {
                if (100 * Rnd.NextDouble() < 5)
                {
                    Cell1.RemoveRightWall();
                    Cell2.RemoveLeftWall();
                }
                if (100 * Rnd.NextDouble() < 5)
                {
                    Cell1.RemoveTopWall();
                    Cell2.RemoveBottomWall();
                }
            }
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }
    }
}