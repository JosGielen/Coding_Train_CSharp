using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Plinko
{
    public class PlinkoSlot
    {
        private double my_Left;
        private double my_Right;
        private int my_Count;
        private double my_Height;
        private double my_Bottom;
        private Rectangle my_Rectangle;

        public PlinkoSlot(double left, double right)
        {
            my_Left = left;
            my_Right = right;
            my_Count = 0;
            my_Height = 3.0;
            my_Rectangle = new Rectangle()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Fill = Brushes.Blue,
                Width = right - left - 3,
                Height = 0
            };
        }

        public double Left
        {
            get { return my_Left; }
            set { my_Left = value; }
        }

        public double Right
        {
            get { return my_Right; }
            set { my_Right = value; }
        }

        public int Count
        {
            get { return my_Count; }
            set { my_Count = value; }
        }

        public void Draw(Canvas c)
        {
            my_Bottom = c.ActualHeight;
            my_Rectangle.SetValue(Canvas.LeftProperty, my_Left + 3);
            my_Rectangle.SetValue(Canvas.TopProperty, my_Bottom - my_Height);
            c.Children.Add(my_Rectangle);
        }

        public void SetHeight(double maxHeight, int maxCount)
        {
            if (maxCount > maxHeight / 3)
            {
                my_Height = my_Count * maxHeight / maxCount;
            }
            else
            {
                my_Height = 3 * my_Count;
            }
            my_Rectangle.Height = my_Height;
            my_Rectangle.SetValue(Canvas.TopProperty, my_Bottom - my_Height);
        }
    }
}
