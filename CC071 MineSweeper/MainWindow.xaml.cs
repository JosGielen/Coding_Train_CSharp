using System.Windows;

namespace MineSweeper
{
    public partial class MainWindow : Window
    {
        private Cell[,] grid;
        private int cols;
        private int rows;
        private double CellSize;
        private int BombCount;
        public bool GameOver;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Start the game at 9 x 9 size
            cols = 9;
            rows = 9;
            CellSize = 25;
            BombCount = 10;
            Canvas1.Width = CellSize * cols;
            Canvas1.Height = CellSize * rows;
            Init();
        }

        private void Init()
        {
            //Make all the cells
            Canvas1.Children.Clear();
            Cell c;
            grid = new Cell[cols, rows];
            for (int I = 0; I < cols; I++)
            {
                for (int J = 0; J < rows; J++)
                {
                    c = new Cell(I, J, false, CellSize, this);
                    grid[I, J] = c;
                    c.Show(Canvas1);
                }
            }
            GameOver = false;
            Canvas1.UpdateLayout();
            //Add the bombs
            int X = 0;
            int Y = 0;
            for (int I = 0; I < BombCount; I++)
            {
                do
                {
                    X = Rnd.Next(cols);
                    Y = Rnd.Next(rows);
                } while (grid[X, Y].GotBomb);
                grid[X, Y].GotBomb = true;
            }
            //Get the number of Neighbours with bombs
            int N = 0;
            for (int I = 0; I < cols; I++)
            {
                for (int J = 0; J < rows; J++)
                {
                    N = 0;
                    if (I > 0)
                    {
                        if (J > 0)
                        {
                            if (grid[I - 1, J - 1].GotBomb) N += 1;
                        }
                        if (grid[I - 1, J].GotBomb) N += 1;
                        if (J < rows - 1)
                        {
                            if (grid[I - 1, J + 1].GotBomb) N += 1;
                        }
                    }
                    if (J > 0)
                    {
                        if (grid[I, J - 1].GotBomb) N += 1;
                    }
                    if (J < rows - 1)
                    {
                        if (grid[I, J + 1].GotBomb) N += 1;
                    }
                    if (I < cols - 1)
                    {
                        if (J > 0)
                        {
                            if (grid[I + 1, J - 1].GotBomb) N += 1;
                        }
                        if (grid[I + 1, J].GotBomb) N += 1;
                        if (J < rows - 1)
                        {
                            if (grid[I + 1, J + 1].GotBomb) N += 1;
                        }
                    }
                    grid[I, J].BombNeighbours = N;
                }
            }
        }

        public void RevealNeighbours(int X, int Y)
        {
            if (X > 0)
            {
                if (Y > 0)
                {
                    if (!grid[X - 1, Y - 1].GotBomb & !grid[X - 1, Y - 1].Revealed)
                    {
                        grid[X - 1, Y - 1].Reveal();
                        if (grid[X - 1, Y - 1].BombNeighbours == 0) RevealNeighbours(X - 1, Y - 1);
                    }
                }
                if (!grid[X - 1, Y].GotBomb & !grid[X - 1, Y].Revealed)
                {
                    grid[X - 1, Y].Reveal();
                    if (grid[X - 1, Y].BombNeighbours == 0) RevealNeighbours(X - 1, Y);
                }
                if (Y < rows - 1)
                {
                    if (!grid[X - 1, Y + 1].GotBomb & !grid[X - 1, Y + 1].Revealed)
                    {
                        grid[X - 1, Y + 1].Reveal();
                        if (grid[X - 1, Y + 1].BombNeighbours == 0) RevealNeighbours(X - 1, Y + 1);
                    }
                }
            }
            if (Y > 0)
            {
                if (!grid[X, Y - 1].GotBomb & !grid[X, Y - 1].Revealed)
                {
                    grid[X, Y - 1].Reveal();
                    if (grid[X, Y - 1].BombNeighbours == 0) RevealNeighbours(X, Y - 1);
                }
            }
            if (Y < rows - 1)
            {
                if (!grid[X, Y + 1].GotBomb & !grid[X, Y + 1].Revealed)
                {
                    grid[X, Y + 1].Reveal();
                    if (grid[X, Y + 1].BombNeighbours == 0) RevealNeighbours(X, Y + 1);
                }
            }
            if (X < cols - 1)
            {
                if (Y > 0)
                {
                    if (!grid[X + 1, Y - 1].GotBomb & !grid[X + 1, Y - 1].Revealed)
                    {
                        grid[X + 1, Y - 1].Reveal();
                        if (grid[X + 1, Y - 1].BombNeighbours == 0) RevealNeighbours(X + 1, Y - 1);
                    }
                }
                if (!grid[X + 1, Y].GotBomb & !grid[X + 1, Y].Revealed)
                {
                    grid[X + 1, Y].Reveal();
                    if (grid[X + 1, Y].BombNeighbours == 0) RevealNeighbours(X + 1, Y);
                }
                if (Y < rows - 1)
                {
                    if (!grid[X + 1, Y + 1].GotBomb & !grid[X + 1, Y + 1].Revealed)
                    {
                        grid[X + 1, Y + 1].Reveal();
                        if (grid[X + 1, Y + 1].BombNeighbours == 0) RevealNeighbours(X + 1, Y + 1);
                    }
                }
            }
        }

        public void CheckWinning()
        {
            bool allDone = true;
            for (int I = 0; I < cols; I++)
            {
                for (int J = 0; J < rows; J++)
                {
                    if (!grid[I, J].Revealed & !grid[I, J].GotBomb)
                    {
                        allDone = false;
                        break;
                    }
                }
            }
            if (allDone)
            {
                for (int I = 0; I < cols; I++)
                {
                    for (int J = 0; J < rows; J++)
                    {
                        grid[I, J].ShowDefused();
                    }
                }
                GameOver = true;
                MessageBox.Show("Congratulations\nYou Won!", "Minesweeper Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void RevealAll()
        {
            for (int I = 0; I < cols; I++)
            {
                for (int J = 0; J < rows; J++)
                {
                    grid[I, J].Reveal();
                }
            }
            GameOver = true;
            MessageBox.Show("Game Over\n\nYou Lost!", "Minesweeper Result", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuNewGame_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Mnu9x9_Click(object sender, RoutedEventArgs e)
        {
            cols = 9;
            rows = 9;
            BombCount = 10;
            Canvas1.Width = CellSize * cols;
            Canvas1.Height = CellSize * rows;
            Init();
        }

        private void Mnu15x15_Click(object sender, RoutedEventArgs e)
        {
            cols = 15;
            rows = 15;
            BombCount = 25;
            Canvas1.Width = CellSize * cols;
            Canvas1.Height = CellSize * rows;
            Init();
        }

        private void Mnu20x20_Click(object sender, RoutedEventArgs e)
        {
            cols = 20;
            rows = 20;
            BombCount = 50;
            Canvas1.Width = CellSize * cols;
            Canvas1.Height = CellSize * rows;
            Init();
        }
    }
}