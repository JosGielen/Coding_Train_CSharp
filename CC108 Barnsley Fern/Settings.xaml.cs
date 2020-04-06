using System;
using JG_Matrix;
using System.Windows;

namespace Barnsley_Ferns
{
    public partial class Settings : Window
    {
        private MainWindow MyMain;
        private double my_p1 = 0.0;
        private double my_p2 = 0.0;
        private double my_p3 = 0.0;
        private double my_p4 = 0.0;
        private Matrix my_F1;
        private Matrix my_F2;
        private Matrix my_F3;
        private Matrix my_F4;
        private Matrix my_C1;
        private Matrix my_C2;
        private Matrix my_C3;
        private Matrix my_C4;
        private string my_Type;

        public Settings(MainWindow parent, string name)
        {
            InitializeComponent();
            MyMain = parent;
            my_F1 = new Matrix(2, 2);
            my_F2 = new Matrix(2, 2);
            my_F3 = new Matrix(2, 2);
            my_F4 = new Matrix(2, 2);
            my_C1 = new Matrix(2, 1);
            my_C2 = new Matrix(2, 1);
            my_C3 = new Matrix(2, 1);
            my_C4 = new Matrix(2, 1);
            my_Type = name;
            TxtFernType.Text = my_Type;
        }

        #region "Properties"

        public double P1
        {
            get { return my_p1; }
            set 
            {
                my_p1 = value;
                TxtP1.Text = my_p1.ToString();
            }
        }

        public double P2
        {
            get { return my_p2; }
            set
            {
                my_p2 = value;
                TxtP2.Text = my_p2.ToString();
            }
        }

        public double P3
        {
            get { return my_p3; }
            set
            {
                my_p3 = value;
                TxtP3.Text = my_p3.ToString();
            }
        }

        public double P4
        {
            get { return my_p4; }
            set
            {
                my_p4 = value;
                TxtP4.Text = my_p4.ToString();
            }
        }

        public Matrix F1
        {
            get
            {
                my_F1.SetValue(0, 0, double.Parse(TxtF1X1.Text));
                my_F1.SetValue(0, 1, double.Parse(TxtF1Y1.Text));
                my_F1.SetValue(1, 0, double.Parse(TxtF1X2.Text));
                my_F1.SetValue(1, 1, double.Parse(TxtF1Y2.Text));
                return my_F1;
            }
            set 
            {
                my_F1 = (Matrix)value.Clone();
                TxtF1X1.Text = my_F1.GetValue(0, 0).ToString();
                TxtF1Y1.Text = my_F1.GetValue(0, 1).ToString();
                TxtF1X2.Text = my_F1.GetValue(1, 0).ToString();
                TxtF1Y2.Text = my_F1.GetValue(1, 1).ToString();
            }
        }

        public Matrix F2
        {
            get
            {
                my_F2.SetValue(0, 0, double.Parse(TxtF2X1.Text));
                my_F2.SetValue(0, 1, double.Parse(TxtF2Y1.Text));
                my_F2.SetValue(1, 0, double.Parse(TxtF2X2.Text));
                my_F2.SetValue(1, 1, double.Parse(TxtF2Y2.Text));
                return my_F2;
            }
            set
            {
                my_F2 = (Matrix)value.Clone();
                TxtF2X1.Text = my_F2.GetValue(0, 0).ToString();
                TxtF2Y1.Text = my_F2.GetValue(0, 1).ToString();
                TxtF2X2.Text = my_F2.GetValue(1, 0).ToString();
                TxtF2Y2.Text = my_F2.GetValue(1, 1).ToString();
            }
        }

