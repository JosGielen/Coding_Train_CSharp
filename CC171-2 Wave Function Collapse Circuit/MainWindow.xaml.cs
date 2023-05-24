using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace Wave_Function_Collapse
{
    public partial class MainWindow : Window
    {
        private readonly int waitTime = 20;
        private int TileCount;  //Number of tiles that can be used
        private int my_Rows;
        private int my_Cols;
        private double CellWidth;
        private double CellHeight;
        private List<Tile> Tiles;
        private Cell[] Grid;
        private bool Recording = false;
        private bool Started = false;
        private int ImageNumber;
        private readonly string ResultFileName = "WaveFunctionCollapse.gif";
        private readonly bool DeleteImages = true;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            my_Rows = 30;
            my_Cols = 40;
            //Create the Original Tiles
            Tiles = new List<Tile>();

            Tiles.Add(new Tile(0, LoadImage(Environment.CurrentDirectory + "\\Tiles\\bridge.png"), "ADA", "ABA", "ADA", "ABA"));
            Tiles.Add(new Tile(1, LoadImage(Environment.CurrentDirectory + "\\Tiles\\component.png"), "CCC", "CCC", "CCC", "CCC"));
            Tiles.Add(new Tile(2, LoadImage(Environment.CurrentDirectory + "\\Tiles\\connection.png"), "ADA", "AAC", "CCC", "CAA"));
            Tiles.Add(new Tile(3, LoadImage(Environment.CurrentDirectory + "\\Tiles\\connection.png"), "ADA", "AAC", "CCC", "CAA"));
            Tiles.Add(new Tile(4, LoadImage(Environment.CurrentDirectory + "\\Tiles\\connection.png"), "ADA", "AAC", "CCC", "CAA"));
            Tiles.Add(new Tile(5, LoadImage(Environment.CurrentDirectory + "\\Tiles\\corner.png"), "AAA", "AAA", "AAC", "CAA"));
            Tiles.Add(new Tile(6, LoadImage(Environment.CurrentDirectory + "\\Tiles\\dskew.png"), "ADA", "ADA", "ADA", "ADA"));
            Tiles.Add(new Tile(7, LoadImage(Environment.CurrentDirectory + "\\Tiles\\skew.png"), "ADA", "ADA", "AAA", "AAA"));
            Tiles.Add(new Tile(8, LoadImage(Environment.CurrentDirectory + "\\Tiles\\substrate.png"), "AAA", "AAA", "AAA", "AAA"));
            Tiles.Add(new Tile(9, LoadImage(Environment.CurrentDirectory + "\\Tiles\\t.png"), "AAA", "ADA", "ADA", "ADA"));
            Tiles.Add(new Tile(10, LoadImage(Environment.CurrentDirectory + "\\Tiles\\track.png"), "ADA", "AAA", "ADA", "AAA"));
            Tiles.Add(new Tile(11, LoadImage(Environment.CurrentDirectory + "\\Tiles\\transition.png"), "ABA", "AAA", "ADA", "AAA"));
            Tiles.Add(new Tile(12, LoadImage(Environment.CurrentDirectory + "\\Tiles\\turn.png"), "ADA", "ADA", "AAA", "AAA"));
            Tiles.Add(new Tile(13, LoadImage(Environment.CurrentDirectory + "\\Tiles\\viad.png"), "AAA", "ADA", "AAA", "ADA"));
            Tiles.Add(new Tile(14, LoadImage(Environment.CurrentDirectory + "\\Tiles\\vias.png"), "ADA", "AAA", "AAA", "AAA"));
            Tiles.Add(new Tile(15, LoadImage(Environment.CurrentDirectory + "\\Tiles\\wire.png"), "AAA", "ABA", "AAA", "ABA"));

            //Create rotated versions of the Original Tiles
            Tile dummy;
            TileCount = 16;
            for (int I = 0; I < 16; I++)
            {
                dummy = Tiles[I].Copy();
                //Rotate each tile 3 times
                for (int J = 0; J < 3; J++)
                {
                    dummy = RotateTile(dummy);
                    dummy.ID = TileCount;
                    TileCount++;
                    Tiles.Add(dummy.Copy());
                }
            }
            //Generate the Rules from the EdgeValues
            for (int I = 0; I < Tiles.Count; I++)
            {
                for (int J = 0; J < Tiles.Count; J++)
                {
                    if (Tiles[I].DownEdge == StringReverse(Tiles[J].UpEdge))
                    {
                        Tiles[I].DownOptions.Add(J);
                    }
                    if (Tiles[I].LeftEdge == StringReverse(Tiles[J].RightEdge))
                    {
                        Tiles[I].LeftOptions.Add(J);
                    }
                    if (Tiles[I].UpEdge == StringReverse(Tiles[J].DownEdge))
                    {
                        Tiles[I].UpOptions.Add(J);
                    }
                    if (Tiles[I].RightEdge == StringReverse(Tiles[J].LeftEdge))
                    {
                        Tiles[I].RightOptions.Add(J);
                    }
                }
            }
            //Remove unwanted cell pairings
            Tiles[5].DownOptions.Remove(31);
            Tiles[5].LeftOptions.Remove(33);
            Tiles[31].UpOptions.Remove(5);
            Tiles[31].LeftOptions.Remove(32);
            Tiles[32].UpOptions.Remove(33);
            Tiles[32].RightOptions.Remove(31);
            Tiles[33].DownOptions.Remove(32);
            Tiles[33].RightOptions.Remove(5);
            //Create the Cells
            Init();
        }

        private string StringReverse (string s)
        {
            string result = "";
            for (int I = 0; I < s.Length; I++)
            {
                result = s[I].ToString() + result;
            }
            return result;
        }

        private void Init()
        {
            canvas1.Children.Clear();
            //Create the Cells
            int index;
            CellWidth = canvas1.ActualWidth / my_Cols;
            CellHeight = canvas1.ActualHeight / my_Rows;
            Grid = new Cell[my_Rows * my_Cols];
            for (int I = 0; I < my_Rows; I++)
            {
                for (int J = 0; J < my_Cols; J++)
                {
                    index = I * my_Cols + J;
                    Grid[index] = new Cell(index, TileCount);
                }
            }
        }

        private Tile RotateTile(Tile t)
        {
            //Rotate the Bitmap 90° clockwise
            WriteableBitmap rotatedBmp = WriteableBitmapExtensions.Rotate(t.TileBmp, 90);
            //Rotate the edgevalues
            string newUp = t.LeftEdge;
            string newRight = t.UpEdge;
            string newDown = t.RightEdge;
            string newLeft = t.DownEdge;
            return new Tile(t.ID + 1, rotatedBmp, newUp, newRight, newDown, newLeft);
        }

        private WriteableBitmap LoadImage(string filename)
        {
            //Load the image from file into a WriteableBitmap
            WriteableBitmap result;
            BitmapImage bitmap;
            FormatConvertedBitmap convertBitmap;
            bitmap = new BitmapImage(new Uri(filename));
            //Change to 32 bpp if needed
            if (bitmap.Format.BitsPerPixel != 32)
            {
                convertBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
                result = new WriteableBitmap(convertBitmap);
            }
            else
            {
                result = new WriteableBitmap(bitmap);
            }
            return result;
        }


        private void Render()
        {
            while (Started)
            {
                //Step 1: Find a random non-collapsed cell with the lowest number of allowed tiles
                //--Remove all collapsed cells
                List<Cell> sortedGrid = new List<Cell>();
                for (int I = 0; I < Grid.Length; I++)
                {
                    if (!Grid[I].Collapsed) sortedGrid.Add(Grid[I]);
                }
                //--End the Render loop if all cells are collapsed
                if (sortedGrid.Count == 0)
                {
                    if (Recording) MakeGif(10);
                    Recording = false;
                    Started = false;
                    BtnStart.Content = "START";
                    return;
                }
                //--Sort the non-collapsed cells according to the number of tileoptions
                sortedGrid = sortedGrid.OrderBy(x => x.TileOptions.Length).ToList();

                if (sortedGrid[sortedGrid.Count -1].TileOptions.Length == 0)
                {
                    Started = false;
                    BtnStart.Content = "START";
                }

                //--Find all the cells with number of allowed tiles = MinValue
                int MinValue = sortedGrid[0].TileOptions.Length;
                int EndIndex = 0;
                for (int I = 0; I < sortedGrid.Count; I++)
                {
                    if (sortedGrid[I].TileOptions.Length == MinValue)
                    {
                        EndIndex = I;
                    }
                    else
                    {
                        break;
                    }
                }
                //Select a random cell with the lowest number of allowed tiles
                int RndCellIndex = Rnd.Next(0, EndIndex + 1);
                int GridIndex = sortedGrid[RndCellIndex].index;
                //Step 2: Collapse this cell
                //--Select a random Tile from this cell allowedTiles
                int RndTileIndex = Rnd.Next(sortedGrid[RndCellIndex].TileOptions.Length);
                int TileIndex;
                if (MinValue == 0)
                {
                    TileIndex = 0;
                }
                else
                {
                    TileIndex = sortedGrid[RndCellIndex].TileOptions[RndTileIndex];
                }
                //--Collapse the cell
                SetImage(GridIndex, TileIndex);
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
                //Step3: Propagate the collapse to neighbour cells
                //--Reset the updated field of all non-collapsed cells
                for (int I = 0; I < Grid.Length; I++)
                {
                    if (Grid[I].Collapsed)
                    {
                        Grid[I].Updated = true;
                    }
                    else
                    {
                        Grid[I].Updated = false;
                    }
                }
                //--Propagate recursively
                Propagate(GridIndex);
                if (Recording)
                {
                    SaveImage(canvas1);
                }
            };
        }

        private void Propagate(int index)
        {
            int upIndex = -1;
            int rightIndex = -1;
            int downIndex = -1;
            int leftIndex = -1;
            int row = (int)Math.Floor((double)index / my_Cols);
            int col = index % my_Cols;
            //Get the neighbour indices
            if (row > 0) upIndex = (row - 1) * my_Cols + col;
            if (row < my_Rows - 1) downIndex = (row + 1) * my_Cols + col;
            if (col > 0) leftIndex = row * my_Cols + col - 1;
            if (col < my_Cols - 1) rightIndex = row * my_Cols + col + 1;
            if (upIndex >= 0)
            {
                if (!Grid[upIndex].Updated)
                {
                    Grid[upIndex].CheckAllowedOptions(Tiles, Grid, my_Rows, my_Cols);
                    Propagate(upIndex);
                }
            }
            if (rightIndex >= 0)
            {
                if (!Grid[rightIndex].Updated)
                {
                    Grid[rightIndex].CheckAllowedOptions(Tiles, Grid, my_Rows, my_Cols);
                    Propagate(rightIndex);
                }
            }
            if (downIndex >= 0)
            {
                if (!Grid[downIndex].Updated)
                {
                    Grid[downIndex].CheckAllowedOptions(Tiles, Grid, my_Rows, my_Cols);
                    Propagate(downIndex);
                }
            }
            if (leftIndex >= 0)
            {
                if (!Grid[leftIndex].Updated)
                {
                    Grid[leftIndex].CheckAllowedOptions(Tiles, Grid, my_Rows, my_Cols);
                    Propagate(leftIndex);
                }
            }
        }

        private void SetImage(int gridIndex, int tileIndex)
        {
            int row = (int)Math.Floor((double)gridIndex / my_Cols);
            int col = gridIndex % my_Cols;
            Grid[gridIndex].img = new Image()
            {
                Source = Tiles[tileIndex].TileBmp
            };
            Grid[gridIndex].img.Width = CellWidth;
            Grid[gridIndex].img.Height = CellHeight;
            Grid[gridIndex].img.Stretch = Stretch.Fill;
            Grid[gridIndex].img.SetValue(Canvas.LeftProperty, col * CellWidth);
            Grid[gridIndex].img.SetValue(Canvas.TopProperty, row * CellHeight);
            canvas1.Children.Add(Grid[gridIndex].img);
            Grid[gridIndex].TileOptions = new int[] { tileIndex };
            Grid[gridIndex].Collapsed = true;
            Grid[gridIndex].Updated = true;
        }

        private void SaveImage(FrameworkElement Element)
        {
            DirectoryInfo dirInfo = Directory.CreateDirectory(Environment.CurrentDirectory + "\\output");
            string dir = dirInfo.FullName;
            string FileName = dir + "\\Image-" + ImageNumber.ToString("0000") + ".png";
            PngBitmapEncoder MyEncoder = new PngBitmapEncoder();
            RenderTargetBitmap renderBmp = new RenderTargetBitmap((int)Element.ActualWidth, (int)Element.ActualHeight, 96, 96, PixelFormats.Default);
            renderBmp.Render(Element);
            try
            {
                MyEncoder.Frames.Add(BitmapFrame.Create(renderBmp));
                FileStream sw = new FileStream(FileName, FileMode.Create);
                MyEncoder.Save(sw);
                sw.Close();
                ImageNumber++;
            }
            catch (Exception)
            {
                MessageBox.Show("The Image could not be saved.", "WaveFunction Collapse Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Create an animated Gif from PNG images with ffmpeg.exe.
        /// </summary>
        /// <param name="frameRate">The desired framerate of the animated Gif</param>
        private void MakeGif(int frameRate)
        {
            string prog = Environment.CurrentDirectory + "\\ffmpeg.exe";
            string args = " -framerate " + frameRate.ToString() + " -i output\\Image-%4d.png " + ResultFileName;
            Process p = Process.Start(prog, args);
            p.WaitForExit();
            //Delete the image files
            if (DeleteImages)
            {
                foreach (string f in Directory.GetFiles(Environment.CurrentDirectory + "\\output"))
                {
                    if (System.IO.Path.GetExtension(f) == ".png")
                    {
                        File.Delete(f);
                    }
                }
            }
        }

        private void Wait()
        {
            Thread.Sleep(waitTime);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Started)
            {
                //Start the collapse
                Init();
                Started = true;
                BtnStart.Content = "STOP";
                Recording = CbRecording.IsChecked.Value;
                Render();
            }
            else
            {
                //Stop the collapse
                Started = false;
                CbRecording.IsChecked = false;
                BtnStart.Content = "START";
            }
        }
    }
}
