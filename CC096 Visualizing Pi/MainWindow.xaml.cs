using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Visualizing_Pi
{
    public partial class MainWindow : Window
    {
        private string Pi;
        private double MyCircleRadius;
        private double[] startAngles = new double[10];
        private double[] endAngles = new double[10];
        private int[] digitCounter = new int[10];
        private Brush[] LineColors = new Brush[10];
        private Brush[] SegmentColors = new Brush[10];
        private int counter = 0;
        private int digit;
        private double angleOffset;
        private Line l;
        private bool MyLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\Pi.txt");
            Pi = sr.ReadToEnd();
            sr.Close();
            //Define the colors
            byte alpha = 100;
            SegmentColors[0] = new SolidColorBrush(Color.FromRgb(255, 121, 117));
            SegmentColors[1] = new SolidColorBrush(Color.FromRgb(135, 250, 135));
            SegmentColors[2] = new SolidColorBrush(Color.FromRgb(70, 140, 240));
            SegmentColors[3] = new SolidColorBrush(Color.FromRgb(216, 216, 110));
            SegmentColors[4] = new SolidColorBrush(Color.FromRgb(70, 185, 75));
            SegmentColors[5] = new SolidColorBrush(Color.FromRgb(195, 86, 125));
            SegmentColors[6] = new SolidColorBrush(Color.FromRgb(60, 70, 247));
            SegmentColors[7] = new SolidColorBrush(Color.FromRgb(228, 168, 97));
            SegmentColors[8] = new SolidColorBrush(Color.FromRgb(110, 210, 210));
            SegmentColors[9] = new SolidColorBrush(Color.FromRgb(140, 140, 240));
            LineColors[0] = new SolidColorBrush(Color.FromArgb(alpha, 255, 121, 117));
            LineColors[1] = new SolidColorBrush(Color.FromArgb(alpha, 135, 250, 135));
            LineColors[2] = new SolidColorBrush(Color.FromArgb(alpha, 70, 140, 240));
            LineColors[3] = new SolidColorBrush(Color.FromArgb(alpha, 216, 216, 110));
            LineColors[4] = new SolidColorBrush(Color.FromArgb(alpha, 70, 185, 75));
            LineColors[5] = new SolidColorBrush(Color.FromArgb(alpha, 195, 86, 125));
            LineColors[6] = new SolidColorBrush(Color.FromArgb(alpha, 60, 70, 247));
            LineColors[7] = new SolidColorBrush(Color.FromArgb(alpha, 228, 168, 97));
            LineColors[8] = new SolidColorBrush(Color.FromArgb(alpha, 110, 210, 210));
            LineColors[9] = new SolidColorBrush(Color.FromArgb(alpha, 140, 140, 240));
            MyLoaded = true;
            Init();
        }

        private void Init()
        {
            MyCanvas.Children.Clear();
            System.Windows.Shapes.Path my_Path;
            PathGeometry my_PG;
            PathFigure my_figure;
            for (int I = 0; I < 10; I++)
            {
                digitCounter[I] = 0;
            }
            //Get the Graph Diameter
            if (MyCanvas.ActualWidth > MyCanvas.ActualHeight)
            {
                MyCircleRadius = MyCanvas.ActualHeight / 2 - 50;
            }
            else
            {
                MyCircleRadius = MyCanvas.ActualWidth / 2 - 50;
            }
            my_Path = new System.Windows.Shapes.Path();
            my_PG = new PathGeometry();
            my_figure = new PathFigure();
            my_PG.Figures.Add(my_figure);
            my_Path.Data = my_PG;
            //Set the arcSegments
            for (int I = 0; I < 10; I++)
            {
                startAngles[I] = (36 * I + 2) * Math.PI / 180;
                endAngles[I] = (36 * (I + 1) - 2) * Math.PI / 180;
                Point arcpt1 = new Point(MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Cos(startAngles[I]), MyCanvas.ActualHeight / 2 + MyCircleRadius * Math.Sin(startAngles[I]));
                Point arcpt2 = new Point(MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Cos(endAngles[I]), MyCanvas.ActualHeight / 2 + MyCircleRadius * Math.Sin(endAngles[I]));
                //Step2: Make an ArcSegment and set it in my_figure
                my_Path = new System.Windows.Shapes.Path();
                my_PG = new PathGeometry();
                my_figure = new PathFigure();
                my_PG.Figures.Add(my_figure);
                my_Path.Data = my_PG;
                my_Path.Stroke = SegmentColors[I];
                my_Path.StrokeThickness = 15.0;
                my_figure.StartPoint = arcpt1;
                my_figure.Segments.Add(new ArcSegment(arcpt2, new Size(MyCircleRadius, MyCircleRadius), 30, false, SweepDirection.Clockwise, true));
                MyCanvas.Children.Add(my_Path);
            }
            //Set the Digit labels
            Symbol lbl;
            double lblAngle;
            for (int I = 0; I < 10; I++)
            {
                lblAngle = (36 * I + 18) * Math.PI / 180;
                Point pt = new Point(0, 0);
                lbl = new Symbol(I.ToString(), pt, Colors.White, "Arial", 18);
                lbl.Left = MyCanvas.ActualWidth / 2 + 1.06 * MyCircleRadius * Math.Cos(lblAngle) - lbl.Width / 2;
                lbl.Top = MyCanvas.ActualHeight / 2 + 1.06 * MyCircleRadius * Math.Sin(lblAngle) - lbl.Height / 2;
                MyCanvas.Children.Add(lbl);
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            for (int I = 0; I < 20; I++)
            {
                l = new Line();
                //Get line start point
                digit = int.Parse(Pi[counter].ToString());
                counter += 1;
                angleOffset = (digitCounter[digit] + 50) * 0.0005;
                digitCounter[digit] += 1;
                if (digitCounter[digit] > 1000) digitCounter[digit] = 0;
                l.Stroke = LineColors[digit];
                l.StrokeThickness = 1.0;
                l.X1 = MyCanvas.ActualWidth / 2 + (MyCircleRadius - 25) * Math.Cos(startAngles[digit] + angleOffset);
                l.Y1 = MyCanvas.ActualHeight / 2 + (MyCircleRadius - 25) * Math.Sin(startAngles[digit] + angleOffset);
                //Get line end point
                digit = int.Parse(Pi[counter].ToString());
                counter += 1;
                angleOffset = (digitCounter[digit] + 50) * 0.0005;
                digitCounter[digit] += 1;
                if (digitCounter[digit] > 1000) digitCounter[digit] = 0;
                l.X2 = MyCanvas.ActualWidth / 2 + (MyCircleRadius - 25) * Math.Cos(startAngles[digit] + angleOffset);
                l.Y2 = MyCanvas.ActualHeight / 2 + (MyCircleRadius - 25) * Math.Sin(startAngles[digit] + angleOffset);
                //Show the line
                MyCanvas.Children.Add(l);
            }
            if (counter > 10000)
            {
                counter = 0;
                Init();
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MyLoaded)
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                Init();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}