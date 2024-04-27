using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Binary_Tree
{
    internal class Node
    {
        private double my_Value;
        private object? my_Item;
        private double X;
        private double Y;
        private int my_Level;
        private Node? Left;
        private Node? Right;

        public Node(double x, double y, int level) 
        {
            my_Value = double.NaN;
            my_Item = null;
            X = x;
            Y = y;
            my_Level = level;
            Left = null;
            Right = null;
        }

        public bool Add(double key, object item, Canvas canv)
        {
            if (double.IsNaN(my_Value))
            {
                double dX = 0.35 * canv.ActualWidth / Math.Pow(2,my_Level);
                my_Value = key;
                my_Item = item;
                Left = new Node(X - dX, Y - 70, my_Level + 1);
                Right = new Node(X + dX, Y - 70, my_Level + 1);
                Draw(canv);
                return true;
            }
            else if (key < my_Value && Left != null)
            {
               return Left.Add(key, item, canv);
            }
            else if(key > my_Value && Right != null) 
            { 
                return Right.Add(key, item, canv);
            }
            return false;
        }

        public object? GetItem(double key)
        {
            if (key == my_Value)
            {
                return my_Item;
            }
            else if (key < my_Value && Left != null)
            {
                return Left.GetItem(key);
            }
            else if (key > my_Value && Right != null)
            {
                return Right.GetItem(key);
            }
            return null;
        }

        public bool Contains(double key)
        {
            if (key == my_Value) 
            { 
                return true; 
            }
            else if (key < my_Value && Left != null)
            {
                return Left.Contains(key);
            }
            else if (key > my_Value && Right != null)
            {
                return Right.Contains(key);
            }
            return false;
        }

        public void Modify(double key, object item)
        {
            if (key == my_Value)
            {
                my_Item = item;
            }
            else if (key < my_Value && Left != null)
            {
                Left.Modify(key, item);
            }
            else if (key > my_Value && Right != null)
            {
                Right.Modify(key, item);
            }
        }

        public void AddKeyToList(List<double> result)
        {
            if (Left != null)
            {
                Left.AddKeyToList(result);
            }
            if (!double.IsNaN(my_Value)) result.Add(my_Value);
            if (Right != null)
            { 
                Right.AddKeyToList(result); 
            }
        }

        public void Draw(Canvas c)
        {
            Ellipse El;
            Label lbl;
            Line l;
            double dX = 0.35 * c.ActualWidth / Math.Pow(2, my_Level);
            l = new Line()
            {
                X1 = X,
                Y1 = Y,
                X2 = X + dX,
                Y2 = Y - 70,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0
            };
            c.Children.Add(l);
            l = new Line()
            {
                X1 = X,
                Y1 = Y,
                X2 = X - dX,
                Y2 = Y - 70,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0
            };
            c.Children.Add(l);
            El = new Ellipse()
            {
                Width = 30,
                Height = 30,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0,
                Fill = Brushes.White
            };
            El.SetValue(Canvas.LeftProperty, X - 15);
            El.SetValue(Canvas.TopProperty, Y - 15);
            c.Children.Add(El);
            lbl = new Label()
            {
                Width = 32,
                Height = 28,
                Content = my_Value.ToString(),
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            lbl.SetValue(Canvas.LeftProperty, X - 14);
            lbl.SetValue(Canvas.TopProperty, Y - 16);
            c.Children.Add(lbl);
            El = new Ellipse()
            {
                Width = 30,
                Height = 30,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0
            };
            El.SetValue(Canvas.LeftProperty, X + dX - 15);
            El.SetValue(Canvas.TopProperty, Y - 70 - 15);
            c.Children.Add(El);
            El = new Ellipse()
            {
                Width = 30,
                Height = 30,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0
            };
            El.SetValue(Canvas.LeftProperty, X - dX - 15);
            El.SetValue(Canvas.TopProperty, Y - 70 - 15);
            c.Children.Add(El);
        }
    }
}
