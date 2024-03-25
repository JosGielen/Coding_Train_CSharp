using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RubiksCube
{
    internal class CubeletFace
    {
        public double Size;
        public Rectangle Rect;
        public double Left;
        public double Top;
        private readonly bool Isfixed;
        private Color my_FaceColor;
        private readonly Color[] my_Colors = new Color[6];
        private int my_FaceColorNumber;

        public CubeletFace(int _ColorNumber, double _left, double _top, double _size, bool _fixed)
        {
            my_FaceColorNumber = _ColorNumber;
            Size = _size;
            Left = _left;
            Top = _top;
            Isfixed = _fixed;
            my_Colors[0] = Color.FromRgb(255, 255, 255);
            my_Colors[1] = Color.FromRgb(255, 255, 0);
            my_Colors[2] = Color.FromRgb(0, 255, 0);
            my_Colors[3] = Color.FromRgb(0, 0, 255);
            my_Colors[4] = Color.FromRgb(255, 180, 0);
            my_Colors[5] = Color.FromRgb(255, 0, 0);
            my_FaceColor = my_Colors[_ColorNumber];
            Rect = new Rectangle()
            {
                Width = _size,
                Height = _size,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0,
                Fill = new SolidColorBrush(my_FaceColor)
            };
            Rect.SetValue(Canvas.LeftProperty, _left);
            Rect.SetValue(Canvas.TopProperty, _top);
        }

        public Color FaceColor
        {
            get { return my_FaceColor; }
            set
            {
                my_FaceColor = value;
                Rect.Fill = new SolidColorBrush(my_FaceColor);
            }
        }

        public int FaceColorNumber
        {
            get { return my_FaceColorNumber; }
            set
            {
                my_FaceColorNumber = value;
                my_FaceColor = my_Colors[my_FaceColorNumber];
                Rect.Fill = new SolidColorBrush(my_Colors[my_FaceColorNumber]);
            }
        }

        public bool Contains(Point pt)
        {
            return Left < pt.X & Left + Size > pt.X & Top < pt.Y & Top + Size > pt.Y;
        }

        public void ToggleColor()
        {
            if (!Isfixed)
            {
                my_FaceColorNumber += 1;
                if (my_FaceColorNumber > 5) my_FaceColorNumber = 0;
                my_FaceColor = my_Colors[my_FaceColorNumber];
                Rect.Fill = new SolidColorBrush(my_FaceColor);
            }
        }

        public void Draw(Canvas c)
        {
            c.Children.Add(Rect);
        }
    }
}
