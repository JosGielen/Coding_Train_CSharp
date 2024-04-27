using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Attraction_and_Repulsion
{
    public partial class MainWindow : Window
    {
        private int WaitTime = 10;
        private delegate void WaitDelegate(int t);
        private List<Vector> Attractors;
        List<Ellipse> ellipses;
        private List<Entity> ents;
        private double maxSpeed = 3.0;
        private double maxForce = 8.0;
        private double Attraction = 1.0;
        private double Repulsion = 8.0;
        private double MinDist = 60.0;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Vector pos;
            Vector dir;
            Entity ent;
            Attractors = new List<Vector>();
            ellipses = new List<Ellipse>();
            //Create an Attractor at the center
            pos = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            Attractors.Add(new Vector(pos.X, pos.Y));
            Ellipse el = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red
            };
            el.SetValue(Canvas.LeftProperty, pos.X - 5);
            el.SetValue(Canvas.TopProperty, pos.Y - 5);
            ellipses.Add(el);
            canvas1.Children.Add(el);

            //Create 50 entities
            ents = new List<Entity>();
            Rect R = new Rect(0.0, 0.0, canvas1.ActualWidth, canvas1.ActualHeight);
            for (int i = 0; i < 50; i++)
            {
                pos = new Vector(canvas1.ActualWidth * Rnd.NextDouble(), canvas1.ActualHeight * Rnd.NextDouble());
                dir = new Vector(2.0 * Rnd.NextDouble() - 1.0, 2.0 * Rnd.NextDouble() - 1.0);
                ent = new Entity(pos, dir, 1.0);
                ent.LimitSpeed(maxSpeed);
                ent.HasConstantSpeed = true;
                ent.LimitForce(maxForce);
                ent.SetWorld(R);
                ent.Life = (int)(300 * Rnd.NextDouble() + 200);
                ent.Draw(canvas1);
                ents.Add(ent);
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Vector v;
            for (int i = 0; i < ents.Count; i++)
            {
                //Apply the forces from the ring of attractors
                foreach (Vector att in Attractors)
                {
                    v = att - ents[i].Location;
                    if (v.Length > MinDist)
                    {
                        v.Normalize();
                        v *= Attraction;
                    }
                    else
                    {
                        v.Normalize();
                        v *= -1 * Repulsion;
                    }
                    ents[i].ApplyForce(v);
                }
                ents[i].Update();
                ents[i].Life--;
                if (ents[i].Life <= 0)
                {
                    ents[i].Life = (int)(300 * Rnd.NextDouble() + 200);
                    ents[i].Location = new Vector(canvas1.ActualWidth * Rnd.NextDouble(), canvas1.ActualHeight * Rnd.NextDouble());
                }
            }
            Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                //Remove all Attractors
                Attractors.Clear();
                for (int i = 0; i < ellipses.Count; i++)
                {
                    if (canvas1.Children.Contains(ellipses[i]))
                    {
                        canvas1.Children.Remove(ellipses[i]);
                    }
                }
                ellipses.Clear();
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                //Create an Attractor at the mouse position
                Point p = e.GetPosition(canvas1);
                Attractors.Add(new Vector(p.X, p.Y));
                Ellipse el = new Ellipse()
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red
                };
                el.SetValue(Canvas.LeftProperty, p.X - 5);
                el.SetValue(Canvas.TopProperty, p.Y - 5);
                ellipses.Add(el);
                canvas1.Children.Add(el);
            }
        }
    }
}