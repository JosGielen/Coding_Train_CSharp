using System.Windows;
using System.Windows.Media;

namespace Dragon_Curve
{
    public partial class MainWindow : Window
    {
        private LineSequence StartLine;
        private LineSequence NewLine;
        private bool App_Started = false;
        private bool Rotating = false;
        private double angle;
        private double endScale;
        private double scale;
        private int Iter;
        private double stepsize;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            canvas1.Children.Clear();
            StartLine = new LineSequence();
            StartLine.AddPoint(new Point(canvas1.ActualWidth / 2, 3.0 * canvas1.ActualHeight / 4));
            StartLine.AddPoint(new Point(canvas1.ActualWidth / 2, 2.0 * canvas1.ActualHeight / 4));
            StartLine.Draw(canvas1);
            NewLine = StartLine.Copy();
            NewLine.Pivot = NewLine.pLine.Points[1];
            NewLine.Draw(canvas1);
            angle = 0.0;
            endScale = 1.0;
            scale = 1.0;
            Iter = 0;
            stepsize = 100;
            Rotating = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!App_Started)
            {
                App_Started = true;
                Init();
                BtnStart.Content = "STOP";
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                App_Started = false;
                BtnStart.Content = "START";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (!Rotating)
            {
                //Duplicate the current curve
                NewLine = StartLine.Copy();
                NewLine.Pivot = NewLine.pLine.Points.Last();
                NewLine.Draw(canvas1);
                Rotating = true;
                angle = 0.0;
                scale = endScale;
                endScale = 1.01 * endScale / Math.Sqrt(2);
                StartLine.pLine.StrokeThickness += 2;
                Iter++;
                if (Iter > 16)
                {
                    App_Started = false;
                    BtnStart.Content = "START";
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                }
                Title = "Dragon Curve level: " + Iter.ToString() + " contains " + (StartLine.pLine.Points.Count - 1).ToString() + " lines";
            }
            else
            {
                //Rotate the copy CCW in small steps till 90° CCW
                NewLine.Rotate(-Math.PI / stepsize);
                angle += Math.PI / stepsize;
                double CurrentScale = scale - 2 * (scale - endScale) * angle / Math.PI;
                canvas1.RenderTransform = new ScaleTransform(CurrentScale, CurrentScale, canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
                if (angle > Math.PI / 2)
                {
                    //Correct any angle deviation from 90°
                    NewLine.Rotate(angle - Math.PI / 2);
                    //Add the copy in reverse to the StartLine (minus the pivot point)
                    for (int I = NewLine.pLine.Points.Count - 2; I >= 0; I--)
                    {
                        StartLine.pLine.Points.Add(NewLine.pLine.Points[I]);
                    }
                    if (Iter > 10) { stepsize -= 10; }
                    Rotating = false;
                }
            }
        }
    }
}