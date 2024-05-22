using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Recursion
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateCircleX(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2, 150, Brushes.Black);
            CreateCircleY(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2, 150, Brushes.Black);
        }

        private void CreateCircleX(double x, double y, double radius, Brush c)
        {
            if (radius > 2)
            {
                Ellipse el = new Ellipse()
                {
                    Width = 2 * radius,
                    Height = 2 * radius,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.0
                };
                el.SetValue(Canvas.LeftProperty, x - radius);
                el.SetValue(Canvas.TopProperty, y - radius);
                canvas1.Children.Add(el);
                CreateCircleX(x + radius, y, radius * 0.5, Brushes.Black);
                CreateCircleX(x - radius, y, radius * 0.5, Brushes.White);
            }
        }

        private void CreateCircleY(double x, double y, double radius, Brush c)
        {
            if (radius > 2)
            {
                Ellipse el = new Ellipse()
                {
                    Width = 2 * radius,
                    Height = 2 * radius,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.0
                };
                el.SetValue(Canvas.LeftProperty, x - radius);
                el.SetValue(Canvas.TopProperty, y - radius);
                canvas1.Children.Add(el);
                CreateCircleY(x, y + radius, radius * 0.5, Brushes.Black);
                CreateCircleY(x, y - radius, radius * 0.5, Brushes.White);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}