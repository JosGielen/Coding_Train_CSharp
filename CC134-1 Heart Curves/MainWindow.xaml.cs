using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace HeartCurve
{
    public partial class MainWindow : Window
    {
        private delegate void DrawDelegate();
        public delegate void WaitDelegate(int t);
        private bool Rendering;
        private int WaitTime;
        private Random Rnd = new Random();
        private PointCollection points;
        private List<Polygon> polys;
        private int total;
        private ScaleTransform ST;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "CLICK TO START";
            WaitTime = 300;
            total = 20;
            Rendering = false;
            //WindowState = WindowState.Maximized;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!Rendering)
            {
                Init();
                Rendering = true;
                Render();
            }
            else
            {
                Rendering = false;
            }
        }

        private void Init()
        {
            double X;
            double Y;
            double t = 0.0;
            Polygon p;
            canvas1.Children.Clear();
            points = new PointCollection();
            polys = new List<Polygon>();
            while (true)
            {
                t += 0.05;
                X = 10 * (16 * Math.Pow(Math.Sin(t), 3));
                Y = 10 * (13 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t));
                points.Add(new Point(X, 10 - Y));
                if (t > 2 * Math.PI) break;
            }
            for (int I = 0; I <= total; I++)
            {
                p = new Polygon()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = Brushes.Black,
                    Points = points
                };
                polys.Add(p);
                canvas1.Children.Add(p);
            }
        }

        private void Render()
        {
            if (!Rendering) return;
            double scale;
            double Xpos;
            double Ypos;
            while (true)
            {
                foreach (Polygon p in polys)
                {
                    scale = 0.4 * Rnd.NextDouble() + 0.1;
                    Xpos = canvas1.ActualWidth * (0.8 * Rnd.NextDouble() + 0.1);
                    Ypos = canvas1.ActualHeight * (0.8 * Rnd.NextDouble() + 0.1);
                    p.SetValue(Canvas.LeftProperty, Xpos);
                    p.SetValue(Canvas.TopProperty, Ypos);
                    p.Fill = Brushes.Red;
                    ST = new ScaleTransform(scale, scale);
                    p.RenderTransform = ST;
                    Dispatcher.Invoke(new WaitDelegate(Wait), DispatcherPriority.ApplicationIdle, WaitTime);
                }
                if (!Rendering)
                {
                    return;
                }
            }
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }


    }
}
