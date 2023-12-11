using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Purple_Rain
{
    internal class Drop
    {
        private double my_X;
        private double my_Y;
        private double depth;
        private double speed;
        private double maxSpeed = 10.0;
        private double length;
        private double maxWidth;
        private double maxHeight;
        private Line dot;
        private static Random rnd = new Random();

        public Drop(double x, double y)
        {
            my_X = x;
            my_Y = y;
            depth = CircleStep(20 * rnd.NextDouble());
            speed = Map(depth, 0, 20, maxSpeed, 5);
            length = Map(depth, 0, 20, 15, 3);
        }
        public void Show(Canvas c)
        {
            dot = new Line()
            {
                Stroke = new SolidColorBrush(Color.FromRgb(138, 43, 226)),
                StrokeThickness = Map(depth, 0, 20, 4, 1),
                X1 = my_X,
                Y1 = my_Y,
                X2 = my_X,
                Y2 = my_Y + length
            };
            maxWidth = c.ActualWidth;
            maxHeight = c.ActualHeight;
            c.Children.Add(dot);
        }

        public void Update()
        {
            my_Y = my_Y + speed;
            if (my_Y > maxHeight)
            {
                depth = CircleStep(20 * rnd.NextDouble());
                speed = Map(depth, 0, 20, maxSpeed, 5);
                length = Map(depth, 0, 20, 15, 3);
                my_X = maxWidth * rnd.NextDouble();
                my_Y = -length;
                dot.X1 = my_X;
                dot.X2 = my_X;
                dot.StrokeThickness = Map(depth, 0, 20, 4, 1);
            }
            dot.Y1 = my_Y;
            dot.Y2 = my_Y + length;
        }

        public double Map(double Value, double low, double high, double min, double max)
        {
            double result = (Value - low) / (high - low);
            return result * (max - min) + min;
        }

        public double CircleStep(double value)
        {
            return Math.Sqrt(40 * value - value * value);
        }
    }
}
