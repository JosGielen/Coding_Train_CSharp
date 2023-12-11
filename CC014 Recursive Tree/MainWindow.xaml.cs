using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Recursive_Tree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double len = 180.0;
        private double Angle = 30.0;
        private RotateTransform RT;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            Init();
            DrawBranch(len, new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight), RT);
        }

        private void SldAngle_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angle = SldAngle.Value;
            Init();
            DrawBranch(len, new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight), RT);
        }

        private void Init()
        {
            canvas1.Children.Clear();
            RT = new RotateTransform()
            {
                Angle = 0.0,
                CenterX = canvas1.ActualWidth / 2,
                CenterY = canvas1.ActualHeight
            };
        }

        private void DrawBranch(double length, Point Start, RotateTransform rt)
        {
            if (length > 3)
            {
                Point endPt = rt.Transform(new Point(Start.X, Start.Y - length));
                RotateTransform r = new RotateTransform(rt.Angle);
                Line l = new Line()
                {
                    Stroke = Brushes.White,
                    StrokeThickness = 2.0,
                    X1 = Start.X,
                    Y1 = Start.Y
                };
                l.X2 = endPt.X;
                l.Y2 = endPt.Y;
                canvas1.Children.Add(l);
                r.CenterX = endPt.X;
                r.CenterY = endPt.Y;
                r.Angle += Angle;
                DrawBranch(0.67 * length, endPt, r);
                r.Angle -= 2 * Angle;
                DrawBranch(0.67 * length, endPt, r);
            }
        }
    }
}