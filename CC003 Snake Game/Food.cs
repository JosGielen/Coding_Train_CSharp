using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snake_Game
{
    internal class Food
    {
        public Point Location { get; set; }
        public Rectangle rect;

        public Food(Point loc, double W, double H) 
        { 
            Location = loc;
            rect = new Rectangle()
            {
                Width = W,
                Height = H,
                Stroke = Brushes.Red,
                StrokeThickness = 1.0,
                Fill = Brushes.Red
            };
            rect.SetValue(Canvas.LeftProperty, loc.X);
            rect.SetValue(Canvas.TopProperty, loc.Y);
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(rect);
        }

        public void UpdateLocation(Point loc)
        {
            Location = loc;
            rect.SetValue(Canvas.LeftProperty, loc.X);
            rect.SetValue(Canvas.TopProperty, loc.Y);
        }
    }
}
