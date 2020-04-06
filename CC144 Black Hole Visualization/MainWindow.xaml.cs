using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace BlackHoleSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly double dt = 0.05;
        public readonly double c = 30;
        public readonly double G = 3.54;

        private bool Rendering = false;
        private BlackHole M87;
        private List<Photon> Photons;
        private int PhotonCount = 400;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Init()
        {
            Photon p;
            Photons = new List<Photon>();
            M87 = new BlackHole(canvas1.ActualWidth / 2 - 100, canvas1.ActualHeight / 2, 5000);
            M87.Show(canvas1);
            for (int I = 0; I < PhotonCount; I++)
            {
                p = new Photon(canvas1.ActualWidth - 10, canvas1.ActualHeight / 2 + (I - 200), new Vector(-c, 0));
                Photons.Add(p);
                p.Show(canvas1);
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                BtnStart.Content = "STOP";
                Rendering = true;
                Init();
                Render();
            }
            else
            {
                BtnStart.Content = "START";
                Rendering = false;
            }
        }

        private void Render()
        { 
            int liveCount = 0;
            do
            {
                liveCount = 0;
                if (!Rendering) return;
                for (int I = 0; I < Photons.Count; I++)
                {
                    if (Photons[I].Alive)
                    {
                        liveCount += 1;
                        M87.Pull(Photons[I]);
                        Photons[I].Update(canvas1);
                    }
                }
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
            }
            while (liveCount > 0);
        }

        private void Wait()
        {
            Thread.Sleep(10);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
