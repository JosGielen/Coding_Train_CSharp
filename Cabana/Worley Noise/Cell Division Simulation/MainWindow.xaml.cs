using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Worley_Noise
{
    public partial class MainWindow : Window
    {
        private int W = 0;
        private int H = 0;
        private Vector Center;
        private List<Seed> Seeds;
        private readonly int closestIndex = 0;
        private BitmapSource bitmap;
        private byte[] PixelData;
        private int Stride = 0;
        private double[] distances;
        private double maxDistance;
        List<Color> my_Colors;
        private int frameCount = 0;
        private bool started = false;
        private bool Recording;
        private int ImageNumber;
        private readonly string ResultFileName = "CellDivision.gif";
        private readonly bool DeleteImages = false;
        private int WaitTime;

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
            Center = new Vector(W / 2, H / 2);
            Image1.Width = W;
            Image1.Height = H;
            maxDistance = W / 4;
            Stride = (int)(Image1.Width * PixelFormats.Rgb24.BitsPerPixel / 8);
            //Create the list of Colors.
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\ThermalGrayScale.cpl");
            my_Colors = pal.GetColors((int)maxDistance);
        }


        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (started)
            {
                started = false;
                BtnStart.Content = "START";
                WaitTime = 10;
                return;
            }
            else
            {
                Recording = CbRecording.IsChecked.Value;
                ImageNumber = 0;
                if (Recording)
                {
                    WaitTime = 100;
                }
                else
                {
                    WaitTime = 10;
                }
                started = true;
                BtnStart.Content = "STOP";
                Init();
                Render();
            }
        }

        private void Init()
        {
            PixelData = new byte[(int)(Stride * Image1.Height)];
            //Create the starting seed point
            Seeds = new List<Seed>
            {
                new Seed(0.5 * Canvas1.ActualWidth, 0.5 * Canvas1.ActualHeight, new Vector())
            };
        }

        private void Render()
        {
            Color my_Color;
            while (started)
            {
                distances = new double[Seeds.Count];
                //Render the image
                for (int X = 0; X < W; X++)
                {
                    for (int Y = 0; Y < H; Y++)
                    {
                        for (int I = 0; I < Seeds.Count; I++)
                        {
                            distances[I] = Math.Sqrt((Seeds[I].X - X) * (Seeds[I].X - X) + (Seeds[I].Y - Y) * (Seeds[I].Y - Y));
                        }
                        Array.Sort(distances);
                        if (distances[closestIndex] > maxDistance) distances[closestIndex] = maxDistance - 1;
                        if (distances[closestIndex] <= 0) distances[closestIndex] = 1;
                        my_Color = my_Colors[(int)(maxDistance - distances[closestIndex])];
                        SetPixel(X, Y, my_Color, PixelData, Stride);
                    }
                }
                bitmap = BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, null, PixelData, Stride);
                Image1.Source = bitmap;
                this.Dispatcher.Invoke(Wait, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                if (Recording)
                {
                    SaveImage();
                }
                if (Recording & Seeds.Count == 21)
                {
                    Recording = false;
                    started = false;
                    CbRecording.IsChecked = false;
                    MakeGif(15);
                }
                //Update the Seeds
                for (int I = 0; I < Seeds.Count; I++)
                {
                    Seeds[I].Update(50, 3, Center, Seeds);
                    //Seeds[I].Edges(W, H);
                }
                frameCount += 1;
                Title = Seeds.Count.ToString() + " Seeds";
                //Devide cells
                int devider;
                if (frameCount >= 45)
                {
                    devider = Rnd.Next(Seeds.Count);
                    Seeds.Add(Seeds[devider].Devide());
                    frameCount = 0;
                }
            }
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
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

        private void SaveImage()
        {
            DirectoryInfo dirInfo = Directory.CreateDirectory(Environment.CurrentDirectory + "\\output");
            string dir = dirInfo.FullName;
            string FileName = dir + "\\Image-" + ImageNumber.ToString("0000") + ".tiff";
            TiffBitmapEncoder MyEncoder = new TiffBitmapEncoder();
            try
            {
                MyEncoder.Frames.Add(BitmapFrame.Create(bitmap));
                FileStream sw = new FileStream(FileName, FileMode.Create);
                MyEncoder.Save(sw);
                sw.Close();
                ImageNumber++;
            }
            catch (Exception)
            {
                MessageBox.Show("The Image could not be saved.", "Worley Noise Cell Division Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Create an animated Gif from PNG images with ffmpeg.exe.
        /// </summary>
        /// <param name="frameRate">The desired framerate of the animated Gif</param>
        private void MakeGif(int frameRate)
        {
            //ffmpeg -i output\Image-0000.tiff -filter_complex "palettegen" palette.png
            //ffmpeg -framerate 15 -i output\Image-%4d.tiff -i palette.png -filter_complex "paletteuse" Worley.gif
            string prog = Environment.CurrentDirectory + "\\ffmpeg.exe";
            string args = " -i output\\Image-0000.tiff -filter_complex \"palettegen\" palette.png";
            if (File.Exists(Environment.CurrentDirectory + "\\palette.png")) File.Delete(Environment.CurrentDirectory + "\\palette.png");
            Process p = Process.Start(prog, args);
            p.WaitForExit();
            args = " -framerate " + frameRate.ToString() + " -i output\\Image-%4d.tiff -i palette.png -filter_complex \"paletteuse\" " + ResultFileName;
            p = Process.Start(prog, args);
            p.WaitForExit();
            //Delete the image files
            if (DeleteImages)
            {
                foreach (string f in Directory.GetFiles(Environment.CurrentDirectory + "\\output"))
                {
                    if (System.IO.Path.GetExtension(f) == ".tiff")
                    {
                        File.Delete(f);
                    }
                }
            }
        }

    }
}
