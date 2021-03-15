using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace CirclePacking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime LastRenderTime;
        private double RenderPeriod = 1;  //25 frames per second 
        private Random Rnd = new Random();
        private Circle c;
        private List<Circle> Circles;
        private int LoopCounter;
        private int ZeroLoopCounter;
        private bool ApplyColor = false;
        private List<Color> Rainbow;
        private double sharpness = 5.0;  //Higher values give more distinct color bands ;
        private int colorCount = 256;
        private double TotalArea;
        private double CircleArea;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Some initial code
            Circles = new List<Circle>();
            LoopCounter = 1;
            ZeroLoopCounter = 0;
            MakeRainbow();
            TotalArea = canvas1.ActualWidth * canvas1.ActualHeight;
            CircleArea = 0.0;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            while ((DateTime.Now - LastRenderTime).TotalMilliseconds < RenderPeriod) Thread.Sleep(1);
            double X;
            double Y;
            bool newCircleOK;
            bool CircleOverlap;
            int newCircleCounter;

            //Add new circles
            newCircleCounter = 0;
            for (int N = 0; N <= LoopCounter; N++)
            {
                X = canvas1.ActualWidth * Rnd.NextDouble();
                Y = canvas1.ActualHeight * Rnd.NextDouble();
                newCircleOK = true;
                c = new Circle(X, Y);
                //Check if the new circle overlaps existing circles
                for (int I = 0; I < Circles.Count; I++)
                {
                    if (c.Overlap(Circles[I], 1)) newCircleOK = false;
                }
                if (newCircleOK)
                {
                    if (ApplyColor)
                    {
                        int index = Rnd.Next(Rainbow.Count);
                        c.Shape.Stroke = new SolidColorBrush(Rainbow[index]);
                        c.Shape.Fill = new SolidColorBrush(Rainbow[index]);
                    }
                    c.Draw(canvas1);
                    Circles.Add(c);
                    newCircleCounter += 1;
                }
            }
            if (newCircleCounter == 0) //No new circles added this render pass;
            {
                if (LoopCounter < 20) LoopCounter += 1; //try adding more circles each render pass;
                ZeroLoopCounter += 1;
            }
            else
            {
                ZeroLoopCounter = 0;
            }
            if (ZeroLoopCounter == 15) //No more room available;
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                if (ApplyColor) ColorCircles();
                return;
            }
            //Grow the circles
            CircleOverlap = true;
            CircleArea = 0.0;
            for (int I = 0; I < Circles.Count; I++)
            {
                CircleOverlap = false;
                for (int J = 0; J < Circles.Count; J++)
                {
                    if (I != J)
                    {
                        if (Circles[I].Overlap(Circles[J], 0)) CircleOverlap = true;
                    }
                }
                if (!CircleOverlap)
                {
                    if (Circles[I].CanGrow())
                    {
                        Circles[I].Grow();
                    }
                }
                else
                {
                    CircleArea += Math.PI * Circles[I].Radius * Circles[I].Radius;
                }
                LastRenderTime = DateTime.Now;
            }
            TxtFill.Text = (100 * CircleArea / TotalArea).ToString("F2");
        }

        private void ColorCircles()
        {
            canvas1.Background = Brushes.Black;
            for (int I = 0; I < Circles.Count; I++)
            {
                int index = Rnd.Next(Rainbow.Count);
                Circles[I].Shape.Stroke = new SolidColorBrush(Rainbow[index]);
                Circles[I].Shape.Fill = new SolidColorBrush(Rainbow[index]);
            }
        }

        private void UnColorCircles()
        {
            canvas1.Background = Brushes.Beige;
            for (int I = 0; I < Circles.Count; I++)
            {
                Circles[I].Shape.Stroke = Brushes.Black;
                Circles[I].Shape.Fill = Brushes.Beige;
            }
        }

        private void MakeRainbow()
        {
            byte r;
            byte g;
            byte b;
            Rainbow = new List<Color>();
            //Fill the Rainbow list
            Rainbow.Clear();
            if (sharpness == 0) sharpness = 1.0 / colorCount;
            for (int I = 0; I <= colorCount; I++)
            {
                if (I < colorCount / 5) //Purple To Blue
                {
                    r = (byte)(155 * (1 - Smooth(0, colorCount / 5, I, sharpness)));
                    g = 0;
                    b = 255;
                }
                else if (I < 2 * colorCount / 5) //Blue to Cyan
                {

                    r = 0;
                    g = (byte)(255 * (Smooth(colorCount / 5, 2 * colorCount / 5, I, sharpness)));
                    b = 255;
                }
                else if (I < 3 * colorCount / 5) //Cyan to Green
                {

                    r = 0;
                    g = 255;
                    b = (byte)(255 * (1 - Smooth(2 * colorCount / 5, 3 * colorCount / 5, I, sharpness)));
                }
                else if (I < 4 * colorCount / 5) //Green to Yellow
                {

                    r = (byte)(255 * (Smooth(3 * colorCount / 5, 4 * colorCount / 5, I, sharpness)));
                    g = 255;
                    b = 0;
                }
                else //Yellow to Red
                {

                    r = 255;
                    g = (byte)(255 * (1 - Smooth(4 * colorCount / 5, colorCount, I, sharpness)));
                    b = 0;
                }
                Rainbow.Add(Color.FromRgb(r, g, b));
            }
        }

        private double Normalize(double min, double max, double X)
        {
            return (X - min) / (max - min);
        }

        private double Sigmoid(double X)
        {
            return 1 / (1 + Math.Exp(-1 * X));
        }

        private double Smooth(double min, double max, double X, double Sharpness)
        {
            double Xn = (2 * Normalize(min, max, X) - 1) * Sharpness;
            double Xmin = Sigmoid(-1 * Sharpness);
            double Xmax = Sigmoid(Sharpness);
            return Normalize(Xmin, Xmax, Sigmoid(Xn));
        }

        private void CbColor_Click(object sender, RoutedEventArgs e)
        {
            ApplyColor = CbColor.IsChecked.Value;
            if (ApplyColor)
            {
                ColorCircles();
            }
            else
            {
                UnColorCircles();
            }
        }
    }
}
