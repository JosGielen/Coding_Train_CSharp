using System.Windows;

namespace Soft_Body_Simulation
{
    public partial class MainWindow : Window
    {
        Particle[] particles;
        private Spring[] Springs;
        private Vector Gravity = new Vector(0, 0.15);
        private double Damping = 0.99;
        private double ParticleMass = 2.0;
        private double SpringStiffness = 0.2;
        private double groundPos;
        private bool AppStarted = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            groundPos = canvas1.ActualHeight;
            MakeBlobby();
        }

        private void MakeBlobby()
        {
            canvas1.Children.Clear();
            Springs = new Spring[43];
            particles = new Particle[20];
            particles[0] = new Particle(ParticleMass, new Point(330, 50), true, groundPos);
            particles[1] = new Particle(ParticleMass, new Point(470, 50), true, groundPos);
            particles[2] = new Particle(ParticleMass, new Point(300, 75), true, groundPos);
            particles[3] = new Particle(ParticleMass, new Point(360, 75), true, groundPos);
            particles[4] = new Particle(ParticleMass, new Point(440, 75), true, groundPos);
            particles[5] = new Particle(ParticleMass, new Point(500, 75), true, groundPos);
            particles[6] = new Particle(ParticleMass, new Point(300, 125), true, groundPos);
            particles[7] = new Particle(ParticleMass, new Point(360, 125), false, groundPos);
            particles[8] = new Particle(ParticleMass, new Point(440, 125), false, groundPos);
            particles[9] = new Particle(ParticleMass, new Point(500, 125), true, groundPos);
            particles[10] = new Particle(ParticleMass, new Point(360, 175), true, groundPos);
            particles[11] = new Particle(ParticleMass, new Point(440, 175), true, groundPos);
            particles[12] = new Particle(ParticleMass, new Point(360, 225), true, groundPos);
            particles[13] = new Particle(ParticleMass, new Point(440, 225), true, groundPos);
            particles[14] = new Particle(ParticleMass, new Point(300, 225), true, groundPos);
            particles[15] = new Particle(ParticleMass, new Point(500, 225), true, groundPos);
            particles[16] = new Particle(ParticleMass, new Point(300, 275), true, groundPos);
            particles[17] = new Particle(ParticleMass, new Point(360, 300), true, groundPos);
            particles[18] = new Particle(ParticleMass, new Point(440, 300), true, groundPos);
            particles[19] = new Particle(ParticleMass, new Point(500, 275), true, groundPos);
            Springs[0] = new Spring(particles[0], particles[2], true);
            Springs[1] = new Spring(particles[0], particles[3], true);
            Springs[2] = new Spring(particles[1], particles[4], true);
            Springs[3] = new Spring(particles[1], particles[5], true);
            Springs[4] = new Spring(particles[2], particles[3], false);
            Springs[5] = new Spring(particles[3], particles[4], true);
            Springs[6] = new Spring(particles[4], particles[5], false);
            Springs[7] = new Spring(particles[2], particles[6], true);
            Springs[8] = new Spring(particles[3], particles[6], false);
            Springs[9] = new Spring(particles[3], particles[7], false);
            Springs[10] = new Spring(particles[4], particles[7], false);
            Springs[11] = new Spring(particles[3], particles[8], false);
            Springs[12] = new Spring(particles[4], particles[8], false);
            Springs[13] = new Spring(particles[4], particles[9], false);
            Springs[14] = new Spring(particles[5], particles[9], true);
            Springs[15] = new Spring(particles[6], particles[7], false);
            Springs[16] = new Spring(particles[7], particles[8], false);
            Springs[17] = new Spring(particles[8], particles[9], false);
            Springs[18] = new Spring(particles[6], particles[10], true);
            Springs[19] = new Spring(particles[7], particles[10], false);
            Springs[20] = new Spring(particles[7], particles[11], false);
            Springs[21] = new Spring(particles[8], particles[10], false);
            Springs[22] = new Spring(particles[8], particles[11], false);
            Springs[23] = new Spring(particles[9], particles[11], true);
            Springs[24] = new Spring(particles[10], particles[11], false);
            Springs[25] = new Spring(particles[10], particles[12], true);
            Springs[26] = new Spring(particles[10], particles[13], false);
            Springs[27] = new Spring(particles[11], particles[12], false);
            Springs[28] = new Spring(particles[11], particles[13], true);
            Springs[29] = new Spring(particles[12], particles[13], false);
            Springs[30] = new Spring(particles[12], particles[14], true);
            Springs[31] = new Spring(particles[13], particles[15], true);
            Springs[32] = new Spring(particles[14], particles[16], true);
            Springs[33] = new Spring(particles[14], particles[17], false);
            Springs[34] = new Spring(particles[12], particles[17], false);
            Springs[35] = new Spring(particles[12], particles[18], false);
            Springs[36] = new Spring(particles[13], particles[17], false);
            Springs[37] = new Spring(particles[13], particles[18], false);
            Springs[38] = new Spring(particles[15], particles[18], false);
            Springs[39] = new Spring(particles[15], particles[19], true);
            Springs[40] = new Spring(particles[16], particles[17], true);
            Springs[41] = new Spring(particles[17], particles[18], true);
            Springs[42] = new Spring(particles[18], particles[19], true);
            for (int I = 0; I < Springs.Length; I++)
            {
                Springs[I].Stiffness = SpringStiffness;
                Springs[I].Damping = Damping;
                Springs[I].Draw(canvas1, true);
            }
            Gravity.Y = 0.15;
            SpringStiffness = 0.2;
            SldGravity.Value = Gravity.Y;
            SldStiffness.Value = SpringStiffness;
        }

