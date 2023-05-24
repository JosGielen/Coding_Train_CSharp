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

namespace Fourier_XY
{
    public partial class MainWindow : Window
    {
        private bool Rendering = false;
        private List<double> SignalX;
        private List<double> SignalY;
        private List<Epicycle> EpicyclesX;
        private List<Epicycle> EpicyclesY;
        private List<Ellipse> CirclesX;
        private List<Ellipse> CirclesY;
        private List<Line> RadiaX;
        private List<Line> RadiaY;
        private Ellipse dotX;
        private Ellipse dotY;
        private Line HLine;
        private Line VLine;
        private Point PtX;
        private Point PtY;
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
            SignalX = new List<double>();
            SignalY = new List<double>();
            CirclesX = new List<Ellipse>();
            CirclesY = new List<Ellipse>();
            RadiaX = new List<Line>();
            RadiaY = new List<Line>();
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
                    SignalX.Add(double.Parse(fields[0]));
                    SignalY.Add(Double.Parse(fields[1]));
                    SkipCounter = 0;
                }
            }
            //Calculate the Epicycle data from the Signals with the Discrete Fourier Transform
            EpicyclesX = DFT.GetSortedEpicycles(SignalX, false);
            EpicyclesY = DFT.GetSortedEpicycles(SignalY, false);
            //Create the Epicycle visualisation (Circles with a Radius line)
            Ellipse Ex;
            Line Lx;
            for (int I = 0; I < EpicyclesX.Count; I++)
            {
                Ex = new Ellipse()
                {
                    Stroke = new SolidColorBrush(Color.FromRgb(80, 80, 80)),
                    StrokeThickness = 1
                };
                CirclesX.Add(Ex);
                canvas1.Children.Add(Ex);
                Lx = new Line
                {
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 1
                };
                RadiaX.Add(Lx);
                canvas1.Children.Add(Lx);
            }
            Ellipse Ey;
            Line Ly;
            for (int I = 0; I < EpicyclesY.Count; I++)
            {
                Ey = new Ellipse()
                {
                    Stroke = new SolidColorBrush(Color.FromRgb(80, 80, 80)),
                    StrokeThickness = 1
                };
                CirclesY.Add(Ey);
                canvas1.Children.Add(Ey);
                Ly = new Line
                {
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 1
                };
                RadiaY.Add(Ly);
                canvas1.Children.Add(Ly);
            }
            //Create the endpoint indicator dot
            dotX = new Ellipse
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                Fill = Brushes.Red,
                Width = 6,
                Height = 6
            };
            canvas1.Children.Add(dotX);
            dotY = new Ellipse
            {
                Stroke = Brushes.DarkGray,
                StrokeThickness = 1,
                Fill = Brushes.Yellow,
                Width = 8,
                Height = 8
            };
            canvas1.Children.Add(dotY);
            //Create the drawing lines
            HLine = new Line()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };
            canvas1.Children.Add(HLine);
            VLine = new Line()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };
            canvas1.Children.Add(VLine);
            //Create the resulting drawing
            p = new Polyline()
            {
                Stroke = Brushes.White,
                StrokeThickness = 1
            };
            canvas1.Children.Add(p);
            TimeStep = 2 * Math.PI / EpicyclesX.Count;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //Draw the Epicycles
            DrawEpicycles();
            //Draw a horizontal line from the EpicyclesY endpoint to the new polyline point
            HLine.X1 = PtY.X;
            HLine.Y1 = PtY.Y;
            HLine.X2 = newPt.X;
            HLine.Y2 = newPt.Y;
            //Draw a Vertical line from the EpicyclesX endpoint to the new polyline point
            VLine.X1 = PtX.X;
            VLine.Y1 = PtX.Y;
            VLine.X2 = newPt.X;
            VLine.Y2 = newPt.Y;
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
            //Draw the EpicycleX
            X = 500;
            Y = 75;
            PreviousX = X;
            PreviousY = Y;
            for (int I = 0; I < EpicyclesX.Count; I++)
            {
                Freq = EpicyclesX[I].Freqency;
                Amp = EpicyclesX[I].Amplitude;
                Phase = EpicyclesX[I].Phase;
                X += Amp * Math.Cos(Freq * Time + Phase);
                Y += Amp * Math.Sin(Freq * Time + Phase);
                CirclesX[I].Width = 2 * Amp;
                CirclesX[I].Height = 2 * Amp;
                CirclesX[I].SetValue(Canvas.LeftProperty, PreviousX - Amp);
                CirclesX[I].SetValue(Canvas.TopProperty, PreviousY - Amp);
                RadiaX[I].X1 = PreviousX;
                RadiaX[I].Y1 = PreviousY;
                RadiaX[I].X2 = X;
                RadiaX[I].Y2 = Y;
                PreviousX = X;
                PreviousY = Y;
            }
            //Final point of EpicycleX.
            dotX.SetValue(Canvas.LeftProperty, X - 4);
            dotX.SetValue(Canvas.TopProperty, Y - 4);
            PtX = new Point(X, Y);
            //Draw the EpicycleY
            X = 125;
            Y = canvas1.ActualHeight / 2 + 50;
            PreviousX = X;
            PreviousY = Y;
            for (int I = 0; I < EpicyclesY.Count; I++)
            {
                Freq = EpicyclesY[I].Freqency;
                Amp = EpicyclesY[I].Amplitude;
                Phase = EpicyclesY[I].Phase;
                X += Amp * Math.Cos(Freq * Time + Phase + Math.PI / 2);
                Y += Amp * Math.Sin(Freq * Time + Phase + Math.PI / 2);
                CirclesY[I].Width = 2 * Amp;
                CirclesY[I].Height = 2 * Amp;
                CirclesY[I].SetValue(Canvas.LeftProperty, PreviousX - Amp);
                CirclesY[I].SetValue(Canvas.TopProperty, PreviousY - Amp);
                RadiaY[I].X1 = PreviousX;
                RadiaY[I].Y1 = PreviousY;
                RadiaY[I].X2 = X;
                RadiaY[I].Y2 = Y;
                PreviousX = X;
                PreviousY = Y;
            }
            //Final point of EpicycleY.
            dotY.SetValue(Canvas.LeftProperty, X - 4);
            dotY.SetValue(Canvas.TopProperty, Y - 4);
            PtY = new Point(X, Y);
            //get the new polyline point as the intersection of both X and Y epicycles endpoints
            newPt.X = PtX.X;
            newPt.Y = PtY.Y;
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
