using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Snake_Game
{
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate();
        private int WaitTime = 20;
        private Snake snake;
        private Food food;
        private int GridSize = 20;
        private double W, H;
        private int WaitCounter = 10;
        private bool AppStarted = false;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MnuNew_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            canvas1.Children.Clear();
            AppStarted = false;
            W = canvas1.ActualWidth / GridSize;
            H = canvas1.ActualHeight / GridSize;
            snake = new Snake(canvas1, W, H);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    snake.Direction = new Vector(0, -H); break;
                case Key.Down:
                    snake.Direction = new Vector(0, H); break;
                case Key.Right:
                    snake.Direction = new Vector(W, 0); break;
                case Key.Left:
                    snake.Direction = new Vector(-W, 0); break;
            }
        }

        private void MnuStart_Click(object sender, RoutedEventArgs e)
        {
            food = new Food(new Point(W * Rnd.Next(3,18), H * Rnd.Next(3,18)), W, H);
            food.Draw(canvas1);
            AppStarted = true;
            Render();
        }

        private void Render()
        {
            if (!AppStarted) return;
            WaitDelegate waitDel = new WaitDelegate(Wait);
            int counter = 0;
            do
            {
                counter++;
                if( counter % WaitCounter == 0 )
                {
                    if ((snake.Location - food.Location).Length < (W + H) / 4)
                    {
                        snake.Grow();
                        food.UpdateLocation(new Point(W * Rnd.Next(3, 18), H * Rnd.Next(3, 18)));
                    }
                    snake.Move();
                    snake.UpdateLocation();
                    if (!snake.CheckValid())
                    {
                        //End Game

                    }
                    if (counter % 1000 == 0) 
                    {
                        WaitTime -= 2;
                        if (WaitTime < 5) WaitTime = 5;
                    }
                }
                Title = "Snake length = " + snake.Length.ToString();
                Dispatcher.Invoke(waitDel, DispatcherPriority.ApplicationIdle);
            } while(true);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Init();
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }
    }
}