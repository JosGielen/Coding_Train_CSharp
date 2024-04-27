using JG_GL;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Texturing_Cloth
{
    public partial class MainWindow : Window
    {
        private Vector3D Gravity = new Vector3D(0.003, -0.001, 0.0);
        private ClothGeometry cloth = new ClothGeometry(15, 10, 10);
        private bool AppLoaded = false;
        private int counter = 0;
        private bool wave = false;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            scene1.Camera.Position = new Vector3D(0.0, -60.0, 300.0);
            scene1.Camera.TargetPosition = new Vector3D(0.0, -60.0, 0.0);
            scene1.Background = Brushes.LightBlue;
            cloth.AmbientMaterial = Color.FromRgb(0, 0, 0);
            cloth.DiffuseMaterial = Color.FromRgb(0, 0, 0);
            cloth.SpecularMaterial = Color.FromRgb(150, 150, 150);
            cloth.Shininess = 80;
            cloth.TextureFile = Environment.CurrentDirectory + "\\BE_Flag.jpg";
            cloth.UseMaterial = true;
            cloth.UseTexture = true;
            cloth.Stiffness = 0.5;
            scene1.AddGeometry(cloth);
            //Lock the left edge of the Cloth in 2 places
            cloth.Particles[0, 0].Locked = true;
            cloth.Particles[0, 9].Locked = true;
            //Add a flag pole
            CylinderGeometry cyl = new CylinderGeometry(5.0, 300.0)
            {
                AmbientMaterial = Color.FromRgb(150, 150, 150),
                DiffuseMaterial = Color.FromRgb(150, 150, 150),
                SpecularMaterial = Color.FromRgb(255, 255, 255),
                Position = new Vector3D(-70, -100, 0),
                Shininess = 20
            };
            scene1.AddGeometry(cyl);
        AppLoaded = true;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!AppLoaded) { return; }
            if (counter % 90 == 0)
            {
                Wave();
            }
            counter++;
            cloth.Update(Gravity);
            cloth.GenerateGeometry(scene1);
            //Render the scene.
            scene1.Render();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Wave()
        {
            Vector3D f;
            double Ampl = 3 * Rnd.NextDouble();
            //TODO Wave the flag
            if (wave)
            {
                f = new Vector3D(0.0, 0.0, Ampl);
                wave = false;
            }
            else
            {
                f = new Vector3D(0.0, 0.0, -Ampl);
                wave = true;
            }
            for (int h = 0; h < 10; h++)
            {
                cloth.Particles[1, h].ApplyForce(f);
            }

        }
    }
}