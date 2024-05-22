using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        private double increase = 0.1;
        private bool ApplyColor = false;
        private BitmapImage bitmap;
        private int Stride;
        private byte[] PixelData;
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
            TotalArea = canvas1.ActualWidth * canvas1.ActualHeight;
            CircleArea = 0.0;
            bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\dog.jpg"));
            Stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            PixelData = new byte[Stride * bitmap.PixelHeight];
            bitmap.CopyPixels(PixelData, Stride, 0);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
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
                        c.Shape.Stroke = new SolidColorBrush(getPixelColor((int)X, (int)Y));
                        c.Shape.Fill = new SolidColorBrush(getPixelColor((int)X, (int)Y));
                    }
                    c.Draw(canvas2);
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
                        Circles[I].Grow(increase);
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
            canvas2.Background = Brushes.Black;
            for (int I = 0; I < Circles.Count; I++)
            {
                Circles[I].Shape.Stroke = new SolidColorBrush(getPixelColor((int)Circles[I].Center.X, (int)Circles[I].Center.Y));
                Circles[I].Shape.Fill = new SolidColorBrush(getPixelColor((int)Circles[I].Center.X, (int)Circles[I].Center.Y));
            }
        }

        private void UnColorCircles()
        {
            canvas2.Background = Brushes.Beige;
            for (int I = 0; I < Circles.Count; I++)
            {
                Circles[I].Shape.Stroke = Brushes.Black;
                Circles[I].Shape.Fill = Brushes.Beige;
            }
        }

        private Color getPixelColor(int x, int y)
        {
            int index = (y * bitmap.PixelWidth + x) * bitmap.Format.BitsPerPixel / 8;
            return Color.FromRgb(PixelData[index + 2], PixelData[index + 1], PixelData[index]);
        }

        //private double Normalize(double min, double max, double X)
        //{
        //    return (X - min) / (max - min);
        //}

        //private double Sigmoid(double X)
        //{
        //    return 1 / (1 + Math.Exp(-1 * X));
        //}

        //private double Smooth(double min, double max, double X, double Sharpness)
        //{
        //    double Xn = (2 * Normalize(min, max, X) - 1) * Sharpness;
        //    double Xmin = Sigmoid(-1 * Sharpness);
        //    double Xmax = Sigmoid(Sharpness);
        //    return Normalize(Xmin, Xmax, Sigmoid(Xn));
        //}

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
