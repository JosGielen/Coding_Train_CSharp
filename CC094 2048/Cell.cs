using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2048
{
    class Cell
    {
        private bool My_Visible;
        private int my_Value;
        private Brush my_Color;
        private Point my_TopLeft;
        private readonly Rectangle my_Rect;
        private readonly Label my_Text;

        public Cell(double width, double height)
        {
            My_Visible = false;
            my_Rect = new Rectangle()
            {
                Width = width,
                Height = height,
                RadiusX = 40,
                RadiusY = 40,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            my_Text = new Label()
            {
                Width = 0.9 * width,
                Height = 0.8 * height,
                Background = Brushes.Transparent,
                FontSize = 52,
                FontWeight = FontWeights.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0.0),
                Content = my_Value.ToString()
            };
        }

        public int CellValue
        {
            get { return my_Value; }
            set 
            { 
                my_Value = value;
                my_Text.Content = value.ToString();
            }
        }

        public Brush BackColor
        {
            get { return my_Color; }
            set
            {
                my_Color = value;
                my_Rect.Fill = value;
            }
        }

        public double TextSize
        {
            get { return my_Text.FontSize; }
            set { my_Text.FontSize = value; }
        }

        public double Top
        {
            get { return my_TopLeft.Y; }
            set
            {
                my_TopLeft.Y = value;
                my_Rect.SetValue(Canvas.TopProperty, value);
                my_Text.SetValue(Canvas.TopProperty, value + 0.1 * my_Rect.Height);
            }
        }

        public double Left
        {
            get { return my_TopLeft.X; }
            set
            {
                my_TopLeft.X = value;
                my_Rect.SetValue(Canvas.LeftProperty, value);
                my_Text.SetValue(Canvas.LeftProperty, value + 0.05 * my_Rect.Width);
            }
        }

        public bool Visible
        {
            get { return My_Visible; }
            set { My_Visible = value; }
        }

        public void SetFocus(bool focus)
        {
            if (focus)
            {
                my_Rect.Stroke = Brushes.Red;
                my_Rect.StrokeThickness = 3.0;
            }
            else
            {
                my_Rect.Stroke = Brushes.Black;
                my_Rect.StrokeThickness = 1.0;
            }
        }

        public void Draw(Canvas c)
        {
            if (My_Visible )
            {
                if (!c.Children.Contains(my_Rect )) { c.Children.Add(my_Rect); }
                if (!c.Children.Contains(my_Text )) { c.Children.Add(my_Text); }
            }
            else
            {
                if (c.Children.Contains(my_Rect)) { c.Children.Remove(my_Rect); }
                if (c.Children.Contains(my_Text)) { c.Children.Remove(my_Text); }
            }
        }
    }
}
