using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Toothpick_Pattern
{
    internal class Toothpick
    {
        private Point my_Center;
        private int my_Length;
        private bool my_Horizontal;
        private Point my_End1;
        private Point my_End2;
        private bool my_End1Free;
        private bool my_End2Free;
        private Color my_Color;
        private bool my_Tested;


        public Toothpick(int X, int Y, int length, bool horizontal, Color c)
        {
            my_Center = new Point(X, Y);
            my_Length = length;
            my_Horizontal = horizontal;
            my_Color = c;
            if (my_Horizontal)
            {
                my_End1 = new Point(my_Center.X - my_Length, my_Center.Y);
                my_End2 = new Point(my_Center.X + my_Length, my_Center.Y);
            }
            else
            {
                my_End1 = new Point(my_Center.X, my_Center.Y - my_Length);
                my_End2 = new Point(my_Center.X, my_Center.Y + my_Length);
            }
            my_Tested = false;
        }

        public Point Center
        { 
            get { return my_Center; } 
        }

        public Point End1
        {
            get { return my_End1; }
        }

        public Point End2
        {
            get { return my_End2; }
        }

        public bool End1Free
        { 
            get { return my_End1Free; } 
        }

        public bool End2Free
        { 
            get { return my_End2Free; } 
        }

        public bool Horizontal
        { 
            get { return my_Horizontal; } 
        }

        public bool Tested
        { 
            get { return my_Tested; } 
        }

        public void Draw(Canvas c, double thickness)
        {
            Line L = new Line()
            {
                X1 = my_End1.X,
                X2 = my_End2.X,
                Y1 = my_End1.Y,
                Y2 = my_End2.Y,
                Stroke = new SolidColorBrush(my_Color),
                StrokeThickness = thickness
            };
            c.Children.Add(L);
        }

        public void CheckEnds(List<Toothpick> picks)
        {
            if (my_Tested) 
            {
                my_End1Free = false;
                my_End2Free = false;
                return; 
            }
            my_End1Free = true;
            my_End2Free = true;
            for (int i = 0; i < picks.Count; i++)
            {
                if (my_Center != picks[i].Center)
                {
                    if (my_End1 == picks[i].End1 || my_End1 == picks[i].End2) 
                    { 
                        my_End1Free = false; 
                    }
                    if (my_End2 == picks[i].End1 || my_End2 == picks[i].End2) 
                    { 
                        my_End2Free = false; 
                    }
                }
            }
            my_Tested = true;
        }
    }
}
