using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CirclePacking
{
    public class Circle
    {
        private Point my_Center;
        private double my_Radius;
        private Brush my_Color;
        private Ellipse my_Ellipse;
        private Rect my_Border;

        public Circle (double X, double Y)
        {
            my_Center = new Point(X, Y);
            my_Radius = 1.0;
            my_Color = Brushes.Black;
            my_Ellipse = new Ellipse()
            {
                Width = 2 * my_Radius,
                Height = 2 * my_Radius,
                Stroke = my_Color,
                StrokeThickness = 1
            };
            my_Ellipse.SetValue(Canvas.LeftProperty, X - my_Radius);
            my_Ellipse.SetValue(Canvas.TopProperty, Y - my_Radius);
            my_Border = new Rect();
        }

        public Point Center
        {
            get { return my_Center; }
        }

        public double Radius
        {
            get { return my_Radius; }
        }


        public Ellipse Shape
        {
            get { return my_Ellipse; }
        }

        public void Draw(Canvas can)
        {
            my_Border.X = 0;
            my_Border.Y = 0;
            my_Border.Width = can.ActualWidth;
            my_Border.Height = can.ActualHeight;
            can.Children.Add(my_Ellipse);
        }

        public void Grow(double stepSize)
        {
            my_Radius += stepSize;
            my_Ellipse.Width = 2 * my_Radius;
            my_Ellipse.Height = 2 * my_Radius;
            my_Ellipse.SetValue(Canvas.LeftProperty, my_Center.X - my_Radius);
            my_Ellipse.SetValue(Canvas.TopProperty, my_Center.Y - my_Radius);
        }

        public bool Overlap(Circle other, double spacing)
        {
            double dist = Math.Sqrt((other.Center.X - my_Center.X) * (other.Center.X - my_Center.X) + (other.Center.Y - my_Center.Y) * (other.Center.Y - my_Center.Y));
            return dist < my_Radius + other.Radius + spacing;
        }

        public bool CanGrow()
        {
            bool result = true;
            if (my_Radius >= my_Center.X | my_Radius + my_Center.X >= my_Border.X + my_Border.Width) result = false;
            if (my_Radius >= my_Center.Y | my_Radius + my_Center.Y >= my_Border.Y + my_Border.Height) result = false;
            return result;
        }
    }
}
