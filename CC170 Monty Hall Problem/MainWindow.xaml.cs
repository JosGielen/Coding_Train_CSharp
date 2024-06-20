using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Monty_Hall_Problem
{
    public partial class MainWindow : Window
    {
        private int Total;
        private int Switched;
        private int SwitchedWon;
        private int Stayed;
        private int StayededWon;
        private Door[] Doors;
        private int WinDoorNr;
        private int ChosenDoorNr;
        private int OpenDoorNr;
        private int Status;
        private bool autoOn = false;
        private bool ShowGUI;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Doors = new Door[3];
            Doors[0] = new Door(canvas1, Lbl1, image1);
            Doors[1] = new Door(canvas2, Lbl2, image2);
            Doors[2] = new Door(canvas3, Lbl3, image3);
            Init();
        }

        private void Init()
        {
            Total = 0;
            Switched = 0;
            SwitchedWon = 0;
            TxtTotal.Text = Total.ToString();
            TxtStayed.Text = "0.0%";
            TxtSwitched.Text = "0.0%";
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            ShowGUI = true;
            Setup();
        }

        private void BtnAuto_Click(object sender, RoutedEventArgs e)
        {
            if (!autoOn)
            {
                ShowGUI = false;
                autoOn = true;
                BtnAuto.Content = "STOP";
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                autoOn = false;
                BtnAuto.Content = "AUTO";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            Setup();
            DoorSelected(Rnd.Next(3));
            if (ShowGUI)
            {
                Thread.Sleep(100);
            }
            DoorSelected(Rnd.Next(3));
            if (ShowGUI)
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(10);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void canvas1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DoorSelected(0);
        }

        private void canvas2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DoorSelected(1);
        }

        private void canvas3_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DoorSelected(2);
        }

        private void Setup()
        {
            //Reset the Game
            //Choose a random winning door 
            WinDoorNr = Rnd.Next(3);
            for (int i = 0; i < 3; i++)
            {
                if (i == WinDoorNr)
                {
                    Doors[i].Init(true);
                }
                else
                {
                    Doors[i].Init(false);
                }
            }
            if (ShowGUI) { LblMessage.Content = "Select one of the Doors."; }
            Status = 1;
        }

        private void DoorSelected(int doorNr)
        {
            if (Status == 1) //Initial door selection
            {
                ChosenDoorNr = doorNr;
                if (ShowGUI)
                {
                    Doors[ChosenDoorNr].Selected = true;
                }
                //Open another door that is not winning
                do
                {
                    OpenDoorNr = Rnd.Next(3);

                } while (OpenDoorNr == WinDoorNr || OpenDoorNr == doorNr);
                if (ShowGUI)
                {
                    Doors[OpenDoorNr].ShowContent();
                    LblMessage.Content = "Do you want to Stay or Switch?";
                }
                Status = 2;
                return;
            }
            if (Status == 2) //Second door Selection (Stay or Switch)
            {
                if (doorNr == OpenDoorNr) { return; }
                Total++;
                if (ShowGUI)
                {
                    Doors[doorNr].ShowContent();
                }
                if (ChosenDoorNr == doorNr) //Stayed
                {
                    Stayed++;
                    if (WinDoorNr == doorNr)
                    {
                        StayededWon++;
                        if (ShowGUI)
                        {
                            LblMessage.Content = "Congratulations You won.";
                        }
                    }
                    else
                    {
                        if (ShowGUI)
                        {
                            LblMessage.Content = "Sorry You lost.";
                        }
                    }
                }
                else //Switched
                {
                    Switched++;
                    if (WinDoorNr == doorNr)
                    {
                        SwitchedWon++;
                        if (ShowGUI)
                        {
                            LblMessage.Content = "Congratulations You won.";
                        }
                    }
                    else
                    {
                        if (ShowGUI)
                        {
                            LblMessage.Content = "Sorry You lost.";
                        }
                    }
                }
                Status = 0;
                //Update the Textboxes
                TxtTotal.Text = Total.ToString();
                TxtStayed.Text = (100 * StayededWon / (double)Stayed).ToString("F2") + "%";
                TxtSwitched.Text = (100 * SwitchedWon / (double)Switched).ToString("F2") + "%";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}