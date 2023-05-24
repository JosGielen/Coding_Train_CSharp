using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Slide_Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image Image1;
        private WriteableBitmap OriginalBmp;
        private int my_Rows;
        private int my_Cols;
        private Slide[,] my_Slides;
        private bool Scrambled;
        private int EmptyRow;
        private int EmptyCol;
        private int my_Moves;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region "Window Events"
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Start with the default image
            string ImageFile = Environment.CurrentDirectory + "\\Images\\Default.gif";
            //Default Size
            my_Rows = 3;
            my_Cols = 3;
            //Show the original Image
            LoadImage(ImageFile);
            Title = "Slide Puzzle Grid " + my_Rows.ToString() + " by " + my_Cols.ToString();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!Scrambled) return;
            //Determine the Row and Column where the mouse was clicked
            Point Pos = e.GetPosition(Canvas1);
            int Col = (int)(my_Cols * Pos.X / Canvas1.Width);
            int Row = (int)(my_Rows * Pos.Y / Canvas1.Height);
            if (Col >= 0 && Col < my_Cols && Row >= 0 && Row < my_Rows)
            {
                if (Math.Abs(Col - EmptyCol) + Math.Abs(Row - EmptyRow) == 1)
                {
                    SwapSlides(Row, Col, EmptyRow, EmptyCol);
                    EmptyRow = Row;
                    EmptyCol = Col;
                    my_Moves += 1;
                    Title = Title = "Slide Puzzle Grid " + my_Rows.ToString() + " by " + my_Cols.ToString() + " : Moves = " + my_Moves.ToString();
                    //Check for Solution
                    for (int I = 0; I < my_Cols; I++)
                    {
                        for (int J = 0; J < my_Rows; J++)
                        {
                            if (my_Slides[I, J].my_CurrentRow != J | my_Slides[I, J].my_CurrentCol != I)
                            {
                                return;
                            }
                        }
                    }
                    //The puzzle is Solved!
                    Canvas1.Children.Clear();
                    Canvas1.Children.Add(Image1);
                    MessageBox.Show("Congratulations! Solved in " + my_Moves.ToString());
                }
                else
                {
                    return;
                }
            }
        }

        #endregion

        #region "Menu Events"

        private void MnuLoad_Click(object sender, RoutedEventArgs e)
        {
            //Use a FileOpenDialog to select an image file and load the image.
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.InitialDirectory = Environment.CurrentDirectory;
            OFD.Filter = "Windows Bitmap (*.bmp,*.dib)|*.bmp;*.dib|JPEG (*.jpg,*.jpeg,*.jfif,*.jpe)|*.jpg;*.jpeg;*.jfif;*.jpe|TIFF (*.tif,tiff)|*.tif;*.tiff|PNG (*.png)|*.png|GIF (*.gif)| *.gif";
            OFD.FilterIndex = 1;
            OFD.RestoreDirectory = true;
            if (OFD.ShowDialog() == true)
            {
                try
                {
                    LoadImage(OFD.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The image can not be opened: " + ex.Message);
                }
            }
        }

        private void MnuChangeGrid_Click(object sender, RoutedEventArgs e)
        {
            GridSettings GSet = new GridSettings(my_Rows, my_Cols);
            GSet.Left = Left + 15;
            GSet.Top = Top + 60;
            GSet.ShowDialog();
            if (GSet.DialogResult.Value == true)
            {
                my_Rows = GSet.Rows;
                my_Cols = GSet.Columns;
                //Delete the current slides
                Canvas1.Children.Clear();
                Canvas1.Children.Add(Image1);
                //Make the new slides but do not show them.
                MakeSlides(OriginalBmp);
                Title = "Slide Puzzle Grid " + my_Rows.ToString() + " by " + my_Cols.ToString();
            }
        }

        private void MnuScramble_Click(object sender, RoutedEventArgs e)
        {
            //Remove the original Image.
            Canvas1.Children.Clear();
            //Devide the Image into slides and scramble them around.
            Scramble();
            ShowSlides();
        }

        private void BtnShowOrig_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Scrambled)
            {
                Canvas1.Children.Clear();
                Canvas1.Children.Add(Image1);
            }
        }

        private void BtnShowOrig_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Scrambled)
            {
                Canvas1.Children.Clear();
                ShowSlides();
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion

        #region "Methods"

        private void LoadImage(string ImgFile)
        {
            //Load the image from file and show it
            BitmapImage bitmap;
            FormatConvertedBitmap convertBitmap;
            bitmap = new BitmapImage(new Uri(ImgFile));
            //Change to 32 bpp if needed
            if (bitmap.Format.BitsPerPixel != 32)
            {
                convertBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
                OriginalBmp = new WriteableBitmap(convertBitmap);
            }
            else
            {
                OriginalBmp = new WriteableBitmap(bitmap);
            }
            Canvas1.Width = OriginalBmp.PixelWidth;
            Canvas1.Height = OriginalBmp.PixelHeight;
            //Show the Image
            Image1 = new Image()
            {
                Source = OriginalBmp,
            };
            Canvas1.Children.Clear();
            Canvas1.Children.Add(Image1);
            //Make the slides but do not show them.
            MakeSlides(OriginalBmp);
        }

        private void MakeSlides(WriteableBitmap OriginalBitmap)
        {
            my_Slides = new Slide[my_Cols, my_Rows];
            WriteableBitmap SlideBitmap;
            int slideWidth = (int)(Canvas1.Width / my_Cols);
            int slideHeight = (int)(Canvas1.Height / my_Rows);
            int oldIndex;
            int newIndex;
            int Xmin;
            int Xmax;
            int Ymin;
            int Ymax;
            int OriginalStride;
            int SlideStride;
            byte[] OriginalPixelData;
            byte[] SlidePixelData;
            Int32Rect Rect;
            OriginalStride = (int)(OriginalBitmap.PixelWidth * OriginalBitmap.Format.BitsPerPixel / 8.0);
            OriginalPixelData = new byte[OriginalStride * OriginalBitmap.PixelHeight];
            OriginalBitmap.CopyPixels(OriginalPixelData, OriginalStride, 0);
            SlideStride = 4 * slideWidth;
            SlidePixelData = new byte[4 * slideWidth * slideHeight];
            Rect = new Int32Rect(0, 0, slideWidth, slideHeight);
            for (int I = 0; I < my_Cols; I++)
            {
                for (int J = 0; J < my_Rows; J++)
                {
                    //Determine the range of pixels from the original image
                    //needed to make the slide image
                    Xmin = slideWidth * I;
                    Xmax = slideWidth * (I + 1);
                    Ymin = slideHeight * J;
                    Ymax = slideHeight * (J + 1);
                    //Copy the range of pixels from the original Bitmap to the slide Bitmap
                    newIndex = 0;
                    for (int Y = Ymin; Y < Ymax; Y++)
                    {
                        for (int X = Xmin; X < Xmax; X++)
                        {
                            oldIndex = 4 * (Y * OriginalBitmap.PixelWidth + X);
                            SlidePixelData[newIndex] = OriginalPixelData[oldIndex];
                            SlidePixelData[newIndex + 1] = OriginalPixelData[oldIndex + 1];
                            SlidePixelData[newIndex + 2] = OriginalPixelData[oldIndex + 2];
                            SlidePixelData[newIndex + 3] = OriginalPixelData[oldIndex + 3];
                            newIndex += 4;
                        }
                    }
                    //Make the Bottom Left slide black
                    if (I == my_Cols - 1 && J == my_Rows - 1)
                    {
                        for (int K = 0; K < SlidePixelData.Length; K += 4)
                        {
                            SlidePixelData[K] = 100;
                            SlidePixelData[K + 1] = 0;
                            SlidePixelData[K + 2] = 100;
                            SlidePixelData[K + 3] = 255;
                        }
                    }
                    SlideBitmap = new WriteableBitmap(Xmax - Xmin, Ymax - Ymin, OriginalBitmap.DpiX, OriginalBitmap.DpiY, OriginalBitmap.Format, OriginalBitmap.Palette);
                    SlideBitmap.WritePixels(Rect, SlidePixelData, SlideStride, 0);
                    //Make the new slide
                    my_Slides[I, J] = new Slide(I, J, SlideBitmap);
                }
            }
            //The empty slide is at the Bottom Left
            EmptyRow = my_Rows - 1;
            EmptyCol = my_Cols - 1;
            Scrambled = false;
        }

        private void Scramble()
        {
            //Move the empty slide 1000 times in random directions
            Random Rnd = new Random();
            int dir;
            int counter = 0;
            int newRow = 0;
            int newCol = 0;
            do
            {
                dir = Rnd.Next(4);
                switch (dir)
                {
                    case 0: //Up
                        newCol = EmptyCol;
                        newRow = EmptyRow - 1;
                        break;
                    case 1: //Left
                        newCol = EmptyCol - 1;
                        newRow = EmptyRow;
                        break;
                    case 2: //Right
                        newCol = EmptyCol + 1;
                        newRow = EmptyRow;
                        break;
                    case 3: //Down
                        newCol = EmptyCol;
                        newRow = EmptyRow + 1;
                        break;
                }
                if (newCol < 0 | newCol >= my_Cols | newRow < 0 | newRow >= my_Rows)
                {
                    continue;
                }
                else
                {
                    SwapSlides(newRow, newCol, EmptyRow, EmptyCol);
                    EmptyRow = newRow;
                    EmptyCol = newCol;
                    counter++;
                }
            } while (counter < 1000);
            Scrambled = true;
            my_Moves = 0;
            Title = Title = "Slide Puzzle Grid " + my_Rows.ToString() + " by " + my_Cols.ToString() + " : Moves = " + my_Moves.ToString();
        }

        private void SwapSlides(int row1, int col1, int row2, int col2)
        {
            //My_Slides indices and positions remain the same but the images get swapped.
            //The currentRow and CurrentCol parameters also get swapped to indicate
            //the new position of their original image.
            //Puzzle is solved if CurrentRow and CurrentCol are equal to their array indices for all slides.
            int tempCol1 = my_Slides[col1, row1].my_CurrentCol;
            int tempRow1 = my_Slides[col1, row1].my_CurrentRow;
            int tempCol2 = my_Slides[col2, row2].my_CurrentCol;
            int tempRow2 = my_Slides[col2, row2].my_CurrentRow;
            my_Slides[col1, row1].SetPosition(tempCol2, tempRow2);
            my_Slides[col2, row2].SetPosition(tempCol1, tempRow1);
            ImageSource TempSource;
            TempSource = my_Slides[col1, row1].GetImageSource();
            my_Slides[col1, row1].SetImageSource(my_Slides[col2, row2].GetImageSource());
            my_Slides[col2, row2].SetImageSource(TempSource);
        }

        private void ShowSlides()
        {
            for (int I = 0; I < my_Cols; I++)
            {
                for (int J = 0; J < my_Rows; J++)
                {
                    my_Slides[I, J].Show(Canvas1);
                }
            }
            DrawDividerLines();
        }

        private void DrawDividerLines()
        {
            Line L;
            for (int I = 1; I < my_Cols; I++)
            {
                L = new Line()
                {
                    X1 = I * Canvas1.Width / my_Cols,
                    Y1 = 0,
                    X2 = I * Canvas1.Width / my_Cols,
                    Y2 = Canvas1.Height,
                    Stroke = Brushes.Black,
                    StrokeThickness = 3
                };
                Canvas1.Children.Add(L);
            }
            for (int I = 1; I < my_Rows; I++)
            {
                L = new Line()
                {
                    X1 = 0,
                    Y1 = I * Canvas1.Height / my_Rows,
                    X2 = Canvas1.Width,
                    Y2 = I * Canvas1.Height / my_Rows,
                    Stroke = Brushes.Black,
                    StrokeThickness = 3
                };
                Canvas1.Children.Add(L);
            }
        }

        #endregion 
    }
}
