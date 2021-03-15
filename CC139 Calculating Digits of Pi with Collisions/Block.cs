using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CollisionPi
{
    class Block
    {
        private double my_X;
        private double my_Size;
        private double my_Speed;  //Positive is to the left 
        private double my_Mass;
        private Rectangle my_Rect;
        private Brush myColor;

        public Block(double x, double size, double speed, double mass, Brush color)
        {
            my_X = x;
            my_Size = size;
            my_Speed = speed;
            my_Mass = mass;
            my_Rect = new Rectangle()
            {
                Width = my_Size,
                Height = my_Size,
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = color,
            };
        }

        public double X
        {
            get { return my_X; }
            set { my_X = value; }
        }

        public double Size
        {
            get { return my_Size; }
            set { my_Size = value; }
        }

        public double Speed
        {
            get { return my_Speed; }
            set { my_Speed = value; }
        }

        public double Mass
        {
            get { return my_Mass; }
            set { my_Mass = value; }
        }

        public Brush Color
        {
            get { return myColor; }
            set { myColor = value; }
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(my_Rect);
            my_Rect.SetValue(Canvas.LeftProperty, my_X);
            my_Rect.SetValue(Canvas.TopProperty, c.ActualHeight - my_Size);
        }

        public void Update()
        {
            my_Rect.SetValue(Canvas.LeftProperty, my_X);
        }

    }
}
