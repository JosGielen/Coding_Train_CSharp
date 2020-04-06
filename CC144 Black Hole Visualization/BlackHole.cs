using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BlackHoleSim
{
    class BlackHole
    {
        private Vector pos;
        private double mass;
        private double sr;
        private Ellipse Hole;
        private Ellipse Orbit;
        private Ellipse Diskouter;
        private Ellipse Diskinner;

        public BlackHole(double x, double y, double m )
        {
            pos = new Vector(x, y);
            mass = m;
            sr = (2 * 3.54 * mass) / (30 * 30);
        }

        public void Pull(Photon p)
        {
            Vector force = pos - p.Pos;
            double theta = Vector.AngleBetween(force, new Vector(1, 0)) * Math.PI / 180;
            double r = force.Length;
            double fg = 3.54 * mass / (r * r);
            double deltaTheta = -fg * (0.05 / 30) * Math.Sin(p.Theta - theta);
            deltaTheta = deltaTheta / (Math.Abs(1.0 - 2.0 * 3.54 * mass / (r * 30 * 30)));
            p.Theta += deltaTheta;
            p.Velocity = 30 * new Vector(Math.Cos(p.Theta), -Math.Sin(p.Theta));
            if (r < sr + 0.2)
            {
                p.Dead();
                p.Trajectory.Stroke = new SolidColorBrush(Color.FromArgb(150, 255, 0, 0));
            }
        }


            public void Show(Canvas c)
            {
            //Draw the Accretion Disk
            Diskouter = new Ellipse()
            {
                Width = 8 * sr,
                Height = 8 * sr,
                Stroke = Brushes.Yellow,
                StrokeThickness = 1.0,
                Fill = Brushes.Yellow
            };
            Diskouter.SetValue(Canvas.LeftProperty, pos.X - 4 * sr);
            Diskouter.SetValue(Canvas.TopProperty, pos.Y - 4 * sr);
            c.Children.Add(Diskouter);
            Diskinner = new Ellipse()
            {
                Width = 6 * sr,
                Height = 6 * sr,
                Stroke = Brushes.Black,
                StrokeThickness = 0.0,
                Fill = Brushes.Black
            };
            Diskinner.SetValue(Canvas.LeftProperty, pos.X - 3 * sr);
            Diskinner.SetValue(Canvas.TopProperty, pos.Y - 3 * sr);
            c.Children.Add(Diskinner);
            //Draw the Photon Orbit Radius
            Orbit = new Ellipse()
            {
                Width = 3 * sr,
                Height = 3 * sr,
                Stroke = Brushes.Orange,
                StrokeThickness = 1.0,
                Fill = Brushes.Black
            };
            Orbit.SetValue(Canvas.LeftProperty, pos.X - 1.5 * sr);
            Orbit.SetValue(Canvas.TopProperty, pos.Y - 1.5 * sr);
            c.Children.Add(Orbit);
            //Draw the SchwartzShield Radius
            Hole = new Ellipse()
            {
                Width = 2 * sr,
                Height = 2 * sr,
                Stroke = Brushes.Purple,
                StrokeThickness = 1.0,
                Fill = Brushes.Black
            };
            Hole.SetValue(Canvas.LeftProperty, pos.X - sr);
            Hole.SetValue(Canvas.TopProperty, pos.Y - sr);
            c.Children.Add(Hole);
            //Draw the Shadow outline at 2.6 * SchwartzShield Radius
            Line sl = new Line()
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 3.0,
                X1 = 0.0,
                Y1 = c.ActualHeight / 2 - 2.6 * sr,
                X2 = c.ActualWidth,
                Y2 = c.ActualHeight / 2 - 2.6 * sr
            };
            c.Children.Add(sl);
            sl = new Line()
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 3.0,
                X1 = 0.0,
                Y1 = c.ActualHeight / 2 + 2.6 * sr,
                X2 = c.ActualWidth,
                Y2 = c.ActualHeight / 2 + 2.6 * sr
            };
            c.Children.Add(sl);
            }
        }
    }
