using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Star_Patterns
{
    public partial class MainWindow : Window
    {
        private List<Tile> Tiles;
        private double TileLength = 75.0;
        private double TileHeight = 75.0;
        private double delta = 0;
        private double angle = 60;
        private Brush FillColor1 = Brushes.LightGreen;
        private Brush FillColor2 = Brushes.Orange;
        private Brush FillColor3 = Brushes.LightBlue;
        private Brush FillColor4 = Brushes.Pink;
        private Brush FillColor5 = Brushes.Beige;
        private Brush FillColor6 = Brushes.LightCyan;
        private Brush StarColor = Brushes.Yellow;
        private List<Brush> My_Brushes;
        private bool App_Loaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //SldHorSize.Value = TileLength;
            //SldVertSize.Value = TileHeight;
            TxtHorSize.Text = TileLength.ToString();
            TxtVertSize.Text = TileHeight.ToString();
            SldDelta.Value = delta;
            SldAngle.Value = angle;
            LstTiling.SelectedIndex = 0;
            My_Brushes = new List<Brush>();
            Type BrushesType = typeof(Brushes);
            BrushConverter bc = new BrushConverter();
            foreach (System.Reflection.PropertyInfo propinfo in BrushesType.GetProperties())
            {
                if (propinfo.PropertyType == typeof(SolidColorBrush))
                {
                    CmbColor1.Items.Add(propinfo.Name);
                    CmbColor2.Items.Add(propinfo.Name);
                    CmbColor3.Items.Add(propinfo.Name);
                    CmbColor4.Items.Add(propinfo.Name);
                    CmbColor5.Items.Add(propinfo.Name);
                    CmbColor6.Items.Add(propinfo.Name);
                    CmbStarColor.Items.Add(propinfo.Name);
                    My_Brushes.Add((Brush)bc.ConvertFromString(propinfo.Name));
                }
            }
            for (int I = 0; I < My_Brushes.Count; I++)
            {
                if (bc.ConvertToString(My_Brushes[I]) == bc.ConvertToString(FillColor1))
                {
                    CmbColor1.SelectedIndex = I;
                }
                if (bc.ConvertToString(My_Brushes[I]) == bc.ConvertToString(FillColor2))
                {
                    CmbColor2.SelectedIndex = I;
                }
                if (bc.ConvertToString(My_Brushes[I]) == bc.ConvertToString(FillColor3))
                {
                    CmbColor3.SelectedIndex = I;
                }
                if (bc.ConvertToString(My_Brushes[I]) == bc.ConvertToString(FillColor4))
                {
                    CmbColor4.SelectedIndex = I;
                }
                if (bc.ConvertToString(My_Brushes[I]) == bc.ConvertToString(FillColor5))
                {
                    CmbColor5.SelectedIndex = I;
                }
                if (bc.ConvertToString(My_Brushes[I]) == bc.ConvertToString(FillColor6))
                {
                    CmbColor6.SelectedIndex = I;
                }
                if (bc.ConvertToString(My_Brushes[I]) == bc.ConvertToString(StarColor))
                {
                    CmbStarColor.SelectedIndex = I;
                }
            }
            App_Loaded = true;
            DrawTiles();
        }

        private void GetColors(object sender, SelectionChangedEventArgs e)
        {
            if (!App_Loaded) return;
            BrushConverter bc = new BrushConverter();
            FillColor1 = (Brush)bc.ConvertFromString(CmbColor1.SelectedItem.ToString());
            FillColor2 = (Brush)bc.ConvertFromString(CmbColor2.SelectedItem.ToString());
            FillColor3 = (Brush)bc.ConvertFromString(CmbColor3.SelectedItem.ToString());
            FillColor4 = (Brush)bc.ConvertFromString(CmbColor4.SelectedItem.ToString());
            FillColor5 = (Brush)bc.ConvertFromString(CmbColor5.SelectedItem.ToString());
            FillColor6 = (Brush)bc.ConvertFromString(CmbColor6.SelectedItem.ToString());
            StarColor = (Brush)bc.ConvertFromString(CmbStarColor.SelectedItem.ToString());
            DrawTiles();
        }

        private void DrawTiles()
        {
            Tiles = new List<Tile>();
            canvas1.Children.Clear();
            switch (LstTiling.SelectedIndex)
            {
                case 0:
                    TriangleTiling1(TileLength, TileHeight);
                    break;
                case 1:
                    TriangleTiling2(TileLength, TileHeight);
                    break;
                case 2:
                    TriangleTiling3(TileLength, TileHeight);
                    break;
                case 3:
                    RectangleTiling1(TileLength, TileHeight);
                    break;
                case 4:
                    RectangleTiling2(TileLength, TileHeight);
                    break;
                case 5:
                    PentagonTiling(TileLength);
                    break;
                case 6:
                    HexagonTiling1(TileLength);
                    break;
                case 7:
                    HexagonTiling2(TileLength);
                    break;
                case 8:
                    OctagonTiling1(TileLength);
                    break;
                case 9:
                    OctagonTiling2(TileLength);
                    break;
                case 10:
                    DecagonTiling(TileLength);
                    break;
                case 11:
                    DodecagonTiling1(TileLength);
                    break;
                case 12:
                    DodecagonTiling2(TileLength);
                    break;
            }
            //Draw the tiles
            for (int I = 0; I < Tiles.Count; I++)
            {
                Tiles[I].MakeStar(delta, angle);
                if (CbShowTiles.IsChecked == true) Tiles[I].Draw(canvas1);
                if (CbShowStar.IsChecked == true) Tiles[I].DrawStar(canvas1);
                if (CbShowLines.IsChecked == true) Tiles[I].DrawLines(canvas1);
            }
        }

        #region "Tiling Patterns"

        /// <summary>
        /// Create a Horizontal grid with 2 Triangles Tiling Pattern
        /// </summary>
        private void TriangleTiling1(double L, double H)
        {
            Tile T;
            double X0 = 0.0;
            double Y0 = 0.0;
            do
            {
                T = new Tile();
                T.AddPoint(X0, Y0);
                T.AddPoint(X0 + L, Y0);
                T.AddPoint(X0 + L, Y0 + H);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0, Y0);
                T.AddPoint(X0 + L, Y0 + H);
                T.AddPoint(X0, Y0 + H);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + L;
                if (X0 >= canvas1.ActualWidth + L / 4)
                {
                    X0 = 0;
                    Y0 = Y0 + H;
                    if (Y0 >= canvas1.ActualHeight) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a Horizontal Grid with 4 Triangles Tiling Pattern
        /// </summary>
        private void TriangleTiling2(double L, double H)
        {
            Tile T;
            double X0 = 0.0;
            double Y0 = 0.0;
            do
            {
                T = new Tile();
                T.AddPoint(X0, Y0);
                T.AddPoint(X0 + L, Y0);
                T.AddPoint(X0 + L / 2, Y0 + H / 2);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + L, Y0);
                T.AddPoint(X0 + L, Y0 + H);
                T.AddPoint(X0 + L / 2, Y0 + H / 2);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + L, Y0 + H);
                T.AddPoint(X0, Y0 + H);
                T.AddPoint(X0 + L / 2, Y0 + H / 2);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0, Y0 + H);
                T.AddPoint(X0, Y0);
                T.AddPoint(X0 + L / 2, Y0 + H / 2);
                T.FillColor = FillColor4;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + L;
                if (X0 >= canvas1.ActualWidth + L / 2)
                {
                    X0 = 0;
                    Y0 = Y0 + H;
                    if (Y0 >= canvas1.ActualHeight) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a 45° Rotated Grid with 2 Triangles Tiling Pattern
        /// </summary>
        private void TriangleTiling3(double L, double H)
        {
            Tile T;
            bool ToLeft = true;
            double X0 = 0.0;
            double Y0 = 0.0;
            do
            {
                T = new Tile();
                T.AddPoint(X0, Y0 - H);
                T.AddPoint(X0 + L / 2, Y0);
                T.AddPoint(X0 - L / 2, Y0);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + L / 2, Y0);
                T.AddPoint(X0, Y0 + H);
                T.AddPoint(X0 - L / 2, Y0);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + L;
                if (X0 >= canvas1.ActualWidth + L / 2)
                {
                    if (ToLeft)
                    {
                        X0 = L / 2;
                        ToLeft = false;
                    }
                    else
                    {
                        X0 = 0;
                        ToLeft = true;
                    }
                    Y0 = Y0 + H;
                    if (Y0 >= canvas1.ActualHeight + H) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a Rectangular grid Tiling Pattern
        /// </summary>
        private void RectangleTiling1(double L, double H)
        {
            Tile T;
            double X0 = 0.0;
            double Y0 = 0.0;
            do
            {
                T = new Tile();
                T.AddPoint(X0, Y0);
                T.AddPoint(X0 + L, Y0);
                T.AddPoint(X0 + L, Y0 + H);
                T.AddPoint(X0, Y0 + H);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + L;
                if (X0 >= canvas1.ActualWidth + L / 2)
                {
                    X0 = 0;
                    Y0 = Y0 + H;
                    if (Y0 >= canvas1.ActualHeight) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a 4 Rotated Rectangles and square Tiling Pattern
        /// </summary>
        private void RectangleTiling2(double L, double H)
        {
            Tile T;
            double X0 = 0.0;
            double Y0 = 0.0;
            do
            {
                T = new Tile();
                T.AddPoint(X0 + H, Y0 + H);
                T.AddPoint(X0 + L, Y0 + H);
                T.AddPoint(X0 + L, Y0 + L);
                T.AddPoint(X0 + H, Y0 + L);
                T.FillColor = FillColor5;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0, Y0);
                T.AddPoint(X0 + H, Y0);
                T.AddPoint(X0 + H, Y0 + L);
                T.AddPoint(X0, Y0 + L);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + H, Y0);
                T.AddPoint(X0 + H + L, Y0);
                T.AddPoint(X0 + H + L, Y0 + H);
                T.AddPoint(X0 + H, Y0 + H);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0, Y0 + L);
                T.AddPoint(X0 + L, Y0 + L);
                T.AddPoint(X0 + L, Y0 + H + L);
                T.AddPoint(X0, Y0 + H + L);
                T.FillColor = FillColor4;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + L, Y0 + H);
                T.AddPoint(X0 + H + L, Y0 + H);
                T.AddPoint(X0 + H + L, Y0 + H + L);
                T.AddPoint(X0 + L, Y0 + H + L);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + H + L;
                if (X0 >= canvas1.ActualWidth + H / 2)
                {
                    X0 = 0;
                    Y0 = Y0 + H + L;
                    if (Y0 >= canvas1.ActualHeight) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a Pentagonal Tiling Pattern
        /// </summary>
        private void PentagonTiling(double L)
        {
            Tile T;
            double X0 = 0.0;
            double Y0 = 0.0;
            double H1 = L * Math.Sin(36 * Math.PI / 180);
            double H2 = L * Math.Sin(72 * Math.PI / 180);
            double W = L * Math.Cos(36 * Math.PI / 180);
            do
            {
                T = new Tile();
                T.AddPoint(X0 - L / 2, Y0 + H2);
                T.AddPoint(X0 + L / 2, Y0 + H2);
                T.AddPoint(X0 + W + L / 2, Y0 + H1 + H2);
                T.AddPoint(X0 + W - L / 2, Y0 + H1 + H2);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0, Y0 - H1);
                T.AddPoint(X0 + W, Y0);
                T.AddPoint(X0 + L / 2, Y0 + H2);
                T.AddPoint(X0 - L / 2, Y0 + H2);
                T.AddPoint(X0 - W, Y0);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + W, Y0);
                T.AddPoint(X0 + W + L, Y0);
                T.AddPoint(X0 + 2 * W + L / 2, Y0 + H2);
                T.AddPoint(X0 + W + L / 2, Y0 + H1 + H2);
                T.AddPoint(X0 + L / 2, Y0 + H2);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 - L / 2, Y0 + H2);
                T.AddPoint(X0 + W - L / 2, Y0 + H1 + H2);
                T.AddPoint(X0, Y0 + H1 + 2 * H2);
                T.AddPoint(X0 - L, Y0 + H1 + 2 * H2);
                T.AddPoint(X0 - W - L / 2, Y0 + H1 + H2);
                T.FillColor = FillColor4;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + W - L / 2, Y0 + H1 + H2);
                T.AddPoint(X0 + W + L / 2, Y0 + H1 + H2);
                T.AddPoint(X0 + 2 * W, Y0 + H1 + 2 * H2);
                T.AddPoint(X0 + W, Y0 + 2 * (H1 + H2));
                T.AddPoint(X0, Y0 + H1 + 2 * H2);
                T.FillColor = FillColor5;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + 2 * W, Y0 + H1 + 2 * H2);
                T.AddPoint(X0 + 2 * W + L, Y0 + H1 + 2 * H2);
                T.AddPoint(X0 + W + L, Y0 + 2 * (H1 + H2));
                T.AddPoint(X0 + W, Y0 + 2 * (H1 + H2));
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + 2 * W + L;
                if (X0 >= canvas1.ActualWidth + L + W)
                {
                    X0 = 0;
                    Y0 = Y0 + 2 * (H1 + H2);
                    if (Y0 >= canvas1.ActualHeight) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a Hexagonal Tiling Pattern
        /// </summary>
        private void HexagonTiling1(double L)
        {
            Tile T;
            double X0 = 0.0;
            double Y0 = 0.0;
            double H = L * Math.Sin(Math.PI / 3);
            do
            {
                T = new Tile();
                T.AddPoint(X0, Y0 - H);
                T.AddPoint(X0 + L, Y0 - H);
                T.AddPoint(X0 + 3 * L / 2, Y0);
                T.AddPoint(X0 + L, Y0 + H);
                T.AddPoint(X0, Y0 + H);
                T.AddPoint(X0 - L / 2, Y0);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + 3 * L / 2, Y0);
                T.AddPoint(X0 + 5 * L / 2, Y0);
                T.AddPoint(X0 + 3 * L, Y0 + H);
                T.AddPoint(X0 + 5 * L / 2, Y0 + 2 * H);
                T.AddPoint(X0 + 3 * L / 2, Y0 + 2 * H);
                T.AddPoint(X0 + L, Y0 + H);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + 3 * L;
                if (X0 >= canvas1.ActualWidth + L / 2)
                {
                    X0 = 0;
                    Y0 = Y0 + 2 * H;
                    if (Y0 >= canvas1.ActualHeight + 2 * H) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a spaced Hexagonal Tiling Pattern
        /// </summary>
        private void HexagonTiling2(double L)
        {
            Tile T;
            bool up = true;
            double X0 = 0.0;
            double Y0 = 0.0;
            double H = L * Math.Sin(Math.PI / 3);
            double W = L * Math.Cos(Math.PI / 3);
            do
            {
                T = new Tile();
                T.AddPoint(X0, Y0);
                T.AddPoint(X0 + L, Y0);
                T.AddPoint(X0 + L + W, Y0 + H);
                T.AddPoint(X0 + L, Y0 + 2 * H);
                T.AddPoint(X0, Y0 + 2 * H);
                T.AddPoint(X0 - W, Y0 + H);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 - W, Y0 + H);
                T.AddPoint(X0, Y0 + 2 * H);
                T.AddPoint(X0 - H, Y0 + 2 * H + W);
                T.AddPoint(X0 - H - W, Y0 + H + W);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0, Y0 + 2 * H);
                T.AddPoint(X0 + L, Y0 + 2 * H);
                T.AddPoint(X0 + L, Y0 + L + 2 * H);
                T.AddPoint(X0, Y0 + L + 2 * H);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + L + W, Y0 + H);
                T.AddPoint(X0 + L + H + W, Y0 + H + W);
                T.AddPoint(X0 + L + H, Y0 + 2 * H + W);
                T.AddPoint(X0 + L, Y0 + 2 * H);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0, Y0 + 2 * H);
                T.AddPoint(X0, Y0 + L + 2 * H);
                T.AddPoint(X0 - H, Y0 + 2 * H + W);
                T.FillColor = FillColor4;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + L, Y0 + 2 * H);
                T.AddPoint(X0 + L + H, Y0 + 2 * H + W);
                T.AddPoint(X0 + L, Y0 + L + 2 * H);
                T.FillColor = FillColor4;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + L + H + W;
                if (up)
                {
                    Y0 = Y0 - H - W;
                    up = false;
                }
                else
                {
                    Y0 = Y0 + H + W;
                    up = true;
                }
                if (X0 >= canvas1.ActualWidth + L + W)
                {
                    X0 = 0;
                    if (up)
                    {
                        Y0 = Y0 + L + 2 * H;
                    }
                    else
                    {
                        Y0 = Y0 + L + 3 * H + W;
                    }
                    up = true;
                    if (Y0 >= canvas1.ActualHeight + 2 * H) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a Octagon Grid Tiling Pattern
        /// </summary>
        private void OctagonTiling1(double L)
        {
            Tile T;
            double X0 = 0.0;
            double Y0 = 0.0;
            double H = L * Math.Sin(Math.PI / 4);
            do
            {
                T = new Tile();
                T.AddPoint(X0, Y0 - H);
                T.AddPoint(X0 + L, Y0 - H);
                T.AddPoint(X0 + L + H, Y0);
                T.AddPoint(X0 + L + H, Y0 + L);
                T.AddPoint(X0 + L, Y0 + L + H);
                T.AddPoint(X0, Y0 + L + H);
                T.AddPoint(X0 - H, Y0 + L);
                T.AddPoint(X0 - H, Y0);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + L + H, Y0 + L);
                T.AddPoint(X0 + L + 2 * H, Y0 + L + H);
                T.AddPoint(X0 + L + H, Y0 + L + 2 * H);
                T.AddPoint(X0 + L, Y0 + L + H);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + L + 2 * H;
                if (X0 >= canvas1.ActualWidth + L)
                {
                    X0 = 0;
                    Y0 = Y0 + L + 2 * H;
                    if (Y0 >= canvas1.ActualHeight + L) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a 45° rotated Octagon Tiling Pattern
        /// </summary>
        private void OctagonTiling2(double L)
        {
            Tile T;
            double X0 = 0.0;
            double Y0 = 0.0;
            double H = L * Math.Sin(Math.PI / 4);
            do
            {
                T = new Tile();
                T.AddPoint(X0 + H, Y0 - H);
                T.AddPoint(X0 + H + L, Y0 - H);
                T.AddPoint(X0 + 2 * H + L, Y0);
                T.AddPoint(X0 + 2 * H + L, Y0 + L);
                T.AddPoint(X0 + H + L, Y0 + H + L);
                T.AddPoint(X0 + H, Y0 + H + L);
                T.AddPoint(X0, Y0 + L);
                T.AddPoint(X0, Y0);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + 2 * H + L, Y0);
                T.AddPoint(X0 + 2 * (H + L), Y0);
                T.AddPoint(X0 + 2 * (H + L), Y0 + L);
                T.AddPoint(X0 + 2 * H + L, Y0 + L);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 - L, Y0 + L);
                T.AddPoint(X0, Y0 + L);
                T.AddPoint(X0 + H, Y0 + L + H);
                T.AddPoint(X0 + H, Y0 + 2 * L + H);
                T.AddPoint(X0, Y0 + 2 * (L + H));
                T.AddPoint(X0 - L, Y0 + 2 * (L + H));
                T.AddPoint(X0 - L - H, Y0 + 2 * L + H);
                T.AddPoint(X0 - L - H, Y0 + L + H);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + H, Y0 + H + L);
                T.AddPoint(X0 + L + H, Y0 + H + L);
                T.AddPoint(X0 + L + H, Y0 + H + 2 * L);
                T.AddPoint(X0 + H, Y0 + H + 2 * L);
                T.FillColor = FillColor4;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + 2 * (L + H);
                if (X0 >= canvas1.ActualWidth + 3 * L)
                {
                    X0 = 0;
                    Y0 = Y0 + 2 * (L + H);
                    if (Y0 >= canvas1.ActualHeight + L) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a Decagon Tiling Pattern
        /// </summary>
        private void DecagonTiling(double L)
        {
            Tile T;
            double H1 = L * Math.Sin(54 * Math.PI / 180);
            double W1 = L * Math.Cos(54 * Math.PI / 180);
            double H2 = L * Math.Sin(18 * Math.PI / 180);
            double W2 = L * Math.Cos(18 * Math.PI / 180);
            double X0 = -W1;
            double Y0 = 0.0;
            do
            {
                T = new Tile();
                T.AddPoint(X0, Y0);
                T.AddPoint(X0 + W1, Y0 - H1);
                T.AddPoint(X0 + W1 + W2, Y0 - H1 - H2);
                T.AddPoint(X0 + W1 + 2 * W2, Y0 - H1);
                T.AddPoint(X0 + 2 * (W1 + W2), Y0);
                T.AddPoint(X0 + 2 * (W1 + W2), Y0 + L);
                T.AddPoint(X0 + W1 + 2 * W2, Y0 + L + H1);
                T.AddPoint(X0 + W1 + W2, Y0 + L + H1 + H2);
                T.AddPoint(X0 + W1, Y0 + L + H1);
                T.AddPoint(X0, Y0 + L);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + 2 * (W1 + W2), Y0 + L);
                T.AddPoint(X0 + 3 * W1 + 2 * W2, Y0 + L + H1);
                T.AddPoint(X0 + 3 * W1 + W2, Y0 + L + H1 + H2);
                T.AddPoint(X0 + 2 * W1 + W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 + W1 + W2, Y0 + L + H1 + H2);
                T.AddPoint(X0 + W1 + 2 * W2, Y0 + L + H1);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + W1, Y0 + L + H1);
                T.AddPoint(X0 + W1 + W2, Y0 + L + H1 + H2);
                T.AddPoint(X0 + 2 * W1 + W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 + 2 * W1 + W2, Y0 + 2 * (L + H1) + H2);
                T.AddPoint(X0 + W1 + W2, Y0 + 2 * L + 3 * H1 + H2);
                T.AddPoint(X0 + W1, Y0 + 2 * L + 3 * H1 + 2 * H2);
                T.AddPoint(X0 + W1 - W2, Y0 + 2 * L + 3 * H1 + H2);
                T.AddPoint(X0 - W2, Y0 + 2 * (L + H1) + H2);
                T.AddPoint(X0 - W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 + W1 - W2, Y0 + L + H1 + H2);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + 2 * W1 + W2, Y0 + 2 * (L + H1) + H2);
                T.AddPoint(X0 + 3 * W1 + W2, Y0 + 2 * L + 3 * H1 + H2);
                T.AddPoint(X0 + 3 * W1 + 2 * W2, Y0 + 2 * L + 3 * H1 + 2 * H2);
                T.AddPoint(X0 + 2 * (W1 + W2), Y0 + 2 * L + 4 * H1 + 2 * H2);
                T.AddPoint(X0 + W1 + 2 * W2, Y0 + 2 * L + 3 * H1 + 2 * H2);
                T.AddPoint(X0 + W1 + W2, Y0 + 2 * L + 3 * H1 + H2);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + 2 * (W1 + W2);
                if (X0 >= canvas1.ActualWidth + L)
                {
                    X0 = -W1;
                    Y0 = Y0 + 2 * L + 4 * H1 + 2 * H2;
                    if (Y0 >= canvas1.ActualHeight + L) break;
                }
            } while (true);
        }

        /// <summary>
        /// Create a Spaced Dodecagon Tiling Pattern
        /// </summary>
        private void DodecagonTiling1(double L)
        {
            Tile T;
            bool up = true;
            double H1 = L * Math.Sin(60 * Math.PI / 180);
            double W1 = L * Math.Cos(60 * Math.PI / 180);
            double H2 = L * Math.Sin(30 * Math.PI / 180);
            double W2 = L * Math.Cos(30 * Math.PI / 180);
            double X0 = 0.0;
            double Y0 = 0.0;
            do
            {
                T = new Tile();
                T.AddPoint(X0, Y0);
                T.AddPoint(X0 + W2, Y0 - H2);
                T.AddPoint(X0 + L + W2, Y0 - H2);
                T.AddPoint(X0 + L + 2 * W2, Y0);
                T.AddPoint(X0 + L + 2 * W2 + W1, Y0 + H1);
                T.AddPoint(X0 + L + 2 * W2 + W1, Y0 + L + H1);
                T.AddPoint(X0 + L + 2 * W2, Y0 + L + 2 * H1);
                T.AddPoint(X0 + L + W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 + W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0, Y0 + L + 2 * H1);
                T.AddPoint(X0 - W1, Y0 + L + H1);
                T.AddPoint(X0 - W1, Y0 + H1);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 - W1, Y0 + L + H1);
                T.AddPoint(X0, Y0 + L + 2 * H1);
                T.AddPoint(X0 - W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 - W2 - W1, Y0 + L + H1 + H2);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 + L + W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 + L + W2, Y0 + 2 * L + 2 * H1 + H2);
                T.AddPoint(X0 + W2, Y0 + 2 * L + 2 * H1 + H2);
                T.FillColor = FillColor4;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + L + 2 * W2 + W1, Y0 + L + H1);
                T.AddPoint(X0 + L + 3 * W2 + W1, Y0 + L + H1 + H2);
                T.AddPoint(X0 + L + 3 * W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 + L + 2 * W2, Y0 + L + 2 * H1);
                T.FillColor = FillColor5;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0, Y0 + L + 2 * H1);
                T.AddPoint(X0 + W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 + W2, Y0 + 2 * L + 2 * H1 + H2);
                T.AddPoint(X0, Y0 + 2 * (L + H1 + H2));
                T.AddPoint(X0 - W2, Y0 + 2 * L + 2 * H1 + H2);
                T.AddPoint(X0 - W2, Y0 + L + 2 * H1 + H2);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + L + 2 * W2, Y0 + L + 2 * H1);
                T.AddPoint(X0 + L + 3 * W2, Y0 + L + 2 * H1 + H2);
                T.AddPoint(X0 + L + 3 * W2, Y0 + 2 * L + 2 * H1 + H2);
                T.AddPoint(X0 + L + 2 * W2, Y0 + 2 * (L + H1 + H2));
                T.AddPoint(X0 + L + W2, Y0 + 2 * L + 2 * H1 + H2);
                T.AddPoint(X0 + L + W2, Y0 + L + 2 * H1 + H2);
                T.FillColor = FillColor6;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + L + 3 * W2 + W1;
                if (up)
                {
                    Y0 = Y0 - (L + H1 + H2);
                    up = false;
                }
                else
                {
                    Y0 = Y0 + (L + H1 + H2);
                    up = true;
                }
                if (X0 >= canvas1.ActualWidth + 2 * W2)
                {
                    X0 = 0;
                    if (up)
                    {
                        Y0 = Y0 + 2 * L + 2 * H1 + 2 * H2;
                        if (Y0 >= canvas1.ActualHeight + L + 2 * H1 + H2) break;
                    }
                    else
                    {
                        Y0 = Y0 + 3 * L + 3 * H1 + 3 * H2;
                        if (Y0 >= canvas1.ActualHeight + 2 * H1) break;
                    }
                    up = true;
                }
            } while (true);
        }

        /// <summary>
        /// Create a stacked Dodecagon and Triangle Tiling Pattern
        /// </summary>
        private void DodecagonTiling2(double L)
        {
            Tile T;
            bool up = true;
            double H1 = L * Math.Sin(Math.PI / 4);
            double H2 = L * Math.Sin(15 * Math.PI / 180);
            double X0 = 0.0;
            double Y0 = 0.0;
            do
            {
                T = new Tile();
                T.AddPoint(X0 - H1, Y0 - 2 * (H1 + H2));
                T.AddPoint(X0 + H2, Y0 - 2 * H1 - H2);
                T.AddPoint(X0 + H1 + H2, Y0 - H1 - H2);
                T.AddPoint(X0 + H1 + 2 * H2, Y0);
                T.AddPoint(X0 + H1 + H2, Y0 + H1 + H2);
                T.AddPoint(X0 + H2, Y0 + 2 * H1 + H2);
                T.AddPoint(X0 - H1, Y0 + 2 * (H1 + H2));
                T.AddPoint(X0 - 2 * H1 - H2, Y0 + 2 * H1 + H2);
                T.AddPoint(X0 - 3 * H1 - H2, Y0 + H1 + H2);
                T.AddPoint(X0 - 3 * H1 - 2 * H2, Y0);
                T.AddPoint(X0 - 3 * H1 - H2, Y0 - H1 - H2);
                T.AddPoint(X0 - 2 * H1 - H2, Y0 - 2 * H1 - H2);
                T.FillColor = FillColor1;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + H1 + 2 * H2, Y0);
                T.AddPoint(X0 + 2 * (H1 + H2), Y0 - H1);
                T.AddPoint(X0 + 3 * H1 + 2 * H2, Y0);
                T.AddPoint(X0 + 2 * (H1 + H2), Y0 + H1);
                T.FillColor = FillColor2;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 - 3 * H1 - 2 * H2, Y0);
                T.AddPoint(X0 - 3 * H1 - H2, Y0 + H1 + H2);
                T.AddPoint(X0 - 4 * H1 - 2 * H2, Y0 + H1);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 - 2 * H1 - H2, Y0 + 2 * H1 + H2);
                T.AddPoint(X0 - H1, Y0 + 2 * (H1 + H2));
                T.AddPoint(X0 - 2 * H1, Y0 + 3 * H1 + 2 * H2);
                T.FillColor = FillColor3;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 - H1, Y0 + 2 * (H1 + H2));
                T.AddPoint(X0 + H2, Y0 + +2 * H1 + H2);
                T.AddPoint(X0, Y0 + 3 * H1 + 2 * H2);
                T.FillColor = FillColor4;
                T.StarColor = StarColor;
                Tiles.Add(T);
                T = new Tile();
                T.AddPoint(X0 + H1 + 2 * H2, Y0);
                T.AddPoint(X0 + 2 * (H1 + H2), Y0 + H1);
                T.AddPoint(X0 + H1 + H2, Y0 + H1 + H2);
                T.FillColor = FillColor4;
                T.StarColor = StarColor;
                Tiles.Add(T);
                X0 = X0 + 3 * H1 + 2 * H2;
                if (up)
                {
                    Y0 = Y0 - (3 * H1 + 2 * H2);
                    up = false;
                }
                else
                {
                    Y0 = Y0 + (3 * H1 + 2 * H2);
                    up = true;
                }
                if (X0 >= canvas1.ActualWidth + 4 * H1)
                {
                    X0 = 0;

                    if (up)
                    {
                        Y0 = Y0 + 6 * H1 + 4 * H2;
                        if (Y0 >= canvas1.ActualHeight + 3 * H1) break;
                    }
                    else
                    {
                        Y0 = Y0 + 9 * H1 + 6 * H2;
                        if (Y0 >= canvas1.ActualHeight + 5 * H1 + 2 * H2) break;
                    }
                    up = true;
                }
            } while (true);
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!App_Loaded) return;
            DrawTiles();
        }

        private void BtnHorSizeUP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TileLength = double.Parse(TxtHorSize.Text);
                TileLength += 5;
                if (TileLength > canvas1.ActualWidth) TileLength = Math.Floor(canvas1.ActualWidth);
                TxtHorSize.Text = TileLength.ToString();
            }
            catch
            {
                //Do nothing
            }
        }

        private void BtnHorSizeDown_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                TileLength = double.Parse(TxtHorSize.Text);
                TileLength -= 5;
                if (TileLength < 10) TileLength = 10;
                TxtHorSize.Text = TileLength.ToString();
            }
            catch
            {
                //Do nothing
            }
        }

        private void TxtHorSize_TextChanged(Object sender, TextChangedEventArgs e)
        {
            double dummy;
            try
            {
                dummy = double.Parse(TxtHorSize.Text);
                if (dummy >= 5 & dummy <= canvas1.ActualWidth)
                {
                    TileLength = dummy;
                    DrawTiles();
                }
            }
            catch
            {
                //Do nothing
            }
        }

        private void BtnVertSizeUP_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                TileHeight = double.Parse(TxtVertSize.Text);
                TileHeight += 5;
                if (TileHeight > canvas1.ActualHeight) TileHeight = Math.Floor(canvas1.ActualHeight);
                TxtVertSize.Text = TileHeight.ToString();
            }
            catch
            {
                //Do nothing
            }
        }

        private void BtnVertSizeDown_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                TileHeight = double.Parse(TxtVertSize.Text);
                TileHeight -= 5;
                if (TileHeight < 10) TileHeight = 10;
                TxtVertSize.Text = TileHeight.ToString();
            }
            catch
            {
                //Do nothing
            }
        }

        private void TxtVertSize_TextChanged(Object sender, TextChangedEventArgs e)
        {
            double dummy;
            try
            {
                dummy = double.Parse(TxtVertSize.Text);
                if (dummy >= 5 & dummy <= canvas1.ActualHeight)
                {
                    TileHeight = dummy;
                    DrawTiles();
                }
            }
            catch
            {
                //Do nothing
            }
        }

        private void SldDelta_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!App_Loaded) return;
            delta = SldDelta.Value;
            DrawTiles();
        }

        private void SldAngle_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!App_Loaded) return;
            angle = SldAngle.Value;
            DrawTiles();
        }

        private void CbShowStar_Click(Object sender, RoutedEventArgs e)
        {
            if (CbShowStar.IsChecked == true)
            {
                CbShowLines.IsEnabled = false;
                CbShowLines.IsChecked = false;
            }
            else
            {
                CbShowLines.IsEnabled = true;
                CbShowLines.IsChecked = true;
            }
            DrawTiles();
        }

        private void CbShowLines_Click(Object sender, RoutedEventArgs e)
        {
            DrawTiles();
        }

        private void CbShowTiles_Click(Object sender, RoutedEventArgs e)
        {
            DrawTiles();
        }
    }
}