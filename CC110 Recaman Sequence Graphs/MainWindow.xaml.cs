using JG_Graphs;
using System.Windows;
using System.Windows.Media;

namespace Recaman_Sequence
{
    public partial class MainWindow : Window
    {
        private List<int> Recaman;
        private ScatterSeries scatSeries1;
        private BarSeries barSeries1;
        private int StepSize = 1;
        private int current;
        private bool Started = false;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Started = false;
        }

        private void Init()
        {
            barGraph.DataSeries.Clear();
            scatterGraph.DataSeries.Clear();
            Recaman = new List<int>();
            StepSize = 1;
            current = 0;
            Recaman.Add(current);
            //Initialize the ScatterGraph
            scatSeries1 = new ScatterSeries(0);
            scatSeries1.AddDataPoint(new Point(0, 0));
            scatSeries1.SeriesName = "Recaman Sequence";
            scatSeries1.LineColor = Brushes.Red;
            scatSeries1.MarkerType = MarkerType.None;
            scatterGraph.LegendPosition = LegendPosition.Top;
            scatterGraph.DataSeries.Add(scatSeries1);
            //Initialize the BarGraph
            barSeries1 = new BarSeries(0);
            barSeries1.SeriesName = "Numbers used in the Recaman Sequence";
            barSeries1.LineColor = Brushes.Blue;
            barSeries1.FillColor = Brushes.Blue;
            //Add the BarGraph X-axis tick labels
            for (int I = 0; I <= 5000; I++)
            {
                barGraph.AddXTickLabel(I.ToString());
            }
            barGraph.LegendPosition = LegendPosition.Top;
            barGraph.DataSeries.Add(barSeries1);
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
            current = NextRecaman();
            Recaman.Add(current);
            //Update the ScatterGraph
            scatSeries1.AddDataPoint(new Point(StepSize, current));
            scatterGraph.Draw();
            StepSize++;
            if (StepSize > 2000)
            {
                BtnStart.Content = "START";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                //Update the BarGraph
                barSeries1.DataList.Clear();
                for (int I = 0; I <= 5000; I++)
                {
                    if (Recaman.Contains(I))
                    {
                        barSeries1.AddDataPoint(Recaman.IndexOf(I));
                    }
                    else
                    {
                        barSeries1.AddDataPoint(0.0);
                    }
                }
                barGraph.Draw();
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