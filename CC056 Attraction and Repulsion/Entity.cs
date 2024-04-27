using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Attraction_and_Repulsion
{
    class Entity
    {
        private Vector my_Location;
        private Vector my_Velocity;
        private Vector my_Acceleration;
        private Vector my_Force;
        private double my_Mass;
        private double my_MaxSpeed;
        private double my_MaxForce;
        private Ellipse my_Shape;
        private Rect my_World;
        private bool my_HasOrientation;
        private bool my_HasConstantVelocity;
        private int my_Life;
        private RotateTransform my_RT = new RotateTransform();

        public Entity(Vector location, Vector velocity, double mass)
        {
            my_Location = location;
            my_Velocity = velocity;
            my_Acceleration = new Vector();
            my_Force = new Vector();
            my_Mass = mass;
            my_MaxSpeed = double.MaxValue;
            my_MaxForce = double.MaxValue;
            my_Shape = new Ellipse() //Default shape is a circle;
            {
                Width = 5.0,
                Height = 5.0,
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = Brushes.Gray,
            };
            my_World = new Rect(0.0, 0.0, double.MaxValue, double.MaxValue);
            my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Shape.Width / 2);
            my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Shape.Height / 2);
            my_HasOrientation = false;
            my_Shape.RenderTransform = my_RT;
        }

        public Vector Location
        {
            get { return my_Location; }
            set { my_Location = value; }
        }

        public Vector Velocity
        {
            get { return my_Velocity; }
            set { my_Velocity = value; }
        }

        public Ellipse Shape
        {
            get { return my_Shape; }
            set { my_Shape = value; }
        }

        public bool HasOrientation
        {
            get { return my_HasOrientation; }
            set { my_HasOrientation = value; }
        }

        public bool HasConstantSpeed
        {
            get { return my_HasConstantVelocity; }
            set { my_HasConstantVelocity = value;}
        }

        public double Orientation
        {
            get
            {
                try
                {
                    if (HasOrientation)
                    {
                        return 180 * Math.Atan2(my_Velocity.Y, my_Velocity.X) / Math.PI;
                    }
                    else
                    {
                        return 0.0;
                    }
                }
                catch (Exception)
                {
                    return 0.0;
                }
            }
        }

        public int Life
        {
            get { return my_Life; }
            set { my_Life = value; }
        }

        public void SetWorld(Rect world)
        {
            my_World = world;
        }

        public void LimitSpeed(double max)
        {
            my_MaxSpeed = max;
        }

        public void LimitForce(double max)
        {
            my_MaxForce = max;
        }

        public void Draw(Canvas c)
        {
            CheckEdges(false);
            my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Shape.Width / 2);
            my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Shape.Height / 2);
            c.Children.Add(my_Shape);
        }

        public void Erase(Canvas c)
        {
            if (c.Children.Contains(my_Shape))
            {
                c.Children.Remove(my_Shape);
            }
        }

        public void ApplyForce(Vector force)
        {
            my_Force += force;
        }

        public void CheckEdges(bool wraparound)
        {
            if (wraparound)
            {
                if (my_Location.X < my_World.Left - my_Shape.Width / 2)
                {
                    my_Location.X = my_World.Right + my_Shape.Width / 2;
                }
                if (my_Location.X > my_World.Right + my_Shape.Width / 2)
                {
                    my_Location.X = my_World.Left - my_Shape.Width / 2;
                }
                if (my_Location.Y < my_World.Top - my_Shape.Width / 2)
                {
                    my_Location.Y = my_World.Bottom + my_Shape.Width / 2;
                }
                if (my_Location.Y > my_World.Bottom + my_Shape.Width / 2)
                {
                    my_Location.Y = my_World.Top - my_Shape.Width / 2;
                }
            }
            else
            {
                if (my_Location.X < my_World.Left + my_Shape.Width / 2)
                {
                    my_Location.X = my_World.Left + my_Shape.Width / 2;
                    my_Velocity.X = -1 * my_Velocity.X;
                }
                if (my_Location.X > my_World.Right - my_Shape.Width / 2)
                {
                    my_Location.X = my_World.Right - my_Shape.Width / 2;
                    my_Velocity.X = -1 * my_Velocity.X;
                }
                if (my_Location.Y < my_World.Top + my_Shape.Width / 2)
                {
                    my_Location.Y = my_World.Top + my_Shape.Width / 2;
                    my_Velocity.Y = -1 * my_Velocity.Y;
                }
                if (my_Location.Y > my_World.Bottom - my_Shape.Width / 2)
                {
                    my_Location.Y = my_World.Bottom - my_Shape.Width / 2;
                    my_Velocity.Y = -1 * my_Velocity.Y;
                }
            }
        }

        public void Update()
        {
            if (my_Force.Length > my_MaxForce)
            {
                my_Force.Normalize();
                my_Force = my_MaxForce * my_Force;
            }
            if (my_Mass > 0)
            {
                my_Acceleration += my_Force / my_Mass;
            }
            else
            {
                my_Acceleration += my_Force;
            }
            my_Velocity += my_Acceleration;
            if (my_Velocity.Length > my_MaxSpeed || my_HasConstantVelocity)
            {
                my_Velocity.Normalize();
                my_Velocity = my_MaxSpeed * my_Velocity;
            }
            my_Location += my_Velocity;
            CheckEdges(false);
            my_Shape.SetValue(Canvas.LeftProperty, my_Location.X);
            my_Shape.SetValue(Canvas.TopProperty, my_Location.Y);
            if (my_HasOrientation)
            {
                double angle = 180 * Math.Atan2(my_Velocity.Y, my_Velocity.X) / Math.PI;
                my_RT.Angle = angle;
            }
            //Reset the force to 0
            my_Acceleration = 0 * my_Acceleration;
            my_Force = 0 * my_Force;
        }
    }
}
