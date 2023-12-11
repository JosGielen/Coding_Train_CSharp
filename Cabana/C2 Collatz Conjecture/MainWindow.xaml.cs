using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Collatz_Conjecture
{

    public partial class MainWindow : Window
    {
        private readonly int WaitTime = 10;
        private bool Rendering = false;
        private readonly double angleOff = 15.0;
        private readonly double length = 10.0;
        private List<Brush> colorList;
        private List<int> series;
        private readonly string ImageFileName = "Collatz-";
        private int frameNumber = 0;
        private readonly string ResultFileName = "Collatz.gif";
        private readonly bool recording = false ;
        private readonly Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            ColorPalette cp = new ColorPalette(Environment.CurrentDirectory + "\\Reds.cpl");
            colorList = cp.GetColorBrushes(2000);
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                BtnStart.Content = "STOP";
                Rendering = true;
                Render();
            }
            else
            {
                BtnStart.Content = "START";
                Rendering = false;
            }
        }

        private void Render()
        {
            int N = 0;
            string imageName = "";
            series = new List<int>();
            for (int I = 1; I <= 1500; I++) 
            {
                if (!Rendering) return;
                series.Clear();
                N = I;
                while (N > 1)
                {
                    series.Add(N);
                    N = Collatz(N);
                }
                series.Add(1);
                DrawSeries();
                Title = I.ToString();
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
                if (recording && I % 8 == 0)
                {
                    frameNumber += 1;
                    imageName = Environment.CurrentDirectory + "\\output\\" + ImageFileName + frameNumber.ToString("000") + ".png";
                SaveImage(imageName);
                    Thread.Sleep(50);
                }
            }
            if (recording)
            {
                MakeGif();
                Environment.Exit(0);
            }
        }

        private int Collatz(int n)
        {
            if (n % 2 == 0)
            {
                return (int)(n / 2);
            }
            else
            {
                return (int)((3 * n + 1) / 2);
           }
        }

        private void DrawSeries()
        {
            if (series.Count > 50) return;
            Line L;
            double X = canvas1.ActualWidth / 6;
            double Y = canvas1.ActualHeight;
            double angle = Math.PI / 4;
            double Xoff = 0.0;
            double Yoff = 0.0;
            int index = Rnd.Next(colorList.Count);
            series.Reverse();
            for (int I = 0; I < series.Count - 1; I++)
            {
                if (series[I + 1] % 2 == 0)
                {
                    angle += angleOff * Math.PI / 180;
                }
                else
                {
                    angle -= angleOff * Math.PI / 180;
                }
                Xoff = -length * Math.Cos(angle);
                Yoff = -length * Math.Sin(angle);
                L = new Line()
                {
                    X1 = X,
                    Y1 = Y,
                    X2 = X + Xoff,
                    Y2 = Y + Yoff,
                    Stroke = colorList[index],
                    StrokeThickness = 5
                };
                canvas1.Children.Add(L);
                X += Xoff;
                Y += Yoff;
            }
        }

        private void SaveImage(string filename)
        {
            BitmapEncoder MyEncoder = new PngBitmapEncoder();
            RenderTargetBitmap renderbmp = new RenderTargetBitmap((int)(canvas1.ActualWidth), (int)(canvas1.ActualHeight), 96, 96, PixelFormats.Default);
            renderbmp.Render(canvas1);
            try
            {
                MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp));
                // Create a FileStream to write the image to the file.
                FileStream sw = new FileStream(filename, FileMode.Create);
                MyEncoder.Save(sw);
                sw.Close();
            }
            catch (Exception)
            { 
            MessageBox.Show("The Image could not be saved.", "Collatz Conjecture error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MakeGif()
        {
            string prog = Environment.CurrentDirectory + "\\ffmpeg.exe";
            string args = "-i output\\" + ImageFileName + "%3d.png -r 20 " + ResultFileName;
            try
            {
            Process.Start(prog, args);
            }
            catch(Exception ex)
            {
                MessageBox.Show("The Gif Image could not be saved.", "Collatz Conjecture error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }

        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
