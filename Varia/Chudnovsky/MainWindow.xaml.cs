using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Numerics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chudnovsky
{
    public partial class MainWindow : Window
    {
        bool started = false;
        int Precision = 0;
        int k = -1;
        BigDecimal T = new BigDecimal(0, 0);
        BigDecimal A = new BigDecimal(0, 0);
        BigDecimal B = new BigDecimal(0, 0);
        BigDecimal N = new BigDecimal(0, 0);
        BigDecimal BigPi = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Chudnovsky PI Calculation Algorithm";
            TxtTruePi.TextWrapping = TextWrapping.Wrap;
            TxtPi.TextWrapping = TextWrapping.Wrap;
            k = -1;
            started = false;
            Precision = 2000;
            string PiChars = "";
            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\PI_1000000.txt");
            for (int I = 0; I <= 2000; I++)
            {
                PiChars += (char)sr.Read();
            }
            sr.Close();
            TxtTruePi.Text = PiChars;
            BigDecimal.Precision = Precision + 1;
            BigDecimal.Notation = NotationType.Scientific;
            T = BigDecimal.SQRT(new BigDecimal(10005, 0));
            T = 426880 * T;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            int correctDigits = 0;
            BigInteger A1 = 545140134;
            BigInteger A2 = 13591409;
            if (!started)
            {
                N = 0;
                started = true;
                BtnNext.Content = "NEXT";
            }
            k += 1;
            A1 = A1 * k + A2;
            A.Mantissa = Fact(6 * k) * A1;
            B.Mantissa = Fact(3 * k) * Fact(k) * Fact(k) * Fact(k);
            for (int I = 1; I <= k; I++)
            {
                B.Mantissa = B.Mantissa * (-262537412640768000);
            }
            N += A / B;
            BigPi = T / N;
            BigPi.Truncate(Precision);
            TxtPi.Text = BigPi.ToString();
            TxtIter.Text = k.ToString();
            //Determine the number of accurate digits calculated
            for (int I = 0; I < TxtTruePi.Text.Length; I++)
            {
                if (I < TxtPi.Text.Length)
                {
                    if (TxtPi.Text[I] == TxtTruePi.Text[I])
                    {
                        correctDigits += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            TxtCorrect.Text = (correctDigits - 1).ToString();
            TxtPi.Text = TxtPi.Text.Substring(0, correctDigits);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private BigInteger Fact(int N)
        {
            BigInteger result = 1;
            for (int I = 1; I <= N; I++)
            {
                result = result * I;
            }
            return result;
        }

        //private void TxtAccuracy_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (!MyLoaded) return;
        //    try
        //    {
        //        Precision = int.Parse(TxtAccuracy.Text);
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Invalid Precision", "Chudnovsky Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }
        //    if (Precision > 5000)
        //    {
        //        Precision = 5000;
        //        TxtTruePi.Text = "";
        //        TxtPi.Text = "";
        //        BtnNext.Content = "START";
        //        k = -1;
        //        started = false;
        //    }
        //}
    }
}