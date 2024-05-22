using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frogger
{
    internal class Obstacle
    {
        private Rectangle my_Rect = new Rectangle();
        private BitmapImage my_Image;
        private double my_Left;
        private double my_Right;
        private double my_Top;
        private double my_Bottom;
        private double my_Width;
        private double my_Height;
        private double my_Speed;
        private double my_Spacing;
        private double RightEdge;

        public Obstacle(double left, double top, double width, double height, double spacing, double speed, BitmapImage image)
        {
            my_Left = left;
            my_Top = top;
            my_Width = width;
            my_Height = height;
            my_Right = left + width;
            my_Bottom = top + height;
            my_Spacing = spacing;
            my_Image = image;
            my_Speed = speed;
        }

        public bool Collision(Frog f)
        {
            if (my_Left > f.Right) return false;
            if (my_Right < f.Left) return false;
            return true;
        }

        public bool CenterCollision(Frog f) //Used for BUSHES Lane
        {
            double center = (my_Left + my_Right) / 2;
            if (center > f.Right) return false;
            if (center < f.Left) return false;
            return true;
        }

        public void Draw(Canvas canv) 
        {
            RightEdge = canv.ActualWidth;
            my_Rect = new Rectangle()
            {
                Width = my_Width,
                Height = my_Height,
                Fill = new ImageBrush(my_Image)
            };
            my_Rect.SetValue(Canvas.LeftProperty, my_Left);
            my_Rect.SetValue(Canvas.TopProperty, my_Top);
            canv.Children.Add(my_Rect);
        }

        public void Update()
        {
            my_Left += my_Speed;
            my_Right += my_Speed;
            if (my_Speed > 0 && my_Left >= RightEdge + my_Spacing) 
            { 
                my_Left = -1.0 * my_Width;
                my_Right = my_Left + my_Width;
            }
            if (my_Speed < 0 && my_Right <= -1 * my_Spacing) 
            { 
                my_Left = RightEdge;
                my_Right = my_Left + my_Width;
            }
            my_Rect.SetValue(Canvas.LeftProperty, my_Left);
        }
    }
}
