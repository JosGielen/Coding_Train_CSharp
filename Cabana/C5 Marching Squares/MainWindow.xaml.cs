using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Marching_Squares
{

    public partial class MainWindow : Window
    {
        private WriteableBitmap writeableBmp;
        private readonly int rez = 2;
        private int cols;
        private int rows;
        private double Xoff;
        private double Yoff;
        private double Zoff;
        private double[,] field;
        private int[,] intField;
        private Vector[,] locations;
        private bool filled = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            writeableBmp = BitmapFactory.New((int)UserArea.ActualWidth, (int)UserArea.ActualHeight);
            Image1.Source = writeableBmp;
            cols = writeableBmp.PixelWidth / rez + 1;
            rows = writeableBmp.PixelWidth / rez + 1;
            field = new double[cols, rows];
            intField = new int[cols, rows];
            locations = new Vector[cols, rows];
            Xoff = 0;
            Yoff = 0;
            Zoff = 0;
            for (int I = 0; I < cols; I++)
            {
                for (int J = 0; J < rows; J++)
                {
                    locations[I, J] = new Vector(I * rez, J * rez);
                }
            }
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            using (writeableBmp.GetBitmapContext())
            {
                writeableBmp.Clear();
                //Fill the field with OpenSimplexNoise
                Xoff = 0;
                for (int I = 0; I < cols; I++)
                {
                    Yoff = 0;
                    for (int J = 0; J < rows; J++)
                    {
                        field[I, J] = FastSimplexNoise.Noise3D(Xoff, Yoff, Zoff);
                        if (field[I, J] < 0.5)
                        {
                            intField[I, J] = 0;
                        }
                        else
                        {
                            intField[I, J] = 1;
                        }
                        if (filled) writeableBmp.FillRectangle((int)((I - 0.5) * rez), (int)((J - 0.5) * rez), (int)((I + 0.5) * rez), (int)((J + 0.5) * rez), Color.FromRgb((byte)(255 * field[I, J]), (byte)(255 * field[I, J]), (byte)(255 * field[I, J])));
                        Yoff += 0.03;
                    }
                    Xoff += 0.03;
                }
                Zoff += 0.006;

                //Marching Squares
                Marching_Square MS;
                Point topLeft;
                Point bottomRight;
                for (int I = 0; I < cols - 1; I++)
                {
                    for (int J = 0; J < rows - 1; J++)
                    {
                        topLeft = new Point((I - 0.5) * rez, (J - 0.5) * rez);
                        bottomRight = new Point((I + 0.5) * rez, (J + 0.5) * rez);
                        MS = new Marching_Square(topLeft, bottomRight);
                        foreach (Line l in MS.GetLines(intField[I, J], intField[I + 1, J], intField[I + 1, J + 1], intField[I, J + 1]))
                        {
                            //Draw 2 lines to make a better visible linethickness 2
                            writeableBmp.DrawLine((int)l.X1, (int)l.Y1, (int)l.X2, (int)l.Y2, Colors.Red);
                            writeableBmp.DrawLine((int)l.X1, (int)l.Y1 + 1, (int)l.X2, (int)l.Y2 + 1, Colors.Red);
                        }
                    }
                }

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
