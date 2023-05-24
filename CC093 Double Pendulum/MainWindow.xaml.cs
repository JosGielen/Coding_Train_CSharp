using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Double_Pendulum
{
    public partial class MainWindow : Window
    {
        private int WaitTime = 20;
        private bool My_Mousedown = false;
        private double X0 = 0.0;
        private double Y0 = 0.0;
        private double X1 = 0.0;
        private double Y1 = 0.0;
        private double X2 = 0.0;
        private double Y2 = 0.0;
        private double A1 = 0.0;
        private double A2 = 0.0;
        private double V1 = 0.0;
        private double V2 = 0.0;
        private double Ac1 = 0.0;
        private double Ac2 = 0.0;
        private double L1 = 0.0;
        private double L2 = 0.0;
        private double M1 = 0.0;
        private double M2 = 0.0;
        private double g = 0.0;
        private Ellipse Ellipse1;
        private Ellipse Ellipse2;
        private Line line1;
        private Line line2;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            A1 = 0;
            A2 = 0;
            L1 = Canvas1.ActualWidth / 4;
            L2 = Canvas1.ActualWidth / 5;
            M1 = 20.0;
            M2 = 20.0;
            g = 1;

            X0 = Canvas1.ActualWidth / 2;
            Y0 = Canvas1.ActualHeight / 4;
            X1 = X0 + L1 * Math.Sin(A1);
            Y1 = Y0 + L1 * Math.Cos(A1);
            X2 = X1 + L2 * Math.Sin(A2);
            Y2 = Y1 + L2 * Math.Cos(A2);
            line1 = new Line
            {
                X1 = X0,
                Y1 = Y0,
                X2 = X1,
                Y2 = Y1,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            line2 = new Line
            {
                X1 = X1,
                Y1 = Y1,
                X2 = X2,
                Y2 = Y2,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            Ellipse1 = new Ellipse
            {
                Fill = Brushes.Blue,
                Width = M1,
                Height = M1
            };
            Ellipse2 = new Ellipse
            {
                Fill = Brushes.Blue,
                Width = M2,
                Height = M2
            };
            Ellipse1.SetValue(Canvas.LeftProperty, X1 - M1 / 2);
            Ellipse1.SetValue(Canvas.TopProperty, Y1 - M1 / 2);
            Ellipse2.SetValue(Canvas.LeftProperty, X2 - M2 / 2);
            Ellipse2.SetValue(Canvas.TopProperty, Y2 - M2 / 2);
            Canvas1.Children.Add(line1);
            Canvas1.Children.Add(line2);
            Canvas1.Children.Add(Ellipse1);
            Canvas1.Children.Add(Ellipse2);
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Draw()
        {
            line1.X1 = X0;
            line1.Y1 = Y0;
            line1.X2 = X1;
            line1.Y2 = Y1;
            line2.X1 = X1;
            line2.Y1 = Y1;
            line2.X2 = X2;
            line2.Y2 = Y2;
            Ellipse1.SetValue(Canvas.LeftProperty, X1 - M1 / 2);
            Ellipse1.SetValue(Canvas.TopProperty, Y1 - M1 / 2);
            Ellipse2.SetValue(Canvas.LeftProperty, X2 - M2 / 2);
            Ellipse2.SetValue(Canvas.TopProperty, Y2 - M2 / 2);
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }

        private void Window_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            double XR1 = 0.0;
            double XR2 = 0.0;
            double YR1 = 0.0;
            double YR2 = 0.0;
            My_Mousedown = true;
            X2 = e.GetPosition(Canvas1).X;
            Y2 = e.GetPosition(Canvas1).Y;
            if (Y2 == Y0) return;
            double A1 = (X0 - X2) / (Y2 - Y0);
            double B1 = (L1 * L1 - L2 * L2 + X2 * X2 - X0 * X0 + Y2 * Y2 - Y0 * Y0) / (2 * (Y2 - Y0));
            double A2 = A1 * A1 + 1;
            double B2 = 2 * A1 * B1 - 2 * A1 * Y0 - 2 * X0;
            double C2 = B1 * B1 - 2 * B1 * Y0 - L1 * L1 + X0 * X0 + Y0 * Y0;
            if (B2 * B2 - 4 * A2 * C2 > 0)
            {
                XR1 = (-B2 + Math.Sqrt(B2 * B2 - 4 * A2 * C2)) / (2 * A2);
                XR2 = (-B2 - Math.Sqrt(B2 * B2 - 4 * A2 * C2)) / (2 * A2);
                YR1 = A1 * XR1 + B1;
                YR2 = A1 * XR2 + B1;
                //Motion continuity
                if (((XR1 - X1) * (XR1 - X1) + (YR1 - Y1) * (YR1 - Y1)) < ((XR2 - X1) * (XR2 - X1) + (YR2 - Y1) * (YR2 - Y1)))
                {
                    X1 = XR1;
                    Y1 = YR1;
                }
                else
                {
                    X1 = XR2;
                    Y1 = YR2;
                }
                Draw();
            }
        }

        private void Window_MouseMove(Object sender, MouseEventArgs e)
        {
            double XR1 = 0.0;
            double XR2 = 0.0;
            double YR1 = 0.0;
            double YR2 = 0.0;
            if (My_Mousedown)
            {
                X2 = e.GetPosition(Canvas1).X;
                Y2 = e.GetPosition(Canvas1).Y;
                if (Y2 == Y0) return;
                double A1 = (X0 - X2) / (Y2 - Y0);
                double B1 = (L1 * L1 - L2 * L2 + X2 * X2 - X0 * X0 + Y2 * Y2 - Y0 * Y0) / (2 * (Y2 - Y0));
                double A2 = A1 * A1 + 1;
                double B2 = 2 * A1 * B1 - 2 * A1 * Y0 - 2 * X0;
                double C2 = B1 * B1 - 2 * B1 * Y0 - L1 * L1 + X0 * X0 + Y0 * Y0;
                if (B2 * B2 - 4 * A2 * C2 > 0)
                {
                    XR1 = (-B2 + Math.Sqrt(B2 * B2 - 4 * A2 * C2)) / (2 * A2);
                    XR2 = (-B2 - Math.Sqrt(B2 * B2 - 4 * A2 * C2)) / (2 * A2);
                    YR1 = A1 * XR1 + B1;
                    YR2 = A1 * XR2 + B1;
                    //Motion continuity
                    if (((XR1 - X1) * (XR1 - X1) + (YR1 - Y1) * (YR1 - Y1)) < ((XR2 - X1) * (XR2 - X1) + (YR2 - Y1) * (YR2 - Y1)))
                    {
                        X1 = XR1;
                        Y1 = YR1;
                    }
                    else
                    {
                        X1 = XR2;
                        Y1 = YR2;
                    }
                    Draw();
                }
            }
        }

        private void Window_MouseLeftButtonUp(Object sender, MouseButtonEventArgs e )
        {
            My_Mousedown = false;
            if (X1 > X0)
            {
                A1 = Math.PI / 2 - Math.Atan((Y1 - Y0) / (X1 - X0));
            }
            else
            {
                A1 = -Math.PI / 2 - Math.Atan((Y1 - Y0) / (X1 - X0));
            }
            if (X2 > X1)
            {
                A2 = Math.PI / 2 - Math.Atan((Y2 - Y1) / (X2 - X1));
            }
            else
            {
                A2 = -Math.PI / 2 - Math.Atan((Y2 - Y1) / (X2 - X1));
            }
            V1 = 0.0;
            V2 = 0.0;
            while (My_Mousedown == false)
            {
                Ac1 = (-g * (2 * M1 + M2) * Math.Sin(A1) - M2 * g * Math.Sin(A1 - 2 * A2) - 2 * Math.Sin(A1 - A2) * M2 * (V2 * V2 * L2 + V1 * V1 * L1 * Math.Cos(A1 - A2))) / (L1 * (2 * M1 + M2 - M2 * Math.Cos(2 * A1 - 2 * A2)));
                Ac2 = (2 * Math.Sin(A1 - A2) * (V1 * V1 * L1 * (M1 + M2) + g * (M1 + M2) * Math.Cos(A1) + V2 * V2 * L2 * M2 * Math.Cos(A1 - A2))) / (L2 * (2 * M1 + M2 - M2 * Math.Cos(2 * A1 - 2 * A2)));
                V1 += Ac1;
                V2 += Ac2;
                V1 *= 0.9995;
                V2 *= 0.9995;
                A1 = A1 + V1;
                A2 = A2 + V2;
                X1 = X0 + L1 * Math.Sin(A1);
                Y1 = Y0 + L1 * Math.Cos(A1);
                X2 = X1 + L2 * Math.Sin(A2);
                Y2 = Y1 + L2 * Math.Cos(A2);
                Draw();
                Dispatcher.Invoke (Wait, DispatcherPriority.ApplicationIdle);
            }
        }

        private void Window_Closed(Object sender, EventArgs e )
        {
            Environment.Exit(0);
        }
    }
}
