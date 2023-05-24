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
        //Lists of tiles that are allowed on the adjacent tiles
        public List<int> UpOptions;
        public List<int> RightOptions;
        public List<int> DownOptions;
        public List<int> LeftOptions;

        public Tile(int id, string ImageFile)
        {
            ID = id;
            //Load the image from file into a WriteableBitmap
            BitmapImage bitmap;
            FormatConvertedBitmap convertBitmap;
            bitmap = new BitmapImage(new Uri(ImageFile));
            //Change to 32 bpp if needed
            if (bitmap.Format.BitsPerPixel != 32)
            {
                convertBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
                TileBmp = new WriteableBitmap(convertBitmap);
            }
            else
            {
                TileBmp = new WriteableBitmap(bitmap);
            }
        }
    }
}
