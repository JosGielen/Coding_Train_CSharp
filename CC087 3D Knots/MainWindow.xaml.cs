using System;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace _3D_Knots
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private Settings settingForm;
        private int knotType;
        private double rotationSpeed;
        private bool ShowTexture;
        private DrawMode MyDrawMode;
        private GLGeometry myKnot;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private double FrameCounter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            settingForm = new Settings(this);
            settingForm.Show();
            settingForm.Left = Left + Width;
            settingForm.Top = Top;
            settingForm.CmbKnotType.SelectedIndex = 0;
            rotationSpeed = 0.003;
            ShowTexture = true;
            MyDrawMode = DrawMode.Fill;
            SetScene();
            CreateKnot1(DrawMode.Fill);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void SetScene()
        {
            myScene.Lights.Clear();
            GLLight l1 = new GLLight(LightType.DirectionalLight)
            {
                Direction = new Vector3D(2.0, 2.0, 2.0),
                Ambient = Color.FromRgb(150, 150, 150),
                Diffuse = Color.FromRgb(255, 255, 255),
                Specular = Color.FromRgb(255, 255, 255)
            };
            myScene.AddLight(l1);
            GLLight l2 = new GLLight(LightType.PointLight)
            {
                Position = new Vector3D(-3.0, 0.0, 1.0),
                Ambient = Color.FromRgb(150, 150, 150),
                Diffuse = Color.FromRgb(200, 200, 200),
                Specular = Color.FromRgb(255, 255, 255),
                Linear = 0.5,
                Quadratic = 0,
                Constant = 0.3
            };
            myScene.AddLight(l2);
            GLLight l3 = new GLLight(LightType.SpotLight)
            {
                Position = new Vector3D(2.0, 2.0, -2.0),
                Direction = new Vector3D(-2.0, -2.0, 2.0),
                Ambient = Color.FromRgb(150, 150, 150),
                Diffuse = Color.FromRgb(255, 255, 255),
                Specular = Color.FromRgb(255, 255, 255),
                CutOff = 8.0,
                OuterCutOff = 12.0,
                Linear = 0.09,
                Quadratic = 0.03,
                Constant = 0.3
            };
            myScene.AddLight(l3);
            //Set the camera position
            myScene.Camera.Position = new Vector3D(0, 0, -300);
            myScene.CameraDefaultPosition = myScene.Camera.Position;
        }

        public void GetParameters()
        {
            knotType = settingForm.KnotType;
            switch(knotType)
            {
                case 1:
                {
                    CreateKnot1(MyDrawMode);
                    break;
                }
                case 2:
                {
                    CreateKnot2(MyDrawMode);
                    break;
                }
                case 3:
                {
                    CreateKnot3(MyDrawMode);
                    break;
                }
                case 4:
                {
                    CreateKnot4(MyDrawMode);
                    break;
                }
                case 5:
                {
                    CreateKnot5(MyDrawMode);
                    break;
                }
                case 6:
                {
                    CreateKnot6(MyDrawMode);
                    break;
                }
                case 7:
                {
                    CreateKnot7(MyDrawMode);
                    break;
                }
                case 8:
                {
                    CreateKnot8(MyDrawMode);
                    break;
                }
            }
        }

        public void SetRotation(double speed)
        {
            rotationSpeed = speed;
            myKnot.RotationSpeed = rotationSpeed;
        }

        public void SetShowTexture(bool show)
        {
            ShowTexture = show;
            GetParameters();
        }

        public void SetDrawMode(int mode)
        {
            switch(mode)
            {
                case 0:
                {
                    MyDrawMode = DrawMode.Points;
                    break;
                }
                case 1:
                {
                    MyDrawMode = DrawMode.Lines;
                    break;
                }
                case 2:
                {
                    MyDrawMode = DrawMode.Fill;
                    break;
                }
            }
        }

        private void SetKnotColor(bool useTex)
        {
            if (useTex )
            {
                myKnot.DiffuseMaterial = Color.FromRgb(0, 0, 0);
                myKnot.AmbientMaterial = Color.FromRgb(0, 0, 0);
                myKnot.SpecularMaterial = Color.FromRgb(150, 150, 150);
                myKnot.Shininess = 20.0;
                myKnot.UseTexture = true;
                myKnot.TextureFile = Environment.CurrentDirectory + "\\Rope.jpg";
            }
            else
            {
                myKnot.DiffuseMaterial = Color.FromRgb(0, 200, 150);
                myKnot.AmbientMaterial = Color.FromRgb(0, 200, 150);
                myKnot.SpecularMaterial = Color.FromRgb(250, 250, 250);
                myKnot.Shininess = 50.0;
                myKnot.UseTexture = false;
            }
        }

        private void CreateKnot1(DrawMode dm )
        {
            myScene.ClearGeometries();
            myKnot = new Knot1Geometry(5, 480, 12, 32)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = rotationSpeed,
                DrawMode = dm,
                TextureScaleX = 1,
                TextureScaleY = 20
            };
            ((Knot1Geometry)myKnot).SetParameters(settingForm);
            SetKnotColor(ShowTexture);
            myScene.AddGeometry(myKnot);
            Title = "3D Knot #1";
        }

        private void CreateKnot2(DrawMode dm)
        {
            myScene.ClearGeometries();
            myKnot = new Knot2Geometry(32, 240, 12, 32)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = rotationSpeed,
                DrawMode = dm,
                TextureScaleX = 1,
                TextureScaleY = 30
            };
            ((Knot2Geometry)myKnot).SetParameters(settingForm);
            SetKnotColor(ShowTexture);
            myScene.AddGeometry(myKnot);
            Title = "3D Knot #2";
        }

        private void CreateKnot3(DrawMode dm)
        {
            myScene.ClearGeometries();
            myKnot = new Knot3Geometry(70, 480, 8, 32)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = rotationSpeed,
                DrawMode = dm,
                TextureScaleX = 1,
                TextureScaleY = 50
            };
            ((Knot3Geometry)myKnot).SetParameters(settingForm);
            SetKnotColor(ShowTexture);
            myScene.AddGeometry(myKnot);
            Title = "3D Knot #3";
        }

        private void CreateKnot4(DrawMode dm)
        {
            myScene.ClearGeometries();
            myKnot = new Knot4Geometry(45, 720, 6, 32)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = rotationSpeed,
                DrawMode = dm,
                TextureScaleX = 1,
                TextureScaleY = 70
            };
            ((Knot4Geometry)myKnot).SetParameters(settingForm);
            SetKnotColor(ShowTexture);
            myScene.AddGeometry(myKnot);
            Title = "3D Knot #4";
        }

        private void CreateKnot5(DrawMode dm)
        {
            myScene.ClearGeometries();
            myKnot = new Knot5Geometry(120, 480, 8, 32)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = rotationSpeed,
                DrawMode = dm,
                TextureScaleX = 1,
                TextureScaleY = 30
            };
            ((Knot5Geometry)myKnot).SetParameters(settingForm);
            SetKnotColor(ShowTexture);
            myScene.AddGeometry(myKnot);
            Title = "3D Knot #5";
        }

        private void CreateKnot6(DrawMode dm)
        {
            myScene.ClearGeometries();
            myKnot = new Knot6Geometry(30, 480, 8, 32)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = rotationSpeed,
                DrawMode = dm,
                TextureScaleX = 1,
                TextureScaleY = 50
            };
            ((Knot6Geometry)myKnot).SetParameters(settingForm);
            SetKnotColor(ShowTexture);
            myScene.AddGeometry(myKnot);
            Title = "3D Knot #6";
        }

        private void CreateKnot7(DrawMode dm)
        {
            myScene.ClearGeometries();
            myKnot = new Knot7Geometry(100, 120, 0.06, 32)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = rotationSpeed,
                DrawMode = dm,
                TextureScaleX = 1,
                TextureScaleY = 30
            };
            ((Knot7Geometry)myKnot).SetParameters(settingForm);
            SetKnotColor(ShowTexture);
            myScene.AddGeometry(myKnot);
            Title = "3D Knot #7";
        }

        private void CreateKnot8(DrawMode dm)
        {
            myScene.ClearGeometries();
            myKnot = new Knot8Geometry(60, 480, 8, 32)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = rotationSpeed,
                DrawMode = dm,
                TextureScaleX = 1,
                TextureScaleY = 30
            };
            ((Knot8Geometry)myKnot).SetParameters(settingForm);
            SetKnotColor(ShowTexture);
            myScene.AddGeometry(myKnot);
            Title = "3D Knot #8";
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (FrameCounter == 0)
            {
                //Starting timing.
                stopwatch.Start();
            }
            FrameCounter++;
            //Determine frame rate in fps (frames per second).
            var frameRate = (long)(FrameCounter / stopwatch.Elapsed.TotalSeconds);
            if (frameRate > 0)
            {
                //Show the frame rate.
                Title = frameRate.ToString() + " fps.";
            }
            //Update the scene
            foreach (GLGeometry geo in myScene.Geometries)
            {
                geo.Update();
            }
            //Render the scene.
            myScene.Render();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (settingForm != null)
            {
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (settingForm != null)
            {
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