        public Matrix F3
        {
            get
            {
                my_F3.SetValue(0, 0, double.Parse(TxtF3X1.Text));
                my_F3.SetValue(0, 1, double.Parse(TxtF3Y1.Text));
                my_F3.SetValue(1, 0, double.Parse(TxtF3X2.Text));
                my_F3.SetValue(1, 1, double.Parse(TxtF3Y2.Text));
                return my_F3;
            }
            set
            {
                my_F3 = (Matrix)value.Clone();
                TxtF3X1.Text = my_F3.GetValue(0, 0).ToString();
                TxtF3Y1.Text = my_F3.GetValue(0, 1).ToString();
                TxtF3X2.Text = my_F3.GetValue(1, 0).ToString();
                TxtF3Y2.Text = my_F3.GetValue(1, 1).ToString();
            }
        }

        public Matrix F4
        {
            get
            {
                my_F4.SetValue(0, 0, double.Parse(TxtF4X1.Text));
                my_F4.SetValue(0, 1, double.Parse(TxtF4Y1.Text));
                my_F4.SetValue(1, 0, double.Parse(TxtF4X2.Text));
                my_F4.SetValue(1, 1, double.Parse(TxtF4Y2.Text));
                return my_F4;
            }
            set
            {
                my_F4 = (Matrix)value.Clone();
                TxtF4X1.Text = my_F4.GetValue(0, 0).ToString();
                TxtF4Y1.Text = my_F4.GetValue(0, 1).ToString();
                TxtF4X2.Text = my_F4.GetValue(1, 0).ToString();
                TxtF4Y2.Text = my_F4.GetValue(1, 1).ToString();
            }
        }

        public Matrix C1
        {
            get
            {
                my_C1.SetValue(0, 0, double.Parse(TxtC1X.Text));
                my_C1.SetValue(1, 0, double.Parse(TxtC1Y.Text));
                return my_C1;
            }
            set
            {
                my_C1 = (Matrix)value.Clone();
                TxtC1X.Text = my_C1.GetValue(0, 0).ToString();
                TxtC1Y.Text = my_C1.GetValue(1, 0).ToString();
            }
        }


        public Matrix C2
        {
            get
            {
                my_C2.SetValue(0, 0, double.Parse(TxtC2X.Text));
                my_C2.SetValue(1, 0, double.Parse(TxtC2Y.Text));
                return my_C2;
            }
            set
            {
                my_C2 = (Matrix)value.Clone();
                TxtC2X.Text = my_C2.GetValue(0, 0).ToString();
                TxtC2Y.Text = my_C2.GetValue(1, 0).ToString();
            }
        }


        public Matrix C3
        {
            get
            {
                my_C3.SetValue(0, 0, double.Parse(TxtC3X.Text));
                my_C3.SetValue(1, 0, double.Parse(TxtC3Y.Text));
                return my_C3;
            }
            set
            {
                my_C3 = (Matrix)value.Clone();
                TxtC3X.Text = my_C3.GetValue(0, 0).ToString();
                TxtC3Y.Text = my_C3.GetValue(1, 0).ToString();
            }
        }

        public Matrix C4
        {
            get
            {
                my_C4.SetValue(0, 0, double.Parse(TxtC4X.Text));
                my_C4.SetValue(1, 0, double.Parse(TxtC4Y.Text));
                return my_C4;
            }
            set
            {
                my_C4 = (Matrix)value.Clone();
                TxtC4X.Text = my_C4.GetValue(0, 0).ToString();
                TxtC4Y.Text = my_C4.GetValue(1, 0).ToString();
            }
        }

        public string Type
        {
            get 
            { 
                my_Type = TxtFernType.Text;
                return my_Type;
            }
            set
            {
                my_Type = value;
                TxtFernType.Text = my_Type;
            }
        }

        #endregion 

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((string)BtnStart.Content == "Start")
                {
                    ((MainWindow)MyMain).Start();
                    BtnStart.Content = "Stop";
                }
            else
                {
                    ((MainWindow)MyMain).Halt();
                    BtnStart.Content = "Start";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The Parameters are not valid.", "Barnsley Fern settings error", MessageBoxButton.OK, MessageBoxImage.Error);            
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)MyMain).MnuShowSettings.IsChecked = false;
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
