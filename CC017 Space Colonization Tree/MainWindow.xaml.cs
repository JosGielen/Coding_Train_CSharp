using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Space_Colonization_Tree
{
    public partial class MainWindow : Window
    {
        private bool App_Loaded = false;
        private int WaitTime = 5;
        private delegate void WaitDelegate(int t);
        private int IterationCount = 0;
        private int previousbranches = 0;
        private bool Growing = true;
        private Random Rnd;
        //Tree data
        private Tree my_Tree;
        private int LeafCount = 350;
        private Leaf[] Leafs;
        private double TreeRadius = 200.0;
        private Vector TreeCenter;
        private Vector RootPosition;
        private double KillDistance = 2.0;
        private double ViewDistance = 50.0;
        private double BranchLength = 5.0;
        private double LeafRadius = 6.0;
        private bool ShowLeaves = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            App_Loaded = true;
        }

        private void Init()
        {
            Rnd = new Random();
            Vector pos;
            //Do the program initialisation
            Leafs = new Leaf[LeafCount];
            TreeCenter = new Vector(canvas1.ActualWidth / 2, TreeRadius);
            RootPosition = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight);
            //  Create a Tree
            my_Tree = new Tree(KillDistance, ViewDistance);
            //  Add leaves to the Tree inside a circle
            double X, Y;
            for (int I = 0; I < LeafCount; I++)
            {
                do
                {
                    X = canvas1.ActualWidth * Rnd.NextDouble();
                    Y = canvas1.ActualHeight * Rnd.NextDouble();
                } while (Math.Sqrt(((X - TreeCenter.X) * (X - TreeCenter.X)) + ((Y - TreeCenter.Y) * (Y - TreeCenter.Y))) > TreeRadius);
                pos = new Vector(X, Y);
                my_Tree.AddLeaf(new Leaf(pos));
            }
            my_Tree.Leafs.CopyTo(Leafs);
            //  Set the Tree Root (= First Branch)
            my_Tree.SetRoot(RootPosition, new Vector(0.0, -1.0), BranchLength);
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            if (!App_Loaded) return;
            if (Growing) Grow(); //Stop growing when no new branches during 20 iterations
        }

        private void Grow()
        {
            List<Branch> newbranches = new List<Branch>();
            newbranches = my_Tree.Grow();
            if (newbranches.Count == previousbranches)
            {
                IterationCount += 1;
                if (my_Tree.Branches.Count > 50 & IterationCount == 5)
                {
                    Growing = false;
                    if (ShowLeaves)
                    {
                        //Draw Circles that represent the leafs
                        Vector pt;
                        Ellipse El;
                        for (int I = 0; I < Leafs.Length; I++)
                        {
                            pt = Leafs[I].Location;
                            El = new Ellipse()
                            {
                                Width = 2 * LeafRadius,
                                Height = 2 * LeafRadius,
                                Stroke = Brushes.Green,
                                StrokeThickness = 1.0,
                                Fill=Brushes.Green
                            };
                            El.SetValue(Canvas.LeftProperty, pt.X - LeafRadius);
                            El.SetValue(Canvas.TopProperty, pt.Y - LeafRadius);
                            canvas1.Children.Add(El);
                        }
                    }
                }
            }
            else
            {
                IterationCount = 0;
                Growing = true;
            }
            previousbranches = newbranches.Count;
            //Draw CylinderLines that represent the branches
            Vector pt1;
            Vector pt2;
            double Thickness;
            Line l;
            for (int I = 0; I < newbranches.Count; I++) //First branch has no Parent
            {
                //Thickness goes from 1 at Location.Y = (TreeCenter + TreeRadius) to 8 at Location.Y = RootPosition;
                //It is reduced by distance from Location to TreeCenter
                Thickness = 7 * (newbranches[I].Parent.Location.Y - TreeCenter.Y) / (RootPosition.Y - TreeCenter.Y) + 1;
                Thickness -= 0.001 * (Math.Sqrt((newbranches[I].Parent.Location.X - TreeCenter.X) * (newbranches[I].Parent.Location.X - TreeCenter.X) + (newbranches[I].Parent.Location.Y - TreeCenter.Y) * (newbranches[I].Parent.Location.Y - TreeCenter.Y)));
                if (Thickness < 1) Thickness = 1;
                pt1 = newbranches[I].Location;
                pt2 = newbranches[I].Parent.Location;
                l = new Line()
                {
                    X1=pt1.X,
                    Y1=pt1.Y,
                    X2=pt2.X,
                    Y2=pt2.Y,
                    Stroke=Brushes.Brown,
                    StrokeThickness = Thickness,
                    Fill=Brushes.Brown
                };
                canvas1.Children.Add(l);
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
    }
}