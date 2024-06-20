using System.Windows;
using System.Windows.Media;

namespace RayTracing
{
    public partial class MainWindow : Window
    {
        Point Center;
        private List<Ray> my_Rays;
        private List<CircleObstacle> Circles;
        private int RayCount;
        private int CircleCount = 8;
        private bool Started = false;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RayCount = 200;
            double rayAngle = 0.1;
            double size;
            Ray r;
            CircleObstacle co;
            Center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            Point pt;
            my_Rays = new List<Ray>();
            for (int I = 0; I < RayCount; I++)
            {
                r = new Ray(Center, rayAngle);
                my_Rays.Add(r);
                r.Show(canvas1);
                rayAngle += 360 / (double)(RayCount - 1);
            }
            //Add the circles (no overlapping)
            Circles = new List<CircleObstacle>();
            do
            {
                size = 20 * Rnd.NextDouble() + 20;
                pt = new Point((canvas1.ActualWidth - 2 * size) * Rnd.NextDouble() + size, (canvas1.ActualHeight - 2 * size) * Rnd.NextDouble() + size);
                bool ok = true;
                for (int I = 0; I < Circles.Count; I++)
                {
                    if ((pt - Center).Length < 3 * size)
                    {
                        ok = false; 
                        break;
                    }
                    if ((pt - Circles[I].Pos).Length <= size + Circles[I].Radius)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    co = new CircleObstacle(pt, size, 360 * Rnd.NextDouble(), Rnd.NextDouble() + 0.6);
                    co.Show(canvas1);
                    Circles.Add(co);
                }
            } while (Circles.Count < CircleCount);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double dist;
            double mindist;
            Point intPt;
            Point closestPt;
            canvas1.Children.Clear();
            //Add the Rays to the canvas;
            for (int I = 0; I < my_Rays.Count; I++)
            {
                my_Rays[I].Show(canvas1);
            }
            //Move the Circles
            for (int I = 0; I < Circles.Count; I++)
            {
                Circles[I].Update();
                //Check for Circle collisions
                for (int J = I + 1; J < Circles.Count; J++)
                {
                    Circles[I].Collide(Circles[J]);
                }
                Circles[I].Show(canvas1);
            }
            //Fint the intersections points between rays and Circles
            for (int I = 0; I < my_Rays.Count(); I++)
            {
                mindist = double.MaxValue;
                for (int J = 0; J < Circles.Count; J++)
                {
                    intPt = Circles[J].LineIntersect(my_Rays[I]);
                    dist = (intPt - Center).Length;
                    if (dist < mindist) 
                    { 
                        mindist = dist;
                        closestPt = intPt;
                    }
                }
                //End the ray at the closest intersect point
                my_Rays[I].X2 = closestPt.X;
                my_Rays[I].Y2 = closestPt.Y;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Started)
            {
                BtnStart.Content = "STOP";
                Started = true;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                BtnStart.Content = "START";
                Started = false;
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }
    }
}