using System.Windows;

namespace Metaballs
{
    internal class Ball
    {
        private double my_X;
        private double my_Y;
        private double my_DirX;
        private double my_DirY;
        private double my_Size;

        public Ball(double size, Point Location, double dirX, double dirY)
        {
            my_Size = size;
            my_X = Location.X;
            my_Y = Location.Y;
            my_DirX = dirX;
            my_DirY = dirY;
        }

        public double X
        {
            get { return my_X; }
            set { my_X = value; }
        }

        public double Y
        {
            get { return my_Y; }
            set { my_Y = value; }
        }

        public double DirX
        {
            get { return my_DirX; }
            set { my_DirX = value; }
        }

        public double DirY
        {
            get { return my_DirY; }
            set { my_DirY = value; }
        }

        public void Update(int width, int height)
        {
            X += DirX;
            Y += DirY;
            if (X < 1.5 * my_Size | X > width - 1.5 * my_Size) DirX = -DirX;
            if (Y < 1.5 * my_Size | Y > height - 1.5 * my_Size) DirY = -DirY;
        }
    }
}