        private void MakeRobby()
        {
            canvas1.Children.Clear();          
            Springs = new Spring[54];
            particles = new Particle[25];
            particles[0] = new Particle(ParticleMass, new Point(370, 250), true, groundPos);
            particles[1] = new Particle(ParticleMass, new Point(430, 250), true, groundPos);
            particles[2] = new Particle(ParticleMass, new Point(300, 300), true, groundPos);
            particles[3] = new Particle(ParticleMass, new Point(370, 300), true, groundPos);
            particles[4] = new Particle(ParticleMass, new Point(430, 300), true, groundPos);
            particles[5] = new Particle(ParticleMass, new Point(500, 300), true, groundPos);
            particles[6] = new Particle(ParticleMass, new Point(370, 350), true, groundPos);
            particles[7] = new Particle(ParticleMass, new Point(430, 350), true, groundPos);
            particles[8] = new Particle(ParticleMass, new Point(300, 350), true, groundPos);
            particles[9] = new Particle(ParticleMass, new Point(340, 350), true, groundPos);
            particles[10] = new Particle(ParticleMass, new Point(460, 350), true, groundPos);
            particles[11] = new Particle(ParticleMass, new Point(500, 350), true, groundPos);
            particles[12] = new Particle(ParticleMass, new Point(370, 400), true, groundPos);
            particles[13] = new Particle(ParticleMass, new Point(430, 400), true, groundPos);
            particles[14] = new Particle(ParticleMass, new Point(310, 420), true, groundPos);
            particles[15] = new Particle(ParticleMass, new Point(330, 420), true, groundPos);
            particles[16] = new Particle(ParticleMass, new Point(470, 420), true, groundPos);
            particles[17] = new Particle(ParticleMass, new Point(490, 420), true, groundPos);
            particles[18] = new Particle(ParticleMass, new Point(355, 480), true, groundPos);
            particles[19] = new Particle(ParticleMass, new Point(445, 480), true, groundPos);
            particles[20] = new Particle(ParticleMass, new Point(355, 520), true, groundPos);
            particles[21] = new Particle(ParticleMass, new Point(375, 520), true, groundPos);
            particles[22] = new Particle(ParticleMass, new Point(425, 520), true, groundPos);
            particles[23] = new Particle(ParticleMass, new Point(445, 520), true, groundPos);
            particles[24] = new Particle(ParticleMass, new Point(400, 430), true, groundPos);
            Springs[0] = new Spring(particles[0], particles[1], true);
            Springs[1] = new Spring(particles[0], particles[3], true);
            Springs[2] = new Spring(particles[1], particles[4], true);
            Springs[3] = new Spring(particles[0], particles[4], false);
            Springs[4] = new Spring(particles[1], particles[3], false);
            Springs[5] = new Spring(particles[2], particles[3], true);
            Springs[6] = new Spring(particles[3], particles[4], false);
            Springs[7] = new Spring(particles[4], particles[5], true);
            Springs[8] = new Spring(particles[2], particles[8], true);
            Springs[9] = new Spring(particles[2], particles[9], false);
            Springs[10] = new Spring(particles[3], particles[6], false);
            Springs[11] = new Spring(particles[3], particles[7], false);
            Springs[12] = new Spring(particles[4], particles[6], false);
            Springs[13] = new Spring(particles[4], particles[7], false);
            Springs[14] = new Spring(particles[5], particles[10], false);
            Springs[15] = new Spring(particles[5], particles[11], true);
            Springs[16] = new Spring(particles[6], particles[7], false);
            Springs[17] = new Spring(particles[9], particles[6], true);
            Springs[18] = new Spring(particles[7], particles[10], true);
            Springs[19] = new Spring(particles[8], particles[9], false);
            Springs[20] = new Spring(particles[10], particles[11], false);
            Springs[21] = new Spring(particles[6], particles[12], true);
            Springs[22] = new Spring(particles[6], particles[13], false);
            Springs[23] = new Spring(particles[7], particles[12], false);
            Springs[24] = new Spring(particles[7], particles[13], true);
            Springs[25] = new Spring(particles[12], particles[13], false);
            Springs[26] = new Spring(particles[8], particles[14], true);
            Springs[27] = new Spring(particles[9], particles[14], false);
            Springs[28] = new Spring(particles[9], particles[15], true);
            Springs[29] = new Spring(particles[10], particles[16], true);
            Springs[30] = new Spring(particles[10], particles[17], false);
            Springs[31] = new Spring(particles[11], particles[17], true);
            Springs[32] = new Spring(particles[14], particles[15], true);
            Springs[33] = new Spring(particles[16], particles[17], true);
            Springs[34] = new Spring(particles[12], particles[18], true);
            Springs[35] = new Spring(particles[12], particles[24], false);
            Springs[36] = new Spring(particles[13], particles[24], false);
            Springs[37] = new Spring(particles[13], particles[19], true);
            Springs[38] = new Spring(particles[18], particles[24], false);
            Springs[39] = new Spring(particles[19], particles[24], false);
            Springs[40] = new Spring(particles[18], particles[20], true);
            Springs[41] = new Spring(particles[18], particles[21], false);
            Springs[42] = new Spring(particles[20], particles[24], false);
            Springs[43] = new Spring(particles[21], particles[24], true);
            Springs[44] = new Spring(particles[22], particles[24], true);
            Springs[45] = new Spring(particles[23], particles[24], false);
            Springs[46] = new Spring(particles[19], particles[22], false);
            Springs[47] = new Spring(particles[19], particles[23], true);
            Springs[48] = new Spring(particles[20], particles[21], true);
            Springs[49] = new Spring(particles[22], particles[23], true);
            Springs[50] = new Spring(particles[3], particles[8], false);
            Springs[51] = new Spring(particles[4], particles[11], false);
            Springs[52] = new Spring(particles[3], particles[9], false);
            Springs[53] = new Spring(particles[4], particles[10], false);
            for (int I = 0; I < Springs.Length; I++)
            {
                Springs[I].Stiffness = SpringStiffness;
                Springs[I].Damping = Damping;
                Springs[I].Draw(canvas1, true);
            }
            Gravity.Y = 0.3;
            SpringStiffness = 1.0;
            SldGravity.Value = Gravity.Y;
            SldStiffness.Value = SpringStiffness;
        }

