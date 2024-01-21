using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cellular_Automata
{
    public partial class MainWindow : Window
    {
        private bool AppStarted;
        private int ImageWidth = 0;
        private int ImageHeight = 0;
        private PixelFormat pf = PixelFormats.Rgb24;
        private int Stride = 0;
        private byte[] pixelData;
        private bool[] CurrentGen;
        private int Generation;
        private byte code;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            image1.Width = canvas1.ActualWidth;
            image1.Height = canvas1.ActualHeight;
            code = 1;
        }

        private void Init()
        {
            ImageWidth = (int)Math.Floor(image1.Width); ;
            ImageHeight = (int)Math.Floor(image1.Height); ;
            Stride = (int)((ImageWidth * pf.BitsPerPixel + 7) / 8);
            pixelData = new byte[Stride * ImageHeight];
            CurrentGen = new bool[ImageWidth];
            //Make the first generation
            for(int i = 0; i < ImageWidth; i++)
            {
                CurrentGen[i] = false;
            }
            CurrentGen[ImageWidth/2] = true;
            Generation = 0;
            //Use a random 8-bit code
            code = (byte)Rnd.Next(256);
            TxtCode.Text = code.ToString();
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
        }

        public void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //Place the current Generation in the PixelArray
            for (int i = 0; i < ImageWidth; i++)
            {
                if (CurrentGen[i])
                {
                    SetPixel(i, Generation, Colors.Yellow, pixelData, Stride);
                }
            }
            //Show the updated Pixelarray
            BitmapSource bitmap = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, pf, null, pixelData, Stride);
            image1.Source = bitmap;
            Generation++;
            if (Generation >= ImageHeight)
            {
                BtnStart.Content = "NEXT";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                AppStarted = false;
            }
            //Make the next Generation
            CurrentGen = MakeNextGen();
        }

        private bool[] MakeNextGen()
        {
            bool[] result = new bool[ImageWidth];
            string BinaryCode = Convert.ToString(code, 2);
            while (BinaryCode.Length < 8)
            {
                BinaryCode = "0" + BinaryCode;
            }
            int left, mid, right;
            for (int i = 0; i < ImageWidth; i++)
            {
                left = i - 1;
                mid = i;
                right= i + 1;
                //Wrap around
                if (left < 0) left = ImageWidth - 1;
                if (right == ImageWidth) right = 0;
                //Calculate the next state of cell i 
                int C1 = CurrentGen[left] ? 1 : 0;
                int C2 = CurrentGen[mid] ? 1 : 0;
                int C3 = CurrentGen[right] ? 1 : 0;
                result[i] = BinaryCode[7 - (4 * C1 + 2 * C2 + C3)] == '1';
            }
            return result;
        }

        private void SetPixel(int x, int y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = x * 3;
            int yIndex = y * PixStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }
    }
}