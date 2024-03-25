using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Approximating_Pi
{
    public partial class MainWindow : Window
    {
        private delegate void RenderDelegate(Point pt);
        private byte[] PixelData;
        private byte[] NewPixelData = new byte[3];
        private WriteableBitmap Writebitmap;
        private int Stride = 0;
        private List<Color> colorList;
        private BitmapPalette palet;
        private double PiEstimate;
        private bool inside;
        private long Total = 0;
        private long inCircle = 0;
        private bool started = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Image1.Width = MyCanvas.ActualWidth;
            Image1.Height = MyCanvas.ActualHeight;
            Stride = (int)(Image1.Width * PixelFormats.Rgb24.BitsPerPixel / 8.0);
            PixelData = new byte[(int)(Stride * Image1.Height)];
            colorList = [Colors.Red, Colors.Green, Colors.Blue];
            palet = new BitmapPalette(colorList);
            Writebitmap = new WriteableBitmap(BitmapSource.Create((int)Image1.Width, (int)Image1.Height, 96, 96, PixelFormats.Rgb24, palet, PixelData, Stride));
            Image1.Source = Writebitmap;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random(0);
            double W = MyCanvas.ActualWidth;
            double H = MyCanvas.ActualHeight;
            double X = 0.0;
            double Y = 0.0;
            Point pt;
            if (!started)
            {
                Total = 0;
                inCircle = 0;
                PiEstimate = 0.0;
                BtnStart.Content = "STOP";
                Writebitmap = new WriteableBitmap(BitmapSource.Create((int)Image1.Width, (int)Image1.Height, 96, 96, PixelFormats.Rgb24, palet, PixelData, Stride));
                Image1.Source = Writebitmap;
                started = true;
            }
            else
            {
                BtnStart.Content = "START";
                started = false;
            }
            while (started)
            {
                for (int I = 0; I < 1000; I++)
                {
                    X = MyCanvas.ActualWidth * rand.NextDouble();
                    Y = MyCanvas.ActualHeight * rand.NextDouble();
                    pt = new Point(X, Y);
                    if (PtDistance(pt, new Point(W / 2, H / 2)) <= W / 2)
                    {
                        inCircle += 1;
                        inside = true;
                    }
                    else
                    {
                        inside = false;
                    }
                    Total += 1;
                }
                pt = new Point(X, Y);
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new RenderDelegate(Draw), pt);
            }
        }

        private void Draw(Point pt)
        {
            Int32Rect rect = new Int32Rect((int)(pt.X), (int)(pt.Y), 1, 1);
            if (inside)
            {
                NewPixelData[0] = 0;
                NewPixelData[1] = 255;
                NewPixelData[2] = 0;
            }
            else
            {
                NewPixelData[0] = 255;
                NewPixelData[1] = 0;
                NewPixelData[2] = 0;
            }
            TxtNumber.Text = Total.ToString();
            TxtPiEstimate.Text = (4.0 * inCircle / Total).ToString();
            if (rect.X < Writebitmap.PixelWidth & rect.Y < Writebitmap.PixelHeight)
            {
                Writebitmap.WritePixels(rect, NewPixelData, Stride, 0);
            }
        }

        public double PtDistance(Point p1, Point p2)
        {
            return Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}