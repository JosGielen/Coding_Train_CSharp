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
    internal class Blob
    {
        private Ellipse my_Shape;
        private double my_Radius;
        private Color my_Color;
        private Point my_Location;
        private double my_Energy;

        public Blob(double x, double y, double r, Color c) 
        {
            my_Shape = new Ellipse()
            {
                Width = 2 * r,
                Height = 2 * r,
                Fill = new SolidColorBrush(c)
            };
            my_Location = new Point(x, y);
            my_Radius = r;
            my_Color = c;
            my_Energy = 0;
        }

        #region "Properties"

        public double Radius
        { 
            get { return my_Radius; } 
            set 
            { 
                my_Radius = value; 
                my_Shape.Width = 2 * my_Radius;
                my_Shape.Height = 2 * my_Radius;
                my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Radius);
                my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Radius);
            }
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
            get { return my_Location.X;} 
            set 
            { 
                my_Location.X = value;
                my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Radius);
            }
        }
        public double Y
        { 
            get { return my_Location.Y;}
            set 
            { 
                my_Location.Y = value;
                my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Radius);
            }
        }
        public Point Location
        {
            get { return my_Location; }
            set
            {
                my_Location = value;
                my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Radius);
                my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Radius );
            }
        }

        public double Energy
        {
            get { return my_Energy; }
            set { my_Energy = value; }
        }

        #endregion

        public void Draw(Canvas c)
        {
            my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Radius);
            my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Radius);
            c.Children.Add(my_Shape);
        }

        public void Erase(Canvas c)
        {
            if (my_Shape != null) { c.Children.Remove(my_Shape); }
        }
    }
}
