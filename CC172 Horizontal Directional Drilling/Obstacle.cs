using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Horizontal_Directional_Drilling
{
    internal class Obstacle
    {
        private Point my_Pos;
        private double my_Height;
        private double my_Width;
        private Brush my_Color;
        private Ellipse my_Shape;

        public Obstacle(Point pos, double W, double H)
        {
            my_Pos = pos;
            my_Width = W;
            my_Height = H;
            my_Shape = new Ellipse()
            {
                Width = my_Width,
                Height = my_Height,
                Fill = my_Color
            };
        }

        public Point Position
        {
            get { return my_Pos; }
            set { my_Pos = value; }
        }

        public double Width
        {
            get { return my_Width; }
            set { my_Width = value; }
        }

        public double Height
        { 
            get { return my_Height; } 
            set { my_Height = value; } 
        }

        public Brush Color
        { 
            get { return my_Color; } 
            set 
            { 
                my_Color = value; 
                my_Shape.Fill = my_Color;
            } 
        }

        public void Draw(Canvas c)
        {
            my_Shape.SetValue(Canvas.LeftProperty, my_Pos.X - my_Width / 2);
            my_Shape.SetValue(Canvas.TopProperty, my_Pos.Y - my_Height / 2);
            c.Children.Add(my_Shape);
        }

        public bool CheckCollision(Point pt)
        {
            return (pt.X - my_Pos.X) * (pt.X - my_Pos.X) / (my_Width * my_Width) + (pt.Y - my_Pos.Y) * (pt.Y - my_Pos.Y) / (my_Height * my_Height) <= 0.25;
        }

    }

}
