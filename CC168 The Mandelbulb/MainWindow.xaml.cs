using System;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Media.Imaging;

namespace _MandelBulb
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private int frameNumber = 0;
        private readonly int MaxFrameNumber = 360;
        private bool started = false;
        private bool Recording;
        private int ImageNumber;
        private readonly string ResultFileName = "MandelBulbGifLoop.gif";
        private readonly bool DeleteImages = true;
        private int WaitTime;
        //MandelBulb Parameters
        private MandelBulbGeometry My_MandelBulb;
        private readonly double my_Size = 200;
        private readonly double my_StepSize = 2;
        private readonly int my_Power = 8;
        private readonly int my_MaxIter = 100;
        private readonly double my_Isolevel = 16;

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
                My_MandelBulb = new MandelBulbGeometry(my_Size, my_StepSize, my_Power, my_MaxIter, my_Isolevel)
                {
                    InitialRotationAxis = new Vector3D(0, 0, 0),
                    Position = new Vector3D(0, 0, 0),
                    AmbientMaterial = Color.FromRgb(0, 0, 0),
                    DiffuseMaterial = Color.FromRgb(0, 0, 0),
                    SpecularMaterial = Color.FromRgb(150, 150, 150),
                    Shininess = 10,
                    UseTexture = true,
                    TextureFile = Environment.CurrentDirectory + "\\rainbow3.jpg",
                    DrawMode = DrawMode.Lines 
                };
                //Allow Camera movement. Remove the next line for a fixed camera
                myScene.Camera = new TrackballCamera(new Vector3D(0, 0, 250), new Vector3D(0, 0, 0), new Vector3D(0, 1, 0));
                myScene.AddGeometry(My_MandelBulb);

                //Creating the Geometry here makes it fixed!!!
                My_MandelBulb.GenerateGeometry(myScene);

                Title = "MandelBulb";
                started = true;
                BtnStart.Content = "STOP";
                Render();
            }
        }

        private void Render()
        {
            while (started)
            {

                //TODO: Rotate the MandelBulb? or Increase the order or iterations or .....

                //Creating the Geometry here allows modifying it (e.g. rotation etc....
                //My_MandelBulb.GenerateGeometry(myScene);

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
