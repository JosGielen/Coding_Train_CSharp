using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Particle_System
{
    internal class Particle
    {
        private double my_X;
        private double my_Y;
        private double my_VX;
        private double my_VY;
        private double my_Size;
        private double my_Alpha;
        private Ellipse my_Ellipse;

        public Particle(double X, double Y, double VX, double VY, double size, Color c)
        {
            my_X = X;
            my_Y = Y;
            my_VX = VX;
            my_VY = VY;
            my_Size = size;
            my_Alpha = 100.0;
            my_Ellipse = new Ellipse()
            {
                Width = size,
                Height = size,
                Fill = new SolidColorBrush(Color.FromArgb((byte)my_Alpha, c.R, c.G, c.B))
            };
            my_Ellipse.SetValue(Canvas.LeftProperty, my_X - my_Size / 2);
            my_Ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2);
        }

        public double Alpha
        {
            get { return my_Alpha; }
        }

        public void Update(double dVX)
        {
            my_VX += dVX;
            my_X += my_VX;
            my_Y -= my_VY;
            my_Size += 0.06 * my_VY;
            my_Ellipse.Width = my_Size;
            my_Ellipse.Height = my_Size;
            my_Ellipse.SetValue(Canvas.LeftProperty, my_X - my_Size / 2);
            my_Ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2);
            my_Alpha *= 0.99;
            if (my_Alpha < 1) my_Alpha = 0;
            my_Ellipse.Fill.Opacity = my_Alpha / 100;
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(my_Ellipse);
        }
    }
}
