using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snakes_and_Ladders
{
    internal class Connection
    {
        private Cell startCell;
        private Cell endCell;
        private bool IsLadder;
        private BitmapImage bmp;
        private static Random Rnd = new Random();

        public Connection(Cell start, Cell end, bool Ladder)
        {
            startCell = start;
            endCell = end;
            IsLadder = Ladder;
        }

        public Cell Start 
        { 
            get { return startCell; } 
        }

        public Cell End 
        { 
            get { return endCell; } 
        }

        public bool Ladder
        { 
            get { return IsLadder; } 
        }

        public void Draw(Canvas c)
        {
            if (IsLadder)
            {
                bmp = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Ladder.gif"));
            }
            else
            {
                if (Rnd.NextDouble() < 0.5)
                {
                    bmp = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Snake1.gif"));
                }
                else
                {
                    bmp = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Snake2.gif"));
                }
            }
            Point startPt = startCell.Center;
            Point endPt = endCell.Center;
            double imgWidth = bmp.Width;
            double imgHeight = bmp.PixelHeight;
            Point Imgcenter = startPt + (endPt - startPt) / 2;
            double angle = Math.Atan2(endPt.Y - startPt.Y, endPt.X - startPt.X + 2.0) + Math.PI / 2;
            FormatConvertedBitmap convertBitmap = new FormatConvertedBitmap(bmp, PixelFormats.Bgra32, null, 50);
            //Adjust the bitmap to go from the startCell to the endCell
            TransformedBitmap scaledBitmap = new TransformedBitmap();
            //Adjust the height of the bitmap
            ScaleTransform myScaleTransform = new ScaleTransform(1.0, (endPt - startPt).Length / imgHeight);
            //Rotate the bitmap to fit the X offset between the cells
            RotateTransform myRotateTransform = new RotateTransform(180 * angle / Math.PI, 0, 0);
            //Combine both Transforms
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myScaleTransform);
            myTransformGroup.Children.Add(myRotateTransform);
            //Create the image and apply the transforms
            Image image = new Image();
            image.Source = convertBitmap;
            image.RenderTransform = myTransformGroup;
            image.SetValue(Canvas.LeftProperty, endCell.Center.X - imgWidth / 2);
            image.SetValue(Canvas.TopProperty, Math.Min(startCell.Center.Y, endCell.Center.Y));
            c.Children.Add(image);
        }
    }
}
