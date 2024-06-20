using System.Windows;
using System.Windows.Controls.Primitives;

namespace Bit_Shifting
{
    public partial class MainWindow : Window
    {
        private ToggleButton[] BitButtons = new ToggleButton[8];
        private int Value;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnShiftLeft.Content = "<<<";
            BtnShiftRight.Content = ">>>";
            Value = 90;
            //Place the toggleButtons in a List
            BitButtons[0] = Bit0;
            BitButtons[1] = Bit1;
            BitButtons[2] = Bit2;
            BitButtons[3] = Bit3;
            BitButtons[4] = Bit4;
            BitButtons[5] = Bit5;
            BitButtons[6] = Bit6;
            BitButtons[7] = Bit7;
            Dec2Bin(Value);
            UpdateData();
        }


        private void BitSelect(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }


        private void UpdateData()
        {
            Value = Bin2Dec(BitButtons);
            TxtDeci.Text = Value.ToString();
            TxtHexa.Text = Bin2Hex(BitButtons);
        }

        private void BtnShiftLeft_Click(object sender, RoutedEventArgs e)
        {
            Value = (Value << 1) & 0xFF;
            Dec2Bin(Value);
            UpdateData();
        }

        private void BtnShiftRight_Click(object sender, RoutedEventArgs e)
        {
            Value = Value >> 1;
            Dec2Bin(Value);
            UpdateData();
        }

        private void Dec2Bin(int val)
        {
            for (int i = 0; i < 8; i++)
            {
                BitButtons[i].IsChecked = false;
            }
            for (int i = 7; i >= 0; i--)
            {
                if (val / Math.Pow(2, i) >= 1)
                {
                    BitButtons[i].IsChecked = true;
                    val = (int)(val % Math.Pow(2, i));
                }
            }
        }

        private byte Bin2Dec(ToggleButton[] buttons)
        {
            int result = 0;
            for (int i = 0; i < 8; i++)
            {
                if (buttons[i].IsChecked == true) { result += (int)Math.Pow(2, i); }
            }
            return (byte)result;
        }

        private string Bin2Hex(ToggleButton[] buttons)
        {
            string result = "#";
            byte nibble1 = 0;
            byte nibble2 = 0;
            for (int i = 0; i < 4; i++)
            {
                if (buttons[i].IsChecked == true) { nibble1 += (byte)Math.Pow(2, i); }
                if (buttons[i + 4].IsChecked == true) { nibble2 += (byte)Math.Pow(2, i); }
            }
            result += Nibble2String(nibble2);
            result += Nibble2String(nibble1);
            return result;
        }

        private string Nibble2String(byte nibble)
        {
            string result = "";
            switch ((int)nibble)
            {
                case (<= 9):
                    result = nibble.ToString();
                    break;
                case 10:
                    result = "A";
                    break;
                case 11:
                    result = "B";
                    break;
                case 12:
                    result = "C";
                    break;
                case 13:
                    result = "D";
                    break;
                case 14:
                    result = "E";
                    break;
                case 15:
                    result = "F";
                    break;
            }
            return result;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}