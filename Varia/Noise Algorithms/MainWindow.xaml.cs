using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Perlin_Noise
{
    public partial class MainWindow : Window
    {
        private readonly int WaitTime = 10;
        private List<Color> Rainbow;
        private readonly int colorCount = 256;
        private double XOff = 0.0;
        private double YOff = 0.0;
        private double ZOff = 0.0;
        private double WOff = 0.0;
        private int Octaves = 1;
        private double Persistence = 1.0;
        private bool App_Started = false;
        private List<Line> lines;
        private readonly int factor = 1;
        private readonly Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ColorPalette cp = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            Rainbow = cp.GetColors(colorCount);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!App_Started)
            {
                App_Started = true;
                BtnStart.Content = "Stop";
                Init(sender, e);
            }
            else
            {
                App_Started = false;
                BtnStart.Content = "Start";
            }
        }

        private void Init(object sender, RoutedEventArgs e)
        {
            if (Rb1D.IsChecked.Value)
            {
                canvas1.Children.Clear();
                lines = new List<Line>();
                Draw();
            }
            else if (Rb2D.IsChecked.Value)
            {
                canvas1.Children.Clear();
                canvas1.Children.Add(Image1);
                Image1.Visibility = Visibility.Visible;
                Image1.Width = (int)canvas1.ActualWidth;
                Image1.Height = (int)canvas1.ActualHeight;
                Image1.Stretch = Stretch.Fill;
                Draw2D();
            }
            else if (Rb3D.IsChecked.Value)
            {
                ZOff += 10 / canvas1.ActualHeight;
                canvas1.Children.Clear();
                canvas1.Children.Add(Image1);
                Image1.Visibility = Visibility.Visible;
                Image1.Width = (int)canvas1.ActualWidth;
                Image1.Height = (int)canvas1.ActualHeight;
                Image1.Stretch = Stretch.Fill;
                Draw3D();
            }
            else if (Rb4D.IsChecked.Value)
            {
                ZOff += 10 / canvas1.ActualHeight;
                canvas1.Children.Clear();
                canvas1.Children.Add(Image1);
                Image1.Visibility = Visibility.Visible;
                Image1.Width = (int)canvas1.ActualWidth;
                Image1.Height = (int)canvas1.ActualHeight;
                Image1.Stretch = Stretch.Fill;
                Draw4D();
            }
        }

        private void Draw()
        {
            int w = (int)canvas1.ActualWidth;
            int h = (int)canvas1.ActualHeight;
            double p;
            double frequency = 0.5;
            while (App_Started)
            {
                XOff += 0.05;
                if (RbRandom.IsChecked.Value )
                {
                    if (CBWide.IsChecked.Value)
                    {
                        p = 0.1 + 0.8*Rnd.NextDouble();
                    }
                    else
                    {
                        p = 0.2 + 0.6 * Rnd.NextDouble();
                    }
                }
                else if (RbPerlin.IsChecked.Value)
                {
                    if (CBWide.IsChecked.Value)
                    {
                        p = PerlinNoise.WideNoise(XOff * frequency, Octaves, Persistence, factor);
                    }
                    else
                    {
                        p = PerlinNoise.Noise(XOff * frequency, Octaves, Persistence);
                    }
                }
                else if (RbOpenSimplex.IsChecked.Value )
                {
                    if (CBWide.IsChecked.Value)
                    {
                       p = OpenSimplexNoise.WideNoise(XOff * frequency, Octaves, Persistence, factor);
                    }
                    else
                    {
                        p = OpenSimplexNoise.Noise(XOff * frequency, Octaves, Persistence);
                    }
                }
                else  //Use FastSimplexNoise
                {
                    if (CBWide.IsChecked.Value)
                    {
                        p = FastSimplexNoise.WideNoise(XOff * frequency / 2, Octaves, Persistence, factor); 
                    }
                    else
                    {
                        p = FastSimplexNoise.Noise(XOff * frequency / 2, Octaves, Persistence); 
                    }
                }
                Line l = new Line()
                {
                    StrokeThickness = 2
                };
                if (CBColor.IsChecked.Value)
                {
                    l.Stroke = new SolidColorBrush(Rainbow[(int)(255 * p)]);
                }
                else
                {
                    l.Stroke = Brushes.Gray;
                }
                if (lines.Count() > 1)
                {
                    l.Y1 = lines[lines.Count - 2].Y2;
                    l.Y2 = (1 - p) * h;
                }
                else
                {
                    l.Y1 = (1 - p) * h;
                    l.Y2 = (1 - p) * h;
                }
                lines.Add(l);
                if (lines.Count > w) { lines.RemoveAt(0); } //Move the line 1 to the left

                for (int I = 1; I < lines.Count; I++)
                {
                    lines[I].X1 = I - 1;
                    lines[I].X2 = I;
                }
                canvas1.Children.Clear();
                for (int I = 0; I < lines.Count; I++)
                {
                    canvas1.Children.Add(lines[I]);
                }
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
            }
        }

        private void Draw2D()
        {
            int w = (int)canvas1.ActualWidth;
            int h = (int)canvas1.ActualHeight;
            double x;
            double y;
            WriteableBitmap Writebitmap;
            int index = 0;
            double p;
            double frequency = 12;
            if (CBColor.IsChecked.Value)
            {
                Writebitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Bgra32, null);
            }
            else
            {
                Writebitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Gray8, null);
            }
            byte[] PixelData;
            int Stride = (int)(Writebitmap.PixelWidth * Writebitmap.Format.BitsPerPixel / 8);
            PixelData = new byte[Stride * Writebitmap.PixelHeight];
            while (App_Started)
            {
                YOff += 10 / canvas1.ActualHeight;
                for (int I = 0; I < Writebitmap.PixelHeight; I++)
                {
                    for (int J = 0; J < Writebitmap.PixelWidth; J++)
                    {
                        x = J / (double)Writebitmap.PixelHeight;
                        y = I / (double)Writebitmap.PixelHeight;
                        index = I * Writebitmap.PixelWidth + J;
                        if (RbRandom.IsChecked.Value)
                        {
                            if (CBWide.IsChecked.Value)
                            {
                                p = 0.1 + 0.8 * Rnd.NextDouble();
                            }
                            else
                            {
                                p = 0.2 + 0.6 * Rnd.NextDouble();
                            }
                        }
                        else if (RbPerlin.IsChecked.Value)
                        {

                            if (CBWide.IsChecked.Value)
                            {
                                p = PerlinNoise.WideNoise2D(x * frequency, y * frequency + YOff, Octaves, Persistence, factor);
                            }
                            else
                            {
                                p = PerlinNoise.Noise2D(x * frequency, y * frequency + YOff, Octaves, Persistence);
                            }
                        }
                        else if(RbOpenSimplex.IsChecked.Value)
                        {
                            if (CBWide.IsChecked.Value)
                            {
                                p = OpenSimplexNoise.WideNoise2D(x * frequency, y * frequency + YOff, Octaves, Persistence, factor);
                            }
                            else
                            {
                                p = OpenSimplexNoise.Noise2D(x * frequency, y * frequency + YOff, Octaves, Persistence);
                            }
                        }
                        else //Use FastSimplexNoise
                        {
                            if (CBWide.IsChecked.Value)
                            {
                                p = FastSimplexNoise.WideNoise2D(x * frequency / 2, y * frequency / 2 + YOff, Octaves, Persistence, factor); 
                            }
                            else
                            {
                                p = FastSimplexNoise.Noise2D(x * frequency/2, y * frequency/2 + YOff, Octaves, Persistence); 
                            }
                        }
                        if (CBColor.IsChecked.Value)
                        {
                            SetPixel(J, I, Rainbow[(int)(255 * p)], PixelData, Stride);
                        }
                         else
                        {
                            PixelData[I * Stride + J] = (byte)(255 * p);
                        }
                    }
                }
                Int32Rect Intrect = new Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1);
                Writebitmap.WritePixels(Intrect, PixelData, Stride, 0);
                Image1.Source = Writebitmap;
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
            }
        }

        private void Draw3D()
        {
            int w = (int)canvas1.ActualWidth;
            int h = (int)canvas1.ActualHeight;
            double x;
            double y;
            WriteableBitmap Writebitmap;
            int index = 0;
            double p;
            double frequency = 12;
            if (CBColor.IsChecked.Value)
            {
                Writebitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Bgra32, null);
            }
            else
            {
                Writebitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Gray8, null);
            }

            byte[] PixelData;
            int Stride = (int)(Writebitmap.PixelWidth * Writebitmap.Format.BitsPerPixel / 8);
            PixelData = new byte[Stride * Writebitmap.PixelHeight];
            while (App_Started)
            {
                ZOff += 10 / canvas1.ActualHeight;
                for (int I = 0; I < Writebitmap.PixelHeight; I++)
                {
                    for (int J = 0; J < Writebitmap.PixelWidth; J++)
                    {
                        x = J / (double)Writebitmap.PixelHeight;
                        y = I / (double)Writebitmap.PixelHeight;
                        index = I * Writebitmap.PixelWidth + J;
                        if (RbRandom.IsChecked.Value)
                        {
                            if (CBWide.IsChecked.Value)
                            {
                                p = 0.1 + 0.8 * Rnd.NextDouble();
                            }
                            else
                            {
                                p = 0.2 + 0.6 * Rnd.NextDouble();
                            }
                        }
                        else if (RbPerlin.IsChecked.Value)
                        {

                            if (CBWide.IsChecked.Value)
                            {
                                p = PerlinNoise.WideNoise3D(x * frequency, y * frequency, ZOff, Octaves, Persistence, factor);
                            }
                            else
                            {
                                p = PerlinNoise.Noise3D(x * frequency, y * frequency, ZOff, Octaves, Persistence);
                            }
                        }
                        else if (RbOpenSimplex.IsChecked.Value)
                        {
                            if (CBWide.IsChecked.Value)
                            {
                                p = OpenSimplexNoise.WideNoise3D(x * frequency, y * frequency, ZOff, Octaves, Persistence, factor);
                            }
                            else
                            {
                                p = OpenSimplexNoise.Noise3D(x * frequency, y * frequency, ZOff, Octaves, Persistence);

                            }
                        }
                        else //Use FastSimplexNoise
                        {
                            if (CBWide.IsChecked.Value)
                            {
                                p = FastSimplexNoise.WideNoise3D(x * frequency/2, y * frequency/2, ZOff, Octaves, Persistence, factor); 
                            }
                            else
                            {
                                p = FastSimplexNoise.Noise3D(x * frequency/2, y * frequency/2, ZOff, Octaves, Persistence); 
                            }
                        }
                        if (CBColor.IsChecked.Value)
                        {
                            SetPixel(J, I, Rainbow[(int)(255 * p)], PixelData, Stride);
                        }
                        else
                        {
                            PixelData[I * Stride + J] = (byte)(255 * p);
                        }
                    }
                }
                Int32Rect Intrect = new Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1);
                Writebitmap.WritePixels(Intrect, PixelData, Stride, 0);
                Image1.Source = Writebitmap;
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
            }
        }

        private void Draw4D()
        {
            int w = (int)canvas1.ActualWidth;
            int h = (int)canvas1.ActualHeight;
            double x;
            double y;
            double angle = 0.0;
            WriteableBitmap Writebitmap;
            int index = 0;
            double p;
            double frequency = 12;
            if (CBColor.IsChecked.Value)
            {
                Writebitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Bgra32, null);
            }
            else
            {
                Writebitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Gray8, null);
            }

            byte[] PixelData;
            int Stride = (int)(Writebitmap.PixelWidth * Writebitmap.Format.BitsPerPixel / 8);
            PixelData = new byte[Stride * Writebitmap.PixelHeight];
            while (App_Started)
            {
                ZOff = 10 * Math.Cos(angle);
                WOff = 10 * Math.Sin(angle);
                for (int I = 0; I < Writebitmap.PixelHeight; I++)
                {
                    for (int J = 0; J < Writebitmap.PixelWidth; J++)
                    {
                        x = J / (double)Writebitmap.PixelHeight;
                        y = I / (double)Writebitmap.PixelHeight;
                        index = I * Writebitmap.PixelWidth + J;
                        if (RbRandom.IsChecked.Value)
                        {
                            if (CBWide.IsChecked.Value)
                            {
                                p = 0.1 + 0.8 * Rnd.NextDouble();
                            }
                            else
                            {
                                p = 0.2 + 0.6 * Rnd.NextDouble();
                            }
                        }
                        else if (RbPerlin.IsChecked.Value)
                        {

                            if (CBWide.IsChecked.Value)
                            {
                                p = PerlinNoise.WideNoise4D(x * frequency, y * frequency, ZOff, WOff, Octaves, Persistence, factor);
                            }
                            else
                            {
                                p = PerlinNoise.Noise4D(x * frequency, y * frequency, ZOff, WOff, Octaves, Persistence);
                            }
                        }
                        else if (RbOpenSimplex.IsChecked.Value)
                        {
                            if (CBWide.IsChecked.Value)
                            {
                                p = OpenSimplexNoise.WideNoise4D(x * frequency, y * frequency, ZOff, WOff, Octaves, Persistence, factor);
                            }
                            else
                            {
                                p = OpenSimplexNoise.Noise4D(x * frequency, y * frequency, ZOff, WOff, Octaves, Persistence);

                            }
                        }
                        else //Use FastSimplexNoise
                        {
                            if (CBWide.IsChecked.Value)
                            {
                                p = FastSimplexNoise.WideNoise4D(x * frequency/2, y * frequency/2, ZOff, WOff, Octaves, Persistence, factor); 
                            }
                            else
                            {
                                p = FastSimplexNoise.Noise4D(x * frequency/2, y * frequency/2, ZOff, WOff, Octaves, Persistence);
                            }
                        }
                        if (CBColor.IsChecked.Value)
                        {
                            SetPixel(J, I, Rainbow[(int)(255 * p)], PixelData, Stride);
                        }
                        else
                        {
                            PixelData[I * Stride + J] = (byte)(255 * p);
                        }
                    }
                }
                Int32Rect Intrect = new Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1);
                Writebitmap.WritePixels(Intrect, PixelData, Stride, 0);
                Image1.Source = Writebitmap;
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
                angle += 0.002 * Math.PI;
                if (angle >= 2 * Math.PI) { angle = 0; }
            }
        }


        private void SetPixel(int X, int Y, Color c, byte[] buffer, int pixStride)
        {
            int xIndex = X * 4;
            int yIndex = Y * pixStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
            buffer[xIndex + yIndex + 3] = c.A;
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnOctavesUP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Octaves = int.Parse(TxtOctaves.Text);
                Octaves += 1;
                TxtOctaves.Text = Octaves.ToString();
                Init(sender, e);
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        private void BtnOctavesDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Octaves = int.Parse(TxtOctaves.Text);
                Octaves -= 1;
                if (Octaves < 1) { Octaves = 1; }
                TxtOctaves.Text = Octaves.ToString();
                Init(sender, e);
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        private void BtnPersistUP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Persistence = double.Parse(TxtPersist.Text.Substring(0, TxtPersist.Text.Length - 1)) / 100;
                Persistence += 0.1;
                if (Persistence > 1) { Persistence = 1; }
                TxtPersist.Text = (100 * Persistence).ToString() + "%";
                Init(sender, e);
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        private void BtnPersistDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Persistence = double.Parse(TxtPersist.Text.Substring(0, TxtPersist.Text.Length - 1)) / 100;
                Persistence -= 0.1;
                if (Persistence < 0) { Persistence = 0; }
                TxtPersist.Text = (100 * Persistence).ToString() + "%";
                Init(sender, e);
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

    }
}
