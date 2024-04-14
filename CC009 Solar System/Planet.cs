using JG_GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shell;

namespace Solar_System
{
    internal class Planet
    {
        private Vector3D my_OrbitCenter;
        private double my_Diameter;
        private double my_RotationTime;
        private double my_OrbitTime;
        private double my_Distance;
        private string my_TextureFile;
        private EllipsoidGeometry my_Shape;
        private double my_RotationAngle;
        private double RotationAngleStep;
        private bool my_CanOrbit;
        private double my_OrbitAngle;
        private double OrbitAngleStep;
        private List<Planet> Moons;

        public Planet(Vector3D Orbitcenter, double diameter) 
        {
            my_OrbitCenter = Orbitcenter;
            my_Diameter = diameter;
            my_Shape = new EllipsoidGeometry(diameter, diameter, diameter, 32, 32)
            {
                AmbientMaterial = Color.FromRgb(0, 0, 0),
                DiffuseMaterial = Color.FromRgb(0, 0, 0),
                SpecularMaterial = Color.FromRgb(0, 0, 0),
                Shininess = 50,
                DrawMode = DrawMode.Fill,
                RotationAxis = new Vector3D(0, 1, 0)
            };
            my_OrbitAngle = 0.0;
            my_RotationAngle = 0.0;
            my_CanOrbit = true;
            Moons = new List<Planet>();
        }

        public double Diameter
        {
            get {  return my_Diameter; }
            set 
            { 
                my_Diameter = value;
                my_Shape.X_Size = my_Diameter;
                my_Shape.Y_Size = my_Diameter;
                my_Shape.Z_Size = my_Diameter;
            }
        }

        /// <summary>
        /// minutes for 1 complete rotation
        /// </summary>
        public double RotationTime
        {
            get { return my_RotationTime; }
            set 
            { 
                my_RotationTime = value;
                if (Math.Abs(my_RotationTime) > 0 )
                {
                    RotationAngleStep = 0.1 / my_RotationTime; //0.1 = 360° / 3600 frames per minute.
                }
                else
                {
                    RotationAngleStep = 0.0;
                }
            }
        }

        /// <summary>
        /// minutes for 1 complete orbit
        /// </summary>
        public double OrbitTime
        {
            get { return my_OrbitTime; }
            set 
            { 
                my_OrbitTime = value;
                if (Math.Abs(my_OrbitTime) > 0 )
                {
                    OrbitAngleStep = 0.1 / my_OrbitTime; //0.1 = 360° / 3600 frames per minute.
                }
                else
                {
                    OrbitAngleStep = 0.0;
                }
            }
        }

        /// <summary>
        /// Use to stop the planet in its orbit so user can look at it in detail.
        /// </summary>
        public bool CanOrbit
        {
            get { return my_CanOrbit; }
            set { my_CanOrbit = value; }
        }

        /// <summary>
        /// Distance from the orbit center to the planet center
        /// </summary>
        public double Distance
        {
            get { return my_Distance; }
            set 
            { 
                my_Distance = value;
                my_Shape.Position = my_OrbitCenter + new Vector3D(my_Distance * Math.Cos(my_OrbitAngle), 0, my_Distance * Math.Sin(my_OrbitAngle));
            }
        }

        public string TextureFile
        {
            get { return my_TextureFile; }
            set 
            {
                my_TextureFile = value;
                my_Shape.TextureFile = my_TextureFile;
                my_Shape.UseTexture = true;
            }
        }

        public double RotationAngle
        {
            get { return my_RotationAngle; }
            set 
            { 
                my_RotationAngle = value;
                my_Shape.RotationAngle = my_RotationAngle;
            }
        }

        public double OrbitAngle
        {
            get { return my_OrbitAngle; }
            set 
            { 
                my_OrbitAngle = value;
                my_Shape.Position = my_OrbitCenter + new Vector3D(my_Distance * Math.Cos(my_OrbitAngle), 0, my_Distance * Math.Sin(my_OrbitAngle));
            }
        }

        public Vector3D Position
        {
            get { return my_Shape.Position; }
        }

        public Vector3D OrbitCenter
        {
            get { return my_OrbitCenter; }
            set { my_OrbitCenter = value;}
        }

        public void AddMoon(Planet m)
        {
            Moons.Add(m);
        }

        public void Draw(GLScene scene)
        {
            scene.AddGeometry(my_Shape);
        }

        public void Update()
        {
            my_RotationAngle -= RotationAngleStep;
            if (Math.Abs(my_RotationAngle) >= 360) my_RotationAngle = 0.0;
            my_Shape.RotationAngle = my_RotationAngle;
            if (my_CanOrbit)
            {
                my_OrbitAngle -= OrbitAngleStep;
                if (Math.Abs(my_OrbitAngle) >= 360) my_OrbitAngle = 0.0;
                my_Shape.Position = my_OrbitCenter + new Vector3D(my_Distance * Math.Cos(my_OrbitAngle), 0, my_Distance * Math.Sin(my_OrbitAngle));
            }
            for (int i = 0; i < Moons.Count; i++)
            {
                Moons[i].OrbitCenter = my_Shape.Position; 
                Moons[i].Update();
            }
        }

        public void AdjustRotationTime(double factor)
        {
            my_RotationTime /= factor;
            RotationAngleStep = 0.1 / my_RotationTime; //0.1 = 360° / 3600 frames per minute.
            for (int i = 0; i < Moons.Count; i++)
            {
                Moons[i].RotationTime /= factor;
            }
        }

        public void AdjustOrbitTime(double factor)
        {
            my_OrbitTime /= factor;
            OrbitAngleStep = 0.1 / my_OrbitTime; //0.1 = 360° / 3600 frames per minute.
            for (int i = 0; i < Moons.Count; i++)
            {
                Moons[i].OrbitTime /= factor;
            }
        }
    }
}
