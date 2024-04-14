using JG_GL;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Solar_System
{
    public partial class MainWindow : Window
    {
        private bool App_Loaded = false;
        private List<Planet> planets = new List<Planet>();
        private List<string> planetData = new List<string>();
        //Simulation Parameters
        private double TimeScale = 1.0 / 250.0;    //General Simulation speed
        //Camera Start Data
        private Vector3D CamStartPos = new Vector3D(200.0, 100.0, 200.0);
        private Vector3D CamTargetPos = new Vector3D(0.0, 0.0, 0.0);
        private Vector3D CamUpDir = new Vector3D(0.0, 1.0, 0.0);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Add a Trackball camera focussed on the Sun.
            scene1.Camera = new TrackballCamera(CamStartPos, CamTargetPos, CamUpDir)
            {
                MoveSpeed = 2.0
            };
            scene1.ViewDistance = 5500;
            //Read the planet Data from the file
            using (StreamReader sr = File.OpenText("Solar System Data.txt"))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    planetData.Add(sr.ReadLine());
                }
            }
            //Create the Sun and planets
            CreatePlanet("Sun"     , 1.0 / 50000.0,    1.0,    1.0);
            CreatePlanet("Mercury" , 1.0 / 1000.0,    0.75,    1.0);
            CreatePlanet("Venus"   , 1.0 / 1000.0,    0.75,    1.0);
            CreatePlanet("Earth"   , 1.0 / 1000.0,    0.75,   8.0);
            CreatePlanet("Mars"    , 1.0 / 1000.0,    0.75,   8.0);
            CreatePlanet("Jupiter" , 1.0 / 2000.0,   0.40,    15.0);
            CreatePlanet("Saturn"  , 1.0 / 2000.0,   0.40,    15.0);
            CreatePlanet("Uranus"  , 1.0 / 2000.0,   0.35,    15.0);
            CreatePlanet("Neptune" , 1.0 / 2000.0,   0.35,    15.0);
            CreatePlanet("Pluto"   , 1.0 / 1000.0,   0.35,    10.0);
            //Create some of the moons
            CreateMoon(planets[3], "Moon",      1.0 / 1000.0, 40.0, 1.0);
            CreateMoon(planets[5], "Io",        1.0 / 750.0, 130.0, 1.0);
            CreateMoon(planets[5], "Europa",    1.0 / 750.0, 95.0, 1.0);
            CreateMoon(planets[5], "Ganymedes", 1.0 / 750.0, 70.0, 1.0);
            CreateMoon(planets[5], "Callisto",  1.0 / 750.0, 60.0, 1.0);
            CreateMoon(planets[6], "Titan",     1.0 / 500.0, 50.0, 1.0);
            CreateMoon(planets[6], "Rhea",      1.0 / 500.0, 90.0, 1.0);
            CreateMoon(planets[7], "Ariel",     1.0 / 500.0, 100.0, 1.0);
            CreateMoon(planets[7], "Umbriel",   1.0 / 500.0, 100.0, 1.0);
            CreateMoon(planets[7], "Titania",   1.0 / 500.0, 100.0, 1.0);
            CreateMoon(planets[7], "Oberon",    1.0 / 500.0, 90.0, 1.0);
            CreateMoon(planets[8], "Triton",    1.0 / 500.0, 90.0, 1.0);
            CreateMoon(planets[9], "Charon",    1.0 / 500.0, 400.0, 1.0);
            App_Loaded = true;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //Update the Sun and Planets (Moons get automatic updated)
            for (int i = 0; i < planets.Count; i++)
            {
                planets[i].Update();
            }
            //Update the scene
            foreach (GLGeometry geo in scene1.Geometries)
            {
                geo.Update();
            }
            //Render the scene.
            scene1.Render();
        }

        /// <summary>
        /// Create a planet (rotates around the solar system center)
        /// </summary>
        /// <param name="name">The name of the planet</param>
        /// <param name="SizeFactor">Scale the size of the planet to reduce the large size differences between the planets.</param>
        /// <param name="DistanceFactor">Scale the distance of the planet from the Sun to allow showing the outer planets.</param>
        /// <param name="RotationTimeFactor">Reduce the rotation of some planets to allow viewing the texture details</param>
        private void CreatePlanet(string name, double SizeFactor, double DistanceFactor, double RotationTimeFactor)
        {
            string[] items;
            Vector3D center = new Vector3D(0.0, 0.0, 0.0);
            Planet p;
            for (int i = 0; i < planetData.Count; i++)
            {
                items = planetData[i].Split(';');
                if (items[0] == name)
                {
                    p = new Planet(center, SizeFactor * double.Parse(items[2]))
                    {
                        RotationTime = TimeScale * RotationTimeFactor * double.Parse(items[1]),
                        Distance = DistanceFactor * double.Parse(items[3]),
                        OrbitTime = TimeScale * double.Parse(items[4]),
                        TextureFile = Environment.CurrentDirectory + "//images//" + name + ".jpg"
                    };
                    p.Draw(scene1);
                    planets.Add(p);
                    break;
                }
            }
        }

        /// <summary>
        /// Create a moon (rotates around a planet)
        /// </summary>
        /// <param name="planet">The planet that has the moon</param>
        /// <param name="name">The name of the moon</param>
        /// <param name="SizeFactor">Scale the size of the moon to reduce the large size differences between the planets.</param>
        /// <param name="DistanceFactor">Scale the distance of the moon from the planet to prevent the moon inside the planet sphere.</param>
        /// <param name="RotationTimeFactor">Reduce the rotation of some moons to allow viewing the texture details</param>
        private void CreateMoon(Planet planet, string name, double SizeFactor, double DistanceFactor, double RotationTimeFactor)
        {
            string[] items;
            Vector3D center = planet.Position;
            Planet m;
            for (int i = 0; i < planetData.Count; i++)
            {
                items = planetData[i].Split(';');
                if (items[0] == name)
                {
                    m = new Planet(center, SizeFactor * double.Parse(items[2]))
                    {
                        RotationTime = TimeScale * RotationTimeFactor * double.Parse(items[1]),
                        Distance = DistanceFactor * double.Parse(items[3]),
                        OrbitTime = TimeScale * double.Parse(items[4]),
                        TextureFile = Environment.CurrentDirectory + "//images//Moon.jpg"
                    };
                    m.Draw(scene1);
                    planet.AddMoon(m);
                    break;
                }
            }
        }

        /// <summary>
        /// Speeds up the time. TimeScale = 1 / value because smaller times give faster orbits and rotations.
        /// </summary>
        private void SldSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!App_Loaded) return;
            double newSpeed = SldSpeed.Value;
            if (newSpeed == 0) newSpeed = 0.1;
            SetTimeScale( 1.0 / newSpeed);
        }

        private void SetTimeScale(double newTimeScale)
        {
            for (int i = 0; i < planets.Count; i++)
            {
                planets[i].AdjustRotationTime(TimeScale / newTimeScale);
                planets[i].AdjustOrbitTime(TimeScale / newTimeScale);
            }
            TimeScale = newTimeScale;
        }

        private void CameraControl_Click(object sender, RoutedEventArgs e)
        {
            if (!App_Loaded) return;
            string target = (string)((Button)sender).Tag;
            for ( int i = 0;i < planets.Count;i++)
            {
                planets[i].CanOrbit = true;
            }
            if (target == "BtnSun")
            {
                SetTimeScale (1.0 / 250.0);
                SldSpeed.Value = 250.0;
                scene1.Camera.Position = CamStartPos;
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnMercury")
            {
                SetTimeScale(1.0 /100.0);
                SldSpeed.Value = 100.0;
                planets[1].CanOrbit = false;
                scene1.Camera.Position = planets[1].Position + new Vector3D(15.0, 1.0, 0.01);
                scene1.Camera.TargetPosition = planets[1].Position;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnVenus")
            {
                SetTimeScale(1.0 / 100.0);
                SldSpeed.Value = 100.0;
                planets[2].CanOrbit = false;
                scene1.Camera.Position = planets[2].Position + new Vector3D(20.0, 1.0, 0.01); ;
                scene1.Camera.TargetPosition = planets[2].Position;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnEarth")
            {
                SetTimeScale(1.0 / 50.0);
                SldSpeed.Value = 50.0;
                planets[3].CanOrbit = false;
                scene1.Camera.Position = planets[3].Position + new Vector3D(35.0, 10.0, 0.01); ;
                scene1.Camera.TargetPosition = planets[3].Position;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnMars")
            {
                SetTimeScale(1.0 / 30.0);
                SldSpeed.Value = 30.0;
                planets[4].CanOrbit = false;
                scene1.Camera.Position = planets[4].Position + new Vector3D(15.0, 1.0, 0.01); ;
                scene1.Camera.TargetPosition = planets[4].Position;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnJupiter")
            {
                SetTimeScale(1.0 / 15.0);
                SldSpeed.Value = 15.0;
                planets[5].CanOrbit = false;
                scene1.Camera.Position = planets[5].Position + new Vector3D(220, 40.0, 0.01); ;
                scene1.Camera.TargetPosition = planets[5].Position;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnSaturn")
            {
                SetTimeScale(1.0 / 25.0);
                SldSpeed.Value = 25.0;
                planets[6].CanOrbit = false;
                scene1.Camera.Position = planets[6].Position + new Vector3D(180, 40.0, 0.01); ;
                scene1.Camera.TargetPosition = planets[6].Position;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnUranus")
            {
                SetTimeScale(1.0 / 15.0);
                SldSpeed.Value = 15.0;
                planets[7].CanOrbit = false;
                scene1.Camera.Position = planets[7].Position + new Vector3D(120, 40.0, 0.01); ;
                scene1.Camera.TargetPosition = planets[7].Position;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnNeptune")
            {
                SetTimeScale(1.0 / 25.0);
                SldSpeed.Value = 25.0;
                planets[8].CanOrbit = false;
                scene1.Camera.Position = planets[8].Position + new Vector3D(90, 30.0, 0.01); ;
                scene1.Camera.TargetPosition = planets[8].Position;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnPluto")
            {
                SetTimeScale(1.0 / 15.0);
                SldSpeed.Value = 15.0;
                planets[9].CanOrbit = false;
                scene1.Camera.Position = planets[9].Position + new Vector3D(20, 12.0, 0.01); ;
                scene1.Camera.TargetPosition = planets[9].Position;
                scene1.Camera.UpdatePosition();
                return;
            }



            if (target == "BtnMercuryOrbit")
            {
                SetTimeScale(1.0 / 100.0);
                SldSpeed.Value = 100.0;
                scene1.Camera.Position = new Vector3D(0.01, 100.0, 0.01);
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnVenusOrbit")
            {
                SetTimeScale(1.0 / 200.0);
                SldSpeed.Value = 200.0;
                scene1.Camera.Position = new Vector3D(0.01, 200.0, 0.01); ;
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnEarthOrbit")
            {
                SetTimeScale(1.0 / 200.0);
                SldSpeed.Value = 200.0;
                scene1.Camera.Position = new Vector3D(0.01, 300.0, 0.01); ;
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnMarsOrbit")
            {
                SetTimeScale(1.0 / 200.0);
                SldSpeed.Value = 200.0;
                scene1.Camera.Position = new Vector3D(0.01, 400.0, 0.01); ;
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnJupiterOrbit")
            {
                SetTimeScale(1.0 / 200.0);
                SldSpeed.Value = 200.0;
                scene1.Camera.Position = new Vector3D(0.01, 800.0, 0.01); ;
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnSaturnOrbit")
            {
                SetTimeScale(1.0 / 300.0);
                SldSpeed.Value = 300.0;
                scene1.Camera.Position = new Vector3D(0.01, 1500.0, 0.01); ;
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnUranusOrbit")
            {
                SetTimeScale(1.0 / 300.0);
                SldSpeed.Value = 300.0;
                scene1.Camera.Position = new Vector3D(0.01, 2300.0, 0.01); ;
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnNeptuneOrbit")
            {
                SetTimeScale(1.0 / 300.0);
                SldSpeed.Value = 300.0;
                scene1.Camera.Position = new Vector3D(0.01, 3800.0, 0.011); ;
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
            if (target == "BtnPlutoOrbit")
            {
                SetTimeScale(1.0 / 300.0);
                SldSpeed.Value = 300.0;
                scene1.Camera.Position = new Vector3D(0.01, 5000.0, 0.01); ;
                scene1.Camera.TargetPosition = CamTargetPos;
                scene1.Camera.UpdatePosition();
                return;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}