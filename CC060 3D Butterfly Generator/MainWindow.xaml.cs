using JG_GL;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Butterfly_Generator
{
    public partial class MainWindow : Window
    {
        WingGeometry w1, w2;
        private double Angle1, Angle2;
        private double anglestep = 0.03;
        private string ResultFileName = "Butterfly.gif";
        private bool Recording = false;
        private int frameCounter = 0;
        BitmapEncoder MyEncoder = new GifBitmapEncoder();
        private delegate void WaitDelegate(int t);
        private int WaitTime;
        //Camera Start Data
        private Vector3D CamStartPos = new Vector3D(50, 160, 150);
        private Vector3D CamTargetPos = new Vector3D(0.0, 0.0, 15.0);
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
            scene1.Camera.UpdateDirection();
            scene1.ViewDistance = 500;
            if (Recording)
            {
                WaitTime = 100;
            }
            else
            {
                WaitTime = 10;
            }
            Angle1 = 0;
            Angle2 = Math.PI;
            w1 = new WingGeometry(80)
            {
                AmbientMaterial = Color.FromRgb(0, 0, 0),
                DiffuseMaterial = Color.FromRgb(0, 0, 0),
                SpecularMaterial = Color.FromRgb(255, 255, 255),
                Shininess = 50,
                RotationAxis = new Vector3D(0.0, 0.0, 1.0),
                RotationAngle = Angle1,
                DrawMode = DrawMode.Fill,
                PointSize=5.0,
                TextureFile = Environment.CurrentDirectory + "\\Wing.jpg",
                UseTexture = true
            };
            scene1.AddGeometry(w1);
            w2 = new WingGeometry(80)
            {
                AmbientMaterial = Color.FromRgb(0, 0, 0),
                DiffuseMaterial = Color.FromRgb(0, 0, 0),
                SpecularMaterial = Color.FromRgb(255, 255, 255),
                Shininess = 50,
                RotationAxis = new Vector3D(0.0, 0.0, 1.0),
                RotationAngle = Angle2,
                DrawMode = DrawMode.Fill,
                PointSize = 5.0,
                TextureFile = Environment.CurrentDirectory + "\\Wing.jpg",
                UseTexture = true
            };
            scene1.AddGeometry(w2);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            Angle1 -= anglestep;
            Angle2 += anglestep;
            if (Math.Abs(Angle1) > Math.PI / 4)
            {
                anglestep = -1 * anglestep;
            }
            w1.RotationAngle = Angle1 - Math.PI / 4;
            w2.RotationAngle = Angle2 + Math.PI / 4;
            //Render the scene.
            scene1.Render();
            if (Recording)
            {
                //Add the canvas content as a BitmapFrame to the GifBitmapEncoder
                RenderTargetBitmap renderbmp = new RenderTargetBitmap((int)(scene1.ActualWidth), (int)(scene1.ActualHeight), 96, 96, PixelFormats.Default);
                renderbmp.Render(scene1);
                MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp));
                frameCounter++;
            }
            Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
            if (frameCounter > Math.PI / Math.Abs(anglestep))
            {
                if (Recording)
                {
                    MyEncoder.Frames.RemoveAt(0);
                    MyEncoder.Frames.RemoveAt(1);
                    // Create a FileStream to write the image to the file.

                    FileStream sw = new FileStream(ResultFileName, FileMode.Create);
                    if (sw != null)
                    {
                        MyEncoder.Save(sw);
                    }
                    Environment.Exit(0);
                }
            }
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}