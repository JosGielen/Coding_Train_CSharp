using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Snakes_and_Ladders
{
    internal class Cell
    {
        private int my_Row;
        private int my_Col;
        private int my_Nr;
        private Size my_Size;
        private Point my_Center;
        private bool my_Reverse;

        public Cell(int row, int col, int Nr, Size size, bool reverse) 
        {
            my_Row = row;
            my_Col = col;
            my_Nr = Nr;
            my_Size = size;
            my_Reverse = reverse;
        }

        public int Row 
        { 
            get { return my_Row; } 
        }

        public int Col 
        { 
            get { return my_Col; } 
        }

        public int Nr 
        { 
            get { return my_Nr; } 
        }

        public Point Center 
        { 
            get { return my_Center; } 
        }

        public double Width
        {
            get { return my_Size.Width; }
        }

        public double Height
        { 
            get { return my_Size.Height;} 
        }

        public void Draw(Canvas c)
        {
            Border bor = new Border()
            {
                BorderBrush = Brushes.DarkRed,
                BorderThickness = new Thickness(2.0),
                Width = my_Size.Width,
                Height = my_Size.Height
            };
            if (my_Reverse)
            {
                bor.SetValue(Canvas.LeftProperty, c.ActualWidth - (my_Col + 1) * my_Size.Width);
                my_Center = new Point((int)(c.ActualWidth - (my_Col + 0.5) * my_Size.Width), (int)(c.ActualHeight - (my_Row + 0.5) * my_Size.Height));
            }
            else
            {
                bor.SetValue(Canvas.LeftProperty, my_Col * my_Size.Width);
                my_Center = new Point((int)((my_Col + 0.5) * my_Size.Width), (int)(c.ActualHeight - (my_Row + 0.5) * my_Size.Height));
            }
            bor.SetValue(Canvas.TopProperty, c.ActualHeight - (my_Row + 1) * my_Size.Height);
            Canvas cellCanvas = new Canvas()
            {
                Background = Brushes.Beige
            };
            TextBox tex = new TextBox()
            {
                FontSize = 26,
                FontWeight = FontWeights.Bold,
                Text = my_Nr.ToString(),
                Width = my_Size.Width - 10,
                Background = Brushes.Beige,
                BorderBrush= Brushes.Beige,
                HorizontalContentAlignment= HorizontalAlignment.Center,
                IsReadOnly= true,
                IsTabStop= false,
                Focusable = false
            };
            tex.SetValue(Canvas.LeftProperty, 5.0);
            tex.SetValue(Canvas.TopProperty, cellCanvas.ActualHeight / 2 + 15);
            bor.Child = cellCanvas;
            cellCanvas.Children.Add(tex);
            c.Children.Add(bor);
        }
    }
}
