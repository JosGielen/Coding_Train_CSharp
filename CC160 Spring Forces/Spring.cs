using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Springs
{
    class Spring
    {
        private double my_Stiffness;
        private double my_Damping;
        private double my_Length;
        private double my_Extension;
        private Particle my_End1;
        private Particle my_End2;
        private readonly Line l;

        public Spring(Particle p1, Particle p2)
        {
            my_Stiffness = 0.1;
            my_Damping = 0.0;
            my_Length = (p2.Position - p1.Position).Length;
            my_Extension = 0;
            my_End1 = p1;
            my_End2 = p2;
            l = new Line()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2.0,
                X1 = p1.Position.X,
                Y1 = p1.Position.Y,
                X2 = p2.Position.X,
                Y2 = p2.Position.Y
            };
        }

        public Particle End1
        {
            get { return my_End1; }
            set { my_End1 = value; }
        }

        public Particle End2
        {
            get { return my_End2; }
            set { my_End2 = value; }
        }

        public double Stiffness
        {
            get { return my_Stiffness; }
            set { my_Stiffness = value; }
        }

        public double Damping
        {
            get { return my_Damping; }
            set 
            { 
                my_Damping = value;
                my_End1.Damping = my_Damping;
                my_End2.Damping = my_Damping;
            }
        }

        public double RestLength
        {
            get { return my_Length; }
            set 
            {
                my_Length = value;
                my_Extension = (my_End2.Position - my_End1.Position).Length - my_Length; 
            }
        }

        public double Extension
        {
            get { return my_Extension; }
            set 
            { 
                my_Extension = value;
                Vector v = my_End2.Position - my_End1.Position;
                v.Normalize();
                my_End2.Position = my_End1.Position + (my_Length + my_Extension) * v;
            }
        }

        public void SetEnd1Position(Point p)
        {
            my_End1.Position = p;
            l.X1 = my_End1.Position.X;
            l.Y1 = my_End1.Position.Y;
            Vector v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length;
            my_End1.Velocity = new Vector(0, 0);
            my_End1.Acceleration = new Vector(0,0);
        }

        public void SetEnd2Position(Point p)
        {
            my_End2.Position = p;
            l.X2 = my_End2.Position.X;
            l.Y2 = my_End2.Position.Y;
            Vector v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length;
            my_End2.Velocity = new Vector(0, 0);
            my_End2.Acceleration = new Vector(0, 0);
        }

        public void Update()
        {
            Vector v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length; 
            v.Normalize();
            Vector force = my_Stiffness * my_Extension * v;
            my_End1.ApplyForce(force);
            my_End2.ApplyForce(-1*force);
        }

        public void Draw (Canvas c)
        {
            c.Children.Add(l);
            my_End1.Draw(c);
            my_End2.Draw(c);
        }

        public void ReDraw()
        {
            my_End1.Update();
            my_End2.Update();
            l.X1 = my_End1.Position.X;
            l.Y1 = my_End1.Position.Y;
            l.X2 = my_End2.Position.X;
            l.Y2 = my_End2.Position.Y;
        }

    }
}
