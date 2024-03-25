using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RDP_Algorithm
{
    public partial class MainWindow : Window
    {
        private List<Vector> points;
        private List<Vector> RDPpoints;
        private int My_Width = 0;
        private int My_Height = 0;
        private double Epsilon = 5.0;
        private bool AppLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            My_Width = (int)(canvas1.ActualWidth);
            My_Height = (int)(canvas1.ActualHeight);
            points = new List<Vector>();
            RDPpoints = new List<Vector>();
            EpSlider.Value = 5.0;
            TxtEpsilon.Text = Epsilon.ToString();
            double X = 0.0;
            double Y = 0.0;
            for (int I = 0; I < My_Width; I++)
            {
                X = I * 5.0 / My_Width;
                Y = My_Height - (Math.Exp(-X) * Math.Cos(2 * Math.PI * X) + 1) * My_Height / 2;
                points.Add(new Vector(I, Y));
            }
            CalculateRDP();
            Draw();
            AppLoaded = true;
        }

        private void EpSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
         {
            if (!AppLoaded) return;
            Epsilon = EpSlider.Value;
            TxtEpsilon.Text = Epsilon.ToString("F1");
            CalculateRDP();
            Draw();
        }

        private void CalculateRDP()
        {
            RDPpoints.Clear();
            RDPpoints.Add(points.First());
            RDP(1, points.Count - 2);
            RDPpoints.Add(points.Last());
        }

        private void RDP(int firstIndex, int lastIndex)
        {
            int nextIndex = FindFurthest(firstIndex, lastIndex);
            if (nextIndex > 0)
            {
                RDP(firstIndex, nextIndex);
                RDPpoints.Add(points[nextIndex]);
                RDP(nextIndex + 1, lastIndex);
            }
        }

        private int FindFurthest(int firstIndex, int lastIndex)
        {
            double recordDistance = -1.0;
            int recordIndex = 0;
            double dist = -1.0;
            if (firstIndex >= lastIndex) return -1;
            for (int I = firstIndex; I <= lastIndex; I++)
            {
                dist = LineDistance(points[firstIndex], points[lastIndex], points[I]);
                if (dist > recordDistance)
                {
                    recordDistance = dist;
                    recordIndex = I;
                }
            }
            if (recordDistance >= Epsilon)
            {
                return recordIndex;
            }
            else
            {
                return -1;
            }
        }

        private double LineDistance(Vector a, Vector b, Vector c)
        {
            Vector ac = c - a;
            Vector ab = b - a;
            Vector normal;
            ab.Normalize();
            ab = (ac * ab) * ab;
            normal = a + ab;
            return (c - normal).Length;
        }

        private void Draw()
        {
            Line L;
            canvas1.Children.Clear();
            for (int I = 0; I < points.Count - 1; I++)
            {
                L = new Line()
                {
                    X1 = points[I].X,
                    Y1 = points[I].Y,
                    X2 = points[I + 1].X,
                    Y2 = points[I + 1].Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 1.0
                };
                canvas1.Children.Add(L);
            }
            for (int I = 0; I < RDPpoints.Count() - 1; I++)
            {
                L = new Line()
                {
                    X1 = RDPpoints[I].X,
                    Y1 = RDPpoints[I].Y,
                    X2 = RDPpoints[I + 1].X,
                    Y2 = RDPpoints[I + 1].Y,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1.5
                };
                canvas1.Children.Add(L);
            }
            TxtRDPpoints.Text = RDPpoints.Count.ToString();
        }

    }
}