using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _Matrix
{
    public partial class MainWindow : Window
    {
        private readonly int PointsNr = 8;
        private readonly Matrix[] points = new Matrix[8];
        private readonly Ellipse[] dots = new Ellipse[8];
        private readonly double dotSize = 8.0;
        private readonly Line[] lines = new Line[12];
        private Matrix ProjectionMatrix;
        private Matrix RotationX;
        private Matrix RotationY;
        private Matrix RotationZ;
        private readonly Matrix[] Projected = new Matrix[8];
        private double W;
        private double H;
        private double AngleX = 0.0;
        private double AngleY = 0.0;
        private double AngleZ = 0.0;
        private readonly double DeltaAngleX = 0.01;
        private readonly double DeltaAngleY = 0.015;
        private readonly double DeltaAngleZ = 0.01;
        private bool Perspective = false;
        private bool Rendering;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private double FrameCounter;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Ellipse El;
            Line L;
            W = canvas1.ActualWidth / 4;
            H = canvas1.ActualHeight / 4;
            //Create the corner points
            int index = 0;
            for (int X = -1; X <= 1; X += 2)
            {
                for (int Y = -1; Y <= 1; Y += 2)
                {
                    for (int Z = -1; Z <= 1; Z += 2)
                    {
                        points[index] = Matrix.FromArray(new double[] { W * X, W * Y, W * Z });
                        index++;
                    }
                }
            }
            //Create the corner dots of the cube
            for (int I=0; I < PointsNr; I++)
            {
                El = new Ellipse()
                {
                    Width = dotSize,
                    Height = dotSize,
                    Fill = Brushes.Black,
                    Visibility = Visibility.Hidden
                };
                dots[I] = El;
                canvas1.Children.Add(El);
            }
            //Create the edges of the cube
            for (int I = 0; I < 12; I++)
            {
                L = new Line()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                lines[I] = L;
                canvas1.Children.Add(L);
            }
            //Create the default Rotation Matrices
            RotationX = new Matrix(3);
            RotationY = new Matrix(3);
            RotationZ = new Matrix(3);
            //Create the default Projection Matrix
            ProjectionMatrix = Matrix.FromArray(new double[,] {{1, 0, 0}, {0, 1, 0}});
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!Rendering)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                //Show the dots
                for (int I = 0; I < PointsNr; I++)
                {
                    dots[I].Visibility = Visibility.Visible; 
                }
            }
            else
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering; ;
                Rendering = false;
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (FrameCounter++ == 0)
            {
                //Starting timing.
                stopwatch.Start();
            }

            //Determine frame rate in fps (frames per second).
            var frameRate = (long)(FrameCounter / stopwatch.Elapsed.TotalSeconds);
            if (frameRate > 0)
            {
                //Show the frame rate.
               Title = frameRate.ToString();
            }
            Matrix[] Rotated = new Matrix[8];
            double Zfactor = 1.0;
            //Create the Rotation Matrices
            RotationX = Matrix.FromArray(new double[,] 
                                         {{1, 0, 0 },
                                         {0, Math.Cos(AngleX), -Math.Sin(AngleX)},
                                         {0, Math.Sin(AngleX), Math.Cos(AngleX)}});
            RotationY = Matrix.FromArray(new double[,]
                                         {{Math.Cos(AngleY), 0, Math.Sin(AngleY)},
                                         {0, 1, 0},
                                         {-Math.Sin(AngleY), 0, Math.Cos(AngleY)}});
            RotationZ = Matrix.FromArray(new double[,]
                                         {{Math.Cos(AngleZ), -Math.Sin(AngleZ), 0},
                                         {Math.Sin(AngleZ), Math.Cos(AngleZ), 0},
                                         {0, 0, 1}});
            //Rotate and project the points on the Canvas
            for (int I = 0; I < points.Length; I++)
            {
                Rotated[I] = RotationX * points[I];
                Rotated[I] = RotationY * Rotated[I];
                Rotated[I] = RotationZ * Rotated[I];
                if (Perspective)
                {
                    Zfactor = 27 / (30 - Rotated[I].GetValue(2, 0) / 30);
                    ProjectionMatrix = Matrix.FromArray(new double[,] { { Zfactor, 0, 0 }, { 0, Zfactor, 0 } });
                }
                else
                {
                    ProjectionMatrix = Matrix.FromArray(new double[,] { { 1, 0, 0 }, { 0, 1, 0 } });
                }
                Projected[I] = ProjectionMatrix * Rotated[I];
            }
            //Draw the cube
            Draw();
            //Update the angles
            AngleX += DeltaAngleX;
            AngleY += DeltaAngleY;
            AngleZ += DeltaAngleZ;
        }

        private void Draw()
        {
            for (int I = 0; I < dots.Length; I++)
            {
                dots[I].SetValue(Canvas.LeftProperty, Projected[I].GetValue(0, 0) + 2 * W - dotSize / 2);
                dots[I].SetValue(Canvas.TopProperty, Projected[I].GetValue(1, 0) + 2 * H - dotSize / 2);
            }
            lines[0].X1 = Projected[0].GetValue(0, 0) + 2 * W;
            lines[0].Y1 = Projected[0].GetValue(1, 0) + 2 * H;
            lines[0].X2 = Projected[1].GetValue(0, 0) + 2 * W;
            lines[0].Y2 = Projected[1].GetValue(1, 0) + 2 * H;

            lines[1].X1 = Projected[0].GetValue(0, 0) + 2 * W;
            lines[1].Y1 = Projected[0].GetValue(1, 0) + 2 * H;
            lines[1].X2 = Projected[2].GetValue(0, 0) + 2 * W;
            lines[1].Y2 = Projected[2].GetValue(1, 0) + 2 * H;

            lines[2].X1 = Projected[3].GetValue(0, 0) + 2 * W;
            lines[2].Y1 = Projected[3].GetValue(1, 0) + 2 * H;
            lines[2].X2 = Projected[1].GetValue(0, 0) + 2 * W;
            lines[2].Y2 = Projected[1].GetValue(1, 0) + 2 * H;

            lines[3].X1 = Projected[3].GetValue(0, 0) + 2 * W;
            lines[3].Y1 = Projected[3].GetValue(1, 0) + 2 * H;
            lines[3].X2 = Projected[2].GetValue(0, 0) + 2 * W;
            lines[3].Y2 = Projected[2].GetValue(1, 0) + 2 * H;

            lines[4].X1 = Projected[4].GetValue(0, 0) + 2 * W;
            lines[4].Y1 = Projected[4].GetValue(1, 0) + 2 * H;
            lines[4].X2 = Projected[5].GetValue(0, 0) + 2 * W;
            lines[4].Y2 = Projected[5].GetValue(1, 0) + 2 * H;

            lines[5].X1 = Projected[4].GetValue(0, 0) + 2 * W;
            lines[5].Y1 = Projected[4].GetValue(1, 0) + 2 * H;
            lines[5].X2 = Projected[6].GetValue(0, 0) + 2 * W;
            lines[5].Y2 = Projected[6].GetValue(1, 0) + 2 * H;

            lines[6].X1 = Projected[7].GetValue(0, 0) + 2 * W;
            lines[6].Y1 = Projected[7].GetValue(1, 0) + 2 * H;
            lines[6].X2 = Projected[5].GetValue(0, 0) + 2 * W;
            lines[6].Y2 = Projected[5].GetValue(1, 0) + 2 * H;

            lines[7].X1 = Projected[7].GetValue(0, 0) + 2 * W;
            lines[7].Y1 = Projected[7].GetValue(1, 0) + 2 * H;
            lines[7].X2 = Projected[6].GetValue(0, 0) + 2 * W;
            lines[7].Y2 = Projected[6].GetValue(1, 0) + 2 * H;

            lines[8].X1 = Projected[0].GetValue(0, 0) + 2 * W;
            lines[8].Y1 = Projected[0].GetValue(1, 0) + 2 * H;
            lines[8].X2 = Projected[4].GetValue(0, 0) + 2 * W;
            lines[8].Y2 = Projected[4].GetValue(1, 0) + 2 * H;

            lines[9].X1 = Projected[1].GetValue(0, 0) + 2 * W;
            lines[9].Y1 = Projected[1].GetValue(1, 0) + 2 * H;
            lines[9].X2 = Projected[5].GetValue(0, 0) + 2 * W;
            lines[9].Y2 = Projected[5].GetValue(1, 0) + 2 * H;

            lines[10].X1 = Projected[2].GetValue(0, 0) + 2 * W;
            lines[10].Y1 = Projected[2].GetValue(1, 0) + 2 * H;
            lines[10].X2 = Projected[6].GetValue(0, 0) + 2 * W;
            lines[10].Y2 = Projected[6].GetValue(1, 0) + 2 * H;

            lines[11].X1 = Projected[3].GetValue(0, 0) + 2 * W;
            lines[11].Y1 = Projected[3].GetValue(1, 0) + 2 * H;
            lines[11].X2 = Projected[7].GetValue(0, 0) + 2 * W;
            lines[11].Y2 = Projected[7].GetValue(1, 0) + 2 * H;
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Perspective = !Perspective;
        }
    }
}
