using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Animated_Sprites
{
    internal class Sprite: Image
    {
        private int HorizontalParts;
        private int VerticalParts;
        private WriteableBitmap bmp;
        private byte[] PixelData;
        private int Stride;
        private double my_Speed;
        private int counter;
        private readonly List<WriteableBitmap> images;

        public Sprite(string imageFile, int H_Parts, int V_Parts)
        {
            images = new List<WriteableBitmap>();
            //Load the imageFile in a byte[]
            BitmapImage bitmap = new BitmapImage(new Uri(imageFile));
            FormatConvertedBitmap convertBitmap;
            //Change to 32 bpp if needed
            if (bitmap.Format.BitsPerPixel != 32)
            {
                convertBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
                bmp = new WriteableBitmap(convertBitmap);
            }
            else
            {
                bmp = new WriteableBitmap(bitmap);
            }
            Stride = bmp.PixelWidth * bmp.Format.BitsPerPixel / 8;
            PixelData = new byte[Stride * bmp.PixelHeight];
            HorizontalParts = H_Parts;
            VerticalParts = V_Parts;
            bmp.CopyPixels(PixelData, Stride, 0);
            //Get the individual images of the spriteSheet
            for (int i = 0; i < VerticalParts; i++)
            {
                for (int j = 0; j < HorizontalParts ; j++)
                {
                    images.Add(GetImagePart(i, j));
                }
            }
            Source = images[0];
            counter = 0;
        }

        private WriteableBitmap GetImagePart(int Vpart, int Hpart)
        {
            Width = bmp.PixelWidth / HorizontalParts;
            Height = bmp.PixelHeight / VerticalParts;
            WriteableBitmap result = new WriteableBitmap((int)Width, (int)Height, 96, 96, PixelFormats.Bgra32, bmp.Palette);
            int PartStride = result.PixelWidth * result.Format.BitsPerPixel / 8;
            byte[] PartBytes = new byte[PartStride * result.PixelHeight];
            int readIndex;
            int writeIndex;
            for (int Y = 0; Y < Height; Y++)
            {
                for(int X = 0; X < Width; X++)
                {
                    readIndex = (int)(Vpart * Height + Y) * Stride + (int)(Hpart * Width + X) * result.Format.BitsPerPixel / 8;
                    writeIndex = Y * PartStride + X * result.Format.BitsPerPixel / 8;
                    if (PixelData[readIndex] > 240 && PixelData[readIndex + 1] > 240 && PixelData[readIndex + 2] > 240)
                    {
                        PartBytes[writeIndex] = PixelData[readIndex];
                        PartBytes[writeIndex + 1] = PixelData[readIndex + 1];
                        PartBytes[writeIndex + 2] = PixelData[readIndex + 2];
                        PartBytes[writeIndex + 3] = 0;
                    }
                    else
                    {
                        PartBytes[writeIndex] = PixelData[readIndex];
                        PartBytes[writeIndex + 1] = PixelData[readIndex + 1];
                        PartBytes[writeIndex + 2] = PixelData[readIndex + 2];
                        PartBytes[writeIndex + 3] = PixelData[readIndex + 3];
                    }
                }
            }
            result.WritePixels(new Int32Rect(0, 0, (int)Width, (int)Height), PartBytes, PartStride, 0);
            return result;
        }

        public double Speed
        {
            get { return my_Speed; }
            set { my_Speed = value; }
        }

        public void Update()
        {
            counter++;
            if (counter >= images.Count)
            {
                counter = 0;
            }
            Source = images[counter];
        }
    }
}
