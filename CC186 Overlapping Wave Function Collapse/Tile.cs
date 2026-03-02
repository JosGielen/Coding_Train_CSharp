using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wave_Function_Collapse
{
    class Tile
    {
        public int ID;
        public int my_Size;
        private byte[] my_PixelData;
        private WriteableBitmap my_TileBmp;
        //Lists of tiles that are allowed on the adjacent tiles
        public List<int> UpOptions;
        public List<int> RightOptions;
        public List<int> DownOptions;
        public List<int> LeftOptions;

        public Tile(int id, int Size, byte[] PixelData)
        {
            ID = id;
            my_Size = Size;
            my_PixelData = PixelData;
            //The image is filled with the same color as the center pixel 
            byte[] ImageData = new byte[PixelData.Length];
            int centerIndex;
            if (Size % 2 == 0)
            {
                centerIndex = Size * (Size - 1) * 2;
            }
            else
            {
                centerIndex = (Size * Size - 1) * 2;
            }
            for (int I = 0; I < PixelData.Length; I += 4)
            {
                ImageData[I + 0] = PixelData[centerIndex + 0];
                ImageData[I + 1] = PixelData[centerIndex + 1];
                ImageData[I + 2] = PixelData[centerIndex + 2];
                ImageData[I + 3] = PixelData[centerIndex + 3];

            }
            my_TileBmp = new WriteableBitmap(Size, Size, 96, 96, PixelFormats.Bgra32, null);
            my_TileBmp.WritePixels(new Int32Rect(0, 0, Size, Size), ImageData, 4 * Size, 0);
            UpOptions = new List<int>();
            RightOptions = new List<int>();
            DownOptions = new List<int>();
            LeftOptions = new List<int>();
        }

        public byte[] PixelData
        {
            get { return my_PixelData; }
        }

        public WriteableBitmap TileBmp
        {
            get { return my_TileBmp; }
        }

        public Tile Copy()
        {
            Tile result = new Tile(ID, my_Size, my_PixelData);
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
