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
        private PixelFormat pixformat;
        private WriteableBitmap Writebitmap;
        private int Stride = 0;
        private byte[] pixelData;
        private int[,] pixelCounts;
        private byte[] buffer = new byte[3];
        private List<Color> colorList;
        private List<Color> my_Colors;
        private BitmapPalette palet;
        private bool Rendering = false;
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
            ColorPalette cp = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            my_Colors = cp.GetColors(25);
            Image1.Width = Canvas1.ActualWidth;
            Image1.Height = Canvas1.ActualHeight;
            W = (int)(Image1.Width);
            H = (int)(Image1.Height);
            Stride = (int)(W * PixelFormats.Rgb24.BitsPerPixel / 8);
            pixelData = new byte[Stride * H];
            pixelCounts = new int[W, H];
            colorList = new List<Color>()
            {
                Colors.Red,
                Colors.Green,
                Colors.Blue
            };
            palet = new BitmapPalette(colorList);
            //Show the start-up image
            Writebitmap = new WriteableBitmap(BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, palet, pixelData, Stride));
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
            int X = (int)(Canvas1.ActualWidth / 2);
            int Y = (int)(Canvas1.ActualHeight / 2);
            while (Rendering)
            {
                SetPixel(X, Y, Stride);
                switch (Rnd.Next(4))
                {
                    case 0:
                        X = X + 1;
                        break;
                    case 1:
                        X = X - 1;
                        break;
                    case 2:
                        Y = Y + 1;
                        break;
                    case 3:
                        Y = Y - 1;
                        break;
                }
                if (X < 0) X = 0;
                if (X > W - 1) X = W - 1;
                if (Y < 0) Y = 0;
                if (Y > H - 1) Y = H - 1;
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), 1);
            }
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
            buffer[0] = my_Colors[pixelCounts[X, Y]].R;
            buffer[1] = my_Colors[pixelCounts[X, Y]].G;
            buffer[2] = my_Colors[pixelCounts[X, Y]].B;
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