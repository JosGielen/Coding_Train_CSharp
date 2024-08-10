using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Soft_Body_Simulation
{
    class Spring
    {
        private double my_Stiffness;
        private double my_Damping;
        private double my_Length;
        private double my_Extension;
        private Particle my_End1;
        private Particle my_End2;
        private bool my_ShowLine;
        private Line my_Line;

        public Spring(Particle p1, Particle p2, bool showLine)
        {
            my_Stiffness = 0.01;
            my_Damping = 0.0;
            my_Length = (p2.Position - p1.Position).Length;
            my_Extension = 0;
            my_End1 = p1;
            my_End2 = p2;
            my_Line = new Line()
            {
                X1 = p1.Position.X,
                Y1 = p1.Position.Y,
                X2 = p2.Position.X,
                Y2 = p2.Position.Y,
                Stroke = Brushes.Red,
                StrokeThickness = 1,
            };
            my_ShowLine = showLine;
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

        public bool ShowLine
        {
            get { return my_ShowLine; }
            set { my_ShowLine = value; }
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
        }

        public void SetEnd1Position(Point p)
        {
            my_End1.Position = p;
            my_Line.X1 = my_End1.Position.X;
            my_Line.Y1 = my_End1.Position.Y;
            Vector v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length;
        }

        public void SetEnd2Position(Point p)
        {
            my_End2.Position = p;
            my_Line.X2 = my_End2.Position.X;
            my_Line.Y2 = my_End2.Position.Y;
            Vector v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length;
        }

        public void Update()
        {
            Vector v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length; 
            v.Normalize();
            Vector force = -1 * my_Stiffness * my_Extension * v;
            my_End1.ApplyForce(-1 * force);
            my_End2.ApplyForce(force);
        }

        public void Draw (Canvas c, bool showParticles)
        {
            if (my_ShowLine)
            {
                c.Children.Add(my_Line);
            }
            if (showParticles)
            {
                my_End1.Draw(c);
                my_End2.Draw(c);
            }
        }

        public void Redraw()
        {
            my_End1.Update();
            my_End2.Update();
            my_Line.X1 = my_End1.Position.X;
            my_Line.Y1 = my_End1.Position.Y;
            my_Line.X2 = my_End2.Position.X;
            my_Line.Y2 = my_End2.Position.Y;
        }
    }
}
