using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Worley_Noise
{
    public partial class MainWindow : Window
    {
        private int W = 0;
        private int H = 0;
        private readonly int SeedCount = 40;
        private List<Point> Seeds;
        private BitmapSource bitmap;
        private byte[] PixelData;
        private int Stride = 0;
        private double maxDistance;
        private int FileSaveFilterIndex = 1;
        private bool UseColor = false;
        private readonly Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Set the Image to fill the canvas.
            W = (int)Canvas1.ActualWidth;
            H = (int)Canvas1.ActualHeight;
            Image1.Width = W;
            Image1.Height = H;
            maxDistance = W / 4;
            //Create a bitmap to show in the Image
            Stride = (int)(Image1.Width * PixelFormats.Rgb24.BitsPerPixel / 8);
            //PixelData = new byte[(int)(Stride * Image1.Height)];
            //bitmap = BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, null, PixelData, Stride);
            //Image1.Source = bitmap;
            //Create the Noise
            WorleyNoise(); 
        }

        private void WorleyNoise()
        {
            Color my_Color;
            PixelData = new byte[(int)(Stride * Image1.Height)];
            //Create the seed points
            Seeds = new List<Point>();
            for (int I = 0; I < SeedCount; I++)
            {
                Seeds.Add(new Point(Rnd.NextDouble() * Canvas1.ActualWidth, Rnd.NextDouble() * Canvas1.ActualHeight));
            }
            if (UseColor )
            {
                double[] distances = new double[SeedCount];
                byte R, G, B;
                for (int X = 0; X < W; X++)
                {
                    for (int Y = 0; Y < H; Y++)
                    {
                        for (int I = 0; I < SeedCount; I++)
                        {
                            distances[I] = Math.Sqrt((Seeds[I].X - X) * (Seeds[I].X - X) + (Seeds[I].Y - Y) * (Seeds[I].Y - Y));
                        }
                        Array.Sort(distances);
                        if (distances[0] > maxDistance) distances[0] = maxDistance - 1;
                        if (distances[1] > maxDistance) distances[1] = maxDistance - 1;
                        if (distances[2] > maxDistance) distances[2] = maxDistance - 1;
                        R = (byte)(255 * (maxDistance - distances[1]) / maxDistance);
                        G = (byte)(255 * (maxDistance - distances[2]) / maxDistance);
                        B = (byte)(255 * (maxDistance - distances[0]) / maxDistance);

                        my_Color = Color.FromRgb(R, G, B);
                        SetPixel(X, Y, my_Color, PixelData, Stride);
                    }
                }
            }
            else
            {
                double[] distances = new double[SeedCount];
                ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\ThermalGrayScale.cpl");
                List<Color> my_Colors = pal.GetColors((int)maxDistance);
                for (int X = 0; X < W; X++)
                {
                    for (int Y = 0; Y < H; Y++)
                    {
                        for (int I = 0; I < SeedCount; I++)
                        {
                            distances[I] = Math.Sqrt((Seeds[I].X - X) * (Seeds[I].X - X) + (Seeds[I].Y - Y) * (Seeds[I].Y - Y));
                        }
                        Array.Sort(distances);
                        if (distances[0] > maxDistance) distances[0] = maxDistance - 1;
                        my_Color = my_Colors[(int)(maxDistance - distances[0])];
                        SetPixel(X, Y, my_Color, PixelData, Stride);
                    }
                }
            }

            bitmap = BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, null, PixelData, Stride);
            Image1.Source = bitmap;

        }

        private void SetPixel(int X, int Y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = X * 3;
            int yIndex = Y * PixStride;
            if (xIndex + yIndex >= 0 & xIndex + yIndex + 2 < buffer.Length)
            {
                buffer[xIndex + yIndex + 0] = c.R;
                buffer[xIndex + yIndex + 1] = c.G;
                buffer[xIndex + yIndex + 2] = c.B;
            }
        }

        private void MnuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Windows Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|TIFF (*.tiff)|*.tiff|PNG (*.png)|*.png",
                FilterIndex = FileSaveFilterIndex,
                RestoreDirectory = true
            };
            if (saveFileDialog1.ShowDialog().Value)
            {
                FileSaveFilterIndex = saveFileDialog1.FilterIndex;
                SaveImage(saveFileDialog1.FileName);
            }
        }

        private void SaveImage(string fileName)
        {
            BitmapEncoder MyEncoder;
            try
            {
                switch (FileSaveFilterIndex)
                {
                    case 1:
                        MyEncoder = new BmpBitmapEncoder();
                        break;
                    case 2:
                        MyEncoder = new JpegBitmapEncoder();
                        break;
                    case 3:
                        MyEncoder = new GifBitmapEncoder();
                        break;
                    case 4:
                        MyEncoder = new TiffBitmapEncoder();
                        break;
                    case 5:
                        MyEncoder = new PngBitmapEncoder();
                        break;
                    default:
                        //Should not occur
                        return;
                }
                MyEncoder.Frames.Add(BitmapFrame.Create(bitmap));
                //Create an instance of StreamWriter to write the histogram to the file.
                FileStream sw = new FileStream(fileName, FileMode.Create);
                MyEncoder.Save(sw);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("The Image could not be saved. Original error: " + Ex.Message, "Worley Noise error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuGray_Click(object sender, RoutedEventArgs e)
        {
            MnuGray.IsChecked = true;
            MnuColor.IsChecked = false;
            UseColor = false;
            WorleyNoise();
        }

        private void MnuColor_Click(object sender, RoutedEventArgs e)
        {
            MnuGray.IsChecked = false;
            MnuColor.IsChecked = true;
            UseColor = true;
            WorleyNoise();
        }
    }
}
