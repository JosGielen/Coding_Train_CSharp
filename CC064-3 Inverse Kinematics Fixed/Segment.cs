using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Inverse_Kinematics_Fixed
{
    class Segment
    {
        private Vector my_StartPt;
        private Vector my_EndPt;
        private double my_Length;
        private double my_Angle;
        private Line my_Line;
        private double my_LineThickness;
        private Brush my_Color;
        private static Random Rnd = new Random();

        public Segment(double x, double y, double length)
        {
            my_StartPt = new Vector(x, y);
            my_Length = length;
            my_Angle = 0.0;
            my_LineThickness = 1.0;
            my_Color = Brushes.Red;
            my_EndPt = my_StartPt + new Vector(my_Length * Math.Cos(my_Angle * Math.PI / 180), my_Length * Math.Sin(my_Angle * Math.PI / 180));
        }

        public Vector StartPt
        {
            get { return my_StartPt; }
        }

        public Vector EndPt
        {
            get { return my_EndPt; }
        }

        public double LineThickness
        {
            get { return my_LineThickness; }
            set { my_LineThickness = value; }
        }

        public Brush LineColor
        {
            get { return my_Color; }
            set { my_Color = value; }
        }

        public void Show(Canvas c)
        {
            my_Line = new Line()
            {
                Stroke = my_Color,
                StrokeThickness = my_LineThickness,
                X1 = my_StartPt.X,
                Y1 = my_StartPt.Y,
                X2 = my_EndPt.X,
                Y2 = my_EndPt.Y
            };
            c.Children.Add(my_Line);
        }

        public void Follow(Vector target)
        {
            Vector dir = target - my_StartPt;
            my_Angle = Vector.AngleBetween(new Vector(1, 0), dir);
            dir.Normalize();
            my_EndPt = target;
            my_StartPt = target - my_Length * dir;
            Update();
        }

        public void SetStart(Vector pos)
        {
            my_StartPt = pos;
            my_EndPt = my_StartPt + new Vector(my_Length * Math.Cos(my_Angle * Math.PI / 180), my_Length * Math.Sin(my_Angle * Math.PI / 180));
            Update();
        }

        public void Update()
        {
            my_Line.X1 = my_StartPt.X;
            my_Line.Y1 = my_StartPt.Y;
            my_Line.X2 = my_EndPt.X;
            my_Line.Y2 = my_EndPt.Y;
        }
    }
}
