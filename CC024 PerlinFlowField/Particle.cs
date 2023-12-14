using System.Windows;

namespace PerlinFlowField
{
    class Particle
    {
        private Vector my_Pos;
        private Vector my_Vel;
        private Vector my_Acc;
        private double my_MaxSpeed;

        public Particle(Vector position, Vector velocity, double maxSpeed)
        {
            my_Pos = position;
            my_Vel = velocity;
            my_MaxSpeed = maxSpeed;
            my_Acc = new Vector();
        }

        public Vector Position
        {
            get { return my_Pos; }
            set { my_Pos = value; }
        }

        public Vector Velocity
        {
            get { return my_Vel; }
            set { my_Vel = value; }
        }

        public void Update()
        {
            my_Vel = my_Vel + my_Acc;
            my_Vel.Normalize();
            my_Vel = my_MaxSpeed * my_Vel;
            my_Pos = my_Pos + my_Vel;
            my_Acc = new Vector();
        }

        public void ApplyForce(Vector force)
        {
            my_Acc = my_Acc + force;
        }
    }
}
