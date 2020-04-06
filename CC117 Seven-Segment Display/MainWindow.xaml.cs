using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace _7SegmentDisplay
{
    public partial class MainWindow : Window
    {
        private bool Rendering = false;
        private int WaitTime = 25;
        private SevenSegmentDisplay[] display = new SevenSegmentDisplay[6];
        private int Number = 0;
        private Settings setform;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region "Window Events"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double displayleft = 0.02 * Canvas1.ActualWidth;
            double displaytop = 0.1 * Canvas1.ActualHeight;
            double displayheight = 0.8 * Canvas1.ActualHeight;
            double displayWidth = (Canvas1.ActualWidth - 50) / 6;
            for (int I = 0; I < 6; I++)
            {
                display[I] = new SevenSegmentDisplay(displayWidth, displayheight, displayleft, displaytop);
                displayleft += display[I].Width + 5;
                display[I].Value = 0;
                display[I].Draw(Canvas1);
            }
            display[2].ShowDot = true;
            setform = new Settings(this);
            setform.Show();
            setform.Left = Left + Width;
            setform.Top = Top;
            SetDefault();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded)
            {
                double displayleft = 0.02 * Canvas1.ActualWidth;
                double displaytop = 0.1 * Canvas1.ActualHeight;
                double displayheight = 0.8 * Canvas1.ActualHeight;
                double displayWidth = (Canvas1.ActualWidth - 50) / 6;
                for (int I = 0; I < 6; I++)
                {
                    display[I].Left = displayleft;
                    display[I].Top = displaytop;
                    display[I].Height = displayheight;
                    display[I].Width = displayWidth;
                    displayleft += display[I].Width + 5;
                }
                setform.Left = Left + Width;
                setform.Top = Top;
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            setform.Left = Left + Width;
            setform.Top = Top;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering )
            {
                BtnStart.Content = "STOP";
                Rendering = true;
                Render();
            }
            else
            {
                BtnStart.Content = "START";
                Rendering = false;
            }
        }

        private void Render()
        {
            while (Rendering)
            {
                Number++;
                if (Number > 999999) { Number = 0; }
                for (int I = 0; I < 6; I++)
                {
                    display[I].Value = (int)Math.Floor(Number / Math.Pow(10, 5 - I)) % 10;
                }
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
            }
        }

        public void UpdateDisplays()
        {
            for (int I = 0; I < 6; I++)
            {
                display[I].BackColor = setform.BackColor;
                display[I].Border = setform.Border;
                display[I].HasCampher = setform.HasCampher;
            if (setform.HasCampher)
                {
                    display[I].Campher = setform.Campher;
                }
            else
                {
                    display[I].Campher = 0.0;
                }
                display[I].IsTilted = setform.IsTilted;
                display[I].SegmentSpace = setform.Space;
                display[I].SegmentThickness = setform.Thickness;
                display[I].SegmentColor = setform.SegmentColor;
            }
        }

        public void SetDefault()
        {
            for (int I = 0; I < 6; I++)
            {
                display[I].SetDefault();
            }
            setform.CmbBackgroundColor.SelectedItem = "Black";
            setform.CmbSegmentColor.SelectedItem = "Red";
            setform.TxtBorder.Text = display[0].Border.ToString();
            setform.TxtCampher.Text = display[0].Campher.ToString();
            setform.TxtSpace.Text = display[0].SegmentSpace.ToString();
            setform.TxtThickness.Text = display[0].SegmentThickness.ToString();
            setform.CbHasCampher.IsChecked = display[0].HasCampher;
            setform.CbIsTilted.IsChecked = display[0].IsTilted;
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }
    }
}
