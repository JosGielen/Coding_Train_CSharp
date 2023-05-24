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
        private readonly int waitTime = 50;
        private int TileCount;  //Number of tiles that can be used
        private int my_Rows;
        private int my_Cols;
        private double CellWidth;
        private double CellHeight;
        private Tile[] Tiles;
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
            my_Cols = 30;
            //Create the Tiles
            TileCount = 5;
            Tiles = new Tile[TileCount];
            Tiles[0] = new Tile(0, Environment.CurrentDirectory + "\\Tiles\\Blank.jpg");
            Tiles[1] = new Tile(1, Environment.CurrentDirectory + "\\Tiles\\Up.jpg");
            Tiles[2] = new Tile(2, Environment.CurrentDirectory + "\\Tiles\\Right.jpg");
            Tiles[3] = new Tile(3, Environment.CurrentDirectory + "\\Tiles\\Down.jpg");
            Tiles[4] = new Tile(4, Environment.CurrentDirectory + "\\Tiles\\Left.jpg");
            //Set the Rules
            Tiles[0].UpOptions = new List<int>() { 0, 1 };
            Tiles[0].RightOptions = new List<int>() { 0, 2 };
            Tiles[0].DownOptions = new List<int>() { 0, 3 };
            Tiles[0].LeftOptions = new List<int>() { 0, 4 };

            Tiles[1].UpOptions = new List<int>() { 2, 3, 4 };
            Tiles[1].RightOptions = new List<int>() { 1, 3, 4 };
            Tiles[1].DownOptions = new List<int>() { 0, 3 };
            Tiles[1].LeftOptions = new List<int>() { 1, 2, 3 };

            Tiles[2].UpOptions = new List<int>() { 2, 3, 4 };
            Tiles[2].RightOptions = new List<int>() { 1, 3, 4 };
            Tiles[2].DownOptions = new List<int>() { 1, 2, 4 };
            Tiles[2].LeftOptions = new List<int>() { 0, 4 };

            Tiles[3].UpOptions = new List<int>() { 0, 1 };
            Tiles[3].RightOptions = new List<int>() { 1, 3, 4 };
            Tiles[3].DownOptions = new List<int>() { 1, 2, 4 };
            Tiles[3].LeftOptions = new List<int>() { 1, 2, 3 };

            Tiles[4].UpOptions = new List<int>() { 2, 3, 4 };
            Tiles[4].RightOptions = new List<int>() { 0, 2 };
            Tiles[4].DownOptions = new List<int>() { 1, 2, 4 };
            Tiles[4].LeftOptions = new List<int>() { 1, 2, 3 };
            //Create the Cells
            Init();
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
                    if (Recording) MakeGif(15);
                    Recording = false;
                    Started = false;
                    BtnStart.Content = "START";
                    return;
                }
                //--Sort the non-collapsed cells according to the number of tileoptions
                sortedGrid = sortedGrid.OrderBy(x => x.TileOptions.Length).ToList();

                if (sortedGrid[sortedGrid.Count -1].TileOptions.Length == 0)
                {
                    Init();
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
                Render();
            }
            else
            {
                //Stop the collapse
                Started = false;
                BtnStart.Content = "START";
            }
        }
    }


    //Maxim Gumin GitHub example
    //    <set size = "14" >
    //	< tiles >
    //		< tile name="bridge" symmetry="I" weight="1.0"/>
    //		<tile name = "component" symmetry="X" weight="20.0"/>
    //		<tile name = "connection" symmetry="T" weight="10.0"/>
    //		<tile name = "corner" symmetry="L" weight="10.0"/>
    //		<tile name = "substrate" symmetry="X" weight="2.0"/>
    //		<tile name = "t" symmetry="T" weight="0.1"/>
    //		<tile name = "track" symmetry="I" weight="2.0"/>
    //		<tile name = "transition" symmetry="T" weight="0.4"/>
    //		<tile name = "turn" symmetry="L" weight="1.0"/>
    //		<tile name = "viad" symmetry="I" weight="0.1"/>
    //		<tile name = "vias" symmetry="T" weight="0.3"/>
    //		<tile name = "wire" symmetry="I" weight="0.5"/>
    //		<tile name = "skew" symmetry="L" weight="2.0"/>
    //		<tile name = "dskew" symmetry="\" weight="2.0"/>
    //	</tiles>
    //	<neighbors>
    //		<neighbor left = "bridge" right="bridge"/>
    //		<neighbor left = "bridge 1" right="bridge 1"/>
    //		<neighbor left = "bridge 1" right="connection 1"/>
    //		<neighbor left = "bridge 1" right="t 2"/>
    //		<neighbor left = "bridge 1" right="t 3"/>
    //		<neighbor left = "bridge 1" right="track 1"/>
    //		<neighbor left = "bridge" right="transition 1"/>
    //		<neighbor left = "bridge 1" right="turn 1"/>
    //		<neighbor left = "bridge 1" right="viad"/>
    //		<neighbor left = "bridge 1" right="vias 1"/>
    //		<neighbor left = "bridge" right="wire"/>
    //		<neighbor left = "component" right="component"/>
    //		<neighbor left = "connection 1" right="component"/>
    //		<neighbor left = "connection" right="connection"/>
    //		<neighbor left = "connection" right="corner"/>
    //		<neighbor left = "t 1" right="connection 1"/>
    //		<neighbor left = "t 2" right="connection 1"/>
    //		<neighbor left = "track 1" right="connection 1"/>
    //		<neighbor left = "turn" right="connection 1"/>
    //		<neighbor left = "substrate" right="corner 1"/>
    //		<neighbor left = "t 3" right="corner 1"/>
    //		<neighbor left = "track" right="corner 1"/>
    //		<neighbor left = "transition 2" right="corner 1"/>
    //		<neighbor left = "transition" right="corner 1"/>
    //		<neighbor left = "turn 1" right="corner 1"/>
    //		<neighbor left = "turn 2" right="corner 1"/>
    //		<neighbor left = "viad 1" right="corner 1"/>
    //		<neighbor left = "vias 1" right="corner 1"/>
    //		<neighbor left = "vias 2" right="corner 1"/>
    //		<neighbor left = "vias" right="corner 1"/>
    //		<neighbor left = "wire 1" right="corner 1"/>
    //		<neighbor left = "substrate" right="substrate"/>
    //		<neighbor left = "substrate" right="t 1"/>
    //		<neighbor left = "substrate" right="track"/>
    //		<neighbor left = "substrate" right="transition 2"/>
    //		<neighbor left = "substrate" right="turn"/>
    //		<neighbor left = "substrate" right="viad 1"/>
    //		<neighbor left = "substrate" right="vias 2"/>
    //		<neighbor left = "substrate" right="vias 3"/>
    //		<neighbor left = "substrate" right="wire 1"/>
    //		<neighbor left = "t 1" right="t 3"/>
    //		<neighbor left = "t 3" right="t 1"/>
    //		<neighbor left = "t 1" right="t 2"/>
    //		<neighbor left = "t 2" right="t 2"/>
    //		<neighbor left = "t 2" right="t"/>
    //		<neighbor left = "t 3" right="track"/>
    //		<neighbor left = "t 1" right="track 1"/>
    //		<neighbor left = "t 2" right="track 1"/>
    //		<neighbor left = "t 1" right="transition 3"/>
    //		<neighbor left = "t 3" right="transition 2"/>
    //		<neighbor left = "t 2" right="transition 3"/>
    //		<neighbor left = "t 3" right="turn"/>
    //		<neighbor left = "t 1" right="turn 1"/>
    //		<neighbor left = "t 2" right="turn 1"/>
    //		<neighbor left = "t 2" right="turn 2"/>
    //		<neighbor left = "t 3" right="viad 1"/>
    //		<neighbor left = "t 1" right="viad"/>
    //		<neighbor left = "t 2" right="viad"/>
    //		<neighbor left = "t 2" right="vias 1"/>
    //		<neighbor left = "t 1" right="vias 1"/>
    //		<neighbor left = "vias 1" right="t 1"/>
    //		<neighbor left = "vias 2" right="t 1"/>
    //		<neighbor left = "wire 1" right="t 1"/>
    //		<neighbor left = "track" right="track"/>
    //		<neighbor left = "track 1" right="track 1"/>
    //		<neighbor left = "track 1" right="transition 3"/>
    //		<neighbor left = "track" right="transition 2"/>
    //		<neighbor left = "track" right="turn"/>
    //		<neighbor left = "track 1" right="turn 1"/>
    //		<neighbor left = "track" right="viad 1"/>
    //		<neighbor left = "track 1" right="viad"/>
    //		<neighbor left = "track" right="vias 2"/>
    //		<neighbor left = "track" right="vias 3"/>
    //		<neighbor left = "track 1" right="vias 1"/>
    //		<neighbor left = "track" right="wire 1"/>
    //		<neighbor left = "transition 2" right="turn"/>
    //		<neighbor left = "transition" right="turn"/>
    //		<neighbor left = "transition 1" right="turn 1"/>
    //		<neighbor left = "transition 2" right="viad 1"/>
    //		<neighbor left = "transition 2" right="vias 2"/>
    //		<neighbor left = "transition 2" right="vias 3"/>
    //		<neighbor left = "transition 2" right="vias"/>
    //		<neighbor left = "wire" right="transition 1"/>
    //		<neighbor left = "transition 2" right="wire 1"/>
    //		<neighbor left = "turn 1" right="turn"/>
    //		<neighbor left = "turn 2" right="turn"/>
    //		<neighbor left = "turn" right="turn 1"/>
    //		<neighbor left = "turn" right="turn 2"/>
    //		<neighbor left = "turn 1" right="viad 1"/>
    //		<neighbor left = "turn" right="viad"/>
    //		<neighbor left = "turn 1" right="vias 2"/>
    //		<neighbor left = "turn 1" right="vias 3"/>
    //		<neighbor left = "turn 1" right="vias"/>
    //		<neighbor left = "turn" right="vias 1"/>
    //		<neighbor left = "turn 1" right="wire 1"/>
    //		<neighbor left = "viad 1" right="viad 1"/>
    //		<neighbor left = "viad 1" right="vias 2"/>
    //		<neighbor left = "viad 1" right="vias 3"/>
    //		<neighbor left = "viad 1" right="wire 1"/>
    //		<neighbor left = "vias 1" right="wire 1"/>
    //		<neighbor left = "vias 2" right="wire 1"/>
    //		<neighbor left = "vias 1" right="vias 3"/>
    //		<neighbor left = "vias 2" right="vias 2"/>
    //		<neighbor left = "vias 2" right="vias"/>
    //		<neighbor left = "wire" right="wire"/>
    //		<neighbor left = "wire 1" right="wire 1"/>
    //		<neighbor left = "bridge 1" right="dskew"/>
    //		<neighbor left = "connection 3" right="dskew"/>
    //		<neighbor left = "dskew" right="dskew"/>
    //		<neighbor left = "skew" right="dskew"/>
    //		<neighbor left = "t" right="dskew"/>
    //		<neighbor left = "t 2" right="dskew"/>
    //		<neighbor left = "t 1" right="dskew"/>
    //		<neighbor left = "track 1" right="dskew"/>
    //		<neighbor left = "transition 1" right="dskew"/>
    //		<neighbor left = "turn 3" right="dskew"/>
    //		<neighbor left = "viad" right="dskew"/>
    //		<neighbor left = "vias 3" right="dskew"/>
    //		<neighbor left = "skew" right="bridge 1"/>
    //		<neighbor left = "skew" right="connection 1"/>
    //		<neighbor left = "corner" right="skew"/>
    //		<neighbor left = "corner 3" right="skew"/>
    //		<neighbor left = "skew" right="dskew"/>
    //		<neighbor left = "skew" right="skew 2"/>
    //		<neighbor left = "skew 1" right="skew"/>
    //		<neighbor left = "skew 1" right="skew 3"/>
    //		<neighbor left = "substrate" right="skew"/>
    //		<neighbor left = "t 3" right="skew"/>
    //		<neighbor left = "t" right="skew 2"/>
    //		<neighbor left = "t 2" right="skew 2"/>
    //		<neighbor left = "t 1" right="skew 2"/>
    //		<neighbor left = "track" right="skew"/>
    //		<neighbor left = "track 1" right="skew 2"/>
    //		<neighbor left = "transition" right="skew"/>
    //		<neighbor left = "transition 1" right="skew 2"/>
    //		<neighbor left = "turn 1" right="skew"/>
    //		<neighbor left = "turn 2" right="skew"/>
    //		<neighbor left = "turn 3" right="skew 2"/>
    //		<neighbor left = "viad 1" right="skew"/>
    //		<neighbor left = "viad" right="skew 2"/>
    //		<neighbor left = "vias" right="skew"/>
    //		<neighbor left = "vias 1" right="skew"/>
    //		<neighbor left = "vias 2" right="skew"/>
    //		<neighbor left = "vias 3" right="skew 2"/>
    //		<neighbor left = "wire 1" right="skew"/>
    //	</neighbors>
    //	<subsets>
    //		<subset name = "Turnless" >
    //			< tile name="bridge"/>
    //			<tile name = "component" />
    //			< tile name="connection"/>
    //			<tile name = "corner" />
    //			< tile name="substrate"/>
    //			<tile name = "t" />
    //			< tile name="track"/>
    //			<tile name = "transition" />
    //			< tile name="viad"/>
    //			<tile name = "vias" />
    //			< tile name="wire"/>
    //			<tile name = "skew" />
    //			< tile name="dskew"/>
    //		</subset>
    //	</subsets>
    //</set>


}
