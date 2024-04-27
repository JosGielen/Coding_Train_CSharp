using JG_GL;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3D_Earthquake_Data
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient httpclient = new HttpClient();
        private EllipsoidGeometry Earth;
        List<EQData> EarthQuakes;
        private double Scale = 0.5;
        private double my_RotationAngle;
        private double RotationAngleStep = 0.0;
        private double MinMagnitude = 2.0;
        //Camera Start Data
        private Vector3D CamStartPos = new Vector3D(0.0, 0.0, 200.0);
        private Vector3D CamTargetPos = new Vector3D(0.0, 0.0, 0.0);
        private Vector3D CamUpDir = new Vector3D(0.0, 1.0, 0.0);

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Add a Trackball camera
            scene1.Camera = new TrackballCamera(CamStartPos, CamTargetPos, CamUpDir)
            {
                MoveSpeed = 2.0
            };
            scene1.ViewDistance = 500;
            //Create the sphere
            Earth = new EllipsoidGeometry(100, 100, 100, 32, 32)
            {
                AmbientMaterial = Color.FromRgb(0, 0, 0),
                DiffuseMaterial = Color.FromRgb(0, 0, 0),
                SpecularMaterial = Color.FromRgb(255, 255, 255),
                Shininess = 50,
                RotationAxis = new Vector3D(0.0, 1.0, 0.0),
                RotationAngle = 0.0,
                DrawMode = DrawMode.Fill,
                TextureFile = Environment.CurrentDirectory + "\\Earth.jpg",
                UseTexture = true
            };
            scene1.AddGeometry(Earth);
            my_RotationAngle = 0.0;
            //Get the earthquake data
            string address = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_week.csv";
            StreamReader reader = new StreamReader(await httpclient.GetStreamAsync(address));
            EarthQuakes = new List<EQData>();
            EQData EQ;
            reader.ReadLine(); //Reads the file Header 
            while (!reader.EndOfStream)
            {
                EQ = new EQData(reader.ReadLine(), Scale);
                if (EQ.Magnitude > MinMagnitude)
                {
                    EarthQuakes.Add(EQ);
                    EQ.Show(scene1);
                }
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            //Rotate the Earth and the earthquake markers
            my_RotationAngle -= RotationAngleStep;
            if (Math.Abs(my_RotationAngle) >= 360) my_RotationAngle = 0.0;
            Earth.RotationAngle = my_RotationAngle;
            for (int i = 0; i < EarthQuakes.Count; i++)
            {
                EarthQuakes[i].Update(RotationAngleStep);
            }
            //Render the scene.
            scene1.Render();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}