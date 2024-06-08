using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Book_of_PI
{
    public partial class MainWindow : Window
    {
        private Color[] my_Colors;
        private double my_Size;
        private List<PIChar> my_CharList;
        private int Cols = 300;
        private int Rows;
        private int maxCount;
        private double my_FontSize = 6;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            my_CharList = new List<PIChar>();
            PIChar pc;
            string pchar;
            int nr;
            int Count = 0;
            my_Size = canvas1.ActualWidth / Cols;
            Rows = (int)(canvas1.ActualHeight / my_Size);
            maxCount = Rows * Cols;
            my_Colors = new Color[10];
            my_Colors[0] = Colors.Blue;
            my_Colors[1] = Colors.Cyan;
            my_Colors[2] = Colors.Green;
            my_Colors[3] = Colors.YellowGreen;
            my_Colors[4] = Colors.Yellow;
            my_Colors[5] = Colors.Orange;
            my_Colors[6] = Colors.OrangeRed;
            my_Colors[7] = Colors.Red;
            my_Colors[8] = Colors.Purple;
            my_Colors[9] = Colors.Brown;
            CB0.Background = new SolidColorBrush(my_Colors[0]);
            CB1.Background = new SolidColorBrush(my_Colors[1]);
            CB2.Background = new SolidColorBrush(my_Colors[2]);
            CB3.Background = new SolidColorBrush(my_Colors[3]);
            CB4.Background = new SolidColorBrush(my_Colors[4]);
            CB5.Background = new SolidColorBrush(my_Colors[5]);
            CB6.Background = new SolidColorBrush(my_Colors[6]);
            CB7.Background = new SolidColorBrush(my_Colors[7]);
            CB8.Background = new SolidColorBrush(my_Colors[8]);
            CB9.Background = new SolidColorBrush(my_Colors[9]);
            using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\PI_1000000.txt"))
            {
                while (!sr.EndOfStream)
                {
                    if (Count >= maxCount) { break; }
                    pchar = ((char)sr.Read()).ToString();
                    nr = 0;
                    int.TryParse(pchar, out nr);
                    pc = new PIChar(Count, my_Size, pchar, my_Colors[nr]);
                    pc.Draw(canvas1);
                    my_CharList.Add(pc);
                    Count++;
                }
            }
        }

        private void CharSelect(object sender, RoutedEventArgs e)
        {
            CBAll.IsChecked=false;
            CBNone.IsChecked=false;
            Update();
        }

        private void CBNone_Click(object sender, RoutedEventArgs e)
        {
            if (CBNone.IsChecked == true)
            {
                CB0.IsChecked = false;
                CB1.IsChecked = false;
                CB2.IsChecked = false;
                CB3.IsChecked = false;
                CB4.IsChecked = false;
                CB5.IsChecked = false;
                CB6.IsChecked = false;
                CB7.IsChecked = false;
                CB8.IsChecked = false;
                CB9.IsChecked = false;
            }
            Update();
        }

        private void CBAll_Click(object sender, RoutedEventArgs e)
        {
            if (CBAll.IsChecked == true)
            {
                CB0.IsChecked = true;
                CB1.IsChecked = true;
                CB2.IsChecked = true;
                CB3.IsChecked = true;
                CB4.IsChecked = true;
                CB5.IsChecked = true;
                CB6.IsChecked = true;
                CB7.IsChecked = true;
                CB8.IsChecked = true;
                CB9.IsChecked = true;
            }
            Update();
        }

        private void Update()
        {
            bool[] charSel = [
                CB0.IsChecked.Value,
                CB1.IsChecked.Value,
                CB2.IsChecked.Value,
                CB3.IsChecked.Value,
                CB4.IsChecked.Value,
                CB5.IsChecked.Value,
                CB6.IsChecked.Value,
                CB7.IsChecked.Value,
                CB8.IsChecked.Value,
                CB9.IsChecked.Value,
            ];
            for (int i = 0; i < my_CharList.Count; i++)
            {
                if (charSel[my_CharList[i].Nr])
                {
                    my_CharList[i].Show();
                }
                else
                {
                    my_CharList[i].Hide();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

    }
}