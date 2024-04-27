using System.Windows.Media.Media3D;

namespace Texturing_Cloth
{
    class Spring
    {
        private double my_Stiffness;
        private double my_Damping;
        private double my_Length;
        private double my_Extension;
        private Particle my_End1;
        private Particle my_End2;

        public Spring(Particle p1, Particle p2)
        {
            my_Stiffness = 0.01;
            my_Damping = 0.0;
            my_Length = (p2.Position - p1.Position).Length;
            my_Extension = 0;
            my_End1 = p1;
            my_End2 = p2;
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

        public void SetEnd1Position(Vector3D p)
        {
            my_End1.Position = p;
        }

        public void SetEnd2Position(Vector3D p)
        {
            my_End2.Position = p;
        }

        public void MoveEnd1(Vector3D v)
        {
            my_End1.Position += v;
        }

        public void MoveEnd2(Vector3D v)
        {
            my_End2.Position += v;
        }

        public void Update(Vector3D gravity)
        {
            Vector3D v = my_End2.Position - my_End1.Position;
            my_Extension = v.Length - my_Length; 
            v.Normalize();
            Vector3D force = -1 * my_Stiffness * my_Extension * v;
            my_End1.ApplyForce(-1 * force + gravity);
            my_End2.ApplyForce(force + gravity);
            my_End1.Update();
            my_End2.Update();
        }
    }
}