        private void Render()
        {
            while (AppStarted)
            {
                //Apply gravity separate to avoid multiple gravity forces
                //on a particle with several springs attached.
                for (int I = 0; I < particles.Length; I++)
                {
                    particles[I].ApplyForce(Gravity);
                }
                //Apply all the spring forces
                for (int I = 0; I < Springs.Length; I++)
                {
                    Springs[I].Update();
                }
                //Draw the new positions and lock the particles to avoid
                //multiple position updates on a particle with several springs attached.
                for (int I = 0; I < Springs.Length; I++)
                {
                    Springs[I].Redraw();
                }
                //Unlock the particles
                for (int I = 0; I < particles.Length; I++)
                {
                    particles[I].Locked = false;
                }
                Dispatcher.Invoke(WaitTime, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }
        }

        private void WaitTime()
        {
            Thread.Sleep(20);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!AppStarted)
            {
                BtnStart.Content = "STOP";
                AppStarted = true;
                Render();
            }
            else
            {
                BtnStart.Content = "START";
                AppStarted = false;
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (RbBlobby.IsChecked == true) 
            {
                MakeBlobby();
            }
            else if (RbRobby.IsChecked == true)
            {
                MakeRobby();
            }
            UpdateInternals();
        }

        private void SldStiffness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            SpringStiffness = SldStiffness.Value;
            TxtStiffness.Text = SpringStiffness.ToString("F2");
            for (int I = 0; I < Springs.Length; I++)
            {
                Springs[I].Stiffness = SpringStiffness;
            }
        }

