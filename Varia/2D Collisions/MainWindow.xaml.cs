using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace _2DCollisions
{
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate();
        private int WaitTime = 10;
        private bool Rendering = false;
        private List<Ball> balls;
        private int ballCount = 25;
        private int minBallSize = 10;
        private int maxBallSize = 30;
        private Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            balls = new List<Ball>();
            Ball B;
            Point Pos;
            double Angle;
            double Size;
            double Mass;
            double Speed;
            do
            {
                Size = (maxBallSize - minBallSize) * rnd.NextDouble() + minBallSize ;
                Mass = Size / 4;
                Speed = 0.5 * rnd.NextDouble() + 0.5;
                Angle = 360 * rnd.NextDouble();
                Pos = new Point((canvas1.ActualWidth - 2 * Size) * rnd.NextDouble() + Size, (canvas1.ActualHeight - 2 * Size) * rnd.NextDouble() + Size);
                bool IsOK = true;
                //Make sure no 2 balls overlap
                for (int I = 0; I < balls.Count; I++)
                {
                    if ((Pos - balls[I].Pos).Length <= Size + balls[I].Radius)
                    {
                        IsOK = false;
                        break;
                    }
                }
                if (IsOK)
                {
                    B = new Ball(Pos, Angle, Size, Mass, Speed);
                    B.Color = new SolidColorBrush(Color.FromRgb((byte)(8 * Size), (byte)(8 * Size), (byte)(8 * Size)));
                    balls.Add(B);
                    B.Show(canvas1);
                }
            } while (balls.Count < ballCount);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                Rendering = true;
                BtnStart.Content = "STOP";
                Render();
            }
            else
            {
                Rendering = false;
                BtnStart.Content = "START";
            }
        }

        private void Render()
        {
            WaitDelegate waitDel = new WaitDelegate(Wait);
            while (Rendering)
            {
                for (int I = 0; I < balls.Count(); I++)
                {
                    balls[I].Update();
                    for (int J = I + 1; J < balls.Count(); J++)
                    {
                        balls[I].Collide(balls[J]);
                    }
                }
                double Energy = 0.0;
                for (int I = 0; I < balls.Count(); I++)
                {
                    Energy += 0.5 * balls[I].Mass * Math.Pow(balls[I].Speed.Length, 2);
                }
                TxtEnergy.Text = Energy.ToString("F4");
                Dispatcher.Invoke(waitDel, DispatcherPriority.ApplicationIdle);
            }
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }
    }
}
