using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snowfall
{
    internal class Snowflake
    {
        private Point my_Position;
        private Vector velocity;
        private BitmapImage bmp;
        private byte[] PixelData;
        private int Stride = 0;
        private int my_Size;
        private bool Persistent;
        private int w;
        private int h;
        private Random Rnd = new Random();


        public Snowflake(Point pos, bool persist, int size) //Size = 10 - 40
        {
            my_Position = pos;
            Persistent = persist;
            my_Size = size;
            if (Persistent)
            {
                velocity = new Vector(0, size / 25.0);
                if (size < 18)
                {
                    flakeDesign1(); //2x2
                }
                else if (size < 26)
                {
                    flakeDesign2(); //3x3
                }
                else if (size < 34 )
                {
                    flakeDesign3(); //4x4
                }
                else
                {
                    flakeDesign4(); //5x5
                }
            }
            else
            {
                velocity = new Vector(0, size / 15.0);
                int shapeNr = Rnd.Next(1,5);
                bmp = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\FlakeL" + shapeNr.ToString() + ".jpg"));
                FormatConvertedBitmap convertBitmap = new FormatConvertedBitmap(bmp, PixelFormats.Bgra32, null, 50);
                TransformedBitmap scaledBitmap =  new TransformedBitmap();
                scaledBitmap.BeginInit();
                scaledBitmap.Source = convertBitmap;
                scaledBitmap.Transform = new ScaleTransform(size / 55.0, size / 55.0);
                scaledBitmap.EndInit();
                WriteableBitmap Wbmp = new WriteableBitmap(scaledBitmap);
                Stride = Wbmp.PixelWidth * Wbmp.Format.BitsPerPixel / 8;
                PixelData = new byte[Stride * Wbmp.PixelHeight];
                Wbmp.CopyPixels(PixelData, Stride, 0);
                w = Wbmp.PixelWidth;
                h = Wbmp.PixelHeight;
            }
            for (int i = 0; i < PixelData.Length; i+=4)
            {
                if (PixelData[i] < 20) { PixelData[i + 3] = 0;}
            }
        }

        public byte[] FlakeData
        {
            get { return PixelData; }
        }

        public bool Persists
        {
            get { return Persistent; }
        }

        public int X
        {
            get { return (int)my_Position.X; }
        }

        public double Y
        {
            get { return my_Position.Y; }
        }

        public int Size
        {
            get { return my_Size; }
        }

        public void SetPosition(Point pos)
        {
            my_Position = pos;
        }

        public void UpdatePosition(Vector wind)
        {
            velocity.X = wind.X;
            my_Position += velocity;
        }

        public void Draw(WriteableBitmap WB)
        {
            if (my_Position.X + w > WB.PixelWidth) { my_Position.X = 0; }
            if (my_Position.X < 0) { my_Position.X = WB.PixelWidth - w; }
            if (my_Position.Y + h > WB.PixelHeight) { my_Position.Y = 0;}
            if (my_Position.Y < 0) { my_Position.Y = WB.PixelHeight - h; }
            WB.WritePixels(new Int32Rect((int)my_Position.X, (int)my_Position.Y, w, h), PixelData, Stride, 0);
        }

        private void flakeDesign1() //2x2 flake 
        {
            PixelData = new byte[16];
            Stride = 8;
            w = 2;
            h = 2;
            SetPixel(0, 0, Stride);
            SetPixel(1, 0, Stride);
            SetPixel(0, 1, Stride);
            SetPixel(1, 1, Stride);
        }

        private void flakeDesign2() //3x3 flake 
        {
            PixelData = new byte[36];
            Stride = 12;
            w = 3;
            h = 3;
            SetPixel(0, 0, Stride);
            SetPixel(1, 0, Stride);
            SetPixel(2, 0, Stride);
            SetPixel(0, 1, Stride);
            SetPixel(1, 1, Stride);
            SetPixel(2, 1, Stride);
            SetPixel(0, 2, Stride);
            SetPixel(1, 2, Stride);
            SetPixel(2, 2, Stride);
        }

        private void flakeDesign3() //4x4 flake 
        {
            PixelData = new byte[64];
            Stride = 16;
            w = 4;
            h = 4;
            SetPixel(1, 0, Stride);
            SetPixel(2, 0, Stride);
            SetPixel(0, 1, Stride);
            SetPixel(1, 1, Stride);
            SetPixel(2, 1, Stride);
            SetPixel(3, 1, Stride);
            SetPixel(0, 2, Stride);
            SetPixel(1, 2, Stride);
            SetPixel(2, 2, Stride);
            SetPixel(3, 2, Stride);
            SetPixel(1, 3, Stride);
            SetPixel(2, 3, Stride);
        }

        private void flakeDesign4() //5x5 flake 
        {
            PixelData = new byte[100];
            Stride = 20;
            w = 5;
            h = 5;
            SetPixel(2, 0, Stride);
            SetPixel(1, 1, Stride);
            SetPixel(2, 1, Stride);
            SetPixel(3, 1, Stride);
            SetPixel(0, 2, Stride);
            SetPixel(1, 2, Stride);
            SetPixel(2, 2, Stride);
            SetPixel(3, 2, Stride);
            SetPixel(4, 2, Stride);
            SetPixel(1, 3, Stride);
            SetPixel(2, 3, Stride);
            SetPixel(3, 3, Stride);
            SetPixel(2, 4, Stride);
        }

        private void SetPixel(int X, int Y, int PixStride)
        {
            int xIndex = X * 4;
            int yIndex = Y * PixStride;
            if (xIndex + yIndex >= 0 & xIndex + yIndex + 2 < PixelData.Length)
            {
                PixelData[xIndex + yIndex + 0] = 255;
                PixelData[xIndex + yIndex + 1] = 255;
                PixelData[xIndex + yIndex + 2] = 255;
                PixelData[xIndex + yIndex + 3] = 255;
            }
        }

        private Color GetPixel(Point p, byte[] buffer, int bitsperPixel, int PixStride)
        {
            Color c = new Color();
            int xIndex = (int)p.X * bitsperPixel / 8;
            int yIndex = (int)p.Y * PixStride;
            if (xIndex + yIndex >= 0 & xIndex + yIndex + 2 < buffer.Length)
            {
                c.R = buffer[xIndex + yIndex + 0];
                c.G = buffer[xIndex + yIndex + 1];
                c.B = buffer[xIndex + yIndex + 2];
            }
            return c;
        }
    }
}
