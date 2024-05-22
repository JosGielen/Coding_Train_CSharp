using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Circle_Morphing
{
    public partial class MainWindow : Window
    {
        private List<Point> CircPath = new List<Point>();
        private List<bool> ShowPoints = new List<bool>();
        private int res = 300;
        private Polygon poly;
        private double radius = 250;
        private int index;
        private bool AddingPoints = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double angle;
            Point pt;
            double oneThird = 2 * Math.PI / 3;
            Point Center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            poly = new Polygon()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3.0
            };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < res; j++)
                {
                    angle = (i + j / (double)res) * oneThird - Math.PI / 2;
                    pt = new Point(Center.X + radius * Math.Cos(angle), Center.Y + radius * Math.Sin(angle));
                    CircPath.Add(pt);
                    poly.Points.Add(pt);
                    ShowPoints.Add(true);
                }
            }
            canvas1.Children.Add(poly);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (poly.Points.Count == 3)
            {
                AddingPoints = true;
                index = 0;
            }
            else if (poly.Points.Count == CircPath.Count)
            {
                AddingPoints = false;
                index = 0;
            }
            if (index < res)
            {
                ShowPoints[index] = AddingPoints;
                ShowPoints[index + res] = AddingPoints;
                ShowPoints[index + 2 * res] = AddingPoints;
            }
            index++;
            poly.Points.Clear();
            for (int i = 0; i < CircPath.Count; i++)
            {
                if (ShowPoints[i]) { poly.Points.Add(CircPath[i]); }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}