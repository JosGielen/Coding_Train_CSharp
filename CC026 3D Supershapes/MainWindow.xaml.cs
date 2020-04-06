using System;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Media.Imaging;

namespace _3D_Supershape
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private EllipsoidGeometry my_Ellipsoid;
        private int frameNumber = 170;
        private readonly int MaxFrameNumber = 250;
        private readonly int Slices = 200;
        private bool started = false;
        private bool Recording;
        private int ImageNumber;
        private readonly string  ResultFileName = "3DSuperShapeGifLoop.gif";
        private readonly bool DeleteImages = true;
        private int WaitTime;

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (started) 
            {
                started = false;
                BtnStart.Content = "START";
                WaitTime = 10;
                return; 
            }
            else
            {
                Recording = CbRecording.IsChecked.Value;
                ImageNumber = 0;
                if (Recording)
                {
                    WaitTime = 100;
                }
                else
                {
                    WaitTime = 10;
                }
                my_Ellipsoid = new EllipsoidGeometry(3.5, Slices)
                {
                    InitialRotationAxis = new Vector3D(-30, 0, 0),
                    Position = new Vector3D(0, 0, 0),
                    AmbientMaterial = Color.FromRgb(0, 0, 0),
                    DiffuseMaterial = Color.FromRgb(0, 0, 0),
                    SpecularMaterial = Color.FromRgb(150, 150, 150),
                    Shininess = 20,
                    UseTexture = true,
                    TextureFile = Environment.CurrentDirectory + "\\rainbow3.jpg"
                };
                myScene.AddGeometry(my_Ellipsoid);
                Title = "3D Supershape";
                started = true;
                BtnStart.Content = "STOP";
                Render();
            }
        }

        private void Render()
        {
            while (started)
            {
                my_Ellipsoid.m2 = 5 * (Math.Sin(2 * Math.PI * frameNumber / MaxFrameNumber) + 1.3);
                my_Ellipsoid.m1 = 5 * (Math.Cos(2 * Math.PI * frameNumber / MaxFrameNumber) + 1.3);
                my_Ellipsoid.GenerateGeometry(myScene);
                myScene.Render();
                frameNumber ++;
                this.Dispatcher.Invoke(Wait, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                if (Recording) 
                { 
                    SaveImage(myScene); 
                }
                if (frameNumber >= MaxFrameNumber)
                {
                    frameNumber = 0;
                }
                if (Recording & frameNumber == 169)
                { 
                    MakeGif(15);
                    Recording = false;
                    CbRecording.IsChecked = false;
                }
            }
        }
        
        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Save the visual content of a FrameworkElement as a PNG image.
        /// </summary>
        /// <param name="Element">A WPF FrameworkElement</param>
        private void SaveImage(FrameworkElement Element)
        {
            DirectoryInfo dirInfo = Directory.CreateDirectory(Environment.CurrentDirectory + "\\output");
            string dir = dirInfo.FullName;
            string FileName = dir + "\\Image-" + ImageNumber.ToString("0000") + ".png";
            PngBitmapEncoder MyEncoder = new PngBitmapEncoder();
            RenderTargetBitmap renderBmp = new RenderTargetBitmap((int)Element.ActualWidth, (int)Element.ActualHeight, 96, 96, PixelFormats.Default);
            renderBmp.Render(Element);
            try
            {
                MyEncoder.Frames.Add(BitmapFrame.Create(renderBmp));
                FileStream sw = new FileStream(FileName, FileMode.Create);
                MyEncoder.Save(sw);
                sw.Close();
                ImageNumber++;
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
