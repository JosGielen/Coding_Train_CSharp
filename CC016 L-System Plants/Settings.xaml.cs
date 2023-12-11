using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace L_System_Plants
{
    public partial class Settings : Window
    {
        private Window MyMain;
        private List<Brush> MyBrushes;
        private List<string> MyBrushStrings;
        private string MyFractalType = "";
        private string MyInitialString = "";
        private double MyInitialLength = 0.0;
        private int MyInitialAngle = 0;
        private bool MyShowLeaves = false;
        private int MyLeavesSize = 6;
        private int MyMaxIter = 0;
        private double MyStartPosX = 0.0;
        private double MyStartPosY = 0.0;
        private Brush MyBranchColor = Brushes.Brown;
        private Brush MyLeavesColor = Brushes.Green;
        private double MyLengthScaling = 0.0;
        private bool MyBranchVariation = false;
        private int MyBranchStartThickness = 12;
        private double MyDeflectionAngle = 0;
        private bool MyAllowRandom = false;
        private int MyRandomPercentage = 20;
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
                    CmbBranchColor.Items.Add(propinfo.Name);
                    CmbLeavesColor.Items.Add(propinfo.Name);
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

        public string InitialString
        {
            get { return MyInitialString; }
            set
            {
                MyInitialString = value;
                TxtInitialString.Text = MyInitialString;
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

        public int InitialAngle
        {
            get { return MyInitialAngle; }
            set
            {
                MyInitialAngle = value;
                TxtInitialAngle.Text = MyInitialAngle.ToString();
            }
        }

        public bool ShowLeaves
        {
            get { return MyShowLeaves; }
            set
            {
                MyShowLeaves = value;
                cbShowLeaves.IsChecked = MyShowLeaves;
            }
        }

        public int LeavesSize
        {
            get { return MyLeavesSize; }
            set
            {
                MyLeavesSize = value;
                TxtLeaveSize.Text = MyLeavesSize.ToString();
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

        public Brush BranchColor
        {
            get { return MyBranchColor; }
            set
            {
                MyBranchColor = value;
                int index = MyBrushStrings.IndexOf(MyBranchColor.ToString());
                CmbBranchColor.SelectedIndex = index;
            }
        }

        public Brush LeavesColor
        {
            get { return MyLeavesColor; }
            set
            {
                MyLeavesColor = value;
                int index = MyBrushStrings.IndexOf(MyLeavesColor.ToString());
                CmbLeavesColor.SelectedIndex = index;
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

        public bool BranchVariation
        {
            get { return MyBranchVariation; }
            set
            {
                MyBranchVariation = value;
                cbBranchVariation.IsChecked = MyBranchVariation;
            }
        }

        public int BranchStartThickness
        {
            get { return MyBranchStartThickness; }
            set
            {
                MyBranchStartThickness = value;
                TxtBranchStartSize.Text = MyBranchStartThickness.ToString();
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

        public bool AllowRandom
        {
            get { return MyAllowRandom; }
            set
            {
                MyAllowRandom = value;
                cbAllowRandom.IsChecked = MyAllowRandom;
            }
        }

        public int RandomPercentage
        {
            get { return MyRandomPercentage; }
            set
            {
                MyRandomPercentage = value;
                TxtRandomPercentage.Text = MyRandomPercentage.ToString();
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
                MyInitialString = TxtInitialString.Text;
                MyInitialLength = double.Parse(TxtInitialLength.Text);
                MyInitialAngle = int.Parse(TxtInitialAngle.Text);
                MyShowLeaves = cbShowLeaves.IsChecked.Value;
                MyLeavesSize = int.Parse(TxtLeaveSize.Text);
                MyMaxIter = int.Parse(TxtMaxIterations.Text);
                MyStartPosX = double.Parse(TxtStartPosX.Text);
                MyStartPosY = double.Parse(TxtStartPosY.Text);
                MyBranchColor = MyBrushes[CmbBranchColor.SelectedIndex];
                MyLeavesColor = MyBrushes[CmbLeavesColor.SelectedIndex];
                MyLengthScaling = double.Parse(TxtLengthScale.Text);
                MyBranchVariation = cbBranchVariation.IsChecked.Value;
                MyBranchStartThickness = int.Parse(TxtBranchStartSize.Text);
                MyDeflectionAngle = int.Parse(TxtDeflectionAngle.Text);
                MyAllowRandom = cbAllowRandom.IsChecked.Value;
                MyRandomPercentage = int.Parse(TxtRandomPercentage.Text);
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)MyMain).MnuShowSettings.IsChecked = false;
            Hide();
        }
    }
}
