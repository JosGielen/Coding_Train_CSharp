using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Flappy_Bird
{
    class Bird
    {
        private double my_X;
        private double my_Y;
        private double my_Speed;
        private double my_Size;
        private bool my_Alive;
        private Ellipse my_ellipse;
        private Canvas my_Canvas;

        public Bird(double X, double Y, double Size, Canvas Can)
        {
            my_X = X;
            my_Y = Y;
            my_Size = Size;
            my_Canvas = Can;
            my_Speed = 0;
            my_Alive = true;
            my_ellipse = new Ellipse()
            {
                Width = my_Size,
                Height = my_Size,
                Fill = Brushes.Red
            };
            my_ellipse.SetValue(Canvas.LeftProperty, my_X - my_Size / 2);
            my_ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2);
        }

        public bool Alive
        {
            get { return my_Alive; }
        }


        public void Draw()
        {
            my_Canvas.Children.Add(my_ellipse);
        }

        public void Update(double downspeed)
        {
            my_Speed += downspeed;
            my_Y += my_Speed;
            if (my_Y > my_Canvas.ActualHeight - my_Size)
            {
                my_Y = my_Canvas.ActualHeight - my_Size;
                my_Alive = false;
            }
            if (my_Y < my_Size)
            {
                my_Y = my_Size;
                my_Alive = false;    //Die when hit the top??;
            }
            my_ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2);
        }

        public void Flap(double upspeed)
        {
            my_Speed -= upspeed;
        }

        public void CheckCollision(Gate g)
        {
            if (my_Y < g.GateTop + my_Size / 2 | my_Y > g.GateBottom - my_Size / 2)
            {
                my_Alive = false;
            }
        }

    }
}
