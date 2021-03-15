using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Springs
{
    class Particle
    {
        private double my_Mass;
        private Vector my_Position;
        private Vector my_Velocity;
        private Vector my_Acceleration;
        private double my_Damping;
        private bool my_Locked;
        private readonly Ellipse bob;
        private double my_Size;
        private bool my_Visible;
        private Brush my_Color;

        public Particle(double mass, Point position)
        {
            my_Mass = mass;
            my_Position = new Vector(position.X, position.Y);
            my_Velocity = new Vector(0,0);
            my_Acceleration = new Vector(0, 0);
            my_Damping = 1.0;
            my_Locked = false;
            my_Color = Brushes.Black;
            my_Size = 4.0;
            bob = new Ellipse()
            {
                Stroke = my_Color,
                Fill = my_Color,
                StrokeThickness = 1.0,
                Width = my_Size,
                Height = my_Size
            };
            my_Visible = true;
        }

        public double Mass
        {
            get { return my_Mass; }
            set { my_Mass = value; }
        }

        public Point Position
        {
            get { return new Point(my_Position.X, my_Position.Y); }
            set 
            { 
                my_Position = new Vector(value.X, value.Y);
                bob.SetValue(Canvas.LeftProperty, my_Position.X - my_Size / 2);
                bob.SetValue(Canvas.TopProperty, my_Position.Y - my_Size / 2);
            }
        }

        public Vector Velocity
        {
            get { return my_Velocity; }
            set { my_Velocity = value; }
        }

        public Vector Acceleration
        {
            get { return my_Acceleration; }
            set { my_Acceleration = value; }
        }

        public double Damping
        {
            get { return my_Damping; }
            set { my_Damping = value; }
        }

        public bool Locked
        {
            get { return my_Locked; }
            set { my_Locked = value; }
        }

        public bool Visible
        {
            get { return my_Visible; }
            set 
            { 
                my_Visible = value;
                if (value)
                {
                    bob.Visibility = Visibility.Visible;
                }
                else
                {
                    bob.Visibility = Visibility.Hidden;
                }
            }
        }

        public double Size
        {
            get { return my_Size; }
            set 
            { 
                my_Size = value;
                bob.Width = value;
                bob.Height = value;
                bob.SetValue(Canvas.LeftProperty, my_Position.X - my_Size / 2);
                bob.SetValue(Canvas.TopProperty, my_Position.Y - my_Size / 2);
            }
        }

        public Brush Color
        {
            get { return my_Color; }
            set 
            { 
                my_Color = value;
                bob.Stroke = value;
                bob.Fill = value; 
            }
        }

        public void ApplyForce(Vector force)
        {
            my_Acceleration += force / my_Mass;
        }

        public void Update()
        {
            if (!my_Locked)
            {
                my_Velocity *= my_Damping;
                my_Velocity += my_Acceleration;
                my_Position += my_Velocity;
                my_Acceleration = new Vector(0, 0);
                bob.SetValue(Canvas.LeftProperty, my_Position.X - my_Size / 2);
                bob.SetValue(Canvas.TopProperty, my_Position.Y - my_Size / 2);
            }
        }

        public void Draw (Canvas c)
        {
            bob.SetValue(Canvas.LeftProperty, my_Position.X - my_Size / 2);
            bob.SetValue(Canvas.TopProperty, my_Position.Y - my_Size / 2);
            if (!c.Children.Contains(bob)) c.Children.Add(bob);
        }

    }
}
