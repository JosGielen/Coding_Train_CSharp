using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2D_Supershapes
{
    public partial class MainWindow : Window
    {
        private Settings settingForm;
        private double Scale = 1.0;
        private int PointCount = 200;
        private PointCollection points;
        private double A = 1.0;
        private double B = 1.0;
        private double M = 0.0;
        private double N1 = 0.2;
        private double N2 = 1.7;
        private double N3 = 1.7;
               
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowSettingForm();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsLoaded ) { return; }
            settingForm.Left = Left + ActualWidth;
            settingForm.Top = Top;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) { return; }
            settingForm.Left = Left + ActualWidth;
            settingForm.Top = Top;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        public void Start()
        {
            canvas1.Children.Clear();
            points = new PointCollection(PointCount);
            GetParameters();
            Polygon poly = new Polygon()
            {
                Stroke = Brushes.White,
                StrokeThickness = 2.0
            };
            double R;
            double X;
            double Y;
            double minX = double.MaxValue;
            double maxX = 0.0;
            double minY = double.MaxValue;
            double maxY = 0.0;
            for(double Angle = 0; Angle <= 2 * Math.PI; Angle += 2 * Math.PI / PointCount )
            {
                R = SuperShape(Angle);
                X = R * Math.Cos(Angle);
                if (X > maxX) { maxX = X; }
                if (X < minX) { minX = X; }
                Y = R * Math.Sin(Angle);
                if (Y > maxY) { maxY = Y; }
                if (Y < minY) { minY = Y; }
                points.Add(new Point(X, Y));
            }
            //Determine the Scale to fit the Polygon inside the canvas
            if (maxX - minX > maxY - minY)
            {
                Scale = (canvas1.ActualWidth - 100) / (maxX - minX);
            }
            else
            {
                Scale = (canvas1.ActualHeight - 100) / (maxY - minY);
            }
            minX = Scale * minX;
            maxX = Scale * maxX;
            minY = Scale * minY;
            maxY = Scale * maxY;
            for (int I = 0; I < points.Count(); I++)
            {
                points[I] = new Point(Scale * points[I].X + (canvas1.ActualWidth - minX - maxX) / 2, Scale * points[I].Y + (canvas1.ActualHeight - minY - maxY) / 2);
            }
            poly.Points = points;
            canvas1.Children.Add(poly);
        }

        private double SuperShape(double Angle)
        {
            double part1 = Math.Pow(Math.Abs(Math.Cos(Angle * M / 4) / A), N2);
            double part2 = Math.Pow(Math.Abs(Math.Sin(Angle * M / 4) / B), N3);
            return 1 / Math.Pow(part1 + part2, 1 / N1);
        }

        private void ShowSettingForm()
        {
            if (settingForm == null )
            {
                settingForm = new Settings(this);
                settingForm.Show();
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
                settingForm.A = A;
                settingForm.B = B;
                settingForm.M = M;
                settingForm.N1 = N1;
                settingForm.N2 = N2;
                settingForm.N3 = N3;
            }
            else
            {
                settingForm.Show();
            }
            settingForm.Update();
        }

        public void GetParameters()
        {
            if (settingForm != null )
            {
                A = settingForm.A;
                B = settingForm.B;
                M = settingForm.M;
                N1 = settingForm.N1;
                N2 = settingForm.N2;
                N3 = settingForm.N3;
            }
        }
    }
}
