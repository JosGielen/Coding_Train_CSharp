using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace L_Systems_General
{
    public partial class Settings : Window
    {
        private Window MyMain;
        private List<Brush> MyBrushes;
        private List<string> MyBrushStrings;
        private string MyFractalType = "";
        private string MyInitString = "";
        private double MyInitialLength = 0.0;
        private double MyInitialAngle = 0;
        private double MyInitialAngleVariation = 0;
        private double MyStartPosX = 0.0;
        private double MyStartPosY = 0.0;
        private double MyDeflectionAngle = 0;
        private double MyLengthScaling = 0.0;
        private Brush MyColor = Brushes.Red;
        private int MyStartIter = 0;
        private int MyMaxIter = 0;
        private string MyArule = "";
        private string MyBrule = "";
        private string MyCrule = "";
        private string MyXrule = "";
        private string MyYrule = "";
        private string MyZrule = "";

        public Settings(Window main)
        {
            InitializeComponent();
            MyMain = main;
            //Fill the combobox with all standard WPF brushes
            MyBrushes = new List<Brush>();
            MyBrushStrings = new List<string>();
            Type brushType = typeof(Brushes);
            BrushConverter bc = new BrushConverter();
            foreach (System.Reflection.PropertyInfo propinfo in brushType.GetProperties())
            {
                if (propinfo.PropertyType == typeof(SolidColorBrush))
                {
                    CmbColors.Items.Add(propinfo.Name);
                    MyBrushes.Add((Brush)bc.ConvertFromString(propinfo.Name));
                    MyBrushStrings.Add(((Brush)bc.ConvertFromString(propinfo.Name)).ToString());
                }
            }
        }

        public string FractalType
        {
            get { return MyFractalType; }
            set
            {
                MyFractalType = value;
                TxtFractalType.Text = MyFractalType;
            }
        }

        public string InitString
        {
            get { return MyInitString; }
            set
            {
                MyInitString = value;
                TxtInitialString.Text = MyInitString;
            }
        }

        public double InitialLength
        {
            get { return MyInitialLength; }
            set
            {
                MyInitialLength = value;
                TxtInitialLength.Text = MyInitialLength.ToString();
            }
        }

        public double InitialAngle
        {
            get { return MyInitialAngle; }
            set
            {
                MyInitialAngle = value;
                TxtInitialAngle.Text = MyInitialAngle.ToString();
            }
        }

        public double InitialAngleVariation
        {
            get { return MyInitialAngleVariation; }
            set
            {
                MyInitialAngleVariation = value;
                TxtAngleVariation.Text = MyInitialAngleVariation.ToString();
            }
        }

        public double StartPosX
        {
            get { return MyStartPosX; }
            set
            {
                MyStartPosX = value;
                TxtStartPosX.Text = MyStartPosX.ToString();
            }
        }

        public double StartPosY
        {
            get { return MyStartPosY; }
            set
            {
                MyStartPosY = value;
                TxtStartPosY.Text = MyStartPosY.ToString();
            }
        }

        public double DeflectionAngle
        {
            get { return MyDeflectionAngle; }
            set
            {
                MyDeflectionAngle = value;
                TxtDeflectionAngle.Text = MyDeflectionAngle.ToString();
            }
        }

        public double LengthScaling
        {
            get { return MyLengthScaling; }
            set
            {
                MyLengthScaling = value;
                TxtLengthScale.Text = MyLengthScaling.ToString();
            }
        }

        public Brush Color
        {
            get { return MyColor; }
            set
            {
                MyColor = value;
                int index = MyBrushStrings.IndexOf(MyColor.ToString());
                CmbColors.SelectedIndex = index; //MyBrushes.IndexOf(MyColor)
            }
        }

        public int StartIter
        {
            get { return MyStartIter; }
            set
            {
                MyStartIter = value;
                TxtStartIteration.Text = MyStartIter.ToString();
            }
        }

        public int MaxIter
        {
            get { return MyMaxIter; }
            set
            {
                MyMaxIter = value;
                TxtMaxIterations.Text = MyMaxIter.ToString();
            }
        }

        public string A_rule
        {
            get { return MyArule; }
            set
            {
                MyArule = value;
                TxtArule.Text = MyArule;
            }
        }

        public string B_rule
        {
            get { return MyBrule; }
            set
            {
                MyBrule = value;
                TxtBrule.Text = MyBrule;
            }
        }

        public string C_rule
        {
            get { return MyCrule; }
            set
            {
                MyCrule = value;
                TxtCrule.Text = MyCrule;
            }
        }

        public string X_rule
        {
            get { return MyXrule; }
            set
            {
                MyXrule = value;
                TxtXrule.Text = MyXrule;
            }
        }

        public string Y_rule
        {
            get { return MyYrule; }
            set
            {
                MyYrule = value;
                TxtYrule.Text = MyYrule;
            }
        }

        public string Z_rule
        {
            get { return MyZrule; }
            set
            {
                MyZrule = value;
                TxtZrule.Text = MyZrule;
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyFractalType = TxtFractalType.Text;
                MyInitString = TxtInitialString.Text;
                MyInitialLength = double.Parse(TxtInitialLength.Text);
                MyInitialAngle = int.Parse(TxtInitialAngle.Text);
                MyInitialAngleVariation = double.Parse(TxtAngleVariation.Text);
                MyStartPosX = double.Parse(TxtStartPosX.Text);
                MyStartPosY = double.Parse(TxtStartPosY.Text);
                MyDeflectionAngle = double.Parse(TxtDeflectionAngle.Text);
                MyLengthScaling = double.Parse(TxtLengthScale.Text);
                MyColor = MyBrushes[CmbColors.SelectedIndex];
                MyStartIter = int.Parse(TxtStartIteration.Text);
                MyMaxIter = int.Parse(TxtMaxIterations.Text);
                MyArule = TxtArule.Text;
                MyBrule = TxtBrule.Text;
                MyCrule = TxtCrule.Text;
                MyXrule = TxtXrule.Text;
                MyYrule = TxtYrule.Text;
                MyZrule = TxtZrule.Text;
                ((MainWindow)MyMain).GetParameters();
            }
            catch
            {
                MessageBox.Show("The Parameters are not valid.", "L-System settings error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)MyMain).BtnNext_Click(sender, e);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)MyMain).MnuShowSettings.IsChecked = false;
            Hide();
        }
    }
}
