using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Monty_Hall_Problem
{
    internal class Door
    {
        private Canvas My_Canvas;
        private Label my_Label;
        private Image my_Image;
        private bool isWin;
        private bool isSelected;


        public Door(Canvas c, Label l, Image img) 
        {
            My_Canvas = c;
            my_Label = l;
            my_Image = img;
            isWin = false;
        }

        public void Init(bool Winning)
        {
            BitmapImage bmp;
            My_Canvas.Background = Brushes.Gray;
            my_Label.Visibility = Visibility.Visible;
            my_Image.Visibility = Visibility.Hidden;
            isWin = Winning;
            isSelected = false;
            if (Winning)
            {
                bmp = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Win.jpg"));
            }
            else
            {
                bmp = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Fail.jpg"));
            }
            my_Image.Source = bmp;
        }

        public bool Selected
        { 
            get { return isSelected; } 
            set 
            {  
                isSelected = value;
                if (isSelected)
                {
                    My_Canvas.Background = Brushes.Green;
                }
                else
                {
                    My_Canvas.Background = Brushes.Gray;
                }
            } 
        }

        public void ShowContent()
        {
            My_Canvas.Background = Brushes.White;
            my_Label.Visibility = Visibility.Hidden;
            my_Image.Visibility = Visibility.Visible;
        }
    }
}
