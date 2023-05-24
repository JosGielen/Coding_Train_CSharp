using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quadtree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int DrawMode = 1;
        private Random Rnd = new Random();
        private QTree QT;
        private Rectangle SelectionBox;
        private Rect r;
        private Ellipse SelectionCircle;
        private List<Point> found;
        private List<Ellipse> Selected;
        private List<Rectangle> SelBoxes;
        private List<Ellipse> SelCircles;
        private bool my_MouseDown = false;
        private Point my_MouseStart;
        private Point my_MouseEnd;
        private double dist;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Selected = new List<Ellipse>();
            SelBoxes = new List<Rectangle>();
            SelCircles = new List<Ellipse>();
            QT = new QTree(new Rect(0.0, 0.0, canvas1.ActualWidth, canvas1.ActualHeight), 4);
            Init();
        }

        private void Init()
        {
            Point pt;
            for (int I = 0; I < 500; I++)
            {
                pt = new Point(1.0 + (canvas1.ActualWidth - 2.0) * Rnd.NextDouble(), 1.0 + (canvas1.ActualHeight - 2.0) * Rnd.NextDouble());
                if (!QT.Insert(pt))
                {
                    //This should not happen 
                    throw new Exception("QuadTree Error: The point could not be added to the QuadTree.");
                }
            }
            canvas1.Children.Clear();
            DrawQTree(QT);
        }

        //Draw the QTree
        private void DrawQTree(QTree tree)
        {
            Rectangle r = new Rectangle()
            {
                Width = tree.Boundary.Width,
                Height = tree.Boundary.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            r.SetValue(Canvas.TopProperty, tree.Boundary.Top);
            r.SetValue(Canvas.LeftProperty, tree.Boundary.Left);
            canvas1.Children.Add(r);
            Ellipse El;
            foreach (Point pt in tree.Points)
            {
                El = new Ellipse()
                {
                    Width = 4.0,
                    Height = 4.0,
                    Stroke = Brushes.Black,
                    Fill = Brushes.Black,
                    StrokeThickness = 1
                };
                El.SetValue(Canvas.TopProperty, pt.Y - 2.0);
                El.SetValue(Canvas.LeftProperty, pt.X - 2.0);
                canvas1.Children.Add(El);
            }
            if (tree.IsDivided)
            {
                DrawQTree(tree.Branches[0]);
                DrawQTree(tree.Branches[1]);
                DrawQTree(tree.Branches[2]);
                DrawQTree(tree.Branches[3]);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            my_MouseDown = false;
            if (DrawMode == 1)
            {
                found = QT.Query(r);
            }
            else
            {
                found = QT.Query(my_MouseStart, dist);
            }
            TxtSelected.Text = found.Count.ToString();
            //Draw red circles at the found point locations
            Ellipse El;
            for (int I = 0; I < found.Count; I++)
            {
                El = new Ellipse()
                {
                    Width = 4,
                    Height = 4,
                    Stroke = Brushes.Red,
                    Fill = Brushes.Red,
                    StrokeThickness = 1
                };
                El.SetValue(Canvas.TopProperty, found[I].Y - 2.0);
                El.SetValue(Canvas.LeftProperty, found[I].X - 2.0);
                canvas1.Children.Add(El);
                Selected.Add(El);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!my_MouseDown) return;
            my_MouseEnd = e.GetPosition(canvas1);
            if (DrawMode == 1)
            {
                r = new Rect(my_MouseStart, my_MouseEnd);
                SelectionBox.Width = r.Width;
                SelectionBox.Height = r.Height;
                SelectionBox.SetValue(Canvas.TopProperty, r.Top);
                SelectionBox.SetValue(Canvas.LeftProperty, r.Left);
                found = QT.Query(r);
            }
            else
            {
                double X1 = my_MouseStart.X;
                double X2 = my_MouseEnd.X;
                double Y1 = my_MouseStart.Y;
                double Y2 = my_MouseEnd.Y;
                dist = Math.Sqrt((X2 - X1) * (X2 - X1) + (Y2 - Y1) * (Y2 - Y1));
                SelectionCircle.Width = 2 * dist;
                SelectionCircle.Height = 2 * dist;
                SelectionCircle.SetValue(Canvas.TopProperty, Y1 - dist);
                SelectionCircle.SetValue(Canvas.LeftProperty, X1 - dist);
                found = QT.Query(my_MouseStart,dist);
            }
            TxtSelected.Text = found.Count.ToString();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            my_MouseDown = true;
            my_MouseStart = e.GetPosition(canvas1);
            if (DrawMode == 1)
            {
                //r = new Rect(my_MouseStart, my_MouseStart);
                SelectionBox = new Rectangle()
                {
                    Width = 0.0,  //r.Width,
                    Height = 0.0, //r.Height,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2
                };
                SelectionBox.SetValue(Canvas.TopProperty, my_MouseStart.Y);
                SelectionBox.SetValue(Canvas.LeftProperty, my_MouseStart.X);
                canvas1.Children.Add(SelectionBox);
                SelBoxes.Add(SelectionBox);
            }
            else
            {
                SelectionCircle = new Ellipse
                {
                    Width = 0.0,
                    Height = 0.0,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2
                };
                canvas1.Children.Add(SelectionCircle);
                SelCircles.Add(SelectionCircle);
                dist = 0.0;
                SelectionCircle.SetValue(Canvas.TopProperty, my_MouseStart.Y);
                SelectionCircle.SetValue(Canvas.LeftProperty, my_MouseStart.X);
            }
            TxtSelected.Text = "0";
        }

        private void RbRectangle_Click(object sender, RoutedEventArgs e)
        {
            if (RbRectangle.IsChecked.Value)
            {
                DrawMode = 1;
            }
            else
            {
                DrawMode = 2;
            }
        }

        private void RbCircle_Click(object sender, RoutedEventArgs e)
        {
            if (RbCircle.IsChecked.Value)
            {
                DrawMode = 2;
            }
            else
            {
                DrawMode = 1;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            foreach( Ellipse El in Selected )
            {
                if (canvas1.Children.Contains(El))
                {
                    canvas1.Children.Remove(El);
                }
            }
            Selected.Clear();
            foreach (Rectangle r in SelBoxes)
            {
                if (canvas1.Children.Contains(r))
                {
                    canvas1.Children.Remove(r);
                }
            }
            foreach (Ellipse El in SelCircles)
            {
                if (canvas1.Children.Contains(El))
                {
                    canvas1.Children.Remove(El);
                }
            }
            Selected.Clear();
            SelBoxes.Clear();
            SelCircles.Clear();
            TxtSelected.Text = "0";
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            QT.Clear();
            Init();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
