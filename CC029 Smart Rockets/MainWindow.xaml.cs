using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Smart_Rockets
{
    public partial class MainWindow : Window
    {
        private int WaitTime = 100;
        private int Volley = 100;
        private int LifeSpan = 100;
        private int Counter = 0;
        private Random rnd = new Random();
        private double Speed = 2.0;
        private bool AllowMutation = true;
        private bool RandomGenePick = true;
        private List<Rocket> Rockets;
        private List<Rocket> MatingPool;
        private Point Target;
        private List<Rect> Obstacles;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Obstacles = new List<Rect>();
            Target = new Point(Canvas1.ActualWidth / 2, Canvas1.ActualHeight / 7);
            Rocket r;
            Rectangle obst;
            //Make a Target
            Ellipse TargetEllipse = new Ellipse()
            {
                Width = 30,
                Height = 30,
                Stroke = Brushes.Yellow,
                StrokeThickness = 5,
                Fill = Brushes.Red
            };
            TargetEllipse.SetValue(Canvas.LeftProperty, Target.X - 15);
            TargetEllipse.SetValue(Canvas.TopProperty, Target.Y - 15);
            //Make just 1 obstacle
            Rect rect = new Rect()
            {
                X = Canvas1.ActualWidth / 3,
                Y = Canvas1.ActualHeight / 2.5,
                Width = Canvas1.ActualWidth / 3,
                Height = 30
            };
            Obstacles.Add(rect);
            //Make the rockets
            Rockets = new List<Rocket>();
            for (int I = 0; I < Volley; I++)
            {
                r = new Rocket(Target, Obstacles, new Point(Canvas1.ActualWidth / 2, Canvas1.ActualHeight - 100))
                {
                    DNA = new DNA(LifeSpan, rnd),
                    ID = I,
                    Speed = Speed,
                    StartDir = Math.PI * rnd.NextDouble()
                };
                Rockets.Add(r);
            }
            //Draw the target and obstacles
            Canvas1.Children.Add(TargetEllipse);
            for (int I = 0; I < Obstacles.Count; I++)
            {
                obst = new Rectangle()
                {
                    Width = Obstacles[I].Width,
                    Height = Obstacles[I].Height,
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 1,
                    Fill = Brushes.Yellow
                };
                obst.SetValue(Canvas.LeftProperty, Obstacles[I].X);
                obst.SetValue(Canvas.TopProperty, Obstacles[I].Y);
                Canvas1.Children.Add(obst);
            }
            //Draw the rockets
            for (int I = 0; I < Volley; I++)
            {
                Canvas1.Children.Add(Rockets[I].drawing);
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void NextGeneration()
        {
            int mateIndex1;
            int mateIndex2;
            Rocket Mate1;
            Rocket Mate2;
            double MaxFitness = 0.0;
            //Determine the fitness of the Rockets
            for (int I = 0; I < Volley; I++)
            {
                if (Rockets[I].MinDistance > 0) Rockets[I].Fitness = (10 / Rockets[I].MinDistance) * (10 / Rockets[I].MinDistance);
            }
            //Determine the maxfitness of all the Rockets
            for (int I = 0; I < Volley; I++)
            {
                if (Rockets[I].Fitness > MaxFitness) MaxFitness = Rockets[I].Fitness;
            }
            //Normalise the fitness of all Rockets with '0.05 for crash, 10 for hit target
            for (int I = 0; I < Volley; I++)
            {
                Rockets[I].Fitness = Rockets[I].Fitness / MaxFitness;
                if (Rockets[I].Crashed) Rockets[I].Fitness = 0.05;
                if (Rockets[I].HitTarget) Rockets[I].Fitness = 70 * (Rockets[I].Fitness + 5 * (LifeSpan - Rockets[I].MinTime));
            }
            //Copy Rocket DNA into the matingpool number of copies = normalized fitness score of the Rocket;
            //Rockets with higher fitness have more chance to become a Mate
            MatingPool = new List<Rocket>();
            for (int I = 0; I < Volley; I++)
            {
                for (int j = 1; j <= (int)(100 * Rockets[I].Fitness); j++)
                {
                    MatingPool.Add(Rockets[I]);
                }
            }
            //Reset the Rockets and assign them new DNA made from DNA of 2 rockets random taken out the matingpool
            for (int I = 0; I < Volley; I++)
            {
                mateIndex1 = rnd.Next(MatingPool.Count);
                mateIndex2 = rnd.Next(MatingPool.Count);
                Mate1 = MatingPool[mateIndex1];
                Mate2 = MatingPool[mateIndex2];
                Rockets[I].Reset();
                Rockets[I].Position = new Point(Canvas1.ActualWidth / 2, Canvas1.ActualHeight - 100);
                Rockets[I].StartDir = Mate1.StartDir;
                Rockets[I].DNA = new DNA(Mate1.DNA, Mate2.DNA, RandomGenePick, AllowMutation, LifeSpan, rnd);
            }
        }

        public void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            int lifecounter;
            lifecounter = 0;
            for (int I = 0; I < Volley; I++)
            {
                Rockets[I].Update(Counter);
                if (Rockets[I].Position.X < 2 | Rockets[I].Position.X > Canvas1.ActualWidth - 2 | Rockets[I].Position.Y < 2 | Rockets[I].Position.Y > Canvas1.ActualHeight - 2) Rockets[I].HitWall = true;
                if (Rockets[I].alive) lifecounter += 1;
            }
            if (lifecounter == 0) Counter = LifeSpan;
            Counter += 1;
            if (Counter > LifeSpan)
            {
                //Count the number of hits
                int hitcounter = 0;
                for (int I = 0; I < Volley; I++)
                {
                    if (Rockets[I].HitTarget) hitcounter += 1;
                }
                Title = hitcounter.ToString() + " Hits!";
                //Create the next generation
                NextGeneration();
                Counter = 0;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}