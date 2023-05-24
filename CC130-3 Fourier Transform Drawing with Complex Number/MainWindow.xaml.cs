using System;
using System.Collections.Generic;
using System.IO;
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

namespace Fourier_Complex
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Signal = new List<Point>();
            Circles = new List<Ellipse>();
            Radia = new List<Line>();
            //Read the Logo from the file into the Signal lists.
            string[] fields;
            int SkipCounter = 0;
            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\CodingTrainLogo.txt");
            while (!sr.EndOfStream)
            {
                fields = sr.ReadLine().Split(';');
                SkipCounter += 1;
                if (SkipCounter == 10)
                {
                    //Skip 9 out of 10 points;
                    Signal.Add(new Point(double.Parse(fields[0]), double.Parse(fields[1])));
                    SkipCounter = 0;
                }
            }
            //Calculate the Epicycle data from the Signals with the Discrete Fourier Transform
            Epicycles = DFT.GetSortedEpicycles(Signal, false);
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
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
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
                CompositionTarget.Rendering += CompositionTarget_Rendering;
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
    }
}
