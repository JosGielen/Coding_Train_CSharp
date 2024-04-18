using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Asteroids
{
    internal class Bullet
    {
        private bool my_Status;
        private Vector my_Pos;
        private Vector my_Dir;  //negative when fired by the ship ; positive when fired by Invader 
        private Ellipse My_ellipse;
        private double my_Radius;

        public Bullet(Vector pos, Vector dir)
        {
            my_Status = true;
            my_Pos = pos;
            my_Dir = dir;
            my_Radius = 3;
            My_ellipse = new Ellipse()
            {
                Fill = Brushes.Red,
                Height = 2 * my_Radius,
                Width = 2 * my_Radius
            };
        }

        public Vector Pos
        {
            get { return my_Pos; }
        }

        public void Update()
        {
            if (my_Status)
            {

                my_Pos += my_Dir;
                My_ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Radius);
                My_ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Radius);
            }
        }

        public void Draw(Canvas c)
        {
            My_ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Radius);
            My_ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Radius);
            c.Children.Add(My_ellipse);
        }

        public void Erase(Canvas c)
        {
            if (c.Children.Contains(My_ellipse))
            {
                c.Children.Remove(My_ellipse);
            }
        }

        public bool Hit(Asteroid ast)
        {
            if (my_Status)
            {
                return ((my_Pos - ast.Position).Length < 0.9 * ast.Radius);
            }
            return false;
        }
    }
}
