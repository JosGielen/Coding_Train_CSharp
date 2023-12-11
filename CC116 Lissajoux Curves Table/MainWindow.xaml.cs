using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lissajoux_Curves_Table
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private bool Rendering;
        private int Rows;
        private int Cols;
        private double Angle;
        private double CircleDia;
        private double[] ColCenters;
        private double[] RowCenters;
        private double[] X;
        private double[] Y;
        private Ellipse[] ColDots;
        private Ellipse[] RowDots;
        private Line[] ColLines;
        private Line[] RowLines;
        private Polyline[,] Figures;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Ellipse El;
            double Spacing = 20.0;
            Cols = 8;
            Rows = 8;
            ColCenters = new double[Cols + 1];
            X = new double[Cols + 1];
            ColDots = new Ellipse[Cols + 1];
            ColLines = new Line[Cols + 1];
            RowCenters = new double[Rows + 1];
            Y = new double[Rows + 1];
            RowDots = new Ellipse[Rows + 1];
            RowLines = new Line[Rows + 1];
            Figures = new Polyline[Cols + 1, Rows + 1];
            //Determine the Circle diameter
            if ((canvas1.ActualWidth - (Cols + 2) * Spacing) / (Cols + 1) < (canvas1.ActualHeight - (Rows + 2) * Spacing) / (Rows + 1))
            {
                CircleDia = (canvas1.ActualWidth - (Cols + 2) * Spacing) / (Cols + 1);
            }
            else
            {
                CircleDia = (canvas1.ActualHeight - (Rows + 2) * Spacing) / (Rows + 1);
            }
            //Calculate the starting positions
            Angle = -1 * Math.PI / 2;
            for (int I = 0; I <= Cols; I++)
            {
                ColCenters[I] = Spacing + I * (Spacing + CircleDia) + CircleDia / 2;
                X[I] = ColCenters[I] + (CircleDia / 2) * Math.Cos(Angle);
            }
            for (int I = 0; I <= Rows; I++)
            {
                RowCenters[I] = Spacing + I * (Spacing + CircleDia) + CircleDia / 2;
                Y[I] = RowCenters[I] + (CircleDia / 2) * Math.Sin(Angle);
            }
            //Draw the Column Circles, Dots and Lines
            for (int I = 1; I <= Cols; I++)
            {
                El = new Ellipse()
                {
                    Width = CircleDia,
                    Height = CircleDia,
                    Stroke = Brushes.White,
                    StrokeThickness = 1.0
                };
                El.SetValue(Canvas.LeftProperty, ColCenters[I] - CircleDia / 2);
                El.SetValue(Canvas.TopProperty, Spacing);
                canvas1.Children.Add(El);
                ColDots[I] = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Fill = Brushes.Red
                };
                ColDots[I].SetValue(Canvas.LeftProperty, X[I] - 3);
                ColDots[I].SetValue(Canvas.TopProperty, Y[0] - 3);
                canvas1.Children.Add(ColDots[I]);
                ColLines[I] = new Line()
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(150, 150, 150, 150)),
                    StrokeThickness = 1.0,
                    X1 = X[I],
                    Y1 = Y[0],
                    X2 = X[I],
                    Y2 = canvas1.ActualHeight
                };
                canvas1.Children.Add(ColLines[I]);
            }
            //Draw the Row Circles, Dots and Lines
            for (int I = 1; I <= Rows; I++)
            {
                El = new Ellipse()
                {
                    Width = CircleDia,
                    Height = CircleDia,
                    Stroke = Brushes.White,
                    StrokeThickness = 1.0
                };
                El.SetValue(Canvas.LeftProperty, Spacing);
                El.SetValue(Canvas.TopProperty, RowCenters[I] - CircleDia / 2);
                canvas1.Children.Add(El);
                RowDots[I] = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Fill = Brushes.Red
                };
                RowDots[I].SetValue(Canvas.LeftProperty, X[0] - 3);
                RowDots[I].SetValue(Canvas.TopProperty, Y[I] - 3);
                canvas1.Children.Add(RowDots[I]);
                RowLines[I] = new Line()
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(150, 150, 150, 150)),
                    StrokeThickness = 1.0,
                    X1 = X[0],
                    Y1 = Y[I],
                    X2 = canvas1.ActualWidth,
                    Y2 = Y[I]
                };
                canvas1.Children.Add(RowLines[I]);
            }
            for (int I = 1; I <= Cols; I++)
            {
                for (int J = 1; J <= Rows; J++)
                {
                    Figures[I, J] = new Polyline()
                    {
                        Stroke = Brushes.White,
                        StrokeThickness = 1.0
                    };
                    canvas1.Children.Add(Figures[I, J]);
                }
            }
            Rendering = true;
            CompositionTarget.Rendering += Render;
        }

        public void Render(object sender, EventArgs e)
        {
            if (!Rendering) return;
            for (int I = 0; I <= Cols; I++)
            {
                X[I] = ColCenters[I] + (CircleDia / 2) * Math.Cos(Angle * I);
            }
            for (int I = 0; I <= Rows; I++)
            {
                Y[I] = RowCenters[I] + (CircleDia / 2) * Math.Sin(Angle * I);
            }
            //Update the Column Dots and Lines
            for (int I = 1; I <= Cols; I++)
            {
                ColDots[I].SetValue(Canvas.LeftProperty, X[I] - 3);
                ColDots[I].SetValue(Canvas.TopProperty, RowCenters[0] + (CircleDia / 2) * Math.Sin(Angle * I) - 3);
                ColLines[I].X1 = X[I];
                ColLines[I].Y1 = RowCenters[0] + (CircleDia / 2) * Math.Sin(Angle * I);
                ColLines[I].X2 = X[I];
                ColLines[I].Y2 = canvas1.ActualHeight;
            }
            //Update the Row Dots and Lines
            for (int I = 1; I <= Rows; I++)
            {
                RowDots[I].SetValue(Canvas.LeftProperty, ColCenters[0] + (CircleDia / 2) * Math.Cos(Angle * I) - 3);
                RowDots[I].SetValue(Canvas.TopProperty, Y[I] - 3);
                RowLines[I].X1 = ColCenters[0] + (CircleDia / 2) * Math.Cos(Angle * I);
                RowLines[I].Y1 = Y[I];
                RowLines[I].X2 = canvas1.ActualWidth;
                RowLines[I].Y2 = Y[I];
            }
            //Draw the Figures
            for (int I = 1; I <= Cols; I++)
            {
                for (int J = 1; J <= Rows; J++)
                {
                    Figures[I, J].Points.Add(new Point(X[I], Y[J]));
                }
            }
            Angle -= 0.01;
            if (Angle < -5 * Math.PI / 2)
            {
                Thread.Sleep(6000);
                //Reset the angle and curves
                Angle = -1 * Math.PI / 2;
                for (int I = 1; I <= Cols; I++)
                {
                    for (int J = 1; J <= Rows; J++)
                    {
                        Figures[I, J].Points.Clear();
                    }
                }
            }
        }
    }
}
