using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;

namespace Slide_Puzzle
{
    class Slide
    {
        public int my_Left;
        public int my_Top;
        public int my_Width;
        public int my_Height;
        public int my_CurrentRow;
        public int my_CurrentCol;
        public Image my_Img;

        public Slide(int col, int row, WriteableBitmap bmp)
        {
            my_Width = bmp.PixelWidth;
            my_Height = bmp.PixelHeight;
            my_Img = new Image();
            my_Img.Source = bmp;
            my_Left = my_Width * col;
            my_Top = my_Height * row;
            my_CurrentRow = row;
            my_CurrentCol = col;
            my_Img.SetValue(Canvas.LeftProperty, (double)my_Left);
            my_Img.SetValue(Canvas.TopProperty, (double)my_Top);
        }

        public void SetPosition(int col, int row)
        {
            my_CurrentRow = row;
            my_CurrentCol = col;
        }

        public void SetImageSource(ImageSource newSource)
        {
            my_Img.Source = newSource;
        }

        public ImageSource GetImageSource()
        {
            return my_Img.Source;
        }

        public void Show(Canvas cnv)
        {
            if (!cnv.Children.Contains(my_Img))
            {
                cnv.Children.Add(my_Img);
            }
            my_Img.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            my_Img.Visibility = Visibility.Hidden;
        }
    }
}
