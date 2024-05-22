using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frogger
{
    internal class Frog
    {
        private Rectangle my_Rect;
        private BitmapImage my_Image;
        private double my_Left;
        private double my_Right;
        private double my_Top;
        private double my_Bottom;
        private double my_Width;
        private double my_Height;
        private double RightEdge;
        public int LaneIndex { get; set; }
        public bool OnLog {  get; set; }
        public double LogSpeed { get; set; }
        public bool Alive;

        public Frog(double size) 
        { 
            my_Left = 0.0;
            my_Right = 0.0;
            my_Top = 0.0;
            my_Bottom = 0.0;
            my_Width = size;
            my_Height = size;
            my_Image = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Sprites\\Frog.gif"));
            Alive = true;
        }

        public double Left 
        { 
            get { return my_Left; }
            set 
            { 
                my_Left = value; 
                my_Right = my_Left + my_Width;
            }
        }

        public double Top 
        { 
            get { return my_Top; }
            set 
            { 
                my_Top = value; 
                my_Bottom = my_Top + my_Height;
            }
        }

        public double Right 
        { 
            get { return my_Right; }
            set 
            { 
                my_Right = value; 
                my_Left = my_Right - my_Width;
            }
        }

        public double Bottom 
        { 
            get { return my_Bottom; }
            set 
            { 
                my_Bottom = value;
                my_Top = my_Bottom - my_Height;
            }
        }

        public double Width
        {
            get { return my_Width; }
            set 
            { 
                my_Width = value;
                my_Right = my_Left + my_Width;
            }
        }

        public double Height
        {
            get { return my_Height; }
            set 
            { 
                my_Height = value;
                my_Bottom = my_Top + my_Height;
            }
        }

        public void SetImage(BitmapImage image)
        {
            my_Image = image;
            my_Rect.Fill = new ImageBrush(my_Image);
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

        public void Erase(Canvas canv)
        {
            if (canv.Children.Contains(my_Rect)) { canv.Children.Remove(my_Rect);}
        }

        public void Update()
        {
            if (OnLog)
            {
                my_Left += LogSpeed;
                my_Right += LogSpeed;
            }
            if (my_Left < 0) 
            { 
                my_Left = 0;
                my_Right = my_Width;
            }
            if (my_Right > RightEdge) 
            { 
                my_Left = RightEdge - my_Width;
                my_Right = RightEdge; 
            }
            my_Rect.SetValue(Canvas.LeftProperty, my_Left);
            my_Rect.SetValue(Canvas.TopProperty, my_Top);
        }
    }
}
