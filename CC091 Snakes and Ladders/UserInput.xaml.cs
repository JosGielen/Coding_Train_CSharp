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

namespace Snakes_and_Ladders
{
    public partial class UserInput : Window
    {
        private MainWindow my_Parent;
        private int ActivePlayer;
        private int DiceThrow;
        Random Rnd = new Random();

        public UserInput( MainWindow parent)
        {
            InitializeComponent();
            my_Parent = parent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LblPlayer1.Content = my_Parent.Player1;
            if (my_Parent.PlayerCount == 1)
            {
                LblPlayer2.Content = "Computer";
            }
            else
            {
                LblPlayer2.Content = my_Parent.Player2;
            }
            ActivePlayer = 1;
            UpdateControls();
        }

        private void BtnPlayer1Throw_Click(object sender, RoutedEventArgs e)
        {
            if (ActivePlayer == 1)
            {
                DiceThrow = Rnd.Next(1, 13);
                TxtPlayer1Throw.Text = DiceThrow.ToString();
            }
        }

        private void BtnPlayer2Throw_Click(object sender, RoutedEventArgs e)
        {
            if (ActivePlayer == 2)
            {
                DiceThrow = Rnd.Next(1, 13);
                TxtPlayer2Throw.Text = DiceThrow.ToString();
            }
        }

        public void GetNextMove()
        {
            if (ActivePlayer == 1)
            {
                ActivePlayer = 2;
            }
            else
            {
                ActivePlayer = 1;
            }
            if (my_Parent.PlayerCount == 1)
            {
                if (ActivePlayer == 1)
                {
                    TxtPlayer2Throw.Text = "";
                    BtnOK.IsEnabled = true;
                }
                else
                {
                    //The computer makes his move.
                    DiceThrow = Rnd.Next(1, 13);
                    TxtPlayer2Throw.Text = DiceThrow.ToString();
                    my_Parent.MovePlayer(2, DiceThrow);
                }
            }
            else
            {
                BtnOK.IsEnabled = true;
            }
            UpdateControls();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            //Prevent double clicking
            BtnOK.IsEnabled = false;
            //Get the Dice throw
            DiceThrow = -1;
            if (my_Parent.UsePlayerDice == true)
            {
                if (ActivePlayer == 1)
                {
                    DiceThrow = CmbPlayer1.SelectedIndex + 2;
                }
                else
                {
                    DiceThrow = CmbPlayer2.SelectedIndex + 2;
                }
            }
            else
            {
                try
                {
                    if (ActivePlayer == 1)
                    {
                        DiceThrow = int.Parse(TxtPlayer1Throw.Text);
                    }
                    else
                    {
                        DiceThrow = int.Parse(TxtPlayer2Throw.Text);
                    }
                }
                catch 
                {
                    DiceThrow = -1;
                }
            }
            if (DiceThrow == -1)
            {
                //TODO Beep
                return;
            }
            my_Parent.MovePlayer(ActivePlayer, DiceThrow);
            DiceThrow = -1;
        }

        private void UpdateControls()
        {
            //Enable All controls
            CmbPlayer1.IsEnabled = true;
            CmbPlayer2.IsEnabled = true;
            BtnPlayer1Throw.IsEnabled = true;
            BtnPlayer2Throw.IsEnabled = true;
            CmbPlayer1.SelectedIndex = -1;
            CmbPlayer2.SelectedIndex = -1;
            //Disable controls that may not be used this turn
            if (ActivePlayer == 1)
            {
                CmbPlayer2.IsEnabled = false;
                BtnPlayer2Throw.IsEnabled = false;
                TxtPlayer1Throw.Text = "";
            }
            else
            {
                CmbPlayer1.IsEnabled = false;
                BtnPlayer1Throw.IsEnabled = false;
                if (my_Parent.PlayerCount == 2) { TxtPlayer2Throw.Text = ""; }
            }
            if (my_Parent.UsePlayerDice)
            {
                BtnPlayer1Throw.IsEnabled = false;
                BtnPlayer2Throw.IsEnabled = false;
            }
            else
            {
                CmbPlayer1.IsEnabled = false;
                CmbPlayer2.IsEnabled = false;
            }
            if (my_Parent.PlayerCount == 1)
            {
                CmbPlayer2.IsEnabled = false;
                BtnPlayer2Throw.IsEnabled = false;
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
