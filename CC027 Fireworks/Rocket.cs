using System.Windows;

namespace Fireworks
{
    class Rocket
    {
        private Vector my_Position;
        private Vector my_Velocity;
        private bool my_Exploded;

        public Rocket()
        {
            my_Position = new Vector(0,0);
            my_Velocity = new Vector(0, 0);
            my_Exploded = true;
        }

        public Vector Position
        {
            get { return my_Position; }
            set { my_Position = value; }
        }

        public Vector Velocity
        {
            get { return my_Velocity; }
            set { my_Velocity = value; }
        }

        public bool Exploded
        {
            get { return my_Exploded; }
        }

        public void Launch(Vector position, Vector velocity)
        {
            my_Position = position;
            my_Velocity = velocity;
            my_Exploded = false;
        }

        public void Update(Vector gravity)
        {
            my_Velocity += gravity;
            my_Position += my_Velocity;
            if (my_Velocity.Y > 0) my_Exploded = true;
        }
    }
}
