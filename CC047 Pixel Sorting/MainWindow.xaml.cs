using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Pixel_Sorting
{
    public partial class MainWindow : Window
    {
        private bool App_Started = false;
        private int WaitTime = 50;
        private delegate void WaitDelegate(int t);
        private BitmapImage bitmap1;
        private WriteableBitmap bitmap2;
        private int Stride;
        private byte[] PixelData1;
        private byte[] PixelData2;
        private List<int> Indices;
        private List<double> Hues;
        private bool UseHue = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CBHue.IsChecked = true;
            canvas2.Width = canvas1.ActualWidth;
            canvas2.Height = canvas1.ActualHeight;
            bitmap1 = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\dog.jpg"));
            Stride = bitmap1.PixelWidth * bitmap1.Format.BitsPerPixel / 8;
            PixelData1 = new byte[Stride * bitmap1.PixelHeight];
            bitmap1.CopyPixels(PixelData1, Stride, 0);
            canvas1.Background = new ImageBrush(bitmap1);
            Init();
        }

        private void Init()
        {
            Indices = new List<int>();
            Hues = new List<double>();
            bitmap2 = new WriteableBitmap(bitmap1);
            PixelData2 = new byte[Stride * bitmap1.PixelHeight];
            bitmap2.CopyPixels(PixelData2, Stride, 0);
            canvas2.Background = new ImageBrush(bitmap2);
        }


        private void Render()
        {
            double R,G,B;
            double min, max;
            double Hue = 0;
            int step = bitmap2.Format.BitsPerPixel / 8;
            //Step 1: Calculate the Hue value of each pixel
            for (int I = 0; I < PixelData2.Length; I += step)
            {
                if (UseHue)
                {
                    //Normalize RGB data
                    R = PixelData2[I] / 255.0;
                    G = PixelData2[I + 1] / 255.0;
                    B = PixelData2[I + 2] / 255.0;
                    //Calculate Hue
                    Hue = R + G + B;
                    Hue = 0;
                    min = 1000.0;
                    max = 0.0;
                    if (R < min) min = R;
                    if (G < min) min = G;
                    if (B < min) min = B;
                    if (R > max) max = R;
                    if (G > max) max = G;
                    if (B > max) max = B;
                    if (min == max) Hue = 0;
                    else if (R == max) Hue = ((G - B) / (max - min)) % 6;
                    else if (G == max) Hue = 2.0 + (B - R) / (max - min);
                    else if (B == max) Hue = 4.0 + (R - G) / (max - min);
                    Hue *= 60;
                    if (Hue < 0) Hue += 360;
                }
                else
                {
                    Hue = PixelData2[I] + PixelData2[I + 1] + PixelData2[I + 2];
                }
                Indices.Add(I);
                Hues.Add(Hue);
            }
            //Step 2: Sort the Hue values
            int minIndex;
            for (int I = 0; I < Hues.Count - 1; I++)
            {
                min = 1000000;
                minIndex = I;
                for (int J = I; J < Hues.Count - 1; J++)
                { 
                    if (Hues[J] < min)
                    {
                        min = Hues[J];
                        minIndex = J; 
                    }
                }
                SwapHues(I, minIndex);
                SwapPixels(step * I, step * minIndex);
                if (I % Stride == 0)
                {
                    if (!App_Started) { break; }
                    //Update canvas2 background 
                    Debug.Print("Pixel " + I.ToString());
                    Int32Rect rect = new Int32Rect(0, 0, bitmap2.PixelWidth, bitmap2.PixelHeight);
                    bitmap2.WritePixels(rect, PixelData2, Stride, 0);
                    canvas2.Background = new ImageBrush(bitmap2);
                    Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
                }
            }
            BtnStart.Content = "START";
            App_Started = false;
        }

        private void SwapHues(int i, int j)
        {
            double dummy = Hues[i];
            Hues[i] = Hues[j];
            Hues[j] = dummy;
        }

        private void SwapPixels(int I, int J)
        {
            byte dummy = PixelData2[I];
            PixelData2[I] = PixelData2[J];
            PixelData2[J] = dummy;
            dummy = PixelData2[I + 1];
            PixelData2[I + 1] = PixelData2[J + 1];
            PixelData2[J + 1] = dummy;
            dummy = PixelData2[I + 2];
            PixelData2[I + 2] = PixelData2[J + 2];
            PixelData2[J + 2] = dummy;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (! App_Started)
            {
                BtnStart.Content = "STOP";
                App_Started = true;
                Init();
                Render();
            }
            else
            {
                BtnStart.Content = "START";
                App_Started = false;
            }
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void CBBrightness_Click(object sender, RoutedEventArgs e)
        {
            if (CBBrightness.IsChecked == true)
            {
                CBHue.IsChecked = false;
                UseHue = false;
                BtnStart.Content = "START";
                App_Started = false;
            }
        }

        private void CBHue_Click(object sender, RoutedEventArgs e)
        {
            if (CBHue.IsChecked == true)
            {
                CBBrightness.IsChecked = false;
                UseHue = true;
                BtnStart.Content = "START";
                App_Started = false;
            }
        }
    }
}
