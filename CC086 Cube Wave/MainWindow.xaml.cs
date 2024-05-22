using JG_GL;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Cube_Wave
{
    public partial class MainWindow : Window
    {
        private List<BoxGeometry> boxes = new List<BoxGeometry>();
        private List<double> distances = new List<double>();
        private double angle;
        private double size = 16;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            scene1.Camera.Position = new Vector3D(250.0, 250.0, 250.0);
            scene1.Camera.TargetPosition = new Vector3D(0.0, 30.0, 0.0);
            scene1.Background = Brushes.Beige;
            double maxD = 5 * size * Math.Sqrt(2); //Distance for the (0,0) center to a corner of x,z plane of the cube
            double ScaledDistance;
            double x, z;
            BoxGeometry box;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    x = 10 * (i - size / 2);
                    z = 10 * (j - size / 2);
                    ScaledDistance = Math.Sqrt(x * x + z * z) / maxD; // 0 in the center and 1 at the corner
                    ScaledDistance = 1.2 * Math.PI * ( 2 * ScaledDistance - 1); //-PI in the center and PI at the corner
                    box = new BoxGeometry(10,100,10)
                    {
                        AmbientMaterial = Color.FromRgb(50, 100, 50),
                        DiffuseMaterial = Color.FromRgb(100, 175, 100),
                        SpecularMaterial = Color.FromRgb(255, 255, 255),
                        Position = new Vector3D(x, 50, z),
                        Shininess = 20
                    };
                    scene1.AddGeometry(box);
                    boxes.Add(box);
                    distances.Add(ScaledDistance);
                }
            }
            angle = 0;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            double Offset;
            for (int i = 0; i < boxes.Count; i++)
            {
                Offset = distances[i] + angle;
                boxes[i].Y_Size = 50 * (Math.Sin(Offset) + 1) + 30;
                boxes[i].GenerateGeometry(scene1);
            }
            scene1.Render();
            angle += 0.05;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}