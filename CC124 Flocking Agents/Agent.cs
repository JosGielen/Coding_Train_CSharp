using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Flocking_Agents
{
    class Agent
    {
        private Vector my_Location;
        private Vector my_Velocity;
        private Vector my_DesiredVelocity;
        private Vector my_Force;
        private Vector my_Acceleration;
        private double my_Mass;
        private double my_MaxSpeed;
        private bool my_ConstantSpeed;
        private double my_MaxForce;
        private bool my_MoveToTarget;            //if (True: Automatic moves to a given target and stops there 
        private double my_Breakingdistance;      //Reduce speed when distance to target < my_Breakingdistance 
        private double my_Accuracy;              //Stop moving when distance to target < my_Accuracy 
        private Vector my_Target;
        private Shape my_Shape;                  //Custom shape for the agent 
        private Size my_Size;                    //Adjust the size of the agent (also for custom shapes) 
        private Brush my_FillColor;
        private Brush my_LineColor;
        private double my_LineThickness;
        private bool my_Rotate;
        private RotateTransform my_RT;

        public Agent(Point location)
        {
            my_Location = new Vector(location.X, location.Y);
            my_Velocity = new Vector();
            my_DesiredVelocity = new Vector();
            my_Acceleration = new Vector();
            my_Mass = 1.0;
            my_MaxSpeed = double.MaxValue;
            my_MaxForce = double.MaxValue;
            my_ConstantSpeed = false;
            my_Breakingdistance = double.MaxValue;
            my_Accuracy = 0.2;
            my_Size = new Size(0.0, 0.0);
            my_FillColor = Brushes.White;
            my_LineColor = Brushes.Black;
            my_LineThickness = 1.0;
            my_Rotate = false;
            my_MoveToTarget = false;
            my_Target = new Vector(location.X, location.Y);
            //Default shape is a triangle
            my_Shape = new Polygon();
            ((Polygon)my_Shape).Points.Add(new Point(0, 0));
            ((Polygon)my_Shape).Points.Add(new Point(-20, -5));
            ((Polygon)my_Shape).Points.Add(new Point(-20, 5));
            my_Shape.Fill = my_FillColor;
            my_Shape.Stroke = my_LineColor;
            my_Shape.StrokeThickness = my_LineThickness;
            my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Size.Width / 2);
            my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Size.Height / 2);
        }

        public Agent(Point location, Shape shape)
        {
            my_Location = new Vector(location.X, location.Y);
            my_Velocity = new Vector();
            my_DesiredVelocity = new Vector();
            my_Acceleration = new Vector();
            my_Mass = 1.0;
            my_MaxSpeed = double.MaxValue;
            my_MaxForce = double.MaxValue;
            my_Breakingdistance = double.MaxValue;
            my_Accuracy = 0.2;
            my_Size = new Size(0.0, 0.0);
            my_FillColor = Brushes.White;
            my_LineColor = Brushes.Black;
            my_LineThickness = 1.0;
            my_Rotate = false;
            my_MoveToTarget = false;
            my_Target = new Vector(location.X, location.Y);
            my_Shape = shape;
            my_Shape.Fill = my_FillColor;
            my_Shape.Stroke = my_LineColor;
            my_Shape.StrokeThickness = my_LineThickness;
            my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Size.Width / 2);
            my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Size.Height / 2);
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

        public double Breakingdistance
        {
            get { return my_Breakingdistance; }
            set { my_Breakingdistance = value; }
        }

        public Size Size
        {
            get { return my_Size; }
            set
            {
                my_Size = value;
                my_Shape.Stretch = Stretch.Uniform;
                my_Shape.Width = my_Size.Width;
                my_Shape.Height = my_Size.Height;
            }
        }

        public bool MoveToTarget
        {
            get { return my_MoveToTarget; }
            set { my_MoveToTarget = value; }
        }

        public Vector Target
        {
            get { return my_Target; }
            set { my_Target = value; }
        }

        public Brush Fill
        {
            get { return my_FillColor; }
            set
            {
                my_FillColor = value;
                my_Shape.Fill = value;
            }
        }

        public Brush Stroke
        {
            get { return my_LineColor; }
            set
            {
                my_LineColor = value;
                my_Shape.Stroke = value;
            }
        }

        public double StrokeThickness
        {
            get { return my_LineThickness; }
            set
            {
                my_LineThickness = value;
                my_Shape.StrokeThickness = value;
            }
        }

        public double MaxSpeed
        {
            get { return my_MaxSpeed; }
            set { my_MaxSpeed = value; }
        }

        public double MaxForce
        {
            get { return my_MaxForce; }
            set { my_MaxForce = value; }
        }

        public bool CanRotate
        {
            get { return my_Rotate; }
            set
            {
                my_Rotate = value;
                if (value)
                {
                    my_RT = new RotateTransform();
                    my_RT.Angle = 0;
                    my_Shape.RenderTransform = my_RT;
                }
                else
                {
                    my_RT = null;
                    my_Shape.RenderTransform = null;
                }

            }
        }

        public double Accuracy
        {
            get { return my_Accuracy; }
            set { my_Accuracy = value; }
        }

        public double Mass
        {
            get { return my_Mass; }
            set { my_Mass = value; }
        }

        public bool UseConstantSpeed
        {
            get { return my_ConstantSpeed; }
            set { my_ConstantSpeed = value; }
        }

        public void SetTarget(Point target)
        {
            my_Target = new Vector(target.X, target.Y);
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(my_Shape);
        }

        public void ApplySteeringForce(Vector target)
        {
            Vector SteeringForce;
            double maxspeed;
            double dist;
            my_Target = target;
            my_DesiredVelocity = target - my_Location;
            dist = my_DesiredVelocity.Length;
            if (dist > my_Breakingdistance)
            {
                maxspeed = my_MaxSpeed;
            }
            else if (dist < my_Accuracy)
            {
                maxspeed = 0;
            }
            else
            {
                if (my_Breakingdistance > 0)
                {
                    maxspeed = my_MaxSpeed * dist / my_Breakingdistance;
                }
                else
                {
                    maxspeed = my_MaxSpeed;
                }
            }
            my_DesiredVelocity.Normalize();
            my_DesiredVelocity = maxspeed * my_DesiredVelocity;
            SteeringForce = my_DesiredVelocity - my_Velocity;
            ApplyForce(SteeringForce);
}

        public void ApplyForce(Vector force)
        {
            my_Force = my_Force + force;
        }

        public void Update()
        {
            if (my_MoveToTarget == true)
            {
                ApplySteeringForce(my_Target);
            }
            else
            {
                my_Accuracy = 0.0;
            }
            if (my_Force.Length > my_MaxForce)
            {
                my_Force.Normalize();
                my_Force = my_MaxForce * my_Force;
            }
            my_Acceleration = my_Force / my_Mass;
            my_Velocity += my_Acceleration;
            if (my_Rotate & my_Velocity.Length > my_Accuracy)
            {
                if (my_Shape.ActualWidth > 0 & my_Shape.ActualHeight > 0)
                {
                    my_RT.CenterX = my_Shape.Width / 2;
                    my_RT.CenterY = my_Shape.Height / 2;
                }
                my_RT.Angle = 180 * Math.Atan2(my_Velocity.Y, my_Velocity.X) / Math.PI;
            }
            if (my_ConstantSpeed == true)
            {
                my_Velocity.Normalize();
                my_Velocity = my_MaxSpeed * my_Velocity;
            }
            my_Location += my_Velocity;
            my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Size.Width / 2);
            my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Size.Height / 2);
            my_Force = 0 * my_Force;
        }

        public void Edges(Canvas c)
        {
            if (my_Location.X < 0)
            {
                my_Location.X = c.ActualWidth;
            }
            else if (my_Location.X > c.ActualWidth)
            {
                my_Location.X = 0;
            }
            if (my_Location.Y < 0)
            {
                my_Location.Y = c.ActualHeight;
            }
            else if (my_Location.Y > c.ActualHeight)
            {
                my_Location.Y = 0;
            }
        }

    }
}
