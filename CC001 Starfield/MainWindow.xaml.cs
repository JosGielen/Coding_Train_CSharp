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

namespace Starfield
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Star> Stars;
        private int StarCount = 800;
        private double speed;
        private Random rnd = new Random();
        private bool Rendering = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Star s;
            Stars = new List<Star>();
            speed = SldSpeed.Value;
            TxtSpeed.Text = speed.ToString();
            for (int I = 0; I < StarCount; I++)
            {
                s = new Star(canvas1.ActualWidth * rnd.NextDouble(), canvas1.ActualHeight * rnd.NextDouble(), 3 * rnd.NextDouble());
                s.Show(canvas1);
                Stars.Add(s);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
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
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            BtnStart.Content = "FULL STOP";
            Rendering = true;
        }

        private void StopRender()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            BtnStart.Content = "ENGAGE";
            Rendering = false;
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            for (int I = 0; I < Stars.Count; I++)
            {
                Stars[I].Update(speed);
            }
        }

        private void SldSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            speed = SldSpeed.Value;
            TxtSpeed.Text = speed.ToString();
        }
    }
}
