using System.Windows;
using System.Windows.Media;

namespace CountDown_Timer
{
    public partial class MainWindow : Window
    {
        private bool my_Started = false;
        private bool BGAlarmColor = false;
        private int AlarmCounter = 0;
        private int my_TotalSeconds = 0;
        private DateTime my_StartTime;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            SetTimerDialog td = new SetTimerDialog();
            td.Left = Left + 25;
            td.Top = Top + 25;
            if (td.ShowDialog() == true)
            {
                my_TotalSeconds = td.TotalSeconds;
                ShowTime(my_TotalSeconds);
                btnStart.IsEnabled = true;
                btnReset.IsEnabled = false;
                LblTime.Background = Brushes.White;
                AlarmCounter = 0;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!my_Started)
            {
                btnStart.Content = "STOP";
                btnSet.IsEnabled = false;
                btnReset.IsEnabled= false;
                my_StartTime = DateTime.Now;
                my_Started = true;
                LblTime.Background = Brushes.White;
                AlarmCounter = 0;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                btnStart.Content = "START";
                btnStart.IsEnabled = false;
                btnSet.IsEnabled = true;
                btnReset.IsEnabled = true;
                my_Started = false;
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (!my_Started) { return; }
            DateTime Current = DateTime.Now;
            int ElapsedTime = (int)(Current - my_StartTime).TotalSeconds;
            int Remaining = my_TotalSeconds - ElapsedTime;
            if (Remaining <= 0)
            {
                Remaining = 0;
                AlarmCounter += 1;
                if (AlarmCounter < 30)
                {
                    if (!BGAlarmColor)
                    {
                        LblTime.Background = Brushes.Red;
                    }
                    else
                    {
                        LblTime.Background = Brushes.White;
                    }
                    BGAlarmColor = !BGAlarmColor;
                    System.Media.SystemSounds.Exclamation.Play();
                    AlarmCounter += 1;
                }
                else
                {
                    LblTime.Background = Brushes.White;
                    btnStart.Content = "START";
                    btnStart.IsEnabled = false;
                    btnSet.IsEnabled = true;
                    btnReset.IsEnabled = true;
                    my_Started = false;
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                }
            }
            ShowTime(Remaining);
            Thread.Sleep(500);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Content = "START";
            btnStart.IsEnabled = true;
            btnReset.IsEnabled = false;
            my_Started = false;
            LblTime.Background = Brushes.White;
            AlarmCounter = 0;
            ShowTime(my_TotalSeconds);
        }

        private void ShowTime(int seconds)
        {
            int my_Hours;
            int my_Minutes;
            int my_Seconds;
            my_Hours = seconds / 3600;
            seconds -= 3600 * my_Hours;
            my_Minutes = seconds / 60;
            seconds -= 60 * my_Minutes;
            my_Seconds = seconds;
            DateTime now = DateTime.Now;
            DateTime dt = new DateTime(now.Year, now.Month, now.Day, my_Hours, my_Minutes, my_Seconds);
            LblTime.Content = dt.ToString("HH:mm:ss");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}