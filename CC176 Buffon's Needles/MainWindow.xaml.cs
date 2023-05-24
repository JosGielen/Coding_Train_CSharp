using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using JG_Graphs;

namespace Buffon_s_Needles
{
    public partial class MainWindow : Window
    {
        private bool Started = false;
        private int WaitTime = 2;
        private int Total;
        private int Crossed;
        private double lineSpacing;
        private double pickLength;
        private List<double> LineX;
        private ScatterSeries ss;
        private Random Rnd = new Random();

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
            Total = 0;
            Crossed = 0;
            lineSpacing = canvas1.ActualWidth / 15;
            pickLength = SldRatio.Value * lineSpacing;
            TxtRatio.Text = SldRatio.Value.ToString();
            SldRatio.IsEnabled = false;
            //Prepare the Canvas
            canvas1.Children.Clear();
            LineX = new List<double>();
            Line l;
            for (double X = 0; X < canvas1.ActualWidth + 1; X += lineSpacing)
            {
                l = new Line()
                {
                    X1 = X,
                    Y1 = 0.0,
                    X2 = X,
                    Y2 = canvas1.ActualHeight,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2
                };
                canvas1.Children.Add(l);
                LineX.Add(X);
            }
            //Prepare the ScatterGraph
            scatter1.DataSeries.Clear();
            ss = new ScatterSeries(1);
            ss.LineColor = Brushes.Red;
            ss.MarkerType = MarkerType.None;
            scatter1.XAxis.AxisLabel = "Number of Toothpicks";
            scatter1.XAxis.AxisLabelFontSize = 14;
            scatter1.YAxis.AxisLabel = "PI Estimate";
            scatter1.YAxis.AxisLabelFontSize = 14;
            scatter1.YAxis.Maximum = 4.0;
            scatter1.YAxis.FixedMaximum = true;
            ss.AddDataPoint(new Point(0, 0));
            scatter1.LegendPosition = LegendPosition.None;
            scatter1.DataSeries.Add(ss);
            scatter1.Draw();
        }

        private void SldRatio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            pickLength = SldRatio.Value * lineSpacing;
            TxtRatio.Text = SldRatio.Value.ToString();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Started)
            {
                Started = true;
                BtnStart.Content = "STOP";
                Init();
                Render();
            }
            else
            {
                Started = false;
                BtnStart.Content = "START";
                SldRatio.IsEnabled = true;
            }
        }

        private void Render()
        {
            Line pick;
            Vector startPt;
            Vector endPt;
            Vector centerPt;
            double ratio;
            double PiEstimate = 0.0;
            while (Started)
            {
                startPt = new Vector(1000 * Rnd.NextDouble(), 1000 * Rnd.NextDouble());
                endPt = new Vector(1000 * Rnd.NextDouble(), 1000 * Rnd.NextDouble());
                centerPt = new Vector(canvas1.ActualWidth * Rnd.NextDouble(), canvas1.ActualHeight * Rnd.NextDouble());
                //Move the startPt and endPt so the length = pickLength
                double RC = (endPt.Y - startPt.Y) / (endPt.X - startPt.X);
                double dX = pickLength / 2 * Math.Sqrt(1 / (1 + RC * RC));
                double dY = RC * dX;
                startPt = new Vector(centerPt.X - dX, centerPt.Y - dY);
                endPt = new Vector(centerPt.X + dX, centerPt.Y + dY);

                Total++;
                TxtTotal.Text = Total.ToString();
                if (Total >= 50000)
                {
                    Started = false;
                    BtnStart.Content = "START";
                    SldRatio.IsEnabled = true;
                }
                //Check if the toothpick crosses a line
                if (LineCrossing(startPt, endPt))
                {
                    Crossed++;
                }
                //Only draw 1 in 5 toothpicks
                if (Total % 5 == 0)
                {
                    pick = new Line()
                    {
                        X1 = centerPt.X - dX,
                        Y1 = centerPt.Y - dY,
                        X2 = centerPt.X + dX,
                        Y2 = centerPt.Y + dY,
                        Stroke = Brushes.White,
                        StrokeThickness = 1.0
                    };
                    if (LineCrossing(startPt, endPt))
                    {
                        pick.Stroke = Brushes.Lime;
                    }
                    canvas1.Children.Add(pick);
                    //Calculate and show the PI Estimate
                    ratio = (double)Crossed / Total;
                    PiEstimate = 2 * pickLength / (ratio * lineSpacing);
                    TxtPiEstimate.Text = PiEstimate.ToString();
                    Dispatcher.Invoke(Wait, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                }
                //Update the graph every 50 toothpicks
                if (Total % 50 == 0)
                {
                    ss.AddDataPoint(new Point(Total, PiEstimate));
                    scatter1.Draw();
                }
            }
        }

        private bool LineCrossing(Vector start, Vector end)
        {
            for (int I = 0; I < LineX.Count; I++)
            {
                if (Math.Sign(start.X - LineX[I]) != Math.Sign(end.X - LineX[I])) return true;
            }
            return false;
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

    }
}
