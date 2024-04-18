using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Asteroids
{
    internal class Explosion
    {
        private Vector My_Pos;
        private Ellipse My_ellipse;
        private int My_Turn = 0;
        private int MaxTurns = 0;
        private bool My_Status;

        public Explosion(Vector pos, bool Big)
        {
            My_Pos = pos;
            My_ellipse = new Ellipse
            {
                Height = 2,
                Width = 2,
                Fill = Brushes.White
            };
            My_Turn = 0;
            if (Big)
            {
                MaxTurns = 30;
            }
            else
            {
                MaxTurns = 6;
            }
            My_Status = true;
        }

        public bool status
        {
            get { return My_Status; }
        }

        public void Update()
        {
            if (My_Status == false) return;
            //Draw a circle in the canvas. Radius and color depend on Turn
            double radius = 2 * My_Turn + 1;
            if (My_Turn <= MaxTurns)
            {
                My_ellipse.Height = 2 * radius;
                My_ellipse.Width = 2 * radius;
                My_ellipse.SetValue(Canvas.LeftProperty, My_Pos.X - radius);
                My_ellipse.SetValue(Canvas.TopProperty, My_Pos.Y - radius);
                if (MaxTurns == 6)
                {
                    switch (My_Turn)
                    {
                        case >= 0 and <= 1:
                            My_ellipse.Fill = Brushes.White;
                            break;
                        case > 1 and <= 3:
                            My_ellipse.Fill = Brushes.Yellow;
                            break;
                        case > 3 and <= 5:
                            My_ellipse.Fill = Brushes.Orange;
                            break;
                        case > 5:
                            My_ellipse.Fill = Brushes.Red;
                            break;
                    }
                }
                else
                {
                    switch (My_Turn)
                    {
                        case >= 0 and <= 8:
                            My_ellipse.Fill = Brushes.White;
                            break;
                        case > 8 and <= 15:
                            My_ellipse.Fill = Brushes.Yellow;
                            break;
                        case > 15 and <= 23:
                            My_ellipse.Fill = Brushes.Orange;
                            break;
                        case > 23:
                            My_ellipse.Fill = Brushes.Red;
                            break;
                    }
                }
                My_Turn += 1;
            }
            else
            {
                My_Status = false;
            }
        }

        public void Draw(Canvas c)
        {
            if (My_Status)
            {
                My_ellipse.SetValue(Canvas.LeftProperty, My_Pos.X - 1);
                My_ellipse.SetValue(Canvas.TopProperty, My_Pos.Y - 1);
                c.Children.Add(My_ellipse);
            }
        }

        public void Erase(Canvas c)
        {
            if (c.Children.Contains(My_ellipse))
            {
                c.Children.Remove(My_ellipse);
            }
        }
    }
}
