using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Forward_Kinematics
{
    class Segment
    {
        private Vector my_StartPt;
        private Vector my_EndPt;
        private double my_Length;
        private double my_Angle;
        private double my_TotalAngle;
        private Line my_Line;
        private double my_LineThickness;
        private Brush my_Color;

        public Segment(double x, double y, double length, double angle)
        {
            my_StartPt = new Vector(x, y);
            my_Length = length;
            my_Angle = angle;
            TotalAngle = angle;
            my_LineThickness = 1.0;
            my_Color = Brushes.Red;
            my_EndPt = my_StartPt + new Vector(my_Length * Math.Cos(my_Angle * Math.PI / 180), my_Length * Math.Sin(my_Angle * Math.PI / 180));
        }

        public Vector StartPt
        {
            get { return my_StartPt; }
            set { my_StartPt = value; }
        }

        public Vector EndPt
        {
            get { return my_EndPt; }
        }

        public double Length
        {
            get { return my_Length; }
            set { my_Length = value; }
        }

        public double Angle
        {
            get { return my_Angle; }
            set { my_Angle = value; }
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

        public double TotalAngle
        {
            get { return my_TotalAngle; }
            set { my_TotalAngle = value; }
        }

        public void Update()
        {
            my_TotalAngle = my_Angle;
            double a = my_TotalAngle * Math.PI / 180;
            my_EndPt = my_StartPt + new Vector(my_Length * Math.Cos(a), my_Length * Math.Sin(a));
            my_Line.X1 = my_StartPt.X;
            my_Line.Y1 = my_StartPt.Y;
            my_Line.X2 = my_EndPt.X;
            my_Line.Y2 = my_EndPt.Y;
        }

        public void Update(Segment parent)
        {
            my_StartPt = parent.EndPt;
            my_TotalAngle = parent.TotalAngle + my_Angle;
            double a = my_TotalAngle * Math.PI / 180;
            my_EndPt = my_StartPt + new Vector(my_Length * Math.Cos(a), my_Length * Math.Sin(a));
            my_Line.X1 = my_StartPt.X;
            my_Line.Y1 = my_StartPt.Y;
            my_Line.X2 = my_EndPt.X;
            my_Line.Y2 = my_EndPt.Y;
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
    }
}
