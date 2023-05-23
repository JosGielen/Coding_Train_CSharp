using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Fractal_TreeObjects
{
    class Branch
    {
        private Point my_StartPt;
        private Point my_EndPt;
        private Line my_Line;
        private Ellipse my_Leaf;
        private bool gotBranches;

        public Branch(Point start_, Point end_)
        {
            my_StartPt = start_;
            my_EndPt = end_;
            gotBranches = false;
        }

        public void Show(Canvas c)
        {
            my_Line = new Line()
            {
                Stroke = Brushes.White,
                StrokeThickness = 2.0,
                X1 = my_StartPt.X,
                Y1 = my_StartPt.Y,
                X2 = my_EndPt.X,
                Y2 = my_EndPt.Y
            };
            c.Children.Add(my_Line);
        }

        public double Length
        {
            get { return (my_EndPt - my_StartPt).Length; }
        }

        public Point EndPt
        {
            get { return my_EndPt; }
        }

        public List<Branch> BranchOut(double angle)
        {
            List<Branch> result = new List<Branch>();
            if (gotBranches == false & (my_EndPt - my_StartPt).Length > 3)
            {
                Point BranchEndPt;
                RotateTransform RT = new RotateTransform()
                {
                    CenterX = my_EndPt.X,
                    CenterY = my_EndPt.Y,
                    Angle = angle
                };
                BranchEndPt = my_EndPt + 0.66 * (my_EndPt - my_StartPt);
                result.Add(new Branch(my_EndPt, RT.Transform(BranchEndPt)));
                RT.Angle = -angle;
                result.Add(new Branch(my_EndPt, RT.Transform(BranchEndPt)));
                gotBranches = true;
            }
            return result;
        }
    }
}
