using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DendriticGrowth
{

    public partial class MainWindow : Window
    {
        private int WaitTime = 1;
        private byte[] PixelData;
        private byte[] NewPixelData = new byte[3];
        private WriteableBitmap Writebitmap;
        private int Stride = 0;
        private List<Color> colorList;
        private BitmapPalette palet;
        private bool App_Started = false;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            //Set the Image size
            image1.Width = canvas1.ActualWidth;
            image1.Height = canvas1.ActualHeight;
            //Make a WriteableBitmap
            Stride = (int)(image1.Width * PixelFormats.Rgb24.BitsPerPixel / 8);
            PixelData = new byte[(int)(Stride * image1.Height)];
            colorList = new List<Color>()
            {
                Colors.Red,
                Colors.Green,
                Colors.Blue
            };
            palet = new BitmapPalette(colorList);
            Writebitmap = new WriteableBitmap(BitmapSource.Create((int)image1.Width, (int)image1.Height, 96, 96, PixelFormats.Rgb24, palet, PixelData, Stride));
            image1.Source = Writebitmap;
            Init();
        }

        private void Init()
        {
            //Make all pixels white
            for (int I = 0; I < PixelData.Length; I++)
            {
                PixelData[I] = 255;
            }
            //Set a seed in the middle
            int index;
            for (int I = (int)(Writebitmap.PixelWidth / 2) - 3; I <= (int)(Writebitmap.PixelWidth / 2) + 3; I++)
            {
                for (int J = (int)(Writebitmap.PixelHeight / 2) - 3; J <= (int)(Writebitmap.PixelHeight / 2) + 3; J++)
                {
                    index = (3 * (J * Writebitmap.PixelWidth + I));
                    PixelData[index] = 0;
                    PixelData[index + 1] = 0;
                    PixelData[index + 2] = 0;
                }
            }
            Int32Rect Intrect;
            Intrect = new Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1);
            Writebitmap.WritePixels(Intrect, PixelData, Stride, 0);
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e )
        {
            if (!App_Started)
            {
                App_Started = true;
                BtnStart.Content = "STOP";
                Render();
            }
            else
            {
                App_Started = false;
                BtnStart.Content = "START";
            }
        }

        private void Render()
        {
            int X;
            int Y;
            bool stuck;
            int dir;
            while (App_Started)
            {
                X = (int)(Writebitmap.PixelWidth * Rnd.NextDouble());
                Y = (int)(Writebitmap.PixelHeight * Rnd.NextDouble());
                while (true)
                {
                    if (X < 2) X = 2;
                    if (X > Writebitmap.PixelWidth - 2) X = Writebitmap.PixelWidth - 2;
                    if (Y < 2) Y = 2;
                    if (Y > Writebitmap.PixelHeight - 2) Y = Writebitmap.PixelHeight - 2;
                    stuck = false;
                    if (IsFixed(X - 1, Y - 1)) stuck = true;
                    if (IsFixed(X - 1, Y)) stuck = true;
                    if (IsFixed(X - 1, Y + 1)) stuck = true;
                    if (IsFixed(X, Y - 1)) stuck = true;
                    if (IsFixed(X, Y + 1)) stuck = true;
                    if (IsFixed(X + 1, Y - 1)) stuck = true;
                    if (IsFixed(X + 1, Y)) stuck = true;
                    if (IsFixed(X + 1, Y + 1)) stuck = true;
                    if (!App_Started | stuck)
                    {
                        break;
                    }
                    else
                    {
                        dir = Rnd.Next(0, 8);
                        switch (dir)
                        {
                            case 0:
                                X = X - 1;
                                Y = Y - 1;
                                break;
                            case 1:
                                X = X - 1;
                                break;
                            case 2:
                                X = X - 1;
                                Y = Y + 1;
                                break;
                            case 3:
                                Y = Y - 1;
                                break;
                            case 4:
                                Y = Y + 1;
                                break;
                            case 5:
                                X = X + 1;
                                Y = Y - 1;
                                break;
                            case 6:
                                X = X + 1;
                                break;
                            case 7:
                                X = X + 1;
                                Y = Y + 1;
                                break;
                        }
                    }
                }
                DrawPixel(X, Y);
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
            }
        }

        private bool IsFixed(int X, int Y )
        { 
            return PixelData[3 * (Y* Writebitmap.PixelWidth + X)] == 0;
        }

        private void DrawPixel(int X, int Y )
        {
            //Set the Pixeldata at position X,Y to 0
            int index = 3 * (Y * Writebitmap.PixelWidth + X);
            PixelData[index] = 0;
            PixelData[index + 1] = 0;
            PixelData[index + 2] = 0;
            //Make a rectangle of size 1,1
            Int32Rect rect = new Int32Rect(X, Y, 1, 1);
            //Set the pixel at X, Y to black
            NewPixelData[0] = 0;
            NewPixelData[1] = 0;
            NewPixelData[2] = 0;
            if (rect.X < Writebitmap.PixelWidth & rect.Y < Writebitmap.PixelHeight)
            {
                Writebitmap.WritePixels(rect, NewPixelData, Stride, 0);
            }
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }

        private void Window_Closing(Object sender, CancelEventArgs e )
        {
            Environment.Exit(0);
        }

    }
}
