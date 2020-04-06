using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BlackHoleSim
{
    class Photon
    {
        private Vector my_Pos;
        private Vector my_Velocity;
        private Polyline my_Trajectory;
        private bool my_Escaped;
        private Ellipse dot;
        private bool my_Alive;
        private double my_Theta;
        private double maxWidth;
        private double maxHeight;

        public Photon(double x, double y, Vector v)
        {
            my_Pos = new Vector(x, y);
            my_Velocity = v;
            my_Alive = true;
            my_Escaped = false;
            my_Theta = Math.PI;
            my_Trajectory = new Polyline()
            {
                Stroke = Brushes.LightGray,
                StrokeThickness = 1.0
            };
        }

        public Vector Pos
        {
            get { return my_Pos; }
            set { my_Pos = value; }
        }

        public Vector Velocity
        {
            get { return my_Velocity; }
            set { my_Velocity = value; }
        }

        public double Theta
        {
            get { return my_Theta; }
            set { my_Theta = value; }
        }

        public bool Alive
        {
            get { return my_Alive; }
            set { my_Alive = value; }
        }

        public Polyline Trajectory
        {
            get { return my_Trajectory; }

        }

        public bool Escaped
        {
            get { return my_Escaped; }
        }


        public void Dead()
        {
            my_Alive = false;
            dot.Visibility = Visibility.Hidden;
        }

        public void Update(Canvas c)
        {
            if (my_Alive)
            {
                my_Trajectory.Points.Add(new Point(my_Pos.X,my_Pos.Y));
                Vector dV = 0.05 * new Vector(my_Velocity.X, my_Velocity.Y);
                my_Pos = my_Pos + dV;
                dot.SetValue(Canvas.LeftProperty, my_Pos.X - 2);
                dot.SetValue(Canvas.TopProperty, my_Pos.Y - 2);
                if (my_Pos.X < 0 | my_Pos.X > maxWidth | my_Pos.Y < 0 | my_Pos.Y > maxHeight)
                {
                    my_Escaped = true;
                    Dead();
                }
            }
        }

        public void Show(Canvas c)
        {
            maxWidth = c.ActualWidth;
            maxHeight = c.ActualHeight;
            dot = new Ellipse()
            {
                Width = 4,
                Height = 4,
                Stroke = Brushes.White,
                StrokeThickness = 1.0,
                Fill = Brushes.White
            };
            dot.SetValue(Canvas.LeftProperty, my_Pos.X - 2);
            dot.SetValue(Canvas.TopProperty, my_Pos.Y - 2);
            c.Children.Add(dot);
            c.Children.Add(my_Trajectory);
        }
    }
}
