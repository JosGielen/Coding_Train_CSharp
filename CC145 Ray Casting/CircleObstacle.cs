using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RayTracing
{
    public class CircleObstacle
    {
        private Point my_Pos;
        private Vector my_Speed;
        private double my_Radius;
        private double maxWidth;
        private double maxHeight;
        private Ellipse my_Ellipse;

        public CircleObstacle(Point pos, double radius, double angle, double speed)
        {
            my_Pos = pos;
            my_Radius = radius;
            my_Speed = speed * new Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180));
        }

        public Point Pos
        {
            get { return my_Pos; }
            set { my_Pos = value; }
        }

        public Vector Speed
        {
            get { return my_Speed; }
            set { my_Speed = value; }
        }

        public double Radius
        {
            get { return my_Radius; }
            set { my_Radius = value; }
        }

        public void Show(Canvas c)
        {
            my_Ellipse = new Ellipse()
            {
                Width = 2 * my_Radius,
                Height = 2 * my_Radius,
                Stroke = Brushes.White,
                StrokeThickness = 1.0,
                Fill = Brushes.Green
            };
            my_Ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Radius);
            my_Ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Radius);
            c.Children.Add(my_Ellipse);
            maxWidth = c.ActualWidth;
            maxHeight = c.ActualHeight;
        }

        public void Update()
        {
            my_Pos = my_Pos + my_Speed;
            //Check Wall collisions
            if (my_Pos.X < my_Radius)
            {
                my_Pos.X = my_Radius + 2;
                my_Speed.X = -1 * my_Speed.X;
            }
            else if (my_Pos.X > maxWidth - my_Radius)
            {
                my_Pos.X = maxWidth - my_Radius - 2;
                my_Speed.X = -1 * my_Speed.X;
            }
            if (my_Pos.Y < my_Radius)
            {
                my_Pos.Y = my_Radius + 2;
                my_Speed.Y = -1 * my_Speed.Y;
            }
            else if (my_Pos.Y > maxHeight - my_Radius)
            {
                my_Pos.Y = maxHeight - my_Radius - 2;
                my_Speed.Y = -1 * my_Speed.Y;
            }
            my_Ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Radius);
            my_Ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Radius);
        }

        public void Collide(CircleObstacle other)
        {
            if ((my_Pos - other.Pos).Length < (my_Radius + other.Radius))
            {
                Vector X1 = new Vector(my_Pos.X, my_Pos.Y);
                Vector X2 = new Vector(other.Pos.X, other.Pos.Y);
                double CP = (my_Speed - other.Speed) * (X1 - X2);
                Vector V1A;
                Vector V2A;
                V1A = my_Speed - CP / ((X1 - X2).Length * (X1 - X2).Length) * (X1 - X2);
                V2A = other.Speed - CP / ((X2 - X1).Length * (X2 - X1).Length) * (X2 - X1);
                my_Pos = other.Pos + 1.01 * (X1 - X2);
                other.Pos = my_Pos + 1.01 * (X2 - X1);
                my_Speed = V1A;
                other.Speed = V2A;
            }
        }

        public Point LineIntersect(Ray r)
        {
            if ((r.Pos -  my_Pos).Length < my_Radius) { return new Point(r.X1 + r.Dir.X, r.Y1 + r.Dir.Y); }
            double A1 = (r.Y2 - r.Y1) / (r.X2 - r.X1);
            double B1 = r.Y1 - A1 * r.X1;
            double A = A1 * A1 + 1;
            double B = 2 * A1 * (B1 - my_Pos.Y) - 2 * my_Pos.X;
            double C = my_Pos.X * my_Pos.X + (B1 - my_Pos.Y) * (B1 - my_Pos.Y) - my_Radius * my_Radius;
            double X1, X2;
            double X, Y;
            double Disc = B * B - 4 * A * C;
            if (Disc >= 0)
            {
                X1 = (-B + Math.Sqrt(Disc)) / (2 * A);
                X2 = (-B - Math.Sqrt(Disc)) / (2 * A);
                if (Math.Abs(X1 - r.X1) < Math.Abs(X2 - r.X1))
                {
                    X = X1;
                    Y = A1 * X1 + B1;
                }
                else
                {
                    X = X2;
                    Y = A1 * X2 + B1;
                }
                if ((r.X2 - r.X1) / (X - r.X1) > 0)
                {
                    return new Point(X, Y);
                }
                else
                {
                    return new Point(r.X1 + 800 * r.Dir.X, r.Y1 + 800 * r.Dir.Y);
                }
            }
            else
            {
                return new Point(r.X1 + 800 * r.Dir.X, r.Y1 + 800 * r.Dir.Y);
            }
        }

        public double Signeddistance(Point pt)
        {
            return (my_Pos - pt).Length - my_Radius;
        }
    }
}
