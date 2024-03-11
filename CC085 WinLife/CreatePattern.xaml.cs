using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WinLife
{
    public partial class CreatePattern : Window
    {
        private int VeldWidth;
        private int VeldHeight;
        private double CelWidth;
        private double CelHeight;
        private int ColNum = 20;
        private int RowNum = 20;
        private bool Modified = false;
        private string PatternName = "";
        private Generation myGen;
        private Generation StartGen;

        public CreatePattern()
        {
            InitializeComponent();
        }

        #region "Initialisation"
        private void init()
        {
            VeldWidth = (int)Canvas1.ActualWidth;
            VeldHeight = (int)Canvas1.ActualHeight;
            CelWidth = VeldWidth / (double)ColNum;
            CelHeight = VeldHeight / (double)RowNum;
            Canvas1.Children.Clear();
            //Create new generations
            myGen = new Generation(ColNum, RowNum);
            StartGen = new Generation(ColNum, RowNum);
            DrawGrid();
        }

        private void DrawGrid()
        {
            Line gridLine;
            //Draw border of the canvas
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = 0,
                Y1 = 0,
                X2 = VeldWidth,
                Y2 = 0
            };
            Canvas1.Children.Add(gridLine);
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = VeldWidth,
                Y1 = 0,
                X2 = VeldWidth,
                Y2 = VeldHeight
            };
            Canvas1.Children.Add(gridLine);
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = VeldHeight
            };
            Canvas1.Children.Add(gridLine);
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = 0,
                Y1 = VeldHeight,
                X2 = VeldWidth,
                Y2 = VeldHeight
            };
            Canvas1.Children.Add(gridLine);
            //Draw Vertical gridlines
            for (int I = 0; I < ColNum - 1; I++)
            {
                gridLine = new Line
                {
                    Stroke = Brushes.LightGray,
                    X1 = CelWidth * (I + 1),
                    Y1 = 0,
                    X2 = CelWidth * (I + 1),
                    Y2 = VeldHeight
                };
                Canvas1.Children.Add(gridLine);
            }
            //Draw Horizontal gridlines
            for (int I = 0; I < RowNum - 1; I++)
            {
                gridLine = new Line
                {
                    Stroke = Brushes.LightGray,
                    X1 = 0,
                    Y1 = CelHeight * (I + 1),
                    X2 = VeldWidth,
                    Y2 = CelHeight * (I + 1)
                };
                Canvas1.Children.Add(gridLine);
            }
        }
        #endregion

        #region "Window Events"
        private void CreatePattern_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int X;
            int Y;
            X = (int)(Math.Floor(e.GetPosition(Canvas1).X / CelWidth));
            Y = (int)(Math.Floor(e.GetPosition(Canvas1).Y / CelHeight));
            if (X >= 0 & X < myGen.Breedte & Y >= 0 & Y < myGen.Hoogte)
            {
                myGen.SetCell(X, Y, true);
                Modified = true;
                Render();
            }
        }

        private void CreatePattern_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            int X;
            int Y;
            X = (int)(Math.Floor(e.GetPosition(Canvas1).X / CelWidth));
            Y = (int)(Math.Floor(e.GetPosition(Canvas1).Y / CelHeight));
            if (X >= 0 & X < myGen.Breedte & Y >= 0 & Y < myGen.Hoogte)
            {
                myGen.SetCell(X, Y, false);
                Modified = true;
                Render();
            }
        }

        private void CreatePattern_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            init();
        }
        #endregion

        #region "Menu Events"
        private void Menu10x10_Click(object sender, RoutedEventArgs e)
        {
            ColNum = 10;
            RowNum = 10;
            Menu20x20.IsChecked = false;
            Menu40x40.IsChecked = false;
            Menu60x60.IsChecked = false;
            Width = 12 * ColNum + 8;
            Height = 12 * RowNum + 99;
        }

        private void Menu20x20_Click(object sender, RoutedEventArgs e)
        {
            ColNum = 20;
            RowNum = 20;
            Menu10x10.IsChecked = false;
            Menu40x40.IsChecked = false;
            Menu60x60.IsChecked = false;
            Width = 12 * ColNum + 8;
            Height = 12 * RowNum + 99;
        }

        private void Menu40x40_Click(object sender, RoutedEventArgs e)
        {
            ColNum = 40;
            RowNum = 40;
            Menu10x10.IsChecked = false;
            Menu20x20.IsChecked = false;
            Menu60x60.IsChecked = false;
            Width = 12 * ColNum + 8;
            Height = 12 * RowNum + 99;
        }

        private void Menu60x60_Click(object sender, RoutedEventArgs e)
        {
            ColNum = 60;
            RowNum = 60;
            Menu10x10.IsChecked = false;
            Menu20x20.IsChecked = false;
            Menu40x40.IsChecked = false;
            Width = 12 * ColNum + 8;
            Height = 12 * RowNum + 99;
        }

        private void MenuVert_Click(object sender, RoutedEventArgs e)
        {
            int b = myGen.Breedte - 1;
            int h = myGen.Hoogte - 1;
            Generation TestGen = new Generation(myGen.Breedte, myGen.Hoogte);
            //flip myGen into TestGen
            for (int Y = 0; Y <= h; Y++)
            {
                for (int X = 0; X <= b; X++)
                {
                    TestGen.SetCell(b - X, Y, myGen.GetCell(X, Y));
                }
            }
            //Copy TestGen back to myGen
            for (int Y = 0; Y <= h; Y++)
            {
                for (int X = 0; X <= b; X++)
                {
                    myGen.SetCell(X, Y, TestGen.GetCell(X, Y));
                }
            }
            Render();
        }

        private void MenuHori_Click(object sender, RoutedEventArgs e)
        {
            int b = myGen.Breedte - 1;
            int h = myGen.Hoogte - 1;
            Generation TestGen = new Generation(myGen.Breedte, myGen.Hoogte);
            //flip myGen into TestGen
            for (int X = 0; X <= b; X++)
            {
                for (int Y = 0; Y <= h; Y++)
                {
                    TestGen.SetCell(X, h - Y, myGen.GetCell(X, Y));
                }
            }
            //Copy TestGen back to myGen
            for (int Y = 0; Y <= h; Y++)
            {
                for (int X = 0; X <= b; X++)
                {
                    myGen.SetCell(X, Y, TestGen.GetCell(X, Y));
                }
            }
            Render();
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            StreamReader myStream = null;
            string myRow;
            char[] c;
            int X, Y;
            int MinX = 0;
            int MinY = 0;
            int MaxX = 0;
            int MaxY = 0;
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
                        PatternName = openFileDialog1.FileName;
                        myGen.Clear();
                        Y = 0;
                        //Lees de afmetingen van de Game of Life in de file
                        ColNum = int.Parse(myStream.ReadLine());
                        RowNum = int.Parse(myStream.ReadLine());
                        MinX = int.Parse(myStream.ReadLine());
                        MinY = int.Parse(myStream.ReadLine());
                        MaxX = int.Parse(myStream.ReadLine());
                        MaxY = int.Parse(myStream.ReadLine());
                        //Pas het veld aan voor deze afmetingen
                        Width = 12 * ColNum + 8;
                        Height = 12 * RowNum + 99;
                        //Lees de Pattern Data
                        //Iedere levende Cell werd weggeschreven als een 1 of een X
                        while (!myStream.EndOfStream)
                        {
                            myRow = myStream.ReadLine();
                            if (MinY + Y < myGen.Hoogte)
                            {
                                //parse the row into chars and fill the Generation
                                c = myRow.ToCharArray();
                                for (X = 0; X < c.GetLength(0); X++)
                                {
                                    if (MinX + X < myGen.Breedte)
                                    {
                                        if (c[X] == '1' | c[X] == 'X')
                                        {
                                            myGen.SetCell(MinX + X, MinY + Y, true);
                                        }
                                        else
                                        {
                                            myGen.SetCell(MinX + X, MinY + Y, false);
                                        }
                                    }
                                }
                                Y = Y + 1;
                            }
                        }
                        Render();
                        Modified = true;
                        Title = "Pattern: " + System.IO.Path.GetFileName(PatternName);
                        Menu10x10.IsChecked = false;
                        Menu20x20.IsChecked = false;
                        Menu40x40.IsChecked = false;
                        Menu60x60.IsChecked = false;
                    }
                }
                catch (Exception Ex)
                {
                    ;
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
            if (PatternName == "")
            {
                //Request filename via SaveAs
                MenuSaveAs_Click(sender, e);
            }
            else
            {
                SavePattern();
            }
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.DefaultExt = ".pat";
            saveFileDialog1.Filter = "Pattern files (*.pat)|*.pat|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                PatternName = saveFileDialog1.FileName;
                SavePattern();
            }
        }

        private void SavePattern()
        {
            StringBuilder sb = new StringBuilder();
            int MinX = ColNum;
            int MinY = RowNum;
            int MaxX = 0;
            int MaxY = 0;
            //Determine the pattern origin and size
            for (int X = 0; X < ColNum; X++)
            {
                for (int Y = 0; Y < RowNum; Y++)
                {
                    if (myGen.GetCell(X, Y))
                    {
                        if (X < MinX) MinX = X;
                        if (Y < MinY) MinY = Y;
                        if (X > MaxX) MaxX = X;
                        if (Y > MaxY) MaxY = Y;
                    }
                }
            }
            //fill a stringBuilder with the Cell data
            for (int Y = MinY; Y <= MaxY; Y++)
            {
                for (int X = MinX; X <= MaxX; X++)
                {
                    if (myGen.GetCell(X, Y))
                    {
                        sb.Append("X");
                    }
                    else
                    {
                        sb.Append(".");
                    }
                }
                sb.AppendLine();
            }
            //Write the data to the File
            StreamWriter outfile = null;
            try
            {
                outfile = new StreamWriter(PatternName);
                if (outfile != null)
                {
                    //Schrijf de Veld afmetingen weg
                    outfile.WriteLine(ColNum);
                    outfile.WriteLine(RowNum);
                    outfile.WriteLine(MinX);
                    outfile.WriteLine(MinY);
                    outfile.WriteLine(MaxX);
                    outfile.WriteLine(MaxY);
                    outfile.Write(sb.ToString());
                    Modified = false;
                    Title = "Pattern: " + System.IO.Path.GetFileName(PatternName);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Cannot write file to disk. Original error: " + Ex.Message);
            }
            finally
            {
                // Check this again, since we need to make sure we didn't throw an exception on open.
                if (outfile != null)
                {
                    outfile.Close();
                }
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
            //TODO : Ask for save pattern?
        }

        private void MenuClear_Click(object sender, RoutedEventArgs e)
        {
            Modified = false;
            myGen.Clear();
            StartGen.Clear();
            Render();
        }

        private void MenuReset_Click(object sender, RoutedEventArgs e)
        {
            //Copy StartGen back to MyGen
            for (int X = 0; X < StartGen.Breedte; X++)
            {
                for (int Y = 0; Y < StartGen.Hoogte; Y++)
                {
                    myGen.SetCell(X, Y, StartGen.GetCell(X, Y));
                }
            }
            Modified = false;
            Render();
        }
        #endregion

        #region "Business Code"
        private void BtnStep_Click(object sender, RoutedEventArgs e)
        {
            if (Modified)
            {
                //Copy MyGen to StartGen
                for (int X = 0; X < myGen.Breedte; X++)
                {
                    for (int Y = 0; Y < myGen.Hoogte; Y++)
                    {
                        StartGen.SetCell(X, Y, myGen.GetCell(X, Y));
                    }
                }
            }
            Modified = false;
            myGen.update();
            Render();
        }

        private void Render()
        {
            Rectangle rect;
            Canvas1.Children.Clear();
            DrawGrid();
            for (int X = 0; X < ColNum; X++)
            {
                for (int Y = 0; Y < RowNum; Y++)
                {
                    if (myGen.GetCell(X, Y))
                    {
                        rect = new Rectangle();
                        rect.Width = CelWidth - 2;
                        rect.Height = CelHeight - 2;
                        rect.Stroke = Brushes.Black;
                        rect.Fill = Brushes.Black;
                        rect.SetValue(Canvas.TopProperty, Y * CelHeight + 1);
                        rect.SetValue(Canvas.LeftProperty, X * CelWidth + 1);
                        Canvas1.Children.Add(rect);
                    }
                }
            }
        }
        #endregion
    }
}
