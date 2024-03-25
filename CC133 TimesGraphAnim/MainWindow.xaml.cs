using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TimesGraphAnim
{
    public partial class MainWindow : Window
    {
        private delegate void RenderDelegate();
        private delegate void WaitDelegate();
        private bool Started = false;
        private List<Brush> MyBrushes;
        private int Total;
        private double MyCircleRadius;
        private Ellipse MyEllipse = new Ellipse();
        private List<Point> startPts = new List<Point>();
        private List<Line> MyLines = new List<Line>();
        private double Multiplier = 2;
        private bool forward = true;
        private double ColorIndex = 0;
        private bool MyLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "CLICK TO START";
            Total = 400;
            //Define the colors
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow continuous.cpl");
            MyBrushes = pal.GetColorBrushes(256);
            MyLoaded = true;
            Init();
        }

        private void Init()
        {
            MyCanvas.Children.Clear();
            //Get the Graph Diameter
            if (MyCanvas.ActualWidth > MyCanvas.ActualHeight)
            {
                MyCircleRadius = MyCanvas.ActualHeight / 2 - 10;
            }
            else
            {
                MyCircleRadius = MyCanvas.ActualWidth / 2 - 10;
            }
            //Set the circle
            MyEllipse.Width = 2 * MyCircleRadius;
            MyEllipse.Height = 2 * MyCircleRadius;
            MyEllipse.Stroke = MyBrushes[(int)ColorIndex];
            MyEllipse.StrokeThickness = 1;
            MyEllipse.SetValue(Canvas.TopProperty, MyCanvas.ActualHeight / 2 - MyCircleRadius);
            MyEllipse.SetValue(Canvas.LeftProperty, MyCanvas.ActualWidth / 2 - MyCircleRadius);
            MyCanvas.Children.Add(MyEllipse);
            //Calculate the Points on the circle
            startPts.Clear();
            for (int I = 0; I < Total; I++)
            {
                startPts.Add(new Point(MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Sin(I * Math.PI / 200), MyCanvas.ActualHeight / 2 - MyCircleRadius * Math.Cos(I * Math.PI / 200)));
            }
            //Make the lines
            double Endvalue;
            MyLines.Clear();
            for (int I = 0; I < Total; I++)
            {
                MyLines.Add(new Line());
                MyLines[I].StrokeThickness = 1;
                Endvalue = Multiplier * I % Total;
                MyLines[I].X1 = startPts[I].X;
                MyLines[I].Y1 = startPts[I].Y;
                MyLines[I].X2 = MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Sin(Endvalue * Math.PI / 200);
                MyLines[I].Y2 = MyCanvas.ActualHeight / 2 - MyCircleRadius * Math.Cos(Endvalue * Math.PI / 200);
                MyLines[I].Stroke = MyBrushes[(int)ColorIndex];
                MyCanvas.Children.Add(MyLines[I]);
            }
        }

        private void Start()
        {
            double StepSize;
            double N;
            while (Started)
            {
                Dispatcher.Invoke(DispatcherPriority.SystemIdle, new RenderDelegate(Render));
                StepSize = SldStep.Value;
                ColorIndex = ColorIndex + 6 * StepSize;
                if (ColorIndex >= 256) ColorIndex = 0;
                N = Math.Floor(Multiplier / StepSize + 0.000001);
                Multiplier = N * StepSize;
                if (forward)
                {
                    Multiplier += StepSize;
                }
                else
                {
                    Multiplier -= StepSize;
                }
                if (Multiplier >= 150 | Multiplier <= 2) forward = !forward;
                Title = "TimesGraph of " + Multiplier.ToString("F3");
                Thread.Sleep((int)SldSpeed.Value);
            }
        }

        private void Render()
        {
            //Generate the TimesGraphs
            double Endvalue;
            MyEllipse.Stroke = MyBrushes[(int)ColorIndex];
            for (int I = 0; I < Total; I++)
            {
                Endvalue = Multiplier * I % Total;
                MyLines[I].X1 = startPts[I].X;
                MyLines[I].Y1 = startPts[I].Y;
                MyLines[I].X2 = MyCanvas.ActualWidth / 2 + MyCircleRadius * Math.Sin(Endvalue * Math.PI / 200);
                MyLines[I].Y2 = MyCanvas.ActualHeight / 2 - MyCircleRadius * Math.Cos(Endvalue * Math.PI / 200);
                MyLines[I].Stroke = MyBrushes[(int)ColorIndex];
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MyLoaded)
            {
                Init();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Started)
            {
                BtnStart.Content = "STOP";
                Started = true;
                Start();
            }
            else
            {
                BtnStart.Content = "START";
                Started = false;
            }
        }
    }
}