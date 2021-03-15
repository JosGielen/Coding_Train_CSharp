using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace CalculatePi
{
    public partial class MainWindow : Window
    {
        //private delegate void RenderDelegate();
        private double CalculatedPie;
        private double CalculatedE;
        private int number = 1;
        private Graphs.ScatterSeries PiCalc;
        private Graphs.ScatterSeries ECalc;
        private Graphs.ScatterSeries PiValue;
        private Graphs.ScatterSeries EValue;
        private bool started = false;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PieGraph.YAxis.Minimum = 3.0;
            PieGraph.YAxis.Maximum = 3.3;
            PieGraph.XAxis.Minimum = 0;
            PieGraph.XAxis.FixedMinimum = true;
            eGraph.YAxis.Minimum = 2.6;
            eGraph.YAxis.Maximum = 2.8;
            eGraph.XAxis.Minimum = 0;
            eGraph.XAxis.FixedMinimum = true;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            bool Add = true;
            if (!started)
            {
                number = 1;
                CalculatedPie = 0.0;
                CalculatedE = 1.0;
                PieGraph.DataSeries.Clear();
                PieGraph.LegendPosition = Graphs.GraphData.LegendPosition.Top;
                eGraph.DataSeries.Clear();
                eGraph.LegendPosition = Graphs.GraphData.LegendPosition.Top;
                PiCalc = new Graphs.ScatterSeries(0);
                PiCalc.SeriesName = "Pie Estimate";
                PiCalc.LineColor = Brushes.Red;
                PiCalc.MarkerType = Graphs.GraphData.MarkerType.None;
                PiValue = new Graphs.ScatterSeries(1);
                PiValue.SeriesName = "Pie Value";
                PiValue.LineColor = Brushes.Blue;
                PiValue.MarkerType = Graphs.GraphData.MarkerType.None;
                ECalc = new Graphs.ScatterSeries(2);
                ECalc.SeriesName = "e Estimate";
                ECalc.LineColor = Brushes.Purple;
                ECalc.MarkerType = Graphs.GraphData.MarkerType.None;
                EValue = new Graphs.ScatterSeries(3);
                EValue.SeriesName = "e Value";
                EValue.LineColor = Brushes.Green;
                EValue.MarkerType = Graphs.GraphData.MarkerType.None;
                PieGraph.DataSeries.Add(PiValue);
                PieGraph.DataSeries.Add(PiCalc);
                eGraph.DataSeries.Add(EValue);
                eGraph.DataSeries.Add(ECalc);
                BtnStart.Content = "STOP";
                started = true;
            }
            else
            {
                BtnStart.Content = "START";
                started = false;
                if (PieGraph.DataSeries[1].DataList.Count >= 10)
                {
                    double averagePi = 0;
                    Point[] estimates = PieGraph.DataSeries[1].DataList.ToArray();
                    for (int I = 0; I < 10; I++)
                    {
                        averagePi += estimates[estimates.Length  - I - 1].Y;
                    }
                    averagePi /= 10;
                    TxtEstimate.Text = averagePi.ToString();
                }
            }
            while (started)
            {
                if (Add)
                {
                    CalculatedPie += 1.0 / number;
                }
                else
                {
                    CalculatedPie -= 1.0 / number;
                }
                CalculatedE = 1.0;
                for (int I = 1; I <= number; I++)
                {
                    CalculatedE *= (1.0 + 1.0 / number);
                }
                Add = !Add;
                PiCalc.AddDataPoint(new Point(number, 4.0 * CalculatedPie));
                ECalc.AddDataPoint(new Point(number, CalculatedE));
                PiValue.AddDataPoint(new Point(number, Math.PI));
                EValue.AddDataPoint(new Point(number, Math.E));
                Dispatcher.Invoke(Redraw, DispatcherPriority.ApplicationIdle);
                number += 2;
                if (number > 2000)
                {
                    BtnStart.Content = "START";
                    started = false;
                    continue;
                }
            }
        }

        private void Redraw()
        {
            if (started )
            { 
                TxtNumber.Text = (number + 1).ToString();
                TxtEstimate.Text = (4.0 * CalculatedPie).ToString();
                TxtEstimate2.Text = CalculatedE.ToString();
                if (number % 10 == 1)
                {
                    PieGraph.Draw();
                    eGraph.Draw();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
