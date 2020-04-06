using System;
using System.Windows;

namespace _2D_Supershapes
{
    public partial class Settings : Window
    {
        private MainWindow myMain;
        public double A = 0.0;
        public double B = 0.0;
        public double M = 0.0;
        public double N1 = 0.0;
        public double N2 = 0.0;
        public double N3 = 0.0;

        public Settings(MainWindow main)
        {
            InitializeComponent();
            myMain = main;
        }

        public void Update()
        {
            TxtA.Text = A.ToString();
            TxtB.Text = B.ToString();
            TxtM.Text = M.ToString();
            TxtN1.Text = N1.ToString();
            TxtN2.Text = N2.ToString();
            TxtN3.Text = N3.ToString();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                A = double.Parse(TxtA.Text);
                B = double.Parse(TxtB.Text);
                M = double.Parse(TxtM.Text);
                N1 = double.Parse(TxtN1.Text);
                N2 = double.Parse(TxtN2.Text);
                N3 = double.Parse(TxtN3.Text);
                myMain.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("The Parameters are not valid.", "3D SuperShape settings error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnAUP_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtA.Text);
            Dummy += 0.1;
            TxtA.Text = Dummy.ToString();
        }

        private void BtnADown_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtA.Text);
            Dummy -= 0.1;
            TxtA.Text = Dummy.ToString();
        }

        private void BtnBUP_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtB.Text);
            Dummy += 0.1;
            TxtB.Text = Dummy.ToString();
        }

        private void BtnBDown_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtB.Text);
            Dummy -= 0.1;
            TxtB.Text = Dummy.ToString();
        }

        private void BtnMUP_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtM.Text);
            Dummy += 0.1;
            TxtM.Text = Dummy.ToString();
        }

        private void BtnMDown_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtM.Text);
            Dummy -= 0.1;
            TxtM.Text = Dummy.ToString();
        }

        private void BtnN1UP_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtN1.Text);
            Dummy += 0.1;
            TxtN1.Text = Dummy.ToString();
        }

        private void BtnN1Down_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtN1.Text);
            Dummy -= 0.1;
            TxtN1.Text = Dummy.ToString();
        }

        private void BtnN2UP_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtN2.Text);
            Dummy += 0.1;
            TxtN2.Text = Dummy.ToString();
        }

        private void BtnN2Down_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtN2.Text);
            Dummy -= 0.1;
            TxtN2.Text = Dummy.ToString();
        }

        private void BtnN3UP_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtN3.Text);
            Dummy += 0.1;
            TxtN3.Text = Dummy.ToString();
        }

        private void BtnN3Down_Click(object sender, RoutedEventArgs e)
        {
            double Dummy = double.Parse(TxtN3.Text);
            Dummy -= 0.1;
            TxtN3.Text = Dummy.ToString();
        }
    }
}
