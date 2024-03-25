using System.Windows;
using System.Windows.Threading;

namespace Snowflake
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private int WaitTime = 1;
        private List<Particle> Snowflake;
        private int deviation;
        private double particleSize;
        private bool App_Loaded = false;
        private bool App_Started = false;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Snowflake = new List<Particle>();
            deviation = 2;
            particleSize = 3;
            sldDeviation.Value = deviation;
            TxtDeviation.Text = deviation.ToString();
            sldSize.Value = particleSize;
            TxtSize.Text = particleSize.ToString();
            App_Loaded = true;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!App_Started)
            {
                App_Started = true;
                BtnStart.Content = "STOP";
                Snowflake.Clear();
                canvas1.Children.Clear();
                Render();
            }
            else
            {
                App_Started = false;
                BtnStart.Content = "START";
            }
        }

        private void Render()
        {
            Particle p;
            bool finished;
            while (App_Started)
            { 
                p = new Particle(particleSize, canvas1.ActualWidth / 2, deviation);
                do
                {
                    finished = p.Update(Snowflake);
                } while (!finished);
                if (p.GetX() >= canvas1.ActualHeight / 2 - 20 | p.GetX() >= canvas1.ActualWidth / 2 - 20) break; //Prevent growth outside the window
                p.Show(canvas1);
                Snowflake.Add(p);
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
            }
            App_Started = false;
            BtnStart.Content = "START";
        }

            private void Wait(int t)
            {
            Thread.Sleep(t);
            }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SldDeviation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
         {
            if (App_Loaded)
            {
                deviation = (int)(sldDeviation.Value);
                TxtDeviation.Text = sldDeviation.Value.ToString();
            }
        }

        private void SldSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
         {
            if (App_Loaded)
            {
                particleSize = sldSize.Value;
                TxtSize.Text = sldSize.Value.ToString();
            }
        }
    }
}