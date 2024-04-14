
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RayTracing
{
    internal class Camera
    {
        private Point my_Pos;
        private double my_Angle;
        private Vector my_Dir;
        private int my_FOV;
        private double my_Size;
        private Rectangle my_Dot;
        private RotateTransform rotT;
        private List<Ray> my_Rays;

        public Camera(Point pos, double angle, int fov, int raycount)
        {
            my_Pos = pos;
            my_Angle = angle;
            my_Size = 10.0;
            rotT = new RotateTransform(angle);
            rotT.CenterX = my_Size;
            rotT.CenterY = my_Size / 2;
            my_Dir = new Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180));
            my_FOV = fov;
            my_Rays = new List<Ray>();
            Ray r;
            double rayAngle = my_Angle - my_FOV / 2;
            for (int I = 0; I < raycount; I++)
            {
                r = new Ray(my_Pos, rayAngle);
                my_Rays.Add(r);
                rayAngle += my_FOV / (double)(raycount - 1);
            }
        }

        public Point Pos
        {
            get { return my_Pos; }
            set { my_Pos = value; }
        }

        public Vector Dir
        {
            get { return my_Dir; }
        }

        public int FOV
        {
            get { return my_FOV; }
            set { my_FOV = value; }
        }

        public double Size
        {
            get { return my_Size; }
            set { my_Size = value; }
        }

        public double Angle
        {
            get { return my_Angle; }
            set
            {
                my_Angle = value;
                my_Dir = new Vector(Math.Cos(Angle * Math.PI / 180), Math.Sin(Angle * Math.PI / 180));
                rotT.Angle = my_Angle;
            }
        }

        public List<Ray> Rays
        {
            get { return my_Rays; }
        }

        public void Show(Canvas c)
        {
            for (int I = 0; I < my_Rays.Count(); I++)
            {
                my_Rays[I].Show(c);
            }
            my_Dot = new Rectangle()
            {
                Width = 2 * my_Size,
                Height = my_Size,
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = Brushes.Red
            };
            my_Dot.SetValue(Canvas.LeftProperty, my_Pos.X - my_Size);
            my_Dot.SetValue(Canvas.TopProperty, my_Pos.Y - my_Size / 2);
            my_Dot.RenderTransform = rotT;
            c.Children.Add(my_Dot);
        }

        public void Update()
        {
            my_Dot.SetValue(Canvas.LeftProperty, my_Pos.X - my_Size);
            my_Dot.SetValue(Canvas.TopProperty, my_Pos.Y - my_Size / 2);
            double rayAngle = my_Angle - my_FOV / 2.0;
            for (int I = 0; I < my_Rays.Count(); I++)
            {
                my_Rays[I].Update(my_Pos, rayAngle);
                rayAngle += my_FOV / (double)(my_Rays.Count - 1);
            }
        }
    }
}
