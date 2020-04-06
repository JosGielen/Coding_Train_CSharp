using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Floyd_Steinberg
{
    public partial class MainWindow : Window
    {
        int ColorNum = 0;
        BitmapImage bitmap;
        byte[] PixelData;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnColorNumUP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorNum = int.Parse(TBColorNum.Text);
                ColorNum += 1;
                if (ColorNum > 255) ColorNum = 255;

                TBColorNum.Text = ColorNum.ToString();
                if (CbGrayscale.IsChecked.Value)
                {
                    TxtColorTotal.Text =ColorNum.ToString();
                }
                else
                {
                    TxtColorTotal.Text = Math.Pow(ColorNum, 3).ToString();
                }
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        private void BtnColorNumDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorNum = int.Parse(TBColorNum.Text);
                ColorNum -= 1;
                if (ColorNum < 2) ColorNum = 2;
                TBColorNum.Text = ColorNum.ToString();
                if (CbGrayscale.IsChecked.Value)
                {
                    TxtColorTotal.Text = ColorNum.ToString();
                }
                else
                {
                    TxtColorTotal.Text = Math.Pow(ColorNum, 3).ToString();
                }
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        private void CbGrayscale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorNum = int.Parse(TBColorNum.Text);
                if (CbGrayscale.IsChecked.Value)
                {
                    TxtColorTotal.Text = ColorNum.ToString();
                    Original.Source = new BitmapImage(new System.Uri(Environment.CurrentDirectory + "\\KittenGray.jpg"));
                }
                else
                {
                    TxtColorTotal.Text = Math.Pow(ColorNum, 3).ToString();
                    Original.Source = new BitmapImage(new System.Uri(Environment.CurrentDirectory + "\\Kitten.jpg"));
                }
            }
            catch (Exception)
            {
                //Do nothing
            }

        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (CbGrayscale.IsChecked.Value)
            {
                bitmap = new BitmapImage(new System.Uri(Environment.CurrentDirectory + "\\KittenGray.jpg"));
            }
            else
            {
                bitmap = new BitmapImage(new System.Uri(Environment.CurrentDirectory + "\\Kitten.jpg"));
            }
            int Stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            PixelData = new byte[Stride * bitmap.PixelHeight];
            bitmap.CopyPixels(PixelData, Stride, 0);
            ColorNum = int.Parse(TBColorNum.Text) - 1;
            int Index;
            int newIndex;
            byte oldR, oldG, oldB;
            byte newR, newG, newB;
            int errR, errG, errB;
            double ditherR, ditherG, ditherB;
            errR = 0;
            errG = 0;
            errB = 0;
            for (int Y = 0; Y < bitmap.PixelHeight - 1; Y++)
            {
                for (int X = 1; X < bitmap.PixelWidth - 1; X++)
                {
                    Index = (bitmap.PixelWidth * Y + X) * bitmap.Format.BitsPerPixel / 8;
                    if (CbGrayscale.IsChecked.Value)
                    {
                        oldB = PixelData[Index];
                        newB = (byte)(255 * Math.Round(ColorNum * oldB / 255.0) / ColorNum);
                        errB = oldB - newB;
                        PixelData[Index] = newB;
                    }
                    else
                    {
                        oldR = PixelData[Index + 2];
                        oldG = PixelData[Index + 1];
                        oldB = PixelData[Index + 0];
                        newR = (byte)(255 * Math.Round(ColorNum * oldR / 255.0) / ColorNum);
                        newG = (byte)(255 * Math.Round(ColorNum * oldG / 255.0) / ColorNum);
                        newB = (byte)(255 * Math.Round(ColorNum * oldB / 255.0) / ColorNum);
                        errR = oldR - newR;
                        errG = oldG - newG;
                        errB = oldB - newB;
                        PixelData[Index + 2] = newR;
                        PixelData[Index + 1] = newG;
                        PixelData[Index + 0] = newB;
                    }
                    if (CbDithering.IsChecked.Value)
                    {
                        //7/16th part of the Error goes to pixel (X + 1, Y)
                        newIndex = (bitmap.PixelWidth * Y + X + 1) * bitmap.Format.BitsPerPixel / 8;
                        if (CbGrayscale.IsChecked.Value)
                        {
                            ditherB = Math.Max(PixelData[newIndex] + errB * 7 / 16.0, 0);
                            PixelData[newIndex] = (byte)ditherB;
                        }
                        else
                        {
                            ditherR = Math.Max(PixelData[newIndex + 2] + errR * 7 / 16.0, 0);
                            ditherG = Math.Max(PixelData[newIndex + 1] + errG * 7 / 16.0, 0);
                            ditherB = Math.Max(PixelData[newIndex + 0] + errB * 7 / 16.0, 0);
                            PixelData[newIndex + 2] = (byte)ditherR;
                            PixelData[newIndex + 1] = (byte)ditherG;
                            PixelData[newIndex + 0] = (byte)ditherB;
                        }
                        //3/16th part of the Error goes to pixel (X - 1, Y + 1)
                        newIndex = (bitmap.PixelWidth * (Y + 1) + X - 1) * bitmap.Format.BitsPerPixel / 8;
                        if (CbGrayscale.IsChecked.Value)
                        {
                            ditherB = Math.Max(PixelData[newIndex] + errB * 3 / 16.0, 0);
                            PixelData[newIndex] = (byte)ditherB;
                        }
                        else
                        {
                            ditherR = Math.Max(PixelData[newIndex + 2] + errR * 3 / 16.0, 0);
                            ditherG = Math.Max(PixelData[newIndex + 1] + errG * 3 / 16.0, 0);
                            ditherB = Math.Max(PixelData[newIndex + 0] + errB * 3 / 16.0, 0);
                            PixelData[newIndex + 2] = (byte)ditherR;
                            PixelData[newIndex + 1] = (byte)ditherG;
                            PixelData[newIndex + 0] = (byte)ditherB;
                        }
                        //5/16th part of the Error goes to pixel (X, Y + 1)
                        newIndex = (bitmap.PixelWidth * (Y + 1) + X) * bitmap.Format.BitsPerPixel / 8;
                        if (CbGrayscale.IsChecked.Value)
                        {
                            ditherB = Math.Max(PixelData[newIndex] + errB * 5 / 16.0, 0);
                            PixelData[newIndex] = (byte)ditherB;
                        }
                        else
                        {
                            ditherR = Math.Max(PixelData[newIndex + 2] + errR * 5 / 16.0, 0);
                            ditherG = Math.Max(PixelData[newIndex + 1] + errG * 5 / 16.0, 0);
                            ditherB = Math.Max(PixelData[newIndex + 0] + errB * 5 / 16.0, 0);
                            PixelData[newIndex + 2] = (byte)ditherR;
                            PixelData[newIndex + 1] = (byte)ditherG;
                            PixelData[newIndex + 0] = (byte)ditherB;
                        }
                        //1/16th part of the Error goes to pixel (X + 1, Y + 1)
                        newIndex = (bitmap.PixelWidth * (Y + 1) + X + 1) * bitmap.Format.BitsPerPixel / 8;
                        if (CbGrayscale.IsChecked.Value)
                        {
                            ditherB = Math.Max(PixelData[newIndex] + errB * 1 / 16.0, 0);
                            PixelData[newIndex] = (byte)ditherB;
                        }
                        else
                        {
                            ditherR = Math.Max(PixelData[newIndex + 2] + errR * 1 / 16.0, 0);
                            ditherG = Math.Max(PixelData[newIndex + 1] + errG * 1 / 16.0, 0);
                            ditherB = Math.Max(PixelData[newIndex + 0] + errB * 1 / 16.0, 0);
                            PixelData[newIndex + 2] = (byte)ditherR;
                            PixelData[newIndex + 1] = (byte)ditherG;
                            PixelData[newIndex + 0] = (byte)ditherB;
                        }
                    }
                }
            }
            Int32Rect Intrect = new Int32Rect(0, 0, bitmap.PixelWidth - 1, bitmap.PixelHeight - 1);
            WriteableBitmap newBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX,bitmap.DpiY,bitmap.Format,bitmap.Palette);
            newBitmap.WritePixels(Intrect, PixelData, Stride, 0);
            Dithered.Source = newBitmap;
        }
    }
}
