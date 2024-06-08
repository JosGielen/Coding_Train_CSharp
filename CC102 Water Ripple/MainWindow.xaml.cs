using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Water_Ripple
{
    public partial class MainWindow : Window
    {
        private int W = 0;
        private int H = 0;
        private WriteableBitmap bmp;
        private byte[] PixelData1;
        private double[,] Buffer1;
        private double[,] Buffer2;
        private double Dampening = 0.95;
        private int Stride = 0;
        private int FrameCounter;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Set the Image to fill the canvas.
            W = (int)canvas1.ActualWidth;
            H = (int)canvas1.ActualHeight;
            image1.Width = W;
            image1.Height = H;
            //Create a bitmap to show in the Image
            Stride = W * PixelFormats.Rgb24.BitsPerPixel / 8;
            PixelData1 = new byte[Stride * H];
            bmp = new WriteableBitmap(W, H, 96, 96, PixelFormats.Rgb24, null);
            //Initialize the buffers
            Buffer1 = new double[W, H];
            Buffer2 = new double[W, H];
            FrameCounter = 0;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            FrameCounter++;
            if (FrameCounter % 5 == 0)
            {
                Buffer1[(int)((canvas1.ActualWidth - 20) * Rnd.NextDouble() + 20), (int)((canvas1.ActualHeight - 20) * Rnd.NextDouble() + 20)] = 255;
            }
            //Calculate the ripple effect
            for (int Y = 1; Y < H - 1; Y++)
            {
                for (int X = 1; X < W - 1; X++)
                {
                    Buffer2[X, Y] = ( 
                        Buffer1[X - 1, Y] + 
                        Buffer1[X + 1, Y] + 
                        Buffer1[X, Y + 1] +  
                        Buffer1[X, Y - 1]) / 2 - Buffer2[X, Y];
                    Buffer2[X, Y] *= Dampening;
                    SetGrayPixel(X, Y, (byte)Buffer2[X, Y], PixelData1, PixelFormats.Rgb24.BitsPerPixel, Stride);
                }
            }
            //Swap the Buffers
            double[,] temp;
            temp = Buffer1;
            Buffer1 = Buffer2;
            Buffer2 = temp;
            //Show the new Data in image1
            bmp.WritePixels(new Int32Rect(0, 0, W, H), PixelData1, Stride, 0);
            image1.Source = bmp;
        }

        private void SetGrayPixel(int x, int y, byte value, byte[] buffer, int BitsperPixel, int PixStride)
        {
            int xIndex = x * BitsperPixel / 8;
            int yIndex = y * PixStride;
            buffer[xIndex + yIndex] = value;
            buffer[xIndex + yIndex + 1] = value;
            buffer[xIndex + yIndex + 2] = value;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}