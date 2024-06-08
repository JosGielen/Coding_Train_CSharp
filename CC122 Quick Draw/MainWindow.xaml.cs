using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quick_Draw
{
    public partial class MainWindow : Window
    {
        private StreamReader my_Reader;
        private string QDFile;
        private string QDLine;
        private Polyline Pline;
        List<Polyline> polylines;
        private int StrokeCounter;
        private int PointCounter;
        private double LastX;
        private double LastY;
        private Line l;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            QDFile = Environment.CurrentDirectory + "\\QuickDraw Files\\full_simplified_cat.ndjson";
            my_Reader = new StreamReader(QDFile);
            if (!my_Reader.EndOfStream) { QDLine = my_Reader.ReadLine(); }
            ShowDrawing();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (!my_Reader.EndOfStream) 
            {
                do
                {
                    QDLine = my_Reader.ReadLine();
                } while(QDLine.Contains("recognized\":false"));
            }
            ShowDrawing();
        }

        private void ShowDrawing()
        {
            BtnNext.IsEnabled = false;
            polylines = new List<Polyline>();
            canvas1.Children.Clear();
            int startIndex = QDLine.IndexOf("\"drawing\":") + 12;
            string QDrawing = QDLine.Substring(startIndex);
            string[] strokes = QDrawing.Split("],[");
            for (int i = 0; i < strokes.Length; i++)
            {
                strokes[i] = strokes[i].Replace("]", "");
                strokes[i] = strokes[i].Replace("[", "");
                strokes[i] = strokes[i].Replace("}", "");
            }
            string[] Xdata;
            string[] Ydata;
            double X, Y;
            double maxX = 0.0;
            double maxY = 0.0;
            for (int i = 0; i < strokes.Length; i += 2)
            {
                Xdata = strokes[i].Split(',');
                Ydata = strokes[i + 1].Split(',');
                if (Xdata.Length != Ydata.Length)
                {
                    Debug.Print("Data mismatch!!");
                    return;
                }
                Pline = new Polyline()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.0,
                };
                for (int j = 0; j < Xdata.Length; j++)
                {
                    X = double.Parse(Xdata[j]);
                    Y = double.Parse(Ydata[j]);
                    if (X > maxX) { maxX = X; }
                    if (Y > maxY) { maxY = Y; }
                    Pline.Points.Add(new Point(X, Y));
                }
                polylines.Add(Pline);
            }
            double scale = Math.Min(canvas1.ActualWidth / maxX, canvas1.ActualHeight / maxY);
            for (int i = 0; i < polylines.Count; i++)
            {
                for (int j = 0; j < polylines[i].Points.Count; j++)
                {
                    polylines[i].Points[j] = new Point(scale * polylines[i].Points[j].X, scale * polylines[i].Points[j].Y);
                }
            }
            StrokeCounter = 0;
            LastX = polylines[0].Points[0].X;
            LastY = polylines[0].Points[0].Y;
            PointCounter = 1;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            double X = polylines[StrokeCounter].Points[PointCounter].X;
            double Y = polylines[StrokeCounter].Points[PointCounter].Y;
            l = new Line()
            {
                X1 = LastX,
                Y1 = LastY,
                X2 = X,
                Y2 = Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0
            };
            canvas1.Children.Add(l);
            PointCounter++;
            if (PointCounter >= polylines[StrokeCounter].Points.Count)
            {
                PointCounter = 0;
                StrokeCounter++;
                if (StrokeCounter >= polylines.Count)
                {
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                    BtnNext.IsEnabled = true;
                    return;
                }
                LastX = polylines[StrokeCounter].Points[PointCounter].X;
                LastY = polylines[StrokeCounter].Points[PointCounter].Y;
                return;
            }
            LastX = X;
            LastY = Y;
            Thread.Sleep(100);
        }

        private void MnuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "ndjson Files (*.ndjson)|*.ndjson|All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
            };
            if (OFD.ShowDialog() == true)
            {
                QDFile = OFD.FileName;
                my_Reader = new StreamReader(QDFile);
                if (!my_Reader.EndOfStream) { QDLine = my_Reader.ReadLine(); }
                ShowDrawing();
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}