using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Metaballs
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private int WaitTime = 10;
        private PixelFormat pf = PixelFormats.Rgb24;
        private int VeldWidth = 0;
        private int VeldHeight = 0;
        private int ImageWidth = 0;
        private int ImageHeight = 0;
        private int Stride = 0;
        private byte[] pixelData;
        private List<Color> Rainbow;
        private Ball[] balls;
        private int BallCount = 6;
        private double BallSize = 15;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VeldWidth = (int)canvas1.ActualWidth;
            VeldHeight = (int)canvas1.ActualHeight;
            Image1.Width = canvas1.ActualWidth;
            Image1.Height = canvas1.ActualHeight;
            Init();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Init()
        {
            Ball bal;
            Random rnd = new Random();
            ImageWidth = VeldWidth;
            ImageHeight = VeldHeight;
            Stride = (int)((ImageWidth * pf.BitsPerPixel + 7) / 8);
            Image1.Width = ImageWidth;
            Image1.Height = ImageHeight;
            //Resize de array
            pixelData = new byte[Stride * (int)ImageHeight];
            //Load the Rainbow colors
            Rainbow = new List<Color>();
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            Rainbow = pal.GetColors(512);
            //Make the balls
            balls = new Ball[BallCount];
            for (int I = 0; I < BallCount; I++)
            {
                bal = new Ball(BallSize, new Point((VeldWidth - 3.2 * BallSize) * rnd.NextDouble() + 1.6 * BallSize , (VeldHeight - 3.2 * BallSize) * rnd.NextDouble() + 1.6 * BallSize), 5 * rnd.NextDouble() - 2.5, 5 * rnd.NextDouble() - 2.5);
                balls[I] = bal;
            }
            Title = "MetaBalls";
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double dist;
            double index = 0;
            for (int I = 0; I < BallCount; I++)
            {
                balls[I].Update(VeldWidth, VeldHeight);
            }
            for (int X = 0; X < VeldWidth; X++)
            {
                for (int Y = 0; Y < VeldHeight; Y++)
                {
                    dist = 0.0;
                    for (int I = 0; I < BallCount; I++)
                    {
                        dist += BallSize / Math.Sqrt((X - balls[I].X) * (X - balls[I].X) + (Y - balls[I].Y) * (Y - balls[I].Y));
                    }
                    index = 1.5 * (1 - dist) * (Rainbow.Count - 1);
                    if (index < 0)
                    {
                        SetPixel(X, Y, Rainbow.First(), pixelData, Stride);
                    }
                    else if (index < Rainbow.Count - 1)
                    {
                        SetPixel(X, Y, Rainbow[(int)index], pixelData, Stride);
                   }
                    else
                    {
                        SetPixel(X, Y, Rainbow.Last(), pixelData, Stride);
                   }
                }
            }
            BitmapSource bitmap = BitmapSource.Create((int)ImageWidth, (int)ImageHeight, 96, 96, pf, null, pixelData, Stride);
            Image1.Source = bitmap;
            Dispatcher.Invoke(DispatcherPriority.Background, new WaitDelegate(Wait), WaitTime);
        }

        private void SetPixel(int x, int y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = x * 3;
            int yIndex = y * PixStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        private void Wait(int t)
        {
            Thread.Sleep(WaitTime);
        }
    }
}