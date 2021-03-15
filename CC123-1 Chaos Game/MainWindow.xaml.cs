using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChaosGame
{
    public partial class MainWindow : Window
    {
        private Settings setter;
        private List<Vector> Points;
        private Vector CurrentPt;
        private Vector MidPt;
        private readonly Random Rnd = new Random();
        private byte[] PixelData;
        private WriteableBitmap Writebitmap;
        private int Stride = 0;
        private Int32Rect Intrect;
        private List<Color> myColors;
        private ColorPalette colorPalet;
        //Parameters
        private int PointsNum = 3;
        private double StepPercentage = 50;
        private bool useColor = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            ShowSettingForm();
            StartRender();
        }

        private void Init()
        {
            Vector p;
            double angle = 0.0;
            double radius = 0.0;
            colorPalet = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            myColors = colorPalet.GetColors(PointsNum);
            Image1.Width = canvas1.ActualWidth;
            Image1.Height = canvas1.ActualHeight;
            MidPt = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            Stride = (int)(Image1.Width * PixelFormats.Rgb24.BitsPerPixel / 8);
            PixelData = new byte[Stride * (int)Image1.Height];
            List<Color> colorList = new List<Color>();
            colorList.Add(Colors.Red);
            colorList.Add(Colors.Green);
            colorList.Add(Colors.Blue);
            BitmapPalette palet = new BitmapPalette(colorList);
            Writebitmap = new WriteableBitmap(BitmapSource.Create((int)Image1.Width, (int)Image1.Height, 96, 96, PixelFormats.Rgb24, palet, PixelData, Stride));
            Image1.Source = Writebitmap;

            radius = canvas1.ActualWidth / 2;
            if (canvas1.ActualHeight / 2 < radius) radius = canvas1.ActualHeight / 2;
            Points = new List<Vector>();
            for (int I = 0; I < PointsNum; I++)
            {
                angle = 2 * Math.PI * I / PointsNum - Math.PI / 2;
                p = new Vector(0.9 * radius * Math.Cos(angle), 0.9 * radius * Math.Sin(angle)) + MidPt;
                Points.Add(p);
                SetPixel((int)p.X, (int)p.Y, Colors.White, PixelData, Stride);
            }
            CurrentPt = new Vector(canvas1.ActualWidth * Rnd.NextDouble(), canvas1.ActualHeight * Rnd.NextDouble());
        }

        private void StartRender()
        {
            //Some initial code
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void StopRender()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Vector nextPt;
            int index;
            for (int I = 0; I <= 100; I++)
            {
                index = Rnd.Next(Points.Count);
                nextPt = Points[index];
                nextPt = Lerp(CurrentPt, nextPt, StepPercentage / 100);
                if (useColor)
                {
                    SetPixel((int)nextPt.X, (int)nextPt.Y, myColors[index], PixelData, Stride);
                }
                else
                {
                    SetPixel((int)nextPt.X, (int)nextPt.Y, Colors.White, PixelData, Stride);
                }
                CurrentPt = nextPt;
            }
            Intrect = new Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1);
            Writebitmap.WritePixels(Intrect, PixelData, Stride, 0);
        }

        private Vector Lerp(Vector V1, Vector V2, double Percentage)
        {
            Vector result = new Vector();
            result.X = V1.X + StepPercentage / 100 * (V2.X - V1.X);
            result.Y = V1.Y + StepPercentage / 100 * (V2.Y - V1.Y);
            return result;
        }

        public void Update()
        {
            if (setter != null)
            {
                PointsNum = setter.PointsNum;
                StepPercentage = setter.StepPercentage;
                useColor = setter.UsePointColors;
            }
            Init();
        }

        private void ShowSettingForm()
        {
            if (setter == null)
            {
                setter = new Settings(this);
                setter.Show();
                setter.Left = Left + Width;
                setter.Top = Top;
                setter.PointsNum = PointsNum;
                setter.StepPercentage = StepPercentage;
                setter.UsePointColors = useColor;
                setter.WriteSettings();
            }
            else
            {
                setter.Show();
            }
        }

        private void SetPixel(int X, int Y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = X * 3;
            int yIndex = Y * PixStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        private void Window_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            StopRender();
            if (setter != null)
            {
                setter.Left = Left + Width;
                setter.Top = Top;
            }
            Init();
            StartRender();
        }

        private void Window_LocationChanged(Object sender, EventArgs e)
        {
            if (setter != null)
            {
                setter.Left = Left + Width;
                setter.Top = Top;
            }
        }
    }
}
