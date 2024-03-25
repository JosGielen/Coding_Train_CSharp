using JG_Math;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NoiseGifLoop
{
    public partial class MainWindow : Window
    {
        private List<Line> lines;
        private bool Rendering = false;
        private bool recording = false;
        private string ResultFileName = "SimplexNoiseGifLoop.gif";
        private double AngleStep = 1;
        private double MaxRadius = 200.0;
        private int Percent = 0;
        private bool useOpenSimplex = true;
        private List<Brush> myColors;
        BitmapEncoder MyEncoder = new GifBitmapEncoder();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ColorPalette cp = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            myColors = cp.GetColorBrushes((int)MaxRadius);
            lines = new List<Line>();
            Line L;
            for (double Angle = 0; Angle <= 360; Angle += AngleStep)
            {
                L = new Line()
                {
                    StrokeThickness = 2
                };
                lines.Add(L);
                canvas1.Children.Add(L);
            }
            Rendering = true;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!Rendering) return;
            double XOff;
            double YOff;
            double Offx;
            double Offy;
            double X;
            double Y;
            double PrevX = 0.0;
            double PrevY = 0.0;
            double F = 5.0; //Determines how fast the Noise changes
            Offx = Math.Cos(2 * Percent * Math.PI / 100);
            Offy = Math.Sin(2 * Percent * Math.PI / 100);
            for (int I = 0; I < lines.Count; I++)
            {
                double Angle = 2 * Math.PI * I / (lines.Count - 1);
                XOff = F * Math.Cos(Angle);
                YOff = F * Math.Sin(Angle);
                double R;
                if (useOpenSimplex)
                {
                    R = MaxRadius * OpenSimplexNoise.Noise2D(XOff + Offx, YOff + Offy);
                }
                else
                {
                    R = MaxRadius * PerlinNoise.Noise2D(XOff + Offx, YOff + Offy);
                }
                X = R * Math.Cos(Angle) + canvas1.ActualWidth / 2;
                Y = R * Math.Sin(Angle) + canvas1.ActualHeight / 2;
                if (I > 0)
                {
                    lines[I].Stroke = myColors[(int)R];
                    lines[I].X1 = PrevX;
                    lines[I].Y1 = PrevY;
                    lines[I].X2 = X;
                    lines[I].Y2 = Y;
                }
                PrevX = X;
                PrevY = Y;
            }
            Percent += 1;
            Thread.Sleep(50);
            if (recording) 
            {
                //Add the canvas content as a BitmapFrame to the GifBitmapEncoder
                RenderTargetBitmap renderbmp = new RenderTargetBitmap((int)(canvas1.ActualWidth), (int)(canvas1.ActualHeight), 96, 96, PixelFormats.Default);
                renderbmp.Render(canvas1);
                MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp));
                Thread.Sleep(10);
            }
            if (Percent > 100)
            {
                Percent = 0;
                if (recording)
                {
                    // Create a FileStream to write the image to the file.
                    FileStream sw = new FileStream(ResultFileName, FileMode.Create);
                    if (sw != null)
                    {
                        MyEncoder.Save(sw);
                    }
                    Environment.Exit(0);
                }
            }
        }
    }
}
