using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Plinko
{
    public class PlinkoBall
    {
        private double my_Diameter;
        private Vector my_Pos;
        private Vector my_Velocity;
        private Ellipse my_Ellipse;
        private double my_maxX;
        private bool isLocked = false;

        public PlinkoBall(double diameter, Vector position)
        {
            my_Diameter = diameter;
            my_Pos = position;
            my_Ellipse = new Ellipse()
            {
                Width = my_Diameter,
                Height = my_Diameter,
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = Brushes.Green
            };
            my_Ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Diameter / 2);
            my_Ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Diameter / 2);
        }

        public Vector Position
        {
            get { return my_Pos; }
            set { my_Pos = value; }
        }

        public void Lock()
        {
            isLocked = true;
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(my_Ellipse);
            my_maxX = c.ActualWidth;
        }

        public void Remove(Canvas c)
        {
            c.Children.Remove(my_Ellipse);
        }

        public void Update(Vector gravity, double Elasticity, List<PlinkoPin> pins)
        {
            double dist = 0.0;
            my_Velocity = my_Velocity + gravity;
            if (isLocked) my_Velocity.X = 0;
            my_Pos = my_Pos + my_Velocity;
            for (int I = 0; I < pins.Count; I++)
            {
                dist = Math.Sqrt((pins[I].Position.X - my_Pos.X) * (pins[I].Position.X - my_Pos.X) + (pins[I].Position.Y - my_Pos.Y) * (pins[I].Position.Y - my_Pos.Y));
                if (dist < (pins[I].Diameter + my_Diameter) / 2)
                {
                    Vector norm = my_Pos - pins[I].Position;
                    norm.Normalize();
                    my_Velocity = Elasticity / 100 * my_Velocity.Length * norm;
                    my_Pos = pins[I].Position + ((pins[I].Diameter + my_Diameter) / 2 + 1) * norm;
                }
                if (my_Pos.X <= my_Diameter | my_Pos.X >= my_maxX) my_Velocity.X = -1 * my_Velocity.X;
            }
            my_Ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Diameter / 2);
            my_Ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Diameter / 2);
        }
    }
}