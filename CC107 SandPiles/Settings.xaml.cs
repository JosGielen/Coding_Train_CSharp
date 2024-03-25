using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SandPiles
{
    public partial class Settings : Window
    {
        private Window My_Main;
        private int my_MaxSand;
        private int my_InitialSand;
        private int my_DistributionNr;
        private string my_Type;
        private List<Color> my_ColorList;

        public Settings()
        {
            InitializeComponent();
        }

        public Settings(Window main, List<Color> colors)
        {
            // This call is required by the designer.
            InitializeComponent();
            My_Main = main;
            my_ColorList = colors;
        }

        public int MaxSand
        {
            get
            {
                my_MaxSand = int.Parse(TxtMaxSand.Text);
                return my_MaxSand;
            }
            set
            {
                my_MaxSand = value;
                TxtMaxSand.Text = my_MaxSand.ToString();
                Init();
            }
        }

        public int DistributionNr
        {
            get { return my_DistributionNr; }
            set
            {
                my_DistributionNr = value;
                if (my_DistributionNr == 4)
                {
                    RB4way.IsChecked = true;
                }
                else if (my_DistributionNr == 8)
                {
                    RB8Way.IsChecked = true;
                }
                else
                {
                    MessageBox.Show("Invalid Sand distribution number.", "SandPiles Settings error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public string Type
        {
            get
            {
                my_Type = TxtType.Text;
                return my_Type;
            }
            set
            {
                my_Type = value;
                TxtType.Text = my_Type;
            }
        }

        public List<Color> ColorList
        {
            get { return my_ColorList; }
            set
            {
                my_ColorList = value;
                Init();
            }
        }

        public int InitialSand
        {
            get
            {
                my_InitialSand = int.Parse(TxtInitialSand.Text);
                return my_InitialSand;
            }
            set
            {
                my_InitialSand = value;
                TxtInitialSand.Text = my_InitialSand.ToString();
            }
        }

        private void Init()
        {
            Button btn;
            LBColors.Items.Clear();
            for (int I = 0; I <= my_MaxSand + 1; I++)
            {
                btn = new Button()
                {
                    Background = new SolidColorBrush(my_ColorList[I]),
                    Height = 15,
                    Width = LBColors.ActualWidth - 15
                };
                LBColors.Items.Add(btn);
            }
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                if ((string)BtnStart.Content == "Start")
                {
                    ((MainWindow)My_Main).Start();
                    BtnStart.Content = "Stop";
                }
                else
                {
                    ((MainWindow)My_Main).Halt();
                    BtnStart.Content = "Start";
                }
            }
            catch
            {
                MessageBox.Show("The Parameters are not valid.", "SandPiles Settings error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)My_Main).MnuShowSettings.IsChecked = false;
            Hide();
        }

        private void BtnColor_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Show a COLOR Selection Dialog
            ((Button)sender).Background = Brushes.Black; //Set the background to the selected color
        }

        private void RB4way_Click(object sender, RoutedEventArgs e)
        {
            if (RB4way.IsChecked == true) my_DistributionNr = 4;
        }

        private void RB8Way_Click(object sender, RoutedEventArgs e)
        {
            if (RB8Way.IsChecked == true) my_DistributionNr = 8;
        }

        private void TxtMaxSand_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtMaxSand.Text == "") return;
            try
            {
                my_MaxSand = int.Parse(TxtMaxSand.Text);
                Init();
            }
            catch
            {
                MessageBox.Show("Invalid Maximum Sand.", "SandPiles Settings Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
