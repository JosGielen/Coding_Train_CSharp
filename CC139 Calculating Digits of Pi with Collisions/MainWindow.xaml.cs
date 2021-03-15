using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CollisionPi
{
    public partial class MainWindow : Window
    {
        private int WaitTime = 10;
        private bool Rendering = false;
        private Block LeftBlock;
        private Block RightBlock;
        private int Collisions = 0;
        private int TimeStep;
        private int Digits = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            canvas1.Children.Clear();
            Collisions = 0;
            Digits = int.Parse(txtDigits.Text);
            TimeStep = (int)Math.Pow(10, Digits + 1);
            LeftBlock = new Block(canvas1.ActualWidth / 3, 50, 0, 1, Brushes.Red );
            RightBlock = new Block(2 * canvas1.ActualWidth / 3, 50, 2.0 / TimeStep, Math.Pow(100, Digits - 1), Brushes.Blue);
            TxtWeight.Text = Math.Pow(100, Digits - 1).ToString("F0") + " kg";
            LeftBlock.Draw(canvas1);
            RightBlock.Draw(canvas1);
            Line L = new Line()
            {
                X1 = 0.0,
                X2 = 0.0,
                Y1 = 0.0,
                Y2 = canvas1.ActualHeight,
                Stroke=Brushes.Black,
                StrokeThickness=4.0,
            };
            canvas1.Children.Add(L);
            L = new Line()
            {
                X1 = 0.0,
                X2 = canvas1.ActualWidth,
                Y1 = canvas1.ActualHeight,
                Y2 = canvas1.ActualHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 4.0,
            };
            canvas1.Children.Add(L);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                Init();
                BtnStart.Content = "STOP";
                Rendering = true;
                Render();
            }
            else
            {
                BtnStart.Content = "START";
                Rendering = false;
            }
        }

        private void Render()
        {
            while (Rendering)
            {
                //collided = False;
                for (int I = 0; I < TimeStep; I++)
                {
                    //Update the block positions
                    LeftBlock.X -= LeftBlock.Speed; //Positive speed reduces X;
                    RightBlock.X -= RightBlock.Speed;
                    //Check for block collision
                    if (LeftBlock.X + LeftBlock.Size >= RightBlock.X)
                    {
                        Collision();
                        Collisions += 1;
                        //collided = True;
                    }
                    //Check for wall collision
                    if (LeftBlock.X <= 0)
                    {
                        LeftBlock.X = 0;
                        LeftBlock.Speed = -1 * LeftBlock.Speed;
                        Collisions += 1;
                        //collided = True;
                    }
                }
                LeftBlock.Update();
                RightBlock.Update();
                txtCollisions.Text = Collisions.ToString();
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
            }
        }

        private void Collision()
        {
            double newV1 = LeftBlock.Speed * (LeftBlock.Mass - RightBlock.Mass) / (LeftBlock.Mass + RightBlock.Mass) + 2 * RightBlock.Mass * RightBlock.Speed / (LeftBlock.Mass + RightBlock.Mass);
            double newV2 = 2 * LeftBlock.Mass * LeftBlock.Speed / (LeftBlock.Mass + RightBlock.Mass) + RightBlock.Speed * (RightBlock.Mass - LeftBlock.Mass) / (LeftBlock.Mass + RightBlock.Mass);
            LeftBlock.Speed = newV1;
            RightBlock.Speed = newV2;
        }

        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }
    }
}
