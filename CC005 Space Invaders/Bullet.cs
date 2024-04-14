using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Space_Invaders
{
    internal class Bullet
    {
        private bool my_Status;
        private double my_X;
        private double my_Y;
        private double my_Dir;  //negative when fired by the ship ; positive when fired by Invader 
        private Ellipse My_ellipse;

        public Bullet(double x, double y, double dir)
        {
            my_Status = true;
            my_X = x;
            my_Y = y;
            my_Dir = dir;
            if (dir > 0)
            {
                My_ellipse = new Ellipse()
                {
                    Fill = Brushes.Lime,
                    Height = 8,
                    Width = 8
                };
            }
            else
            {
                My_ellipse = new Ellipse()
                {
                    Fill = Brushes.Red,
                    Height = 4,
                    Width = 4
                };
            }

        }

        public double X
        {
            get { return my_X; }
        }

        public double Y
        {
            get { return my_Y; }
        }

        public void Update()
        {
            if (my_Status)
            {
                my_Y += my_Dir;
                My_ellipse.SetValue(Canvas.TopProperty, my_Y - 2.0);
            }
        }

        public void Draw(Canvas c)
        {
            My_ellipse.SetValue(Canvas.LeftProperty, my_X - 2.0);
            My_ellipse.SetValue(Canvas.TopProperty, my_Y - 2.0);
            c.Children.Add(My_ellipse);
        }

        public void Erase(Canvas c)
        {
            if (c.Children.Contains(My_ellipse))
            {
                c.Children.Remove(My_ellipse);
            }
        }

        public bool Hit(Invader inv)
        {
            if (my_Status && my_Dir < 0) //Bullet fired by the ship
            {
                return (my_X > inv.left && my_X < inv.right && my_Y < inv.bottom && my_Y > inv.top);
            }
            return false;
        }

        public bool Hit(double left, double right, double bottom, double top)
        {
            if (my_Status && my_Dir > 0) //Bullet fired by an Invader
            {
                return (my_X > left && my_X < right && my_Y < bottom && my_Y > top);
            }
            return false;
        }
    }
}
