using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fireworks
{
    public partial class MainWindow : Window
    {
        private WriteableBitmap Writebitmap;
        private byte[] PixelData;
        private int Stride;
        private int RocketCount = 15;
        private Vector gravity;
        private Rocket[] myFireworks;
        private List<Particle> my_Particles;
        private readonly int TrailLength = 20;
        private List<Color> myColors;
        private readonly Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myColors = new List<Color>();
            myColors.Add(Colors.White);
            myColors.Add(Colors.Red);
            myColors.Add(Colors.LimeGreen);
            myColors.Add(Colors.GreenYellow);
            myColors.Add(Colors.LightBlue);
            myColors.Add(Colors.Magenta);
            myColors.Add(Colors.Cyan);
            myColors.Add(Colors.Yellow);
            myColors.Add(Colors.Orange);
            myColors.Add(Colors.Pink);
            myColors.Add(Colors.LightGreen);
            Init();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Init()
        { 
            //Resize the Image Control
            int w = (int)canvas1.ActualWidth + 2;
            int h = (int)canvas1.ActualHeight + 2;
            Image1.Width = w;
            Image1.Height = h;
            Image1.Stretch = Stretch.Fill;
            //Make a writeable bitmap the size of the Image control
            Writebitmap = new WriteableBitmap(w + 1, h + 1, 96, 96, PixelFormats.Bgra32, null);
            Stride = (int)(Writebitmap.PixelWidth * Writebitmap.Format.BitsPerPixel / 8);
            PixelData = new byte[Stride * Writebitmap.PixelHeight];
            myFireworks = new Rocket[RocketCount];
            my_Particles = new List<Particle>();
            gravity = new Vector(0, 0.02);
            for (int I = 0; I < RocketCount; I++)
            {
                myFireworks[I] = new Rocket();
            }
            Rendering();
        }


        private void Rendering()
        {
            Vector p;
            Vector v;
            while (true)
            {
                //Relaunch exploded rockets
                for (int I = 0; I < RocketCount; I++)
                {
                    if (myFireworks[I].Exploded)
                    {
                        if (Rnd.NextDouble() > 0.97)
                        {
                            p = new Vector(Image1.Width * (0.8 * Rnd.NextDouble() +0.1), Image1.Height);
                            v = new Vector(0, -1 * Rnd.NextDouble() - 3);
                            myFireworks[I].Launch(p, v);
                        }
                    }
                }
                //Update live rockets
                for (int I = 0; I < RocketCount; I++)
                {
                    if (!myFireworks[I].Exploded)
                    {
                        myFireworks[I].Update(gravity);
                        if (myFireworks[I].Exploded) CreateFirework(myFireworks[I].Position);
                    }
                }
                //Update the particles
                for (int I = my_Particles.Count - 1; I >= 0; I--)
                {
                    if (my_Particles[I].LifeTime > 0)
                    {
                        my_Particles[I].Update(gravity);
                    }
                    else
                    {
                        my_Particles.Remove(my_Particles[I]);
                    }
                }
                //Draw The Rockets
                for (int I = 0; I < RocketCount; I++)
                {
                    if (!myFireworks[I].Exploded)
                    {
                        SetPixel4(myFireworks[I].Position, Colors.Red, PixelData, Stride);
                    }
                }
                //Draw The Particles
                for (int I = 0; I < my_Particles.Count; I++)
                {
                    if (my_Particles[I].LifeTime > 0)
                    {
                        SetPixel4(my_Particles[I].Position, my_Particles[I].Color , PixelData, Stride);
                    }
                }
                //Fade the pixels back to black
                byte Fadestep = (byte)(255 / TrailLength);
                for (int X = 0; X < Writebitmap.PixelWidth; X++)
                {
                    for (int Y = 0; Y < Writebitmap.PixelHeight; Y++)
                    {
                        FadePixel(X, Y, PixelData, Stride, Fadestep);
                    }
                }
                //Update the Image
                Int32Rect Intrect = new Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1);
                Writebitmap.WritePixels(Intrect, PixelData, Stride, 0);
                Image1.Source = Writebitmap;
                Dispatcher.Invoke(Wait, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }
        }

        private void CreateFirework(Vector position)
        {
            int particleCount = Rnd.Next(100) + 100;
            Color c = myColors[Rnd.Next(myColors.Count)];
            double angle;
            double t;
            double X;
            double Y;
            double Speed;
            Particle[] NewParticles = new Particle[1];
            int particletype = Rnd.Next(3);

            switch (particletype)
            {
                case 0:
                    //Make a firework with random particles
                    particleCount = Rnd.Next(200) + 200;
                    NewParticles = new Particle[particleCount];
                    for (int I = 0; I < particleCount; I++)
                    {
                        NewParticles[I] = new Particle(position, c, Rnd.Next(30) + 20);
                        angle = 2 * Math.PI * Rnd.NextDouble();
                        Speed = 3* Rnd.NextDouble() + 0.5;
                        X = Speed * Math.Cos(angle);
                        Y = Speed * Math.Sin(angle);
                        NewParticles[I].Velocity = new Vector(X,Y);
                    }
                    break;
                case 1:
                    //Make a ring firework
                    particleCount = Rnd.Next(100) + 100;
                    NewParticles = new Particle[particleCount];
                    Speed = 2*Rnd.NextDouble() + 2.0;
                    for (int I = 0; I < particleCount; I++)
                    {
                        NewParticles[I] = new Particle(position, c, Rnd.Next(30) + 20);
                        angle = 2 * Math.PI * Rnd.NextDouble();
                        X =  Speed * Math.Cos(angle);
                        Y =  Speed * Math.Sin(angle) + 0.5;
                        NewParticles[I].Velocity = new Vector(X, Y);
                    }
                    break;
                case 2:
                    //Make a heart firework
                    particleCount = Rnd.Next(100) + 100;
                    NewParticles = new Particle[particleCount];
                    t = 0.0;
                    Speed = 0.1 * Rnd.NextDouble() +0.1;
                    for (int I = 0; I < particleCount; I++)
                    {
                        NewParticles[I] = new Particle(position, c, Rnd.Next(30) + 20);
                        t += 2 * Math.PI / particleCount;
                        X = Speed * (16 * Math.Pow(Math.Sin(t), 3.0));
                        Y = -1 * Speed * (13 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t));
                        NewParticles[I].Velocity = new Vector(X, Y);
                    }
                    break;
            }
            for (int I = 0; I < particleCount; I++)
            {
                my_Particles.Add(NewParticles[I]);
            }
        }

        private void SetPixel1(Vector pos, Color c , byte[] buffer , int PixStride)
        {
            if (pos.X < 0 | pos.X > Image1.Width | pos.Y < 0 | pos.Y > Image1.Height) return;
            //SetPixel allowing for Alpha.
            int xIndex = (int)pos.X * 4;
            int yIndex = (int)pos.Y * PixStride;
            buffer[xIndex + yIndex] = c.B;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.R;
            buffer[xIndex + yIndex + 3] = c.A;
        }

        private void SetPixel4(Vector pos, Color c, byte[] buffer, int PixStride)
        {
            if (pos.X < 1 | pos.X > Image1.Width - 1 | pos.Y < 1 | pos.Y > Image1.Height - 1) return;
            //SetPixel allowing for Alpha.
            int xIndex = (int)pos.X * 4;
            int yIndex = (int)pos.Y * PixStride;
            buffer[xIndex + yIndex] = c.B;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.R;
            buffer[xIndex + yIndex + 3] = c.A;
            xIndex = (int)(pos.X) * 4;
            yIndex = (int)(pos.Y + 1) * PixStride;
            buffer[xIndex + yIndex] = c.B;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.R;
            buffer[xIndex + yIndex + 3] = c.A;
            xIndex = (int)(pos.X + 1) * 4;
            yIndex = (int)(pos.Y) * PixStride;
            buffer[xIndex + yIndex] = c.B;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.R;
            buffer[xIndex + yIndex + 3] = c.A;
            xIndex = (int)(pos.X + 1) * 4;
            yIndex = (int)(pos.Y + 1) * PixStride;
            buffer[xIndex + yIndex] = c.B;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.R;
            buffer[xIndex + yIndex + 3] = c.A;
        }

        private void SetPixelBlend(Vector pos, Color c , byte[] buffer , int PixStride)
        {
            //SetPixel allowing for Alpha and color blending
            if (pos.X < 0 | pos.X > Image1.Width | pos.Y < 0 | pos.Y > Image1.Height) return;
            int xIndex = (int)pos.X * 4;
            int yIndex = (int)pos.Y * PixStride;
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

        private void FadePixel(int X , int Y , byte[] buffer , int PixStride , byte fadeStep )
        {
            int xIndex = X * 4;
            int yIndex = Y * PixStride;
            double A = buffer[xIndex + yIndex + 3];
            A -= fadeStep;
            if (A < 0) A = 0;
            buffer[xIndex + yIndex + 3] = (byte)A;
        }

        private void Wait()
        {
            Thread.Sleep(5);
        }
    }
}
