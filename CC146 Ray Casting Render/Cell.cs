using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RayTracing
{
    internal class Cell
    {
        private int my_Row;
        private int my_Col;
        private double my_Size;
        private bool my_IsCurrent;
        private bool my_IsVisited;
        private bool my_HasTopWall;
        private bool my_HasLeftWall;
        private bool my_HasBottomWall;
        private bool my_HasRightWall;
        private Line TopLine;
        private Line LeftLine;
        private Line BottomLine;
        private Line RightLine;
        private System.Windows.Shapes.Rectangle floor;

        public Cell(int row, int col, int size)
        {
            my_Row = row;
            my_Col = col;
            my_Size = size;
            my_IsCurrent = false;
            my_IsVisited = false;
            my_HasTopWall = true;
            my_HasLeftWall = true;
            my_HasBottomWall = true;
            my_HasRightWall = true;
        }

        public int Row
        {
            get { return my_Row; }
        }

        public int Col
        {
            get { return my_Col; }
        }

        public bool IsCurrent
        {
            get { return my_IsCurrent; }
            set
            {
                my_IsCurrent = value;
                if (my_IsCurrent)
                {
                    floor.Fill = Brushes.Green;
                }
                else
                {
                    if (my_IsVisited)
                    {
                        floor.Fill = Brushes.Black;
                    }
                }
            }
        }

        public bool IsVisited
        {
            get { return my_IsVisited; }
            set
            {
                my_IsVisited = value;
                if (my_IsVisited & !my_IsCurrent)
                {
                    floor.Fill = Brushes.Black;
                }
            }
        }

        public bool HasTopWall
        {
            get { return my_HasTopWall; }
        }

        public bool HasLeftWall
        {
            get { return my_HasLeftWall; }
        }

        public bool HasBottomWall
        {
            get { return my_HasBottomWall; }
        }

        public bool HasRightWall
        {
            get { return my_HasRightWall; }
        }

        public void RemoveTopWall()
        {
            TopLine.StrokeThickness = 0.0;
            my_HasTopWall = false;
        }

        public void RemoveLeftWall()
        {
            LeftLine.StrokeThickness = 0.0;
            my_HasLeftWall = false;
        }

        public void RemoveBottomWall()
        {
            BottomLine.StrokeThickness = 0.0;
            my_HasBottomWall = false;
        }

        public void RemoveRightWall()
        {
            RightLine.StrokeThickness = 0.0;
            my_HasRightWall = false;
        }

        public void SetFill(Brush fillColor)
        {
            floor.Fill = fillColor;
        }

        public void Draw(Canvas c)
        {
            double Top = my_Row * my_Size;
            double Left = my_Col * my_Size;
            //Draw the walls as seperate lines
            TopLine = new Line()
            {
                Stroke = Brushes.White,
                StrokeThickness = 4,
                X1 = Left,
                Y1 = Top,
                X2 = Left + my_Size,
                Y2 = Top
            };
            c.Children.Add(TopLine);
            LeftLine = new Line()
            {
                Stroke = Brushes.White,
                StrokeThickness = 4,
                X1 = Left,
                Y1 = Top,
                X2 = Left,
                Y2 = Top + my_Size
            };
            c.Children.Add(LeftLine);
            BottomLine = new Line()
            {
                Stroke = Brushes.White,
                StrokeThickness = 4,
                X1 = Left,
                Y1 = Top + my_Size,
                X2 = Left + my_Size,
                Y2 = Top + my_Size
            };
            c.Children.Add(BottomLine);
            RightLine = new Line()
            {
                Stroke = Brushes.White,
                StrokeThickness = 4,
                X1 = Left + my_Size,
                Y1 = Top,
                X2 = Left + my_Size,
                Y2 = Top + my_Size
            };
            c.Children.Add(RightLine);
            //Draw the fill
            floor = new Rectangle()
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 0.0,
                Width = my_Size,
                Height = my_Size,
                Fill = Brushes.Gray
            };
            floor.SetValue(Canvas.TopProperty, Top);
            floor.SetValue(Canvas.LeftProperty, Left);
            c.Children.Add(floor);
        }
    }
}
