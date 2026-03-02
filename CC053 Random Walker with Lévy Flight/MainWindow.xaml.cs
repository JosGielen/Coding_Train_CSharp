using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Random_Walker
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private int W = 0;
        private int H = 0;
        private WriteableBitmap Writebitmap;
        private int Stride = 0;
        private byte[] buffer = new byte[3];
        private bool Rendering = false;
        private double stepSize;
        private int flightChance = 1;  // % chance of a Lévy flight
        private double maxFlightLength = 100;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            Image1.Width = Canvas1.ActualWidth;
            Image1.Height = Canvas1.ActualHeight;
            W = (int)(Image1.Width);
            H = (int)(Image1.Height);
            Stride = (int)(W * PixelFormats.Rgb24.BitsPerPixel / 8);
            //Show the start-up image
            Writebitmap = new WriteableBitmap(W, H, 96, 96, PixelFormats.Rgb24, null);
            Image1.Source = Writebitmap;
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                BtnStart.Content = "Stop";
                Init();
                Rendering = true;
                Render();
            }
            else
            {
                BtnStart.Content = "Start";
                Rendering = false;
            }
        }

        public void Render()
        {
            //Random StartPosition
            Vector V = new Vector((int)(Canvas1.ActualWidth / 2), (int)(Canvas1.ActualHeight / 2));
            while (Rendering)
            {
                SetPixel((int)V.X, (int)V.Y, Stride);
                Vector step = new Vector(2 * Rnd.NextDouble() - 1, 2 * Rnd.NextDouble() - 1);
                step.Normalize();
                if (100 * Rnd.NextDouble() < flightChance)
                {
                    stepSize = maxFlightLength * Rnd.NextDouble();
                }
                else
                {
                    stepSize = 2.0;
                }
                step *= stepSize;
                V += step;
                if (V.X < 0) V.X = 0;
                if (V.X > W - 1) V.X = W - 1;
                if (V.Y < 0) V.Y = 0;
                if (V.Y > H - 1) V.Y = H - 1;
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), 20);
            }
        }

        private void SetPixel(int X, int Y, int PixStride)
        {
            int xIndex = X * 3;
            int yIndex = Y * PixStride;
            //Make a rectangle with Width=1 and Height=1
            Int32Rect rect = new Int32Rect(X, Y, 1, 1);
            buffer[0] = 255;
            buffer[1] = 255;
            buffer[2] = 255;
            if (rect.X < Writebitmap.PixelWidth & rect.Y < Writebitmap.PixelHeight)
            {
                Writebitmap.WritePixels(rect, buffer, PixStride, 0);
            }
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Image1.Width = Canvas1.ActualWidth;
            Image1.Height = Canvas1.ActualHeight;
        }
    }
}