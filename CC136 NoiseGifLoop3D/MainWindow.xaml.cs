using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace NoiseGifLoop3D
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private int WaitTime = 100;
        private NoiseEllipsoidGeometry myEllipsoid;
        private int percent;
        private bool recording = false;
        private string ResultFileName = "OpenSimplexNoiseGifLoop.gif";
        BitmapEncoder MyEncoder = new GifBitmapEncoder();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (recording)
            {
                Width = 380;
                Height = 350;
            }
            else
            {
                Width = 680;
                Height = 650;
            }
            myEllipsoid = new NoiseEllipsoidGeometry(7, 7, 7, 100, 100)
            {
                InitialRotationAxis = new Vector3D(90, 0, 0),
                Position = new Vector3D(0, 0, 0),
                AmbientMaterial = Color.FromRgb(0, 0, 0),
                DiffuseMaterial = Color.FromRgb(0, 0, 0),
                SpecularMaterial = Color.FromRgb(200, 200, 200),
                Shininess = 20,
                UseTexture = true,
                TextureFile = Environment.CurrentDirectory + "\\rainbow3.jpg"
            };
            myScene.AddGeometry(myEllipsoid);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            myEllipsoid.GenerateGeometry(myScene);
            //Render the scene.
            myScene.Render();
            Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
            percent += 1;
            myEllipsoid.Percent = percent;
            if (recording)
            {
                //Add the canvas content as a BitmapFrame to the GifBitmapEncoder
                RenderTargetBitmap renderbmp = new RenderTargetBitmap((int)(myScene.ActualWidth), (int)(myScene.ActualHeight), 96, 96, PixelFormats.Default);
                renderbmp.Render(myScene);
                MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp));
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), WaitTime);
            }
            Title = "Percent = " + percent.ToString();
            if (percent >= 100)
            {
                percent = 0;
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

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myScene = null;
            Environment.Exit(0);
        }
    }
}