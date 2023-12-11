using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Julia_Set
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int lineCount;
        private PixelFormat pf = PixelFormats.Rgb24;
        private int Stride = 0;
        private byte[] pixelData;
        private readonly ImageBrush imgbrush = new ImageBrush();
        private string my_PaletteFile = "";
        private readonly List<Color> Colors = new List<Color>();
        private double[,] Iters;
        private double Xmin = 0D;
        private double Xmax = 0D;
        private double Ymin = 0D;
        private double Ymax = 0D;
        private double X1 = 0D;
        private double Y1 = 0D;
        private double X2 = 0D;
        private double Y2 = 0D;
        private double CalcRatio = 0.0;
        private double ConstRe = 0.2;
        private double ConstIm = 0.565;
        private int Zmax = 100;     //Default Bail-out value
        private int Nmax = 500;     //Default Max number of iterations
        private double Colormultiplier = 1;
        private double ColorStartIndex = 0;
        private int VeldWidth = 0;
        private int VeldHeight = 0;
        private int MaxColIndex = 0;
        private bool Started = false;
        private bool CanResize = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region "Window Events"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtBailout.Text = Zmax.ToString();
            TxtMaxIter.Text = Nmax.ToString();
            TxtConstRe.Text = ConstRe.ToString();
            TxtConstIm.Text = ConstIm.ToString();
            LblColorMag.Content = "Color Magnifier : " + Colormultiplier.ToString();
            LblColorScroll.Content = "Color Scroll : " + ColorStartIndex.ToString();
            //Set the initial Canvas Size
            VeldWidth = 700;
            VeldHeight = 600;
            ResizeWindow(VeldWidth, VeldHeight);
            ResetFractalData(VeldWidth, VeldHeight);
            CalcRatio = VeldWidth / VeldHeight;
            //Initial window = (-1.5,-1.5) - (1.5,1.5);
            Xmin = -1.5D;
            Xmax = 1.5D;
            Ymin = -1.5D;
            Ymax = 1.5D;
            X1 = Xmin;
            X2 = Xmax;
            Y1 = Ymin;
            Y2 = Ymax;
            //Set the initial color palette
            my_PaletteFile = Environment.CurrentDirectory + "\\default.cpl";
            OpenPalette(my_PaletteFile);
            imgbrush.ImageSource = BitmapSource.Create(VeldWidth, VeldHeight, 96, 96, pf, null, pixelData, Stride);
            imgbrush.Stretch = Stretch.UniformToFill;
            Canvas1.Background = imgbrush;
            //Set the rubberband properties
            RBand.AspectRatio = (double)VeldWidth / VeldHeight;
            RBand.Stroke = Brushes.Yellow;
            RBand.BoxFillOpacity = 0.3;
            RBand.BoxFillColor = Brushes.LightGray;
            RBand.CornerSize = 5;
            RBand.IsEnabled = false;
            CanResize = true;
            Title = "Julia Set";
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RBand.Mouse_Down(e.GetPosition(Canvas1));
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            RBand.Mouse_Move(e.GetPosition(Canvas1));
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RBand.Mouse_Up(e.GetPosition(Canvas1));
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            double W = Canvas1.ActualWidth;
            double H = Canvas1.ActualHeight;
            double X1N;
            double X2N;
            double Y1N;
            double Y2N;
            if (RBand.IsEnabled & RBand.IsDrawn)
            {
                X1N = X1 + RBand.TopLeftCorner.X * (X2 - X1) / W;
                X2N = X1 + RBand.BottomRightCorner.X * (X2 - X1) / W;
                Y1N = Y1 + RBand.TopLeftCorner.Y * (Y2 - Y1) / H;
                Y2N = Y1 + RBand.BottomRightCorner.Y * (Y2 - Y1) / H;
                if (3.5 / (X2N - X1N) > 10000000000000.0)
                {
                    if (MessageBox.Show("The requested zoom exceeds the calculation accuracy.\n Do you wish to proceed?", "Fractally Information", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No) return;
                }
                X1 = X1N;
                X2 = X2N;
                Y1 = Y1N;
                Y2 = Y2N;
                RBand.Clear();
                KeepRatio();
                BtnCalc_Click(this, new RoutedEventArgs());
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CanResize)
            {
                //Fit the Canvas into the new window size
                Canvas1.Width = e.NewSize.Width - 168;
                Canvas1.Height = e.NewSize.Height - 94;
                VeldWidth = (int)(e.NewSize.Width - 168);
                VeldHeight = (int)(e.NewSize.Height - 94);
                KeepRatio();
                RBand.AspectRatio = VeldWidth / VeldHeight;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion

        #region "Menu"

        private void MnuOpenFractal_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            StreamReader myStream = null;
            double newX1 = 0;
            double newX2 = 0;
            double newY1 = 0;
            double newY2 = 0;
            bool check = false;
            OFD.InitialDirectory = Environment.CurrentDirectory;
            OFD.Filter = "Fractals (*.frc)|*.frc";
            OFD.FilterIndex = 1;
            OFD.RestoreDirectory = true;
            if (OFD.ShowDialog() == true)
            {
                try
                {
                    myStream = new StreamReader(OFD.FileName);
                    if (myStream != null)
                    {
                        // Lees de Instelling data uit de file
                        VeldWidth = int.Parse(myStream.ReadLine());
                        VeldHeight = int.Parse(myStream.ReadLine());
                        my_PaletteFile = myStream.ReadLine();
                        newX1 = double.Parse(myStream.ReadLine());
                        newX2 = double.Parse(myStream.ReadLine());
                        newY1 = double.Parse(myStream.ReadLine());
                        newY2 = double.Parse(myStream.ReadLine());
                        ConstRe = double.Parse(myStream.ReadLine());
                        ConstIm = double.Parse(myStream.ReadLine());
                        Colormultiplier = double.Parse(myStream.ReadLine());
                        ColorStartIndex = double.Parse(myStream.ReadLine());
                        Zmax = int.Parse(myStream.ReadLine());
                        Nmax = int.Parse(myStream.ReadLine());
                        check = bool.Parse(myStream.ReadLine());
                        //Pas de Window en array afmetingen aan
                        ResizeWindow(VeldWidth, VeldHeight);
                        ResetFractalData(VeldWidth, VeldHeight);
                        //Lees de Fractal data
                        for (int I = 0; I < VeldWidth; I++)
                        {
                            for (int J = 0; J < VeldHeight; J++)
                            {
                                Iters[I, J] = double.Parse(myStream.ReadLine());
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Cannot read the Fractal data. Original error: " + Ex.Message, "Fractally error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                finally
                {
                    //Check of myStream wel open is want er kan een exception geweest zijn.
                    if (myStream != null)
                    {
                        myStream.Close();
                    }
                }
                //Laad de palette kleuren
                OpenPalette(my_PaletteFile);
                //Zet de controls op de nieuwe waarden
                TxtBailout.Text = Zmax.ToString();
                TxtMaxIter.Text = Nmax.ToString();
                TxtConstRe.Text = ConstRe.ToString();
                TxtConstIm.Text = ConstIm.ToString();
                SldColorMag.Value = Colormultiplier;
                SldColorScroll.Value = ColorStartIndex;
                CBSmooth.IsChecked = check;
                X1 = newX1;
                X2 = newX2;
                Y1 = newY1;
                Y2 = newY2;
                //Onthoud de berekende waarden
                CalcRatio = VeldWidth / VeldHeight;
                //Teken de fractal
                UpdateColors();
                //Update the progressbar
                PBStatus.Value = 100;
            }
            RBand.IsEnabled = true;
        }

        private void MnuSavefractal_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Fractals (*.frc)|*.frc",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            StreamWriter myStream = null;
            if (SFD.ShowDialog() == true)
            {
                try
                {
                    myStream = new StreamWriter(SFD.FileName);
                    if ((myStream != null))
                    {
                        //Write the fractal data to the file
                        myStream.WriteLine(VeldWidth);
                        myStream.WriteLine(VeldHeight);
                        myStream.WriteLine(my_PaletteFile);
                        myStream.WriteLine(X1);
                        myStream.WriteLine(X2);
                        myStream.WriteLine(Y1);
                        myStream.WriteLine(Y2);
                        myStream.WriteLine(ConstRe);
                        myStream.WriteLine(ConstIm);
                        myStream.WriteLine(Colormultiplier);
                        myStream.WriteLine(ColorStartIndex);
                        myStream.WriteLine(Zmax);
                        myStream.WriteLine(Nmax);
                        myStream.WriteLine(CBSmooth.IsChecked);
                        for (int I = 0; I < VeldWidth; I++)
                        {
                            for (int J = 0; J < VeldHeight; J++)
                            {
                                myStream.WriteLine(Iters[I, J]);
                            }
                        }
                        //Update the progressbar
                        PBStatus.Value = 100;
                    }
                }
                catch (Exception Ex)
                {
                    //Update the progressbar
                    PBStatus.Value = 0;
                    MessageBox.Show("Cannot save the Fractal data. Original error: " + Ex.Message, "Fractally error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void MnuSave1024_Click(object sender, RoutedEventArgs e)
        {
            CalculateToFile(1024, 768);
        }

        private void MnuSave1280_Click(object sender, RoutedEventArgs e)
        {
            CalculateToFile(1280, 1024);
        }

        private void MnuSave1600_Click(object sender, RoutedEventArgs e)
        {
            CalculateToFile(1600, 1200);
        }

        private void MnuSave1366_Click(object sender, RoutedEventArgs e)
        {
            CalculateToFile(1366, 768);
        }

        private void MnuSave1920_Click(object sender, RoutedEventArgs e)
        {
            CalculateToFile(1920, 1080);
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuResetScale_Click(object sender, RoutedEventArgs e)
        {
            TxtConstRe.IsEnabled = true;
            TxtConstIm.IsEnabled = true;
            Xmin = -1.5D;
            Xmax = 1.5D;
            Ymin = -1.5D;
            Ymax = 1.5D;
            X1 = Xmin;
            X2 = Xmax;
            Y1 = Ymin;
            Y2 = Ymax;
            KeepRatio();
            BtnCalc_Click(this, new RoutedEventArgs());
        }

        private void MnuOpenPalette_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Color Palettes (*.cpl)|*.cpl",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (OFD.ShowDialog() == true)
            {
                my_PaletteFile = OFD.FileName;
                OpenPalette(my_PaletteFile);
                UpdateColors();
            }
        }

        #endregion

        #region "Controls"

        private void BtnBailoutUP_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtBailout.Text);
            dummy += 50;
            TxtBailout.Text = dummy.ToString();
        }

        private void BtnBailoutDown_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtBailout.Text);
            if (dummy > 50)
            {
                dummy -= 50;
                TxtBailout.Text = dummy.ToString();
            }
        }

        private void BtnMaxIterUP_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtMaxIter.Text);
            dummy += 100;
            TxtMaxIter.Text = dummy.ToString();
        }

        private void BtnMaxIterDown_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtMaxIter.Text);
            if (dummy > 100)
            {
                dummy -= 100;
                TxtMaxIter.Text = dummy.ToString();
            }
        }

        private void BtnConstReUP_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtConstRe.Text);
            dummy += 0.1;
            TxtConstRe.Text = dummy.ToString();
        }

        private void BtnConstReDown_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtConstRe.Text);
            dummy -= 0.1;
            TxtConstRe.Text = dummy.ToString();
        }

        private void BtnConstImUp_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtConstIm.Text);
            dummy += 0.1;
            TxtConstIm.Text = dummy.ToString();
        }

        private void BtnConstImDown_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtConstIm.Text);
            dummy -= 0.1;
            TxtConstIm.Text = dummy.ToString();
        }

        private void CBSmooth_Click(object sender, RoutedEventArgs e)
        {
            BtnCalc_Click(this, new RoutedEventArgs());
        }

        private void SldColorMag_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SldColorMag.Value != Math.Round(SldColorMag.Value))
            {
                SldColorMag.Value = Math.Round(SldColorMag.Value);
            }
            if (SldColorMag.Value != Colormultiplier)
            {
                if (SldColorMag.Value == 0)
                {
                    Colormultiplier = 1;
                }
                else if (SldColorMag.Value < 0)
                {
                    Colormultiplier = 1 + SldColorMag.Value * 0.9 / 50;
                }
                else
                {
                    Colormultiplier = SldColorMag.Value;
                }
                LblColorMag.Content = "Color Magnifier : " + Colormultiplier.ToString();
                UpdateColors();
            }
        }

        private void SldColorScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SldColorScroll.Value != Math.Round(SldColorScroll.Value))
            {
                SldColorScroll.Value = Math.Round(SldColorScroll.Value);
            }
            if (SldColorScroll.Value != ColorStartIndex)
            {
                ColorStartIndex = SldColorScroll.Value;
                LblColorScroll.Content = "Color Scroll : " + ColorStartIndex.ToString();
                UpdateColors();
            }
        }

        #endregion

        #region "Fractal Calculation"

        private void BtnCalc_Click(object sender, RoutedEventArgs e)
        {
            //Start/stop the application
            Started = !Started;
            if (Started)
            {
                BtnCalc.Content = "STOP";
                double.TryParse(TxtConstRe.Text, out ConstRe);
                double.TryParse(TxtConstIm.Text, out ConstIm);
                int.TryParse(TxtBailout.Text, out Zmax);
                int.TryParse(TxtMaxIter.Text, out Nmax);
                //Set the Canvas size to the actual Fractal size
                ResizeWindow(VeldWidth, VeldHeight);
                //Reset the arrays
                ResetFractalData(VeldWidth, VeldHeight);
                CalcRatio = (double)VeldWidth / VeldHeight;
            }
            else
            {
                BtnCalc.Content = "CALCULATE";
            }
            //Calculate the Fractal Line per Line
            lineCount = 0;
            while (Started)
            {
                Dispatcher.Invoke(RenderLine, DispatcherPriority.SystemIdle);
                lineCount += 1;
                if (lineCount >= VeldHeight)
                {
                    BtnCalc.Content = "CALCULATE";
                    Started = false;
                }
            }
            RBand.IsEnabled = true;
        }

        private void RenderLine()
        {
            //Fill the buffer with the pixels from line J
            int ColIndex;
            double X0;
            double Y0 = Y1 + lineCount * (Y2 - Y1) / VeldHeight;  //Calculate Y value of lineCount
            for (int I = 0; I < VeldWidth; I++)
            {
                X0 = X1 + I * (X2 - X1) / VeldWidth;      //Calculate X value of the pixel
                                                          //Use the selected fractal type
                Iters[I, lineCount] = Julia(X0, Y0);
                if (Iters[I, lineCount] < 0)
                {
                    SetPixel(I, lineCount, Color.FromRgb(0, 0, 0), pixelData, Stride);
                }
                else
                {
                    ColIndex = (int)(Colormultiplier * Iters[I, lineCount] + ColorStartIndex) % MaxColIndex;
                    SetPixel(I, lineCount, Colors[ColIndex], pixelData, Stride);
                }
            }
            //Update the progressbar
            PBStatus.Value = 100 * (lineCount + 1) / (double)VeldHeight;
            //Show the partially calculated fractal
            BitmapSource bmp = BitmapSource.Create(VeldWidth, VeldHeight, 96, 96, pf, null, pixelData, Stride);
            imgbrush.ImageSource = bmp;
        }

        private void SetPixel(int x, int y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = x * 3;
            int yIndex = y * PixStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        private double Julia(double X0, double Y0)
        {
            double X;     //X coordinate during iterations
            double Y;     //Y coordinate during iterations
            int N;
            double modul;
            double Xtemp;
            MaxColIndex = Colors.Count - 1;
            N = 0;
            X = X0;
            Y = Y0;
            while (X * X + Y * Y < Zmax & N < Nmax)
            {
                Xtemp = X * X - Y * Y + ConstRe;
                Y = 2 * X * Y + ConstIm;
                X = Xtemp;
                N += 1;
            }
            if (N >= Nmax)
            {
                return -1;
            }
            else
            {
                //Do 2 more iterations for the color smoothing to work OK
                Xtemp = X * X - Y * Y + ConstRe;
                Y = 2 * X * Y + ConstIm;
                X = Xtemp;
                N += 1;
                Xtemp = X * X - Y * Y + ConstRe;
                Y = 2 * X * Y + ConstIm;
                X = Xtemp;
                N += 1;
                //return the Color Index
                if (CBSmooth.IsChecked == true)
                {
                    modul = Math.Sqrt(X * X + Y * Y);
                    return N - Math.Log(Math.Log(modul)) / Math.Log(2);
                }
                else
                {
                    return N;
                }
            }
        }

        #endregion

        #region "Utilities"

        private void CalculateToFile(int my_W, int my_H)
        {
            int ColIndex;
            double Iter;
            double X0;
            double Y0;
            double X1N;
            double X2N;
            double Y1N;
            double Y2N;
            int my_stride = (my_W * pf.BitsPerPixel + 7) / 8;
            byte[] pix = new byte[my_stride * my_H];
            SaveFileDialog SFD = new SaveFileDialog();
            FileStream myStream = null;
            SFD.InitialDirectory = Environment.CurrentDirectory;
            SFD.Filter = "Bitmap (*.bmp)|*.bmp|JPEG format (*.jpg)|*.jpg";
            SFD.FilterIndex = 1;
            SFD.RestoreDirectory = true;
            if (SFD.ShowDialog() == true)
            {
                try
                {
                    myStream = new FileStream(SFD.FileName, FileMode.Create);
                    if (myStream != null)
                    {
                        Cursor = Cursors.Wait;
                        MaxColIndex = Colors.Count - 1;
                        //Scale X or Y-axis to keep aspect ratio
                        if ((double)my_W / my_H > (double)VeldWidth / VeldHeight)
                        {
                            //Adjust Fractal Width
                            Y1N = Y1;
                            Y2N = Y2;
                            X1N = (X1 + X2) / 2 - ((double)my_W / my_H) / ((double)VeldWidth / VeldHeight) * (X2 - X1) / 2;
                            X2N = (X1 + X2) / 2 + ((double)my_W / my_H) / ((double)VeldWidth / VeldHeight) * (X2 - X1) / 2;
                        }
                        else
                        {
                            //Adjust Fractal Height
                            X1N = X1;
                            X2N = X2;
                            Y1N = (Y1 + Y2) / 2 - ((double)my_H / my_W) / ((double)VeldHeight / VeldWidth) * (Y2 - Y1) / 2;
                            Y2N = (Y1 + Y2) / 2 + ((double)my_H / my_W) / ((double)VeldHeight / VeldWidth) * (Y2 - Y1) / 2;
                        }
                        //Calculate number of iterations for each pixel.
                        for (int I = 0; I < my_W; I++)
                        {
                            X0 = X1N + I * (X2N - X1N) / my_W;     //Calculate X value of the pixel
                            for (int J = 0; J < my_H; J++)
                            {
                                Y0 = Y1N + J * (Y2N - Y1N) / my_H; //Calculate Y value of the pixel
                                Iter = Julia(X0, Y0);
                                if (Iter < 0)
                                {
                                    SetPixel(I, J, Color.FromRgb(0, 0, 0), pix, my_stride);
                                }
                                else
                                {
                                    ColIndex = (int)(Colormultiplier * Iter + ColorStartIndex) % MaxColIndex;
                                    SetPixel(I, J, Colors[ColIndex], pix, my_stride);
                                }
                            }
                        }
                        BitmapSource bmp = BitmapSource.Create(my_W, my_H, 96, 96, pf, null, pix, my_stride);
                        //Save the bmp to the file
                        if (SFD.FilterIndex == 1)
                        {
                            BmpBitmapEncoder enc = new BmpBitmapEncoder();
                            enc.Frames.Add(BitmapFrame.Create(bmp));
                            enc.Save(myStream);
                        }
                        else if (SFD.FilterIndex == 2)
                        {
                            JpegBitmapEncoder enc = new JpegBitmapEncoder();
                            enc.QualityLevel = 100;
                            enc.Frames.Add(BitmapFrame.Create(bmp));
                            enc.Save(myStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    PBStatus.Value = 0;
                    MessageBox.Show("Cannot save the Image data. Original error: " + ex.Message, "Fractally error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                // Check this again, since we need to make sure we didn't throw an exception on open.
                if (myStream != null)
                {
                    myStream.Close();
                }
                Cursor = Cursors.Arrow;
            }
        }

        private void OpenPalette(string pal)
        {
            StreamReader myStream = null;
            string myLine;
            string[] txtparts;
            byte r;
            byte g;
            byte b;
            try
            {
                myStream = new StreamReader(pal);
                if (myStream != null)
                {
                    Colors.Clear();
                    MaxColIndex = int.Parse(myStream.ReadLine());
                    // Lees de palette kleuren
                    for (int I = 0; I < MaxColIndex; I++)
                    {
                        myLine = myStream.ReadLine();
                        txtparts = myLine.Split(';');
                        r = byte.Parse(txtparts[0]);
                        g = byte.Parse(txtparts[1]);
                        b = byte.Parse(txtparts[2]);
                        Colors.Add(Color.FromRgb(r, g, b));
                    }
                    if (ColorStartIndex <= MaxColIndex)
                    {
                        SldColorScroll.Value = ColorStartIndex;
                    }
                    else
                    {
                        SldColorScroll.Value = SldColorScroll.Minimum;
                    }
                    SldColorScroll.Maximum = MaxColIndex;
                    SldColorScroll.TickFrequency = (int)(MaxColIndex / 10);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Cannot read file. Original error: " + Ex.Message, "Fractally error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Check this again, since we need to make sure we didn't throw an exception on open.
                if (myStream != null)
                {
                    myStream.Close();
                }
            }
        }

        private void ResizeWindow(int w, int h)
        {
            //Resize the entire window when the Fractal-Image size needs to change
            //but do not process the Window.SizeChange Event!
            CanResize = false;
            Width = w + 168;
            Height = h + 94;
            Canvas1.Width = w;
            Canvas1.Height = h;
            CanResize = true;
        }

        private void KeepRatio()
        {
            double NewRatio = VeldWidth / VeldHeight;
            double MidX = (X1 + X2) / 2;
            double MidY = (Y1 + Y2) / 2;
            //Scale X or Y-axis to keep aspect ratio
            if (NewRatio > CalcRatio)
            {
                //Adjust Fractal Height
                Y1 = MidY - (X2 - X1) / (2 * NewRatio);
                Y2 = MidY + (X2 - X1) / (2 * NewRatio);
            }
            else
            {
                //Adjust Fractal Width
                X1 = MidX - NewRatio * (Y2 - Y1) / 2;
                X2 = MidX + NewRatio * (Y2 - Y1) / 2;
            }
        }

        private void ResetFractalData(int w, int h)
        {
            if (w > 0 & h > 0)
            {
                Stride = (int)((w * pf.BitsPerPixel + 7) / 8.0);
                //Resize de arrays
                pixelData = new byte[Stride * h];
                Iters = new double[w, h];
            }
        }

        private void UpdateColors()
        {
            //Update the fractal colors
            MaxColIndex = Colors.Count();
            SldColorScroll.Maximum = MaxColIndex;
            SldColorScroll.TickFrequency = (int)(MaxColIndex / 10.0);
            int ColIndex;
            Cursor = Cursors.Wait;
            for (int I = 0; I < VeldWidth; I++)
            {
                for (int J = 0; J < VeldHeight; J++)
                {
                    if (Iters[I, J] < 0)
                    {
                        SetPixel(I, J, Color.FromRgb(0, 0, 0), pixelData, Stride);
                    }
                    else
                    {
                        ColIndex = (int)(Colormultiplier * Iters[I, J] + ColorStartIndex) % MaxColIndex;
                        SetPixel(I, J, Colors[ColIndex], pixelData, Stride);
                    }
                }
            }
            //Show the bitmap
            BitmapSource bmp = BitmapSource.Create(VeldWidth, VeldHeight, 96, 96, pf, null, pixelData, Stride);
            imgbrush.ImageSource = bmp;
            Cursor = Cursors.Arrow;
        }
        #endregion
    }
}
