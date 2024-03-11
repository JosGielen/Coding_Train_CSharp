using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WinLife
{
    public partial class MainWindow : Window
    {
        private delegate void RenderDelegate();
        private PixelFormat pf = PixelFormats.Rgb24;
        private int VeldWidth = 0;
        private int VeldHeight = 0;
        private int ImageWidth = 0;
        private int ImageHeight = 0;
        private int Stride = 0;
        private byte[] pixelData;
        private byte[] whiteArray;
        private bool AppRunning = false;
        private bool AppLoaded = false;
        private bool EditMode = false;
        private Generation myGen;
        private Generation StartGen;
        private bool Modified;
        private string GenName; //Path + Filename + extension 

        public MainWindow()
        {
            InitializeComponent();
        }

        #region "Initialisation"

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VeldWidth = 200;
            VeldHeight = 200;
            canvas1.Width = 2 * VeldWidth;
            canvas1.Height= 2 * VeldHeight;
            AppLoaded = true;
            Init();
        }

        private void Init()
        {
            //Image is 2x Veld size (we draw 4 pixels per cell)
            ImageWidth = 2 * VeldWidth;
            ImageHeight = 2 * VeldHeight;
            Stride = (int)((ImageWidth * pf.BitsPerPixel + 7) / 8.0);
            Image1.Width = ImageWidth;
            Image1.Height = ImageHeight;
            //Resize de arrays
            pixelData = new byte[Stride * ImageHeight];
            whiteArray = new byte[Stride * ImageHeight];
            //Vul de white Array met Witte pixels
            for (int x = 0; x < ImageWidth; x++)
            {
                for (int y = 0; y < ImageHeight; y++)
                {
                    SetPixel(x, y, Color.FromRgb(255, 255, 255), whiteArray, Stride);
                }
            }
            //Create new generations
            myGen = new Generation(VeldWidth, VeldHeight);
            StartGen = new Generation(VeldWidth, VeldHeight);
            AppRunning = false;
            Modified = false;
            EditMode = false;
            GenName = "";
            Title = "Game of Life: Initialized";
            Render();
        }

        #endregion

        #region "Window Events"

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppLoaded)
            {
                //VeldWidth = (int)(Width / 2);
                //VeldHeight = (int)(Height / 2);
                //Init();
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            int X;
            int Y;
            if (EditMode == true)
            {
                X = (int)(e.GetPosition(Image1).X / 2);
                Y = (int)(e.GetPosition(Image1).Y / 2);
                if (X >= 0 & X < myGen.Breedte & Y >= 0 & Y < myGen.Hoogte)
                {
                    Title = "Game of Life: Edit mode (" + X.ToString() + " , " + Y.ToString() + ")";
                }
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int X;
            int Y;
            if (EditMode == true)
            {
                X = (int)(e.GetPosition(Image1).X / 2);
                Y = (int)(e.GetPosition(Image1).Y / 2);
                if (X >= 0 & X < myGen.Breedte & Y >= 0 & Y < myGen.Hoogte)
                {
                    myGen.SetCell(X, Y, true);
                    Modified = true;
                    Dispatcher.Invoke(DispatcherPriority.SystemIdle, new RenderDelegate(Render));
                }
            }
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            int X;
            int Y;
            if (EditMode == true)
            {
                X = (int)(e.GetPosition(Image1).X / 2);
                Y = (int)(e.GetPosition(Image1).Y / 2);
                if (X >= 0 & X < myGen.Breedte & Y >= 0 & Y < myGen.Hoogte)
                {
                    myGen.SetCell(X, Y, false);
                    Modified = true;
                    Dispatcher.Invoke(DispatcherPriority.SystemIdle, new RenderDelegate(Render));
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            AppLoaded = false;
            Environment.Exit(0);
        }
        #endregion

        #region "Menu Events"

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            if (Modified == true)
            {
                //Request for a save
                if (MessageBox.Show("Do you want to save this Game of Life?", "Game of Life", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.None) == MessageBoxResult.Yes)
                {
                    MenuSaveAs_Click(sender, e);
                }
            }
            myGen.Clear();
            GenName = "";
            Title = "Game of Life: New";
            Modified = false;
            EditMode = false;
            Dispatcher.Invoke(DispatcherPriority.SystemIdle, new RenderDelegate(Render));
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            StreamReader myStream = null;
            //int fileWidth = 0;
            //int fileHeight = 0;
            string myRow;
            Char[] c;
            int Y;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Multiselect = false;
            openFileDialog1.DefaultExt = ".gol";
            openFileDialog1.Filter = "Game of Life files (*.gol)|*.gol|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    myStream = new StreamReader(openFileDialog1.OpenFile());
                    if (myStream != null)
                    {
                        GenName = openFileDialog1.FileName;
                        myGen.Clear();
                        Y = 0;
                        //Lees de afmetingen van de Game of Life in de file
                        VeldWidth = int.Parse(myStream.ReadLine());
                        VeldHeight = int.Parse(myStream.ReadLine());
                        //Pas het veld aan voor deze afmetingen
                        canvas1.Width = 2 * VeldWidth;
                        canvas1.Height = 2 * VeldHeight;
                        Init();
                        //Lees de Game of Life Data
                        //Iedere levende Cell werd weggeschreven als een 1 of een X
                        while (!myStream.EndOfStream)
                        {
                            myRow = myStream.ReadLine();
                            if (Y < myGen.Hoogte)
                            {
                                //parse the row into chars and fill the Generation
                                c = myRow.ToCharArray();
                                for (int X = 0; X < c.GetLength(0); X++)
                                {
                                    if (X < myGen.Breedte)
                                    {
                                        if (c[X] == '1' | c[X] == 'X')
                                        {
                                            myGen.SetCell(X, Y, true);
                                        }
                                        else
                                        {
                                            myGen.SetCell(X, Y, false);
                                        }
                                    }

                                }
                                Y++;
                            }
                        }
                        Dispatcher.Invoke(DispatcherPriority.SystemIdle, new RenderDelegate(Render));
                        Modified = false;
                        EditMode = false;
                        Title = "Game of Life: " + Path.GetFileName(GenName);
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Cannot read file from disk. Original error: " + Ex.Message);
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
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            if (GenName == "")
            {
                //Request filename via SaveAs
                MenuSaveAs_Click(sender, e);
            }
            else
            {
                SaveFile();
            }
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.Filter = "Game of Life files (*.gol)|*.gol|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                GenName = saveFileDialog1.FileName;
                SaveFile();
            }
        }

        private void SaveFile()
        {
            StringBuilder sb = new StringBuilder();
            //fill a stringBuilder with the Cell data
            for (int Y = 0; Y < myGen.Hoogte; Y++)
            {
                for (int X = 0; X < myGen.Breedte; X++)
                {
                    if (myGen.GetCell(X, Y))
                    {
                        sb.Append('X');
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
                sb.AppendLine();
            }
            //Write the data to the File
            StreamWriter outfile = null;
            try
            {
                outfile = new StreamWriter(GenName);
                if (outfile != null)
                {
                    //Schrijf de Veld afmetingen weg
                    outfile.WriteLine(VeldWidth);
                    outfile.WriteLine(VeldHeight);
                    outfile.Write(sb.ToString());

                    Modified = false;
                    EditMode = false;
                    Title = "Game of Life: " + Path.GetFileName(GenName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot write file to disk. Original error: " + ex.Message);
            }
            finally
            {
                if (outfile != null)
                {
                    outfile.Close();
                }
            }

        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            if (Modified == true)
            {
                //Request for a save
                if (MessageBox.Show("Do you want to save this Game of Life?", "Game of Life", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.None) == MessageBoxResult.Yes)
                {
                    MenuSaveAs_Click(sender, e);
                }
            }
            Environment.Exit(0);
        }

        private void Menu100_Click(object sender, RoutedEventArgs e)
        {
            VeldWidth = 100;
            VeldHeight = 100;
            canvas1.Width = 2 * VeldWidth;
            canvas1.Height = 2 * VeldHeight;
            Init();
            UpdateLayout();
            Menu200.IsChecked = false;
            Menu300.IsChecked = false;
            Menu400.IsChecked = false;
        }

        private void Menu200_Click(object sender, RoutedEventArgs e)
        {
            VeldWidth = 200;
            VeldHeight = 200;
            canvas1.Width = 2 * VeldWidth;
            canvas1.Height = 2 * VeldHeight;
            Init();
            UpdateLayout();
            Menu100.IsChecked = false;
            Menu300.IsChecked = false;
            Menu400.IsChecked = false;
        }

        private void Menu300_Click(object sender, RoutedEventArgs e)
        {
            VeldWidth = 300;
            VeldHeight = 300;
            canvas1.Width = 2 * VeldWidth;
            canvas1.Height = 2 * VeldHeight;
            Init();
            UpdateLayout();
            Menu100.IsChecked = false;
            Menu200.IsChecked = false;
            Menu400.IsChecked = false;
        }

        private void Menu400_Click(object sender, RoutedEventArgs e)
        {
            VeldWidth = 400;
            VeldHeight = 400;
            canvas1.Width = 2 * VeldWidth;
            canvas1.Height = 2 * VeldHeight;
            Init();
            UpdateLayout();
            Menu100.IsChecked = false;
            Menu200.IsChecked = false;
            Menu300.IsChecked = false;
        }

        private void MenuClear_Click(object sender, RoutedEventArgs e)
        {
            EditMode = false;
            Modified = false;
            myGen.Clear();
            if (GenName != "")
            {
                Title = "Game of Life: Cleared " + Path.GetFileName(GenName);
            }
            else
            {
                Title = "Game of Life: Cleared";
            }
            Dispatcher.Invoke(DispatcherPriority.SystemIdle, new RenderDelegate(Render));
        }

        private void MenuEdit_Click(object sender, RoutedEventArgs e)
        {
            AppRunning = false;
            EditMode = true;
        }

        private void MenuCreate_Click(object sender, RoutedEventArgs e)
        {
            CreatePattern CP = new CreatePattern();
            CP.Show();
        }

        private void MenuInsert_Click(object sender, RoutedEventArgs e)
        {
            //Step1: Open a file dialog to select a pattern
            StreamReader myStream = null;
            CoordinateDialog cd = new CoordinateDialog(VeldWidth, VeldHeight);
            string myRow;
            Generation TestGen;
            Char[] c;
            int row = 0;
            int MinX = 0;
            int MinY = 0;
            int MaxX = 0;
            int MaxY = 0;
            int OriginX = 0;
            int OriginY = 0;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Multiselect = false;
            openFileDialog1.DefaultExt = ".pat";
            openFileDialog1.Filter = "Pattern files (*.pat)|*.pat|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    myStream = new StreamReader(openFileDialog1.OpenFile());
                    if (myStream != null)
                    {
                        row = 0;
                        //Lees de afmetingen van de Game of Life in de file
                        myStream.ReadLine();
                        myStream.ReadLine();
                        MinX = int.Parse(myStream.ReadLine());
                        MinY = int.Parse(myStream.ReadLine());
                        MaxX = int.Parse(myStream.ReadLine());
                        MaxY = int.Parse(myStream.ReadLine());
                        TestGen = new Generation(MaxX - MinX + 1, MaxY - MinY + 1);
                        //Lees de Pattern Data in testGen
                        //Iedere levende Cell werd weggeschreven als een 1 of een X
                        while (!myStream.EndOfStream)
                        {
                            myRow = myStream.ReadLine();
                            if (row < TestGen.Hoogte)
                            {
                                //parse the row into chars and fill TestGen
                                c = myRow.ToCharArray();
                                for (int X = 0; X < c.GetLength(0); X++)
                                {
                                    if (X < TestGen.Breedte)
                                    {
                                        if (c[X] == '1' | c[X] == 'X')
                                        {
                                            TestGen.SetCell(X, row, true);
                                        }
                                        else
                                        {
                                            TestGen.SetCell(X, row, false);
                                        }
                                    }
                                }
                                row++;
                            }
                        }
                        //Step2: Ask for the coordinates of the origin (=top left corner) of the pattern.
                        if (cd.ShowDialog() == true)
                        {
                            OriginX = (int)cd.Getcoordinate().X;
                            OriginY = (int)cd.Getcoordinate().Y;
                            if (OriginX + MaxX - MinX > VeldWidth | OriginY + MaxY - MinY > VeldHeight)
                            {
                                MessageBox.Show("The pattern does not fit into the Field!", "Game of Life Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                            }
                            else
                            {
                                //Copy TestGen into myGen at the origin position
                                for (int X = 0; X < TestGen.Breedte; X++)
                                {
                                    for (int Y = 0; Y < TestGen.Hoogte; Y++)
                                    {
                                        myGen.SetCell(OriginX + X, OriginY + Y, TestGen.GetCell(X, Y));
                                    }
                                }
                            }
                            Modified = true;
                            Render();
                        }
                    }
                }
                catch (Exception Ex)
                {

                    MessageBox.Show("Cannot read file from disk. Original error: " + Ex.Message);
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
        }

        private void MenuSingle_Click(object sender, RoutedEventArgs e)
        {
            EditMode = false;
            Modified = false;
            myGen.update();
            Title = "Game of Life : " + System.IO.Path.GetFileName(GenName) + " Generation " + myGen.Volgnummer.ToString();
            Dispatcher.Invoke(DispatcherPriority.SystemIdle, new RenderDelegate(Render));
        }

        private void MenuStart_Click(object sender, RoutedEventArgs e)
        {
            //Set menu item availability
            MenuStart.IsEnabled = false;
            MenuReset.IsEnabled = false;
            MenuClear.IsEnabled = false;
            MenuSize.IsEnabled = false;
            MenuCreate.IsEnabled = false;
            MenuInsert.IsEnabled = false;
            MenuEdit.IsEnabled = false;
            MenuSingle.IsEnabled = false;
            MenuStop.IsEnabled = true;
            MenuFile.IsEnabled = false;
            //Copy MyGen to StartGen
            for (int X = 0; X < myGen.Breedte; X++)
            {
                for (int Y = 0; Y < myGen.Hoogte; Y++)
                {
                    StartGen.SetCell(X, Y, myGen.GetCell(X, Y));
                }
            }
            //Start the game and render while the application is Idle
            //This allows other events to stop the Do loop
            AppRunning = true;
            EditMode = false;
            Modified = false;
            if (GenName != "")
            {
                Title = "Game of Life: " + Path.GetFileName(GenName);
            }
            else
            {
                Title = "Game of Life";
            }
            while (AppRunning)
            {
                myGen.update();
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new RenderDelegate(Render));
            }
        }

        private void MenuStop_Click(object sender, RoutedEventArgs e)
        {
            AppRunning = false;
            MenuStart.IsEnabled = true;
            MenuReset.IsEnabled = true;
            MenuClear.IsEnabled = true;
            MenuSize.IsEnabled = true;
            MenuCreate.IsEnabled = true;
            MenuInsert.IsEnabled = true;
            MenuEdit.IsEnabled = true;
            MenuSingle.IsEnabled = true;
            MenuStop.IsEnabled = false;
            MenuFile.IsEnabled = true;
        }

        private void MenuReset_Click(object sender, RoutedEventArgs e)
        {
            //Copy StartGen back to MyGen
            for (int X = 0; X < myGen.Breedte; X++)
            {
                for (int Y = 0; Y < myGen.Hoogte; Y++)
                {
                    myGen.SetCell(X, Y, StartGen.GetCell(X, Y));
                }
            }
            EditMode = false;
            Modified = false;
            Render();
        }
        #endregion

        #region "Business Methods"
        private void SetPixel(int x, int y, Color c, byte[] buffer, int PixStride)
        {
            int xIndex = x * 3;
            int yIndex = y * PixStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        private void Render()
        {
            //Fill the pixelData array with white pixels
            Array.Copy(whiteArray, pixelData, pixelData.Length);
            //Fill the buffer with the pixels that need drawing in black
            for (int X = 0; X < VeldWidth; X++)
            {
                for (int Y = 0; Y < VeldHeight; Y++)
                {
                    if (myGen.GetCell(X, Y))
                    {
                        SetPixel(2 * X, 2 * Y, Color.FromRgb(0, 0, 0), pixelData, Stride);
                        SetPixel(2 * X + 1, 2 * Y, Color.FromRgb(0, 0, 0), pixelData, Stride);
                        SetPixel(2 * X, 2 * Y + 1, Color.FromRgb(0, 0, 0), pixelData, Stride);
                        SetPixel(2 * X + 1, 2 * Y + 1, Color.FromRgb(0, 0, 0), pixelData, Stride);
                    }
                }
            }
            BitmapSource bitmap = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, pf, null, pixelData, Stride);
            Image1.Source = bitmap;
            Thread.Sleep(40);
        }
        #endregion
    }
}