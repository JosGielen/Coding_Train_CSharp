using Perlin_Noise;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fire_Effect
{
    public partial class MainWindow : Window
    {
        int W, H;
        double[,] Buffer1;
        double[,] Buffer2;
        double[,] Cooling;
        private WriteableBitmap bmp;
        private byte[] PixelData1;
        private int Stride = 0;
        private double Xoff, Yoff, Zoff;
        private double XoffInc = 0.025;
        private double YoffStart = 40.0;
        private double YoffInc = 0.03;
        private double CoolingStrength = 5.0;
        private List<Color> colors = new List<Color>();
        //Gif Loop data
        private int Percent = 0;
        private int CycleCounter = 0;
        private bool recording = false;
        private string ResultFileName = "FireGifLoop.gif";
        BitmapEncoder MyEncoder = new GifBitmapEncoder();

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
            //Load the Fire colors
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Fire.cpl");
            colors = pal.GetColors(512);
            //Create a bitmap to show in the Image
            Stride = W * PixelFormats.Rgb24.BitsPerPixel / 8;
            PixelData1 = new byte[Stride * H];
            bmp = new WriteableBitmap(W, H, 96, 96, PixelFormats.Rgb24, null);
            //Initialize the buffers
            Buffer1 = new double[W, H];
            Buffer2 = new double[W, H];
            Cooling = new double[W, H];
            Xoff = 0.0;
            Yoff = 0.0;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void StartFire()
        {
            //Set the fire start Line
            for (int i = 0; i < W; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    Buffer1[i, H - j] = 255;
                    Buffer2[i, H - j] = 255;
                }
            }
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            StartFire();
            Yoff = YoffStart;
            Zoff = 2 * Math.Sin(2 * Math.PI * Percent / 300);
            for (int Y = 1; Y < H - 1; Y++)
            {
                Xoff = 0;
                for(int X = 1; X < W - 1; X++)
                {
                    Buffer2[X, Y - 1] = Buffer1[X, Y] - CoolingStrength * PerlinNoise.WideNoise3D(Xoff, Yoff, Zoff, 2, 0.5, 1);
                    if (Buffer2[X, Y - 1] < 0) { Buffer2[X, Y - 1] = 0; }
                    SetGrayPixel(X, Y, (byte)Buffer2[X, Y - 1], PixelData1, PixelFormats.Rgb24.BitsPerPixel, Stride);
                    Xoff += XoffInc;
                }
                Yoff += YoffInc;
            }
            //Swap the buffers
            double[,] temp = Buffer1;
            Buffer1 = Buffer2;
            Buffer2 = temp;
            //Show the new Data in image1
            bmp.WritePixels(new Int32Rect(0, 0, W, H), PixelData1, Stride, 0);
            image1.Source = bmp;
            Percent += 1;
            if (recording)
            {
                //Add the canvas content as a BitmapFrame to the GifBitmapEncoder
                RenderTargetBitmap renderbmp = new RenderTargetBitmap((int)(canvas1.ActualWidth), (int)(canvas1.ActualHeight), 96, 96, PixelFormats.Default);
                renderbmp.Render(canvas1);
                MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp));
                Thread.Sleep(50);
            }
            if (Percent > 300)
            {
                if (recording)
                {
                    // Create a FileStream to write the image to the file.
                    FileStream sw = new FileStream(ResultFileName, FileMode.Create);
                    if (sw != null)
                    {
                        MyEncoder.Save(sw);
                    }
                    Environment.Exit(0);
                }
                Percent = 0;
                //Wait 2 cycles to get the complete fire going before recording.
                CycleCounter++;
                if (CycleCounter == 2)
                {
                    recording = true;
                }
            }
        }

        private void SetGrayPixel(int x, int y, byte ColorIndex, byte[] buffer, int BitsperPixel, int PixStride)
        {
            int xIndex = x * BitsperPixel / 8;
            int yIndex = y * PixStride;
            buffer[xIndex + yIndex] = colors[ColorIndex].R;
            buffer[xIndex + yIndex + 1] = colors[ColorIndex].G;
            buffer[xIndex + yIndex + 2] = colors[ColorIndex].B;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}