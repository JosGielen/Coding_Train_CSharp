using System.Windows;
using System.Windows.Media;

namespace Particle_System
{
    public partial class MainWindow : Window
    {
        private bool Rendering = false;
        private List<Particle> particles;
        private List<Color> Rainbow;
        private int ColorIndex;
        private int counter;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                Init();
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                BtnStart.Content = "Stop";
                Rendering = true;
            }
            else
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                BtnStart.Content = "Start";
                Rendering = false;
            }
        }

        private void Init()
        {
            Particle p;
            particles = new List<Particle>();
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            Rainbow = pal.GetColors(256);
            ColorIndex = 0;
            counter = 0;
            canvas1.Children.Clear();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Particle p;
            Color c;
            double X;
            double Y;
            for (int I = 0; I <= 2; I++)
            {
                X = canvas1.ActualWidth / 2 + 10 * Rnd.NextDouble() - 5;
                Y = canvas1.ActualHeight - 10;
                c = Rainbow[ColorIndex];
                p = new Particle(X, Y, 0.2 * Rnd.NextDouble() - 0.1, 1.0, 8.0, c);
                particles.Add(p);
                p.Draw(canvas1);
                counter++;
                if (counter % 50 == 0) ColorIndex++;
                if (ColorIndex >= Rainbow.Count ) ColorIndex = 0;
            }
            for (int I = particles.Count - 1; I >= 0; I--)
            {
                particles[I].Update(0.04 * Rnd.NextDouble() - 0.02);
                if (particles[I].Alpha <= 0)
                {
                    particles.RemoveAt(I);
                    canvas1.Children.RemoveAt(I);
                }
            }
            Title = particles.Count.ToString() + " particles.";
        }
    }
}