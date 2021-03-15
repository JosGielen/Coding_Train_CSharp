using System.Windows;

namespace Connect4
{
    public partial class GameSettings : Window
    {
        private int my_Playeraantal;
        private string my_Player1;
        private string my_Player2;
        private Size my_Size;
        private GameMode my_GameMode;

        public GameSettings(Settings setting, MainWindow parent)
        {
            InitializeComponent();
            my_Playeraantal = setting.PlayerCount;
            my_Player1 = setting.PlayerName1;
            my_Player2 = setting.PlayerName2;
            my_Size = setting.GameSize;
            my_GameMode = setting.GameMode;
            if (my_Playeraantal == 1) RbSinglePlayer.IsChecked = true;
            if (my_Playeraantal == 2) RbTwoPlayers.IsChecked = true;
            TxtPlayer1Name.Text = my_Player1;
            TxtPlayer2Name.Text = my_Player2;
            if (my_Size.Width == 7 && my_Size.Height == 6) CmbSize.SelectedIndex = 0;
            if (my_Size.Width == 8 && my_Size.Height == 8) CmbSize.SelectedIndex = 1;
            if (my_Size.Width == 10 && my_Size.Height == 10) CmbSize.SelectedIndex = 2;
            if (my_Size.Width == 12 && my_Size.Height == 12) CmbSize.SelectedIndex = 3;
            if (my_GameMode == GameMode.Easy) CmbDifficulty.SelectedIndex = 0;
            if (my_GameMode == GameMode.Normal) CmbDifficulty.SelectedIndex = 1;
            if (my_GameMode == GameMode.Hard) CmbDifficulty.SelectedIndex = 2;
        }

        public int Playeraantal
        {
            get { return my_Playeraantal; }
            set { my_Playeraantal = value; }
        }

public string Player1
        {
            get { return my_Player1; }
            set { my_Player1 = value; }
        }

public string Player2
        {
            get { return my_Player2; }
            set { my_Player2 = value; }
        }

public GameMode GameMode
        {
            get { return my_GameMode; }
            set { my_GameMode = value; }
        }

public Size GameSize
        {
            get { return my_Size; }
            set { my_Size = value; }
        }

        private void RbSinglePlayer_Click(object sender, RoutedEventArgs e)
        {
            TxtPlayer2Name.Text = "Computer";
            TxtPlayer2Name.IsReadOnly = true;
        }

        private void RbTwoPlayers_Click(object sender, RoutedEventArgs e)
        {
            if (TxtPlayer2Name.Text.Equals("Computer"))
            {
                TxtPlayer2Name.Text = "";
                TxtPlayer2Name.IsReadOnly = false;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (RbSinglePlayer.IsChecked == true) my_Playeraantal = 1;
            if (RbTwoPlayers.IsChecked == true) my_Playeraantal = 2;
            my_Player1 = TxtPlayer1Name.Text;
            my_Player2 = TxtPlayer2Name.Text;
            if (my_Player1 == "Player") my_Player1 = ""; //Dont call the player Player.;
            switch (CmbDifficulty.SelectedIndex)
            {
                case 0:
                    my_GameMode = GameMode.Easy;
                    break;
                case 1:
                    my_GameMode = GameMode.Normal;
                    break;
                case 2:
                    my_GameMode = GameMode.Hard;
                    break;
            }
            switch (CmbSize.SelectedIndex)
            {
                case 0:
                    my_Size = new Size(7, 6);
                    break;
                case 1:
                    my_Size = new Size(8, 8);
                    break;
                case 2:
                    my_Size = new Size(10, 10);
                    break;
                case 3:
                    my_Size = new Size(12, 12);
                    break;
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public struct Move
    {
        public int col;
        public int score;
    }

    public enum GameMode
    {
        Easy,
        Normal,
        Hard
    }

    public struct Settings
    {
        public int PlayerCount;
        public string PlayerName1;
        public string PlayerName2;
        public Size GameSize;
        public GameMode GameMode;
    }
}
