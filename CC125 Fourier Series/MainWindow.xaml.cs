using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Fourier_Y
{
    public partial class MainWindow : Window
    {
        private bool Rendering = false;
        private List<double> Wave;
        private double radius = 80;
        private Point center;
        private double startX;
        private double X;
        private double Y;
        private int K = 5;
        private double DeltaX;
        private double Time;
        private double TimeStep;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Wave = new List<double>();
            radius = 80;
            center = new Point(2.5 * radius, canvas1.ActualHeight / 2);
            startX = 5.5 * radius;
            TimeStep = 0.02;
            sldIterations.Value = K;
            TxtIterations.Text = K.ToString();
            DeltaX = (canvas1.ActualWidth - startX) / (6 * Math.PI / TimeStep);
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            int N = 0;
            X = center.X;
            Y = center.Y;
            double PreviousX = center.X;
            double PreviousY = center.Y;
            double R;
            Ellipse El;
            Line cl;
            Ellipse dot;
            canvas1.Children.Clear();
            for (int I = 0; I < K; I++)
            {
                N = 2 * I + 1;
                R = radius * 4 / (N * Math.PI);
                X += R * Math.Cos(N * Time);
                Y += R * Math.Sin(N * Time);
                El = new Ellipse() //Epicycle;
                {
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1,
                    Width = 2 * R,
                    Height = 2 * R
                };
                El.SetValue(Canvas.LeftProperty, PreviousX - R);
                El.SetValue(Canvas.TopProperty, PreviousY - R);
                canvas1.Children.Add(El);
                cl = new Line() //Radius line of the Epicycle;
                {
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 1,
                    X1 = PreviousX,
                    Y1 = PreviousY,
                    X2 = X,
                    Y2 = Y
                };
                canvas1.Children.Add(cl);
                PreviousX = X;
                PreviousY = Y;
            }
            Wave.Insert(0, Y);
            //Final point that draws the wave.
            dot = new Ellipse()
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 1,
                Fill = Brushes.Yellow,
                Width = 8,
                Height = 8
            };
            dot.SetValue(Canvas.LeftProperty, X - 4);
            dot.SetValue(Canvas.TopProperty, Y - 4);
            canvas1.Children.Add(dot);
            Polyline p = new Polyline()
            {
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            for (int I = Wave.Count - 1; I >= 0; I--)
            {
                p.Points.Add(new Point(startX + I * DeltaX, Wave[I]));
            }
            canvas1.Children.Add(p);
            Line l2 = new Line()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                X1 = X,
                Y1 = Y,
                X2 = startX,
                Y2 = Y
            };
            canvas1.Children.Add(l2);
            if (Wave.Count > 6 * Math.PI / TimeStep)
            {
                Wave.Remove(Wave.Last());
            }
            Time += TimeStep;

        }

        private void sldIterations_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            K = (int)sldIterations.Value;
            TxtIterations.Text = K.ToString();
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                BtnStart.Content = "Stop";
                Rendering = true;
            }
            else
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                Rendering = false;
            }
        }
    }
}
