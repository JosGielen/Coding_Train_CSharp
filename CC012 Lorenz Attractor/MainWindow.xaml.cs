using System;
using System.Collections.Generic;
using JG_GL;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Lorenz_Attractor
{
    public partial class MainWindow : Window
    {
        private bool App_Loaded = false;
        private PolyLineGeometry my_Polyline;
        //Lorenz Attractor data
        private double Sigma = 10.0;
        private double Rho = 28.0;
        private double Beta = 8.0 / 3.0;
        private double X = -3.0;
        private double Y = 1.0;
        private double Z = 20.0;
        private double dX = 0.0;
        private double dY = 0.0;
        private double dZ = 0.0;
        private double dt = 0.01;
        private List<Vector3D> points = new List<Vector3D>();
        //Camera positioning
        private Vector3D CamStartPos = new Vector3D(0.0, 20.0, 80.0);
        private Vector3D CamStartTarget = new Vector3D(0.0, 0.0, 0.0);
        private Vector3D CamUpDir = new Vector3D(0.0, 1.0, 0.0);
        //FPS data
        private DateTime LastRenderTime;
        private int Framecounter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            my_Polyline = new PolyLineGeometry(3.0, 3.0, 3.0, false)
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                DrawMode = DrawMode.Lines,
                PointSize = 5.0,
                LineWidth = 5.0,
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = 0.002
            };
            Scene1.ShowAxes = false;
            Scene1.ShowGrid = false;
            Scene1.Camera.Position = CamStartPos;
            Scene1.Camera.TargetPosition = CamStartTarget;
            Scene1.Camera.UpDirection = CamUpDir;
            for (int I = 0; I <= 4; I++)
            {
                GetLorenzPoint();
            }
            my_Polyline.Points = points;
            my_Polyline.SetVertexColors(Environment.CurrentDirectory + "\\Rainbow continuous.cpl", 512);
            Scene1.AddGeometry(my_Polyline);
            //Initialize FPS counter
            LastRenderTime = DateTime.Now;
            Framecounter = 0;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            App_Loaded = true;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!App_Loaded) return;
            //Show FPS
            Framecounter += 1;
            if (Framecounter == 25)
            {
                double fps = (int)(25000 / (DateTime.Now - LastRenderTime).TotalMilliseconds);
                Title = "FPS = " + fps.ToString();
                LastRenderTime = DateTime.Now;
                Framecounter = 0;
            }
            //Create the Lorenz Attractor
            if (points.Count < 15000)
            {
                for (int I = 0; I <= 4; I++)
                {
                    GetLorenzPoint();
                }
                my_Polyline.Points = points;
                my_Polyline.GenerateGeometry(Scene1);
            }
            my_Polyline.Update();
            Scene1.Render();
        }

        private void GetLorenzPoint()
        {
            dX = Sigma * (Y - X) * dt;
            dY = (X * (Rho - Z) - Y) * dt;
            dZ = (X * Y - Beta * Z) * dt;
            X += dX;
            Y += dY;
            Z += dZ;
            points.Add(new Vector3D(X + 3, Y, Z - 30));
        }
    }
}
