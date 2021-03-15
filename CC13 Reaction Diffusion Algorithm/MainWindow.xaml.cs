using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DiffReaction
{
    public partial class MainWindow : Window
    {
        private Settings settingForm;
        private PixelFormat pf = PixelFormats.Rgb24;
        private int VeldWidth = 0;
        private int VeldHeight = 0;
        private int ImageWidth = 0;
        private int ImageHeight = 0;
        private int Stride = 0;
        private byte[] pixelData;
        private bool AppRunning = false;
        private bool AppLoaded = false;
        private List<Color> myColors;
        private bool UseColor;
        private Generation myGen;
        private Generation StartGen;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            ImageWidth = VeldWidth;
            ImageHeight = VeldHeight;
            Stride = (int)((ImageWidth * pf.BitsPerPixel + 7) / 8);
            Image1.Width = ImageWidth;
            Image1.Height = ImageHeight;
            //Resize de array
            pixelData = new byte[Stride * ImageHeight];
            //Create new generations
            myGen = new Generation(VeldWidth, VeldHeight);
            myGen.DiffA = 1.0;     //1.0;
            myGen.DiffB = 0.5;     //0.5;
            myGen.Feed = 0.055;    //0.055;
            myGen.Kill = 0.062;    //0.062;
            StartGen = new Generation(VeldWidth, VeldHeight);
            StartGen.DiffA = 1.0;      //1.0;
            StartGen.DiffB = 0.5;    //0.5;
            StartGen.Feed = 0.055;   //0.055;
            StartGen.Kill = 0.062;   //0.062;
                                     //Create a Settings Form
            if (settingForm == null)
            {
                settingForm = new Settings(this);
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
                settingForm.Show();
                settingForm.DiffA = myGen.DiffA;
                settingForm.DiffB = myGen.DiffB;
                settingForm.Feed = myGen.Feed;
                settingForm.Kill = myGen.Kill;
                settingForm.UseColor = UseColor;
            }
            AppRunning = false;
            Title = "R-D: Initialized";
            Render();
        }

        #region "Window Events"

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            VeldWidth = 250;
            VeldHeight = 250;
            Width = 258;  //VeldWidth + 2* Window Borderthickness;
            Height = 303; //VeldHeight + 2 * Window Borderthickenss + Menu Height + Window Titlebar Height;
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\ThermalBlack.cpl");
            myColors = pal.GetColors(256);
            AppLoaded = true;
            Init();
        }

        private void Window_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            if (AppLoaded)
            {
                VeldWidth = (int)(Width - 8);
                VeldHeight = (int)(Height - 53);
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
                Init();
            }
        }

        private void Window_LocationChanged(Object sender, EventArgs e)
        {
            if (AppLoaded)
            {
                settingForm.Left = Left + Width;
                settingForm.Top = Top;
            }
        }

        private void Window_Closed(Object sender, EventArgs e)
        {
            AppLoaded = false;
            Environment.Exit(0);
        }

        #endregion

        #region "Menu Events"

        private void MenuNew_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            myGen.Clear();
            Title = "R-D: New";
            Dispatcher.Invoke(Render, DispatcherPriority.SystemIdle);
        }

        private void MenuOpen_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            StreamReader myStream = null;
            int fileWidth = 0;
            int fileHeight = 0;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Multiselect = false;
            openFileDialog1.DefaultExt = ".*";
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog().Value)
            {
                try
                {
                    myStream = new StreamReader(openFileDialog1.OpenFile());
                    if (myStream != null)
                    {
                        //Lees de afmetingen van de Reaction-Diffusion setting in de file
                        fileWidth = int.Parse(myStream.ReadLine());
                        fileHeight = int.Parse(myStream.ReadLine());
                        //Pas het veld aan voor deze afmetingen
                        Width = fileWidth + 10;
                        Height = fileHeight + 54;
                        myGen = new Generation(fileWidth, fileHeight);
                        //Lees de Reaction-Diffusion Parameters
                        myGen.DiffA = double.Parse(myStream.ReadLine());
                        myGen.DiffB = double.Parse(myStream.ReadLine());
                        myGen.Feed = double.Parse(myStream.ReadLine());
                        myGen.Kill = double.Parse(myStream.ReadLine());
                        UseColor = bool.Parse(myStream.ReadLine());
                        Dispatcher.Invoke(Render, DispatcherPriority.SystemIdle);
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Cannot read file from disk. Original error: " + Ex.Message);
                }
                finally
                {
                    // Check this again, since we need to make sure we didn't throw an exception on open.
                    if ((myStream != null))
                    {
                        myStream.Close();
                    }
                }

            }
        }

        private void MenuSaveAs_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.Filter = "All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog().Value)
            {
                //Write the Data to the File
                StreamWriter outfile = new StreamWriter(saveFileDialog1.FileName);
                outfile.WriteLine(VeldWidth);
                outfile.WriteLine(VeldHeight);
                outfile.WriteLine(myGen.DiffA);
                outfile.WriteLine(myGen.DiffB);
                outfile.WriteLine(myGen.Feed);
                outfile.WriteLine(myGen.Kill);
                outfile.WriteLine(UseColor.ToString());
                outfile.Close();
                Title = "R-D: " + System.IO.Path.GetFileName(saveFileDialog1.FileName);
            }
        }

        private void MenuExit_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Menu250_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            AppLoaded = false;
            Width = 258;
            AppLoaded = true;
            Height = 303;
            Menu300.IsChecked = false;
            Menu350.IsChecked = false;
            Menu400.IsChecked = false;
        }

        private void Menu300_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            AppLoaded = false;
            Width = 308;
            AppLoaded = true;
            Height = 353;
            Menu250.IsChecked = false;
            Menu350.IsChecked = false;
            Menu400.IsChecked = false;
        }

        private void Menu350_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            AppLoaded = false;
            Width = 358;
            AppLoaded = true;
            Height = 403;
            Menu250.IsChecked = false;
            Menu300.IsChecked = false;
            Menu400.IsChecked = false;
        }

        private void Menu400_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            AppLoaded = false;
            Width = 408;
            AppLoaded = true;
            Height = 453;
            Menu250.IsChecked = false;
            Menu300.IsChecked = false;
            Menu350.IsChecked = false;
        }


        #endregion

        #region "Business Methods"

        public void Start()
        {
            myGen = new Generation(VeldWidth, VeldHeight);
            StartGen = new Generation(VeldWidth, VeldHeight);
            myGen.DiffA = settingForm.DiffA;
            myGen.DiffB = settingForm.DiffB;
            myGen.Feed = settingForm.Feed;
            myGen.Kill = settingForm.Kill;
            UseColor = settingForm.UseColor;
            //Copy MyGen to StartGen
            for (int X = 0; X < myGen.Breedte; X++)
            {
                for (int Y = 0; Y < myGen.Hoogte; Y++)
                {
                    StartGen.SetCellA(X, Y, myGen.GetCellA(X, Y));
                    StartGen.SetCellB(X, Y, myGen.GetCellB(X, Y));
                }
            }
            AppRunning = true;
            while (AppRunning)
            {
                for (int I = 0; I <= 20; I++)
                {
                    myGen.update();
                }
                Title = "R-D: Gen " + myGen.Volgnummer.ToString();
                Dispatcher.Invoke(Render, DispatcherPriority.ApplicationIdle);
            }
        }

        public void Halt()
        {
            AppRunning = false;
        }

        private void Render()
        {
            int ColIndex;
            byte gray;
            for (int X = 0; X < VeldWidth; X++)
            {
                for (int Y = 0; Y < VeldHeight; Y++)
                {
                    if (UseColor)
                    {
                        if (myGen.GetCellB(X, Y) > 0)
                        {
                            if (myGen.GetCellB(X, Y) < 1)
                            {
                                ColIndex = (int)(2 * 255 * (1 - myGen.GetCellB(X, Y)));
                            }
                            else
                            {
                                ColIndex = 0;
                            }
                        }
                        else
                        {
                            ColIndex = 255;
                        }
                        SetPixel(X, Y, myColors[ColIndex % 256], pixelData, Stride);
                    }
                    else
                    {
                        if (myGen.GetCellB(X, Y) > 0)
                        {
                            if (myGen.GetCellB(X, Y) < 1)
                            {
                                gray = (byte)(255 * (1 - myGen.GetCellB(X, Y)));
                            }
                            else
                            {
                                gray = 0;
                            }
                        }
                        else
                        {
                            gray = 255;
                        }
                        SetPixel(X, Y, Color.FromRgb(gray, gray, gray), pixelData, Stride);
                    }
                }
            }
            BitmapSource bitmap = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, pf, null, pixelData, Stride);
            Image1.Source = bitmap;
        }

        private void SetPixel(int x, int y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = x * 3;
            int yIndex = y * PixStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        #endregion

    }
}
