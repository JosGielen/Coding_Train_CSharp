using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Asteroids
{
    internal class Asteroid
    {
        private Vector my_Position;
        private Vector my_Direction;
        private double my_Radius;
        private Polygon my_Shape;
        private double my_Angle;
        private int my_Life;
        private double my_Speed;
        private double rotationSpeed;
        private static Random Rnd = new Random();

        public Asteroid(Vector pos, Vector dir, double Size, double Speed)
        {
            my_Position = pos;
            my_Direction = dir;
            my_Radius = Size;
            my_Life = 10;
            my_Speed = Speed;
            rotationSpeed = 3 * Rnd.NextDouble() - 1.5;
            my_Angle = 0;
            my_Shape = new Polygon()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = Brushes.Gray
            };
            double angle;
            double R;
            Point p;
            for (int i = 0; i < 15; i++)
            {
                angle = i * 2 * Math.PI / 14;
                R = my_Radius * (0.4 * Rnd.NextDouble() + 0.6);
                p = new Point(R * Math.Cos(angle), R * Math.Sin(angle));
                my_Shape.Points.Add(p);
            }
        }

        public Vector Position
        {
            get { return my_Position; }
            set { my_Position = value; }
        }

        public double Radius
        {
            get { return my_Radius; }
        }

        public int Life
        {
            get { return my_Life; }
            set { my_Life = value; }
        }

        public void Draw(Canvas c)
        {
            my_Shape.SetValue(Canvas.LeftProperty, my_Position.X);
            my_Shape.SetValue(Canvas.TopProperty, my_Position.Y);
            c.Children.Add(my_Shape);
        }

        public void Update()
        {
            my_Angle += rotationSpeed;
            my_Position += my_Speed * my_Direction;
            my_Shape.SetValue(Canvas.LeftProperty, my_Position.X);
            my_Shape.SetValue(Canvas.TopProperty, my_Position.Y);
            my_Shape.RenderTransform = new RotateTransform(my_Angle, 0, 0);

        }

        public List<Asteroid> Split()
        {
            if (my_Radius < 30) return null;
            List<Asteroid> result = new List<Asteroid>();
            //Create 2 new smaller Asteroids
            Vector offset = new Vector(my_Direction.Y, -my_Direction.X);
            offset.Normalize();
            Asteroid Ast1 = new Asteroid(my_Position + 0.5 * my_Radius * offset, my_Direction + offset, my_Radius / 1.5, my_Speed / 2);
            Asteroid Ast2 = new Asteroid(my_Position - 0.5 * my_Radius * offset, my_Direction - offset, my_Radius / 1.5, my_Speed / 2);
            Ast1.Life = 5;
            Ast2.Life = 5;
            result.Add(Ast1);
            result.Add(Ast2);
            return result;
        }

        public void Erase(Canvas c)
        {
            if (c.Children.Contains(my_Shape))
            {
                c.Children.Remove(my_Shape);
            }
        }
    }
}
