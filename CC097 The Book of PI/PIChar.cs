using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Book_of_PI
{
    class PIChar
    {
        private Rectangle my_Rect;
        private int my_Index;
        private double my_Size;
        private string my_Digit;
        private Brush my_RectColor;

        public PIChar(int index, double size, string digit, Color brush)
        {
            my_Index = index;
            my_Size = size;
            my_Digit = digit;
            my_RectColor = new SolidColorBrush(brush);
            my_Rect = new Rectangle()
            {
                Width = size,
                Height = size,
                Stroke = my_RectColor,
                Fill = my_RectColor,
                StrokeThickness = 1.0
            };
        }

        public int Nr
        {
            get 
            {
                int result;
                int.TryParse(my_Digit, out result);
                return result;
            }
        }

        public void Show()
        {
            my_Rect.Stroke = my_RectColor;
            my_Rect.Fill = my_RectColor;
        }

        public void Hide()
        {
            my_Rect.Stroke = Brushes.White;
            my_Rect.Fill = Brushes.White;
        }

        public void Draw(Canvas c)
        {
            double cols = c.ActualWidth / my_Size;
            double left = my_Size * (my_Index % cols);
            double top = my_Size * (int)(my_Index / cols);
            my_Rect.SetValue(Canvas.LeftProperty, left);
            my_Rect.SetValue (Canvas.TopProperty, top);
            c.Children.Add(my_Rect);
        }
    }
}
