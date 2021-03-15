using System.Windows;
using System.Windows.Media;

namespace Fireworks
{
    public class Particle
    {
        private Vector my_Position;
        private Vector my_Velocity;
        private Color my_Color;
        private int my_LifeTime;

        public Particle(Vector position, Color color, int lifetime)
        {
            my_Position = position;
            my_Color = color;
            my_LifeTime = lifetime;
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

        public int LifeTime
        {
            get { return my_LifeTime; }
            set { my_LifeTime = value; }
        }

        public Color Color
        {
            get { return my_Color; }
        }

        public void Update(Vector gravity)
        {
            my_Velocity += gravity;
            my_Velocity *= 0.95;
            my_Position += my_Velocity;
            my_LifeTime -= 1;
        }
    }
}
