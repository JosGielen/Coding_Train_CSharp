using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RayTracing
{
    internal class Ray
    {
        private Point my_Pos;
        private Vector my_Dir;
        private Line my_Line;
        //DEBUG CODE
        private double LineLength = 800;

        public Ray(Point pos, double angle)
        {
            my_Pos = pos;
            my_Dir = new Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180));
        }

        public Point Pos
        {
            get { return my_Pos; }
            set { my_Pos = value; }
        }

public Vector Dir
        {
            get { return my_Dir; }
            set { my_Dir = value; }
        }

public double X1
        {
            get { return my_Pos.X; }
        }

public double Y1
        {
            get { return my_Pos.Y; }
        }

public double X2
        {
            get { return (my_Pos + my_Dir).X; }
            set { my_Line.X2 = value; }
        }

public double Y2
        {
            get { return (my_Pos + my_Dir).Y; }
            set { my_Line.Y2 = value; }
        }

public void Show(Canvas c)
        {
            my_Line = new Line()
            {
                Stroke = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)), //Brushes.White,
                StrokeThickness = 1.0,
                X1 = my_Pos.X,
                Y1 = my_Pos.Y,
                X2 = (my_Pos + 800 * my_Dir).X,
                Y2 = (my_Pos + 800 * my_Dir).Y
            };
            c.Children.Add(my_Line);
}

        public void Update(Point Pos, double angle)
        {
            my_Pos = Pos;
            my_Line.X1 = my_Pos.X;
            my_Line.Y1 = my_Pos.Y;
            my_Dir = new Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180));
            my_Line.X2 = (my_Pos + LineLength * my_Dir).X;
            my_Line.Y2 = (my_Pos + LineLength * my_Dir).Y;
        }
    }
}
