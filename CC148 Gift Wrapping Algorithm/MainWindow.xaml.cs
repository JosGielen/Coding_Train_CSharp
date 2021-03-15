using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Computational_Geometry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int WaitTime = 10;
        private Random Rnd = new Random();
        private List<Point> points;
        private List<Point> shell;
        private int PointCount = 500;
        //Lines to show how the algorithm works
        private Line refLine;
        private Line testLine;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Point p;
            Ellipse El;
            points = new List<Point>();
            shell = new List<Point>();
            //Draw the points in the canvas
            for (int I = 0; I < PointCount; I++)
            {
                p = new Point(canvas1.ActualWidth * Rnd.NextDouble(), canvas1.ActualHeight * Rnd.NextDouble());
                points.Add(p);
                El = new Ellipse()
                {
                    Width = 4,
                    Height = 4,
                    Fill = Brushes.Black
                };
                El.SetValue(Canvas.LeftProperty, p.X - 2);
                El.SetValue(Canvas.TopProperty, p.Y - 2);
                canvas1.Children.Add(El);
            }
            //Add the show lines to the canvas
            refLine = new Line()
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 1.0,
                X1 = 0.0,
                Y1 = 0.0,
                X2 = 0.0,
                Y2 = 0.0
            };
            canvas1.Children.Add(refLine);
            testLine = new Line()
            {
                Stroke = Brushes.Green,
                StrokeThickness = 1.0,
                X1 = 0.0,
                Y1 = 0.0,
                X2 = 0.0,
                Y2 = 0.0
            };
            canvas1.Children.Add(testLine);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            //Sort the points to get the left most
            Point left;
            Point referencePt;
            Point testPt;
            Vector RefV;
            Vector TestV;
            left = points[0];
            for (int I = 0; I < points.Count; I++)
            {
                if (left.X > points[I].X)
                {
                    left = points[I];
                }
            }
            //Calculate the Shell starting from the left most point
            shell.Add(left);
            do
            {
                referencePt = points[shell.Count + 1];
                DrawRefLine(shell.Last(), referencePt);
                for (int I = 0; I < points.Count; I++)
                {
                    testPt = points[I];
                    DrawTestLine(shell.Last(), testPt);
                    RefV = referencePt - shell.Last();
                    TestV = testPt - shell.Last();
                    if (Vector.CrossProduct(RefV, TestV) < 0)
                    {
                        referencePt = testPt;
                        DrawRefLine(shell.Last(), referencePt);
                    }
                    Dispatcher.Invoke(Wait, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                }
                if (referencePt == shell[0])
                {
                    DrawShell(shell.Last(), shell[0]);
                    DrawTestLine(shell.Last(), shell.Last());
                    break;
                }
                else
                {
                    shell.Add(referencePt);
                    DrawShell(shell[shell.Count - 2], shell[shell.Count - 1]);
                }
            } while (true);
            Title = "FINISHED";
        }

        private void DrawRefLine(Point begin, Point finish )
        {
            refLine.X1 = begin.X;
            refLine.Y1 = begin.Y;
            refLine.X2 = finish.X;
            refLine.Y2 = finish.Y;
        }

        private void DrawTestLine(Point begin, Point finish )
        {
            testLine.X1 = begin.X;
            testLine.Y1 = begin.Y;
            testLine.X2 = finish.X;
            testLine.Y2 = finish.Y;
        }

        private void DrawShell(Point previous, Point current )
        {
            Ellipse EL = new Ellipse()
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Red
            };
            EL.SetValue(Canvas.LeftProperty, current.X - 4);
            EL.SetValue(Canvas.TopProperty, current.Y - 4);
            canvas1.Children.Add(EL);
            Line l = new Line()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2.0,
                X1 = previous.X,
                Y1 = previous.Y,
                X2 = current.X,
                Y2 = current.Y
            };
            canvas1.Children.Add(l);
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }
    }
}
