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
using System.Windows.Threading;

namespace CalculateRndPi
{
    public partial class MainWindow
    {
        private delegate void RenderDelegate();
        private double CalculatedPi;
        private double Counter = 0;
        private double Coprimes = 0;
        private int N1 = 0;
        private int N2 = 0;
        private readonly int MaxN = 100000000;
        private Graphs.ScatterSeries PiCalc1;
        private Graphs.ScatterSeries PiValue;
        private bool started = false;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Random Rnd = new Random();
            if (!started)
            {
                Counter = 0;
                Coprimes = 0;
                PieGraph.DataSeries.Clear();
                PieGraph.LegendPosition = Graphs.GraphData.LegendPosition.Top;
                PiCalc1 = new Graphs.ScatterSeries(0);
                PiCalc1.SeriesName = "Pie Estimate";
                PiCalc1.LineColor = Brushes.Red;
                PiCalc1.MarkerType = Graphs.GraphData.MarkerType.None;
                PiValue = new Graphs.ScatterSeries(1);
                PiValue.SeriesName = "Pie Value";
                PiValue.LineColor = Brushes.Blue;
                PiValue.MarkerType = Graphs.GraphData.MarkerType.None;
                PieGraph.DataSeries.Add(PiCalc1);
                PieGraph.DataSeries.Add(PiValue);

                PieGraph.YAxis.Minimum = 3.12;
                PieGraph.YAxis.Maximum = 3.16;
                PieGraph.XAxis.Minimum = 0;
                PieGraph.XAxis.Maximum = 500;
                PieGraph.XAxis.FixedMaximum = true;


                BtnStart.Content = "STOP";
                started = true;
            }
            else
            {
                BtnStart.Content = "START";
                started = false;
            }
            while (started)
            {
                N1 = Rnd.Next(MaxN);
                N2 = Rnd.Next(MaxN);
                if (N1 > 0 & N2 > 0)
                {
                    Counter += 1;
                    if (GGD(N1, N2) == 1) Coprimes += 1;
                    if (Coprimes > 0)
                    {
                        if (Counter % 1000 == 0)
                        {
                            CalculatedPi = Math.Sqrt(6 * Counter / Coprimes);
                            PiCalc1.AddDataPoint(new Point(Counter / 1000, CalculatedPi));
                            PiValue.AddDataPoint(new Point(Counter / 1000, Math.PI));
                            Dispatcher.Invoke(Redraw, DispatcherPriority.ApplicationIdle);
                        }
                        if (Counter > 500000) PieGraph.XAxis.FixedMaximum = false;
                        if (Counter > 3000000)
                        {
                            PiCalc1.DataList.RemoveAt(0);
                            PiValue.DataList.RemoveAt(0);
                            continue;
                        }
                    }
                }
            }
        }

        private int GGD(int Num1, int Num2)
        {
            int Dummy = 0;
            if (Num1 == Num2) return Num1;
            if (Num1 < Num2)
            {
                Dummy = Num1;
                Num1 = Num2;
                Num2 = Dummy;
            }
            do
            {
                Dummy = Num1;
                Num1 = Num2;
                Num2 = Dummy % Num2;
            } while (Num2 > 0);
            return Num1;
        }

        private void Redraw()
        {
            TxtNumber.Text = Counter.ToString();
            TxtEstimate.Text = CalculatedPi.ToString();
            PieGraph.Draw();
            //Thread.Sleep(50)
        }

        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

