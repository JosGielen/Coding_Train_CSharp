using JG_Graphs;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Climate_Spiral
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec" };
        private List<double> tempData = new List<double>();
        private List<string> years = new List<string>();
        private List<Brush> myColors;
        private double MinTemp;
        private double MaxTemp;
        private double TempRange;
        private Point center;
        private double degreeSpace;
        private double angle;
        private Label YearLabel;
        private int currentYear;
        private int currentMonth;
        private double previousTemp;
        private Line ln;
        private LineSeries ls;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load the data
            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\Global Temp 1880 - 2023.txt");
            string[] line;
            while(!sr.EndOfStream )
            {
                line = sr.ReadLine().Split(',');
                years.Add(line[0]);
                for (int I = 1; I < 13; I++)
                {
                    if (line[I] != "***") tempData.Add(double.Parse(line[I]));
                }
            }
            previousTemp = tempData[0];
            currentMonth = 0;
            currentYear = 0;
            //Get the Temperature range
            MinTemp = double.MaxValue;
            MaxTemp = double.MinValue;
            for (int I = 0; I < tempData.Count; I++)
            {
                if (tempData[I] < MinTemp) MinTemp = tempData[I];
                if (tempData[I] > MaxTemp) MaxTemp = tempData[I];
            }
            TempRange = MaxTemp - MinTemp;
            //Get a color palette
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            myColors = pal.GetColorBrushes(256);
            //Prepare the LineGraph
            LineGraph1.DataSeries.Clear();
            ls = new LineSeries(1);
            ls.LineColor = Brushes.Red;
            ls.MarkerType = MarkerType.None;
            LineGraph1.XAxis.AxisLabel = "Time";
            LineGraph1.XAxis.AxisLabelFontSize = 14;
            LineGraph1.YAxis.AxisLabel = "Temperature";
            LineGraph1.YAxis.AxisLabelFontSize = 14;
            LineGraph1.YAxis.Minimum = -0.5;
            LineGraph1.YAxis.Maximum = 1.1;
            LineGraph1.YAxis.FixedMaximum = true;
            LineGraph1.LegendPosition = LegendPosition.None;
            LineGraph1.DataSeries.Add(ls);
            for (int I = 0; I < years.Count; I++)
            {
                LineGraph1.AddXTickLabel(years[I]);
            }
            LineGraph1.Draw();
            //Draw the Spiral outline
            degreeSpace = 120;
            center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            // -1° circle
            Ellipse El = new Ellipse()
            {
                Width = degreeSpace,
                Height = degreeSpace,
                Stroke = Brushes.Blue,
                StrokeThickness = 2.0
            };
            El.SetValue(Canvas.LeftProperty, center.X - El.Width / 2);
            El.SetValue(Canvas.TopProperty, center.Y - El.Height / 2);
            canvas1.Children.Add(El);
            Label l = new Label()
            {
                Content = "-1°",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background=Brushes.White
            };
            l.SetValue(Canvas.LeftProperty, center.X - 15);
            l.SetValue(Canvas.TopProperty, center.Y - degreeSpace / 2 - 15);
            canvas1.Children.Add(l);
            // 0° circle
            El = new Ellipse()
            {
                Width = 2 * degreeSpace,
                Height = 2 * degreeSpace,
                Stroke = Brushes.Green,
                StrokeThickness = 2.0
            };
            El.SetValue(Canvas.LeftProperty, center.X - El.Width / 2);
            El.SetValue(Canvas.TopProperty, center.Y - El.Height / 2);
            canvas1.Children.Add(El);
            l = new Label()
            {
                Content = "0°",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background = Brushes.White
            };
            l.SetValue(Canvas.LeftProperty, center.X - 10);
            l.SetValue(Canvas.TopProperty, center.Y - degreeSpace - 15);
            canvas1.Children.Add(l);
            // +1° circle
            El = new Ellipse()
            {
                Width = 3 * degreeSpace,
                Height = 3 * degreeSpace,
                Stroke = Brushes.Red,
                StrokeThickness = 2.0
            };
            El.SetValue(Canvas.LeftProperty, center.X - El.Width / 2);
            El.SetValue(Canvas.TopProperty, center.Y - El.Height / 2);
            canvas1.Children.Add(El);
            l = new Label()
            {
                Content = "+1°",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background = Brushes.White
            };
            l.SetValue(Canvas.LeftProperty, center.X - 15);
            l.SetValue(Canvas.TopProperty, center.Y - 3 * degreeSpace / 2 - 15);
            canvas1.Children.Add(l);
            //Year label
            YearLabel = new Label()
            {
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                Background = Brushes.White
            };
            YearLabel.SetValue(Canvas.LeftProperty, center.X - 35);
            YearLabel.SetValue(Canvas.TopProperty, center.Y - 25);
            canvas1.Children.Add(YearLabel);
            //Month labels
            angle = - Math.PI / 2;  //Start Januari at the 12 o'clock position
            for(int I = 0; I < months.Length; I++)
            {
                l = new Label()
                {
                    Content = months[I],
                    FontSize = 14,
                    FontWeight = FontWeights.Bold
                };
                l.SetValue(Canvas.LeftProperty, center.X + (3 * degreeSpace / 2 + 35) * Math.Cos(angle)-15);
                l.SetValue(Canvas.TopProperty, center.Y + (3 * degreeSpace / 2 + 35) * Math.Sin(angle)-15);
                canvas1.Children.Add(l);
                angle += 2 * Math.PI / 12;
            }
            angle = -Math.PI / 2; //Start the spiral at the 12 o'clock position
            CompositionTarget.Rendering += Render;
        }

        private void Render(object sender, EventArgs e)
        {
            if (currentMonth >= tempData.Count)
            {
                CompositionTarget.Rendering -= Render;
                return;
            }
            double currentTemp = tempData[currentMonth];
            Brush TempColor = myColors[(int)(255 - 255*(MaxTemp - currentTemp ) / TempRange)];
            if (currentMonth % 12 == 0)
            {
                double Avg = 0.0;
                int count = 0;
                for (int I = 0; I < 12; I++)
                {
                    if (currentMonth + I < tempData.Count )
                    {
                        Avg += tempData[currentMonth + I];
                        count++;
                    }
                }
                Avg /= count;
                Brush YearColor = myColors[(int)(255 - 255 * (MaxTemp - Avg) / TempRange)];
                YearLabel.Content = years[currentYear];
                YearLabel.Foreground = YearColor;
                ls.AddDataPoint(Avg);
                LineGraph1.Draw();
                currentYear++;
            }
            ln = new Line()
            {
                X1 = center.X + 0.5 * (previousTemp + 2) * degreeSpace * Math.Cos(angle),
                Y1 = center.Y + 0.5 * (previousTemp + 2) * degreeSpace * Math.Sin(angle),
                X2 = center.X + 0.5 * (currentTemp + 2)  * degreeSpace * Math.Cos(angle + Math.PI / 12),
                Y2 = center.Y + 0.5 * (currentTemp + 2)  * degreeSpace * Math.Sin(angle + Math.PI / 12),
                Stroke = TempColor,
                StrokeThickness = 1.0
            };
            canvas1.Children.Add(ln);
            currentMonth++;
            previousTemp = currentTemp;
            angle += Math.PI / 12;
        }
    }
}
