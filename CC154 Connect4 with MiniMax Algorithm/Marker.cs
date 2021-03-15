using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Connect4
{
    class Marker
    {
        private Ellipse my_Shape;
        private int my_PlayerNr;
        private int my_Row;
        private int my_Col;

        public Marker()
        {
            my_Shape = new Ellipse()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Brushes.White
            };
        }

        public double CelWidth
        {
            get { return my_Shape.Width; }
            set { my_Shape.Width = value; }
        }

        public double CelHeight
        {
            get { return my_Shape.Height; }
            set { my_Shape.Height = value; }
        }

        public Ellipse Shape
        {
            get { return my_Shape; }
            set { my_Shape = value; }
        }

        public int Row
        {
            get { return my_Row; }
            set { my_Row = value; }
        }

        public int Col
        {
            get { return my_Col; }
            set { my_Col = value; }
        }

        public int PlayerNr
        {
            get { return my_PlayerNr; }
            set
            {
                my_PlayerNr = value;
                if (value == 0)
                {
                    my_Shape.Fill = Brushes.White;
                }
                else if (value == 1)
                {
                    my_Shape.Fill = Brushes.Yellow;
                }
                else if (value == 2)
                {
                    my_Shape.Fill = Brushes.Red;
                }
                else
                {
                    throw new Exception("Invalid player number.");
                }
            }
        }

        public void Draw(Canvas c, double Xorig, double Yorig)
        {
            my_Shape.SetValue(Canvas.LeftProperty, Xorig);
            my_Shape.SetValue(Canvas.TopProperty, Yorig);
            c.Children.Add(my_Shape);
        }
    }
}
