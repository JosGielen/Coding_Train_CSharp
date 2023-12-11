using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Pentominoes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Program members
        private delegate void RenderDelegate(int I, double X, double Y);
        private RenderDelegate renderDelegate;
        private delegate void WaitDelegate();
        private WaitDelegate waitDelegate;
        private bool AppRunning = false;
        private bool AppLoaded = false;
        private int RowNum = 6;
        private int ColNum = 10;
        private double CelWidth = 0.0;
        private double CelHeight = 0.0;
        private bool[] Cells = new bool[61];  //false = Occupied, true = !Occupied ;
        private Pentomino[] Pent = new Pentomino[64];
        private int SleepTime = 0;
        private int GroupNumber = 0;
        private int[] CelGroup = new int[61];
        private bool Animated = true;
        private bool SingleStep = false;
        private bool Nextstep = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region "Initialization"

        private void Init()
        {
            Line gridLine;
            double VeldWidth;
            double VeldHeight;
            VeldWidth = Canvas1.ActualWidth;
            VeldHeight = Canvas1.ActualHeight;
            CelWidth = VeldWidth / ColNum;
            CelHeight = VeldHeight / RowNum;
            Canvas1.Children.Clear();
            //Free all Cells
            for (int I = 0; I <= 60; I++)
            {
                Cells[I] = true;
            }
            //Set all Pentominoes to not used and give them the cell dimensions
            for (int I = 1; I <= 63; I++)
            {
                Pent[I].Used = false;
                Pent[I].CelWidth = CelWidth;
                Pent[I].CelHeight = CelHeight;
            }
            //Draw border of the canvas
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = 1,
                Y1 = 0,
                X2 = VeldWidth,
                Y2 = 0
            };
            Canvas1.Children.Add(gridLine);
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = VeldWidth,
                Y1 = 0,
                X2 = VeldWidth,
                Y2 = VeldHeight
            };
            Canvas1.Children.Add(gridLine);
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = 1,
                Y1 = 0,
                X2 = 1,
                Y2 = VeldHeight
            };
            Canvas1.Children.Add(gridLine);
            gridLine = new Line
            {
                Stroke = Brushes.LightGray,
                X1 = 1,
                Y1 = VeldHeight,
                X2 = VeldWidth,
                Y2 = VeldHeight
            };
            Canvas1.Children.Add(gridLine);
            //Draw Vertical gridlines
            for (int I = 0; I < ColNum - 1; I++)
            {
                gridLine = new Line
                {
                    Stroke = Brushes.LightGray,
                    X1 = CelWidth * (I + 1),
                    Y1 = 0,
                    X2 = CelWidth * (I + 1),
                    Y2 = VeldHeight
                };
                Canvas1.Children.Add(gridLine);
            }
            //Draw Horizontal gridlines
            for (int I = 0; I < RowNum - 1; I++)
            {
                gridLine = new Line
                {
                    Stroke = Brushes.LightGray,
                    X1 = 0,
                    Y1 = CelHeight * (I + 1),
                    X2 = VeldWidth,
                    Y2 = CelHeight * (I + 1)
                };
                Canvas1.Children.Add(gridLine);
            }
        }

        private void CreatePentominoes()
        {
            //Create color array for the Pentomino colors
            Brush[] colors = new Brush[13];
            colors[1] = new SolidColorBrush(Color.FromRgb(247, 151, 167));
            colors[2] = new SolidColorBrush(Color.FromRgb(208, 168, 157));
            colors[3] = new SolidColorBrush(Color.FromRgb(196, 196, 130));
            colors[4] = new SolidColorBrush(Color.FromRgb(175, 206, 145));
            colors[5] = new SolidColorBrush(Color.FromRgb(155, 220, 155));
            colors[6] = new SolidColorBrush(Color.FromRgb(150, 215, 185));
            colors[7] = new SolidColorBrush(Color.FromRgb(150, 200, 200));
            colors[8] = new SolidColorBrush(Color.FromRgb(160, 190, 167));
            colors[9] = new SolidColorBrush(Color.FromRgb(160, 160, 240));
            colors[10] = new SolidColorBrush(Color.FromRgb(190, 160, 240));
            colors[11] = new SolidColorBrush(Color.FromRgb(190, 160, 200));
            colors[12] = new SolidColorBrush(Color.FromRgb(230, 160, 200));
            //L pentomino not rotated, not mirrored
            Pent[1] = new Pentomino("L", 1, colors[1]);
            Pent[1].Cells.Add(new Point(0, 0));
            Pent[1].Cells.Add(new Point(0, 1));
            Pent[1].Cells.Add(new Point(0, 2));
            Pent[1].Cells.Add(new Point(0, 3));
            Pent[1].Cells.Add(new Point(1, 3));
            Pent[1].Points.Add(new Point(0, 0));
            Pent[1].Points.Add(new Point(0, 4));
            Pent[1].Points.Add(new Point(2, 4));
            Pent[1].Points.Add(new Point(2, 3));
            Pent[1].Points.Add(new Point(1, 3));
            Pent[1].Points.Add(new Point(1, 0));
            //L pentomino 90° rotated, not mirrored
            Pent[2] = new Pentomino("L", 2, colors[1]);
            Pent[2].Cells.Add(new Point(0, 0));
            Pent[2].Cells.Add(new Point(1, 0));
            Pent[2].Cells.Add(new Point(2, 0));
            Pent[2].Cells.Add(new Point(3, 0));
            Pent[2].Cells.Add(new Point(0, 1));
            Pent[2].Points.Add(new Point(0, 0));
            Pent[2].Points.Add(new Point(4, 0));
            Pent[2].Points.Add(new Point(4, 1));
            Pent[2].Points.Add(new Point(1, 1));
            Pent[2].Points.Add(new Point(1, 2));
            Pent[2].Points.Add(new Point(0, 2));
            //L pentomino 180° rotated, not mirrored
            Pent[3] = new Pentomino("L", 3, colors[1]);
            Pent[3].Cells.Add(new Point(0, 0));
            Pent[3].Cells.Add(new Point(1, 0));
            Pent[3].Cells.Add(new Point(1, 1));
            Pent[3].Cells.Add(new Point(1, 2));
            Pent[3].Cells.Add(new Point(1, 3));
            Pent[3].Points.Add(new Point(0, 0));
            Pent[3].Points.Add(new Point(2, 0));
            Pent[3].Points.Add(new Point(2, 4));
            Pent[3].Points.Add(new Point(1, 4));
            Pent[3].Points.Add(new Point(1, 1));
            Pent[3].Points.Add(new Point(0, 1));
            //L pentomino 270° rotated, not mirrored
            Pent[4] = new Pentomino("L", 4, colors[1]);
            Pent[4].Cells.Add(new Point(0, 0));
            Pent[4].Cells.Add(new Point(0, 1));
            Pent[4].Cells.Add(new Point(-1, 1));
            Pent[4].Cells.Add(new Point(-2, 1));
            Pent[4].Cells.Add(new Point(-3, 1));
            Pent[4].Points.Add(new Point(0, 0));
            Pent[4].Points.Add(new Point(1, 0));
            Pent[4].Points.Add(new Point(1, 2));
            Pent[4].Points.Add(new Point(-3, 2));
            Pent[4].Points.Add(new Point(-3, 1));
            Pent[4].Points.Add(new Point(0, 1));
            //L pentomino not rotated, mirrored
            Pent[5] = new Pentomino("L", 5, colors[1]);
            Pent[5].Cells.Add(new Point(0, 0));
            Pent[5].Cells.Add(new Point(0, 1));
            Pent[5].Cells.Add(new Point(0, 2));
            Pent[5].Cells.Add(new Point(0, 3));
            Pent[5].Cells.Add(new Point(-1, 3));
            Pent[5].Points.Add(new Point(0, 0));
            Pent[5].Points.Add(new Point(1, 0));
            Pent[5].Points.Add(new Point(1, 4));
            Pent[5].Points.Add(new Point(-1, 4));
            Pent[5].Points.Add(new Point(-1, 3));
            Pent[5].Points.Add(new Point(0, 3));
            //L pentomino 90° rotated, mirrored
            Pent[6] = new Pentomino("L", 6, colors[1]);
            Pent[6].Cells.Add(new Point(0, 0));
            Pent[6].Cells.Add(new Point(0, 1));
            Pent[6].Cells.Add(new Point(1, 1));
            Pent[6].Cells.Add(new Point(2, 1));
            Pent[6].Cells.Add(new Point(3, 1));
            Pent[6].Points.Add(new Point(0, 0));
            Pent[6].Points.Add(new Point(1, 0));
            Pent[6].Points.Add(new Point(1, 1));
            Pent[6].Points.Add(new Point(4, 1));
            Pent[6].Points.Add(new Point(4, 2));
            Pent[6].Points.Add(new Point(0, 2));
            //L pentomino 180° rotated, mirrored
            Pent[7] = new Pentomino("L", 7, colors[1]);
            Pent[7].Cells.Add(new Point(0, 0));
            Pent[7].Cells.Add(new Point(1, 0));
            Pent[7].Cells.Add(new Point(0, 1));
            Pent[7].Cells.Add(new Point(0, 2));
            Pent[7].Cells.Add(new Point(0, 3));
            Pent[7].Points.Add(new Point(0, 0));
            Pent[7].Points.Add(new Point(2, 0));
            Pent[7].Points.Add(new Point(2, 1));
            Pent[7].Points.Add(new Point(1, 1));
            Pent[7].Points.Add(new Point(1, 4));
            Pent[7].Points.Add(new Point(0, 4));
            //L pentomino 270° rotated, mirrored
            Pent[8] = new Pentomino("L", 8, colors[1]);
            Pent[8].Cells.Add(new Point(0, 0));
            Pent[8].Cells.Add(new Point(1, 0));
            Pent[8].Cells.Add(new Point(2, 0));
            Pent[8].Cells.Add(new Point(3, 0));
            Pent[8].Cells.Add(new Point(3, 1));
            Pent[8].Points.Add(new Point(0, 0));
            Pent[8].Points.Add(new Point(4, 0));
            Pent[8].Points.Add(new Point(4, 2));
            Pent[8].Points.Add(new Point(3, 2));
            Pent[8].Points.Add(new Point(3, 1));
            Pent[8].Points.Add(new Point(0, 1));
            //F pentomino not rotated, not mirrored
            Pent[9] = new Pentomino("F", 9, colors[2]);
            Pent[9].Cells.Add(new Point(0, 0));
            Pent[9].Cells.Add(new Point(1, 0));
            Pent[9].Cells.Add(new Point(0, 1));
            Pent[9].Cells.Add(new Point(0, 2));
            Pent[9].Cells.Add(new Point(-1, 1));
            Pent[9].Points.Add(new Point(0, 0));
            Pent[9].Points.Add(new Point(2, 0));
            Pent[9].Points.Add(new Point(2, 1));
            Pent[9].Points.Add(new Point(1, 1));
            Pent[9].Points.Add(new Point(1, 3));
            Pent[9].Points.Add(new Point(0, 3));
            Pent[9].Points.Add(new Point(0, 2));
            Pent[9].Points.Add(new Point(-1, 2));
            Pent[9].Points.Add(new Point(-1, 1));
            Pent[9].Points.Add(new Point(0, 1));
            //F pentomino 90° rotated, not mirrored
            Pent[10] = new Pentomino("F", 10, colors[2]);
            Pent[10].Cells.Add(new Point(0, 0));
            Pent[10].Cells.Add(new Point(0, 1));
            Pent[10].Cells.Add(new Point(1, 1));
            Pent[10].Cells.Add(new Point(1, 2));
            Pent[10].Cells.Add(new Point(-1, 1));
            Pent[10].Points.Add(new Point(0, 0));
            Pent[10].Points.Add(new Point(1, 0));
            Pent[10].Points.Add(new Point(1, 1));
            Pent[10].Points.Add(new Point(2, 1));
            Pent[10].Points.Add(new Point(2, 3));
            Pent[10].Points.Add(new Point(1, 3));
            Pent[10].Points.Add(new Point(1, 2));
            Pent[10].Points.Add(new Point(-1, 2));
            Pent[10].Points.Add(new Point(-1, 1));
            Pent[10].Points.Add(new Point(0, 1));
            //F pentomino 180° rotated, not mirrored
            Pent[11] = new Pentomino("F", 11, colors[2]);
            Pent[11].Cells.Add(new Point(0, 0));
            Pent[11].Cells.Add(new Point(0, 1));
            Pent[11].Cells.Add(new Point(0, 2));
            Pent[11].Cells.Add(new Point(1, 1));
            Pent[11].Cells.Add(new Point(-1, 2));
            Pent[11].Points.Add(new Point(0, 0));
            Pent[11].Points.Add(new Point(1, 0));
            Pent[11].Points.Add(new Point(1, 1));
            Pent[11].Points.Add(new Point(2, 1));
            Pent[11].Points.Add(new Point(2, 2));
            Pent[11].Points.Add(new Point(1, 2));
            Pent[11].Points.Add(new Point(1, 3));
            Pent[11].Points.Add(new Point(-1, 3));
            Pent[11].Points.Add(new Point(-1, 2));
            Pent[11].Points.Add(new Point(0, 2));
            //F pentomino 270° rotated, not mirrored
            Pent[12] = new Pentomino("F", 12, colors[2]);
            Pent[12].Cells.Add(new Point(0, 0));
            Pent[12].Cells.Add(new Point(0, 1));
            Pent[12].Cells.Add(new Point(1, 1));
            Pent[12].Cells.Add(new Point(2, 1));
            Pent[12].Cells.Add(new Point(1, 2));
            Pent[12].Points.Add(new Point(0, 0));
            Pent[12].Points.Add(new Point(1, 0));
            Pent[12].Points.Add(new Point(1, 1));
            Pent[12].Points.Add(new Point(3, 1));
            Pent[12].Points.Add(new Point(3, 2));
            Pent[12].Points.Add(new Point(2, 2));
            Pent[12].Points.Add(new Point(2, 3));
            Pent[12].Points.Add(new Point(1, 3));
            Pent[12].Points.Add(new Point(1, 2));
            Pent[12].Points.Add(new Point(0, 2));
            //F pentomino not rotated, mirrored
            Pent[13] = new Pentomino("F", 13, colors[2]);
            Pent[13].Cells.Add(new Point(0, 0));
            Pent[13].Cells.Add(new Point(1, 0));
            Pent[13].Cells.Add(new Point(1, 1));
            Pent[13].Cells.Add(new Point(2, 1));
            Pent[13].Cells.Add(new Point(1, 2));
            Pent[13].Points.Add(new Point(0, 0));
            Pent[13].Points.Add(new Point(2, 0));
            Pent[13].Points.Add(new Point(2, 1));
            Pent[13].Points.Add(new Point(3, 1));
            Pent[13].Points.Add(new Point(3, 2));
            Pent[13].Points.Add(new Point(2, 2));
            Pent[13].Points.Add(new Point(2, 3));
            Pent[13].Points.Add(new Point(1, 3));
            Pent[13].Points.Add(new Point(1, 1));
            Pent[13].Points.Add(new Point(0, 1));
            //F pentomino 90° rotated, mirrored
            Pent[14] = new Pentomino("F", 14, colors[2]);
            Pent[14].Cells.Add(new Point(0, 0));
            Pent[14].Cells.Add(new Point(0, 1));
            Pent[14].Cells.Add(new Point(-1, 1));
            Pent[14].Cells.Add(new Point(-2, 1));
            Pent[14].Cells.Add(new Point(-1, 2));
            Pent[14].Points.Add(new Point(0, 0));
            Pent[14].Points.Add(new Point(1, 0));
            Pent[14].Points.Add(new Point(1, 2));
            Pent[14].Points.Add(new Point(0, 2));
            Pent[14].Points.Add(new Point(0, 3));
            Pent[14].Points.Add(new Point(-1, 3));
            Pent[14].Points.Add(new Point(-1, 2));
            Pent[14].Points.Add(new Point(-2, 2));
            Pent[14].Points.Add(new Point(-2, 1));
            Pent[14].Points.Add(new Point(0, 1));
            //F pentomino 180° rotated, mirrored
            Pent[15] = new Pentomino("F", 15, colors[2]);
            Pent[15].Cells.Add(new Point(0, 0));
            Pent[15].Cells.Add(new Point(0, 1));
            Pent[15].Cells.Add(new Point(0, 2));
            Pent[15].Cells.Add(new Point(1, 2));
            Pent[15].Cells.Add(new Point(-1, 1));
            Pent[15].Points.Add(new Point(0, 0));
            Pent[15].Points.Add(new Point(1, 0));
            Pent[15].Points.Add(new Point(1, 2));
            Pent[15].Points.Add(new Point(2, 2));
            Pent[15].Points.Add(new Point(2, 3));
            Pent[15].Points.Add(new Point(0, 3));
            Pent[15].Points.Add(new Point(0, 2));
            Pent[15].Points.Add(new Point(-1, 2));
            Pent[15].Points.Add(new Point(-1, 1));
            Pent[15].Points.Add(new Point(0, 1));
            //F pentomino 270° rotated, mirrored
            Pent[16] = new Pentomino("F", 16, colors[2]);
            Pent[16].Cells.Add(new Point(0, 0));
            Pent[16].Cells.Add(new Point(0, 1));
            Pent[16].Cells.Add(new Point(1, 1));
            Pent[16].Cells.Add(new Point(-1, 1));
            Pent[16].Cells.Add(new Point(-1, 2));
            Pent[16].Points.Add(new Point(0, 0));
            Pent[16].Points.Add(new Point(1, 0));
            Pent[16].Points.Add(new Point(1, 1));
            Pent[16].Points.Add(new Point(2, 1));
            Pent[16].Points.Add(new Point(2, 2));
            Pent[16].Points.Add(new Point(0, 2));
            Pent[16].Points.Add(new Point(0, 3));
            Pent[16].Points.Add(new Point(-1, 3));
            Pent[16].Points.Add(new Point(-1, 1));
            Pent[16].Points.Add(new Point(0, 1));
            //N pentomino not rotated, not mirrored
            Pent[17] = new Pentomino("N", 17, colors[3]);
            Pent[17].Cells.Add(new Point(0, 0));
            Pent[17].Cells.Add(new Point(0, 1));
            Pent[17].Cells.Add(new Point(0, 2));
            Pent[17].Cells.Add(new Point(-1, 2));
            Pent[17].Cells.Add(new Point(-1, 3));
            Pent[17].Points.Add(new Point(0, 0));
            Pent[17].Points.Add(new Point(1, 0));
            Pent[17].Points.Add(new Point(1, 3));
            Pent[17].Points.Add(new Point(0, 3));
            Pent[17].Points.Add(new Point(0, 4));
            Pent[17].Points.Add(new Point(-1, 4));
            Pent[17].Points.Add(new Point(-1, 2));
            Pent[17].Points.Add(new Point(0, 2));
            //N pentomino 90° rotated, not mirrored;
            Pent[18] = new Pentomino("N", 18, colors[3]);
            Pent[18].Cells.Add(new Point(0, 0));
            Pent[18].Cells.Add(new Point(1, 0));
            Pent[18].Cells.Add(new Point(1, 1));
            Pent[18].Cells.Add(new Point(2, 1));
            Pent[18].Cells.Add(new Point(3, 1));
            Pent[18].Points.Add(new Point(0, 0));
            Pent[18].Points.Add(new Point(2, 0));
            Pent[18].Points.Add(new Point(2, 1));
            Pent[18].Points.Add(new Point(4, 1));
            Pent[18].Points.Add(new Point(4, 2));
            Pent[18].Points.Add(new Point(1, 2));
            Pent[18].Points.Add(new Point(1, 1));
            Pent[18].Points.Add(new Point(0, 1));
            //N pentomino 180° rotated, not mirrored
            Pent[19] = new Pentomino("N", 19, colors[3]);
            Pent[19].Cells.Add(new Point(0, 0));
            Pent[19].Cells.Add(new Point(0, 1));
            Pent[19].Cells.Add(new Point(-1, 1));
            Pent[19].Cells.Add(new Point(-1, 2));
            Pent[19].Cells.Add(new Point(-1, 3));
            Pent[19].Points.Add(new Point(0, 0));
            Pent[19].Points.Add(new Point(1, 0));
            Pent[19].Points.Add(new Point(1, 2));
            Pent[19].Points.Add(new Point(0, 2));
            Pent[19].Points.Add(new Point(0, 4));
            Pent[19].Points.Add(new Point(-1, 4));
            Pent[19].Points.Add(new Point(-1, 1));
            Pent[19].Points.Add(new Point(0, 1));
            //N pentomino 270° rotated, not mirrored
            Pent[20] = new Pentomino("N", 20, colors[3]);
            Pent[20].Cells.Add(new Point(0, 0));
            Pent[20].Cells.Add(new Point(1, 0));
            Pent[20].Cells.Add(new Point(2, 0));
            Pent[20].Cells.Add(new Point(2, 1));
            Pent[20].Cells.Add(new Point(3, 1));
            Pent[20].Points.Add(new Point(0, 0));
            Pent[20].Points.Add(new Point(3, 0));
            Pent[20].Points.Add(new Point(3, 1));
            Pent[20].Points.Add(new Point(4, 1));
            Pent[20].Points.Add(new Point(4, 2));
            Pent[20].Points.Add(new Point(2, 2));
            Pent[20].Points.Add(new Point(2, 1));
            Pent[20].Points.Add(new Point(0, 1));
            //N pentomino not rotated, mirrored
            Pent[21] = new Pentomino("N", 21, colors[3]);
            Pent[21].Cells.Add(new Point(0, 0));
            Pent[21].Cells.Add(new Point(0, 1));
            Pent[21].Cells.Add(new Point(0, 2));
            Pent[21].Cells.Add(new Point(1, 2));
            Pent[21].Cells.Add(new Point(1, 3));
            Pent[21].Points.Add(new Point(0, 0));
            Pent[21].Points.Add(new Point(1, 0));
            Pent[21].Points.Add(new Point(1, 2));
            Pent[21].Points.Add(new Point(2, 2));
            Pent[21].Points.Add(new Point(2, 4));
            Pent[21].Points.Add(new Point(1, 4));
            Pent[21].Points.Add(new Point(1, 3));
            Pent[21].Points.Add(new Point(0, 3));
            //N pentomino 90° rotated, mirrored;
            Pent[22] = new Pentomino("N", 22, colors[3]);
            Pent[22].Cells.Add(new Point(0, 0));
            Pent[22].Cells.Add(new Point(1, 0));
            Pent[22].Cells.Add(new Point(2, 0));
            Pent[22].Cells.Add(new Point(0, 1));
            Pent[22].Cells.Add(new Point(-1, 1));
            Pent[22].Points.Add(new Point(0, 0));
            Pent[22].Points.Add(new Point(3, 0));
            Pent[22].Points.Add(new Point(3, 1));
            Pent[22].Points.Add(new Point(1, 1));
            Pent[22].Points.Add(new Point(1, 2));
            Pent[22].Points.Add(new Point(-1, 2));
            Pent[22].Points.Add(new Point(-1, 1));
            Pent[22].Points.Add(new Point(0, 1));
            //N pentomino 180° rotated, mirrored;
            Pent[23] = new Pentomino("N", 23, colors[3]);
            Pent[23].Cells.Add(new Point(0, 0));
            Pent[23].Cells.Add(new Point(0, 1));
            Pent[23].Cells.Add(new Point(1, 1));
            Pent[23].Cells.Add(new Point(1, 2));
            Pent[23].Cells.Add(new Point(1, 3));
            Pent[23].Points.Add(new Point(0, 0));
            Pent[23].Points.Add(new Point(1, 0));
            Pent[23].Points.Add(new Point(1, 1));
            Pent[23].Points.Add(new Point(2, 1));
            Pent[23].Points.Add(new Point(2, 4));
            Pent[23].Points.Add(new Point(1, 4));
            Pent[23].Points.Add(new Point(1, 2));
            Pent[23].Points.Add(new Point(0, 2));
            //N pentomino 270° rotated, mirrored
            Pent[24] = new Pentomino("N", 24, colors[3]);
            Pent[24].Cells.Add(new Point(0, 0));
            Pent[24].Cells.Add(new Point(1, 0));
            Pent[24].Cells.Add(new Point(0, 1));
            Pent[24].Cells.Add(new Point(-1, 1));
            Pent[24].Cells.Add(new Point(-2, 1));
            Pent[24].Points.Add(new Point(0, 0));
            Pent[24].Points.Add(new Point(2, 0));
            Pent[24].Points.Add(new Point(2, 1));
            Pent[24].Points.Add(new Point(1, 1));
            Pent[24].Points.Add(new Point(1, 2));
            Pent[24].Points.Add(new Point(-2, 2));
            Pent[24].Points.Add(new Point(-2, 1));
            Pent[24].Points.Add(new Point(0, 1));
            //P pentomino not rotated, not mirrored
            Pent[25] = new Pentomino("P", 25, colors[4]);
            Pent[25].Cells.Add(new Point(0, 0));
            Pent[25].Cells.Add(new Point(0, 1));
            Pent[25].Cells.Add(new Point(0, 2));
            Pent[25].Cells.Add(new Point(1, 0));
            Pent[25].Cells.Add(new Point(1, 1));
            Pent[25].Points.Add(new Point(0, 0));
            Pent[25].Points.Add(new Point(2, 0));
            Pent[25].Points.Add(new Point(2, 2));
            Pent[25].Points.Add(new Point(1, 2));
            Pent[25].Points.Add(new Point(1, 3));
            Pent[25].Points.Add(new Point(0, 3));
            //P pentomino 90° rotated, not mirrored
            Pent[26] = new Pentomino("P", 26, colors[4]);
            Pent[26].Cells.Add(new Point(0, 0));
            Pent[26].Cells.Add(new Point(1, 0));
            Pent[26].Cells.Add(new Point(2, 0));
            Pent[26].Cells.Add(new Point(1, 1));
            Pent[26].Cells.Add(new Point(2, 1));
            Pent[26].Points.Add(new Point(0, 0));
            Pent[26].Points.Add(new Point(3, 0));
            Pent[26].Points.Add(new Point(3, 2));
            Pent[26].Points.Add(new Point(1, 2));
            Pent[26].Points.Add(new Point(1, 1));
            Pent[26].Points.Add(new Point(0, 1));
            //P pentomino 180° rotated, not mirrored
            Pent[27] = new Pentomino("P", 27, colors[4]);
            Pent[27].Cells.Add(new Point(0, 0));
            Pent[27].Cells.Add(new Point(0, 1));
            Pent[27].Cells.Add(new Point(0, 2));
            Pent[27].Cells.Add(new Point(-1, 1));
            Pent[27].Cells.Add(new Point(-1, 2));
            Pent[27].Points.Add(new Point(0, 0));
            Pent[27].Points.Add(new Point(1, 0));
            Pent[27].Points.Add(new Point(1, 3));
            Pent[27].Points.Add(new Point(-1, 3));
            Pent[27].Points.Add(new Point(-1, 1));
            Pent[27].Points.Add(new Point(0, 1));
            //P pentomino 270° rotated, not mirrored
            Pent[28] = new Pentomino("P", 28, colors[4]);
            Pent[28].Cells.Add(new Point(0, 0));
            Pent[28].Cells.Add(new Point(1, 0));
            Pent[28].Cells.Add(new Point(0, 1));
            Pent[28].Cells.Add(new Point(1, 1));
            Pent[28].Cells.Add(new Point(2, 1));
            Pent[28].Points.Add(new Point(0, 0));
            Pent[28].Points.Add(new Point(2, 0));
            Pent[28].Points.Add(new Point(2, 1));
            Pent[28].Points.Add(new Point(3, 1));
            Pent[28].Points.Add(new Point(3, 2));
            Pent[28].Points.Add(new Point(0, 2));
            //P pentomino not rotated, mirrored
            Pent[29] = new Pentomino("P", 29, colors[4]);
            Pent[29].Cells.Add(new Point(0, 0));
            Pent[29].Cells.Add(new Point(1, 0));
            Pent[29].Cells.Add(new Point(0, 1));
            Pent[29].Cells.Add(new Point(1, 1));
            Pent[29].Cells.Add(new Point(1, 2));
            Pent[29].Points.Add(new Point(0, 0));
            Pent[29].Points.Add(new Point(2, 0));
            Pent[29].Points.Add(new Point(2, 3));
            Pent[29].Points.Add(new Point(1, 3));
            Pent[29].Points.Add(new Point(1, 2));
            Pent[29].Points.Add(new Point(0, 2));
            //P pentomino 90° rotated, mirrored;
            Pent[30] = new Pentomino("P", 30, colors[4]);
            Pent[30].Cells.Add(new Point(0, 0));
            Pent[30].Cells.Add(new Point(1, 0));
            Pent[30].Cells.Add(new Point(0, 1));
            Pent[30].Cells.Add(new Point(1, 1));
            Pent[30].Cells.Add(new Point(-1, 1));
            Pent[30].Points.Add(new Point(0, 0));
            Pent[30].Points.Add(new Point(2, 0));
            Pent[30].Points.Add(new Point(2, 2));
            Pent[30].Points.Add(new Point(-1, 2));
            Pent[30].Points.Add(new Point(-1, 1));
            Pent[30].Points.Add(new Point(0, 1));
            //P pentomino 180° rotated, mirrored
            Pent[31] = new Pentomino("P", 31, colors[4]);
            Pent[31].Cells.Add(new Point(0, 0));
            Pent[31].Cells.Add(new Point(0, 1));
            Pent[31].Cells.Add(new Point(0, 2));
            Pent[31].Cells.Add(new Point(1, 1));
            Pent[31].Cells.Add(new Point(1, 2));
            Pent[31].Points.Add(new Point(0, 0));
            Pent[31].Points.Add(new Point(1, 0));
            Pent[31].Points.Add(new Point(1, 1));
            Pent[31].Points.Add(new Point(2, 1));
            Pent[31].Points.Add(new Point(2, 3));
            Pent[31].Points.Add(new Point(0, 3));
            //P pentomino 270° rotated, mirrored
            Pent[32] = new Pentomino("P", 32, colors[4]);
            Pent[32].Cells.Add(new Point(0, 0));
            Pent[32].Cells.Add(new Point(1, 0));
            Pent[32].Cells.Add(new Point(2, 0));
            Pent[32].Cells.Add(new Point(0, 1));
            Pent[32].Cells.Add(new Point(1, 1));
            Pent[32].Points.Add(new Point(0, 0));
            Pent[32].Points.Add(new Point(3, 0));
            Pent[32].Points.Add(new Point(3, 1));
            Pent[32].Points.Add(new Point(2, 1));
            Pent[32].Points.Add(new Point(2, 2));
            Pent[32].Points.Add(new Point(0, 2));
            //Y pentomino not rotated, not mirrored
            Pent[33] = new Pentomino("Y", 33, colors[5]);
            Pent[33].Cells.Add(new Point(0, 0));
            Pent[33].Cells.Add(new Point(0, 1));
            Pent[33].Cells.Add(new Point(0, 2));
            Pent[33].Cells.Add(new Point(0, 3));
            Pent[33].Cells.Add(new Point(-1, 1));
            Pent[33].Points.Add(new Point(0, 0));
            Pent[33].Points.Add(new Point(1, 0));
            Pent[33].Points.Add(new Point(1, 4));
            Pent[33].Points.Add(new Point(0, 4));
            Pent[33].Points.Add(new Point(0, 2));
            Pent[33].Points.Add(new Point(-1, 2));
            Pent[33].Points.Add(new Point(-1, 1));
            Pent[33].Points.Add(new Point(0, 1));
            //Y pentomino 90° rotated, not mirrored
            Pent[34] = new Pentomino("Y", 34, colors[5]);
            Pent[34].Cells.Add(new Point(0, 0));
            Pent[34].Cells.Add(new Point(0, 1));
            Pent[34].Cells.Add(new Point(1, 1));
            Pent[34].Cells.Add(new Point(-1, 1));
            Pent[34].Cells.Add(new Point(-2, 1));
            Pent[34].Points.Add(new Point(0, 0));
            Pent[34].Points.Add(new Point(1, 0));
            Pent[34].Points.Add(new Point(1, 1));
            Pent[34].Points.Add(new Point(2, 1));
            Pent[34].Points.Add(new Point(2, 2));
            Pent[34].Points.Add(new Point(-2, 2));
            Pent[34].Points.Add(new Point(-2, 1));
            Pent[34].Points.Add(new Point(0, 1));
            //Y pentomino 180° rotated, not mirrored
            Pent[35] = new Pentomino("Y", 35, colors[5]);
            Pent[35].Cells.Add(new Point(0, 0));
            Pent[35].Cells.Add(new Point(0, 1));
            Pent[35].Cells.Add(new Point(0, 2));
            Pent[35].Cells.Add(new Point(0, 3));
            Pent[35].Cells.Add(new Point(1, 2));
            Pent[35].Points.Add(new Point(0, 0));
            Pent[35].Points.Add(new Point(1, 0));
            Pent[35].Points.Add(new Point(1, 2));
            Pent[35].Points.Add(new Point(2, 2));
            Pent[35].Points.Add(new Point(2, 3));
            Pent[35].Points.Add(new Point(1, 3));
            Pent[35].Points.Add(new Point(1, 4));
            Pent[35].Points.Add(new Point(0, 4));
            //Y pentomino 270° rotated, not mirrored
            Pent[36] = new Pentomino("Y", 36, colors[5]);
            Pent[36].Cells.Add(new Point(0, 0));
            Pent[36].Cells.Add(new Point(1, 0));
            Pent[36].Cells.Add(new Point(2, 0));
            Pent[36].Cells.Add(new Point(3, 0));
            Pent[36].Cells.Add(new Point(1, 1));
            Pent[36].Points.Add(new Point(0, 0));
            Pent[36].Points.Add(new Point(4, 0));
            Pent[36].Points.Add(new Point(4, 1));
            Pent[36].Points.Add(new Point(2, 1));
            Pent[36].Points.Add(new Point(2, 2));
            Pent[36].Points.Add(new Point(1, 2));
            Pent[36].Points.Add(new Point(1, 1));
            Pent[36].Points.Add(new Point(0, 1));
            //Y pentomino not rotated, mirrored
            Pent[37] = new Pentomino("Y", 37, colors[5]);
            Pent[37].Cells.Add(new Point(0, 0));
            Pent[37].Cells.Add(new Point(0, 1));
            Pent[37].Cells.Add(new Point(0, 2));
            Pent[37].Cells.Add(new Point(0, 3));
            Pent[37].Cells.Add(new Point(1, 1));
            Pent[37].Points.Add(new Point(0, 0));
            Pent[37].Points.Add(new Point(1, 0));
            Pent[37].Points.Add(new Point(1, 1));
            Pent[37].Points.Add(new Point(2, 1));
            Pent[37].Points.Add(new Point(2, 2));
            Pent[37].Points.Add(new Point(1, 2));
            Pent[37].Points.Add(new Point(1, 4));
            Pent[37].Points.Add(new Point(0, 4));
            //Y pentomino 90° rotated, mirrored;
            Pent[38] = new Pentomino("Y", 38, colors[5]);
            Pent[38].Cells.Add(new Point(0, 0));
            Pent[38].Cells.Add(new Point(1, 0));
            Pent[38].Cells.Add(new Point(2, 0));
            Pent[38].Cells.Add(new Point(3, 0));
            Pent[38].Cells.Add(new Point(2, 1));
            Pent[38].Points.Add(new Point(0, 0));
            Pent[38].Points.Add(new Point(4, 0));
            Pent[38].Points.Add(new Point(4, 1));
            Pent[38].Points.Add(new Point(3, 1));
            Pent[38].Points.Add(new Point(3, 2));
            Pent[38].Points.Add(new Point(2, 2));
            Pent[38].Points.Add(new Point(2, 1));
            Pent[38].Points.Add(new Point(0, 1));
            //Y pentomino 180° rotated, mirrored
            Pent[39] = new Pentomino("Y", 39, colors[5]);
            Pent[39].Cells.Add(new Point(0, 0));
            Pent[39].Cells.Add(new Point(0, 1));
            Pent[39].Cells.Add(new Point(0, 2));
            Pent[39].Cells.Add(new Point(0, 3));
            Pent[39].Cells.Add(new Point(-1, 2));
            Pent[39].Points.Add(new Point(0, 0));
            Pent[39].Points.Add(new Point(1, 0));
            Pent[39].Points.Add(new Point(1, 4));
            Pent[39].Points.Add(new Point(0, 4));
            Pent[39].Points.Add(new Point(0, 3));
            Pent[39].Points.Add(new Point(-1, 3));
            Pent[39].Points.Add(new Point(-1, 2));
            Pent[39].Points.Add(new Point(0, 2));
            //Y pentomino 270° rotated, mirrored
            Pent[40] = new Pentomino("Y", 40, colors[5]);
            Pent[40].Cells.Add(new Point(0, 0));
            Pent[40].Cells.Add(new Point(0, 1));
            Pent[40].Cells.Add(new Point(1, 1));
            Pent[40].Cells.Add(new Point(2, 1));
            Pent[40].Cells.Add(new Point(-1, 1));
            Pent[40].Points.Add(new Point(0, 0));
            Pent[40].Points.Add(new Point(1, 0));
            Pent[40].Points.Add(new Point(1, 1));
            Pent[40].Points.Add(new Point(3, 1));
            Pent[40].Points.Add(new Point(3, 2));
            Pent[40].Points.Add(new Point(-1, 2));
            Pent[40].Points.Add(new Point(-1, 1));
            Pent[40].Points.Add(new Point(0, 1));
            //T pentomino not rotated, mirror = identical;
            Pent[41] = new Pentomino("T", 41, colors[6]);
            Pent[41].Cells.Add(new Point(0, 0));
            Pent[41].Cells.Add(new Point(1, 0));
            Pent[41].Cells.Add(new Point(2, 0));
            Pent[41].Cells.Add(new Point(1, 1));
            Pent[41].Cells.Add(new Point(1, 2));
            Pent[41].Points.Add(new Point(0, 0));
            Pent[41].Points.Add(new Point(3, 0));
            Pent[41].Points.Add(new Point(3, 1));
            Pent[41].Points.Add(new Point(2, 1));
            Pent[41].Points.Add(new Point(2, 3));
            Pent[41].Points.Add(new Point(1, 3));
            Pent[41].Points.Add(new Point(1, 1));
            Pent[41].Points.Add(new Point(0, 1));
            //T pentomino 90° rotated, mirror = identical;
            Pent[42] = new Pentomino("T", 42, colors[6]);
            Pent[42].Cells.Add(new Point(0, 0));
            Pent[42].Cells.Add(new Point(0, 1));
            Pent[42].Cells.Add(new Point(0, 2));
            Pent[42].Cells.Add(new Point(1, 1));
            Pent[42].Cells.Add(new Point(2, 1));
            Pent[42].Points.Add(new Point(0, 0));
            Pent[42].Points.Add(new Point(1, 0));
            Pent[42].Points.Add(new Point(1, 1));
            Pent[42].Points.Add(new Point(3, 1));
            Pent[42].Points.Add(new Point(3, 2));
            Pent[42].Points.Add(new Point(1, 2));
            Pent[42].Points.Add(new Point(1, 3));
            Pent[42].Points.Add(new Point(0, 3));
            //T pentomino 180° rotated, mirror = identical;
            Pent[43] = new Pentomino("T", 43, colors[6]);
            Pent[43].Cells.Add(new Point(0, 0));
            Pent[43].Cells.Add(new Point(0, 1));
            Pent[43].Cells.Add(new Point(0, 2));
            Pent[43].Cells.Add(new Point(1, 2));
            Pent[43].Cells.Add(new Point(-1, 2));
            Pent[43].Points.Add(new Point(0, 0));
            Pent[43].Points.Add(new Point(1, 0));
            Pent[43].Points.Add(new Point(1, 2));
            Pent[43].Points.Add(new Point(2, 2));
            Pent[43].Points.Add(new Point(2, 3));
            Pent[43].Points.Add(new Point(-1, 3));
            Pent[43].Points.Add(new Point(-1, 2));
            Pent[43].Points.Add(new Point(0, 2));
            //T pentomino 270° rotated, mirror = identical;
            Pent[44] = new Pentomino("T", 44, colors[6]);
            Pent[44].Cells.Add(new Point(0, 0));
            Pent[44].Cells.Add(new Point(0, 1));
            Pent[44].Cells.Add(new Point(0, 2));
            Pent[44].Cells.Add(new Point(-1, 1));
            Pent[44].Cells.Add(new Point(-2, 1));
            Pent[44].Points.Add(new Point(0, 0));
            Pent[44].Points.Add(new Point(1, 0));
            Pent[44].Points.Add(new Point(1, 3));
            Pent[44].Points.Add(new Point(0, 3));
            Pent[44].Points.Add(new Point(0, 2));
            Pent[44].Points.Add(new Point(-2, 2));
            Pent[44].Points.Add(new Point(-2, 1));
            Pent[44].Points.Add(new Point(0, 1));
            //U pentomino not rotated, mirror = identical;
            Pent[45] = new Pentomino("U", 45, colors[7]);
            Pent[45].Cells.Add(new Point(0, 0));
            Pent[45].Cells.Add(new Point(0, 1));
            Pent[45].Cells.Add(new Point(1, 1));
            Pent[45].Cells.Add(new Point(2, 1));
            Pent[45].Cells.Add(new Point(2, 0));
            Pent[45].Points.Add(new Point(0, 0));
            Pent[45].Points.Add(new Point(1, 0));
            Pent[45].Points.Add(new Point(1, 1));
            Pent[45].Points.Add(new Point(2, 1));
            Pent[45].Points.Add(new Point(2, 0));
            Pent[45].Points.Add(new Point(3, 0));
            Pent[45].Points.Add(new Point(3, 2));
            Pent[45].Points.Add(new Point(0, 2));
            //U pentomino 90° rotated, mirror = identical;
            Pent[46] = new Pentomino("U", 46, colors[7]);
            Pent[46].Cells.Add(new Point(0, 0));
            Pent[46].Cells.Add(new Point(1, 0));
            Pent[46].Cells.Add(new Point(0, 1));
            Pent[46].Cells.Add(new Point(0, 2));
            Pent[46].Cells.Add(new Point(1, 2));
            Pent[46].Points.Add(new Point(0, 0));
            Pent[46].Points.Add(new Point(2, 0));
            Pent[46].Points.Add(new Point(2, 1));
            Pent[46].Points.Add(new Point(1, 1));
            Pent[46].Points.Add(new Point(1, 2));
            Pent[46].Points.Add(new Point(2, 2));
            Pent[46].Points.Add(new Point(2, 3));
            Pent[46].Points.Add(new Point(0, 3));
            //U pentomino 180° rotated, mirror = identical;
            Pent[47] = new Pentomino("U", 47, colors[7]);
            Pent[47].Cells.Add(new Point(0, 0));
            Pent[47].Cells.Add(new Point(1, 0));
            Pent[47].Cells.Add(new Point(2, 0));
            Pent[47].Cells.Add(new Point(0, 1));
            Pent[47].Cells.Add(new Point(2, 1));
            Pent[47].Points.Add(new Point(0, 0));
            Pent[47].Points.Add(new Point(3, 0));
            Pent[47].Points.Add(new Point(3, 2));
            Pent[47].Points.Add(new Point(2, 2));
            Pent[47].Points.Add(new Point(2, 1));
            Pent[47].Points.Add(new Point(1, 1));
            Pent[47].Points.Add(new Point(1, 2));
            Pent[47].Points.Add(new Point(0, 2));
            //U pentomino 270° rotated, mirror = identical;
            Pent[48] = new Pentomino("U", 48, colors[7]);
            Pent[48].Cells.Add(new Point(0, 0));
            Pent[48].Cells.Add(new Point(1, 0));
            Pent[48].Cells.Add(new Point(1, 1));
            Pent[48].Cells.Add(new Point(1, 2));
            Pent[48].Cells.Add(new Point(0, 2));
            Pent[48].Points.Add(new Point(0, 0));
            Pent[48].Points.Add(new Point(2, 0));
            Pent[48].Points.Add(new Point(2, 3));
            Pent[48].Points.Add(new Point(0, 3));
            Pent[48].Points.Add(new Point(0, 2));
            Pent[48].Points.Add(new Point(1, 2));
            Pent[48].Points.Add(new Point(1, 1));
            Pent[48].Points.Add(new Point(0, 1));
            //V pentomino not rotated, mirror = identical;
            Pent[49] = new Pentomino("V", 49, colors[8]);
            Pent[49].Cells.Add(new Point(0, 0));
            Pent[49].Cells.Add(new Point(0, 1));
            Pent[49].Cells.Add(new Point(0, 2));
            Pent[49].Cells.Add(new Point(1, 2));
            Pent[49].Cells.Add(new Point(2, 2));
            Pent[49].Points.Add(new Point(0, 0));
            Pent[49].Points.Add(new Point(1, 0));
            Pent[49].Points.Add(new Point(1, 2));
            Pent[49].Points.Add(new Point(3, 2));
            Pent[49].Points.Add(new Point(3, 3));
            Pent[49].Points.Add(new Point(0, 3));
            //V pentomino 90° rotated, mirror = identical;
            Pent[50] = new Pentomino("V", 50, colors[8]);
            Pent[50].Cells.Add(new Point(0, 0));
            Pent[50].Cells.Add(new Point(0, 1));
            Pent[50].Cells.Add(new Point(0, 2));
            Pent[50].Cells.Add(new Point(-1, 2));
            Pent[50].Cells.Add(new Point(-2, 2));
            Pent[50].Points.Add(new Point(0, 0));
            Pent[50].Points.Add(new Point(1, 0));
            Pent[50].Points.Add(new Point(1, 3));
            Pent[50].Points.Add(new Point(-2, 3));
            Pent[50].Points.Add(new Point(-2, 2));
            Pent[50].Points.Add(new Point(0, 2));
            //V pentomino 180° rotated, mirror = identical;
            Pent[51] = new Pentomino("V", 51, colors[8]);
            Pent[51].Cells.Add(new Point(0, 0));
            Pent[51].Cells.Add(new Point(1, 0));
            Pent[51].Cells.Add(new Point(2, 0));
            Pent[51].Cells.Add(new Point(2, 1));
            Pent[51].Cells.Add(new Point(2, 2));
            Pent[51].Points.Add(new Point(0, 0));
            Pent[51].Points.Add(new Point(3, 0));
            Pent[51].Points.Add(new Point(3, 3));
            Pent[51].Points.Add(new Point(2, 3));
            Pent[51].Points.Add(new Point(2, 1));
            Pent[51].Points.Add(new Point(0, 1));
            //V pentomino 270° rotated, mirror = identical;
            Pent[52] = new Pentomino("V", 52, colors[8]);
            Pent[52].Cells.Add(new Point(0, 0));
            Pent[52].Cells.Add(new Point(0, 1));
            Pent[52].Cells.Add(new Point(0, 2));
            Pent[52].Cells.Add(new Point(1, 0));
            Pent[52].Cells.Add(new Point(2, 0));
            Pent[52].Points.Add(new Point(0, 0));
            Pent[52].Points.Add(new Point(3, 0));
            Pent[52].Points.Add(new Point(3, 1));
            Pent[52].Points.Add(new Point(1, 1));
            Pent[52].Points.Add(new Point(1, 3));
            Pent[52].Points.Add(new Point(0, 3));
            //W pentomino not rotated, mirror = identical;
            Pent[53] = new Pentomino("W", 53, colors[9]);
            Pent[53].Cells.Add(new Point(0, 0));
            Pent[53].Cells.Add(new Point(0, 1));
            Pent[53].Cells.Add(new Point(1, 1));
            Pent[53].Cells.Add(new Point(1, 2));
            Pent[53].Cells.Add(new Point(2, 2));
            Pent[53].Points.Add(new Point(0, 0));
            Pent[53].Points.Add(new Point(1, 0));
            Pent[53].Points.Add(new Point(1, 1));
            Pent[53].Points.Add(new Point(2, 1));
            Pent[53].Points.Add(new Point(2, 2));
            Pent[53].Points.Add(new Point(3, 2));
            Pent[53].Points.Add(new Point(3, 3));
            Pent[53].Points.Add(new Point(1, 3));
            Pent[53].Points.Add(new Point(1, 2));
            Pent[53].Points.Add(new Point(0, 2));
            //W pentomino 90° rotated, mirror = identical;
            Pent[54] = new Pentomino("W", 54, colors[9]);
            Pent[54].Cells.Add(new Point(0, 0));
            Pent[54].Cells.Add(new Point(0, 1));
            Pent[54].Cells.Add(new Point(-1, 1));
            Pent[54].Cells.Add(new Point(-1, 2));
            Pent[54].Cells.Add(new Point(-2, 2));
            Pent[54].Points.Add(new Point(0, 0));
            Pent[54].Points.Add(new Point(1, 0));
            Pent[54].Points.Add(new Point(1, 2));
            Pent[54].Points.Add(new Point(0, 2));
            Pent[54].Points.Add(new Point(0, 3));
            Pent[54].Points.Add(new Point(-2, 3));
            Pent[54].Points.Add(new Point(-2, 2));
            Pent[54].Points.Add(new Point(-1, 2));
            Pent[54].Points.Add(new Point(-1, 1));
            Pent[54].Points.Add(new Point(0, 1));
            //W pentomino 180° rotated, mirror = identical;
            Pent[55] = new Pentomino("W", 55, colors[9]);
            Pent[55].Cells.Add(new Point(0, 0));
            Pent[55].Cells.Add(new Point(1, 0));
            Pent[55].Cells.Add(new Point(1, 1));
            Pent[55].Cells.Add(new Point(2, 1));
            Pent[55].Cells.Add(new Point(2, 2));
            Pent[55].Points.Add(new Point(0, 0));
            Pent[55].Points.Add(new Point(2, 0));
            Pent[55].Points.Add(new Point(2, 1));
            Pent[55].Points.Add(new Point(3, 1));
            Pent[55].Points.Add(new Point(3, 3));
            Pent[55].Points.Add(new Point(2, 3));
            Pent[55].Points.Add(new Point(2, 2));
            Pent[55].Points.Add(new Point(1, 2));
            Pent[55].Points.Add(new Point(1, 1));
            Pent[55].Points.Add(new Point(0, 1));
            //W pentomino 270° rotated, mirror = identical;
            Pent[56] = new Pentomino("W", 56, colors[9]);
            Pent[56].Cells.Add(new Point(0, 0));
            Pent[56].Cells.Add(new Point(1, 0));
            Pent[56].Cells.Add(new Point(0, 1));
            Pent[56].Cells.Add(new Point(-1, 1));
            Pent[56].Cells.Add(new Point(-1, 2));
            Pent[56].Points.Add(new Point(0, 0));
            Pent[56].Points.Add(new Point(2, 0));
            Pent[56].Points.Add(new Point(2, 1));
            Pent[56].Points.Add(new Point(1, 1));
            Pent[56].Points.Add(new Point(1, 2));
            Pent[56].Points.Add(new Point(0, 2));
            Pent[56].Points.Add(new Point(0, 3));
            Pent[56].Points.Add(new Point(-1, 3));
            Pent[56].Points.Add(new Point(-1, 1));
            Pent[56].Points.Add(new Point(0, 1));
            //Z pentomino not rotated, not mirrored, 180° rotated = identical;
            Pent[57] = new Pentomino("Z", 57, colors[10]);
            Pent[57].Cells.Add(new Point(0, 0));
            Pent[57].Cells.Add(new Point(1, 0));
            Pent[57].Cells.Add(new Point(1, 1));
            Pent[57].Cells.Add(new Point(1, 2));
            Pent[57].Cells.Add(new Point(2, 2));
            Pent[57].Points.Add(new Point(0, 0));
            Pent[57].Points.Add(new Point(2, 0));
            Pent[57].Points.Add(new Point(2, 2));
            Pent[57].Points.Add(new Point(3, 2));
            Pent[57].Points.Add(new Point(3, 3));
            Pent[57].Points.Add(new Point(1, 3));
            Pent[57].Points.Add(new Point(1, 1));
            Pent[57].Points.Add(new Point(0, 1));
            //Z pentomino 90° rotated, not mirrored, 270° rotated = identical;
            Pent[58] = new Pentomino("Z", 58, colors[10]);
            Pent[58].Cells.Add(new Point(0, 0));
            Pent[58].Cells.Add(new Point(0, 1));
            Pent[58].Cells.Add(new Point(-1, 1));
            Pent[58].Cells.Add(new Point(-2, 1));
            Pent[58].Cells.Add(new Point(-2, 2));
            Pent[58].Points.Add(new Point(0, 0));
            Pent[58].Points.Add(new Point(1, 0));
            Pent[58].Points.Add(new Point(1, 2));
            Pent[58].Points.Add(new Point(-1, 2));
            Pent[58].Points.Add(new Point(-1, 3));
            Pent[58].Points.Add(new Point(-2, 3));
            Pent[58].Points.Add(new Point(-2, 1));
            Pent[58].Points.Add(new Point(0, 1));
            //Z pentomino not rotated, mirrored, 180° rotated = identical;
            Pent[59] = new Pentomino("Z", 59, colors[10]);
            Pent[59].Cells.Add(new Point(0, 0));
            Pent[59].Cells.Add(new Point(1, 0));
            Pent[59].Cells.Add(new Point(0, 1));
            Pent[59].Cells.Add(new Point(0, 2));
            Pent[59].Cells.Add(new Point(-1, 2));
            Pent[59].Points.Add(new Point(0, 0));
            Pent[59].Points.Add(new Point(2, 0));
            Pent[59].Points.Add(new Point(2, 1));
            Pent[59].Points.Add(new Point(1, 1));
            Pent[59].Points.Add(new Point(1, 3));
            Pent[59].Points.Add(new Point(-1, 3));
            Pent[59].Points.Add(new Point(-1, 2));
            Pent[59].Points.Add(new Point(0, 2));
            //Z pentomino 90° rotated, mirrored, 270° rotated = identical;
            Pent[60] = new Pentomino("Z", 60, colors[10]);
            Pent[60].Cells.Add(new Point(0, 0));
            Pent[60].Cells.Add(new Point(0, 1));
            Pent[60].Cells.Add(new Point(1, 1));
            Pent[60].Cells.Add(new Point(2, 1));
            Pent[60].Cells.Add(new Point(2, 2));
            Pent[60].Points.Add(new Point(0, 0));
            Pent[60].Points.Add(new Point(1, 0));
            Pent[60].Points.Add(new Point(1, 1));
            Pent[60].Points.Add(new Point(3, 1));
            Pent[60].Points.Add(new Point(3, 3));
            Pent[60].Points.Add(new Point(2, 3));
            Pent[60].Points.Add(new Point(2, 2));
            Pent[60].Points.Add(new Point(0, 2));
            //I pentomino not rotated, mirror = identical, 180° rotated = identical;
            Pent[61] = new Pentomino("I", 61, colors[11]);
            Pent[61].Cells.Add(new Point(0, 0));
            Pent[61].Cells.Add(new Point(0, 1));
            Pent[61].Cells.Add(new Point(0, 2));
            Pent[61].Cells.Add(new Point(0, 3));
            Pent[61].Cells.Add(new Point(0, 4));
            Pent[61].Points.Add(new Point(0, 0));
            Pent[61].Points.Add(new Point(1, 0));
            Pent[61].Points.Add(new Point(1, 5));
            Pent[61].Points.Add(new Point(0, 5));
            //I pentomino 90° rotated, mirror = identical, 270° rotated = identical;
            Pent[62] = new Pentomino("I", 62, colors[11]);
            Pent[62].Cells.Add(new Point(0, 0));
            Pent[62].Cells.Add(new Point(1, 0));
            Pent[62].Cells.Add(new Point(2, 0));
            Pent[62].Cells.Add(new Point(3, 0));
            Pent[62].Cells.Add(new Point(4, 0));
            Pent[62].Points.Add(new Point(0, 0));
            Pent[62].Points.Add(new Point(5, 0));
            Pent[62].Points.Add(new Point(5, 1));
            Pent[62].Points.Add(new Point(0, 1));
            //X pentomino not rotated, mirror = identical, all rotations are identical;
            Pent[63] = new Pentomino("X", 63, colors[12]);
            Pent[63].Cells.Add(new Point(0, 0));
            Pent[63].Cells.Add(new Point(0, 1));
            Pent[63].Cells.Add(new Point(0, 2));
            Pent[63].Cells.Add(new Point(-1, 1));
            Pent[63].Cells.Add(new Point(1, 1));
            Pent[63].Points.Add(new Point(0, 0));
            Pent[63].Points.Add(new Point(1, 0));
            Pent[63].Points.Add(new Point(1, 1));
            Pent[63].Points.Add(new Point(2, 1));
            Pent[63].Points.Add(new Point(2, 2));
            Pent[63].Points.Add(new Point(1, 2));
            Pent[63].Points.Add(new Point(1, 3));
            Pent[63].Points.Add(new Point(0, 3));
            Pent[63].Points.Add(new Point(0, 2));
            Pent[63].Points.Add(new Point(-1, 2));
            Pent[63].Points.Add(new Point(-1, 1));
            Pent[63].Points.Add(new Point(0, 1));
        }

        #endregion

        #region "Window1 Events"

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            CreatePentominoes();
            AppLoaded = true;
            Width = 60 * ColNum;
            Height = 60 * RowNum;
            Init();
        }

        private void Window1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppLoaded)
            {
                Init();
            }
        }

        private void Window1_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        #region "Menu Events"

        private void MenuAnim_Click(object sender, RoutedEventArgs e)
        {
            Animated = MenuAnim.IsChecked;
            MenuFastest.IsEnabled = Animated;
            Menu25FPS.IsEnabled = Animated;
            Menu10FPS.IsEnabled = Animated;
            Menu2FPS.IsEnabled = Animated;
            MenuSingleStep.IsEnabled = Animated;
            MenuStep.IsEnabled = Animated;
        }

        private void MenuStart_Click(object sender, RoutedEventArgs e)
        {
            //Set menu item availability
            MenuStart.IsEnabled = false;
            MenuSize.IsEnabled = false;
            MenuStop.IsEnabled = true;
            if (SingleStep)
            {
                MenuStep.IsEnabled = true;
            }
            Init();
            ResizeMode = ResizeMode.NoResize;
            AppRunning = true;
            //Start the search
            if (Solve(0))
            {
                //FOUND A SOLUTION!!
                MenuStop_Click(sender, e);
                return;
            }
        }

        private void MenuStop_Click(object sender, RoutedEventArgs e)
        {
            AppRunning = false;
            ResizeMode = ResizeMode.CanResize;
            //Set menu item availability
            MenuStart.IsEnabled = true;
            MenuSize.IsEnabled = true;
            MenuStop.IsEnabled = false;
            MenuStep.IsEnabled = false;
        }

        private void Menu6x10_Click(object sender, RoutedEventArgs e)
        {
            ColNum = 10;
            RowNum = 6;
            Width = 60 * ColNum;
            Height = 60 * RowNum;
            Menu5x12.IsChecked = false;
            Menu4x15.IsChecked = false;
            Menu3x20.IsChecked = false;
            Init();
        }

        private void Menu5x12_Click(object sender, RoutedEventArgs e)
        {
            ColNum = 12;
            RowNum = 5;
            Width = 60 * ColNum;
            Height = 60 * RowNum;
            Menu6x10.IsChecked = false;
            Menu4x15.IsChecked = false;
            Menu3x20.IsChecked = false;
            Init();
        }

        private void Menu4x15_Click(object sender, RoutedEventArgs e)
        {
            ColNum = 15;
            RowNum = 4;
            Width = 60 * ColNum;
            Height = 60 * RowNum;
            Menu6x10.IsChecked = false;
            Menu5x12.IsChecked = false;
            Menu3x20.IsChecked = false;
            Init();
        }

        private void Menu3x20_Click(object sender, RoutedEventArgs e)
        {
            ColNum = 20;
            RowNum = 3;
            Width = 60 * ColNum;
            Height = 60 * RowNum;
            Menu6x10.IsChecked = false;
            Menu5x12.IsChecked = false;
            Menu4x15.IsChecked = false;
            Init();
        }

        private void MenuFastest_Click(object sender, RoutedEventArgs e)
        {
            SleepTime = 0;
            Menu25FPS.IsChecked = false;
            Menu10FPS.IsChecked = false;
            Menu2FPS.IsChecked = false;
            MenuSingleStep.IsChecked = false;
            SingleStep = false;
            Nextstep = true;
            MenuStep.IsEnabled = false;
        }

        private void Menu25FPS_Click(object sender, RoutedEventArgs e)
        {
            SleepTime = 20;
            MenuFastest.IsChecked = false;
            Menu10FPS.IsChecked = false;
            Menu2FPS.IsChecked = false;
            MenuSingleStep.IsChecked = false;
            SingleStep = false;
            Nextstep = true;
            MenuStep.IsEnabled = false;
        }

        private void Menu10FPS_Click(object sender, RoutedEventArgs e)
        {
            SleepTime = 50;
            MenuFastest.IsChecked = false;
            Menu25FPS.IsChecked = false;
            Menu2FPS.IsChecked = false;
            MenuSingleStep.IsChecked = false;
            SingleStep = false;
            Nextstep = true;
            MenuStep.IsEnabled = false;
        }

        private void Menu2FPS_Click(object sender, RoutedEventArgs e)
        {
            SleepTime = 250;
            MenuFastest.IsChecked = false;
            Menu25FPS.IsChecked = false;
            Menu10FPS.IsChecked = false;
            MenuSingleStep.IsChecked = false;
            SingleStep = false;
            Nextstep = true;
            MenuStep.IsEnabled = false;
        }

        private void MenuSingleStep_Click(object sender, RoutedEventArgs e)
        {
            MenuFastest.IsChecked = false;
            Menu25FPS.IsChecked = false;
            Menu10FPS.IsChecked = false;
            Menu2FPS.IsChecked = false;
            SingleStep = true;
            if (AppRunning)
            {
                MenuStep.IsEnabled = true;
            }
        }

        private void MenuStep_Click(object sender, RoutedEventArgs e)
        {
            Nextstep = true;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        #region "Business Methods"
        private bool CheckFit(int index, int PentIndex)
        {
            int X = index % ColNum;
            int Y = (int)Math.Truncate((double)index / ColNum);
            //Check if the Pentomino can be placed at the index location
            foreach (Point p in Pent[PentIndex].Cells)
            {
                //check if the pentomino fits inside the box
                if (X + p.X < 0 | Y + p.Y < 0 | X + p.X >= ColNum | Y + p.Y >= RowNum) return false;
                //check if all cells are available
                if (!Cells[(int)(ColNum * (Y + p.Y) + X + p.X)]) return false;
            }
            return true;
        }

        private bool CheckFreeSize(int index, int PentIndex)
        {
            //Check if the free areas created by placing this pentomino are multiples of 5
            int X = index % ColNum;
            int Y = (int)Math.Truncate((double)index / ColNum);
            int groupIndex = 0;
            bool result = true;
            bool FitIsOK = true;
            //Step1: Simulate fitting the pentomino
            foreach (Point p in Pent[PentIndex].Cells)
            {
                Cells[(int)(ColNum * (Y + p.Y) + X + p.X)] = false;
            }
            for (int I = 1; I <= 63; I++)
            {
                if (Pent[I].Type == Pent[PentIndex].Type)
                {
                    Pent[I].Used = true;
                }
            }
            //Step2: Devide the free cells into groups
            for (int I = 0; I <= 59; I++)
            {
                CelGroup[I] = 0;
            }
            GroupNumber = 0;
            for (int I = 0; I <= 59; I++)
            {
                if (Cells[I] & CelGroup[I] == 0)
                {
                    GroupNumber += 1;
                    GroupCells(I, GroupNumber);
                }
            }
            //Step3: Check the size of the free groups
            int groupsize = 0;
            for (int I = 1; I <= GroupNumber; I++)
            {
                groupsize = 0;
                for (int J = 0; J <= 59; J++)
                {
                    if (CelGroup[J] == I) groupsize += 1;
                }
                if (groupsize % 5 != 0)
                {
                    result = false;
                    break;
                }
                if (groupsize == 5)
                {
                    //There must be a free pentomino that can be placed in this group
                    groupIndex = NextFreeIndex(I);
                    FitIsOK = false;
                    for (int J = 1; J <= 63; J++)
                    {
                        if (Pent[J].Used == false)
                        {
                            if (CheckFit(groupIndex, J))
                            {
                                FitIsOK = true;
                            }
                        }
                    }
                    if (!FitIsOK)
                    {
                        result = false;
                        break;
                    }
                }
            }
            //Step4: Remove the simulated pentomino fitting
            foreach (Point p in Pent[PentIndex].Cells)
            {
                Cells[(int)(ColNum * (Y + p.Y) + X + p.X)] = true;
            }
            for (int I = 1; I <= 63; I++)
            {
                if (Pent[I].Type == Pent[PentIndex].Type)
                {
                    Pent[I].Used = false;
                }
            }
            return result;
        }

        private void GroupCells(int celindex, int GroupNr)
        {
            //Recursive method for placing all adjacent free cells into one group.
            if (Cells[celindex] & CelGroup[celindex] == 0)
            {
                CelGroup[celindex] = GroupNr;
                if (celindex % ColNum > 0) GroupCells(celindex - 1, GroupNr);
                if (celindex % ColNum < ColNum - 1) GroupCells(celindex + 1, GroupNr);
                if (celindex > ColNum - 1) GroupCells(celindex - ColNum, GroupNr);
                if (celindex < ColNum * (RowNum - 1)) GroupCells(celindex + ColNum, GroupNr);
            }
        }

        private int NextFreeIndex(int gr)
        {
            //Find the index of the next free cell
            //return -1 if all cells are occupied (solution found)
            if (gr == 0)
            {
                for (int I = 0; I <= 59; I++)
                {
                    if (Cells[I]) return I;
                }
            }
            else
            {
                for (int I = 0; I <= 59; I++)
                {
                    if (Cells[I] & CelGroup[I] == gr) return I;
                }
            }
            return -1;
        }

        private void PlacePentomino(int Index, int PentIndex)
        {
            double X = Index % ColNum;
            double Y = Math.Truncate((double)Index / ColNum);
            //Mark all pentominoes of the same type as used
            for (int I = 1; I <= 63; I++)
            {
                if (Pent[I].Type == Pent[PentIndex].Type)
                {
                    Pent[I].Used = true;
                }
            }
            //Draw the Pentomino
            Nextstep = false;
            if (Animated)
            {
                renderDelegate = new RenderDelegate(DrawPentomino);
                Dispatcher.Invoke(renderDelegate, DispatcherPriority.ApplicationIdle, PentIndex, X, Y);
                if (!SingleStep)
                {
                    Thread.Sleep(SleepTime);
                }
                else
                {
                    while (Nextstep == false)
                    {
                        waitDelegate = new WaitDelegate(WaitForStep);
                        Dispatcher.Invoke(waitDelegate, DispatcherPriority.ApplicationIdle);
                    };
                }
            }
            else
            {
                DrawPentomino(PentIndex, X, Y);
            }
            //Set all used Cells as occupied
            foreach (Point p in Pent[PentIndex].Cells)
            {
                Cells[(int)(ColNum * (Y + p.Y) + X + p.X)] = false;
            }
        }

        private void DrawPentomino(int index, double X, double Y)
        {
            Pent[index].Draw(Canvas1, X * CelWidth, Y * CelHeight);
        }

        private void RemovePentomino(int Index, int PentIndex)
        {
            double X = Index % ColNum;
            double Y = Math.Truncate((double)Index / ColNum);
            //Mark all pentominoes of the same type as not used
            for (int I = 1; I <= 63; I++)
            {
                if (Pent[I].Type == Pent[PentIndex].Type)
                {
                    Pent[I].Used = false;
                }
            }
            //Remove the Pentomino from the Canvas.Children collection
            Nextstep = false;
            if (Animated)
            {
                renderDelegate = new RenderDelegate(ErasePentomino);
                Dispatcher.Invoke(renderDelegate, DispatcherPriority.ApplicationIdle, PentIndex, X, Y);
                if (!SingleStep)
                {
                    Thread.Sleep(SleepTime);
                }
                else
                {
                    while (Nextstep == false)
                    {
                        Dispatcher.Invoke(waitDelegate, DispatcherPriority.ApplicationIdle);
                    };
                }
            }
            else
            {
                ErasePentomino(PentIndex, X, Y);
            }
            //Set all freed cells as not occupied
            foreach (Point p in Pent[PentIndex].Cells)
            {
                Cells[(int)(ColNum * (Y + p.Y) + X + p.X)] = true;
            }
        }

        private void ErasePentomino(int index, double X, double Y)
        {
            if (Canvas1.Children.Contains(Pent[index].Poly))
            {
                Canvas1.Children.Remove(Pent[index].Poly);
            }
        }

        private void WaitForStep()
        {
            Thread.Sleep(20);
        }

        private bool Solve(int Index)
        {
            int NewIndex = 0;
            for (int I = 1; I <= 63; I++)
            {
                if (!AppRunning) return false;
                if (Pent[I].Used == false)
                {
                    if (CheckFit(Index, I))
                    {
                        if (CheckFreeSize(Index, I))
                        {
                            PlacePentomino(Index, I);
                            NewIndex = NextFreeIndex(0);
                            if (NewIndex == -1)
                            {
                                return true; //Found a solution!!
                            }
                            else
                            {
                                if (!Solve(NewIndex) & AppRunning)
                                {
                                    RemovePentomino(Index, I);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false; //No pentomino fits at the Index cell
        }

        #endregion
    }
}
