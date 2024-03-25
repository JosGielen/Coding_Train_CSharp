using System.Numerics;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Apollonian_Gasket
{
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate(int t);
        private Random Rnd = new Random();
        private List<Triple> Triples;
        private List<Circle> AllCircles;
        private List<Circle> newCircles;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            Render();
        }

        private void Init()
        {
            canvas1.Children.Clear();
            Triples = new List<Triple>();
            AllCircles = new List<Circle>();
            Circle C1, C2, C3;
            Ellipse el;
            //Draw a circle that fits inside the canvas
            System.Windows.Vector center = new System.Windows.Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            double R1 = canvas1.ActualWidth / 2 - 20;
            C1 = new Circle(new Complex(center.X, center.Y), -1 / R1);
            //Draw a circle that is tangent to the first on the inside
            double R2 = Rnd.Next(70, (int)(R1 / 2));
            System.Windows.Vector v = new System.Windows.Vector(2 * Rnd.NextDouble() - 1, 2 * Rnd.NextDouble() - 1);
            v.Normalize();
            v = center + (R1 - R2) * v;
            C2 = new Circle(new Complex(v.X, v.Y), 1 / R2);
            //Draw a circle with radius R1 - R2 that is tangent to both other circles
            double R3 = R1 - R2;
            v = center - v;
            v.Normalize();
            v = center + (R1 - R3) * v;
            C3 = new Circle(new Complex(v.X, v.Y), 1 / R3);
            //A triple contains 3 circles that are mutually tangent
            Triple T = new Triple(C1, C2, C3);
            Triples.Add(T);
            //Add the circles to a list. This is needed to check for doubles.
            AllCircles.Add(C1);
            AllCircles.Add(C2);
            AllCircles.Add(C3);
            C1.Draw(canvas1);
            C2.Draw(canvas1);
            C3.Draw(canvas1);
        }


        private void Render()
        {
            do
            {
                List<Triple> newTriples = new List<Triple>();
                //Each Triple can generate up to 4 new circles that are tangent to all 3 circles of the Triple
                foreach (Triple t in Triples)
                {
                    newCircles = t.GetTangentCircles();
                    foreach (Circle c in newCircles)
                    {
                        if (ValidateCircle(c, t))
                        {
                            AllCircles.Add(c);
                            c.Draw(canvas1);
                            newTriples.Add(new Triple(t.Circle1, t.Circle2, c));
                            newTriples.Add(new Triple(t.Circle1, t.Circle3, c));
                            newTriples.Add(new Triple(t.Circle2, t.Circle3, c));
                        }
                    }
                }
                Triples = newTriples;
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), 100);
            } while (Triples.Count() > 0);
        }

        private bool ValidateCircle(Circle c, Triple t)
        {
            if (c.Radius < 2) { return false; }
            for (int i = 0; i < AllCircles.Count(); i++)
            {
                if (AllCircles[i].Equals(c)) { return false; }
            }
            if (!c.IsTangent(t.Circle1)) { return false; }
            if (!c.IsTangent(t.Circle2)) { return false; }
            if (!c.IsTangent(t.Circle3)) { return false; }
            return true;
        }

        private void Wait(int time)
        {
            Thread.Sleep(time);
        }
    }
}