using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fluid_Simulation
{
    public partial class MainWindow : Window
    {
        private Random Rnd = new Random();
        private int N;
        private double Speed = 10.0;
        private Fluid F;
        private int iter = 8;
        private double TimeStep = 0.03;
        private double PerlinX = 0.0;
        private int colorCount = 650;
        private Point center;
        private double DyeRate = 800;
        private bool Rotation;
        private bool Started = false;
        //Render data
        private List<Color> my_Colors;
        private BitmapPalette palet;
        private PixelFormat PixFormat;
        private int BytesPerPixel;
        private WriteableBitmap Writebitmap;
        private byte[] PixelData;
        private byte[] NewPixelData;
        private int Stride = 0;
        private int CellSize = 2;
        private Int32Rect Intrect;
        //FPS data
        private DateTime LastRenderTime;
        private int Framecounter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            //Load a color palette
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Thermal.cpl");
            my_Colors = pal.GetColors(colorCount);
            if (canvas1.ActualWidth > canvas1.ActualHeight)
            {
                N = (int)(canvas1.ActualHeight / CellSize);
            }
            else
            {
                N = (int)(canvas1.ActualWidth / CellSize);
            }
            center = new Point(N / 2, N / 2);
            //Create a Fluid
            F = new Fluid(N, TimeStep, 0, 0.05);
            //Create a writeable bitmap to render the Fluid
            Speed = 5.0;
            image1.Width = N * CellSize;   //canvas1.ActualWidth
            image1.Height = N * CellSize;  //canvas1.ActualHeight
            image1.Stretch = Stretch.Fill;

            PixFormat = PixelFormats.Rgb24;
            BytesPerPixel = PixFormat.BitsPerPixel / 8;
            Stride = N * CellSize * BytesPerPixel;
            PixelData = new byte[Stride * (int)image1.Height];
            NewPixelData = new byte[CellSize * CellSize * BytesPerPixel];
            Writebitmap = new WriteableBitmap(BitmapSource.Create((int)image1.Width, (int)image1.Height, 96, 96, PixelFormats.Rgb24, null, PixelData, Stride));
            image1.Source = Writebitmap;
            //Initialize FPS counter
            LastRenderTime = DateTime.Now;
            Framecounter = 0;
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            if (!Started) return;
            double angle;
            //Show FPS
            Framecounter += 1;
            if (Framecounter == 10)
            {
                double fps = (int)(10000 / (DateTime.Now - LastRenderTime).TotalMilliseconds);
                Title = "FPS = " + fps.ToString();
                LastRenderTime = DateTime.Now;
                Framecounter = 0;
            }
            //Add some dye to the Fluid
            if (Rotation)
            {
                angle = 2 * Math.PI * PerlinNoise.WideNoise(PerlinX, 2, 0.8, 2);
            }
            else
            {
                angle = 0.0;
            }
            PerlinX += 0.01;
            for (int I = -1; I <= 1; I++)
            {
                for (int J = -1; J <= 1; J++)
                {
                    F.AddDensity((int)center.X + I, (int)center.Y + J, DyeRate);
                }
            }
            F.AddVelocity((int)center.X, (int)center.Y, Speed * Math.Cos(angle), Speed * Math.Sin(angle));
            F.FadeDensity(0.05 * DyeRate / N);
            //Update the Fluid
            F.TimeStep(iter);
            RenderFluid();
        }
        private void RenderFluid()
        {
            int ColorIndex;
            for (int J = 0; J < N; J++)
            {
                for (int I = 0; I < N; I++)
                {
                    ColorIndex = (int)F.density[I, J];
                    if (ColorIndex >= my_Colors.Count) ColorIndex = my_Colors.Count - 1;
                    Intrect = new Int32Rect(I * CellSize, J * CellSize, CellSize, CellSize);
                    for (int K = 0; K < CellSize * CellSize; K++)
                    {
                        NewPixelData[BytesPerPixel * K] = my_Colors[ColorIndex].R;
                        NewPixelData[BytesPerPixel * K + 1] = my_Colors[ColorIndex].G;
                        NewPixelData[BytesPerPixel * K + 2] = my_Colors[ColorIndex].B;
                    }
                    if (Intrect.X < Writebitmap.PixelWidth & Intrect.Y < Writebitmap.PixelHeight)
                    {
                        Writebitmap.WritePixels(Intrect, NewPixelData, CellSize * BytesPerPixel, 0);
                    }
                }
            }
        }

        private void SldSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Speed = SldSpeed.Value;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Started)
            {
                Started = true;
                BtnStart.Content = "STOP";
                Rotation = CbRotate.IsChecked.Value;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                Started = false;
                BtnStart.Content = "START";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        private void CbRotate_Click(object sender, RoutedEventArgs e)
        {
            Rotation = CbRotate.IsChecked.Value;
        }
    }
}
