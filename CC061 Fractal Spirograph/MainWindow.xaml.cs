using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fractal_Spirograph
{
    public partial class MainWindow : Window
    {
        private int W = 0;
        private int H = 0;
        private WriteableBitmap Writebitmap;
        private int Stride = 0;
        private byte[] pixelData;
        private int[,] pixelCounts;
        private byte[] buffer = new byte[3];
        private bool Rendering = false;
        private Settings settingForm;
        private int EpicycleCount = 6;
        private int speedStep = -4;
        private int RadiusFactor = 33;
        private bool innerCircles = false;
        private List<Epicycle> Epicycles;
        private double Time;
        private double TimeStep = 0.0005;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            settingForm = new Settings(this);
            settingForm.Left = Left + Width;
            settingForm.Top = Top;
            settingForm.Show();
            settingForm.CircleCount = EpicycleCount;
            settingForm.RadiusFactor = RadiusFactor;
            settingForm.SpeedFactor = speedStep;
            settingForm.TimeStep = TimeStep;
        }

        private void Init()
        {
            Epicycle epi;
            double Radius = canvas1.ActualWidth / 6;
            Epicycles = new List<Epicycle>();
            Time = 0.0;
            //Get the settings
            EpicycleCount = settingForm.CircleCount;
            RadiusFactor = settingForm.RadiusFactor;
            speedStep = settingForm.SpeedFactor;
            innerCircles = settingForm.InnerCircles;
            TimeStep = settingForm.TimeStep;
            //Create the Bitmap
            Image1.Width = canvas1.ActualWidth;
            Image1.Height = canvas1.ActualHeight;
            W = (int)Image1.Width;
            H = (int)Image1.Height;
            Stride = W * PixelFormats.Rgb24.BitsPerPixel / 8;
            pixelData = new byte[Stride * H];
            pixelCounts = new int[W, H];
            //Show the start-up image
            Writebitmap = new WriteableBitmap(BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, null, pixelData, Stride));
            Image1.Source = Writebitmap;

            //Create the Epicycles
            for (int I = 0; I < EpicycleCount; I++)
            {
                epi = new Epicycle(Math.Pow(speedStep, I), Radius);
                Epicycles.Add(epi);
                Radius = RadiusFactor * Radius / 100;
            }
        }

        private void Render()
        {
            Point pt;
            while (Rendering)
            {
                pt = SumEpicycles();
                SetPixel((int)pt.X, (int)pt.Y, Stride);
                Time += TimeStep;
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
                if (Time >= 2 * Math.PI)
                {
                    //Reset the drawing
                    settingForm.BtnStart.Content = "Start";
                    Halt();
                }
            }
        }

        private Point SumEpicycles()
        {
            double X;
            double Y;
            //Draw the Epicycles
            X = canvas1.ActualWidth / 2;
            Y = canvas1.ActualHeight / 2;
            for (int I = 0; I < Epicycles.Count - 1; I++)
            {
                if (innerCircles)
                {
                    X += (Epicycles[I].Radius - Epicycles[I + 1].Radius) * Math.Cos(Epicycles[I].Speed * Time - Math.PI / 2);
                    Y += (Epicycles[I].Radius - Epicycles[I + 1].Radius) * Math.Sin(Epicycles[I].Speed * Time - Math.PI / 2);
                }
                else
                {
                    X += (Epicycles[I].Radius + Epicycles[I + 1].Radius) * Math.Cos(Epicycles[I].Speed * Time - Math.PI / 2);
                    Y += (Epicycles[I].Radius + Epicycles[I + 1].Radius) * Math.Sin(Epicycles[I].Speed * Time - Math.PI / 2);
                }
            }
            return new Point(X, Y);
        }

        private void SetPixel(int X, int Y, int PixStride)
        {
            int xIndex = X * 3;
            int yIndex = Y * PixStride;
            int colorIndex = pixelData[xIndex + yIndex];
            pixelCounts[X, Y] += 1;
            if (pixelCounts[X, Y] > 24) pixelCounts[X, Y] = 24;
            //Make a rectangle with Width=1 and Height=1
            Int32Rect rect = new Int32Rect(X, Y, 1, 1);
            buffer[0] = 255;
            buffer[1] = 0;
            buffer[2] = 0;
            if (rect.X < Writebitmap.PixelWidth & rect.Y < Writebitmap.PixelHeight)
            {
                Writebitmap.WritePixels(rect, buffer, PixStride, 0);
            }
        }

        private void Wait()
        {
            Thread.Sleep(1);
        }

        public void Start()
        {
            Init();
            Rendering = true;
            Render();
        }

        public void Halt()
        {
            Rendering = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e )
        {
            if (IsLoaded)
            {
                Init();
            settingForm.Left = Left + Width;
                settingForm.Top = Top;
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e )
        {
            if (IsLoaded)
            {
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settingForm.Close();
            Environment.Exit(0);
        }
    }
}
