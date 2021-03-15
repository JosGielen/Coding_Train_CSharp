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
using System.Windows.Shapes;

namespace DiffReaction
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private MainWindow MyMain;
        private double MyDiffA = 0.0;
        private double MyDiffB = 0.0;
        private double MyFeed = 0.0;
        private double MyKill = 0.0;
        private bool myUseColor = false;

        public Settings(MainWindow main)
        {
            InitializeComponent();
            MyMain = main;
        }

        public double DiffA
        {
            get { return MyDiffA; }
            set
            {
                MyDiffA = value;
                TxtDiffA.Text = MyDiffA.ToString();
            }
        }

        public double DiffB
        {
            get { return MyDiffB; }
            set
            {
                MyDiffB = value;
                TxtDiffB.Text = MyDiffB.ToString();
            }
        }

        public double Feed
        {
            get { return MyFeed; }
            set
            {
                MyFeed = value;
                TxtFeed.Text = MyFeed.ToString();
            }
        }

        public double Kill
        {
            get { return MyKill; }
            set
            {
                MyKill = value;
                TxtKill.Text = MyKill.ToString();
            }
        }

        public bool UseColor
        {
            get { return myUseColor; }
            set
            {
                myUseColor = value;
                CBUseColor.IsChecked = value;
            }
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            if (BtnStart.Content.Equals("Start"))
            {
                BtnStart.Content = "Stop";
                try
                {
                    MyDiffA = double.Parse(TxtDiffA.Text);
                    MyDiffB = double.Parse(TxtDiffB.Text);
                    MyFeed = double.Parse(TxtFeed.Text);
                    MyKill = double.Parse(TxtKill.Text);
                    myUseColor = CBUseColor.IsChecked.Value;
                    MyMain.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The Parameters are not valid.", "L-System settings error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
            }
            else
            {
                MyMain.Halt();
                BtnStart.Content = "Start";
            }
        }

        private void BtnClose_Click(Object sender, RoutedEventArgs e)
        {
            MyMain.Close();
            Hide();
        }
    }
}
