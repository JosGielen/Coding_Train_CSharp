using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Pentominoes
{
    class Pentomino
    {
        private string my_Type;
        private double my_CelWidth;
        private double my_CelHeight;
        private PointCollection my_Cells;
        private PointCollection my_Points;
        private Polygon my_Poly;
        private Brush my_Color;
        private bool my_Used;

        public Pentomino()
        {
            my_Type = "";
            my_CelWidth = 0.0;
            my_CelHeight = 0.0;
            my_Color = Brushes.White;
            my_Cells = new PointCollection();
            my_Points = new PointCollection();
            my_Poly = new Polygon();
            my_Poly.Stroke = Brushes.Black;
            my_Poly.StrokeThickness = 1;
            my_Poly.Fill = my_Color;
            my_Used = false;
        }

        public Pentomino(string Type, int ID, Brush color)
        {
            my_Type = Type;
            my_CelWidth = 0.0;
            my_CelHeight = 0.0;
            my_Color = color;
            my_Cells = new PointCollection();
            my_Points = new PointCollection();
            my_Poly = new Polygon();
            my_Poly.Stroke = Brushes.Black;
            my_Poly.StrokeThickness = 1;
            my_Poly.Fill = my_Color;
            my_Used = false;
        }

        public string Type
        {
            get { return my_Type; }
        }

        public double CelWidth
        {
            get { return my_CelWidth; }
            set
            {
                my_CelWidth = value;
                my_Poly.Points.Clear();
                foreach (Point p in my_Points)
                {
                    my_Poly.Points.Add(new Point(p.X * my_CelWidth, p.Y * my_CelHeight));
                }
            }
        }

        public double CelHeight
        {
            get { return my_CelHeight; }
            set
            {
                my_CelHeight = value;
                my_Poly.Points.Clear();
                foreach (Point p in my_Points)
                {
                    my_Poly.Points.Add(new Point(p.X * my_CelWidth, p.Y * my_CelHeight));
                }
            }
        }

        public PointCollection Points
        {
            get { return my_Points; }
            set
            {
                my_Points = value;
                my_Poly.Points.Clear();
                foreach (Point p in my_Points)
                {
                    my_Poly.Points.Add(new Point(p.X * my_CelWidth, p.Y * my_CelHeight));
                }
            }
        }

        public PointCollection Cells
        {
            get { return my_Cells; }
            set { my_Cells = value; }

        }

        public Brush Color
        {
            get { return my_Color; }
            set
            {
                my_Color = value;
                my_Poly.Fill = my_Color;
            }
        }

        public bool Used
        {
            get { return my_Used; }
            set { my_Used = value; }
        }

        public Polygon Poly
        {
            get { return my_Poly; }
        }

        public void Draw(Canvas c, double Xorig, double Yorig)
        {
            my_Poly.SetValue(Canvas.LeftProperty, Xorig);
            my_Poly.SetValue(Canvas.TopProperty, Yorig);
            c.Children.Add(my_Poly);
        }
    }
}
