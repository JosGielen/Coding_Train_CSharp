using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Maurer_Rose
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int N = 0;
        private int D = 0;
        private Polygon P1;
        private Polygon P2;

        public MainWindow()
        {
            InitializeComponent();
            P1 = new Polygon()
            {
                Stroke = Brushes.White,
                StrokeThickness = 1.0
            };
            P2 = new Polygon()
            {
                Stroke = Brushes.Magenta,
                StrokeThickness = 3.0
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            canvas1.Children.Add(P1);
            canvas1.Children.Add(P2);
            Draw();
        }

        private void Draw()
        {
            if (!IsLoaded) return;
            double MaxR = 0.0;
            if (canvas1.ActualWidth > canvas1.ActualHeight)
            {
                MaxR = canvas1.ActualHeight / 2;
            }
            else
            {
                MaxR = canvas1.ActualWidth / 2;
            }
            P1.Points.Clear();
            P2.Points.Clear();
            N = (int)(SliderN.Value);
            D = (int)(SliderD.Value);
            Point center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            double K = 0.0;
            double R = 0.0;
            double X = 0.0;
            double Y = 0.0;
            for (int I = 0; I <= 360; I++)
            {
                K = I * D * Math.PI / 180;
                R = Math.Sin(N * K) * MaxR;
                X = R * Math.Cos(K);
                Y = R * Math.Sin(K);
                P1.Points.Add(new Point(center.X + X, center.Y + Y));
            }
            for (double I = 0; I <= 360; I++)
            {
                ;
                K = I * Math.PI / 180;
                R = Math.Sin(N * K) * MaxR;
                X = R * Math.Cos(K);
                Y = R * Math.Sin(K);
                P2.Points.Add(new Point(center.X + X, center.Y + Y));
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Draw();
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