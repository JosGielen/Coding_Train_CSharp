using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace _2DCollisions
{
    class Ball
    {
        private Vector my_Pos;
        private double my_Angle;
        private double my_Radius;
        private double my_Mass;
        private Vector my_Velocity;
        private double maxWidth;
        private double maxHeight;
        private Brush my_Color;
        private Ellipse my_Shape;

        public Ball(Vector pos, double angle, double radius, double mass, double speed)
        {
            my_Pos = pos;
            my_Angle = angle;
            my_Radius = radius;
            my_Velocity = speed * new Vector(Math.Cos(my_Angle * Math.PI / 180), Math.Sin(my_Angle * Math.PI / 180));
            my_Mass = mass;
            my_Color = Brushes.Red;
        }

        public Vector Pos
        {
            get { return my_Pos; }
            set { my_Pos = value; }
        }

        public double Radius
        {
            get { return my_Radius; }
            set { my_Radius = value; }
        }

        public double Mass
        {
            get { return my_Mass; }
            set { my_Mass = value; }
        }

        public Vector Velocity
        {
            get { return my_Velocity; }
            set { my_Velocity = value; }
        }

        public double Speed
        {
            get { return my_Velocity.Length; }
        }

        public Brush Color
        {
            get { return my_Color; }
            set { my_Color = value; }
        }

        public void Show(Canvas c)
        {
            my_Shape = new Ellipse()
            {
                Width = 2 * my_Radius,
                Height = 2 * my_Radius,
                Stroke = my_Color,
                StrokeThickness = 1.0,
                Fill = my_Color
            };
            my_Shape.SetValue(Canvas.LeftProperty, my_Pos.X - my_Radius);
            my_Shape.SetValue(Canvas.TopProperty, my_Pos.Y - my_Radius);
            c.Children.Add(my_Shape);
            maxWidth = c.ActualWidth;
            maxHeight = c.ActualHeight;
        }

        public void Update(bool bounce)
        {
            my_Pos += my_Velocity;
            if (bounce)
            {
                if (my_Pos.X < my_Radius)
                {
                    my_Pos.X = my_Radius + 2;
                    my_Velocity.X = -1 * my_Velocity.X;
                }
                if (my_Pos.X > maxWidth - my_Radius)
                {
                    my_Pos.X = maxWidth - my_Radius - 2;
                    my_Velocity.X = -1 * my_Velocity.X;
                }
                if (my_Pos.Y < my_Radius)
                {
                    my_Pos.Y = my_Radius + 2;
                    my_Velocity.Y = -1 * my_Velocity.Y;
                }
                if (my_Pos.Y > maxHeight - my_Radius)
                {
                    my_Pos.Y = maxHeight - my_Radius - 2;
                    my_Velocity.Y = -1 * my_Velocity.Y;
                }
            }
            else
            {
                if (my_Pos.X < my_Radius)
                {
                    my_Pos.X = maxWidth - my_Radius - 2;
                }
                if (my_Pos.X > maxWidth - my_Radius)
                {
                    my_Pos.X = my_Radius + 2;
                }
                if (my_Pos.Y < my_Radius)
                {
                    my_Pos.Y = maxHeight - my_Radius - 2;
                }
                if (my_Pos.Y > maxHeight - my_Radius)
                {
                    my_Pos.Y = my_Radius + 2;
                }
            }
            my_Shape.SetValue(Canvas.LeftProperty, my_Pos.X - my_Radius);
            my_Shape.SetValue(Canvas.TopProperty, my_Pos.Y - my_Radius);
        }

        public void Collide(Ball other)
        {
            if ((my_Pos - other.Pos).Length < (my_Radius + other.Radius))
            {
                Vector P1 = my_Pos;
                Vector P2 = other.Pos;
                double CP = (my_Velocity - other.Velocity) * (P1 - P2) / Math.Pow((P1 - P2).Length, 2);
                Vector V1A = my_Velocity - 2 * other.Mass / (my_Mass + other.Mass) * CP * (P1 - P2);
                Vector V2A = other.Velocity - 2 * my_Mass / (my_Mass + other.Mass) * CP * (P2 - P1);
                my_Pos = other.Pos + 1.01 * (P1 - P2);
                other.Pos = my_Pos + 1.01 * (P2 - P1);
                my_Velocity = V1A;
                other.Velocity = V2A;
            }
        }
    }
}
