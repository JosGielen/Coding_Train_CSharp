using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Agar
{
    internal class Hunter
    {
        private Polygon my_Shape;
        private double my_Radius;
        private Color my_Color;
        private Point my_Location;
        private double my_Energy;
        private Vector my_Dir;
        private Random Rnd = new Random();

        public Hunter(double x, double y, double r, Color c)
        {
            my_Shape = new Polygon()
            {
                Fill = new SolidColorBrush(c)
            };
            double mag;
            for (int i = 0; i < 20; i++)
            {
                if (i % 2 == 0)
                {
                    mag = r;
                }
                else
                {
                    mag = r / 3;
                }
                my_Shape.Points.Add(new Point(mag * Math.Cos(i * Math.PI / 10), mag * Math.Sin(i * Math.PI / 10)));
            }
            my_Shape.Points.Add(my_Shape.Points[0]);
            my_Location = new Point(x, y);
            my_Dir = new Vector(2 * Rnd.NextDouble() - 1, 2 * Rnd.NextDouble() - 1);
            my_Radius = r;
            my_Color = c;
            my_Energy = 0;
        }

        #region "Properties"

        public double Radius
        {
            get { return my_Radius; }
        }

        public Color Color
        {
            get { return my_Color; }
            set
            {
                my_Color = value;
                my_Shape.Fill = new SolidColorBrush(my_Color);
            }
        }

        public double X
        {
            get { return my_Location.X; }
            set
            {
                my_Location.X = value;
                my_Shape.SetValue(Canvas.LeftProperty, my_Location.X);
            }
        }
        public double Y
        {
            get { return my_Location.Y; }
            set
            {
                my_Location.Y = value;
                my_Shape.SetValue(Canvas.TopProperty, my_Location.Y);
            }
        }
        public Point Location
        {
            get { return my_Location; }
            set
            {
                my_Location = value;
                my_Shape.SetValue(Canvas.LeftProperty, my_Location.X);
                my_Shape.SetValue(Canvas.TopProperty, my_Location.Y);
            }
        }

        public Vector Dir
        {
            get { return my_Dir; }
            set { my_Dir = value; }
        }

        public double Energy
        {
            get { return my_Energy; }
            set { my_Energy = value; }
        }

        #endregion

        public void Draw(Canvas c)
        {
            my_Shape.SetValue(Canvas.LeftProperty, my_Location.X);
            my_Shape.SetValue(Canvas.TopProperty, my_Location.Y);
            c.Children.Add(my_Shape);
        }

        public void Erase(Canvas c)
        {
            if (my_Shape != null) { c.Children.Remove(my_Shape); }
        }

    }
}
