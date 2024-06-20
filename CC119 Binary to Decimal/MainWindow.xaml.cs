using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Binary_Conversions
{
    public partial class MainWindow : Window
    {
        List<Color> MyColors;
        private ToggleButton[] RedButtons = new ToggleButton[8];
        private byte redDeci;
        private ToggleButton[] GreenButtons = new ToggleButton[8];
        private byte greenDeci;
        private ToggleButton[] BlueButtons = new ToggleButton[8];
        private byte blueDeci;
        private Color myColor;
        private bool AppLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Fill the combobox with all standard WPF brushes
            MyColors = new List<Color>();
            Type brushType = typeof(Brushes);
            BrushConverter bc = new BrushConverter();
            ColorConverter cc = new ColorConverter();
            foreach (System.Reflection.PropertyInfo propinfo in brushType.GetProperties())
            {
                if (propinfo.PropertyType == typeof(SolidColorBrush))
                {
                    CmbColors.Items.Add(propinfo.Name);
                    MyColors.Add((Color)cc.ConvertFromInvariantString(propinfo.Name));
                }
            }
            CmbColors.SelectedIndex = 7;
            //Place the toggleButtons
            RedButtons[0] = Red0;
            RedButtons[1] = Red1;
            RedButtons[2] = Red2;
            RedButtons[3] = Red3;
            RedButtons[4] = Red4;
            RedButtons[5] = Red5;
            RedButtons[6] = Red6;
            RedButtons[7] = Red7;
            GreenButtons[0] = Green0;
            GreenButtons[1] = Green1;
            GreenButtons[2] = Green2;
            GreenButtons[3] = Green3;
            GreenButtons[4] = Green4;
            GreenButtons[5] = Green5;
            GreenButtons[6] = Green6;
            GreenButtons[7] = Green7;
            BlueButtons[0] = Blue0;
            BlueButtons[1] = Blue1;
            BlueButtons[2] = Blue2;
            BlueButtons[3] = Blue3;
            BlueButtons[4] = Blue4;
            BlueButtons[5] = Blue5;
            BlueButtons[6] = Blue6;
            BlueButtons[7] = Blue7;
            AppLoaded = true;
        }

        private void BitSelect(object sender, RoutedEventArgs e)
        {
            UpdateData();
            CmbColors.SelectedIndex = -1;
        }

        private void CmbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppLoaded && CmbColors.SelectedIndex >= 0)
            {
                Color c = MyColors[CmbColors.SelectedIndex];
                canvas1.Background = new SolidColorBrush(c);
                redDeci = c.R;
                bool[] redBits = Dec2Bin(redDeci);
                greenDeci = c.G;
                bool[] greenBits = Dec2Bin(greenDeci);
                blueDeci = c.B;
                bool[] blueBits = Dec2Bin(blueDeci);
                for (int i = 0; i < 8; i++)
                {
                    RedButtons[i].IsChecked = redBits[i];
                    GreenButtons[i].IsChecked = greenBits[i];
                    BlueButtons[i].IsChecked = blueBits[i];
                }
                UpdateData();
            }
        }

        private byte Bin2Dec(ToggleButton[] buttons)
        {
            int result = 0;
            for (int i = 0; i < 8; i++)
            {
                if (buttons[i].IsChecked == true ) { result += (int)Math.Pow(2, i); }
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

        private void UpdateData()
        {
            redDeci = Bin2Dec(RedButtons);
            TxtRedDeci.Text = redDeci.ToString();
            TxtRedHexa.Text = Bin2Hex(RedButtons);
            greenDeci = Bin2Dec(GreenButtons);
            TxtGreenDeci.Text = greenDeci.ToString();
            TxtGreenHexa.Text = Bin2Hex(GreenButtons);
            blueDeci = Bin2Dec(BlueButtons);
            TxtBlueDeci.Text = blueDeci.ToString();
            TxtBlueHexa.Text = Bin2Hex(BlueButtons);
            myColor = Color.FromRgb(redDeci, greenDeci, blueDeci);
            canvas1.Background = new SolidColorBrush(myColor);
        }

        private bool[] Dec2Bin(byte b)
        {
            bool[] result = new bool[8];
            for (int i = 7; i >=0; i--)
            {
                if (b / Math.Pow(2, i) >= 1 )
                {
                    result[i] = true;
                    b = (byte)(b - Math.Pow(2, i));
                }
            }
            return result;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}