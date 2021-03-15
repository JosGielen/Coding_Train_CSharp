using System;
using System.Windows;
using System.Windows.Controls;

namespace ChaosGame
{
    public partial class Settings : Window
    {
        public int PointsNum = 3;
        public double StepPercentage = 0.5;
        public bool UsePointColors = true;
        private MainWindow my_Parent;

        public Settings(MainWindow parent)
        {
            InitializeComponent();
            my_Parent = parent;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            SldStep.Value = Math.Floor(SldStep.Value);
            StepPercentage = SldStep.Value;
            TxtStep.Text = StepPercentage.ToString() + "%";
        }

        private void TxtNum_TextChanged(Object sender, TextChangedEventArgs e)
        {
            try
            {
                PointsNum = int.Parse(TxtNum.Text);
                if (PointsNum < 3) PointsNum = 3;
                TxtNum.Text = PointsNum.ToString();
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        private void BtnNumUP_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                PointsNum = int.Parse(TxtNum.Text);
                PointsNum += 1;
                TxtNum.Text = PointsNum.ToString();
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        private void BtnNumDown_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                PointsNum = int.Parse(TxtNum.Text);
                PointsNum -= 1;
                if (PointsNum < 3) PointsNum = 3;
                TxtNum.Text = PointsNum.ToString();
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        private void BtnOK_Click(Object sender, RoutedEventArgs e)
        {
            ReadSettings();
            my_Parent.Update();
        }

        private void BtnCancel_Click(Object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ReadSettings()
        {
            //Read the values in the settingform to the setting variables
            PointsNum = int.Parse(TxtNum.Text);
            StepPercentage = SldStep.Value;
            UsePointColors = CBColors.IsChecked.Value;
        }

        public void WriteSettings()
        {
            //Show the setting variables in the settingform
            TxtNum.Text = PointsNum.ToString();
            SldStep.Value = StepPercentage;
            TxtStep.Text = StepPercentage.ToString() + "%";
            CBColors.IsChecked = UsePointColors;
        }

        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Prevent closing the form so the settings remain accessible
            Hide();
            e.Cancel = true;
        }

        private void MnuClose_Click(Object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
