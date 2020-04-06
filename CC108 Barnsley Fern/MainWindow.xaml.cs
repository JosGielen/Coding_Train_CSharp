using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Barnsley_Ferns
{
    public partial class MainWindow : Window
    {
        private bool Rendering = false;
        private string ImageFileName;
        private string SettingsFileName;
        private string FilePath;
        private int FileSaveFilterIndex = 1;
        private int W = 0;
        private int H = 0;
        private BitmapSource bitmap;
        private byte[] PixelData;
        private int Stride = 0;
        private Color my_Color;
        private bool App_Started = false;
        private Settings settingForm;
        private double P1;
        private double P2;
        private double P3;
        private double P4;
        private JG_Matrix.Matrix F1;
        private JG_Matrix.Matrix F2;
        private JG_Matrix.Matrix F3;
        private JG_Matrix.Matrix F4;
        private JG_Matrix.Matrix C1;
        private JG_Matrix.Matrix C2;
        private JG_Matrix.Matrix C3;
        private JG_Matrix.Matrix C4;
        private JG_Matrix.Matrix point;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
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
            if (App_Started)
            {
                if (settingForm != null)
                {
                    settingForm.Left = Left + Width;
                    settingForm.Top = Top;
                }
                Init();
                if (Rendering) Start();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            W = (int)canvas1.ActualWidth;
            H = (int)canvas1.ActualHeight;
            Image1.Width = W;
            Image1.Height = H;
            point = JG_Matrix.Matrix.FromArray(new double[] { 0, 0 });
            P1 = 0.01;
            P2 = 0.85;
            P3 = 0.07;
            P4 = 0.07;
            F1 = JG_Matrix.Matrix.FromArray(new double[,] { { 0.00, 0.00 }, { 0.00, 0.16 } });
            F2 = JG_Matrix.Matrix.FromArray(new double[,] { { 0.85, 0.04 }, { -0.04, 0.85 } });
            F3 = JG_Matrix.Matrix.FromArray(new double[,] { { 0.20, -0.26 }, { 0.23, 0.22 } });
            F4 = JG_Matrix.Matrix.FromArray(new double[,] { { -0.15, 0.28 }, { 0.26, 0.24 } });
            C1 = JG_Matrix.Matrix.FromArray(new double[] { 0.0, 0.00 });
            C2 = JG_Matrix.Matrix.FromArray(new double[] { 0.0, 1.60 });
            C3 = JG_Matrix.Matrix.FromArray(new double[] { 0.0, 1.60 });
            C4 = JG_Matrix.Matrix.FromArray(new double[] { 0.0, 0.44 });
            my_Color = Colors.Lime;
            Init();
            MnuShowSettings.IsChecked = true;
            ShowSettingForm();
            App_Started = true;
        }

        private void Init()
        {
            W = (int)canvas1.ActualWidth;
            H = (int)canvas1.ActualHeight;
            Image1.Width = W;
            Image1.Height = H;
            Stride = (int)(Image1.Width * PixelFormats.Rgb24.BitsPerPixel / 8);
            PixelData = new byte[(int)(Stride * Image1.Height)];
            bitmap = BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, null, PixelData, Stride);
            Image1.Source = bitmap;
        }

        private void ShowSettingForm()
        {
            if (settingForm == null)
            {
                settingForm = new Settings(this, "None");
                settingForm.Show();
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
                settingForm.P1 = P1;
                settingForm.P2 = P2;
                settingForm.P3 = P3;
                settingForm.P4 = P4;
                settingForm.F1 = F1;
                settingForm.F2 = F2;
                settingForm.F3 = F3;
                settingForm.F4 = F4;
                settingForm.C1 = C1;
                settingForm.C2 = C2;
                settingForm.C3 = C3;
                settingForm.C4 = C4;
            }
            else
            {
                settingForm.Show();
            }
        }

        public void Start()
        {
            Init();
            GetParameters();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            Rendering = true;
        }

        public void Halt()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            Rendering = false;
        }

        private void SetPixel(int X, int Y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = X * 3;
            int yIndex = Y * PixStride;
            if (xIndex + yIndex >= 0 & xIndex + yIndex + 2 < buffer.Length)
            {
                buffer[xIndex + yIndex + 0] = c.R;
                buffer[xIndex + yIndex + 1] = c.G;
                buffer[xIndex + yIndex + 2] = c.B;
            }
        }

        public void GetParameters()
        {
            if (settingForm != null)
            {
                P1 = settingForm.P1;
                P2 = settingForm.P2;
                P3 = settingForm.P3;
                P4 = settingForm.P4;
                F1 = settingForm.F1;
                F2 = settingForm.F2;
                F3 = settingForm.F3;
                F4 = settingForm.F4;
                C1 = settingForm.C1;
                C2 = settingForm.C2;
                C3 = settingForm.C3;
                C4 = settingForm.C4;
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double test;
            int X, Y;
            //Draw a new point
            for (int I = 0; I <= 500; I++)
            {
                X = (int)((point.GetValue(0, 0) + 3) * canvas1.ActualWidth / 6);
                Y = (int)((10.5 - point.GetValue(1, 0)) * canvas1.ActualHeight / 10.5);
                SetPixel(X, Y, my_Color, PixelData, Stride);
                //Appy point transformation
                test = Rnd.NextDouble();
                if (test < P1)
                {
                    point = F1 * point + C1;
                }
                else if (test < P1 + P2)
                {
                    point = F2 * point + C2;
                }
                else if (test < P1 + P2 + P3)
                {
                    point = F3 * point + C3;
                }
                else
                {
                    point = F4 * point + C4;
                }
            }
            bitmap = BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, null, PixelData, Stride);
            Image1.Source = bitmap;
        }

        private void MnuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            StreamReader sr;
            //Show an OpenFile dialog
            if (FilePath == "")
            {
                openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            }
            else
            {
                openFileDialog1.InitialDirectory = FilePath;
            }
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == true)
            {
                FilePath = Path.GetDirectoryName(openFileDialog1.FileName);
                SettingsFileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                sr = new StreamReader(openFileDialog1.FileName);
                Init();
                //Read the setting data from the file
                P1 = Double.Parse(sr.ReadLine());
                P2 = Double.Parse(sr.ReadLine());
                P3 = Double.Parse(sr.ReadLine());
                P4 = Double.Parse(sr.ReadLine());

                F1 = JG_Matrix.Matrix.FromString(sr.ReadLine());
                F2 = JG_Matrix.Matrix.FromString(sr.ReadLine());
                F3 = JG_Matrix.Matrix.FromString(sr.ReadLine());
                F4 = JG_Matrix.Matrix.FromString(sr.ReadLine());
                C1 = JG_Matrix.Matrix.FromString(sr.ReadLine());
                C2 = JG_Matrix.Matrix.FromString(sr.ReadLine());
                C3 = JG_Matrix.Matrix.FromString(sr.ReadLine());
                C4 = JG_Matrix.Matrix.FromString(sr.ReadLine());
                if (settingForm != null)
                {
                    settingForm.Show();
                    settingForm.Left = Left + Width;
                    settingForm.Top = Top;
                    settingForm.TxtFernType.Text = SettingsFileName;
                    settingForm.P1 = P1;
                    settingForm.P2 = P2;
                    settingForm.P3 = P3;
                    settingForm.P4 = P4;
                    settingForm.F1 = F1;
                    settingForm.F2 = F2;
                    settingForm.F3 = F3;
                    settingForm.F4 = F4;
                    settingForm.C1 = C1;
                    settingForm.C2 = C2;
                    settingForm.C3 = C3;
                    settingForm.C4 = C4;
                }
            }
        }

        private void MnuFileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            BitmapEncoder MyEncoder = new BmpBitmapEncoder();
            if (FilePath == "")
            {
                saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            }
            else
            {
                saveFileDialog1.InitialDirectory = FilePath;
            }
            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(SettingsFileName);
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog().Value)
            {
                SettingsFileName = Path.GetFileName(saveFileDialog1.FileName);
                FilePath = Path.GetDirectoryName(saveFileDialog1.FileName);
                SaveSettings(SettingsFileName);
            }
        }

        private void SaveSettings(string fileName)
        {
            StreamWriter myStream = null;
            GetParameters();
            try
            {
                myStream = new StreamWriter(fileName);
                if (myStream != null)
                {
                    //Write the setting data to the file
                    myStream.WriteLine(P1.ToString());
                    myStream.WriteLine(P2.ToString());
                    myStream.WriteLine(P3.ToString());
                    myStream.WriteLine(P4.ToString());
                    myStream.WriteLine(F1.ToLineString());
                    myStream.WriteLine(F2.ToLineString());
                    myStream.WriteLine(F3.ToLineString());
                    myStream.WriteLine(F4.ToLineString());
                    myStream.WriteLine(C1.ToLineString());
                    myStream.WriteLine(C2.ToLineString());
                    myStream.WriteLine(C3.ToLineString());
                    myStream.WriteLine(C4.ToLineString());
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("The Settings could not be saved. Original error: " + Ex.Message, "PROGRAM NAME error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (myStream != null) myStream.Close();
            }
        }

        private void MnuImageSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            BmpBitmapEncoder MyEncoder = new BmpBitmapEncoder();
            if (FilePath == "")
            {
                saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            }
            else
            {
                saveFileDialog1.InitialDirectory = FilePath;
            }
            saveFileDialog1.Filter = "Windows Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|TIFF (*.tiff)|*.tiff|PNG (*.png)|*.png";
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(ImageFileName);
            saveFileDialog1.FilterIndex = FileSaveFilterIndex;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog().Value)
            {
                FileSaveFilterIndex = saveFileDialog1.FilterIndex;
                ImageFileName = Path.GetFileName(saveFileDialog1.FileName);
                FilePath = Path.GetDirectoryName(saveFileDialog1.FileName);
                SaveImage(ImageFileName);
            }
        }

        private void SaveImage(string fileName)
        {
            BitmapEncoder MyEncoder = new BmpBitmapEncoder();
            try
            {
                switch (FileSaveFilterIndex)
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
                    default:
                        //Should not occur
                        return;
                }
                MyEncoder.Frames.Add(BitmapFrame.Create(bitmap));
                //Create an instance of StreamWriter to write the histogram to the file.
                FileStream sw = new FileStream(fileName, FileMode.Create);
                MyEncoder.Save(sw);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("The Image could not be saved. Original error: " + Ex.Message, "Barnslet Ferns error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (settingForm != null) settingForm.Close();
            Environment.Exit(0);
        }

        private void MnuShowSettings_Click(object sender, RoutedEventArgs e)
        {
            if (MnuShowSettings.IsChecked)
            {
                ShowSettingForm();
            }
            else
            {
                if (settingForm != null) settingForm.Hide();
            }
        }
    }
}
