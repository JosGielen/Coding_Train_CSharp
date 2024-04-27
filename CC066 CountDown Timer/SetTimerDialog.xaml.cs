using System.Windows;

namespace CountDown_Timer
{
    /// <summary>
    /// Interaction logic for SetTimerDialog.xaml
    /// </summary>
    public partial class SetTimerDialog : Window
    {
        private int my_Hours;
        private int my_Minutes;
        private int my_Seconds;
        private int my_TotalSeconds;
        private int maxSeconds = 359999;

        public SetTimerDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            my_Hours = 0;
            my_Minutes = 0;
            my_Seconds = 0;
            my_TotalSeconds = 0;
        }

        public int Hours
        {
            get { return my_Hours; }
        }

        public int Minutes
        {
            get { return my_Minutes; }
        }

        public int Seconds
        { 
            get { return my_Seconds; } 
        }

        public int TotalSeconds
        {
            get { return my_TotalSeconds; }
        }

        private void BtnHourUP_Click(object sender, RoutedEventArgs e)
        {
            if (my_TotalSeconds < maxSeconds - 3600)
            {
                my_TotalSeconds += 3600;
                UpdateTextboxes(my_TotalSeconds);
            }
        }

        private void BtnHourDown_Click(object sender, RoutedEventArgs e)
        {
            if (my_TotalSeconds > 3600)
            {
                my_TotalSeconds -= 3600;
                UpdateTextboxes(my_TotalSeconds);
            }
        }

        private void BtnMinuteUP_Click(object sender, RoutedEventArgs e)
        {
            if (my_TotalSeconds < maxSeconds - 60)
            {
                my_TotalSeconds += 60;
                UpdateTextboxes(my_TotalSeconds);
            }
        }

        private void BtnMinuteDown_Click(object sender, RoutedEventArgs e)
            {
            if (my_TotalSeconds > 60)
            {
                my_TotalSeconds -= 60;
                UpdateTextboxes(my_TotalSeconds);
            }
        }

        private void BtnSecondUP_Click(object sender, RoutedEventArgs e)
        {
            if (my_TotalSeconds < maxSeconds)
            {
                my_TotalSeconds ++;
                UpdateTextboxes(my_TotalSeconds);
            }
        }

        private void BtnSecondDown_Click(object sender, RoutedEventArgs e)
        {
            if (my_TotalSeconds > 0)
            {
                my_TotalSeconds --;
                UpdateTextboxes(my_TotalSeconds);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdateTextboxes(int seconds)
        {
            my_Hours = seconds / 3600;
            seconds -= 3600 * my_Hours;
            my_Minutes = seconds / 60;
            seconds -= 60 * my_Minutes;
            my_Seconds = seconds;
            TxtHour.Text = my_Hours.ToString();
            TxtMinute.Text = my_Minutes.ToString();
            TxtSecond.Text = my_Seconds.ToString();
        }
    }
}



