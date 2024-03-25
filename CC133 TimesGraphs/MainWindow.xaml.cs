using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TimesGraphs
{
    public partial class MainWindow : Window
    {
        private Brush[] MyBrushes = new Brush[10];
        private Ellipse MyEllipse = new Ellipse();
        private List<Point> startPts = new List<Point>();
        private List<Line> MyLines = new List<Line>();
        private int Multiplier = 2;
        private int ColorIndex = 0;
        private bool MyLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Define the colors
            MyBrushes[0] = Brushes.LightBlue;
            MyBrushes[1] = Brushes.Cyan;
            MyBrushes[2] = Brushes.Green;
            MyBrushes[3] = Brushes.YellowGreen;
            MyBrushes[4] = Brushes.Yellow;
            MyBrushes[5] = Brushes.Orange;
            MyBrushes[6] = Brushes.Red;
            MyBrushes[7] = Brushes.Violet;
            MyBrushes[8] = Brushes.Pink;
            MyBrushes[9] = Brushes.White;
            MyLoaded = true;
            Init();
            DrawPie();
            Title = "TimesGraph of " + Multiplier.ToString();
        }

        private void Init()
        {
            double MyCircleRadius;
            MyCanvas.Children.Clear();
            MyLines.Clear();
            //Get the Graph Diameter
            if (MyCanvas.ActualWidth > MyCanvas.ActualHeight)
            {
                MyCircleRadius = MyCanvas.ActualHeight / 2 - 10;
            }
            else
            {
                MyCircleRadius = MyCanvas.ActualWidth / 2 - 10;
            }
            //Set the circle values
            MyEllipse.Width = 2 * MyCircleRadius;
            MyEllipse.Height = 2 * MyCircleRadius;
            MyEllipse.StrokeThickness = 1;
            MyEllipse.SetValue(Canvas.TopProperty, MyCanvas.ActualHeight / 2 - MyCircleRadius);
            MyEllipse.SetValue(Canvas.LeftProperty, MyCanvas.ActualWidth / 2 - MyCircleRadius);
            MyCanvas.Children.Add(MyEllipse);
            //Calculate the Points on the circle
            startPts.Clear();
            for (int I = 0; I <= 400; I++)
            {
                startPts.Add(new Point(MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Sin(I * Math.PI / 200), MyCanvas.ActualHeight / 2 - MyCircleRadius * Math.Cos(I * Math.PI / 200)));
            }
            //Make the lines
            for (int I = 0; I <= 400; I++)
            {
                MyLines.Add(new Line());
                MyCanvas.Children.Add(MyLines[I]);
            }
        }

        private void DrawPie()
        {
            //Generate the TimesGraph
            MyEllipse.Stroke = MyBrushes[ColorIndex];
            for (int I = 0; I < 400; I++)
            {
                MyLines[I].X1 = startPts[I].X;
                MyLines[I].Y1 = startPts[I].Y;
                MyLines[I].X2 = startPts[Multiplier * I % 400].X;
                MyLines[I].Y2 = startPts[Multiplier * I % 400].Y;
                MyLines[I].Stroke = MyBrushes[ColorIndex];
                MyLines[I].StrokeThickness = 1;
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MyLoaded)
            {
                Init();
                DrawPie();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Multiplier++;
            ColorIndex++;
            if (ColorIndex > 9) ColorIndex = 0;
            DrawPie();
            Title = "TimesGraph of " + Multiplier.ToString();
        }
    }
}