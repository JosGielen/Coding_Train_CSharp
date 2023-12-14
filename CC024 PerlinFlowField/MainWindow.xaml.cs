using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PerlinFlowField
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private Settings settingForm;
        private bool App_Started = false;
        private double CellSize = 15;
        private double ForceMag = 0.3;
        private double XYChange = 0.1;
        private double ZChange = 0.001;
        private int ParticleCount = 10000;
        private double maxSpeed = 1.0;
        private byte TrailLength = 75;
        private bool RandomSpawn = true;
        private bool UseColor = false;
        private int cols;
        private int rows;
        private Vector[,] FlowField;
        private List<Particle> Particles;
        private WriteableBitmap Writebitmap;
        private byte[] PixelData;
        private int Stride;
        private int colorCount = 360;
        private List<Color> my_Colors;
        private double ZOff = 0.0;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow continuous.cpl");
            my_Colors = pal.GetColors(colorCount);
            ShowSettingForm();
        }

        public void Start()
        {
            GetParameters();
            Init();
            if (!App_Started)
            {
                App_Started = true;
                if (settingForm != null)
                {
                    settingForm.BtnStart.Content = "STOP";
                }
                while (App_Started)
                {
                    ZOff += ZChange;
                    DrawParticles();
                    Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new WaitDelegate(Wait), 10);
                }
            }
            else
            {
                App_Started = false;
                if (settingForm != null)
                {
                    settingForm.BtnStart.Content = "START";
                }
            }
        }

        private void Init()
        {
            //Resize the Flowfield
            cols = (int)(canvas1.ActualWidth / CellSize);
            rows = (int)(canvas1.ActualHeight / CellSize);
            FlowField = new Vector[rows, cols];
            //Create the Particles
            Particles = new List<Particle>();
            Particle p;
            for (int I = 0; I < ParticleCount; I++)
            {
                p = new Particle(new Vector(canvas1.ActualWidth * Rnd.NextDouble(), canvas1.ActualHeight * Rnd.NextDouble()), new Vector(0, 0), maxSpeed);
                Particles.Add(p);
            }
            //Resize the Image Control
            int w = (int)(canvas1.ActualWidth) + 2;
            int h = (int)(canvas1.ActualHeight) + 2;
            Image1.Width = w;
            Image1.Height = h;
            Image1.Stretch = Stretch.Fill;
            //Make a writeable bitmap the size of the Image control
            Writebitmap = new WriteableBitmap(w + 1, h + 1, 96, 96, PixelFormats.Bgra32, null);
            Stride = (int)(Writebitmap.PixelWidth * Writebitmap.Format.BitsPerPixel / 8.0);
            PixelData = new byte[Stride * Writebitmap.PixelHeight];
        }

        private void ShowSettingForm()
        {
            if (settingForm == null)
            {
                settingForm = new Settings(this);
                settingForm.Show();
                settingForm.Left = Left + Width - 15;
                settingForm.Top = Top;
                settingForm.CellSize = CellSize;
                settingForm.MaxForce = ForceMag;
                settingForm.XYChange = XYChange;
                settingForm.ZChange = ZChange;
                settingForm.ParticleCount = ParticleCount;
                settingForm.Speed = maxSpeed;
                settingForm.TrailLength = TrailLength;
                settingForm.RandomSpawn = RandomSpawn;
                settingForm.UseColor = UseColor;
            }
            else
            {
                settingForm.Show();
            }
            settingForm.Update();
        }

        public void GetParameters()
        {
            if (settingForm != null)
            {
                CellSize = settingForm.CellSize;
                ForceMag = settingForm.MaxForce;
                XYChange = settingForm.XYChange;
                ZChange = settingForm.ZChange;
                ParticleCount = settingForm.ParticleCount;
                maxSpeed = settingForm.Speed;
                TrailLength = settingForm.TrailLength;
                RandomSpawn = settingForm.RandomSpawn;
                UseColor = settingForm.UseColor;
            }
        }

        private void DrawParticles()
        {
            double xOff = 0;
            double yOff = 0;
            int row;
            int col;
            double angle;
            Vector V;
            double XN;
            double YN;
            Vector pos;
            Vector vel;
            //Calculate the FlowField
            for (row = 0; row < rows; row++)
            {
                xOff = 0.0;
                for (col = 0; col < cols; col++)
                {
                    angle = 4 * Math.PI * PerlinNoise.Noise3D(xOff, yOff, ZOff);
                    V = new Vector(Math.Cos(angle), Math.Sin(angle));
                    V = ForceMag * V;
                    FlowField[row, col] = V;
                    xOff += XYChange;
                }
                yOff += XYChange;
            }
            //Fade the pixels back to white
            byte Fadestep = 1;
            if (UseColor)
            {
                Fadestep = (byte)(255.0 / TrailLength);
            }
            for (int X = 0; X < Writebitmap.PixelWidth; X++)
            {
                for (int Y = 0; Y < Writebitmap.PixelHeight; Y++)
                {
                    FadePixel(X, Y, PixelData, Stride, Fadestep);
                }
            }
            //Draw the particles
            for (int I = 0; I < Particles.Count; I++)
            {
                row = (int)(Particles[I].Position.Y / CellSize) - 1;
                col = (int)(Particles[I].Position.X / CellSize) - 1;
                if (row < 0) row = 0;
                if (col < 0) col = 0;

                V = FlowField[row, col];
                Particles[I].ApplyForce(V);
                Particles[I].Update();
                //When the particles leave the field:
                if (RandomSpawn)
                {
                    //Reset at random position
                    XN = (canvas1.ActualWidth) * Rnd.NextDouble();
                    YN = (canvas1.ActualHeight) * Rnd.NextDouble();
                    row = (int)(YN / CellSize) - 1;
                    col = (int)(XN / CellSize) - 1;
                    if (row < 0) row = 0;
                    if (col < 0) col = 0;
                    pos = new Vector(XN, YN);
                    vel = FlowField[row, col];
                    if (Particles[I].Position.X < 0)
                    {
                        Particles[I].Position = pos;
                        Particles[I].Velocity = vel;
                    }
                    else if (Particles[I].Position.X > canvas1.ActualWidth)
                    {
                        Particles[I].Position = pos;
                        Particles[I].Velocity = vel;
                    }
                    if (Particles[I].Position.Y < 0)
                    {
                        Particles[I].Position = pos;
                        Particles[I].Velocity = vel;
                    }
                    else if (Particles[I].Position.Y > canvas1.ActualHeight)
                    {
                        Particles[I].Position = pos;
                        Particles[I].Velocity = vel;
                    }
                }
                else
                {
                    //Wrap to the opposite side
                    if (Particles[I].Position.X < 0)
                    {
                        Particles[I].Position = new Vector(canvas1.ActualWidth, Particles[I].Position.Y);
                    }
                    else if (Particles[I].Position.X > canvas1.ActualWidth)
                    {
                        Particles[I].Position = new Vector(0, Particles[I].Position.Y);
                    }
                    if (Particles[I].Position.Y < 0)
                    {
                        Particles[I].Position = new Vector(Particles[I].Position.X, canvas1.ActualHeight);
                    }
                    else if (Particles[I].Position.Y > canvas1.ActualHeight)
                    {
                        Particles[I].Position = new Vector(Particles[I].Position.X, 0);
                    }
                }
                if (UseColor)
                {
                    int index = (int)(Vector.AngleBetween(Particles[I].Velocity, new Vector(1, 0)) + 180) % colorCount;
                    SetPixel((int)(Particles[I].Position.X), (int)(Particles[I].Position.Y), my_Colors[index], PixelData, Stride);
                }
                else
                {
                    SetPixelBlend((int)(Particles[I].Position.X), (int)(Particles[I].Position.Y), Color.FromArgb(TrailLength, 0, 0, 0), PixelData, Stride);
                }
            }
            //Update the Image
            Int32Rect Intrect = new Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1);
            Writebitmap.WritePixels(Intrect, PixelData, Stride, 0);
            Image1.Source = Writebitmap;
        }

        private void SetPixel(int X, int Y, Color c, byte[] buffer, int PixStride)
        {
            //SetPixel allowing for Alpha and color blending
            int xIndex = X * 4;
            int yIndex = Y * PixStride;
            buffer[xIndex + yIndex] = c.B;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.R;
            buffer[xIndex + yIndex + 3] = c.A;
        }

        private void SetPixelBlend(int X, int Y, Color c, byte[] buffer, int PixStride)
        {
            //SetPixel allowing for Alpha and color blending
            int xIndex = X * 4;
            int yIndex = Y * PixStride;
            int B = buffer[xIndex + yIndex] + c.B;
            int G = buffer[xIndex + yIndex + 1] + c.G;
            int R = buffer[xIndex + yIndex + 2] + c.R;
            int A = buffer[xIndex + yIndex + 3] + c.A;
            if (B > 255) B = 255;
            if (G > 255) G = 255;
            if (R > 255) R = 255;
            if (A > 255) A = 255;
            buffer[xIndex + yIndex] = (byte)B;
            buffer[xIndex + yIndex + 1] = (byte)G;
            buffer[xIndex + yIndex + 2] = (byte)R;
            buffer[xIndex + yIndex + 3] = (byte)A;
        }

        private void FadePixel(int X, int Y, byte[] buffer, int PixStride, byte fadeStep)
        {
            int xIndex = X * 4;
            int yIndex = Y * PixStride;
            double A = buffer[xIndex + yIndex + 3];
            A -= fadeStep;
            if (A < 0) A = 0;
            buffer[xIndex + yIndex + 3] = (byte)A;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsLoaded) return;
            Init();
            settingForm.Left = Left + ActualWidth - 15;
            settingForm.Top = Top;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            settingForm.Left = Left + ActualWidth - 15;
            settingForm.Top = Top;
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}