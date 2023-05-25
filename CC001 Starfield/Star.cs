using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Starfield
{
    internal class Star
    {
        private Point pos;
        private double size;
        private double maxWidth;
        private double maxHeight;
        private Ellipse dot;
        private static Random rnd = new Random();

        public Star(double x, double y, double starSize)
        {
            pos = new Point(x, y);
            size = starSize;
        }

        public void Show(Canvas c)
        {
            dot = new Ellipse()
            {
                Width = size,
                Height = size,
                Stroke = Brushes.White,
                StrokeThickness = 1.0,
                Fill = Brushes.White
            };
            dot.SetValue(Canvas.LeftProperty, pos.X);
            dot.SetValue(Canvas.TopProperty, pos.Y);
            maxWidth = c.ActualWidth;
            maxHeight = c.ActualHeight;
            c.Children.Add(dot);
        }

        public void Update(double speed)
        {
            Vector v = pos - new Point(maxWidth / 2, maxHeight / 2); //From window center towards the current position
            pos = pos + 0.01 * speed * v;
            if (pos.X < 0 | pos.Y > maxWidth | pos.Y < 0 | pos.Y > maxHeight)
            {
                pos.X = 0.8 * maxWidth * rnd.NextDouble();
                pos.Y = 0.8 * maxHeight * rnd.NextDouble();
                dot.Width = 0;
                dot.Height = 0;
            }
            dot.Width += 0.02 * speed;
            dot.Height += 0.02 * speed;
            dot.SetValue(Canvas.LeftProperty, pos.X);
            dot.SetValue(Canvas.TopProperty, pos.Y);
        }
    }
}
