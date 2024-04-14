using System.Windows;
using JG_GL;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Spherical_Geometry
{
    public partial class MainWindow : Window
    {
        EllipsoidGeometry El;
        //Camera Start Data
        private Vector3D CamStartPos = new Vector3D(0.0, 0.0, 300.0);
        private Vector3D CamTargetPos = new Vector3D(0.0, 0.0, 0.0);
        private Vector3D CamUpDir = new Vector3D(0.0, 1.0, 0.0);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Add a Trackball camera
            scene1.Camera = new TrackballCamera(CamStartPos, CamTargetPos, CamUpDir)
            {
                MoveSpeed = 2.0
            };
            scene1.ViewDistance = 500;
            //Create the sphere
            El = new EllipsoidGeometry(200, 200, 200, 32, 32)
            {
                AmbientMaterial = Color.FromRgb(255, 255, 255),
                DiffuseMaterial = Color.FromRgb(255, 255, 255),
                SpecularMaterial = Color.FromRgb(255, 255, 255),
                Shininess = 50,
                RotationAxis = new Vector3D(0.0,0.0,1.0),
                RotationAngle = 30,
                DrawMode = DrawMode.Lines,
                TextureFile = Environment.CurrentDirectory + "\\rainbow3.jpg",
                UseTexture = false
            };
            scene1.AddGeometry(El);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //Render the scene.
            scene1.Render();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ModeChanged(object sender, RoutedEventArgs e)
        {
            if (RbPoints.IsChecked == true) 
            { 
                El.DrawMode= DrawMode.Points;
                El.PointSize = 3.0;
                El.UseTexture = false;
                El.UseMaterial = true;
                CbUseTexture.IsChecked = false;
            }
            if (RbLines.IsChecked == true) { El.DrawMode = DrawMode.Lines; }
            if (RbFilled.IsChecked == true) { El.DrawMode = DrawMode.Fill; }
        }

        private void SldDetail_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) { return; }
            int detail = (int)SldDetail.Value;
            El.Slices = detail;
            El.Stacks = detail;
            El.GenerateGeometry(scene1);
        }

        private void CbUseTexture_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) { return; }
            if (CbUseTexture.IsChecked.Value)
            {
                El.UseMaterial = false;
                El.UseTexture = true;
            }
            else
            {
                El.UseMaterial = true;
                El.UseTexture = false;
            }
            El.GenerateGeometry(scene1);
        }
    }
}