using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rose_Patterns
{
    public partial class MainWindow : Window
    {
        private int N = 0;
        private int D = 0;
        private Polygon P;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            P = new Polygon()
            {
                Stroke = Brushes.White,
                StrokeThickness = 1.0
            };
            canvas1.Children.Add(P);
            Draw();
        }

        private void Draw()
        {
            if (!IsLoaded) return;
            P.Points.Clear();
            N = (int)(SliderN.Value);
            D = (int)(SliderD.Value);
            Point center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            double K = N / D;
            double R = 0;
            for (double A = 0; A <= 2 * D * Math.PI; A += 0.01)
            {
                R = 0.9 * canvas1.ActualWidth / 2 * Math.Cos(N * A / D);
                P.Points.Add(new Point(center.X + R * Math.Cos(A), center.Y + R * Math.Sin(A)));
            }
        }

        private void SliderN_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Draw();
        }

        private void SliderD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Draw();
        }
    }
}