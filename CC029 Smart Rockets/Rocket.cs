using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Smart_Rockets
{
    public class Rocket
    {
        private int my_ID;
        private DNA my_DNA;
        private bool my_Life = true;
        private double my_MinDistance = double.MaxValue;
        private bool my_HitTarget = false;
        private bool my_HitWall = false;
        private bool my_Crashed = false;
        private int my_MinTime = 0;
        private double my_Fitness = 0;
        private Point my_Position;
        private double my_Speed = 0.0;
        private double my_StartDir = 0.0;
        private double my_SpeedX = 0.0;
        private double my_SpeedY = 0.0;
        private Ellipse My_ell;
        private Point my_Target;
        private List<Rect> my_obstacles;

        public Rocket(Point target, List<Rect> obstacles, Point pos)
        {
            my_Target = target;
            my_obstacles = new List<Rect>();
            my_Position = pos;
            for (int I = 0; I < obstacles.Count; I++)
            {
                my_obstacles.Add(obstacles[I]);
            }
            My_ell = new Ellipse()
            {
                Width = 6,
                Height = 6,
                Fill = Brushes.White
            };
            My_ell.SetValue(Canvas.LeftProperty, my_Position.X - 3);
            My_ell.SetValue(Canvas.TopProperty, my_Position.Y - 3);
        }

        public DNA DNA
        {
            get { return my_DNA; }
            set { my_DNA = value; }
        }

        public bool HitTarget
        {
            get { return my_HitTarget; }
        }

        public bool Crashed
        {
            get { return my_Crashed; }
        }

        public int MinTime
        {
            get { return my_MinTime; }
        }

        public double Fitness
        {
            get { return my_Fitness; }
            set { my_Fitness = value; }
        }

        public Point Position
        {
            get { return my_Position; }
            set
            {
                my_Position = value;
                My_ell.SetValue(Canvas.LeftProperty, my_Position.X - 3);
                My_ell.SetValue(Canvas.TopProperty, my_Position.Y - 3);
            }
        }

        public double Speed
        {
            get { return my_Speed; }
            set
            {
                my_Speed = value;
                my_SpeedX = my_Speed * Math.Cos(my_StartDir);
                my_SpeedY = my_Speed * Math.Sin(my_StartDir);
            }
        }

        public double StartDir
        {
            get { return my_StartDir; }
            set
            {
                my_StartDir = value;
                my_SpeedX = my_Speed * Math.Cos(my_StartDir);
                my_SpeedY = my_Speed * Math.Sin(my_StartDir);
            }
        }

        public Ellipse drawing
        {
            get { return My_ell; }
            set { My_ell = value; }
        }

        public double MinDistance
        {
            get { return my_MinDistance; }
        }

        public bool HitWall
        {
            get { return my_HitWall; }
            set { my_HitWall = value; }
        }

        public int ID
        {
            get { return my_ID; }
            set { my_ID = value; }
        }

        public bool alive
        {
            get { return my_Life; }
        }

        public void Update(int counter)
        {
            double dist;
            if (my_Crashed | my_HitWall | my_HitTarget)
            {
                my_Life = false;
                return;
            }
            if (counter < my_DNA.Genes.Count)
            {
                my_SpeedX += my_DNA.Genes[counter].Size * Math.Cos(my_DNA.Genes[counter].Dir);
                my_SpeedY += my_DNA.Genes[counter].Size * Math.Sin(my_DNA.Genes[counter].Dir);
                my_Position.X += my_SpeedX;
                my_Position.Y -= my_SpeedY;
                dist = Math.Sqrt((my_Target.X - my_Position.X) * (my_Target.X - my_Position.X) + (my_Target.Y - my_Position.Y) * (my_Target.Y - my_Position.Y));
                if (dist < my_MinDistance)
                {
                    my_MinDistance = dist;
                    my_MinTime = counter;
                }
                if (dist < 10) my_HitTarget = true;
                for (int I = 0; I < my_obstacles.Count; I++)
                {
                    if (my_Position.X > my_obstacles[I].X & my_Position.X < my_obstacles[I].X + my_obstacles[I].Width & my_Position.Y > my_obstacles[I].Y & my_Position.Y < my_obstacles[I].Y + my_obstacles[I].Height)
                    {
                        my_Crashed = true;
                    }
                }
                My_ell.SetValue(Canvas.LeftProperty, my_Position.X - 3);
                My_ell.SetValue(Canvas.TopProperty, my_Position.Y - 3);
            }
        }

        public void Reset()
        {
            my_Life = true;
            my_MinDistance = double.MaxValue;
            my_HitTarget = false;
            my_HitWall = false;
            my_Crashed = false;
            my_MinTime = 0;
            my_Fitness = 0;
        }

    }
}
