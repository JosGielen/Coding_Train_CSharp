using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BouncingDVDLogo
{
    public partial class MainWindow : Window
    {
        private Random Rnd = new Random();
        private byte[] cb = new byte[3];
        private double X;
        private double Y;
        private double dX;
        private double dY;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmap;
            FormatConvertedBitmap convertBitmap;
            WriteableBitmap Writebitmap;
            bitmap = new BitmapImage(new System.Uri(Environment.CurrentDirectory + "\\DVD-Logo.png"));
            //Change to 32 bpp if needed
            if (bitmap.Format.BitsPerPixel != 32)
            {
                convertBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
                Writebitmap = new WriteableBitmap(convertBitmap);
            }
            else
            {
                Writebitmap = new WriteableBitmap(bitmap);
            }
            //Show the picture in a WPF Image control
            image1.Source = Writebitmap;
            X = (canvas1.ActualWidth - ImgCanvas.ActualWidth) * Rnd.NextDouble();
            Y = (canvas1.ActualHeight - ImgCanvas.ActualHeight) * Rnd.NextDouble();
            dX = 2.0;
            dY = 2.0;
            ImgCanvas.Width = Writebitmap.PixelWidth;
            ImgCanvas.Height = Writebitmap.PixelHeight;
            ImgCanvas.SetValue(Canvas.LeftProperty, X);
            ImgCanvas.SetValue(Canvas.TopProperty, Y);
            Rnd.NextBytes(cb);
            ImgCanvas.Background = new SolidColorBrush(Color.FromRgb(cb[0], cb[1], cb[2]));
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            bool colorchange = false;
            X += dX;
            Y += dY;
            if (X < 0)
            {
                X = 0;
                dX = -dX;
                colorchange = true;
            }
            if (X > canvas1.ActualWidth - ImgCanvas.ActualWidth)
            {
                X = canvas1.ActualWidth - ImgCanvas.ActualWidth;
                dX = -dX;
                colorchange = true;
            }
            if (Y < 0)
            {
                Y = 0;
                dY = -dY;
                colorchange = true;
            }
            if (Y > canvas1.ActualHeight - ImgCanvas.ActualHeight)
            {
                Y = canvas1.ActualHeight - ImgCanvas.ActualHeight;
                dY = -dY;
                colorchange = true;
            }
            ImgCanvas.SetValue(Canvas.LeftProperty, X);
            ImgCanvas.SetValue(Canvas.TopProperty, Y);
            if (colorchange)
            {
                Rnd.NextBytes(cb);
                ImgCanvas.Background = new SolidColorBrush(Color.FromRgb(cb[0], cb[1], cb[2]));
            }
        }
    }
}
