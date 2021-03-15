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

namespace Springs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int SpringCount = 50;
        private Spring[] Springs;
        private readonly double SpringConstant = 0.15;
        private readonly double Damping = 0.998;
        private readonly double BobSize = 4.0;


        private bool mouseIsDown = false;
        private Point mousePos;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Springs = new Spring[SpringCount];
            Particle[] particles = new Particle[SpringCount + 1];
            for (int I = 0; I < particles.Length; I++)
            {
                particles[I] = new Particle(5.0, new Point( I * canvas1.ActualWidth / (SpringCount + 1), canvas1.ActualHeight / 2));
            }
            particles[0].Locked = true;
            particles[SpringCount].Locked = true;
            for(int I= 0; I < SpringCount; I++)
            {
                Springs[I] = new Spring(particles[I], particles[I+1]);
                Springs[I].Stiffness = SpringConstant;
                Springs[I].Damping = Damping;
                Springs[I].End1.Size = BobSize;
                Springs[I].End2.Size = BobSize;
                Springs[I].Draw(canvas1);
            }
            CompositionTarget.Rendering += Render;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseIsDown = true;
            mousePos = e.GetPosition(canvas1);
            Springs[(int)(SpringCount -1)].SetEnd2Position(mousePos);
            Springs[(int)(SpringCount -1)].End2.Locked = true;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                mousePos = e.GetPosition(canvas1);
                Springs[(int)(SpringCount -1)].SetEnd2Position(mousePos);
                Springs[(int)(SpringCount -1)].End2.Locked = true;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseIsDown = false;
            Springs[(int)(SpringCount / 2.0)].End2.Locked = false;
        }

        private void Render(object sender, EventArgs e)
        {
            for (int I = 0; I < Springs.Length; I++)
            {
                Springs[I].Update();
            }
            for (int I = 0; I < Springs.Length; I++)
            {
                Springs[I].ReDraw();
            }
        }
    }
}
