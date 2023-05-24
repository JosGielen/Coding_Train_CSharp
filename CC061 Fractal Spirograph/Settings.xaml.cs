using System;
using System.Windows;

namespace Fractal_Spirograph
{
    public partial class Settings : Window
    {
        private Window MyMain;
        private int My_CircleCount = 0;
        private int My_RadiusFactor = 0;
        private int my_SpeedFactor = 0;
        private bool my_InnerCircles = false;
        private double my_TimeStep = 0.0;

        public Settings(Window main)
        {
            // This call is required by the designer.
            InitializeComponent();
            MyMain = main;
        }

        public int CircleCount
        {
            get { return My_CircleCount; }
            set
            {
                My_CircleCount = value;
                TxtAantal.Text = My_CircleCount.ToString();
            }
        }

        public int RadiusFactor
        {
            get { return My_RadiusFactor; }
            set
            {
                My_RadiusFactor = value;
                TxtRadius.Text = My_RadiusFactor.ToString() + "%";
            }
        }


        public bool InnerCircles
        {
            get { return RbInner.IsChecked.Value; }
            set
            {
                my_InnerCircles = value;
                RbInner.IsChecked = value;
                RbOuter.IsChecked = !value;
            }
        }

        public int SpeedFactor
        {
            get { return my_SpeedFactor; }
            set
            {
                my_SpeedFactor = value;
                TxtSpeed.Text = my_SpeedFactor.ToString();
            }
        }

        public double TimeStep
        {
            get { return my_TimeStep; }
            set
            {
                my_TimeStep = value;
                TxtStep.Text = my_TimeStep.ToString();
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (BtnStart.Content.Equals("Start"))
            {
                BtnStart.Content = "Stop";
                try
                {
                    My_CircleCount = int.Parse(TxtAantal.Text);
                    My_RadiusFactor = int.Parse(TxtRadius.Text.Substring(0, 2));
                    my_SpeedFactor = int.Parse(TxtSpeed.Text);
                    my_InnerCircles = RbInner.IsChecked.Value;
                    my_TimeStep = double.Parse(TxtStep.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The Parameters are not valid.", "L-System settings error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                ((MainWindow)MyMain).Start();
            }
            else
            {
                ((MainWindow)MyMain).Halt();
                BtnStart.Content = "Start";
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            MyMain.Close();
            Environment.Exit(0);
        }

        private void BtnAantalUP_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                My_CircleCount = int.Parse(TxtAantal.Text);
                My_CircleCount += 1;
                TxtAantal.Text = My_CircleCount.ToString();
            }
            catch (Exception ex)
            {
                //Do nothing
            }
        }

        private void BtnAantalDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                My_CircleCount = int.Parse(TxtAantal.Text);
                My_CircleCount -= 1;
                if (My_CircleCount < 2) My_CircleCount = 2;
                TxtAantal.Text = My_CircleCount.ToString();
            }
            catch (Exception ex)
            {
                //Do nothing
            }
        }

        private void BtnRadiusUP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                My_RadiusFactor = int.Parse(TxtRadius.Text.Substring(0, 2));
                My_RadiusFactor += 5;
                TxtRadius.Text = My_RadiusFactor.ToString() + "%";

            }
            catch (Exception ex)
            {
                //Do nothing
            }
        }

        private void BtnRadiusDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                My_RadiusFactor = int.Parse(TxtRadius.Text.Substring(0, 2));
                My_RadiusFactor -= 5;
                if (My_RadiusFactor < 10) My_RadiusFactor = 10;
                TxtRadius.Text = My_RadiusFactor.ToString() + "%";
            }
            catch (Exception ex)
            {
                //Do nothing
            }
        }

        private void BtnSpeedUP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                my_SpeedFactor = int.Parse(TxtSpeed.Text);
                my_SpeedFactor += 1;
                TxtSpeed.Text = my_SpeedFactor.ToString();
            }
            catch (Exception ex)
            {
                //Do nothing
            }
        }

        private void BtnSpeedDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                my_SpeedFactor = int.Parse(TxtSpeed.Text);
                my_SpeedFactor -= 1;
                TxtSpeed.Text = my_SpeedFactor.ToString();
            }
            catch (Exception ex)
            {
                //Do nothing
            }
        }

    }
}
