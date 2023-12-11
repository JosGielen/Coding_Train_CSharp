using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace L_System_Plants
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Settings settingForm;
        private string MyString = "";
        //Start settings
        private string FractalType = "Initial Settings";
        private string InitialString = "X";
        private double InitialLength = 0.38;
        private int InitialAngle = 270;
        private bool ShowLeaves = false;
        private int LeavesSize = 6;
        private int MaxIter = 7;
        private double StartPosX = 0.5;
        private double StartPosY = 1;
        private Brush BranchColor = Brushes.Brown;
        private Brush LeavesColor = Brushes.Green;
        private double LengthScaling = 0.5;
        private bool BranchVariation = false;
        private int BranchStartThickness = 1;
        private double DeflectionAngle = 35;
        private bool AllowRandom = false;
        private int RandomPercentage = 20;
        private string A_rule = "AA";
        private string B_rule = "B";
        private string C_rule = "C";
        private string X_rule = "A[+X][-X]AX";
        private string Y_rule = "Y";
        private string Z_rule = "Z";

        public MainWindow()
        {
            InitializeComponent();
        }

        #region "Window Events"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtType.Text = FractalType;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (settingForm != null)
            {
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (settingForm != null)
            {
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
            }
            Generate();
        }

        #endregion

        #region "Menus"

        private void MnuShowSettings_Click(object sender, RoutedEventArgs e)
        {
            if (MnuShowSettings.IsChecked)
            {
                if (settingForm == null)
                {
                    settingForm = new Settings(this);
                    settingForm.FractalType = FractalType;
                    settingForm.InitialString = InitialString;
                    settingForm.InitialLength = InitialLength;
                    settingForm.InitialAngle = InitialAngle;
                    settingForm.ShowLeaves = ShowLeaves;
                    settingForm.LeavesSize = LeavesSize;
                    settingForm.MaxIter = MaxIter;
                    settingForm.StartPosX = StartPosX;
                    settingForm.StartPosY = StartPosY;
                    settingForm.BranchColor = BranchColor;
                    settingForm.LeavesColor = LeavesColor;
                    settingForm.LengthScaling = LengthScaling;
                    settingForm.BranchVariation = BranchVariation;
                    settingForm.BranchStartThickness = BranchStartThickness;
                    settingForm.DeflectionAngle = DeflectionAngle;
                    settingForm.AllowRandom = AllowRandom;
                    settingForm.RandomPercentage = RandomPercentage;
                    settingForm.A_rule = A_rule;
                    settingForm.B_rule = B_rule;
                    settingForm.C_rule = C_rule;
                    settingForm.X_rule = X_rule;
                    settingForm.Y_rule = Y_rule;
                    settingForm.Z_rule = Z_rule;
                    settingForm.Left = Left + Width;
                    settingForm.Top = Top;
                }
                settingForm.Show();
            }
            else
            {
                settingForm.Hide();
            }
        }

        private void MnuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Filter = "L-System File (*.txt)|*.txt";
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    StreamReader sr = new StreamReader(openFileDialog1.FileName);
                    // Read the data from the file.
                    FractalType = System.IO.Path.GetFileName(openFileDialog1.FileName);
                    InitialString = sr.ReadLine();
                    InitialLength = double.Parse(sr.ReadLine());
                    InitialAngle = int.Parse(sr.ReadLine());
                    ShowLeaves = bool.Parse(sr.ReadLine());
                    LeavesSize = int.Parse(sr.ReadLine());
                    MaxIter = int.Parse(sr.ReadLine());
                    StartPosX = double.Parse(sr.ReadLine());
                    StartPosY = double.Parse(sr.ReadLine());
                    BranchColor = (Brush)bc.ConvertFromString(sr.ReadLine());
                    LeavesColor = (Brush)bc.ConvertFromString(sr.ReadLine());
                    LengthScaling = double.Parse(sr.ReadLine());
                    BranchVariation = bool.Parse(sr.ReadLine());
                    BranchStartThickness = int.Parse(sr.ReadLine());
                    DeflectionAngle = int.Parse(sr.ReadLine());
                    AllowRandom = bool.Parse(sr.ReadLine());
                    RandomPercentage = int.Parse(sr.ReadLine());
                    A_rule = sr.ReadLine();
                    B_rule = sr.ReadLine();
                    C_rule = sr.ReadLine();
                    X_rule = sr.ReadLine();
                    Y_rule = sr.ReadLine();
                    Z_rule = sr.ReadLine();
                    if (settingForm != null)
                    {
                        settingForm.FractalType = FractalType;
                        settingForm.InitialString = InitialString;
                        settingForm.InitialLength = InitialLength;
                        settingForm.InitialAngle = InitialAngle;
                        settingForm.ShowLeaves = ShowLeaves;
                        settingForm.LeavesSize = LeavesSize;
                        settingForm.MaxIter = MaxIter;
                        settingForm.StartPosX = StartPosX;
                        settingForm.StartPosY = StartPosY;
                        settingForm.BranchColor = BranchColor;
                        settingForm.LeavesColor = LeavesColor;
                        settingForm.LengthScaling = LengthScaling;
                        settingForm.BranchVariation = BranchVariation;
                        settingForm.BranchStartThickness = BranchStartThickness;
                        settingForm.DeflectionAngle = DeflectionAngle;
                        settingForm.AllowRandom = AllowRandom;
                        settingForm.RandomPercentage = RandomPercentage;
                        settingForm.A_rule = A_rule;
                        settingForm.B_rule = B_rule;
                        settingForm.C_rule = C_rule;
                        settingForm.X_rule = X_rule;
                        settingForm.Y_rule = Y_rule;
                        settingForm.Z_rule = Z_rule;
                        settingForm.Title = "L-Systems Settings : " + System.IO.Path.GetFileName(openFileDialog1.FileName);
                    }
                    txtType.Text = FractalType;
                }
                catch
                {
                    MessageBox.Show("The Parameters in File " + System.IO.Path.GetFileName(openFileDialog1.FileName) + " are not valid.", "L-System error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MnuFileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.Filter = "L-System File (*.txt)|*.txt";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                StreamWriter outfile = new StreamWriter(saveFileDialog1.FileName);
                outfile.WriteLine(InitialString);
                outfile.WriteLine(InitialLength.ToString());
                outfile.WriteLine(InitialAngle.ToString());
                outfile.WriteLine(ShowLeaves.ToString());
                outfile.WriteLine(LeavesSize.ToString());
                outfile.WriteLine(MaxIter.ToString());
                outfile.WriteLine(StartPosX.ToString());
                outfile.WriteLine(StartPosY.ToString());
                outfile.WriteLine(BranchColor.ToString());
                outfile.WriteLine(LeavesColor.ToString());
                outfile.WriteLine(LengthScaling.ToString());
                outfile.WriteLine(BranchVariation.ToString());
                outfile.WriteLine(BranchStartThickness.ToString());
                outfile.WriteLine(DeflectionAngle.ToString());
                outfile.WriteLine(AllowRandom.ToString());
                outfile.WriteLine(RandomPercentage.ToString());
                outfile.WriteLine(A_rule);
                outfile.WriteLine(B_rule);
                outfile.WriteLine(C_rule);
                outfile.WriteLine(X_rule);
                outfile.WriteLine(Y_rule);
                outfile.WriteLine(Z_rule);
                FractalType = System.IO.Path.GetFileName(saveFileDialog1.FileName);
                txtType.Text = FractalType;
            }
        }

        private void MnuImageSave_Click(object sender, RoutedEventArgs e)
        {
            Rect R = new Rect(MyCanvas.Margin.Left, MyCanvas.Margin.Top, MyCanvas.ActualWidth, MyCanvas.ActualHeight);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)R.Right, (int)R.Bottom, 96.0, 96.0, PixelFormats.Default);
            rtb.Render(MyCanvas);
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            BitmapEncoder MyEncoder = new BmpBitmapEncoder();
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.Filter = "Windows Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|TIFF (*.tiff)|*.tiff|PNG (*.png)|*.png";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                try
                {
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            MyEncoder = new BmpBitmapEncoder();
                            break;
                        case 2:
                            MyEncoder = new JpegBitmapEncoder();
                            break;
                        case 3:
                            MyEncoder = new GifBitmapEncoder();
                            break;
                        case 4:
                            MyEncoder = new TiffBitmapEncoder();
                            break;
                        case 5:
                            MyEncoder = new PngBitmapEncoder();
                            break;
                    }
                    MyEncoder.Frames.Add(BitmapFrame.Create(rtb));
                    // Create an instance of StreamWriter to write the Image to the file.
                    FileStream sw = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    MyEncoder.Save(sw);
                }
                catch
                {
                    MessageBox.Show("The Image could not be saved.", "AreaPixelcount error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            if (settingForm != null)
            {
                settingForm.Close();
                Environment.Exit(0);
            }
        }
        #endregion

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            Generate();
        }

        public void GetParameters()
        {
            FractalType = "Custom Type";
            InitialString = settingForm.InitialString;
            InitialLength = settingForm.InitialLength;
            InitialAngle = settingForm.InitialAngle;
            ShowLeaves = settingForm.ShowLeaves;
            LeavesSize = settingForm.LeavesSize;
            MaxIter = settingForm.MaxIter;
            StartPosX = settingForm.StartPosX;
            StartPosY = settingForm.StartPosY;
            BranchColor = settingForm.BranchColor;
            LeavesColor = settingForm.LeavesColor;
            LengthScaling = settingForm.LengthScaling;
            BranchVariation = settingForm.BranchVariation;
            BranchStartThickness = settingForm.BranchStartThickness;
            DeflectionAngle = settingForm.DeflectionAngle;
            AllowRandom = settingForm.AllowRandom;
            RandomPercentage = settingForm.RandomPercentage;
            A_rule = settingForm.A_rule;
            B_rule = settingForm.B_rule;
            C_rule = settingForm.C_rule;
            X_rule = settingForm.X_rule;
            Y_rule = settingForm.Y_rule;
            Z_rule = settingForm.Z_rule;
            txtType.Text = FractalType;
            Generate();
        }

        #region "L-system routines"

        private void Generate()
        {
            string newString;
            Random rnd = new Random();
            MyString = InitialString;
            double Length;
            int Angle;
            Cursor = Cursors.Wait;
            do
            {
                MyString = InitialString;
                Length = InitialLength * MyCanvas.ActualWidth;
                Angle = InitialAngle;
                for (int I = 1; I <= MaxIter; I++)
                {
                    Length = LengthScaling * Length;
                    newString = "";
                    for (int J = 0; J < MyString.Length; J++)
                    {
                        if (MyString[J] == 'A')
                        {
                            newString += A_rule;
                        }
                        else if (MyString[J] == 'B')
                        {
                            newString += B_rule;
                        }
                        else if (MyString[J] == 'C')
                        {
                            newString += C_rule;
                        }
                        else if (MyString[J] == 'X')
                        {
                            if (AllowRandom)
                            {
                                if (rnd.NextDouble() > RandomPercentage / 200) newString += X_rule;
                            }
                            else
                            {
                                newString += X_rule;
                            }
                        }
                        else if (MyString[J] == 'Y')
                        {
                            if (AllowRandom)
                            {
                                if (rnd.NextDouble() > RandomPercentage / 200) newString += Y_rule;
                            }
                            else
                            {
                                newString += Y_rule;
                            }
                        }
                        else if (MyString[J] == 'Z')
                        {
                            if (AllowRandom)
                            {
                                if (rnd.NextDouble() > RandomPercentage / 200) newString += Z_rule;
                            }
                            else
                            {
                                newString += Z_rule;
                            }
                        }
                        else
                        {
                            newString += MyString[J];
                        }
                    }
                    MyString = newString;
                }
            } while (MyString.Length < 1);
            Draw(MyString, Length, Angle);
            Cursor = Cursors.Arrow;
        }

        private void Draw(string s, double length, int angle)
        {
            Line l;
            Ellipse e;
            Random rnd = new Random();
            double randomlengthfactor;
            List<Point> positions = new List<Point>();
            List<int> angles = new List<int>();
            double distance;
            double thickness;
            List<Ellipse> leaves = new List<Ellipse>();
            Point startPt;
            Point endPt = new Point()
            {
                X = StartPosX * MyCanvas.ActualWidth,
                Y = StartPosY * MyCanvas.ActualHeight
            };
            MyCanvas.Children.Clear();
            for (int I = 0; I < s.Length; I++)
            {
                if (s[I] == 'A' | s[I] == 'B' | s[I] == 'C')
                {
                    startPt = endPt;
                    if (AllowRandom)
                    {
                        //Random branch length
                        randomlengthfactor = 0.2 + 1.6 * rnd.NextDouble();
                    }
                    else
                    {
                        randomlengthfactor = 1;
                    }
                    endPt.X = startPt.X + length * randomlengthfactor * Math.Cos(angle * Math.PI / 180);
                    endPt.Y = startPt.Y + length * randomlengthfactor * Math.Sin(angle * Math.PI / 180);
                    if (BranchVariation)
                    {
                        //Branch thickness varies with distance from startpoint and node
                        distance = Math.Sqrt(Math.Pow(StartPosX * MyCanvas.ActualWidth - endPt.X, 2) + Math.Pow(StartPosY * MyCanvas.ActualHeight - endPt.Y, 2));
                        thickness = BranchStartThickness * (1 - distance / MyCanvas.ActualHeight);
                    }
                    else
                    {
                        thickness = BranchStartThickness;
                    }
                    if (thickness < 1.0) thickness = 1.0;
                    l = new Line
                    {
                        X1 = startPt.X,
                        Y1 = startPt.Y,
                        X2 = endPt.X,
                        Y2 = endPt.Y,
                        Stroke = BranchColor,
                        StrokeThickness = thickness
                    };
                    MyCanvas.Children.Add(l);
                }
                else if (s[I] == 'X' | s[I] == 'Y' | s[I] == 'Z')
                {
                    if (ShowLeaves)
                    {
                        //Collect all leaves in a list
                        e = new Ellipse();
                        e.SetValue(Canvas.LeftProperty, endPt.X);
                        e.SetValue(Canvas.TopProperty, endPt.Y);
                        e.Height = LeavesSize;
                        e.Width = LeavesSize;
                        e.Fill = LeavesColor;
                        leaves.Add(e);
                    }
                }
                else if (s[I] == '-')
                {
                    angle = (int)(angle - DeflectionAngle) % 360;
                }
                else if (s[I] == '+')
                {
                    angle = (int)(angle + DeflectionAngle) % 360;
                }
                else if (s[I] == '[')
                {
                    positions.Add(endPt);
                    if (AllowRandom & rnd.NextDouble() < RandomPercentage / 50)
                    {
                        angles.Add((int)(angle + (0.35 - 0.7 * rnd.NextDouble()) * DeflectionAngle));
                    }
                    else
                    {
                        angles.Add(angle);
                    }
                }
                else if (s[I] == ']')
                {
                    endPt = positions.Last();
                    angle = angles.Last();
                    positions.RemoveAt(positions.Count - 1);
                    angles.RemoveAt(angles.Count - 1);
                }
            }
            if (ShowLeaves)
            {
                for (int I = 0; I < leaves.Count; I++) //Show the leaves last
                {
                    MyCanvas.Children.Add(leaves[I]);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (settingForm != null)
            {
                settingForm.Close();
                Environment.Exit(0);
            }
        }
        #endregion

    }
}
