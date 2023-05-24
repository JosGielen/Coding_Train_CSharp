using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wave_Function_Collapse
{
    class Cell
    {
        public int index;
        public Image img;
        public int MaxOpts;
        public int[] TileOptions;  //Possible tile IDs that can go in this cell
        public bool Updated;
        public bool Collapsed;

        public Cell(int Id, int maxOptions)
        {
            index = Id;
            MaxOpts = maxOptions;
            TileOptions = new int[maxOptions];
            for (int I = 0; I < maxOptions; I++)
            {
                TileOptions[I] = I;
            }
            Collapsed = false;
            Updated = false;
        }

        public void CheckAllowedOptions(Tile[] Tiles, Cell[] grid, int maxRow, int maxCol)
        {
            List<int> AllowedOptions = new List<int>();
            List<int> AllowedUpOptions = new List<int>();
            List<int> AllowedRightOptions = new List<int>();
            List<int> AllowedDownOptions = new List<int>();
            List<int> AllowedLeftOptions = new List<int>();
            if (Collapsed) return;
            if (! Updated)
            {
                int upIndex = -1;
                int rightIndex = -1;
                int downIndex = -1;
                int leftIndex = -1;
                int row = (int)Math.Floor((double)index / maxCol);
                int col = index % maxCol;
                //Get the neighbour indices
                if (row > 0) upIndex = (row - 1) * maxCol + col;
                if (row < maxRow - 1) downIndex = (row + 1) * maxCol + col;
                if (col > 0) leftIndex = row * maxCol + col - 1;
                if (col < maxCol - 1) rightIndex = row * maxCol + col + 1;
                //Check UP neighbour Cell
                if (upIndex >= 0)
                {
                    foreach(int tileID in grid[upIndex].TileOptions )
                    {
                        foreach(int opt in Tiles[tileID].DownOptions )
                        {
                            if (!AllowedUpOptions.Contains(opt)) AllowedUpOptions.Add(opt);
                        }
                    }
                }
                //Check RIGHT neighbour Cell
                if (rightIndex >= 0)
                {
                    foreach (int tileID in grid[rightIndex].TileOptions)
                    {
                        foreach (int opt in Tiles[tileID].LeftOptions)
                        {
                            if (!AllowedRightOptions.Contains(opt)) AllowedRightOptions.Add(opt);
                        }
                    }
                }
                //Check DOWN neighbour Cell
                if (downIndex >= 0)
                {
                    foreach (int tileID in grid[downIndex].TileOptions)
                    {
                        foreach (int opt in Tiles[tileID].UpOptions)
                        {
                            if (!AllowedDownOptions.Contains(opt)) AllowedDownOptions.Add(opt);
                        }
                    }
                }
                //Check LEFT neighbour Cell
                if (leftIndex >= 0)
                {
                    foreach (int tileID in grid[leftIndex].TileOptions)
                    {
                        foreach (int opt in Tiles[tileID].RightOptions)
                        {
                            if (!AllowedLeftOptions.Contains(opt)) AllowedLeftOptions.Add(opt);
                        }
                    }
                }
                //Determine the options that are allowed by all 4 neighbours
                bool allowed;
                for (int I = 0; I < MaxOpts; I++)
                {
                    allowed = true;
                    if (upIndex >= 0 && !AllowedUpOptions.Contains(I)) allowed = false;
                    if (rightIndex >= 0 && !AllowedRightOptions.Contains(I)) allowed = false;
                    if (downIndex >= 0 && !AllowedDownOptions.Contains(I)) allowed = false;
                    if (leftIndex >= 0 && !AllowedLeftOptions.Contains(I)) allowed = false;
                    if (allowed) AllowedOptions.Add(I);
                }
                TileOptions = AllowedOptions.ToArray();
                Updated = true;
            }
        }
    }
}
