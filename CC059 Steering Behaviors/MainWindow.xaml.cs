using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Steering_Behaviors
{
    public partial class MainWindow : Window
    {
        private bool Rendering;
        private Symbol sym;
        private List<Agent> Agents;
        private Random Rnd = new Random();
        private int num = 500;
        private List<Brush> my_brushes;
        private bool UseColor = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sym = new Symbol("")
            {
                Fill = Brushes.LightGray,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1.0,
                FontFamily = new FontFamily("Arial"),
                FontSize = 100,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Bold,
                Origin = new Point(100, 200)
            };
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            my_brushes = pal.GetColorBrushes(256);
            Init();
        }

        private void Init()
        {
            Agent agt;
            Agents = new List<Agent>();
            for (int I = 0; I <= num; I++)
            {
                agt = new Agent(new Point(Canvas1.ActualWidth * Rnd.NextDouble(), Canvas1.ActualHeight * Rnd.NextDouble()), 1.0, 3.0, 4.0, Brushes.Black);
                agt.Size = 4;
                agt.Breakingdistance = 100;
                agt.Draw(Canvas1);
                Agents.Add(agt);
            }
            SetTargets();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            SetTargets();
        }

        private void CbUseColor_Click(object sender, RoutedEventArgs e)
        {
            UseColor = CbUseColor.IsChecked.Value;
            for (int I = 0; I <= num; I++)
            {
                if (UseColor)
                {
                    Canvas1.Background = Brushes.Black;
                    Agents[I].Color = my_brushes[I % 256];
                }
                else
                {
                    Canvas1.Background = Brushes.Beige;
                    Agents[I].Color = Brushes.Black;
                }
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                StartRender();
            }
            else
            {
                StopRender();
            }
        }

        private void StartRender()
        {
            //Some initial code
            for (int I = 0; I < Agents.Count; I++)
            {
                Agents[I].Location = new Vector(Canvas1.ActualWidth * Rnd.NextDouble(), Canvas1.ActualHeight * Rnd.NextDouble());
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            BtnStart.Content = "Stop";
            Rendering = true;
        }

        private void StopRender()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            BtnStart.Content = "Start";
            Rendering = false;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            FleeFromMouse();
            for (int I = 0; I < Agents.Count; I++)
            {
                Agents[I].Update();
            }
        }

        private void FleeFromMouse()
        {
            Vector mouseloc;
            if (Canvas1.IsMouseOver)
            {
                mouseloc = (Vector)Mouse.GetPosition(Canvas1);
                for (int I = 0; I < Agents.Count; I++)
                {
                    Vector MouseForce = Agents[I].Location - mouseloc;
                    Double ForceMag = 100/ MouseForce.Length; 
                    MouseForce.Normalize();
                    MouseForce = ForceMag * MouseForce;
                    Agents[I].ApplyForce(MouseForce); //(Agents[I].Location - mouseloc));
                }
            }
        }

        private void SetTargets()
        {
            Point pt;
            Point tangent;
            PathGeometry geo;
            sym.Text = TxtInput.Text;
            geo = sym.Geometry.GetFlattenedPathGeometry();
            for (int I = 0; I <= num; I++)
            {
                geo.GetPointAtFractionLength(I / (double)num, out pt, out tangent);
                Agents[I].SetTarget(pt);
            }
        }
    }
}