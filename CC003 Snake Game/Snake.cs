using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Snake_Game
{
    internal class Snake
    {
        private List<Point> BodyLocation;
        private List<Vector> BodyDirection;
        private List<Rectangle> rects;
        public int Length {  get; set; }
        private Vector MoveDir;
        private Canvas Field;
        private double W, H;
        private Random Rnd = new Random();

        public Snake(Canvas c, double w, double h)
        {
            Length = 1;
            BodyLocation = new List<Point>();
            BodyDirection = new List<Vector>();
            rects = new List<Rectangle>();
            Field = c;
            W = w;
            H = h;
            MoveDir = new Vector(W, 0);
            BodyLocation.Add(new Point(W * Rnd.Next(3,18), H * Rnd.Next(3,18)));
            BodyDirection.Add(new Vector(W, 0)); //Start moving to the right.
            Rectangle R = new Rectangle()
            {
                Width = W,
                Height = H,
                Stroke = Brushes.Brown,
                StrokeThickness = 1.0,
                Fill = Brushes.Brown
            };
            R.SetValue(Canvas.LeftProperty, BodyLocation[0].X);
            R.SetValue(Canvas.TopProperty, BodyLocation[0].Y);
            Field.Children.Add(R);
            rects.Add(R);
        }

        public Point Location
        {
            get { return BodyLocation[0]; }
        }

        public Vector Direction
        {
            get { return MoveDir; }
            set 
            { 
                MoveDir = value;
            }
        }

        public void Grow()
        {
            //Create a new body part by extending the head
            Length++;
            Point newPart = new Point(BodyLocation.Last().X + BodyDirection.Last().X, BodyLocation.Last().Y + BodyDirection.Last().Y);
            BodyLocation.Add(newPart);
            BodyDirection.Add(BodyDirection.Last());
            Rectangle R = new Rectangle()
            {
                Width = W,
                Height = H,
                Stroke = Brushes.Brown,
                StrokeThickness = 1.0,
                Fill = Brushes.Beige
            };
            rects.Add(R);
            Field.Children.Add(R);
        }

        public void Move()
        {
            //Move the rest of the snake forward
            for (int i = BodyLocation.Count() -1; i > 0; i--)
            {
                BodyLocation[i] = BodyLocation[i - 1];
            }
            //Move the head of the snake towards the current movement direction
            BodyLocation[0] += MoveDir;
            //Update the directions of the different snake parts.
            for(int i = BodyDirection.Count() - 1; i > 1; i--)
            {
                BodyDirection[i] = BodyDirection[i - 1];
            }
            BodyDirection[0] = MoveDir;
        }

        public bool CheckValid()
        {
            bool result = true;
            //Check wall collision
            double XPos = BodyLocation[0].X;
            double YPos = BodyLocation[0].Y;
            if (XPos < 0 || XPos > Field.ActualWidth || YPos < 0 || YPos > Field.ActualHeight) result = false;
            //Check self collision
            for(int i = 1; i < BodyLocation.Count; i++)
            {
                if ((BodyLocation[i] - BodyLocation[0]).Length < (W + H) / 4)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }


        public void UpdateLocation()
        {
            for (int i = 0; i < rects.Count(); i++)
            {
                rects[i].SetValue(Canvas.LeftProperty, BodyLocation[i].X);
                rects[i].SetValue(Canvas.TopProperty, BodyLocation[i].Y);
            }
        }
    }
}
