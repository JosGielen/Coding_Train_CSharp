using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace _3D_Perlin_Terrain
{
    public partial class MainWindow : Window
    {
        private bool Rendering = false;
        private MeshGeometry Mesh;
        private readonly int XSize = 250;
        private readonly int ZSize = 250;
        private readonly double CellSize = 0.3;
        private double Scale = 0.15;
        private readonly double PeakHeight = 8.0;
        private int Roughness = 4;
        private readonly double[,] Elevations;
        private bool UseOpenSimplex = false ;
        private bool UseFastSimplex = false; 
        private double ZOff = 0.0;

        public MainWindow()
        {
            InitializeComponent();
            Elevations = new double[XSize, ZSize];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Mesh = new MeshGeometry(XSize, ZSize, CellSize)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 0, 0),
                RotationSpeed = 0.0,
                AmbientMaterial = Colors.Black,
                DiffuseMaterial = Color.FromRgb(150, 150, 150),
                SpecularMaterial = Colors.Black,
                Shininess = 30,
                DrawMode = DrawMode.Fill,
                LineWidth = 1.0,
                PointSize = 1.0,
                UseMaterial = true,
                UseVertexColors = false,
                UseTexture = false,
                TextureScaleX = 1.0,
                TextureScaleY = 1.0,
                VertexColorIntensity = 1.0
            };
            Scene1.Lights[0].Direction = new Vector3D(-3, -2, -1);
            //Scene1.Camera.Position = new Vector3D(0, 12.0, 42.0);
            Scene1.Camera = new FixedCamera(new Vector3D(0, 1.5 * PeakHeight, ZSize * CellSize / 2), new Vector3D(0, 2, 0), new Vector3D(0, 1, 0));
            Scene1.AddGeometry(Mesh);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                Scale = SldScale.Value;
                Roughness = (int)SldRoughness.Value;
                SldScale.IsEnabled = false;
                SldRoughness.IsEnabled = false;
                Init();
                Mesh.TextureFile = Environment.CurrentDirectory + "\\Terrain.jpg";
                Mesh.UseTexture = true;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                BtnStart.Content = "STOP";
                Rendering = true;
            }
            else
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                SldScale.IsEnabled = true;
                SldRoughness.IsEnabled = true;
                BtnStart.Content = "START";
                Rendering = false;
            }
        }

        private void Init()
        {
            UseOpenSimplex = RbOpenSimplex.IsChecked.Value;
            UseFastSimplex = RbFastSimplex.IsChecked.Value;
            double h;
            for (int I = 0; I < XSize; I++)
            {
                for (int J = 0; J < ZSize -1; J++)
                {
                    if (UseFastSimplex)
                    {
                        h = PeakHeight * FastSimplexNoise.WideNoise2D(I * Scale * CellSize / 2, J * Scale * CellSize / 2 + ZOff / 2, Roughness, 0.5, 1) - 0.3;
                    }
                    else if (UseOpenSimplex)
                    {
                        h = PeakHeight * OpenSimplexNoise.WideNoise2D(I * Scale * CellSize, J * Scale * CellSize + ZOff, Roughness, 0.5, 1) - 0.3;
                    }
                    else
                    {
                        h = PeakHeight * PerlinNoise.WideNoise2D(I * Scale * CellSize, J * Scale * CellSize + ZOff, Roughness, 0.5, 1) - 0.3;
                    }
                    if (h < 0) { h = 0.05; }
                    Elevations[I, J] = h;
                }
                Elevations[I, ZSize - 1] = 0.8 * PeakHeight;
            }
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            if (!Rendering) { return; }
            double h;
            for (int I = 0; I < XSize; I++)
            {
                if (UseFastSimplex)
                {
                    h = PeakHeight * FastSimplexNoise.WideNoise2D(I * Scale * CellSize / 2, ZOff / 2, Roughness, 0.5, 1) - 0.3;
                }
                else if (UseOpenSimplex)
                {
                    h = PeakHeight * OpenSimplexNoise.WideNoise2D(I * Scale * CellSize, ZOff, Roughness, 0.5, 1) - 0.3;
                }
                else
                {
                    h = PeakHeight * PerlinNoise.WideNoise2D(I * Scale * CellSize, ZOff, Roughness, 0.5, 1) - 0.3;
                }
                if (h < 0) { h = 0.05; }
                Elevations[I, 0] = h;
                for (int J = ZSize - 1; J > 0; J--)
                {
                    Elevations[I, J] = Elevations[I, J - 1];
                }
            }
            ZOff -= Scale * CellSize;
            Mesh.Heights = Elevations;
            GC.Collect();
            Mesh.GenerateGeometry(Scene1);
            //Render the scene.
            Scene1.Render();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
