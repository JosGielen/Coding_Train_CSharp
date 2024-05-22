using System.Windows;

namespace Snakes_and_Ladders
{
    /// <summary>
    /// Interaction logic for UserInput.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private int my_PlayerCount;
        private string my_Player1Name;
        private string my_Player2Name;
        private bool my_UsePlayerDice;
        private bool my_ExactEnding;

        public Settings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            my_PlayerCount = 1;
            my_UsePlayerDice = false;
        }

        public int PlayerCount
        {
            get { return my_PlayerCount; }
        }

        public string Player1
        { 
            get { return my_Player1Name; } 
        }

        public string Player2
        { 
            get { return my_Player2Name; } 
        }

        public bool UsePlayerDice
        {
            get { return my_UsePlayerDice; }
        }

        public bool ExactEnding
        {
            get { return my_ExactEnding; }
        }

        private void RbSinglePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (RbSinglePlayer.IsChecked == true)
            {
                my_PlayerCount = 1;
            }
            else
            {
                my_PlayerCount = 2;
            }
        }

        private void RbTwoPlayers_Click(object sender, RoutedEventArgs e)
        {
            if (RbTwoPlayers.IsChecked == true)
            {
                my_PlayerCount = 2;
            }
            else
            {
                my_PlayerCount = 1;
            }
        }

        private void RbUseDice_Click(object sender, RoutedEventArgs e)
        {
            if (RbUseDice.IsChecked == true)
            {
                my_UsePlayerDice = true;
            }
            else
            {
                my_UsePlayerDice = false;
            }
        }

        private void RbRandom_Click(object sender, RoutedEventArgs e)
        {
            if (RbRandom.IsChecked == true)
            {
                my_UsePlayerDice = false;
            }
            else
            {
                my_UsePlayerDice = true;
            }
        }

        private void CBExact_Click(object sender, RoutedEventArgs e)
        {
            my_ExactEnding = CBExact.IsChecked == true;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            my_Player1Name = TxtPlayer1Name.Text;
            my_Player2Name = TxtPlayer2Name.Text;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
