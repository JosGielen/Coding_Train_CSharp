using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace L_Systems_General
{
    public partial class MainWindow : Window
    {
        private Settings settingForm;
        private string MyString = "";
        private int Iter = 0;
        //Start settings
        private string FractalType = "Dragon Curve";
        private string InitialString = "AX";
        private double InitialLength = 0.6;
        private double InitialAngle = 0;
        private double StartPosX = 1 / 4.2;
        private double StartPosY = 1.9 / 3;
        private double InitialAngleVariation = -45;
        private double LengthScaling = 1 / Math.Sqrt(2);
        private Brush MyColor = Brushes.Red;
        private double DeflectionAngle = 90.0;
        private int StartIter = 0;
        private int MaxIter = 15;
        private string A_rule = "A";
        private string B_rule = "B";
        private string C_rule = "C";
        private string X_rule = "X+YA+";
        private string Y_rule = "-AX-Y";
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
                    settingForm.InitString = InitialString;
                    settingForm.InitialLength = InitialLength;
                    settingForm.InitialAngle = InitialAngle;
                    settingForm.InitialAngleVariation = InitialAngleVariation;
                    settingForm.LengthScaling = LengthScaling;
                    settingForm.StartPosX = StartPosX;
                    settingForm.StartPosY = StartPosY;
                    settingForm.StartIter = StartIter;
                    settingForm.MaxIter = MaxIter;
                    settingForm.Color = MyColor;
                    settingForm.DeflectionAngle = DeflectionAngle;
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

        private void MnuFileOpen_Click(Object sender, RoutedEventArgs e)
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
                    FractalType = sr.ReadLine();
                    InitialString = sr.ReadLine();
                    InitialLength = double.Parse(sr.ReadLine());
                    InitialAngle = int.Parse(sr.ReadLine());
                    InitialAngleVariation = int.Parse(sr.ReadLine());
                    LengthScaling = double.Parse(sr.ReadLine());
                    StartPosX = double.Parse(sr.ReadLine());
                    StartPosY = double.Parse(sr.ReadLine());
                    StartIter = int.Parse(sr.ReadLine());
                    MaxIter = int.Parse(sr.ReadLine());
                    MyColor = (Brush)bc.ConvertFromString(sr.ReadLine());
                    DeflectionAngle = int.Parse(sr.ReadLine());
                    A_rule = sr.ReadLine();
                    B_rule = sr.ReadLine();
                    C_rule = sr.ReadLine();
                    X_rule = sr.ReadLine();
                    Y_rule = sr.ReadLine();
                    Z_rule = sr.ReadLine();
                    sr.Close();
                    if (settingForm != null)
                    {
                        settingForm.FractalType = FractalType;
                        settingForm.InitString = InitialString;
                        settingForm.InitialLength = InitialLength;
                        settingForm.InitialAngle = InitialAngle;
                        settingForm.InitialAngleVariation = InitialAngleVariation;
                        settingForm.LengthScaling = LengthScaling;
                        settingForm.StartPosX = StartPosX;
                        settingForm.StartPosY = StartPosY;
                        settingForm.StartIter = StartIter;
                        settingForm.MaxIter = MaxIter;
                        settingForm.Color = MyColor;
                        settingForm.DeflectionAngle = DeflectionAngle;
                        settingForm.A_rule = A_rule;
                        settingForm.B_rule = B_rule;
                        settingForm.C_rule = C_rule;
                        settingForm.X_rule = X_rule;
                        settingForm.Y_rule = Y_rule;
                        settingForm.Z_rule = Z_rule;
                        settingForm.Title = "L-Systems Settings : " + System.IO.Path.GetFileName(openFileDialog1.FileName);
                    }
                    Iter = StartIter;
                    txtType.Text = FractalType;
                }
                catch
                {
                    Exception ex;
                    MessageBox.Show("The Parameters in File " + System.IO.Path.GetFileName(openFileDialog1.FileName) + " are not valid.", "L-System error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MnuFileSave_Click(Object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.Filter = "L-System File (*.txt)|*.txt";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                StreamWriter outfile = new StreamWriter(saveFileDialog1.FileName);
                outfile.WriteLine(FractalType);
                outfile.WriteLine(InitialString);
                outfile.WriteLine(InitialLength.ToString());
                outfile.WriteLine(InitialAngle.ToString());
                outfile.WriteLine(InitialAngleVariation.ToString());
                outfile.WriteLine(LengthScaling.ToString());
                outfile.WriteLine(StartPosX.ToString());
                outfile.WriteLine(StartPosY.ToString());
                outfile.WriteLine(StartIter.ToString());
                outfile.WriteLine(MaxIter.ToString());
                outfile.WriteLine(MyColor.ToString());
                outfile.WriteLine(DeflectionAngle.ToString());
                outfile.WriteLine(A_rule);
                outfile.WriteLine(B_rule);
                outfile.WriteLine(C_rule);
                outfile.WriteLine(X_rule);
                outfile.WriteLine(Y_rule);
                outfile.WriteLine(Z_rule);
                outfile.Close();
            }
        }

        private void MnuImageSave_Click(object sender, RoutedEventArgs e)
        {
            Rect rect = new Rect(MyCanvas.Margin.Left, MyCanvas.Margin.Top, MyCanvas.ActualWidth, MyCanvas.ActualHeight);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right, (int)rect.Bottom, 96.0, 96.0, PixelFormats.Default);
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

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Iter = StartIter;
            Generate();
        }

        public void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (Iter < MaxIter)
            {
                Iter += 1;
                Generate();
            }
        }

        public void GetParameters()
        {
            FractalType = settingForm.FractalType;
            InitialString = settingForm.InitString;
            InitialLength = settingForm.InitialLength;
            InitialAngle = settingForm.InitialAngle;
            InitialAngleVariation = settingForm.InitialAngleVariation;
            LengthScaling = settingForm.LengthScaling;
            StartPosX = settingForm.StartPosX;
            StartPosY = settingForm.StartPosY;
            StartIter = settingForm.StartIter;
            MaxIter = settingForm.MaxIter;
            MyColor = settingForm.Color;
            DeflectionAngle = settingForm.DeflectionAngle;
            A_rule = settingForm.A_rule;
            B_rule = settingForm.B_rule;
            C_rule = settingForm.C_rule;
            X_rule = settingForm.X_rule;
            Y_rule = settingForm.Y_rule;
            Z_rule = settingForm.Z_rule;
            Iter = StartIter;
            txtType.Text = FractalType;
            Generate();
        }

        #region "L-system routines"

        private void Generate()
        {
            string newString = "";
            MyString = InitialString;
            double Length = InitialLength * MyCanvas.ActualWidth;
            double Angle = InitialAngle;
            Cursor = Cursors.Wait;
            txtIter.Text = Iter.ToString();
            for (int I = 1; I <= Iter; I++)
            {
                Length = LengthScaling * Length;
                Angle = (Angle + InitialAngleVariation) % 360;
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
                        newString += X_rule;
                    }
                    else if (MyString[J] == 'Y')
                    {
                        newString += Y_rule;
                    }
                    else if (MyString[J] == 'Z')
                    {
                        newString += Z_rule;
                    }
                    else
                    {
                        newString += MyString[J];
                    }
                }
                MyString = newString;
            }
            Draw(MyString, Length, Angle);
            Cursor = Cursors.Arrow;
        }

        private void Draw(string s, double length, double angle)
        {
            Line l;
            List<Point> positions = new List<Point>();
            List<double> angles = new List<double>();
            Point startPt = new Point();
            Point endPt = new Point
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
                    endPt.X = startPt.X + length * Math.Cos(angle * Math.PI / 180);
                    endPt.Y = startPt.Y + length * Math.Sin(angle * Math.PI / 180);
                    l = new Line
                    {
                        X1 = startPt.X,
                        Y1 = startPt.Y,
                        X2 = endPt.X,
                        Y2 = endPt.Y,
                        Stroke = MyColor,
                        StrokeThickness = 1
                    };
                    MyCanvas.Children.Add(l);
                }
                else if (s[I] == '-')
                {
                    if (FractalType.Equals("Sierpinski") & Iter % 2 == 0)
                    {
                        angle = (angle + DeflectionAngle) % 360;
                    }
                    else
                    {
                        angle = (angle - DeflectionAngle) % 360;
                    }
                }
                else if (s[I] == '+')
                {
                    if (FractalType.Equals("Sierpinski") & Iter % 2 == 0)
                    {
                        angle = (angle - DeflectionAngle) % 360;
                    }
                    else
                    {
                        angle = (angle + DeflectionAngle) % 360;
                    }
                }
                else if (s[I] == '[')
                {
                    positions.Add(endPt);
                    angles.Add(angle);
                }
                else if (s[I] == ']')
                {
                    endPt = positions[positions.Count - 1];
                    angle = angles[angles.Count - 1];
                    positions.RemoveAt(positions.Count - 1);
                    angles.RemoveAt(angles.Count - 1);
                }
            }
        }

        #endregion
    }
}
