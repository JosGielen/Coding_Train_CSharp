using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Space_Invaders
{
    internal class Invader
    {
        private Image img;
        private double my_W;
        private double my_H;
        private double my_X;
        private double my_Y;
        private int my_Life;
        private bool my_Fire;
        private double dY;
        private bool my_Status;

        public static double dX;
        private static Random Rnd = new Random();

        public Invader(string imagefile, double X, double Y, double speed, bool canFire)
        {
            BitmapImage bitmap;
            bitmap = new BitmapImage(new Uri(imagefile));
            img = new Image();
            img.Source = bitmap;
            my_W = bitmap.Width;
            my_H = bitmap.Height;
            my_X = X;
            my_Y = Y;
            my_Life = 10;
            my_Fire = canFire;
            dY = speed;
            dX = 0.5;
        }

        public double width
        {
            get { return my_W; }
        }

        public double height
        {
            get { return my_H; }
        }

        public int Life
        {
            get { return my_Life; }
            set { my_Life = value; }
        }

        public bool Status
        {
            get { return my_Status; }
            set { my_Status = value; }
        }

        public double left
        {
            get { return my_X; }
        }

        public double right
        {
            get { return my_X + my_W; }
        }

        public double top
        {
            get { return my_Y; }
        }

        public double bottom
        {
            get { return my_Y + my_H; }
        }

        public void Update()
        {
            my_X += dX;
            my_Y += dY;
            img.SetValue(Canvas.LeftProperty, my_X);
            img.SetValue(Canvas.TopProperty, my_Y);
        }

        public void Draw(Canvas c)
        {
            img.SetValue(Canvas.LeftProperty, my_X);
            img.SetValue(Canvas.TopProperty, my_Y);
            c.Children.Add(img);
        }

        public void Erase(Canvas c)
        {
            if (c.Children.Contains(img))
            {
                c.Children.Remove(img);
            }
        }

        public bool CanFire()
        {
            if (!my_Fire) return false;
            if (Rnd.NextDouble() < 0.005) return true;
            return false;
        }
    }
}
