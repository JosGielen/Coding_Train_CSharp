using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Plinko
{
    public class PlinkoPin
    {
        private double my_Diameter;
        private Vector my_Pos;
        private Ellipse my_Ellipse;

        public PlinkoPin(double diameter, Vector position)
        {
            my_Diameter = diameter;
            my_Pos = position;
            my_Ellipse = new Ellipse()
            {
                Width = my_Diameter + 2.0,
                Height = my_Diameter + 2.0,
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = Brushes.Red
            };
            my_Ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Diameter / 2);
            my_Ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Diameter / 2);
        }

        public Vector Position
        {
            get { return my_Pos; }
            set { my_Pos = value; }
        }

        public double Diameter
        {
            get { return my_Diameter; }
            set { my_Diameter = value; }
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(my_Ellipse);
        }
    }
}