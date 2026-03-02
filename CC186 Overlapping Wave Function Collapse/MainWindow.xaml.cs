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
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Wave_Function_Collapse
{
    public partial class MainWindow : Window
    {
        private readonly int waitTime = 20;
        private int TileCount;  //Number of tiles that can be used
        private int TileSize = 3; //Size of one tile image (width and height)
        private double CellSize = 10.0; //Size of one pixel in the resulting image
        private int TileRows;     //Number of tiles made from one row of the source image
        private int TileCols;     //Number of tiles made from one column of the source image
        private int CellRows;     //Number of cell rows on the Canvas
        private int CellColumns;  //Number of cell columns on the Canvas
        private List<Tile> Tiles;
        private Cell[] Grid;
        private string ImageFile;
        private WriteableBitmap SourceBmp;
        private int MaxDepth = 300;
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
            //Load the source image
            ImageFile = Environment.CurrentDirectory + "\\Images\\Flowers.bmp";
            SourceBmp = LoadImage(ImageFile);
            SourceImage.Source = SourceBmp;
            //Create the Original Tiles
            InitTiles();
            //Set the possible neighbours of each tile
            SetPossibleNeighbours();
            //Create the Cells
            InitCells();
        }

        private void InitTiles()
        {
            TileRows = SourceBmp.PixelHeight;
            TileCols = SourceBmp.PixelWidth;
            TileCount = TileRows * TileCols;
            Tiles = new List<Tile>();
            int TileCounter = 0;
            int oldIndex;
            int newIndex;
            int OriginalStride;
            byte[] OriginalPixelData;
            byte[] TilePixelData;
            OriginalStride = (int)(SourceBmp.PixelWidth * SourceBmp.Format.BitsPerPixel / 8.0);
            OriginalPixelData = new byte[OriginalStride * SourceBmp.PixelHeight];
            SourceBmp.CopyPixels(OriginalPixelData, OriginalStride, 0);
            for (int I = 0; I < TileRows; I++)  //0 - 9
            {
                for (int J = 0; J < TileCols; J++)
                {
                    //Copy the range of pixels from the original Bitmap to the Tiles Bitmaps
                    newIndex = 0;
                    TilePixelData = new byte[4 * TileSize * TileSize];
                    for (int X = 0; X < TileSize; X++)
                    {
                        for (int Y = 0; Y < TileSize; Y++)
                        {
                            oldIndex = 4 * (((I + X) % SourceBmp.PixelHeight) * SourceBmp.PixelWidth + (J + Y) % SourceBmp.PixelWidth);
                            TilePixelData[newIndex] = OriginalPixelData[oldIndex];
                            TilePixelData[newIndex + 1] = OriginalPixelData[oldIndex + 1];
                            TilePixelData[newIndex + 2] = OriginalPixelData[oldIndex + 2];
                            TilePixelData[newIndex + 3] = OriginalPixelData[oldIndex + 3];
                            newIndex += 4;
                        }
                    }
                    //Make the new Tile
                    Tiles.Add(new Tile(TileCounter, TileSize, TilePixelData));
                    TileCounter++;
                }
            }
        }

        private void SetPossibleNeighbours()
        {
            for (int I = 0; I < Tiles.Count; I++)
            {
                for (int J = 0; J < Tiles.Count; J++)
                {
                    if (Utilities.CompareUpPixelData(Tiles[I].PixelData, Tiles[J].PixelData, TileSize))
                    {
                        Tiles[I].UpOptions.Add(J);
                    }
                    if (Utilities.CompareDownPixelData(Tiles[I].PixelData, Tiles[J].PixelData, TileSize))
                    {
                        Tiles[I].DownOptions.Add(J);
                    }
                    if (Utilities.CompareRightPixelData(Tiles[I].PixelData, Tiles[J].PixelData, TileSize))
                    {
                        Tiles[I].RightOptions.Add(J);
                    }
                    if (Utilities.CompareLeftPixelData(Tiles[I].PixelData, Tiles[J].PixelData, TileSize))
                    {
                        Tiles[I].LeftOptions.Add(J);
                    }
                }
            }
        }

        private void InitCells()
        {
            canvas1.Children.Clear();
            //Create the Cells
            int index;
            CellRows = (int)(canvas1.ActualHeight / CellSize);
            CellColumns = (int)(canvas1.ActualWidth / CellSize);
            Grid = new Cell[CellRows * CellColumns];
            for (int I = 0; I < CellRows; I++)
            {
                for (int J = 0; J < CellColumns; J++)
                {
                    index = I * CellColumns + J;
                    Grid[index] = new Cell(index, TileCount);
                }
            }
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
                    if (Recording) MakeGif(20);
                    Recording = false;
                    Started = false;
                    MnuOpen.IsEnabled = true;
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
                Propagate(GridIndex, 0);
                if (Recording)
                {
                    SaveImage(canvas1);
                }
            };
        }

        private void Propagate(int index, int depth)
        {
            if (depth > MaxDepth) { return; }
            int upIndex = -1;
            int rightIndex = -1;
            int downIndex = -1;
            int leftIndex = -1;
            int row = (int)Math.Floor((double)index / CellColumns);
            int col = index % CellColumns;
            //Get the neighbour indices
            if (row > 0) upIndex = (row - 1) * CellColumns + col;
            if (row < CellRows - 1) downIndex = (row + 1) * CellColumns + col;
            if (col > 0) leftIndex = row * CellColumns + col - 1;
            if (col < CellColumns - 1) rightIndex = row * CellColumns + col + 1;
            if (upIndex >= 0)
            {
                if (!Grid[upIndex].Updated)
                {
                    Grid[upIndex].CheckAllowedOptions(Tiles, Grid, CellRows, CellColumns);
                    Propagate(upIndex, depth + 1);
                }
            }
            if (rightIndex >= 0)
            {
                if (!Grid[rightIndex].Updated)
                {
                    Grid[rightIndex].CheckAllowedOptions(Tiles, Grid, CellRows, CellColumns);
                    Propagate(rightIndex, depth + 1);
                }
            }
            if (downIndex >= 0)
            {
                if (!Grid[downIndex].Updated)
                {
                    Grid[downIndex].CheckAllowedOptions(Tiles, Grid, CellRows, CellColumns);
                    Propagate(downIndex, depth + 1);
                }
            }
            if (leftIndex >= 0)
            {
                if (!Grid[leftIndex].Updated)
                {
                    Grid[leftIndex].CheckAllowedOptions(Tiles, Grid, CellRows, CellColumns);
                    Propagate(leftIndex, depth + 1);
                }
            }
        }

        private void SetImage(int gridIndex, int tileIndex)
        {
            int row = (int)Math.Floor((double)gridIndex / CellColumns);
            int col = gridIndex % CellColumns;
            Grid[gridIndex].img = new Image()
            {
                Source = Tiles[tileIndex].TileBmp
            };
            Grid[gridIndex].img.Width = CellSize;
            Grid[gridIndex].img.Height = CellSize;
            Grid[gridIndex].img.Stretch = Stretch.Fill;
            Grid[gridIndex].img.SetValue(Canvas.LeftProperty, col * CellSize);
            Grid[gridIndex].img.SetValue(Canvas.TopProperty, row * CellSize);
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
                Started = true;
                BtnStart.Content = "STOP";
                MnuOpen.IsEnabled = false;
                Recording = CbRecording.IsChecked.Value;
                Render();
            }
            else
            {
                //Stop the collapse
                Started = false;
                MnuOpen.IsEnabled = true;
                CbRecording.IsChecked = false;
                BtnStart.Content = "START";
            }
        }

        private void MnuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory + "\\Images",
                Filter = "Bitmap (*.bmp)|*.bmp|JPEG format (*.jpg)|*.jpg",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (ofd.ShowDialog() == true)
            {
                ImageFile = ofd.FileName;
                SourceBmp = LoadImage(ImageFile);
                SourceImage.Source = SourceBmp;
                canvas1.Children.Clear();
                //Create the Original Tiles
                InitTiles();
                //Set the possible neighbours of each tile
                SetPossibleNeighbours();
                //Create the Cells
                InitCells();
            }
        }

        private void MnuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.InitialDirectory = Environment.CurrentDirectory;
            SFD.Filter = "Bitmap (*.bmp)|*.bmp|JPEG format (*.jpg)|*.jpg";
            SFD.FilterIndex = 1;
            SFD.RestoreDirectory = true;
            if (SFD.ShowDialog() == true)
            {
                RenderTargetBitmap renderBmp = new RenderTargetBitmap((int)canvas1.ActualWidth, (int)canvas1.ActualHeight, 96, 96, PixelFormats.Default);
                renderBmp.Render(canvas1);
                BitmapEncoder enc = new BmpBitmapEncoder();
                try
                {
                    ImageNumber++;
                }
                catch (Exception)
                {
                    MessageBox.Show("The Image could not be saved.", "WaveFunction Collapse Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                try
                {
                    //Save the RenderBmp to the file
                    if (SFD.FilterIndex == 1)
                    {
                        enc = new BmpBitmapEncoder();
                    }
                    else if (SFD.FilterIndex == 2)
                    {
                        enc = new JpegBitmapEncoder();
                        ((JpegBitmapEncoder)enc).QualityLevel = 100;
                    }
                    enc.Frames.Add(BitmapFrame.Create(renderBmp));
                    FileStream sw = new FileStream(SFD.FileName, FileMode.Create);
                    enc.Save(sw);
                    sw.Close();
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Cannot save the result Image. Original error: " + Ex.Message, "Overlapping Wave Function Collapse error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
