using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace QuickSortVisualization
{
    public partial class MainWindow : Window
    {
        public delegate void SwapDelegate(int a, int b);
        public delegate void FinishDelegate(); //Used to detect when the multiThreaded version has finished
        private int WaitTime = 1;
        private bool App_Started = false;
        private double[] values;
        private List<Line> lines;
        private List<Brush> myColors;
        private Random Rnd = new Random();
        private int LineWidth = 1;
        private bool Multithreaded;
        private DateTime StartTime;
        private int Pivotcount;  //Used to detect when the multiThreaded version has finished

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            Line l;
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            Pivotcount = 0;
            values = new double[(int)(canvas1.ActualWidth / LineWidth)];
            lines = new List<Line>();
            myColors = pal.GetColorBrushes((int)canvas1.ActualHeight - 10);
            canvas1.Children.Clear();
            WaitTime = (int)SldWaitTime.Value;
            Multithreaded = CbMulti.IsChecked.Value;
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
                if (Multithreaded)
                {
                    Thread T = new Thread(() => QuickSort(0, values.Length - 1));
                    T.Start();
                }
                else
                {
                    QuickSort(0, values.Length - 1);
                    BtnStart.Content = "START";
                    App_Started = false;
                }
            }
            else
            {
                BtnStart.Content = "START";
                App_Started = false;
            }
        }

        private void Finish()
        {
            TxtTime.Text = ((DateTime.Now - StartTime).TotalMilliseconds / 1000).ToString("F2");
            BtnStart.Content = "START";
            App_Started = false;
        }

        private bool QuickSort(int first, int last)
        {
            if (!App_Started | first >= last) return true;
            int index = Partition(first, last);
            Pivotcount += 1;
            if (Multithreaded)
            {
                Thread T1 = new Thread(() => QuickSort(first, index - 1));
                Thread T2 = new Thread(() => QuickSort(index + 1, last));
                T1.Start();
                T2.Start();
                T1.Join();
                T2.Join();
            }
            else
            {
                QuickSort(first, index - 1);
                QuickSort(index + 1, last);
            }

            Pivotcount -= 1;
            if (Pivotcount == 0)
            {
                //Finished;
                Dispatcher.Invoke(DispatcherPriority.SystemIdle, new FinishDelegate(Finish));
            }
            return true;
        }

        private int Partition(int first, int last)
        {
            double pivotValue = values[last];
            int pivotIndex = first;
            for (int I = first; I < last; I++)
            {
                if (values[I] < pivotValue)
                {
                    Dispatcher.Invoke(DispatcherPriority.SystemIdle, new SwapDelegate(SwapItems), I, pivotIndex);
                    pivotIndex += 1;
                }
            }
            Dispatcher.Invoke(DispatcherPriority.SystemIdle, new SwapDelegate(SwapItems), pivotIndex, last);
            return pivotIndex;
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
            Thread.Sleep(WaitTime);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SldWaitTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!App_Started) return;
            WaitTime = (int)SldWaitTime.Value;
        }
    }
}