using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Recaman_Sequence
{
    public partial class MainWindow : Window
    {
        private List<int> Recaman;
        private int StepSize = 1;
        private int last;
        private int current;
        Path Arc;
        PathGeometry ArcPG;
        PathFigure ArcFigure;
        ArcSegment ArcSeg;
        private bool down;
        private bool Started = false;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            down = true;
            Started = false;
        }

        private void Init()
        {
            canvas1.Children.Clear();
            Recaman = new List<int>();
            //Create a Path that contains a PathGeometry that contains a PathFigure that contains all the ArcSegments
            //A Path is a UIElement and can be drawn in a Canvas.
            //PathGeometry, PathFigure and ArcSegment are System.Windows.Media elements and can therefore not be drawn in a Canvas.
            Arc = new Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1.0
            };
            ArcPG = new PathGeometry();
            ArcFigure = new PathFigure();
            ArcFigure.StartPoint = new Point(0.0, canvas1.ActualHeight / 2);
            ArcPG.Figures.Add(ArcFigure);
            Arc.Data = ArcPG;
            canvas1.Children.Add(Arc);
            StepSize = 1;
            last = 0;
            current = 0;
            Recaman.Add(current);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Started)
            {
                BtnStart.Content = "STOP";
                Started = true;
                Init();
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                BtnStart.Content = "START";
                Started = false;
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            double W;
            last = current;
            current = NextRecaman();
            Recaman.Add(current);
            StepSize++;
            W = Math.Abs(current - last);
            //Draw the Recaman Sequence as arcs.
            ArcSeg = new ArcSegment();
            ArcSeg.Point = new Point(2 * current, canvas1.ActualHeight / 2);
            ArcSeg.Size = new Size(0.5 * W , 0.5 * W);
            if (current > last)
            {
                if (down)
                {
                    ArcSeg.SweepDirection = SweepDirection.Counterclockwise;
                }
                else
                {
                    ArcSeg.SweepDirection = SweepDirection.Clockwise;
                }
            }
            else
            {
                if (down)
                {
                    ArcSeg.SweepDirection = SweepDirection.Clockwise;
                }
                else
                {
                    ArcSeg.SweepDirection = SweepDirection.Counterclockwise;
                }
            }
            ArcFigure.Segments.Add(ArcSeg);
            down = !down;
            if (StepSize > 170)
            {
                BtnStart.Content = "START";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
            Thread.Sleep(10);
        }

        private int NextRecaman()
        {
            int result = current - StepSize;
            if (result < 0 || Recaman.Contains(result))
            {
                result = current + StepSize;
            }
            return result;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}