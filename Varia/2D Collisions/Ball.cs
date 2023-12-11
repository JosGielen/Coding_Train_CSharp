using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace _2DCollisions
{
    class Ball
    {
        private Point my_Pos;
        private double my_Angle;
        private double my_Radius;
        private double my_Mass;
        private Vector my_Speed;
        private double maxWidth;
        private double maxHeight;
        private Brush my_Color;
        private Ellipse my_Shape;

        public Ball(Point pos, double angle, double radius, double mass, double speed)
        {
            my_Pos = pos;
            my_Angle = angle;
            my_Radius = radius;
            my_Speed = new Vector(Math.Cos(my_Angle * Math.PI / 180), Math.Sin(my_Angle * Math.PI / 180));
            my_Mass = mass;
            my_Color = Brushes.Red; 
        }

        public Point Pos
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

        public Vector Speed
        {
            get { return my_Speed; }
            set { my_Speed = value; }
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

        public void Update()
        {
            my_Pos += my_Speed;
            if (my_Pos.X < my_Radius )
            {
                my_Pos.X = my_Radius + 2;
                my_Speed.X = -1 * my_Speed.X;
            }
            if (my_Pos.X > maxWidth - my_Radius )
            {
                my_Pos.X = maxWidth - my_Radius - 2;
                my_Speed.X = -1 * my_Speed.X;
            }
            if (my_Pos.Y < my_Radius)
            {
                my_Pos.Y = my_Radius + 2;
                my_Speed.Y = -1 * my_Speed.Y;
            }
            if (my_Pos.Y > maxHeight  - my_Radius)
            {
                my_Pos.Y = maxHeight  - my_Radius - 2;
                my_Speed.Y = -1 * my_Speed.Y;
            }
            my_Shape.SetValue(Canvas.LeftProperty, my_Pos.X - my_Radius);
            my_Shape.SetValue(Canvas.TopProperty, my_Pos.Y - my_Radius);
        }

        public void Collide(Ball other)
        {
            if ((my_Pos - other.Pos).Length < (my_Radius + other.Radius))
            {
                Vector X1 = new Vector(my_Pos.X, my_Pos.Y);
                Vector X2 = new Vector(other.Pos.X, other.Pos.Y);
                double CP = (my_Speed - other.Speed) * (X1 - X2);
                Vector V1A = my_Speed - 2 * other.Mass / (my_Mass + other.Mass) * CP / Math.Pow((X1 - X2).Length , 2) * (X1 - X2);
                Vector V2A = other.Speed - 2 * my_Mass / (my_Mass + other.Mass) * CP / Math.Pow((X2 - X1).Length , 2) * (X2 - X1);
                my_Pos = other.Pos + 1.01 * (X1 - X2);
                other.Pos = my_Pos + 1.01 * (X2 - X1);
                my_Speed = V1A;
                other.Speed = V2A;
            }
        }
    }
}
