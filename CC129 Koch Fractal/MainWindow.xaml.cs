using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Koch_Fractal
{
    public partial class MainWindow : Window
    {
        private delegate void RenderDelegate();
        private bool Rendering = false;
        private int My_Order;
        private int Total;
        private double Len;
        private Point Center;
        private Vector[] Linear;
        private Vector[] Path;
        private Vector[] Interpolated;
        private Line[] Lines;
        private Brush[] My_Brushes = { Brushes.Red, Brushes.DeepSkyBlue, Brushes.Lime, Brushes.Yellow };
        private bool Recording;
        private int WaitTime;
        private bool UseColors;
        private BitmapEncoder MyEncoder;
        private string ResultFileName = "KochMorphing.gif";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Height > Width + 67)
            {
                Width = Height - 67;
            }
            else
            {
                Height = Width + 67;
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                Rendering = true;
                BtnStart.Content = "STOP";
                RenderDelegate renderDelegate = new RenderDelegate(Render);
                Recording = CbRecording.IsChecked.Value;
                UseColors = CbUseColor.IsChecked.Value;
                if (Recording)
                {
                    MyEncoder = new GifBitmapEncoder();
                    WaitTime = 500;
                }
                else
                {
                    WaitTime = 100;
                }
                int maxOrder = int.Parse(TxtMaxOrder.Text);

                Vector[] PrevPath;
                //STEP 1: Zero order starts as 3 points in the center and morphs out to a Triangle.
                My_Order = 0;
                Center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
                Len = canvas1.ActualWidth / 1.5;
                Line L;
                Total = 4;
                Linear = new Vector[4];
                Path = new Vector[4];
                Interpolated = new Vector[4];
                Lines = new Line[4];
                //Calculate the points
                Linear[0] = new Vector(Center.X - 1, Center.Y + 1);
                Linear[1] = new Vector(Center.X,     Center.Y - 1);
                Linear[2] = new Vector(Center.X + 1, Center.Y + 1);
                Linear[3] = Linear[0];
                Interpolated[0] = Linear[0];
                Interpolated[1] = Linear[1];
                Interpolated[2] = Linear[2];
                Interpolated[3] = Linear[3];
                Path[0] = new Vector(Center.X - Len / 2, Center.Y + Len / (2 * Math.Sqrt(3)));
                Path[1] = new Vector(Center.X, Center.Y - Len / Math.Sqrt(3));
                Path[2] = new Vector(Center.X + Len / 2, Center.Y + Len / (2 * Math.Sqrt(3)));
                Path[3] = Path[0];
                //Draw the lines
                canvas1.Children.Clear();
                for (int i = 0; i < 3; i++)
                {
                    Lines[i] = new Line
                    {
                        StrokeThickness = 2.0,
                        X1 = Interpolated[i].X,
                        Y1 = Interpolated[i].Y,
                        X2 = Interpolated[(i + 1) % 2].X,
                        Y2 = Interpolated[(i + 1) % 2].Y
                    };
                    if (UseColors)
                    {
                        Lines[i].Stroke = My_Brushes[i];
                    }
                    else
                    {
                        Lines[i].Stroke = Brushes.DeepSkyBlue;
                    }
                    canvas1.Children.Add(Lines[i]);
                }
                //Morph the lines from Linear to Path
                for (double p = 0; p <= 1; p += 0.02)
                {
                    if (!Rendering) { return; }
                    for (int I = 0; I < Total; I++)
                    {
                        Interpolated[I] = Lerp(Linear[I], Path[I], p);
                    }
                    Dispatcher.Invoke(renderDelegate, DispatcherPriority.ApplicationIdle);
                    Thread.Sleep(WaitTime);
                    if (Recording) { AddFrame(canvas1); }
                }
                //STEP 2: Higher orders are formed by linear interpolating 2 points in each line 
                //        and adding a center point
                for (My_Order = 1; My_Order <= maxOrder; My_Order++)
                {
                    Len = canvas1.ActualWidth / (2 * Math.Pow(4, My_Order));
                    Total = 3 * (int)Math.Pow(4, My_Order) + 1;
                    PrevPath = Path;
                    Linear = new Vector[Total];
                    Path = new Vector[Total];
                    Interpolated = new Vector[Total];
                    Lines = new Line[Total];
                    //Calculate the new points
                    int counter = 0;
                    for (int I = 0; I < PrevPath.Length - 1; I++)
                    {
                        Linear[counter] = PrevPath[I];
                        counter++;
                        Linear[counter] = Lerp(PrevPath[I], PrevPath[(I + 1) % PrevPath.Length], 1.0 / 3);
                        counter++;
                        Linear[counter] = Lerp(PrevPath[I], PrevPath[(I + 1) % PrevPath.Length], 0.5);
                        counter++;
                        Linear[counter] = Lerp(PrevPath[I], PrevPath[(I + 1) % PrevPath.Length], 2.0 / 3);
                        counter++;
                    }
                    Linear[counter] = PrevPath[0];
                    for (int I = 0; I < Total; I++)
                    {
                        Path[I] = Linear[I];
                        if (I % 4 == 2 )
                        {
                            Vector v = Linear[I + 1] - Linear[I - 1];
                            Path[I] = Linear[I - 1] + RotateVector(v, -60);
                        }
                        Interpolated[I] = Linear[I];
                    }
                    canvas1.Children.Clear();
                    //Draw the initial lines
                    for (int I = 0; I < Interpolated.Length; I++)
                    {
                        L = new Line
                        {
                            StrokeThickness = 2.0,
                            X1 = Interpolated[I].X,
                            Y1 = Interpolated[I].Y,
                            X2 = Interpolated[(I + 1) % Interpolated.Length].X,
                            Y2 = Interpolated[(I + 1) % Interpolated.Length].Y
                        };
                        if (UseColors)
                        {
                            L.Stroke = My_Brushes[I & 3];
                        }
                        else
                        {
                            L.Stroke = Brushes.DeepSkyBlue;
                        }
                        Lines[I] = L;
                        canvas1.Children.Add(L);
                    }
                    //Morph the lines from Linear to Path
                    for (double p = 0; p <= 1; p += 0.02)
                    {
                        if (!Rendering) { return; }
                        for (int I = 0; I < Total; I++)
                        {
                            Interpolated[I] = Lerp(Linear[I], Path[I], p);
                        }
                        Dispatcher.Invoke(renderDelegate, DispatcherPriority.ApplicationIdle);
                        Thread.Sleep(WaitTime);
                        if (Recording) { AddFrame(canvas1); }
                    }
                }
                //Wait to show the last curve (needed for looping animated Gif images)
                if (Recording)
                {
                    for (int I = 0; I < 50; I++)
                    {
                        Dispatcher.Invoke(renderDelegate, DispatcherPriority.ApplicationIdle);
                        Thread.Sleep(WaitTime);
                        AddFrame(canvas1);
                    }
                    //Save the Frames as an animated GIF.
                    MakeGif();
                    Recording = false;
                }
                Rendering = false;
                BtnStart.Content = "START";
            }
            else
            {
                Rendering = false;
                BtnStart.Content = "START";
            }
        }

        static Vector RotateVector(Vector v, double degrees)
        {
            double radians = Math.PI * degrees / 180;
            Vector result = new Vector();
            result.X = v.X * Math.Cos(radians) - v.Y * Math.Sin(radians);
            result.Y = v.X * Math.Sin(radians) + v.Y * Math.Cos(radians);
            return result;
        }

        private Vector Lerp(Vector V1, Vector V2, double percent)
        {
            double X = V1.X + percent * (V2.X - V1.X);
            double Y = V1.Y + percent * (V2.Y - V1.Y);
            return new Vector(X, Y);
        }


        private void Render()
        {
            for (int I = 0; I < Interpolated.Length - 1; I++)
            {
                Lines[I].X1 = Interpolated[I].X;
                Lines[I].Y1 = Interpolated[I].Y;
                Lines[I].X2 = Interpolated[(I + 1) % Interpolated.Length].X;
                Lines[I].Y2 = Interpolated[(I + 1) % Interpolated.Length].Y;
            }
        }

        private void AddFrame(FrameworkElement Element)
        {
            //Render a FrameworkElement and add it as a Frame to a BitmapEncoder
            RenderTargetBitmap renderBmp = new RenderTargetBitmap((int)Element.ActualWidth, (int)Element.ActualHeight, 96, 96, PixelFormats.Default);
            renderBmp.Render(Element);
            MyEncoder.Frames.Add(BitmapFrame.Create(renderBmp));
        }

        private void MakeGif()
        {
            // Create a FileStream to write the image to the file.
            FileStream sw = new FileStream(ResultFileName, FileMode.Create);
            if (sw != null)
            {
                MyEncoder.Save(sw);
            }
        }
    }
}