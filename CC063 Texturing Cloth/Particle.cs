using JG_GL;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Texturing_Cloth
{
    class Particle
    {
        private double my_Mass;
        private Vector3D my_Position;
        private Vector3D my_Velocity;
        private Vector3D my_Acceleration;
        private double my_Damping;
        private bool my_Locked;
        private bool my_Show;
        private EllipsoidGeometry bob;
        private double my_Size;

        public Particle(double mass, Vector3D position, bool show)
        {
            my_Mass = mass;
            my_Position = position;
            my_Velocity = new Vector3D(0, 0, 0);
            my_Acceleration = new Vector3D(0, 0, 0);
            my_Damping = 1.0;
            my_Locked = false;
            my_Size = 1.0;
            my_Show = show;
            if (my_Show)
            {
                bob = new EllipsoidGeometry(my_Size, my_Size, my_Size)
                {
                    AmbientMaterial = Color.FromRgb(255, 255, 255),
                    DiffuseMaterial = Color.FromRgb(255, 255, 255),
                    SpecularMaterial = Color.FromRgb(255, 255, 255),
                    Shininess = 50,
                    Position = my_Position
                };
            }
        }

        public double Mass
        {
            get { return my_Mass; }
            set { my_Mass = value; }
        }

        public Vector3D Position
        {
            get { return my_Position; }
            set 
            { 
                my_Position = value;
                if (my_Show) bob.Position = my_Position;
            }
        }

        public Vector3D Velocity
        {
            get { return my_Velocity; }
            set { my_Velocity = value; }
        }

        public Vector3D Acceleration
        {
            get { return my_Acceleration; }
            set { my_Acceleration = value; }
        }

        public double Damping
        {
            get { return my_Damping; }
            set { my_Damping = value; }
        }

        public bool Locked
        {
            get { return my_Locked; }
            set { my_Locked = value; }
        }

        public double Size
        {
            get { return my_Size; }
            set 
            { 
                my_Size = value;
                if (my_Show)
                {
                    bob.X_Size = my_Size;
                    bob.Y_Size = my_Size;
                    bob.Z_Size = my_Size;
                }
            }
        }

        public void ApplyForce(Vector3D force)
        {
            my_Acceleration += force / my_Mass;
        }

        public void Update()
        {
            if (!my_Locked)
            {
                my_Velocity *= my_Damping;
                my_Velocity += my_Acceleration;
                my_Position += my_Velocity;
                my_Acceleration = new Vector3D(0, 0, 0);
                if(my_Show) bob.Position = my_Position;
            }
            else
            {
                my_Acceleration = new Vector3D(0, 0, 0);
            }
        }
    }
}
