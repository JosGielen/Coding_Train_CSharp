using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snowflake
{
    internal class Particle
    {
        private Vector Pos;
        private double Size;
        private Ellipse my_dot;
        private double my_MaxX;
        private int my_YDev;
        private static Random Rnd = new Random();

        public Particle(double dotsize, double maxX, int Ydeviation)
        {
            Pos = new Vector(maxX, Rnd.Next(Ydeviation));
            Size = dotsize;
            my_MaxX = maxX;
            my_YDev = Ydeviation;
        }

        public double GetX()
        {
            return Pos.X;
        }

        public bool Update(List<Particle> particleList)
        {
            bool result = false;
            Pos.X -= 1;
            Pos.Y += Rnd.Next(-1 * my_YDev, my_YDev + 1);
            //Constrain in ±30° cone
            if (Pos.Y > 0.57 * Pos.X)
            {
                Pos.Y = 0.57 * Pos.X;
            }
            else if (Pos.Y < -0.57 * Pos.X)
            {
                Pos.Y = -0.57 * Pos.X;
            }
            //Check is Finished
            if (Pos.X < 1) result = true;
            foreach (Particle p in particleList)
            {
                if ((p.Pos.X - Pos.X) * (p.Pos.X - Pos.X) + (p.Pos.Y - Pos.Y) * (p.Pos.Y - Pos.Y) <= Size * Size)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void Show(Canvas c)
        {
            Point Pt1 = new Point(c.ActualWidth / 2 + Pos.X, c.ActualHeight / 2 + Pos.Y);
            Point Pt2 = new Point(c.ActualWidth / 2 + Pos.X, c.ActualHeight / 2 - Pos.Y);
            RotateTransform rotT = new RotateTransform(30, c.ActualWidth / 2, c.ActualHeight / 2);
            //Rotate 30° to make 1 direction vertical
            Pt1 = rotT.Transform(Pt1);
            Pt2 = rotT.Transform(Pt2);
            rotT.Angle = 60;
            for (int I = 0; I <= 5; I++)
            {
                //Set the calculated Dot
                Pt1 = rotT.Transform(Pt1);
                my_dot = new Ellipse
                {
                    Height = Size,
                    Width = Size,
                    Stroke = Brushes.White,
                    StrokeThickness = 1.0,
                    Fill = Brushes.White
                };
                my_dot.SetValue(Canvas.LeftProperty, Pt1.X);
                my_dot.SetValue(Canvas.TopProperty, Pt1.Y);
                c.Children.Add(my_dot);
                //Set the mirror image dot
                Pt2 = rotT.Transform(Pt2);
                my_dot = new Ellipse
                {
                    Height = Size,
                    Width = Size,
                    Stroke = Brushes.White,
                    StrokeThickness = 1.0,
                    Fill = Brushes.White
                };
                my_dot.SetValue(Canvas.LeftProperty, Pt2.X);
                my_dot.SetValue(Canvas.TopProperty, Pt2.Y);
                c.Children.Add(my_dot);
            }
        }
    }
}
