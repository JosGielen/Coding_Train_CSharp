using JG_GL;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Menger_Sponge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool App_Loaded = false;
        private BoxGeometry box;
        private List<BoxGeometry> Sponge;
        //Camera positioning
        private Vector3D CamStartPos = new Vector3D(40.0, 20.0, 40.0);
        private Vector3D CamViewDir = new Vector3D(-40.0, -20.0, -40.0);
        private Vector3D CamUpDir = new Vector3D(0.0, 1.0, 0.0);
        //Camera Mouse control
        private bool MousebuttonDown = false;
        private double MouseSensitivity = 0.5;
        private Point MouseStartPos;
        private Point MouseEndPos;
        //FPS data
        private bool ShowFPS = false;
        private DateTime LastRenderTime;
        private int Framecounter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Set up some lights
            myScene.Lights.Clear();
            //Pointlight to illuminate the center of the Menger Sponge
            GLLight l = new GLLight(LightType.PointLight)
            {
                Ambient = Colors.LightGray,
                Diffuse = Colors.LightGray,
                Specular = Colors.White,
                Position = new Vector3D(0, 0, 0),
                Constant = 1,
                Linear = 0.0,
                Quadratic = 0.0
            };
            myScene.AddLight(l);
            //Directional light to illuminate the outside of the Menger Sponge
            l = new GLLight(LightType.DirectionalLight)
            {
                Ambient = Colors.LightGray,
                Diffuse = Colors.White,
                Specular = Colors.White,
                Direction = new Vector3D(1, 2, -2),
                Constant = 0.1,
                Linear = 0.0,
                Quadratic = 0.0
            };
            myScene.AddLight(l);
            //Add a free flying camera so you can enter the sponge and look around
            myScene.Camera = new FreeFlyingCamera(CamStartPos, CamViewDir, CamUpDir)
            {
                MoveSpeed = 0.5
            };

            //Do the program initialisation: Start with 1 Cube
            Sponge = new List<BoxGeometry>();
            box = new BoxGeometry(10.0, 10.0, 10.0)
            {
                AmbientMaterial = Color.FromRgb(100, 90, 25),
                DiffuseMaterial = Color.FromRgb(200, 180, 50),
                SpecularMaterial = Color.FromRgb(255, 255, 255),
                Shininess = 50,
                Position = new Vector3D(0.0, 0.0, 0.0),
                DrawMode = DrawMode.Fill
            };
            Sponge.Add(box);
            myScene.AddGeometry(box);
            //Initialize FPS counter
            LastRenderTime = DateTime.Now;
            Framecounter = 0;
            App_Loaded = true;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!App_Loaded) return;
            if (ShowFPS)
            {
                Framecounter += 1;
                if (Framecounter == 25)
                {
                    double fps = (int)(25000 / (DateTime.Now - LastRenderTime).TotalMilliseconds);
                    Title = "FPS = " + fps.ToString();
                    LastRenderTime = DateTime.Now;
                    Framecounter = 0;
                }
            }
            else
            {
                Title = "VertexCount = " + (myScene.Geometries.Count * 24).ToString();
            }
            //Update the scene
            foreach (GLGeometry geo in myScene.Geometries)
            {
                geo.Update();
            }
            //Render the scene.
            myScene.Render();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseStartPos = e.GetPosition(myScene);
            MousebuttonDown = true;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //Camera View direction change
            if (MousebuttonDown)
            {
                MouseEndPos = e.GetPosition(myScene);
                Vector3D dummyPos = myScene.Camera.Position;
                ((FreeFlyingCamera)myScene.Camera).Horizontal(MouseSensitivity * (MouseStartPos - MouseEndPos).X);
                ((FreeFlyingCamera)myScene.Camera).Vertical(MouseSensitivity * (MouseStartPos - MouseEndPos).Y);
                MouseStartPos = MouseEndPos;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MousebuttonDown = false;
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Mouse wheel used for Camera Forward/Backward movement.
            double amount = 0.1 * MouseSensitivity * e.Delta;
            ((FreeFlyingCamera)myScene.Camera).Forward(amount);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Keyboard movement through the Scene
            switch (e.Key)
            {
                case Key.Up: //Camera look up
                    {
                        ((FreeFlyingCamera)myScene.Camera).Forward(1.0);
                        break;
                    }
                case Key.Down: //Camera look down
                    {
                        ((FreeFlyingCamera)myScene.Camera).Forward(-1.0);
                        break;
                    }
                case Key.Right: //Camera look to the right
                    {
                        ((FreeFlyingCamera)myScene.Camera).Horizontal(-1.0);
                        break;
                    }
                case Key.Left: //Camera look to the left
                    {
                        ((FreeFlyingCamera)myScene.Camera).Horizontal(1.0);
                        break;
                    }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        //Reset the camera and target positions.
                        ((FreeFlyingCamera)myScene.Camera).Position = CamStartPos;
                        ((FreeFlyingCamera)myScene.Camera).TargetPosition = new Vector3D(0.0, 0.0, 0.0);
                        break;
                    }
                case Key.Space:
                    {
                        //Add another level of detail to the sponge
                        List<BoxGeometry> newSponge = new List<BoxGeometry>();
                        for (int I = 0; I < Sponge.Count; I++)
                        {
                            newSponge.AddRange(Devide(Sponge[I]));
                        }
                        Sponge = newSponge;
                        myScene.ClearGeometries();
                        foreach (BoxGeometry b in Sponge)
                        {
                            myScene.AddGeometry(b);
                        }
                        break;
                    }
            }
        }

        private List<BoxGeometry> Devide(BoxGeometry b)
        {
            List<BoxGeometry> newboxes = new List<BoxGeometry>();
            //Create 8 new BoxGeometries
            BoxGeometry box;
            double newSize = b.X_Size / 3;
            double centerX;
            double centerY;
            double centerZ;
            int sum;
            for (int I = -1; I <= 1; I++)
            {
                for (int J = -1; J <= 1; J++)
                {
                    for (int K = -1; K <= 1; K++)
                    {
                        sum = Math.Abs(I) + Math.Abs(J) + Math.Abs(K);
                        if (sum > 1)
                        {
                            centerX = b.Position.X + I * newSize;
                            centerY = b.Position.Y + J * newSize;
                            centerZ = b.Position.Z + K * newSize;

                            box = new BoxGeometry(newSize, newSize, newSize)
                            {
                                AmbientMaterial = Color.FromRgb(100, 90, 25),
                                DiffuseMaterial = Color.FromRgb(200, 180, 50),
                                SpecularMaterial = Color.FromRgb(255, 255, 255),
                                Shininess = 50,
                                DrawMode = DrawMode.Fill,
                                Position = new Vector3D(centerX, centerY, centerZ)
                            };
                            newboxes.Add(box);
                        }
                    }
                }
            }
            return newboxes;
        }
    }
}
