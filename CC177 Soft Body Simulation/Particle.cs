using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Soft_Body_Simulation
{
    class Particle
    {
        private double my_Mass;
        private Vector my_Position;
        private Vector my_Velocity;
        private Vector my_Acceleration;
        private double my_Damping;
        private bool my_Locked;
        private bool my_Show;
        private Ellipse bob;
        private double my_Size;
        private double groundPos;

        public Particle(double mass, Point position, bool show, double ground)
        {
            my_Mass = mass;
            my_Position = new Vector(position.X, position.Y);
            my_Velocity = new Vector(0, 0);
            my_Acceleration = new Vector(0, 0);
            my_Damping = 1.0;
            my_Locked = false;
            my_Size = 8.0;
            my_Show = show;
            groundPos = ground;
            bob = new Ellipse()
            {
                Width = my_Size,
                Height = my_Size,
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = Brushes.Black
            };
            bob.SetValue(Canvas.LeftProperty, my_Position.X - my_Size / 2);
            bob.SetValue(Canvas.TopProperty, my_Position.Y - my_Size / 2);
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
                if (my_Show)
                {
                    bob.SetValue(Canvas.LeftProperty, my_Position.X - my_Size / 2);
                    bob.SetValue(Canvas.TopProperty, my_Position.Y - my_Size / 2);
                }
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

        public bool Show
        {
            get { return my_Show; }
            set { my_Show = value; }
        }

        public bool Locked
        {
            get { return my_Locked; }
            set { my_Locked = value; }
        }

        public double Size
        {
            get { return my_Size; }
            set 
            { 
                my_Size = value;
                if (my_Show)
                {
                    bob.Width = my_Size;
                    bob.Height = my_Size;
                    bob.SetValue(Canvas.LeftProperty, my_Position.X - my_Size / 2);
                    bob.SetValue(Canvas.TopProperty, my_Position.Y - my_Size / 2);
                }
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
                //Set ground level
                if (my_Position.Y >= groundPos)
                {
                    my_Position.Y = groundPos;
                }
                if(my_Show)
                {
                    bob.SetValue(Canvas.LeftProperty, my_Position.X - my_Size / 2);
                    bob.SetValue(Canvas.TopProperty, my_Position.Y - my_Size / 2);
                }
                my_Locked = true;
            }
            else
            {
                my_Acceleration = new Vector(0, 0);
            }
        }

        public void Draw (Canvas c)
        {
            if (my_Show)
            {
                bob.SetValue(Canvas.LeftProperty, my_Position.X - my_Size / 2);
                bob.SetValue(Canvas.TopProperty, my_Position.Y - my_Size / 2);
                if (!c.Children.Contains(bob)) c.Children.Add(bob);
            }
        }
    }
}
