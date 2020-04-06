using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;

namespace Animated_Hilbert_Curve
{  
    public partial class MainWindow : Window
    {
        private delegate void RenderDelegate();
        private bool Rendering = false;
        private int My_Order;
        private int Total;
        private double Len;
        private Vector[] Linear;
        private Vector[] Path;
        private Vector[] Interpolated;
        private Line[] Lines;
        private Brush[] My_Brushes = { Brushes.Red, Brushes.DeepSkyBlue, Brushes.Lime, Brushes.Yellow };
        private bool Recording;
        private bool DeleteImages = true;
        private int WaitTime;
        private bool UseColors;
        private int FrameNumber = 0;
        private string ResultFileName = "HilbertMorph.gif";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(Height > Width + 67)
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
                Recording = CbRecording.IsChecked.Value ;
                UseColors = CbUseColor.IsChecked.Value;
                Vector[] PrevPath;
                //STEP 1: Zero order starts as a point in the center and morphs out to order 1.
                My_Order = 1;
                Len = canvas1.ActualWidth / Math.Pow(2, My_Order);
                Total = (int)Math.Pow(2, 2 * My_Order);
                Line L;
                Array.Resize(ref Linear, Total);
                Array.Resize(ref Path, Total);
                Array.Resize(ref Interpolated, Total);
                Array.Resize(ref Lines, Total);
                if (Recording)
                {
                    WaitTime = 500;
                }
                else
                {
                    WaitTime = 100;
                }
                int maxOrder = int.Parse(TxtMaxOrder.Text);
                //Calculate the points
                for(int I=0; I < Total; I++)
                {
                    Linear[I] = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
                    Path[I] = Hilbert(I);
                    Interpolated[I] = Linear[I];
                }
                //Draw the lines
                canvas1.Children.Clear();
                for(int I = 0; I < Interpolated.Length - 1; I++)
                {
                    L = new Line
                    {
                        StrokeThickness = 2.0,
                        X1 = Interpolated[I].X,
                        Y1 = Interpolated[I].Y,
                        X2 = Interpolated[I + 1].X,
                        Y2 = Interpolated[I + 1].Y
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
                for(double p = 0; p <= 1; p += 0.04)
                {
                    if (!Rendering) { return; }
                    for(int I=0; I < Total; I++)
                    {
                        Interpolated[I] = Lerp(Linear[I], Path[I], p);
                    }
                    Dispatcher.Invoke(renderDelegate, DispatcherPriority.ApplicationIdle);
                    Thread.Sleep(WaitTime);
                    if (Recording) { SaveImage(canvas1); }
                }
                //STEP 2: Higher orders are formed by linear interpolating 4 points in each line 
                //        (except in the connection lines)
                for(My_Order = 2; My_Order <= maxOrder; My_Order++)
                {
                    Len = canvas1.ActualWidth / Math.Pow(2, My_Order);
                    Total = (int)Math.Pow(2, 2 * My_Order);
                    PrevPath = Path; 
                    Array.Resize(ref Linear, Total);
                    Array.Resize(ref Path, Total);
                    Array.Resize(ref Interpolated, Total);
                    Array.Resize(ref Lines, Total);
                    //Calculate 4 points between the points of the Previous Path 
                    //But not between the 3rd and 4th point (= connecting line)
                    int index;
                    int counter = 0;
                    for(int I = 0; I < PrevPath.Length-1; I++)
                    {
                        index = I & 3;
                        if (index < 3)
                        {
                            for(double p = 0; p <= 0.8; p += 0.2)
                            {
                                Linear[counter] = Lerp(PrevPath[I], PrevPath[I + 1], p);
                                counter++;
                            }
                        }
                        else if(index==3)
                        {
                            Linear[counter] = PrevPath[I];
                            counter++;
                        }
                    }
                    Linear[counter] = PrevPath.Last();
                    for(int I = 0; I < Total; I++)
                    {
                        Path[I] = Hilbert(I);
                        Interpolated[I] = Linear[I];
                    }
                    canvas1.Children.Clear();
                    //Draw the initial lines
                    for(int I = 0; I < Interpolated.Length-1; I++)
                    {
                        L = new Line
                        {
                            StrokeThickness = 2.0,
                            X1 = Interpolated[I].X,
                            Y1 = Interpolated[I].Y,
                            X2 = Interpolated[I + 1].X,
                            Y2 = Interpolated[I + 1].Y
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
                    for (double p = 0; p <= 1; p += 0.04)
                    {
                        if (!Rendering) { return; }
                        for (int I = 0; I < Total; I++)
                        {
                            Interpolated[I] = Lerp(Linear[I], Path[I], p);
                        }
                        Dispatcher.Invoke(renderDelegate, DispatcherPriority.ApplicationIdle);
                        Thread.Sleep(WaitTime);
                        if (Recording) { SaveImage(canvas1); }
                    }
                }
                //Wait to show the last curve (needed for looping animated Gif images)
                if (Recording )
                {
                    for (int I = 0; I < 40; I++)
                    {
                        Dispatcher.Invoke(renderDelegate, DispatcherPriority.ApplicationIdle);
                        Thread.Sleep(WaitTime);
                        SaveImage(canvas1);
                    }
                    //Convert the PNG images into an animated GIF.
                    MakeGif(15);
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

        private Vector Lerp(Vector V1 , Vector V2, double percent)
        {
            double X = V1.X + percent * (V2.X - V1.X);
            double Y = V1.Y + percent * (V2.Y - V1.Y);
            return new Vector(X, Y);
        }


        private void Render()
        {
            for (int I=0; I < Interpolated.Length - 1; I++)
            {
                Lines[I].X1 = Interpolated[I].X;
                Lines[I].Y1 = Interpolated[I].Y;
                Lines[I].X2 = Interpolated[I + 1].X;
                Lines[I].Y2 = Interpolated[I + 1].Y;
            }
        }

        private Vector Hilbert(int Nr)
        {
            int Index;
            int Offset;
            double Dummy;
            Vector[] V = { new Vector(0, 0), 
                           new Vector(0, 1),
                           new Vector(1, 1),
                           new Vector(1, 0) 
                         };
            Vector Result;
            Index = Nr & 3;
            Result = V[Index];
            for(int I=1; I < My_Order;I++)
            {
                Offset = (int)Math.Pow(2, I);
                Nr >>= 2;
                Index = Nr & 3;
                if (Index == 0)
                {
                    Dummy = Result.X;
                    Result.X = Result.Y;
                    Result.Y = Dummy;
                }
                if (Index == 1)
                {
                    Result.Y += Offset;
                }
                if (Index == 2)
                {
                    Result.X += Offset;
                    Result.Y += Offset;
                }
                if (Index == 3)
                {
                    Dummy = Offset - 1 - Result.X;
                    Result.X = Offset - 1 - Result.Y;
                    Result.Y = Dummy;
                    Result.X += Offset;
                }
            }
            Result.X = Len / 2 + Len * Result.X;
            Result.Y = Len / 2 + Len * Result.Y;
            return Result;
        }

        /// <summary>
        /// Save the visual content of a FrameworkElement as a PNG image.
        /// </summary>
        /// <param name="Element">A WPF FrameworkElement</param>
        private void SaveImage(FrameworkElement Element)
        {
            DirectoryInfo dirInfo = Directory.CreateDirectory(Environment.CurrentDirectory + "\\output");
            string dir = dirInfo.FullName;
            string FileName = dir + "\\Image-" + FrameNumber.ToString("0000") + ".png";
            PngBitmapEncoder MyEncoder = new PngBitmapEncoder();
            RenderTargetBitmap renderBmp = new RenderTargetBitmap((int)Element.ActualWidth, (int)Element.ActualHeight, 96, 96, PixelFormats.Default);
            renderBmp.Render(Element);
            try
            {
                MyEncoder.Frames.Add(BitmapFrame.Create(renderBmp));
                FileStream sw = new FileStream(FileName, FileMode.Create);
                MyEncoder.Save(sw);
                sw.Close();
                FrameNumber += 1;
            }
            catch (Exception)
            {
                MessageBox.Show("The Image could not be saved.", "Animated Hilbert Curve Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Create an animated Gif from PNG images with ffmpeg.exe.
        /// </summary>
        /// <param name="frameRate">The desired framerate of the animated Gif</param>
        private void MakeGif(int frameRate)
        {
            string prog = Environment.CurrentDirectory + "\\ffmpeg.exe";
            string args = " -framerate " + frameRate.ToString() + " -i output\\Image-%4d.png " + ResultFileName;
            Process p = Process.Start(prog, args);
            p.WaitForExit();
            //Delete the image files
            if (DeleteImages)
            {
                foreach (string f in Directory.GetFiles(Environment.CurrentDirectory + "\\output"))
                {
                    if (System.IO.Path.GetExtension(f) == ".png")
                    {
                        File.Delete(f);
                    }
                }
            }
        }
    }
}
