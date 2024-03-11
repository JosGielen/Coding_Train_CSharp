using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Steering_Behaviors
{
    class Agent
    {
        private Vector my_Location;
        private Vector my_Velocity;
        private Vector my_Acceleration;
        private double my_Mass;
        private double my_MaxSpeed;
        private double my_MaxForce;
        private Vector my_Target;
        private double my_Size;
        private Brush my_Color;
        private double my_Breakingdistance;
        private Vector my_Force;
        private Ellipse my_Shape;

        public Agent(Point location, double mass, double maxSpeed, double maxForce, Brush color)
        {
            my_Location = new Vector(location.X, location.Y);
            my_Velocity = new Vector();
            my_Acceleration = new Vector();
            my_Mass = mass;
            my_MaxSpeed = maxSpeed;
            my_MaxForce = maxForce;
            my_Color = color;
            my_Shape = new Ellipse()
            {
                Stroke = color,
                StrokeThickness = 1.0,
                Fill = color,
                Width = 2,
                Height = 2
            };
            my_Shape.SetValue(Canvas.LeftProperty, Location.X);
            my_Shape.SetValue(Canvas.TopProperty, Location.Y);
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

        public double Size
        {
            get { return my_Size; }
            set
            {
                my_Size = value;
                my_Shape.Width = my_Size;
                my_Shape.Height = my_Size;
            }
        }

        public Brush Color
        {
            get { return my_Color; }
            set
            {
                my_Color = value;
                my_Shape.Stroke = my_Color;
                my_Shape.Fill = my_Color;
            }
        }

        public void SetTarget(Point target)
        {
            my_Target = new Vector(target.X, target.Y);
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(my_Shape);
        }

        public void ApplyForce(Vector force)
        {
            my_Force = my_Force + force;
            if (my_Force.Length > my_MaxForce)
            {
                my_Force.Normalize();
                my_Force = my_MaxForce * my_Force;
            }
        }

        private Vector GetSteeringForce(Vector target)
        {
            Vector DesiredVelocity;
            Vector Steering;
            double maxspeed;
            double dist;
            DesiredVelocity = target - my_Location;
            dist = DesiredVelocity.Length;
            if (dist > my_Breakingdistance)
            {
                maxspeed = my_MaxSpeed;
            }
            else if (dist < 0.5)
            {
                maxspeed = 0;
            }
            else
            {
                maxspeed = (my_MaxSpeed * dist / my_Breakingdistance);
            }
            DesiredVelocity.Normalize();
            DesiredVelocity = maxspeed * DesiredVelocity;
            Steering = DesiredVelocity - my_Velocity;
            return Steering;
        }

        public void Update()
        {
            ApplyForce(GetSteeringForce(my_Target));
            my_Acceleration = my_Force / my_Mass;
            my_Velocity += my_Acceleration;
            my_Location += my_Velocity;
            my_Force = 0 * my_Force;
            my_Shape.SetValue(Canvas.LeftProperty, Location.X);
            my_Shape.SetValue(Canvas.TopProperty, Location.Y);
        }
    }
}
