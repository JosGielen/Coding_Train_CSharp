using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fourier_Complex_Freehand
{
    public partial class MainWindow : Window
    {
        private bool Rendering = false;
        private List<Point> Signal;
        private List<Epicycle> Epicycles;
        private List<Ellipse> Circles;
        private List<Line> Radia;
        private Ellipse dot;
        private Point newPt;
        private Polyline p;
        private double Time;
        private double TimeStep;
        private bool my_MouseDown;
        private Point previousPt;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            my_MouseDown = false;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (my_MouseDown || !Rendering) return;
            //Draw the Epicycles
            DrawEpicycles();
            //Add the new polyline point to the polyline
            p.Points.Add(newPt);
            Time += TimeStep;
            if (Time >= 2 * Math.PI)
            {
                //Reset the drawing
                Time = 0;
                p.Points.Clear();
            }
        }

        private void DrawEpicycles()
        {
            double X;
            double Y;
            double PreviousX;
            double PreviousY;
            double Freq;
            double Amp;
            double Phase;
            //Draw the Epicycles
            X = canvas1.ActualWidth / 2;
            Y = canvas1.ActualHeight / 2;
            PreviousX = X;
            PreviousY = Y;
            for (int I = 0; I < Epicycles.Count; I++)
            {
                Freq = Epicycles[I].Freqency;
                Amp = Epicycles[I].Amplitude;
                Phase = Epicycles[I].Phase;
                X += Amp * Math.Cos(Freq * Time + Phase);
                Y += Amp * Math.Sin(Freq * Time + Phase);
                Circles[I].Width = 2 * Amp;
                Circles[I].Height = 2 * Amp;
                Circles[I].SetValue(Canvas.LeftProperty, PreviousX - Amp);
                Circles[I].SetValue(Canvas.TopProperty, PreviousY - Amp);
                Radia[I].X1 = PreviousX;
                Radia[I].Y1 = PreviousY;
                Radia[I].X2 = X;
                Radia[I].Y2 = Y;
                PreviousX = X;
                PreviousY = Y;
            }
            //Final point of Epicycles.
            dot.SetValue(Canvas.LeftProperty, X - 4);
            dot.SetValue(Canvas.TopProperty, Y - 4);
            newPt = new Point(X, Y);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                canvas1.Children.Clear();
                BtnStart.Content = "STOP";
                Rendering = true;
            }
            else
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                BtnStart.Content = "START";
                Rendering = false;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Rendering) return;
            my_MouseDown = true;
            Time = 0;
            Circles = new List<Ellipse>();
            Radia = new List<Line>();
            canvas1.Children.Clear();
            Signal = new List<Point>();
            previousPt = e.GetPosition(canvas1);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Rendering) return;
            Line l;
            double X;
            double Y;
            if (my_MouseDown)
            {
                X = e.GetPosition(canvas1).X;
                Y = e.GetPosition(canvas1).Y;
                Signal.Add(new Point(X - canvas1.ActualWidth / 2, Y - canvas1.ActualHeight / 2));
                l = new Line()
                {
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1.0,
                    X1 = previousPt.X,
                    Y1 = previousPt.Y,
                    X2 = X,
                    Y2 = Y
                };
                canvas1.Children.Add(l);
                previousPt = new Point(X, Y);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Rendering) return;
            my_MouseDown = false;
            //Skip 9 out of 10 points                   
            List<Point> ReducedSignal = new List<Point>();
            for (int I = 0; I < Signal.Count; I++)
            {
                if (I % 10 == 0) ReducedSignal.Add(Signal[I]);
            }
            //Calculate the Epicycle data from the Signals with the Discrete Fourier Transform
            Epicycles = DFT.GetSortedEpicycles(ReducedSignal, false);
            //Create the Epicycle visualisation (Circles with a Radius line)
            Ellipse Ex;
            Line Lx;
            for (int I = 0; I < Epicycles.Count; I++)
            {
                Ex = new Ellipse()
                {
                    Stroke = new SolidColorBrush(Color.FromRgb(80, 80, 80)),
                    StrokeThickness = 1
                };
                Circles.Add(Ex);
                canvas1.Children.Add(Ex);
                Lx = new Line
                {
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 1
                };
                Radia.Add(Lx);
                canvas1.Children.Add(Lx);
            }
            //Create the endpoint indicator dot
            dot = new Ellipse
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                Fill = Brushes.Red,
                Width = 6,
                Height = 6
            };
            canvas1.Children.Add(dot);
            //Create the resulting drawing
            p = new Polyline()
            {
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            canvas1.Children.Add(p);
            TimeStep = 2 * Math.PI / Epicycles.Count;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

    }
}
