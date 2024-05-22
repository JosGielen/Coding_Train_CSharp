using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Langton_Ant
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private int W = 0;
        private int H = 0;
        private WriteableBitmap bmp;
        private int Stride = 0;
        private int BitsPerPixel;
        private byte[] pixelData;
        private bool Rendering = false;
        private int posX;
        private int posY;
        private int dir;
        private int stepCount;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            Image1.Width = Canvas1.ActualWidth;
            Image1.Height = Canvas1.ActualHeight;
            W = (int)Image1.Width;
            H = (int)Image1.Height;
            BitsPerPixel = PixelFormats.Rgb24.BitsPerPixel;
            Stride = W * BitsPerPixel / 8;
            pixelData = new byte[Stride * H];
            //Fill the pixelData with white pixels
            for (int i =  0; i < pixelData.Length; i++)
            {
                pixelData[i] = 255;
            }
            posX = W / 2;
            posY = H / 2;
            dir = 0;
            stepCount = 0;
            FlipPixel(posX, posY);
            //Show the start-up image
            bmp = new WriteableBitmap(BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, null, pixelData, Stride));
            Image1.Source = bmp;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
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
            while (Rendering)
            {
                stepCount++;
                Title = stepCount.ToString();
                //At a white square, turn 90° clockwise, flip the color of the square, move forward one unit
                if (GetPixel(posX, posY).R == 255)
                {
                    FlipPixel(posX, posY);
                    FlipPixel(posX + 1, posY);
                    FlipPixel(posX, posY + 1);
                    FlipPixel(posX + 1, posY + 1);
                    dir--;
                    if (dir == -1) { dir = 3; }
                }
                //At a black square, turn 90° counter - clockwise, flip the color of the square, move forward one unit
                else if (GetPixel(posX, posY).R == 0)
                {
                    FlipPixel(posX, posY);
                    FlipPixel(posX + 1, posY);
                    FlipPixel(posX, posY + 1);
                    FlipPixel(posX + 1, posY + 1);
                    dir++;
                    if (dir == 4) { dir = 0; }
                }
                bmp.WritePixels(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight), pixelData, Stride, 0);
                Image1.Source = bmp;
                switch (dir)
                {
                    case 0:
                        posY-=2;
                        break;
                    case 1:
                        posX+=2;
                        break;
                    case 2:
                        posY+=2;
                        break;
                    case 3:
                        posX-=2;
                        break;
                }
                if (posX < 0) posX = W - 2;
                if (posX > W - 2) posX = 0;
                if (posY < 0) posY = H - 2;
                if (posY > H - 2) posY = 0;
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), 1);
            }
        }

        private void FlipPixel(int X, int Y)
        {
            int xIndex = X * BitsPerPixel / 8;
            int yIndex = Y * Stride;
            if (xIndex + yIndex >= 0 & xIndex + yIndex + 2 < pixelData.Length)
            {
                if (pixelData[xIndex + yIndex] == 0)
                {
                    pixelData[xIndex + yIndex + 0] = 255;
                    pixelData[xIndex + yIndex + 1] = 255;
                    pixelData[xIndex + yIndex + 2] = 255;
                }
                else
                {
                    pixelData[xIndex + yIndex + 0] = 0;
                    pixelData[xIndex + yIndex + 1] = 0;
                    pixelData[xIndex + yIndex + 2] = 0;
                }
            }
        }

        private Color GetPixel(int X, int Y)
        {
            Color c = new Color();
            int xIndex = X * BitsPerPixel / 8;
            int yIndex = Y * Stride;
            if (xIndex + yIndex >= 0 & xIndex + yIndex + 2 < pixelData.Length)
            {
                c.R = pixelData[xIndex + yIndex + 0];
                c.G = pixelData[xIndex + yIndex + 1];
                c.B = pixelData[xIndex + yIndex + 2];
            }
            return c;
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Image1.Width = Canvas1.ActualWidth;
            Image1.Height = Canvas1.ActualHeight;
        }
    }

    public enum Dir
    {
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4,
    }
}