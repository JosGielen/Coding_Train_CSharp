using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3D_Cloth
{
    public partial class MainWindow : Window
    {
        private readonly int RasterSize = 15;
        private Particle[,] particles;
        private List<Spring> Springs;
        private Vector3D Gravity = new Vector3D(0, -0.001, 0.0);
        private double damping = 0.998;
        private double ParticleMass = 5.0;
        private double SpringLength = 5.0;
        private double SpringStiffness = 0.3;
        private bool ShowParticles = false;
        private bool AppLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            scene1.Camera.Position = new Vector3D(0.0, 50.0, 130.0);
            scene1.Camera.TargetPosition = new Vector3D(0.0, 0.0, 0.0);
            //Step 1: Create the 3D Cloth as a square grid of Particles
            double x, z;
            Springs = new List<Spring>();
            particles = new Particle[RasterSize, RasterSize];
            for (int i = 0; i < RasterSize; i++)
            {
                for (int j = 0; j < RasterSize; j++)
                {
                    x = SpringLength * (i - RasterSize / 2);
                    z = SpringLength * (j - RasterSize / 2);
                    particles[i, j] = new Particle(ParticleMass, new Point3D(x, 15.0, z), ShowParticles);
                    particles[i, j].Damping = damping;
                }
            }
            //Step 2: Connect the Particles with Springs
            for (int i = 0; i < RasterSize - 1; i++)
            {
                for (int j = 0; j < RasterSize - 1; j++)
                {
                    Spring sp1 = new Spring(particles[i,j], particles[i + 1, j]);
                    Spring sp2 = new Spring(particles[i, j], particles[i, j + 1]);
                    sp1.Stiffness = SpringStiffness;
                    sp2.Stiffness = SpringStiffness;
                    Springs.Add(sp1);
                    Springs.Add(sp2);
                    sp1.Draw(scene1, ShowParticles);
                    sp2.Draw(scene1, ShowParticles);
                }
            }
            //Connect the last row and the last column
            for (int i = 0; i < RasterSize - 1; i++)
            {
                Spring sp1 = new Spring(particles[i, RasterSize - 1], particles[i + 1, RasterSize - 1]);
                Spring sp2 = new Spring(particles[RasterSize - 1, i], particles[RasterSize - 1, i + 1]);
                sp1.Stiffness = SpringStiffness;
                sp2.Stiffness = SpringStiffness;
                Springs.Add(sp1);
                Springs.Add(sp2);
                sp1.Draw(scene1, ShowParticles);
                sp2.Draw(scene1, ShowParticles);
            }
            //Lock the upper two corners
            particles[0, 0].Locked = true;
            particles[RasterSize - 1, 0].Locked = true;
            particles[RasterSize - 1, RasterSize - 1].Locked = true;
            particles[0,RasterSize - 1].Locked = true;
            AppLoaded = true;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!AppLoaded) { return; }
            for (int i = 0; i < Springs.Count; i++)
            {
                Springs[i].Update(Gravity);
            }
            for (int i = 0; i < Springs.Count; i++)
            {
                Springs[i].Redraw();
            }
            //Render the scene.
            scene1.Render();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!AppLoaded) { return; }
            particles[RasterSize - 1, RasterSize - 1].Locked = false;
            particles[0, RasterSize - 1].Locked = false;
        }
    }
}