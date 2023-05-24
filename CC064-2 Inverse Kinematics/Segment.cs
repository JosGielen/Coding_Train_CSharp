using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Inverse_Kinematics
{
    class Segment
    {
        private Vector my_StartPt;
        private Vector my_EndPt;
        private double my_Length;
        private Line my_Line;
        private double my_LineThickness;
        private Brush my_Color;

        public Segment(double length)
        {
            my_StartPt = new Vector(0, 0);
            my_Length = length;
            my_LineThickness = 1.0;
            my_Color = Brushes.Red;
            my_EndPt = my_StartPt;
        }

        public Vector StartPt
        {
            get { return my_StartPt; }
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
            if (target != my_StartPt)
            {
                Vector dir = target - my_StartPt;
                dir.Normalize();
                my_EndPt = target;
                my_StartPt = target - my_Length * dir;
                my_Line.X1 = my_StartPt.X;
                my_Line.Y1 = my_StartPt.Y;
                my_Line.X2 = my_EndPt.X;
                my_Line.Y2 = my_EndPt.Y;
            }
        }
    }
}
