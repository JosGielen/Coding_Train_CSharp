using System.Numerics;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Apollonian_Gasket
{
    internal class Circle
    {
        public Complex Center { get; set; }
        public double Curvature { get; set; }

        public Circle(Complex center, double curvature)
        {
            Center = center;
            Curvature = curvature;
        }

        public double Radius
        {
            get { return Math.Abs(1 / Curvature); }
            set
            {
                Curvature = 1 / value;
            }
        }

        public void Draw(Canvas c)
        {
            Ellipse el = new Ellipse()
            {
                Width = 2 * Radius,
                Height = 2 * Radius,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            };
            el.SetValue(Canvas.LeftProperty, Center.Real - Radius);
            el.SetValue(Canvas.TopProperty, Center.Imaginary - Radius);
            c.Children.Add(el);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() == typeof(Circle))
            {
                return (((Circle)obj).Center - Center).Magnitude < 0.01 && Math.Abs(((Circle)obj).Radius - Radius) < 0.01;
            }
            return base.Equals(obj);
        }

        public bool IsTangent(Circle c)
        {
            double dist = Distance(c);
            double sumR = c.Radius + Radius;
            double diffR = Math.Abs(c.Radius - Radius);
            return Math.Abs(dist - sumR) < 0.01 || Math.Abs(dist - diffR) < 0.01;
        }

        public double Distance(Circle c)
        {
            return (c.Center - Center).Magnitude;
        }
    }
}