        private void SldGravity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            Gravity.Y = SldGravity.Value;
            TxtGravity.Text = Gravity.ToString();
        }

        private void RbBlobby_Click(object sender, RoutedEventArgs e)
        {
            MakeBlobby();
            UpdateInternals();
        }

        private void RbRobby_Click(object sender, RoutedEventArgs e)
        {
            MakeRobby();
            UpdateInternals();
        }

        private void CbInternals_Click(object sender, RoutedEventArgs e)
        {
            UpdateInternals();
        }

        private void UpdateInternals()
        {
            if (RbBlobby.IsChecked == true)
            {
                if (CbInternals.IsChecked == true)
                {
                    particles[7].Show = true;
                    particles[8].Show = true;
                    for (int I = 0; I < Springs.Length; I++)
                    {
                        Springs[I].ShowLine = true;
                    }
                }
                else
                {
                    particles[7].Show = false;
                    particles[8].Show = false;
                    Springs[4].ShowLine = false;
                    Springs[6].ShowLine = false;
                    Springs[8].ShowLine = false;
                    Springs[9].ShowLine = false;
                    Springs[10].ShowLine = false;
                    Springs[11].ShowLine = false;
                    Springs[12].ShowLine = false;
                    Springs[13].ShowLine = false;
                    Springs[15].ShowLine = false;
                    Springs[16].ShowLine = false;
                    Springs[17].ShowLine = false;
                    Springs[19].ShowLine = false;
                    Springs[20].ShowLine = false;
                    Springs[21].ShowLine = false;
                    Springs[22].ShowLine = false;
                    Springs[24].ShowLine = false;
                    Springs[26].ShowLine = false;
                    Springs[27].ShowLine = false;
                    Springs[29].ShowLine = false;
                    Springs[33].ShowLine = false;
                    Springs[34].ShowLine = false;
                    Springs[35].ShowLine = false;
                    Springs[36].ShowLine = false;
                    Springs[37].ShowLine = false;
                    Springs[38].ShowLine = false;
                }
                canvas1.Children.Clear();
                for (int I = 0; I < Springs.Length; I++)
                {
                    Springs[I].Draw(canvas1, true);
                }
            }
            else if (RbRobby.IsChecked == true)
            {
                if (CbInternals.IsChecked == true)
                {
                    for (int I = 0; I < Springs.Length; I++)
                    {
                        Springs[I].ShowLine = true;
                    }
                }
                else
                {
                    Springs[3].ShowLine = false;
                    Springs[4].ShowLine = false;
                    Springs[6].ShowLine = false;
                    Springs[9].ShowLine = false;
                    Springs[10].ShowLine = false;
                    Springs[11].ShowLine = false;
                    Springs[12].ShowLine = false;
                    Springs[13].ShowLine = false;
                    Springs[14].ShowLine = false;
                    Springs[16].ShowLine = false;
                    Springs[19].ShowLine = false;
                    Springs[20].ShowLine = false;
                    Springs[22].ShowLine = false;
                    Springs[23].ShowLine = false;
                    Springs[25].ShowLine = false;
                    Springs[27].ShowLine = false;
                    Springs[30].ShowLine = false;
                    Springs[35].ShowLine = false;
                    Springs[36].ShowLine = false;
                    Springs[38].ShowLine = false;
                    Springs[39].ShowLine = false;
                    Springs[41].ShowLine = false;
                    Springs[42].ShowLine = false;
                    Springs[45].ShowLine = false;
                    Springs[46].ShowLine = false;
                    Springs[50].ShowLine = false;
                    Springs[51].ShowLine = false;
                    Springs[52].ShowLine = false;
                    Springs[53].ShowLine = false;
                }
                canvas1.Children.Clear();
                for (int I = 0; I < Springs.Length; I++)
                {
                    Springs[I].Draw(canvas1, true);
                }
            }

        }
    }
}