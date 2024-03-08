using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VoronatorSharp;

namespace Voronoi_Stippling
{

    public partial class MainWindow : Window
    {
        private int SeedPointsAmount;
        private List<Vector2> SeedPoints;
        private List<Color> SeedColors;
        private IEnumerable<Triangle> Triangles;
        private List<Vector2> CircumCenters;
        private Voronator vor;
        private List<Polygon> Polygons;
        private bool AppStarted = false;
        private Random Rnd = new Random();
        private BitmapImage bitmap;
        byte[] PixelData;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bitmap = new BitmapImage(new System.Uri(Environment.CurrentDirectory + "\\Dog.jpg"));
            Original.Source = bitmap;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!AppStarted)
            {
                BtnStart.Content = "STOP";
                Canvas2.Children.Clear();
                SeedPointsAmount = int.Parse(TxtPtsNum.Text);
                //Create the Seed Points
                GeneratePoints(Canvas2.ActualWidth, Canvas2.ActualHeight, 5);
                //Create the Voronoi Diagram
                Init();
                //Draw the selectred parts
                Render();
            }
            else
            {
                BtnStart.Content = "START";
                AppStarted = false;
            }
        }

        private void Init()
        {
            //Calculate the Delaunay Triangulation
            GenerateTriangles();
            //Calculate the centers of the circumcircles
            GetCircumCenters();
            //Generate the Voronoi Polygons
            GenerateVoronoiPolygons();
            AppStarted = true;
        }

        private void Render()
        {
            if (!AppStarted)
            {
                return;
            }
            Canvas2.Children.Clear();
            if (CBPoints.IsChecked == true) DrawPoints();
            if (CBTriangles.IsChecked == true) DrawTriangles();
            if (CBCenters.IsChecked == true) DrawCircumCenters();
            if (CBPolygons.IsChecked == true) DrawPolygons();
            if (CBColors.IsChecked == true) ColorPolygons();
        }

        private void GeneratePoints(double MaxX, double MaxY, double margin)
        {
            SeedPoints = new List<Vector2>();
            for (int i = 0; i < SeedPointsAmount; i++)
            {
                float pointX = (float)(margin + Rnd.NextDouble() * (MaxX - 2 * margin));
                float pointY = (float)(margin + Rnd.NextDouble() * (MaxY - 2 * margin));
                SeedPoints.Add(new Vector2(pointX, pointY));
            }
        }

        private void DrawPoints()
        {
            foreach (Vector2 vec in SeedPoints)
            {
                Ellipse myEllipse = new Ellipse()
                {
                    Fill = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 3,
                    Height = 3
                };
                myEllipse.SetValue(Canvas.LeftProperty, vec.x - 0.5 * myEllipse.Width);
                myEllipse.SetValue(Canvas.TopProperty, vec.y - 0.5 * myEllipse.Height);
                Canvas2.Children.Add(myEllipse);
            }
        }

        private void GenerateTriangles()
        {
            Delaunator del = new Delaunator(SeedPoints);
            Triangles = del.GetTriangles();
        }

        private void DrawTriangles()
        {
            foreach (Triangle tr in Triangles)
            {
                Polygon poly = new Polygon()
                {
                    Stroke = Brushes.LightPink,
                    StrokeThickness = 1.0
                };
                poly.Points.Add(new Point(tr.Point1.x, tr.Point1.y));
                poly.Points.Add(new Point(tr.Point2.x, tr.Point2.y));
                poly.Points.Add(new Point(tr.Point3.x, tr.Point3.y));
                Canvas2.Children.Add(poly);
            }
        }

        private void GetCircumCenters()
        {
            CircumCenters = new List<Vector2>();
            foreach (Triangle tr in Triangles)
            {
                CircumCenters.Add(tr.Circumcenter);
            }
        }

        private void DrawCircumCenters()
        {
            foreach (Vector2 vec in CircumCenters)
            {
                Ellipse myEllipse = new Ellipse()
                {
                    Fill = Brushes.Blue,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 4,
                    Height = 4
                };
                myEllipse.SetValue(Canvas.LeftProperty, vec.x - 0.5 * myEllipse.Width);
                myEllipse.SetValue(Canvas.TopProperty, vec.y - 0.5 * myEllipse.Height);
                Canvas2.Children.Add(myEllipse);
            }
        }

        private void GenerateVoronoiPolygons()
        {
            vor = new Voronator(SeedPoints, new Vector2(2, 2), new Vector2((float)(Canvas2.ActualWidth - 2), (float)(Canvas2.ActualHeight - 2)));
            Polygons = new List<Polygon>();
            List<Vector2> polypoints;
            Polygon poly;
            for (int i = 0; i < SeedPoints.Count; i++)
            {
                try
                {
                    polypoints = vor.GetClippedPolygon(i);
                    if (polypoints != null)
                    {
                        poly = new Polygon();
                        foreach (Vector2 vec in polypoints)
                        {
                            poly.Points.Add(new Point(vec.x, vec.y));
                        }
                        Polygons.Add(poly);
                    }
                }
                catch 
                { 
                    //Do nothing
                }
            }
        }

        private void DrawPolygons()
        {
            foreach (Polygon poly in Polygons)
            {
                poly.Stroke = Brushes.LightSteelBlue;
                poly.StrokeThickness = 1.0;
                poly.Fill = null;
                if (!Canvas2.Children.Contains(poly)) Canvas2.Children.Add(poly);
            }
        }

        private void ColorPolygons()
        {
            //Color the Voronoi Polygons with the average color of the corresponding image pixels
            int Stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            PixelData = new byte[Stride * bitmap.PixelHeight];
            bitmap.CopyPixels(PixelData, Stride, 0);
            double[] redtotals = new double[SeedPoints.Count()];
            double[] greentotals = new double[SeedPoints.Count()];
            double[] bluetotals = new double[SeedPoints.Count()];
            double[] pixelcounts = new double[SeedPoints.Count()];
            SeedColors = new List<Color>();
            int cellIndex = 0;
            Color c;
            for (int row = 0; row < Original.Height; row++)
            {
                for (int col = 0; col < Original.Width; col++)
                {
                    cellIndex = vor.Find(new Vector2(col, row), cellIndex);
                    c = getPixelColor(col, row);
                    redtotals[cellIndex] += c.R;
                    greentotals[cellIndex] += c.G;
                    bluetotals[cellIndex] += c.B;
                    pixelcounts[cellIndex] += 1;
                }
            }
            for (int I = 0; I < Polygons.Count(); I++)
            {
                c = Color.FromRgb((byte)(redtotals[I] / pixelcounts[I]), (byte)(greentotals[I] / pixelcounts[I]), (byte)(bluetotals[I] / pixelcounts[I]));
                if (CBPolygons.IsChecked == false) Polygons[I].Stroke = new SolidColorBrush(c);
                Polygons[I].Fill = new SolidColorBrush(c);
                SeedColors.Add(c);
                if (!Canvas2.Children.Contains(Polygons[I])) Canvas2.Children.Add(Polygons[I]);
            }
        }

        private Color getPixelColor(int x, int y)
        {
            int index = (y * (int)Original.Width + x) * bitmap.Format.BitsPerPixel / 8; ;
            return Color.FromRgb(PixelData[index + 2], PixelData[index + 1], PixelData[index + 0]);
        }

        private void BtnPtsNumUP_Click(object sender, RoutedEventArgs e)
        {
            SeedPointsAmount = int.Parse(TxtPtsNum.Text);
            SeedPointsAmount += 50;
            TxtPtsNum.Text = SeedPointsAmount.ToString();
        }

        private void BtnPtsNumDown_Click(object sender, RoutedEventArgs e)
        {
            SeedPointsAmount = int.Parse(TxtPtsNum.Text);
            if (SeedPointsAmount > 50) SeedPointsAmount -= 50;
            TxtPtsNum.Text = SeedPointsAmount.ToString();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Render();
        }

        private void BtnRelaxing_Click(object sender, RoutedEventArgs e)
        {
            //Unweighted relaxing of the Voronoi diagram
            Canvas2.Children.Clear();
            double CP;
            double Area;
            double CX;
            double CY;
            List<Vector2> polyPoints;
            List<Vector2> Centroids;
            Vector2 v1, v2;
            while (AppStarted)
            {
                Centroids = new List<Vector2>();
                //Calculate the centroids of each polygon
                for (int i = 0; i < SeedPoints.Count(); i++)
                {
                    Area = 0;
                    CX = 0;
                    CY = 0;
                    polyPoints = vor.GetClippedPolygon(i);
                    if (polyPoints != null)
                    {
                        for (int j = 0; j < polyPoints.Count(); j++)
                        {
                            v1 = polyPoints[j];
                            v2 = polyPoints[(j + 1) % polyPoints.Count()];
                            CP = v1.x * v2.y - v2.x * v1.y;
                            Area += CP;
                            CX += (v1.x + v2.x) * CP;
                            CY += (v1.y + v2.y) * CP;
                        }
                        Area /= 2;
                        CX /= (6 * Area);
                        CY /= (6 * Area);
                        Centroids.Add(new Vector2((float)CX, (float)CY));
                    }
                    else
                    {
                        Centroids.Add(new Vector2(SeedPoints[i].x, SeedPoints[i].y));
                    }
                }
                //Move the seedpoints towards the centroids
                for (int i = 0; i < SeedPoints.Count(); i++)
                {
                    SeedPoints[i] = Lerp(SeedPoints[i], Centroids[i], (float)0.1);
                }
                //Recalculate the Voronoi Diagram
                Init();
                Render();
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
            }
        }

        private Vector2 Lerp(Vector2 v1, Vector2 v2, float fraction)
        {
            float RX = v1.x + fraction * (v2.x - v1.x);
            float RY = v1.y + fraction * (v2.y - v1.y);
            return new Vector2(RX, RY);
        }

        private void Wait()
        {
            Thread.Sleep(5);
        }

        private void BtnStippling_Click(object sender, RoutedEventArgs e)
        {

            //Without weighted relaxing
            int x, y;
            Color c;
            SeedPoints = new List<Vector2>();
            SeedPointsAmount = int.Parse(TxtPtsNum.Text); ;
            int Stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            PixelData = new byte[Stride * bitmap.PixelHeight];
            bitmap.CopyPixels(PixelData, Stride, 0);
            double Brightness;
            Canvas2.Children.Clear();
            for (int i = 0; i < SeedPointsAmount; i++)
            {
                x = Rnd.Next(bitmap.PixelWidth);
                y = Rnd.Next(bitmap.PixelHeight);
                c = getPixelColor(x, y);
                Brightness = c.R + c.G + c.B;
                if (Brightness < Rnd.Next(500))
                {
                    SeedPoints.Add(new Vector2(x, y));
                }
                else
                {
                    i--;
                }
            }
            Init();
            Render();
            if (CBRelaxing.IsChecked == true) //Use Weighted relaxing
            {
                List<Vector2> Centroids;
                double[] CentX;
                double[] CentY;
                double[] Weights;
                int cellIndex = 0;
                double weight;
                while (true)
                {
                    Centroids = new List<Vector2>();
                    CentX = new double[SeedPoints.Count()];
                    CentY = new double[SeedPoints.Count()];
                    Weights = new double[SeedPoints.Count()];
                    SeedColors = new List<Color>();

                    //Calculate the centroids of each polygon weighted by the image pixels brightness
                    for (int row = 0; row < Original.Height; row++)
                    {
                        for (int col = 0; col < Original.Width; col++)
                        {
                            cellIndex = vor.Find(new Vector2(col, row), cellIndex);
                            c = getPixelColor(col, row);
                            Brightness = (c.R + c.G + c.B) / 3;
                            weight = 1 - Brightness / 255.0;
                            CentX[cellIndex] += col * weight;
                            CentY[cellIndex] += row * weight;
                            Weights[cellIndex] += weight;
                        }
                    }
                    for (int i = 0; i < SeedPoints.Count(); i++)
                    {
                        if (Weights[i] > 0)
                        {
                            Centroids.Add(new Vector2((float)(CentX[i] / Weights[i]), (float)(CentY[i] / Weights[i])));
                        }
                        else
                        {
                            Centroids.Add(new Vector2(SeedPoints[i].x, SeedPoints[i].y));
                        }
                    }
                    //Move the seedpoints towards the centroids
                    for (int i = 0; i < SeedPoints.Count(); i++)
                    {
                        SeedPoints[i] = Lerp(SeedPoints[i], Centroids[i], (float)0.1);
                    }
                    //Recalculate the Voronoi Diagram
                    try
                    {
                        Init();
                    }
                    catch
                    {
                        //Do nothing
                    }
                    Render();
                    Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}