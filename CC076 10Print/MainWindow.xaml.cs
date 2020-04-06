using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;

namespace _10Print
{
    public partial class MainWindow : Window
    {
        private readonly int waitTime = 10;
        private readonly double spacing = 15.0;
        private double X = 0.0;
        private double Y = 0.0;
        private Line l;
        private readonly Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            do
            {
                if (Rnd.NextDouble() < 0.5)
                {
                    l = new Line()
                    {
                        X1 = X,
                        Y1 = Y,
                        X2 = X + spacing,
                        Y2 = Y + spacing,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                }
                else
                {
                    l = new Line()
                    {
                        X1 = X,
                        Y1 = Y + spacing,
                        X2 = X + spacing,
                        Y2 = Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                }
                canvas1.Children.Add(l);
                Dispatcher.Invoke(Wait, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                X += spacing;
                if (X > canvas1.ActualWidth)
                {
                    X = 0;
                    Y += spacing;
                }
            } while (Y < canvas1.ActualHeight);
        }

        private void Wait()
        {
            Thread.Sleep(waitTime);
        }

    }
}
