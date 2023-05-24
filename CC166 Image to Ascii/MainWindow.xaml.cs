using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;

namespace ASCII_Art
{
    public partial class MainWindow : Window
    {
        private readonly int waitTime = 100;
        private BitmapImage bitmap;
        private BitmapImage LowResBmp;
        private byte[] PixelData;
        private string density = "Ñ@W$9876543210?!abc;:+=-,._ ";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bitmap = new BitmapImage(new System.Uri(Environment.CurrentDirectory + "\\dog.jpg"));
            LowResBmp = new BitmapImage(new System.Uri(Environment.CurrentDirectory + "\\dogLR.jpg"));
            Original.Width = Canvas1.ActualWidth;
            Original.Height = Canvas1.ActualHeight;
            Original.Source = bitmap;
            Original.Stretch = Stretch.Uniform;
        }


        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            int Stride = LowResBmp.PixelWidth * LowResBmp.Format.BitsPerPixel / 8;
            double W = Canvas1.ActualWidth / LowResBmp.PixelWidth;
            double H = Canvas1.ActualHeight / LowResBmp.PixelHeight;
            double Avg;
            double max = 0.0;
            int stringIndex;
            Label lab;
            PixelData = new byte[Stride * LowResBmp.PixelHeight];
            LowResBmp.CopyPixels(PixelData, Stride, 0);
            int Index;
            byte R, G, B;
            string reverse = StringReverse(density);
            Canvas1.Background = Brushes.Black;
            for (int Y = 0; Y < LowResBmp.PixelHeight; Y++)
            {
                for (int X = 0; X < LowResBmp.PixelWidth; X++)
                {
                    Index = (LowResBmp.PixelWidth * Y + X) * LowResBmp.Format.BitsPerPixel / 8;
                    R = PixelData[Index + 2];
                    G = PixelData[Index + 1];
                    B = PixelData[Index + 0];
                    Avg = (R + G + B) / 3;
                    if (Avg > max) max = Avg;
                }
            }

            for (int Y = 0; Y < LowResBmp.PixelHeight; Y++)
            {
                for (int X = 0; X < LowResBmp.PixelWidth; X++)
                {
                    Index = (LowResBmp.PixelWidth * Y + X) * LowResBmp.Format.BitsPerPixel / 8;
                    R = PixelData[Index + 2];
                    G = PixelData[Index + 1];
                    B = PixelData[Index + 0];
                    Avg = (R + G + B) / 3;
                    stringIndex = (int)Math.Floor(reverse.Length * Avg / (max+2));
                    lab = new Label();
                    lab.Content = reverse[stringIndex];
                    lab.Width = W;
                    lab.Height = H;
                    lab.Padding = new Thickness(0);
                    lab.FontSize = 9;
                    lab.Background = Brushes.Black;
                    lab.Foreground = Brushes.White; 
                    lab.SetValue(Canvas.LeftProperty, X * W);
                    lab.SetValue(Canvas.TopProperty, Y * H);
                    Canvas1.Children.Add(lab);
                }
                Dispatcher.Invoke(Wait, DispatcherPriority.Background);
            }
        }

        private string StringReverse(string s)
        {
            string result = "";
            for (int I = 0; I < s.Length; I++)
            {
                result = s[I].ToString() + result;
            }
            return result;
        }

        private void Wait()
        {
            Thread.Sleep(waitTime);
        }
    }
}
