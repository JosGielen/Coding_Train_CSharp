using System;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Numerics;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace Mandel_Pi
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private int WaitTime = 1;
        private bool App_Started = false;
        private int digits = 7;
        private BigDecimal c;
        private BigDecimal eps;
        private BigDecimal z;
        private BigInteger Iterations;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Image1.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Mandelbrot.jpg"));
        }

        private void Init()
        {
            try
            {
                digits = int.Parse(TxtDigits.Text);
            }
            catch
            {
                return;
            }
            BigDecimal.Precision = digits * digits + 1;
            BigDecimal.AlwaysTruncate = true;
            BigDecimal Hundred = new BigDecimal(1, 2);
            BigDecimal exp = digits - 1;
            c = 0.25;
            eps = 1.0 / BigDecimal.Pow(Hundred, exp);
            c = c + eps;
            z = 0.0;
            Iterations = 0;
            Render();
        }


        private void Render()
        {
            while (App_Started)
            {
                for (int I = 0; I <= 2567; I++)
                {
                    if (z < 2)
                    {
                        z = z * z + c;
                        Iterations += 1;
                    }
                    else
                    {
                        TxtIters.Text = Iterations.ToString();
                        App_Started = false;
                        BtnStart.Content = "START";
                        break;
                   }
                }
                TxtIters.Text = Iterations.ToString();
                Dispatcher.Invoke(new WaitDelegate(Wait), DispatcherPriority.ApplicationIdle, WaitTime);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            if (!App_Started)
            {
                BtnStart.Content = "STOP";
                App_Started = true;
                Init();
            }
            else
            {
                App_Started = false;
                BtnStart.Content = "START";
            }
        }
    }
}
