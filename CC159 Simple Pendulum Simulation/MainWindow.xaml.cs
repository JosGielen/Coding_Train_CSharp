using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Pendulum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point origin;
        private Line l;
        private readonly double LineLength = 300;
        private Ellipse bob;
        private double bobSize = 50;
        private readonly double gravity = 9.81;
        private bool mouseIsDown = false;
        private double angle;
        private double angleVel;
        private double angleAcc;
        private Point BobPosition;
        private double damping = 0.0003;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            origin = new Point(canvas1.ActualWidth / 2, 0);
            angle = 0;
            BobPosition = new Point((LineLength + bobSize / 2) * Math.Sin(angle * Math.PI / 180) + origin.X, (LineLength + bobSize / 2) * Math.Cos(angle * Math.PI / 180) + origin.Y);
            l = new Line()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 4.0,
                Fill = Brushes.Yellow,
                X1 = origin.X,
                Y1 = origin.Y,
                X2 = BobPosition.X,
                Y2 = BobPosition.Y
            };
            canvas1.Children.Add(l);
            bob = new Ellipse()
            {
                Stroke = Brushes.Blue ,
                StrokeThickness = 1.0,
                Fill = Brushes.Blue ,
                Width= bobSize,
                Height= bobSize
            };
            bob.SetValue(Canvas.LeftProperty, BobPosition.X - bobSize / 2);
            bob.SetValue(Canvas.TopProperty, BobPosition.Y - bobSize / 2);
            canvas1.Children.Add(bob);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseIsDown = true;
            CompositionTarget.Rendering -= Render;
            angleVel = 0.0;
            angleAcc = 0.0;
            Vector v = e.GetPosition(canvas1) - origin;
            v.Normalize();
            v *= LineLength + bobSize / 2;
            BobPosition = new Point(origin.X + v.X, origin.Y + v.Y)  ;
            l.X2 = BobPosition.X;
            l.Y2 = BobPosition.Y;
            bob.SetValue(Canvas.LeftProperty, BobPosition.X - bobSize / 2);
            bob.SetValue(Canvas.TopProperty, BobPosition.Y - bobSize / 2);
            angle = Vector.AngleBetween(v, new Vector(0, 1));
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                Vector v = e.GetPosition(canvas1) - origin;
                v.Normalize();
                v *= LineLength + bobSize / 2;
                BobPosition = new Point(origin.X + v.X, origin.Y + v.Y);
                l.X2 = BobPosition.X;
                l.Y2 = BobPosition.Y;
                bob.SetValue(Canvas.LeftProperty, BobPosition.X - bobSize / 2);
                bob.SetValue(Canvas.TopProperty, BobPosition.Y - bobSize / 2);
                angle = Vector.AngleBetween(v, new Vector(0, 1));
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseIsDown = false;
            CompositionTarget.Rendering += Render;
        }

        private void Render(object sender, EventArgs e)
        {
            if (mouseIsDown) return;
            angleAcc = -1 * Math.Sin(angle * Math.PI / 180) * gravity / LineLength;
            angleVel += angleAcc;
            angleVel *= 1 - damping;
            angle += angleVel;
            BobPosition = new Point((LineLength + bobSize / 2) * Math.Sin(angle * Math.PI / 180) + origin.X, (LineLength + bobSize / 2) * Math.Cos(angle * Math.PI / 180) + origin.Y);
            l.X2 = BobPosition.X;
            l.Y2 = BobPosition.Y;
            bob.SetValue(Canvas.LeftProperty, BobPosition.X - bobSize / 2);
            bob.SetValue(Canvas.TopProperty, BobPosition.Y - bobSize / 2);
        }
    }
}
