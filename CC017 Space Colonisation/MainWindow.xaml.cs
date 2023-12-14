using JG_GL;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace _3D_Space_Colonisation_Tree
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
        private int LeafCount = 250;
        private Leaf[] Leafs;
        private double TreeRadius = 35.0;
        private Vector3D TreeCenter;
        private Vector3D RootPosition;
        private double KillDistance = 2.0;
        private double ViewDistance = 35.0;
        private double BranchLength = 4.0;
        private bool ShowLeaves = true;
        private bool UseTexture = false;
        //Camera positioning
        private double Distance = 150; //Camera distance from the center of the scene
        private double rotateAngle = 0;

        public MainWindow()
        {
            InitializeComponent();
            Leafs = new Leaf[LeafCount];
            TreeCenter = new Vector3D(0.0, 0.5 * TreeRadius, 0.0);
            RootPosition = new Vector3D(0.0, -2 * TreeRadius, 0.0);
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Init();
            Scene1.Camera.Position = new Vector3D(0, 0, Distance);
            BoxGeometry b = new BoxGeometry(150, 20, 150)
            {
                Position = RootPosition,
                AmbientMaterial = Colors.Green,
                DiffuseMaterial = Colors.Green,
                SpecularMaterial = Colors.Black,
                Shininess = 50
            };
            if (UseTexture)
            {
                b.TextureFile = Environment.CurrentDirectory + "\\grass.jpg";
                b.UseTexture = true;
            }
            Scene1.AddGeometry(b);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            App_Loaded = true;
        }

        private void Init()
        {
            Rnd = new Random();
            Vector3D pos;
            //Do the program initialisation
            //  Create a Tree
            my_Tree = new Tree(KillDistance, ViewDistance);
            //  Add leaves to the Tree inside a spherical volume
            for (int I = 0; I < LeafCount; I++)
            {
                double X = TreeRadius * (2 * Rnd.NextDouble() - 1);
                double Y = TreeRadius * (2 * Rnd.NextDouble() - 1);
                double Z = TreeRadius * (2 * Rnd.NextDouble() - 1);
                pos = new Vector3D(X, Y, Z) + TreeCenter;
                my_Tree.AddLeaf(new Leaf(pos));
            }
            my_Tree.Leafs.CopyTo(Leafs);
            //  Set the Tree Root (= First Branch)
            my_Tree.SetRoot(RootPosition, new Vector3D(0.0, 1.0, 0.0), BranchLength);
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            if (!App_Loaded) return;
            if (Growing) Grow(); //Stop growing when no new branches during 20 iterations
            //Render the scene.
            Scene1.Render();
        }

        private void Grow()
        {
            List< Branch> newbranches = new List<Branch>();
            newbranches = my_Tree.Grow();
            if (newbranches.Count == previousbranches)
            {
                IterationCount += 1;
                if (my_Tree.Branches.Count > 50 & IterationCount == 5)
                {
                    Growing = false;
                    if (ShowLeaves)
                    {
                        //Draw Ellipsoids that represent the leafs
                        Vector3D pt;
                        EllipsoidGeometry El;
                        for (int I = 0; I < Leafs.Length; I++)
                        {
                            pt = Leafs[I].Location;
                            El = new EllipsoidGeometry(7, 7, 7, 6, 6)
                            {
                                Position = Leafs[I].Location,
                                AmbientMaterial = Color.FromRgb(0, 160, 0),
                                DiffuseMaterial = Color.FromRgb(0, 160, 0),
                                SpecularMaterial = Colors.White,
                                Shininess = 50
                            };
                            if (UseTexture)
                            {
                                El.AmbientMaterial = Color.FromRgb(0, 100, 0);
                                El.DiffuseMaterial = Color.FromRgb(0, 100, 0);
                                El.TextureScaleX = 2;
                                El.TextureScaleY = 2;
                                El.TextureFile = Environment.CurrentDirectory + "\\leaf1.jpg";
                                El.UseTexture = true;
                            }
                            Scene1.AddGeometry(El);
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
            Vector3D pt1;
            Vector3D pt2;
            double Thickness;
            CylinderLineGeometry cyl;
            for (int I = 0; I < newbranches.Count; I++) //First branch has no Parent
             {
                //Thickness goes from 1 at Location.Y = (TreeCenter + TreeRadius) to 8 at Location.Y = RootPosition;
                //It is reduced by XZ distance from Location to TreeCenter
                Thickness = 7 * (newbranches[I].Parent.Location.Y - (TreeCenter.Y + TreeRadius)) / (RootPosition.Y - (TreeCenter.Y + TreeRadius)) + 1;
                Thickness -= 0.1 * (Math.Sqrt((newbranches[I].Parent.Location.X - TreeCenter.X) * (newbranches[I].Parent.Location.X - TreeCenter.X) + (newbranches[I].Parent.Location.Z - TreeCenter.Z) * (newbranches[I].Parent.Location.Z - TreeCenter.Z)));
                if (Thickness < 0.5) Thickness = 0.5;
                pt1 = newbranches[I].Location;
                pt2 = newbranches[I].Parent.Location;
                cyl = new CylinderLineGeometry(pt1, pt2, Thickness, 6)
                {
                    Position = new Vector3D(0, 0, 0),
                    InitialRotationAxis = new Vector3D(0, 0, 0),
                    AmbientMaterial = Color.FromRgb(120, 100, 20),
                    DiffuseMaterial = Color.FromRgb(120, 100, 20),
                    SpecularMaterial = Colors.White,
                    Shininess = 30
                };
                if (UseTexture)
                {
                    cyl.AmbientMaterial = Color.FromRgb(100, 70, 20);
                    cyl.DiffuseMaterial = Color.FromRgb(100, 70, 20);
                    cyl.TextureFile = Environment.CurrentDirectory + "\\bark.jpg";
                    cyl.UseTexture = true;
                }
                Scene1.AddGeometry(cyl);
            }
            Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}