using System;
using System.Windows;
using System.Windows.Controls;

namespace _3D_Knots
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private Window MyMain;
        private int MyKnotType = 1;
        private bool AllowUpdate;
        public double A1 = 0.0;
        public double A2 = 0.0;
        public double A3 = 0.0;
        public double A4 = 0.0;
        public double A5 = 0.0;
        public double A6 = 0.0;
        public double A7 = 0.0;
        public double A8 = 0.0;
        public double B1 = 0.0;
        public double B2 = 0.0;
        public double B3 = 0.0;
        public double B4 = 0.0;
        public double B5 = 0.0;
        public double B6 = 0.0;
        public double B7 = 0.0;
        public double B8 = 0.0;
        public double C1 = 0.0;
        public double C2 = 0.0;
        public double C3 = 0.0;
        public double C4 = 0.0;
        public double C5 = 0.0;
        public double C6 = 0.0;
        public double C7 = 0.0;
        public double C8 = 0.0;

        public Settings(Window main)
        {
            InitializeComponent();
            MyMain = main;
            AllowUpdate = true;
        }

        public int KnotType
        {
            get { return MyKnotType; }
            set { MyKnotType = value; }
        }

        private void CmbKnotType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AllowUpdate = false;
            MyKnotType = CmbKnotType.SelectedIndex + 1;
            ClearParameters();
            switch (MyKnotType )
            {
                case 1:
                {
                    TxtFormulas.Text = "X = A1 * cos(A2 * mu) + A3 * cos(A4 * mu) + A5 * cos(A6 * mu) + A7 * cos(A8 * mu) \n";
                    TxtFormulas.Text += "Y = B1 * sin(B2 * mu) + B3 * sin(B4 * mu) \n";
                    TxtFormulas.Text += "Z = C1 * sin(C2 * mu) * C3 * sin(C4 * mu) + C5 * sin(C6 * mu) + C7 * sin(C8 * mu)";
                    A1 = 10.0;
                    A2 = 1.0;
                    A3 = 10.0;
                    A4 = 3.0;
                    A5 = 1.0;
                    A6 = 2.0;
                    A7 = 1.0;
                    A8 = 4.0;
                    B1 = 6.0;
                    B2 = 1.0;
                    B3 = 10.0;
                    B4 = 3.0;
                    C1 = 4.0;
                    C2 = 3.0;
                    C3 = 1.0;
                    C4 = 2.5;
                    C5 = 4.0;
                    C6 = 4.0;
                    C7 = -2.0;
                    C8 = 6.0;
                    SetEnabled("A1", true);
                    SetEnabled("A2", true);
                    SetEnabled("A3", true);
                    SetEnabled("A4", true);
                    SetEnabled("A5", true);
                    SetEnabled("A6", true);
                    SetEnabled("A7", true);
                    SetEnabled("A8", true);
                    SetEnabled("B1", true);
                    SetEnabled("B2", true);
                    SetEnabled("B3", true);
                    SetEnabled("B4", true);
                    SetEnabled("B5", false);
                    SetEnabled("B6", false);
                    SetEnabled("B7", false);
                    SetEnabled("B8", false);
                    SetEnabled("C1", true);
                    SetEnabled("C2", true);
                    SetEnabled("C3", true);
                    SetEnabled("C4", true);
                    SetEnabled("C5", true);
                    SetEnabled("C6", true);
                    SetEnabled("C7", true);
                    SetEnabled("C8", true);
                        break;
                }
                case 2:
                {
                    TxtFormulas.Text = "X = A1 * cos(A2 * mu + A3) + A4 * cos(A5 * mu \n)";
                    TxtFormulas.Text += "Y = B1 * sin(B2 * mu) + B3 * sin(B4 * mu \n)";
                    TxtFormulas.Text += "Z = C1 * sin(C2 * mu) + C3 * sin(C4 * mu)";
                    A1 = 4.0 / 3.0;
                    A2 = 1.0;
                    A3 = Math.PI;
                    A4 = 2.0;
                    A5 = 3.0;
                    B1 = 4.0 / 3.0;
                    B2 = 1.0;
                    B3 = 2.0;
                    B4 = 3.0;
                    C1 = 1.0;
                    C2 = 4.0;
                    C3 = 0.5;
                    C4 = 2.0;
                    SetEnabled("A1", true);
                    SetEnabled("A2", true);
                    SetEnabled("A3", true);
                    SetEnabled("A4", true);
                    SetEnabled("A5", true);
                    SetEnabled("A6", false);
                    SetEnabled("A7", false);
                    SetEnabled("A8", false);
                    SetEnabled("B1", true);
                    SetEnabled("B2", true);
                    SetEnabled("B3", true);
                    SetEnabled("B4", true);
                    SetEnabled("B5", false);
                    SetEnabled("B6", false);
                    SetEnabled("B7", false);
                    SetEnabled("B8", false);
                    SetEnabled("C1", true);
                    SetEnabled("C2", true);
                    SetEnabled("C3", true);
                    SetEnabled("C4", true);
                    SetEnabled("C5", false);
                    SetEnabled("C6", false);
                    SetEnabled("C7", false);
                    SetEnabled("C8", false);
                    break;
                }
                case 3:
                {
                    TxtFormulas.Text = "X = cos(mu) * (1 + cos(A1 * mu / A2) / 2.0) \n";
                    TxtFormulas.Text += "Y = sin(mu) * (1 + cos(B1 * mu / B2) / 2.0) \n";
                    TxtFormulas.Text += "Z = sin(C1 * mu / C2) / 2.0";
                    A1 = 7.0;
                    A2 = 4.0;
                    B1 = 7.0;
                    B2 = 4.0;
                    C1 = 7.0;
                    C2 = 4.0;
                    SetEnabled("A1", true);
                    SetEnabled("A2", true);
                    SetEnabled("A3", false);
                    SetEnabled("A4", false);
                    SetEnabled("A5", false);
                    SetEnabled("A6", false);
                    SetEnabled("A7", false);
                    SetEnabled("A8", false);
                    SetEnabled("B1", true);
                    SetEnabled("B2", true);
                    SetEnabled("B3", false);
                    SetEnabled("B4", false);
                    SetEnabled("B5", false);
                    SetEnabled("B6", false);
                    SetEnabled("B7", false);
                    SetEnabled("B8", false);
                    SetEnabled("C1", true);
                    SetEnabled("C2", true);
                    SetEnabled("C3", false);
                    SetEnabled("C4", false);
                    SetEnabled("C5", false);
                    SetEnabled("C6", false);
                    SetEnabled("C7", false);
                    SetEnabled("C8", false);
                    break;
                }
                case 4:
                {
                    TxtFormulas.Text = "r = A1 + A2 * sin(A3 * beta) \n";
                    TxtFormulas.Text += "theta = B1 * beta \n";
                    TxtFormulas.Text += "phi = C1 * PI * sin(C2 * beta)";
                    A1 = 0.8;
                    A2 = 1.6;
                    A3 = 6.0;
                    B1 = 2.0;
                    C1 = 0.6;
                    C2 = 12.0;
                    SetEnabled("A1", true);
                    SetEnabled("A2", true);
                    SetEnabled("A3", true);
                    SetEnabled("A4", false);
                    SetEnabled("A5", false);
                    SetEnabled("A6", false);
                    SetEnabled("A7", false);
                    SetEnabled("A8", false);
                    SetEnabled("B1", true);
                    SetEnabled("B2", false);
                    SetEnabled("B3", false);
                    SetEnabled("B4", false);
                    SetEnabled("B5", false);
                    SetEnabled("B6", false);
                    SetEnabled("B7", false);
                    SetEnabled("B8", false);
                    SetEnabled("C1", true);
                    SetEnabled("C2", true);
                    SetEnabled("C3", false);
                    SetEnabled("C4", false);
                    SetEnabled("C5", false);
                    SetEnabled("C6", false);
                    SetEnabled("C7", false);
                    SetEnabled("C8", false);
                    break;
                }
                case 5:
                {
                    TxtFormulas.Text = "r = A1 * sin(A2 * beta + A3 * PI) \n";
                    TxtFormulas.Text += "theta = B1 * beta \n";
                    TxtFormulas.Text += "phi = C1 * PI * sin(C2 * beta)";
                    A1 = 0.72;
                    A2 = 6.0;
                    A3 = 0.5;
                    B1 = 4.0;
                    C1 = 0.2;
                    C2 = 6.0;
                    SetEnabled("A1", true);
                    SetEnabled("A2", true);
                    SetEnabled("A3", true);
                    SetEnabled("A4", false);
                    SetEnabled("A5", false);
                    SetEnabled("A6", false);
                    SetEnabled("A7", false);
                    SetEnabled("A8", false);
                    SetEnabled("B1", true);
                    SetEnabled("B2", false);
                    SetEnabled("B3", false);
                    SetEnabled("B4", false);
                    SetEnabled("B5", false);
                    SetEnabled("B6", false);
                    SetEnabled("B7", false);
                    SetEnabled("B8", false);
                    SetEnabled("C1", true);
                    SetEnabled("C2", true);
                    SetEnabled("C3", false);
                    SetEnabled("C4", false);
                    SetEnabled("C5", false);
                    SetEnabled("C6", false);
                    SetEnabled("C7", false);
                    SetEnabled("C8", false);
                    break;
                }
                case 6:
                {
                    TxtFormulas.Text = "X = cos(mu) * (A1 - cos(A2 * mu / (2 * A3 + 1))) \n";
                    TxtFormulas.Text += "Y = sin(mu) * (B1 - cos(B2 * mu / (2 * A3 + 1))) \n";
                    TxtFormulas.Text += "Z = -sin(C1 * mu / (2 * A3 + 1))";
                    A1 = 2.0;
                    A2 = 2.0;
                    A3 = 2.0;
                    B1 = 2.0;
                    B2 = 2.0;
                    C1 = 2.0;
                    SetEnabled("A1", true);
                    SetEnabled("A2", true);
                    SetEnabled("A3", true);
                    SetEnabled("A4", false);
                    SetEnabled("A5", false);
                    SetEnabled("A6", false);
                    SetEnabled("A7", false);
                    SetEnabled("A8", false);
                    SetEnabled("B1", true);
                    SetEnabled("B2", true);
                    SetEnabled("B3", false);
                    SetEnabled("B4", false);
                    SetEnabled("B5", false);
                    SetEnabled("B6", false);
                    SetEnabled("B7", false);
                    SetEnabled("B8", false);
                    SetEnabled("C1", true);
                    SetEnabled("C2", false);
                    SetEnabled("C3", false);
                    SetEnabled("C4", false);
                    SetEnabled("C5", false);
                    SetEnabled("C6", false);
                    SetEnabled("C7", false);
                    SetEnabled("C8", false);
                    break;
                }
                case 7:
                {
                    TxtFormulas.Text = "X = A1 * sin(A2 * mu) * cos(A3 * mu) + A4 * cos(A5 * mu) * sin(A6 * mu) + A7 * sin(A8 * mu) \n";
                    TxtFormulas.Text += "Y = B1 * sin(B2 * mu) * sin(B3 * mu) + B4 * cos(B5 * mu) * cos(B6 * mu) + B7 * cos(B8 * mu) \n";
                    TxtFormulas.Text += "Z = C1 * cos(C2 * mu)";
                    A1 = -0.45;
                    A2 = 1.5;
                    A3 = 1.0;
                    A4 = -0.3;
                    A5 = 1.5;
                    A6 = 1.0;
                    A7 = -0.5;
                    A8 = 1.0;
                    B1 = -0.45;
                    B2 = 1.5;
                    B3 = 1.0;
                    B4 = 0.3;
                    B5 = 1.5;
                    B6 = 1.0;
                    B7 = 0.5;
                    B8 = 1.0;
                    C1 = 0.75;
                    C2 = 1.5;
                    SetEnabled("A1", true);
                    SetEnabled("A2", true);
                    SetEnabled("A3", true);
                    SetEnabled("A4", true);
                    SetEnabled("A5", true);
                    SetEnabled("A6", true);
                    SetEnabled("A7", true);
                    SetEnabled("A8", true);
                    SetEnabled("B1", true);
                    SetEnabled("B2", true);
                    SetEnabled("B3", true);
                    SetEnabled("B4", true);
                    SetEnabled("B5", true);
                    SetEnabled("B6", true);
                    SetEnabled("B7", true);
                    SetEnabled("B8", true);
                    SetEnabled("C1", true);
                    SetEnabled("C2", true);
                    SetEnabled("C3", false);
                    SetEnabled("C4", false);
                    SetEnabled("C5", false);
                    SetEnabled("C6", false);
                    SetEnabled("C7", false);
                    SetEnabled("C8", false);
                    break;
                }
                case 8:
                {
                    TxtFormulas.Text = "X = A1 * cos(A2 * mu) + A3 * sin(A4 * mu) + A5 * cos(A6 * mu) + A7 * sin(A8 * mu) \n";
                    TxtFormulas.Text += "Y = B1 * cos(B2 * mu) + B3 * sin(B4 * mu) + B5 * cos(B6 * mu) + B7 * sin(B8 * mu) \n";
                    TxtFormulas.Text += "Z = C1 * cos(C2 * mu) + C3 * sin(C4 * mu)";
                    A1 = -0.22;
                    A2 = 1.0;
                    A3 = -1.28;
                    A4 = 1.0;
                    A5 = -0.44;
                    A6 = 3.0;
                    A7 = -0.78;
                    A8 = 3.0;
                    B1 = -0.1;
                    B2 = 2.0;
                    B3 = -0.27;
                    B4 = 2.0;
                    B5 = 0.38;
                    B6 = 4.0;
                    B7 = 0.46;
                    B8 = 4.0;
                    C1 = 0.7;
                    C2 = 3.0;
                    C3 = -0.4;
                    C4 = 3.0;
                    SetEnabled("A1", true);
                    SetEnabled("A2", true);
                    SetEnabled("A3", true);
                    SetEnabled("A4", true);
                    SetEnabled("A5", true);
                    SetEnabled("A6", true);
                    SetEnabled("A7", true);
                    SetEnabled("A8", true);
                    SetEnabled("B1", true);
                    SetEnabled("B2", true);
                    SetEnabled("B3", true);
                    SetEnabled("B4", true);
                    SetEnabled("B5", true);
                    SetEnabled("B6", true);
                    SetEnabled("B7", true);
                    SetEnabled("B8", true);
                    SetEnabled("C1", true);
                    SetEnabled("C2", true);
                    SetEnabled("C3", true);
                    SetEnabled("C4", true);
                    SetEnabled("C5", false);
                    SetEnabled("C6", false);
                    SetEnabled("C7", false);
                    SetEnabled("C8", false);
                    break;
                }
            }
            UpdateParameters();
            AllowUpdate = true;
            ((MainWindow)MyMain).GetParameters();
        }

        private void ClearParameters()
        {
            A1 = 0;
            A2 = 0;
            A3 = 0;
            A4 = 0;
            A5 = 0;
            A6 = 0;
            A7 = 0;
            A8 = 0;
            B1 = 0;
            B2 = 0;
            B3 = 0;
            B4 = 0;
            B5 = 0;
            B6 = 0;
            B7 = 0;
            B8 = 0;
            C1 = 0;
            C2 = 0;
            C3 = 0;
            C4 = 0;
            C5 = 0;
            C6 = 0;
            C7 = 0;
            C8 = 0;
        }

        private void UpdateParameters()
        {
            SldA1.Value = A1;
            TxtA1.Text = A1.ToString();
            SldA2.Value = A2;
            TxtA2.Text = A2.ToString();
            SldA3.Value = A3;
            TxtA3.Text = A3.ToString();
            SldA4.Value = A4;
            TxtA4.Text = A4.ToString();
            SldA5.Value = A5;
            TxtA5.Text = A5.ToString();
            SldA6.Value = A6;
            TxtA6.Text = A6.ToString();
            SldA7.Value = A7;
            TxtA7.Text = A7.ToString();
            SldA8.Value = A8;
            TxtA8.Text = A8.ToString();
            SldB1.Value = B1;
            TxtB1.Text = B1.ToString();
            SldB2.Value = B2;
            TxtB2.Text = B2.ToString();
            SldB3.Value = B3;
            TxtB3.Text = B3.ToString();
            SldB4.Value = B4;
            TxtB4.Text = B4.ToString();
            SldB5.Value = B5;
            TxtB5.Text = B5.ToString();
            SldB6.Value = B6;
            TxtB6.Text = B6.ToString();
            SldB7.Value = B7;
            TxtB7.Text = B7.ToString();
            SldB8.Value = B8;
            TxtB8.Text = B8.ToString();
            SldC1.Value = C1;
            TxtC1.Text = C1.ToString();
            SldC2.Value = C2;
            TxtC2.Text = C2.ToString();
            SldC3.Value = C3;
            TxtC3.Text = C3.ToString();
            SldC4.Value = C4;
            TxtC4.Text = C4.ToString();
            SldC5.Value = C5;
            TxtC5.Text = C5.ToString();
            SldC6.Value = C6;
            TxtC6.Text = C6.ToString();
            SldC7.Value = C7;
            TxtC7.Text = C7.ToString();
            SldC8.Value = C8;
            TxtC8.Text = C8.ToString();
        }

        private void SetEnabled(string parameter, bool enabled)
        {
            switch (parameter)
            {
                case "A1":
                    {
                        SldA1.IsEnabled = enabled;
                        TxtA1.IsEnabled = enabled;
                        break;
                    }
                case "A2":
                    {
                        SldA2.IsEnabled = enabled;
                        TxtA2.IsEnabled = enabled;
                        break;
                    }
                case "A3":
                    {
                        SldA3.IsEnabled = enabled;
                        TxtA3.IsEnabled = enabled;
                        break;
                    }
                case "A4":
                    {
                        SldA4.IsEnabled = enabled;
                        TxtA4.IsEnabled = enabled;
                        break;
                    }
                case "A5":
                    {
                        SldA5.IsEnabled = enabled;
                        TxtA5.IsEnabled = enabled;
                        break;
                    }
                case "A6":
                    {
                        SldA6.IsEnabled = enabled;
                        TxtA6.IsEnabled = enabled;
                        break;
                    }
                case "A7":
                    {
                        SldA7.IsEnabled = enabled;
                        TxtA7.IsEnabled = enabled;
                        break;
                    }
                case "A8":
                    {
                        SldA8.IsEnabled = enabled;
                        TxtA8.IsEnabled = enabled;
                        break;
                    }
                case "B1":
                    {
                        SldB1.IsEnabled = enabled;
                        TxtB1.IsEnabled = enabled;
                        break;
                    }
                case "B2":
                    {
                        SldB2.IsEnabled = enabled;
                        TxtB2.IsEnabled = enabled;
                        break;
                    }
                case "B3":
                    {
                        SldB3.IsEnabled = enabled;
                        TxtB3.IsEnabled = enabled;
                        break;
                    }
                case "B4":
                    {
                        SldB4.IsEnabled = enabled;
                        TxtB4.IsEnabled = enabled;
                        break;
                    }
                case "B5":
                    {
                        SldB5.IsEnabled = enabled;
                        TxtB5.IsEnabled = enabled;
                        break;
                    }
                case "B7":
                    {
                        SldB7.IsEnabled = enabled;
                        TxtB7.IsEnabled = enabled;
                        break;
                    }
                case "B8":
                    {
                        SldB8.IsEnabled = enabled;
                        TxtB8.IsEnabled = enabled;
                        break;
                    }
                case "C1":
                    {
                        SldC1.IsEnabled = enabled;
                        TxtC1.IsEnabled = enabled;
                        break;
                    }
                case "C2":
                    {
                        SldC2.IsEnabled = enabled;
                        TxtC2.IsEnabled = enabled;
                        break;
                    }
                case "C3":
                    {
                        SldC3.IsEnabled = enabled;
                        TxtC3.IsEnabled = enabled;
                        break;
                    }
                case "C4":
                    {
                        SldC4.IsEnabled = enabled;
                        TxtC4.IsEnabled = enabled;
                        break;
                    }
                case "C5":
                    {
                        SldC5.IsEnabled = enabled;
                        TxtC5.IsEnabled = enabled;
                        break;
                    }
                case "C7":
                    {
                        SldC7.IsEnabled = enabled;
                        TxtC7.IsEnabled = enabled;
                        break;
                    }
                case "C8":
                    {
                        SldC8.IsEnabled = enabled;
                        TxtC8.IsEnabled = enabled;
                        break;
                    }
            }
        }

        private void ParameterChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AllowUpdate)
            {
                A1 = SldA1.Value;
                TxtA1.Text = A1.ToString();
                A2 = SldA2.Value;
                TxtA2.Text = A2.ToString();
                A3 = SldA3.Value;
                TxtA3.Text = A3.ToString();
                A4 = SldA4.Value;
                TxtA4.Text = A4.ToString();
                A5 = SldA5.Value;
                TxtA5.Text = A5.ToString();
                A6 = SldA6.Value;
                TxtA6.Text = A6.ToString();
                A7 = SldA7.Value;
                TxtA7.Text = A7.ToString();
                A8 = SldA8.Value;
                TxtA8.Text = A8.ToString();
                B1 = SldB1.Value;
                TxtB1.Text = B1.ToString();
                B2 = SldB2.Value;
                TxtB2.Text = B2.ToString();
                B3 = SldB3.Value;
                TxtB3.Text = B3.ToString();
                B4 = SldB4.Value;
                TxtB4.Text = B4.ToString();
                B5 = SldB5.Value;
                TxtB5.Text = B5.ToString();
                B6 = SldB6.Value;
                TxtB6.Text = B6.ToString();
                B7 = SldB7.Value;
                TxtB7.Text = B7.ToString();
                B8 = SldB8.Value;
                TxtB8.Text = B8.ToString();
                C1 = SldC1.Value;
                TxtC1.Text = C1.ToString();
                C2 = SldC2.Value;
                TxtC2.Text = C2.ToString();
                C3 = SldC3.Value;
                TxtC3.Text = C3.ToString();
                C4 = SldC4.Value;
                TxtC4.Text = C4.ToString();
                C5 = SldC5.Value;
                TxtC5.Text = C5.ToString();
                C6 = SldC6.Value;
                TxtC6.Text = C6.ToString();
                C7 = SldC7.Value;
                TxtC7.Text = C7.ToString();
                C8 = SldC8.Value;
                TxtC8.Text = C8.ToString();
                ((MainWindow)MyMain).GetParameters();
            }
        }

        private void TxtRotation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllowUpdate)
            {
                double rotSpeed = double.Parse(TxtRotation.Text);
                ((MainWindow)MyMain).SetRotation(rotSpeed);
            }
        }

        private void CBTexture_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate)
            {
                ((MainWindow)MyMain).SetShowTexture(CBTexture.IsChecked.Value);
            }
        }

        private void CmbDrawMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllowUpdate)
            {
                ((MainWindow)MyMain).SetDrawMode(CmbDrawMode.SelectedIndex);
            }
        }
    }
}
