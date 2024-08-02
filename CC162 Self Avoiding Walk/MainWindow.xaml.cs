using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Self_Avoiding_Walk
{
    public partial class MainWindow : Window
    {
        private Cell[,] cells;
        private List<Cell> cellList;
        private int stepSize = 80;
        private int Cols;
        private int Rows;
        private bool Solved;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Cols = (int)Math.Floor(canvas1.ActualWidth / stepSize);
            Rows = (int)Math.Floor(canvas1.ActualHeight / stepSize);
            cells = new Cell[Cols, Rows];
            cellList = new List<Cell>();
            Solved = false;
            //Create all cells unused
            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    cells[i, j] = new Cell(i, j);
                    cells[i, j].Draw(canvas1, stepSize);
                }
            }
            //Check the available neightbours
            List<int> free;
            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    free = cells[i, j].FreeNeighbours(cells);
                }
            }
            //Start in the middle
            cellList.Add(cells[Cols / 2, Rows / 2]);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (Solved) { return; }
            Cell last = cellList.Last();
            last.Used = true;
            Cell next = null;
            List<int> FreeDirs = last.FreeNeighbours(cells);
            if (FreeDirs.Count > 0)
            {
                int free = FreeDirs[Rnd.Next(FreeDirs.Count)];
                if (free == 0) //Left
                {
                    next = cells[last.Col - 1, last.Row];
                }
                if (free == 1) //Right
                {
                    next = cells[last.Col + 1, last.Row];
                }
                if (free == 2) //Top
                {
                    next = cells[last.Col, last.Row - 1];
                }
                if (free == 3) //Bottom
                {
                    next = cells[last.Col, last.Row + 1];
                }
                if (next != null)
                {
                    Line l = new Line()
                    {
                        X1 = (last.Col + 0.5) * stepSize,
                        Y1 = (last.Row + 0.5) * stepSize,
                        X2 = (next.Col + 0.5) * stepSize,
                        Y2 = (next.Row + 0.5) * stepSize,
                        Stroke = Brushes.Black,
                        StrokeThickness = 4.0
                    };
                    canvas1.Children.Add(l);
                    cellList.Add(next);
                    last.SetTried(free);
                    next.Used = true;
                    //Check solved
                    bool AllUsed = true;
                    for (int i = 0; i < Cols; i++)
                    {
                        for (int j = 0; j < Rows; j++)
                        {
                            if (!cells[i, j].Used)
                            {
                                AllUsed = false;
                                break;
                            }
                        }
                    }
                    if (AllUsed == true)
                    {
                        Solved = true;
                        CompositionTarget.Rendering -= CompositionTarget_Rendering;
                    }
                }
            }
            else
            {
                cellList.Last().Used = false;
                cellList.Last().UnTried();
                cellList.RemoveAt(cellList.Count - 1);
                canvas1.Children.RemoveAt(canvas1.Children.Count - 1);

            }
            Thread.Sleep(10);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}