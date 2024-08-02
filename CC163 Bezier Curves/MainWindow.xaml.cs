using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Bezier_Curves
{
    public partial class MainWindow : Window
    {
        private Point[] Anchors;
        private Rectangle[] AnchorRects;
        private Point[] ControlPoints;
        private Ellipse[] ControlCircles;
        private Line[] ControlLines;
        private Path BezierSquare;
        private PathGeometry BezierPG;
        private PathFigure BezierFigure;
        private double ControlDistance = 60;
        private BezierSegment[] segments;
        private bool my_MouseDown = false;
        private int SelectedAnchor = -1;
        private int SelectedControl = -1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Create all the parts
            Anchors = new Point[4];
            AnchorRects = new Rectangle[4];
            ControlPoints = new Point[8];
            ControlCircles = new Ellipse[8];
            ControlLines = new Line[8];
            segments = new BezierSegment[4];
            BezierSquare = new Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2.0
            };
            BezierPG = new PathGeometry();
            BezierFigure = new PathFigure();
            for (int i = 0; i < 4; i++)
            {
                segments[i] = new BezierSegment();
            }
            Init();
        }

        private void Init()
        {
            //Show 4 Bezier Curves that form a rounded square
            canvas1.Children.Clear();
            double X = canvas1.ActualWidth / 4;
            double Y = canvas1.ActualHeight / 4;
            //Set the initial locations of the Anchors and ControlPoints
            Anchors[0] = new Point(X, Y);
            Anchors[1] = new Point(3 * X, Y);
            Anchors[2] = new Point(3 * X, 3 * Y);
            Anchors[3] = new Point(X, 3 * Y);
            ControlPoints[0] = new Point(X + ControlDistance, Y - ControlDistance);
            ControlPoints[1] = new Point(3 * X - ControlDistance, Y - ControlDistance);
            ControlPoints[2] = new Point(3 * X + ControlDistance, Y + ControlDistance);
            ControlPoints[3] = new Point(3 * X + ControlDistance, 3 * Y - ControlDistance);
            ControlPoints[4] = new Point(3 * X - ControlDistance, 3 * Y + ControlDistance);
            ControlPoints[5] = new Point(X + ControlDistance, 3 * Y + ControlDistance);
            ControlPoints[6] = new Point(X - ControlDistance, 3 * Y - ControlDistance);
            ControlPoints[7] = new Point(X - ControlDistance, Y + ControlDistance);
            //Draw the Anchors as green Rectangles
            for (int i = 0; i < 4; i++)
            {
                AnchorRects[i] = new Rectangle()
                {
                    Width = 16,
                    Height = 16,
                    Stroke = Brushes.Green,
                    StrokeThickness = 1.0
                };
                AnchorRects[i].SetValue(Canvas.LeftProperty, Anchors[i].X - 8.0);
                AnchorRects[i].SetValue(Canvas.TopProperty, Anchors[i].Y - 8.0);
                canvas1.Children.Add(AnchorRects[i]);
            }
            //Draw the Controls as Red Circles and Lines
            for (int i = 0; i < 8; i++)
            {
                ControlCircles[i] = new Ellipse()
                {
                    Width = 16,
                    Height = 16,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1.0
                };
                ControlCircles[i].SetValue(Canvas.LeftProperty, ControlPoints[i].X - 8.0);
                ControlCircles[i].SetValue(Canvas.TopProperty, ControlPoints[i].Y - 8.0);
                canvas1.Children.Add(ControlCircles[i]);
                ControlLines[i] = new Line()
                {
                    X1 = Anchors[(int)((i + 1) / 2.0) % 4].X,
                    Y1 = Anchors[(int)((i + 1) / 2.0) % 4].Y,
                    X2 = ControlPoints[i].X,
                    Y2 = ControlPoints[i].Y,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1.0
                };
                canvas1.Children.Add(ControlLines[i]);
            }
            //Build the 4 Bezier curves and add them to the BezierSquare Path.
            BezierFigure.StartPoint = Anchors[0];
            segments[0].Point1 = ControlPoints[0];
            segments[0].Point2 = ControlPoints[1];
            segments[0].Point3 = Anchors[1];
            segments[1].Point1 = ControlPoints[2];
            segments[1].Point2 = ControlPoints[3];
            segments[1].Point3 = Anchors[2];
            segments[2].Point1 = ControlPoints[4];
            segments[2].Point2 = ControlPoints[5];
            segments[2].Point3 = Anchors[3];
            segments[3].Point1 = ControlPoints[6];
            segments[3].Point2 = ControlPoints[7];
            segments[3].Point3 = Anchors[0];
            BezierFigure.Segments.Add(segments[0]);
            BezierFigure.Segments.Add(segments[1]);
            BezierFigure.Segments.Add(segments[2]);
            BezierFigure.Segments.Add(segments[3]);
            BezierPG.Figures.Add(BezierFigure);
            BezierSquare.Data = BezierPG;
            canvas1.Children.Add(BezierSquare);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(canvas1);
            for (int i = 0; i < 4; i++)
            {
                if (Math.Abs(pt.X - Anchors[i].X) <= 8 && Math.Abs(pt.Y - Anchors[i].Y) <= 8)
                {
                    AnchorRects[i].Fill = Brushes.Blue;
                    SelectedAnchor = i;
                    my_MouseDown = true;
                    return;
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (Math.Sqrt((pt.X - ControlPoints[i].X) * (pt.X - ControlPoints[i].X) + (pt.Y - ControlPoints[i].Y) * (pt.Y - ControlPoints[i].Y)) <=8 )
                {
                    ControlCircles[i].Fill = Brushes.Blue;
                    SelectedControl = i;
                    my_MouseDown = true;
                    return;
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (my_MouseDown)
            {
                Point pt = e.GetPosition(canvas1);
                if (SelectedAnchor >= 0)
                {
                    Anchors[SelectedAnchor].X = pt.X;
                    Anchors[SelectedAnchor].Y = pt.Y;
                    ReDraw();
                }
                if (SelectedControl >= 0)
                {
                    ControlPoints[SelectedControl].X = pt.X;
                    ControlPoints[SelectedControl].Y = pt.Y;
                    ReDraw();
                }
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectedAnchor >= 0)
            {
                AnchorRects[SelectedAnchor].Fill = Brushes.White;
            }
            if (SelectedControl >= 0)
            {
                ControlCircles[SelectedControl].Fill = Brushes.White;
            }
            SelectedAnchor = -1;
            SelectedControl = -1;
            my_MouseDown = false;
        }

        private void ReDraw()
        {
            if (SelectedAnchor >= 0)
            {
                AnchorRects[SelectedAnchor].SetValue(Canvas.LeftProperty, Anchors[SelectedAnchor].X - 8.0);
                AnchorRects[SelectedAnchor].SetValue(Canvas.TopProperty, Anchors[SelectedAnchor].Y - 8.0);
            }
            if (SelectedControl >= 0)
            {
                ControlCircles[SelectedControl].SetValue(Canvas.LeftProperty, ControlPoints[SelectedControl].X - 8.0);
                ControlCircles[SelectedControl].SetValue(Canvas.TopProperty, ControlPoints[SelectedControl].Y - 8.0);
            }
            for (int i = 0; i < 8; i++)
            {
                ControlLines[i].X1 = Anchors[(int)((i + 1) / 2.0) % 4].X;
                ControlLines[i].Y1 = Anchors[(int)((i + 1) / 2.0) % 4].Y;
                ControlLines[i].X2 = ControlPoints[i].X;
                ControlLines[i].Y2 = ControlPoints[i].Y;
            }
            BezierFigure.StartPoint = Anchors[0];
            segments[0].Point1 = ControlPoints[0];
            segments[0].Point2 = ControlPoints[1];
            segments[0].Point3 = Anchors[1];
            segments[1].Point1 = ControlPoints[2];
            segments[1].Point2 = ControlPoints[3];
            segments[1].Point3 = Anchors[2];
            segments[2].Point1 = ControlPoints[4];
            segments[2].Point2 = ControlPoints[5];
            segments[2].Point3 = Anchors[3];
            segments[3].Point1 = ControlPoints[6];
            segments[3].Point2 = ControlPoints[7];
            segments[3].Point3 = Anchors[0];
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ControlDistance = 50.0;
            Init();
        }

        private void SldDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) { return; }
            ControlDistance = SldDistance.Value;
            double X = canvas1.ActualWidth / 4;
            double Y = canvas1.ActualHeight / 4;
            //Set the initial locations of the Anchors and ControlPoints
            Anchors[0] = new Point(X, Y);
            Anchors[1] = new Point(3 * X, Y);
            Anchors[2] = new Point(3 * X, 3 * Y);
            Anchors[3] = new Point(X, 3 * Y);
            ControlPoints[0] = new Point(X + ControlDistance, Y - ControlDistance);
            ControlPoints[1] = new Point(3 * X - ControlDistance, Y - ControlDistance);
            ControlPoints[2] = new Point(3 * X + ControlDistance, Y + ControlDistance);
            ControlPoints[3] = new Point(3 * X + ControlDistance, 3 * Y - ControlDistance);
            ControlPoints[4] = new Point(3 * X - ControlDistance, 3 * Y + ControlDistance);
            ControlPoints[5] = new Point(X + ControlDistance, 3 * Y + ControlDistance);
            ControlPoints[6] = new Point(X - ControlDistance, 3 * Y - ControlDistance);
            ControlPoints[7] = new Point(X - ControlDistance, Y + ControlDistance);
            //Update the Anchor Rectangles
            for (int i = 0; i < 4; i++)
            {
                AnchorRects[i].SetValue(Canvas.LeftProperty, Anchors[i].X - 8.0);
                AnchorRects[i].SetValue(Canvas.TopProperty, Anchors[i].Y - 8.0);
            }
            //Update the Control Circles and Lines
            for (int i = 0; i < 8; i++)
            {
                ControlCircles[i].SetValue(Canvas.LeftProperty, ControlPoints[i].X - 8.0);
                ControlCircles[i].SetValue(Canvas.TopProperty, ControlPoints[i].Y - 8.0);
                ControlLines[i].X1 = Anchors[(int)((i + 1) / 2.0) % 4].X;
                ControlLines[i].Y1 = Anchors[(int)((i + 1) / 2.0) % 4].Y;
                ControlLines[i].X2 = ControlPoints[i].X;
                ControlLines[i].Y2 = ControlPoints[i].Y;
            }
            //Build the 4 Bezier curves and add them to the BezierSquare Path.
            BezierFigure.StartPoint = Anchors[0];
            segments[0].Point1 = ControlPoints[0];
            segments[0].Point2 = ControlPoints[1];
            segments[0].Point3 = Anchors[1];
            segments[1].Point1 = ControlPoints[2];
            segments[1].Point2 = ControlPoints[3];
            segments[1].Point3 = Anchors[2];
            segments[2].Point1 = ControlPoints[4];
            segments[2].Point2 = ControlPoints[5];
            segments[2].Point3 = Anchors[3];
            segments[3].Point1 = ControlPoints[6];
            segments[3].Point2 = ControlPoints[7];
            segments[3].Point3 = Anchors[0];
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}