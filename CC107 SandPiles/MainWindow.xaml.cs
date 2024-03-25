using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SandPiles
{
    public partial class MainWindow : Window
    {
        private Settings settingForm;
        private int W = 0;
        private int H = 0;
        private int MaxSand;
        private int InitialSand;
        private int DistributionNr;
        private PixelFormat pf = PixelFormats.Rgb24;
        private BitmapSource bitmap;
        private int Stride = 0;
        private byte[] pixelData;
        private List<Color> colorList;
        private bool App_Loaded = false;
        private bool Started = false;
        private bool Pause = false;
        private int[,] SandPile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Init();
            App_Loaded = true;
            MnuShowSettings.IsChecked = true;
            MnuShowSettings_Click(sender, e);
        }

        private void Init()
        {
            W = (int)Canvas1.ActualWidth;
            H = (int)Canvas1.ActualHeight;
            Image1.Width = W;
            Image1.Height = H;
            Stride = (int)((W * pf.BitsPerPixel + 7) / 8.0);
            //Resize de arrays
            SandPile = new int[W, H];
            pixelData = new byte[Stride * H];
            //Get all system colors
            colorList = new List<Color>();
            foreach (System.Reflection.PropertyInfo propinfo in typeof(Colors).GetProperties())
            {
                if (propinfo.PropertyType == typeof(Color))
                {
                    colorList.Add((Color)ColorConverter.ConvertFromString(propinfo.Name));
                }
            }
            //Modify some colors
            colorList[0] = Colors.White;
            colorList[1] = Colors.Yellow;
            colorList[2] = Colors.Orange;
            colorList[3] = Colors.Red;
            colorList[4] = Colors.DarkRed;
            colorList[5] = Colors.Black;
            //Set the initial parameters
            MaxSand = 3;
            InitialSand = (int)(1.5 * W * H);
            DistributionNr = 4;
            //Fill the InitPixelData with white pixels
            for (int X = 0; X < W; X++)
            {
                for (int Y = 0; Y < H; Y++)
                {
                    SetPixel(X, Y, Color.FromRgb(255, 255, 255), pixelData, Stride);
                }
            }
            bitmap = BitmapSource.Create(W, H, 96, 96, pf, null, pixelData, Stride);
            Image1.Source = bitmap;
        }

        public void Start()
        {
            Init();
            GetParameters();
            //Poor the sand in the middle
            SandPile[W / 2, H / 2] = InitialSand;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public void Continu()
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public void Halt()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        public void GetParameters()
        {
            if (settingForm != null)
            {
                MaxSand = settingForm.MaxSand;
                InitialSand = settingForm.InitialSand;
                DistributionNr = settingForm.DistributionNr;
                colorList = settingForm.ColorList;
            }
        }

        public void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            int spill;
            int count;
            //Distribute the sand
            for (int I = 0; I < 20; I++)
            {
                count = 0;
                for (int X = 1; X < W - 1; X++)
                {
                    for (int Y = 1; Y < H - 1; Y++)
                    {
                        if (SandPile[X, Y] > MaxSand)
                        {
                            spill = (int)Math.Floor(SandPile[X, Y] / (double)DistributionNr);
                            SandPile[X, Y] -= DistributionNr * spill;
                            SandPile[X - 1, Y] += spill;
                            SandPile[X + 1, Y] += spill;
                            SandPile[X, Y - 1] += spill;
                            SandPile[X, Y + 1] += spill;
                            if (DistributionNr == 8)
                            {
                                SandPile[X - 1, Y - 1] += spill;
                                SandPile[X - 1, Y + 1] += spill;
                                SandPile[X + 1, Y - 1] += spill;
                                SandPile[X + 1, Y + 1] += spill;
                            }
                            count += 1;
                        }
                    }
                }
                //Update the pixeldata colors
                for (int X = 0; X < W; X++)
                {
                    for (int Y = 0; Y < H; Y++)
                    {
                        if (SandPile[X, Y] <= MaxSand)
                        {
                            SetPixel(X, Y, colorList[SandPile[X, Y]], pixelData, Stride);
                        }
                        else
                        {
                            SetPixel(X, Y, colorList[MaxSand + 1], pixelData, Stride);
                        }
                    }
                }
                if (count == 0)
                {
                    Halt();
                    settingForm.BtnStart.Content = "Start";
                }
            }
            bitmap = BitmapSource.Create(W, H, 96, 96, pf, null, pixelData, Stride);
            Image1.Source = bitmap;
        }


        private void SetPixel(int X, int Y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = X * 3;
            int yIndex = Y * PixStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (settingForm != null) settingForm.Close();
            Environment.Exit(0);
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
            if (App_Loaded)
            {
                if (settingForm != null)
                {
                    settingForm.Left = Left + Width;
                    settingForm.Top = Top;
                }
                Init();
            }
        }

        private void MnuSaveImage_Click(Object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            int FileSaveFilterIndex = 1;
            FileStream sw = null;
            BitmapEncoder MyEncoder = new BmpBitmapEncoder();
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.Filter = "Windows Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|TIFF (*.tiff)|*.tiff|PNG (*.png)|*.png";
            saveFileDialog1.FilterIndex = FileSaveFilterIndex;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                FileSaveFilterIndex = saveFileDialog1.FilterIndex;
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
                    // Create an instance of StreamWriter to write the histogram to the file.
                    sw = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    MyEncoder.Save(sw);
                }
                catch
                {
                    MessageBox.Show("The Image could not be saved.", "AreaPixelcount error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                    }
                }
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            if (settingForm != null) settingForm.Close();
            Environment.Exit(0);
        }

        private void MnuShowSettings_Click(object sender, RoutedEventArgs e)
        {
            if (MnuShowSettings.IsChecked)
            {
                if (settingForm == null)
                {
                    settingForm = new Settings(this, colorList);
                    settingForm.Show();
                    settingForm.Left = Left + Width;
                    settingForm.Top = Top;
                    settingForm.Type = "Default";
                    settingForm.MaxSand = MaxSand;
                    settingForm.InitialSand = InitialSand;
                    settingForm.DistributionNr = DistributionNr;
                }
                else
                {
                    settingForm.Show();
                }
            }
            else
            {
                settingForm.Hide();
            }
        }
    }
}