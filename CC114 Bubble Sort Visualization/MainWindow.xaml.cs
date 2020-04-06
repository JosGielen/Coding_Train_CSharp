using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BubbleSortVisualization
{
    public partial class MainWindow : Window
    {
        private int WaitTime = 1;
        private bool App_Started = false;
        private double[] values;
        private List<Line> lines;
        private List<Brush> myColors;
        private Random Rnd = new Random();
        private int LineWidth = 1;
        private DateTime StartTime;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            Line l;
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            values = new double[(int)(canvas1.ActualWidth / LineWidth)];
            lines = new List<Line>();
            myColors = pal.GetColorBrushes((int)canvas1.ActualHeight - 10);
            canvas1.Children.Clear();
            WaitTime = (int)SldWaitTime.Value;
            for (int I = 0; I < values.Length; I++)
            {
                values[I] = (canvas1.ActualHeight - 20) * Rnd.NextDouble();
                l = new Line()
                {
                    X1 = LineWidth * I,
                    Y1 = canvas1.ActualHeight,
                    X2 = LineWidth * I,
                    Y2 = canvas1.ActualHeight - values[I],
                    Stroke = myColors[(int)values[I]],
                    StrokeThickness = LineWidth
                };
                lines.Add(l);
                canvas1.Children.Add(l);
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!App_Started)
            {
                BtnStart.Content = "STOP";
                App_Started = true;
                Init();
                StartTime = DateTime.Now;
                BubbleSort();
            }
            else
            {
                BtnStart.Content = "START";
                App_Started = false;
            }
        }

        private void BubbleSort()
        {
            int last = values.Length - 1;
            while (App_Started)
            {
                for (int I = 0; I < last; I++)
                {
                    if (values[I] > values[I + 1])
                    {
                        SwapItems(I, I + 1);
                    }
                }
                last -= 1;
                if (last < 2)
                {
                    TxtTime.Text = ((DateTime.Now - StartTime).TotalMilliseconds / 1000).ToString("F2");
                    BtnStart.Content = "START";
                    App_Started = false;
                }
            }

        }

        private void SwapItems(int a, int b)
        {
            double temp = values[a];
            values[a] = values[b];
            values[b] = temp;
            lines[a].Y2 = canvas1.ActualHeight - values[a];
            lines[a].Stroke = myColors[(int)values[a]];
            lines[b].Y2 = canvas1.ActualHeight - values[b];
            lines[b].Stroke = myColors[(int)values[b]];
            Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
        }

        private void Wait()
        { 
            Thread.Sleep(WaitTime);
        }

        private void SldWaitTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WaitTime = (int)SldWaitTime.Value;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
