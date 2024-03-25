using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Apollonian_Gasket
{
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate(int t);
        List<Circle> AllCircles = new List<Circle>();
        private Random Rnd = new Random();
        private double R1;
        private double R2fraction;
        private System.Windows.Vector V2;
        private bool useColor = true;
        private List<Brush> myColors = new List<Brush>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Test.cpl");
            myColors = pal.GetColorBrushes(800);
            //Draw a circle that fits inside the canvas
            System.Windows.Vector center = new System.Windows.Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            R1 = canvas1.ActualWidth / 2 - 20;
            Circle C1 = new Circle(new Complex(center.X, center.Y), -1 / R1);
            C1.hasGasket = false;
            R2fraction = 0.2 * Rnd.NextDouble() + 0.2;
            V2 = new System.Windows.Vector(2 * Rnd.NextDouble() - 1, 2 * Rnd.NextDouble() - 1);
            CreateGasket(C1);
            for (int i = 1; i < AllCircles.Count(); i++)
            {
                if (AllCircles[i].Radius > 10)
                {
                    Debug.Print(AllCircles.Count().ToString());
                    if(!AllCircles[i].hasGasket) CreateGasket(AllCircles[i]);
                }
            }
        }

        private void CreateGasket(Circle C1)
        {
            if (C1.hasGasket) { return; }
            C1.hasGasket = true;
            if (C1.Curvature > 0) { C1.Curvature = -1 * C1.Curvature; }
            List<Triple> Triples = new List<Triple>();
            System.Windows.Vector Cent = new System.Windows.Vector(C1.Center.Real, C1.Center.Imaginary);
            //Draw a circle that is tangent to the Circle C1 on the inside
            double R2 = R2fraction * C1.Radius;
            System.Windows.Vector v = V2;
            v.Normalize();
            v = Cent + (C1.Radius - R2) * v;
            Circle C2 = new Circle(new Complex(v.X, v.Y), 1 / R2);
            C2.hasGasket = false;
            //Draw a circle with radius R1 - R2 that is tangent to both other circles
            double R3 = C1.Radius - R2;
            v = Cent - v;
            v.Normalize();
            v = Cent + (C1.Radius - R3) * v;
            Circle C3 = new Circle(new Complex(v.X, v.Y), 1 / R3);
            C3.hasGasket = false;
            //A triple contains 3 circles that are mutually tangent
            Triple T = new Triple(C1, C2, C3);
            Triples.Add(T);
            //Add the circles to a list. This is needed to check for doubles.
            AllCircles.Add(C1);
            AllCircles.Add(C2);
            AllCircles.Add(C3);
            if (useColor)
            {
                C1.color = myColors[(int)(C1.Radius / R1 * (myColors.Count() - 1))];
                C2.color = myColors[(int)(C2.Radius / R1 * (myColors.Count() - 1))];
                C3.color = myColors[(int)(C3.Radius / R1 * (myColors.Count() - 1))];
            }
            C1.Draw(canvas1);
            C2.Draw(canvas1);
            C3.Draw(canvas1);
            do
            {
                List<Triple> newTriples = new List<Triple>();
                List<Circle> newCircles;

                //Each Triple can generate up to 4 new circles that are tangent to all 3 circles of the Triple
                foreach (Triple t in Triples)
                {
                    newCircles = t.GetTangentCircles();
                    foreach (Circle c in newCircles)
                    {
                        if (ValidateCircle(c, AllCircles, t))
                        {
                            AllCircles.Add(c);
                            c.hasGasket = false;
                            c.color = myColors[(int)(c.Radius / t.Circle1.Radius * (myColors.Count() - 1))];
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

        private bool ValidateCircle(Circle c, List<Circle> Circles, Triple t)
        {
            if (c.Radius < 2) { return false; }
            for (int i = 0; i < Circles.Count(); i++)
            {
                if (Circles[i].Equals(c)) { return false; }
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}