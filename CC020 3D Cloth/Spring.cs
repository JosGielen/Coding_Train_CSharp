using JG_GL;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3D_Cloth
{
    class Spring
    {
        private double my_Stiffness;
        private double my_Damping;
        private double my_Length;
        private double my_Extension;
        private Particle my_End1;
        private Particle my_End2;
        private GLScene my_Scene;
        private JG_GL.LineGeometry my_Line;

        public Spring(Particle p1, Particle p2)
        {
            my_Stiffness = 0.01;
            my_Damping = 0.0;
            my_Length = (p2.Position - p1.Position).Length;
            my_Extension = 0;
            my_End1 = p1;
            my_End2 = p2;
            my_Line = new JG_GL.LineGeometry()
            {
                StartPt = new Vector3D(p1.Position.X, p1.Position.Y, p1.Position.Z),
                EndPt = new Vector3D(p2.Position.X, p2.Position.Y, p2.Position.Z),
                AmbientMaterial = Color.FromRgb(255, 255, 255),
                DiffuseMaterial = Color.FromRgb(255, 255, 255),
                SpecularMaterial = Color.FromRgb(255, 255, 255),
                Shininess = 50,
            };
        }

        public Particle End1
        {
            get { return my_End1; }
            set { my_End1 = value; }
        }

        public Particle End2
        {
            get { return my_End2; }
            set { my_End2 = value; }
        }

        public double Stiffness
        {
            get { return my_Stiffness; }
            set { my_Stiffness = value; }
        }

        public double Damping
        {
            get { return my_Damping; }
            set 
            { 
                my_Damping = value;
                my_End1.Damping = my_Damping;
                my_End2.Damping = my_Damping;
            }
        }

        public double RestLength
        {
            get { return my_Length; }
            set 
            {
                my_Length = value;
                my_Extension = (my_End2.Position - my_End1.Position).Length - my_Length; 
            }
        }

        public double Extension
        {
            get { return my_Extension; }
        }

        public void SetEnd1Position(Point3D p)
        {
            my_End1.Position = p;
            my_Line.StartPt = new Vector3D(my_End1.Position.X, my_End1.Position.Y, my_End1.Position.Z);
            Vector3D v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length;
        }

        public void SetEnd2Position(Point3D p)
        {
            my_End2.Position = p;
            my_Line.EndPt = new Vector3D(my_End2.Position.X, my_End2.Position.Y, my_End2.Position.Z);
            Vector3D v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length;
        }

        public void Update(Vector3D gravity)
        {
            Vector3D v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length; 
            v.Normalize();
            Vector3D force = -1 * my_Stiffness * my_Extension * v;
            my_End1.ApplyForce(-1 * force + gravity);
            my_End2.ApplyForce(force + gravity);
        }

        public void Draw (GLScene scene, bool showParticles)
        {
            scene.AddGeometry(my_Line);
            if (showParticles)
            {
                my_End1.Draw(scene);
                my_End2.Draw(scene);
            }
            my_Scene = scene;
        }

        public void Redraw()
        {
            my_End1.Update();
            my_End2.Update();
            my_Line.StartPt = new Vector3D(my_End1.Position.X, my_End1.Position.Y, my_End1.Position.Z);
            my_Line.EndPt = new Vector3D(my_End2.Position.X, my_End2.Position.Y, my_End2.Position.Z);
            my_Line.GenerateGeometry(my_Scene);
        }
    }
}
