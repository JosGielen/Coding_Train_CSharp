using ChaosGame;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Falling_Sand
{
    public partial class MainWindow : Window
    {
        private bool AppStarted;
        private int ImageWidth = 0;
        private int ImageHeight = 0;
        private PixelFormat pf = PixelFormats.Rgb24;
        private int Stride = 0;
        private byte[] pixelData;
        private List<Vector> Grains;
        private List<Color> myColors;
        private ColorPalette colorPalet;
        private bool MyMouseDown;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            image1.Width = canvas1.ActualWidth;
            image1.Height = canvas1.ActualHeight;
            Init();
        }

        private void Init()
        {
            ImageWidth = (int)Math.Floor(image1.Width);
            ImageHeight = (int)Math.Floor(image1.Height);
            Stride = (int)((ImageWidth * pf.BitsPerPixel + 7) / 8.0);
            pixelData = new byte[Stride * ImageHeight];
            colorPalet = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            myColors = colorPalet.GetColors(20000);
            Grains = new List<Vector>();
            Grains.Add(new Vector(ImageWidth/2, 0.1 * ImageHeight));    
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!AppStarted)
            {
                AppStarted = true;
                BtnStart.Content = "STOP";
                Init();
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                AppStarted = false;
                BtnStart.Content = "START";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        public void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //Let the grains of sand fall down
            for (int I = 0; I < Grains.Count; I++)
            {
                if (Grains[I].Y >= ImageHeight - 1)
                {
                    Grains.RemoveAt(I);
                    I--;
                    continue;
                }
                if (GetPixel((int)Grains[I].X, (int)Grains[I].Y + 1, pixelData, Stride) == Colors.Black)
                {
                    SetPixel((int)Grains[I].X, (int)Grains[I].Y, Colors.Black, pixelData, Stride);
                    SetPixel((int)Grains[I].X, (int)Grains[I].Y + 1, myColors[I % myColors.Count], pixelData, Stride);
                    Grains[I] = new Vector(Grains[I].X, Grains[I].Y + 1);
                }
                else
                {
                    if (Rnd.NextDouble() > 0.5)
                    {
                        if (GetPixel((int)Grains[I].X - 1, (int)Grains[I].Y + 1, pixelData, Stride) == Colors.Black)
                        {
                            SetPixel((int)Grains[I].X, (int)Grains[I].Y, Colors.Black, pixelData, Stride);
                            SetPixel((int)Grains[I].X - 1, (int)Grains[I].Y + 1, myColors[I % myColors.Count], pixelData, Stride);
                            Grains[I] = new Vector(Grains[I].X - 1, Grains[I].Y + 1);
                        }
                    }
                    else
                    {
                        if (GetPixel((int)Grains[I].X + 1, (int)Grains[I].Y + 1, pixelData, Stride) == Colors.Black)
                        {
                            SetPixel((int)Grains[I].X, (int)Grains[I].Y, Colors.Black, pixelData, Stride);
                            SetPixel((int)Grains[I].X + 1, (int)Grains[I].Y + 1, myColors[I % myColors.Count], pixelData, Stride);
                            Grains[I] = new Vector(Grains[I].X + 1, Grains[I].Y + 1);
                        }
                    }
                }
            }
            //Show the updated Pixelarray
            BitmapSource bitmap = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, pf, null, pixelData, Stride);
            image1.Source = bitmap;
        }

        private void SetPixel(int x, int y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = x * 3;
            int yIndex = y * PixStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        private Color GetPixel(int x, int y, byte[] buffer, int PixStride)
        {
            Color result = new Color();
            int xIndex = x * 3;
            int yIndex = y * PixStride;
            result.R = buffer[xIndex + yIndex];
            result.G = buffer[xIndex + yIndex + 1];
            result.B = buffer[xIndex + yIndex + 2];
            result.A = 255;
            return result;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyMouseDown = true;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (MyMouseDown)
            {
                Point v = e.GetPosition(canvas1);
                //Add sand grains
                Grains.Add(new Vector(v.X, v.Y));
                Grains.Add(new Vector(v.X + 1, v.Y));
                Grains.Add(new Vector(v.X - 1, v.Y));
                Grains.Add(new Vector(v.X, v.Y + 1));
                Grains.Add(new Vector(v.X, v.Y - 1));
                Grains.Add(new Vector(v.X + 1, v.Y + 1));
                Grains.Add(new Vector(v.X + 1, v.Y - 1));
                Grains.Add(new Vector(v.X - 1, v.Y + 1));
                Grains.Add(new Vector(v.X - 1, v.Y - 1));
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyMouseDown = false;
        }
    }
}