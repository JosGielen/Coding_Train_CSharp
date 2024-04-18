using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Asteroids
{
    internal class Ship
    {
        private Vector my_Position;
        private Vector my_Velocity;
        private Vector my_Acceleration;
        private double my_Angle;
        private double my_MaxSpeed;
        private int my_Life;
        private Ellipse[] my_Shield;
        private double ShieldDia;
        private Image ImgShip;
        private BitmapImage bitmap;
        public double ShipWidth { get; set; }
        public double ShipHeight { get; set; }

        public Ship(Vector pos, int life, string imageFile)
        {
            my_Position = pos;
            my_Velocity = new Vector(0.0, 0.0);
            my_MaxSpeed = 4.0;
            my_Life = life;
            my_Shield = new Ellipse[my_Life];
            bitmap = new BitmapImage(new Uri(imageFile));
            ImgShip = new Image();
            ImgShip.Source = bitmap;
            ShipWidth = bitmap.Width;
            ShipHeight = bitmap.Height;
            //Give the ship a shield
            my_Shield = new Ellipse[10];
            for (int i = 0; i < my_Life; i++)
            {
                my_Shield[i] = new Ellipse()
                {
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 2.0
                };
            }
        }

        public Vector Position
        {
            get { return my_Position; }
            set { my_Position = value; }
        }

        public Vector Direction
        {
            get
            {
                double rads = Math.PI * (my_Angle - 90) / 180;
                return new Vector(Math.Cos(rads), Math.Sin(rads));
            }
        }

        public Ellipse[] Shield
        {
            get { return my_Shield; }
        }

        public void Draw(Canvas c)
        {
            my_Angle = 0;
            ImgShip.SetValue(Canvas.LeftProperty, my_Position.X - (ShipWidth / 2));
            ImgShip.SetValue(Canvas.TopProperty, my_Position.Y - (ShipHeight / 2));
            c.Children.Add(ImgShip);

            if (ShipWidth > ShipHeight)
            {
                ShieldDia = ShipWidth + 8.0;
            }
            else
            {
                ShieldDia = ShipHeight + 8.0;
            }
            for (int i = 0; i < 10; i++)
            {
                my_Shield[i].Width = ShieldDia + 2 * i;
                my_Shield[i].Height = ShieldDia + 2 * i;
                my_Shield[i].SetValue(Canvas.LeftProperty, my_Position.X - (ShieldDia / 2 + i));
                my_Shield[i].SetValue(Canvas.TopProperty, my_Position.Y - (ShieldDia / 2 + i));
                c.Children.Add(my_Shield[i]);
            }
        }

        public void Accelerate(double power)
        {
            double rads = Math.PI * (my_Angle - 90) / 180;
            my_Acceleration = power * new Vector(Math.Cos(rads), Math.Sin(rads));
        }

        public void Rotate(double angleStep)
        {
            my_Angle += angleStep;
        }

        public void Update()
        {
            if (my_Acceleration.Length > 0)
            {
                my_Velocity += my_Acceleration;
            }
            else
            {
                my_Velocity = 0.98 * my_Velocity; //Automatic slow down
            }
            if (my_Velocity.Length > my_MaxSpeed)
            {
                my_Velocity.Normalize();
                my_Velocity *= my_MaxSpeed;
            }
            my_Position += my_Velocity;
            my_Acceleration = new Vector(0, 0);
            ImgShip.SetValue(Canvas.LeftProperty, my_Position.X - (ShipWidth / 2));
            ImgShip.SetValue(Canvas.TopProperty, my_Position.Y - (ShipHeight / 2));
            ImgShip.RenderTransform = new RotateTransform(my_Angle, ShipWidth / 2, ShipHeight / 2);
            for (int i = 0; i < 10; i++)
            {
                my_Shield[i].SetValue(Canvas.LeftProperty, my_Position.X - (ShieldDia / 2 + i));
                my_Shield[i].SetValue(Canvas.TopProperty, my_Position.Y - (ShieldDia / 2 + i));
            }
        }

        public void Erase(Canvas c)
        {
            if (c.Children.Contains(ImgShip))
            {
                c.Children.Remove(ImgShip);
            }
            for (int i = 0; i < 10; i++)
            {
                if (c.Children.Contains(my_Shield[i]))
                {
                    c.Children.Remove(my_Shield[i]);
                }
            }
        }
    }
}
