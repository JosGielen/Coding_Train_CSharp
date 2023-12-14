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

namespace SuperEllipse
{
    public partial class MainWindow : Window
    {
        private double A = 100.0;
        private double B = 100.0;
        private double N = 2.0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            canvas1.Children.Clear();
            Polygon poly = new Polygon()
            {
                Stroke = Brushes.White,
                StrokeThickness = 1.0
            };
            double X = 0.0;
            double Y = 0.0;
            for (double Angle = 0; Angle <= 2 * Math.PI; Angle += Math.PI / 50)
            {
                ;
                X = Math.Pow(Math.Abs(Math.Cos(Angle)), 2 / N) * A * Math.Sign(Math.Cos(Angle));
                Y = Math.Pow(Math.Abs(Math.Sin(Angle)), 2 / N) * B * Math.Sign(Math.Sin(Angle));
                poly.Points.Add(new Point(X + canvas1.ActualWidth / 2, Y + canvas1.ActualHeight / 2));
            }
            canvas1.Children.Add(poly);
        }

        private void SldN_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            N = SldN.Value;
            TxtN.Text = N.ToString("F3");
            Draw();
        }
    }
}