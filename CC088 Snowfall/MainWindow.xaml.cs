using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snowfall
{
    public partial class MainWindow : Window
    {
        private BitmapImage bitmap;
        private FormatConvertedBitmap convertBitmap;
        private WriteableBitmap bmp;
        private WriteableBitmap backImg;
        private byte[] PixelData;
        private int Stride = 0;
        private List<Snowflake> snowflakes;
        private int FlakeNumbers;
        private Vector wind;
        private Random Rnd = new Random();
        private double[] skyline;
        private double XOff;
        private double YOff;
        private int counter;
        private Size OrigSize;
        private Size NewSize;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OrigSize = new Size(canvas1.ActualWidth, canvas1.ActualHeight);
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void Init()
        {
            NewSize = new Size(canvas1.ActualWidth,canvas1.ActualHeight);
            image1.Width = canvas1.ActualWidth;
            image1.Height = canvas1.ActualHeight;
            if (NewSize.Width < 1000)
            {
                FlakeNumbers = 500;
            }
            else
            {
                FlakeNumbers = 1000;
            }
            bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Skyline.jpg"));
            convertBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 50);
            TransformedBitmap scaledBitmap = new TransformedBitmap();
            scaledBitmap.BeginInit();
            scaledBitmap.Source = convertBitmap;
            scaledBitmap.Transform = new ScaleTransform(NewSize.Width/OrigSize.Width , NewSize.Height / OrigSize.Height );
            scaledBitmap.EndInit();
            bmp = new WriteableBitmap(scaledBitmap);
            backImg = new WriteableBitmap(bmp);
            canvas1.Background = new ImageBrush(backImg);
            Stride = bmp.PixelWidth * bmp.Format.BitsPerPixel / 8;
            PixelData = new byte[Stride * bmp.PixelHeight];
            bmp.CopyPixels(PixelData, Stride, 0);
            image1.Stretch = Stretch.Fill;
            skyline = new double[bmp.PixelWidth];
            //Determine the skyline
            for (int i = 0; i < bmp.PixelWidth; i++)
            {
                for (int j = 0; j < bmp.PixelHeight; j++)
                {
                    if (GetPixel(i, j, PixelData, bmp.Format.BitsPerPixel, Stride).R > 5)
                    {
                        skyline[i] = j;
                        break;
                    }
                }
            }
            XOff = Rnd.Next(1000);
            YOff = Rnd.Next(1000);
            counter = 0;
            Snowflake flake;
            snowflakes = new List<Snowflake>();
            int size;
            bool persist;
            //Create the snowflakes
            for (int i = 0; i < FlakeNumbers; i++)
            {
                persist = Rnd.NextDouble() < 0.70;
                //Create a more realistic size distribution (between 10 and 40)
                double dist = 40 * Rnd.NextDouble();
                size = (int)(Math.Pow(dist, 3) / 2133 + 10);
                flake = new Snowflake(new Point(canvas1.ActualWidth * Rnd.NextDouble(), (canvas1.ActualHeight) * Rnd.NextDouble()), persist, size);
                flake.Draw(bmp);
                snowflakes.Add(flake);
            }
            image1.Source = bmp;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            //Update the snowflakes
            for (int i = 0; i  < snowflakes.Count; i++)
            {
                //Use Perlin Noise to simulate wind
                wind = new Vector(3 * PerlinNoise.Noise2D(XOff, YOff) - 1, 0);
                XOff += 0.05;
                snowflakes[i].UpdatePosition(wind);
            }
            YOff += 0.005;
            //Build the next frame
            bmp = new WriteableBitmap(backImg);
            for (int i = 0; i < snowflakes.Count; i++)
            {
                snowflakes[i].Draw(bmp);
                //Persistent snowflakes that hit the skyline create a snow layer in the background bitmap.
                if (snowflakes[i].Persists)
                {
                    if (Math.Abs(snowflakes[i].Y - skyline[snowflakes[i].X]) < 3.0)
                    {
                        byte[] PixData = new byte[36];
                        SetPixel(0, 0, PixData, 12);
                        SetPixel(1, 0, PixData, 12);
                        SetPixel(2, 0, PixData, 12);
                        SetPixel(0, 1, PixData, 12);
                        SetPixel(1, 1, PixData, 12);
                        SetPixel(2, 1, PixData, 12);
                        SetPixel(0, 2, PixData, 12);
                        SetPixel(1, 2, PixData, 12);
                        SetPixel(2, 2, PixData, 12);
                        backImg.WritePixels(new Int32Rect(snowflakes[i].X, (int)snowflakes[i].Y, 2,2), PixData, 12, 0);
                        skyline[snowflakes[i].X] -= 0.25;
                        snowflakes[i].SetPosition(new Point(canvas1.ActualWidth * Rnd.NextDouble(), 0));
                        counter++;
                    }
                }
            }
            //Smooth a thick snow layer to prevent dendritic growth.
            if (counter >= 2000)
            {
                counter = 0;
                for (int i = 1; i < skyline.Length - 1; i++)
                {
                    skyline[i] = (skyline[i - 1] + skyline[i] + skyline[i + 1]) / 3;
                }
            }
            image1.Source = bmp;
        }

        private void SetPixel(int X, int Y, byte[] buffer, int PixStride)
        {
            int xIndex = X * 4;
            int yIndex = Y * PixStride;
            if (xIndex + yIndex >= 0 & xIndex + yIndex + 2 < buffer.Length)
            {
                buffer[xIndex + yIndex + 0] = 255;
                buffer[xIndex + yIndex + 1] = 255;
                buffer[xIndex + yIndex + 2] = 255;
                buffer[xIndex + yIndex + 3] = 255;
            }
        }

        private Color GetPixel(int X, int Y, byte[] buffer, int bitsPerPixel, int PixStride)
        {
            Color c = new Color();
            int xIndex = X * bitsPerPixel / 8;
            int yIndex = Y * PixStride;
            if (xIndex + yIndex >= 0 & xIndex + yIndex + 2 < buffer.Length)
            {
                c.R = buffer[xIndex + yIndex + 0];
                c.G = buffer[xIndex + yIndex + 1];
                c.B = buffer[xIndex + yIndex + 2];
            }
            return c;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsLoaded) { return; }
            Init();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}