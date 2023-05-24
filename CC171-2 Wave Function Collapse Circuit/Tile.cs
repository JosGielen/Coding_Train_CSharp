using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wave_Function_Collapse
{
    class Tile
    {
        public int ID;
        public WriteableBitmap TileBmp;
        public string UpEdge;
        public string RightEdge;
        public string DownEdge;
        public string LeftEdge;
        //Lists of tiles that are allowed on the adjacent tiles
        public List<int> UpOptions;
        public List<int> RightOptions;
        public List<int> DownOptions;
        public List<int> LeftOptions;

        public Tile(int id, WriteableBitmap bmp, string up, string right, string down, string left)
        {
            ID = id;
            TileBmp = bmp;
            UpEdge = up;
            RightEdge = right;
            DownEdge = down;
            LeftEdge = left;
            UpOptions = new List<int>();
            RightOptions = new List<int>();
            DownOptions = new List<int>();
            LeftOptions = new List<int>();
        }

        public Tile Copy()
        {
            Tile result = new Tile(ID, TileBmp.Clone(), UpEdge, RightEdge, DownEdge, LeftEdge);
            for (int I = 0; I < UpOptions.Count; I++)
            {
                result.UpOptions.Add(UpOptions[I]);
            }
            for (int I = 0; I < RightOptions.Count; I++)
            {
                result.RightOptions.Add(RightOptions[I]);
            }
            for (int I = 0; I < DownOptions.Count; I++)
            {
                result.DownOptions.Add(DownOptions[I]);
            }
            for (int I = 0; I < LeftOptions.Count; I++)
            {
                result.LeftOptions.Add(LeftOptions[I]);
            }
            return result;
        }
    }
}
