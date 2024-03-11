using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Particle_Clock
{
    public partial class MainWindow : Window
    {
        private Random Rnd = new Random();
        private int Hrptsnum = 100;
        private int Minptsnum = 100;
        private int Secptsnum = 80;
        private string PrevHr;
        private string PrevMin;
        private string PrevSec;
        private Symbol Hrsym;
        private Symbol Minsym;
        private Symbol Secsym;
        private List<Agent> HrAgents;
        private List<Agent> MinAgents;
        private List<Agent> SecAgents;
        private double w;
        private double h;
        private Point midPt;
        private Path HrPath;
        private PathGeometry HrPG;
        private PathFigure Hrfigure;
        private ArcSegment HrSeg;
        private Path MinPath;
        private PathGeometry MinPG;
        private PathFigure Minfigure;
        private ArcSegment MinSeg;
        private Path SecPath;
        private PathGeometry SecPG;
        private PathFigure Secfigure;
        private ArcSegment SecSeg;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            //Set the Canvas background
            BitmapImage bmp;
            ImageBrush imgbrush;
            bmp = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Clockface.png"));
            imgbrush = new ImageBrush
            {
                Stretch = Stretch.Fill,
                ImageSource = bmp
            };
            Canvas1.Background = imgbrush;
            w = Canvas1.ActualWidth;
            h = Canvas1.ActualHeight;
            midPt = new Point(w / 2, h / 2);
            //Set the Hours Arc
            HrPath = new Path()
            {
                Stroke = Brushes.Green,
                StrokeThickness = 8
            };
            HrPG = new PathGeometry();
            Hrfigure = new PathFigure()
            {
                StartPoint = new Point(w / 2, 5)
            };
            HrSeg = new ArcSegment()
            {
                SweepDirection = SweepDirection.Clockwise,
                IsStroked = true
            };
            Hrfigure.Segments.Add(HrSeg);
            HrPG.Figures.Add(Hrfigure);
            HrPath.Data = HrPG;
            Canvas1.Children.Add(HrPath);
            //Set the Minutes Arc
            MinPath = new Path()
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 8
            };
            MinPG = new PathGeometry();
            Minfigure = new PathFigure()
            {
                StartPoint = new Point(w / 2, 13)
            };
            MinSeg = new ArcSegment()
            {
                SweepDirection = SweepDirection.Clockwise,
                IsStroked = true
            };
            Minfigure.Segments.Add(MinSeg);
            MinPG.Figures.Add(Minfigure);
            MinPath.Data = MinPG;
            Canvas1.Children.Add(MinPath);
            //Set the Seconds Arc
            SecPath = new Path()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 8
            };
            SecPG = new PathGeometry();
            Secfigure = new PathFigure()
            {
                StartPoint = new Point(w / 2, 21)
            };
            SecSeg = new ArcSegment()
            {
                SweepDirection = SweepDirection.Clockwise,
                IsStroked = true
            };
            Secfigure.Segments.Add(SecSeg);
            SecPG.Figures.Add(Secfigure);
            SecPath.Data = SecPG;
            Canvas1.Children.Add(SecPath);
            //Set the digital ParticleClock
            PrevHr = "";
            PrevMin = "";
            PrevSec = "";
            Agent agt;
            Hrsym = new Symbol("")
            {
                Fill = Brushes.LightGray,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1.0,
                FontFamily = new FontFamily("Arial"),
                FontSize = 40,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Bold,
                Origin = new Point(90, 150)
            };
            HrAgents = new List<Agent>();
            for (int I = 0; I <= Hrptsnum; I++)
            {
                agt = new Agent(midPt, 1.0, 2.0, 2.0)
                {
                    Size = 3,
                    Color = Brushes.Green,
                    Breakingdistance = 50
                };
                agt.Draw(Canvas1);
                HrAgents.Add(agt);
            }
            Minsym = new Symbol("")
            {
                Fill = Brushes.LightGray,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1.0,
                FontFamily = new FontFamily("Arial"),
                FontSize = 40,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Bold,
                Origin = new Point(170, 150)
            };
            MinAgents = new List<Agent>();
            for (int I = 0; I <= Minptsnum; I++)
            {
                agt = new Agent(midPt, 1.0, 3.0, 3.0)
                {
                    Size = 3,
                    Color = Brushes.Blue,
                    Breakingdistance = 50
                };
                agt.Draw(Canvas1);
                MinAgents.Add(agt);
            }
            Secsym = new Symbol("")
            {
                Fill = Brushes.LightGray,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1.0,
                FontFamily = new FontFamily("Arial"),
                FontSize = 40,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Bold,
                Origin = new Point(250, 150)
            };
            SecAgents = new List<Agent>();
            for (int I = 0; I <= Secptsnum; I++)
            {
                agt = new Agent(midPt, 1.0, 4.0, 4.0)
                {
                    Size = 3,
                    Color = Brushes.Red,
                    Breakingdistance = 20
                };
                agt.Draw(Canvas1);
                SecAgents.Add(agt);
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void SetTargets(string Hour, string Minute, string Second)
        {
            Point pt;
            Point tangent;
            PathGeometry geo;
            Size arcsize;
            Point endpt;
            int hrs;
            int mins;
            int secs;
            double angle;
            //Update the Clock
            if (Hour != PrevHr)
            {
                Hrsym.Text = Hour + ":";
                geo = Hrsym.Geometry.GetFlattenedPathGeometry();
                for (int I = 0; I <= Hrptsnum; I++)
                {
                    geo.GetPointAtFractionLength(I / (double)Hrptsnum, out pt, out tangent);
                    HrAgents[I].Location = new Vector(90 + 60 * Rnd.NextDouble(), 150 + 50 * Rnd.NextDouble());
                    HrAgents[I].SetTarget(pt);
                }
                hrs = int.Parse(Hour);
                angle = (hrs % 12) * Math.PI / 6 - Math.PI / 2;
                arcsize = new Size(w / 2 - 5, h / 2 - 5);
                endpt = new Point(midPt.X + arcsize.Width * Math.Cos(angle), midPt.Y + arcsize.Height * Math.Sin(angle));
                HrSeg.Point = endpt;
                HrSeg.Size = arcsize;
                HrSeg.RotationAngle = angle;
                HrSeg.IsLargeArc = (hrs % 12) > 6;
                PrevHr = Hour;
            }
            if (Minute != PrevMin)
            {
                Minsym.Text = Minute + ":";
                geo = Minsym.Geometry.GetFlattenedPathGeometry();
                for (int I = 0; I <= Minptsnum; I++)
                {
                    geo.GetPointAtFractionLength(I / (double)Minptsnum, out pt, out tangent);
                    MinAgents[I].Location = new Vector(170 + 60 * Rnd.NextDouble(), 150 + 50 * Rnd.NextDouble());
                    MinAgents[I].SetTarget(pt);
                }
                mins = int.Parse(Minute);
                angle = mins * Math.PI / 30 - Math.PI / 2;
                arcsize = new Size(w / 2 - 13, h / 2 - 13);
                endpt = new Point(midPt.X + arcsize.Width * Math.Cos(angle), midPt.Y + arcsize.Height * Math.Sin(angle));
                MinSeg.Point = endpt;
                MinSeg.Size = arcsize;
                MinSeg.RotationAngle = angle;
                MinSeg.IsLargeArc = mins > 30;
                PrevMin = Minute;
            }
            if (Second != PrevSec)
            {
                Secsym.Text = Second;
                geo = Secsym.Geometry.GetFlattenedPathGeometry();
                for (int I = 0; I <= Secptsnum; I++)
                {
                    geo.GetPointAtFractionLength(I / (double)Secptsnum, out pt, out tangent);
                    SecAgents[I].Location = new Vector(250 + 60 * Rnd.NextDouble(), 150 + 50 * Rnd.NextDouble());
                    SecAgents[I].SetTarget(pt);
                }
                secs = int.Parse(Second);
                angle = secs * Math.PI / 30 - Math.PI / 2;
                arcsize = new Size(w / 2 - 21, h / 2 - 21);
                endpt = new Point(midPt.X + arcsize.Width * Math.Cos(angle), midPt.Y + arcsize.Height * Math.Sin(angle));
                SecSeg.Point = endpt;
                SecSeg.Size = arcsize;
                SecSeg.RotationAngle = angle;
                SecSeg.IsLargeArc = secs > 30;
                PrevSec = Second;
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            SetTargets(DateTime.Now.Hour.ToString("00"), DateTime.Now.Minute.ToString("00"), DateTime.Now.Second.ToString("00"));
            for (int I = 0; I < HrAgents.Count; I++)
            {
                HrAgents[I].Update();
            }
            for (int I = 0; I < MinAgents.Count; I++)
            {
                MinAgents[I].Update();
            }
            for (int I = 0; I < SecAgents.Count; I++)
            {
                SecAgents[I].Update();
            }
        }
    }
}