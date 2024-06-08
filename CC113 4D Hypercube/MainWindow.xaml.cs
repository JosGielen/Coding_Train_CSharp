using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _4D_HyperCube
{
    public partial class MainWindow : Window
    {
        private readonly int PointsNr = 16;
        private readonly double dotSize = 8.0;
        private Point Center;
        private double EdgeLength;
        private Matrix[] points;
        private Ellipse[] dots;
        private Line[] lines;
        private Matrix ProjectionMatrix2D;
        private Matrix ProjectionMatrix3D;
        private Matrix[] Projected3D;
        private Matrix[] Projected2D;
        private double AngleX = 0.0;
        private double AngleY = 0.0;
        private double AngleZ = 0.0;
        private double AngleW = 0.0;
        private Matrix RotationX;
        private Matrix RotationY;
        private Matrix RotationZ;
        private Matrix RotationXW;
        private Matrix RotationYW;
        private Matrix RotationZW;
        private readonly double DeltaAngleX = 0.01;
        private readonly double DeltaAngleY = 0.01;
        private readonly double DeltaAngleZ = 0.01;
        private readonly double DeltaAngleW = 0.01;
        private bool Perspective = true;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private double FrameCounter;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Ellipse El;
            Line L;
            points = new Matrix[16];
            dots = new Ellipse[16];
            lines = new Line[32];
            Projected3D = new Matrix[16];
            Projected2D = new Matrix[16];
            EdgeLength = canvas1.ActualWidth / 3;
            Center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            //Create the corner points
            int index = 0;
            for (int X = -1; X <= 1; X += 2)
            {
                for (int Y = -1; Y <= 1; Y += 2)
                {
                    for (int Z = -1; Z <= 1; Z += 2)
                    {
                        for (int W = -1; W <= 1; W += 2)
                        {
                            points[index] = Matrix.FromArray(new double[] { X, Y, Z, W });
                            index++;
                        }
                    }
                }
            }
            //Create the corner dots of the cube
            for (int I = 0; I < PointsNr; I++)
            {
                El = new Ellipse()
                {
                    Width = dotSize,
                    Height = dotSize,
                    Fill = Brushes.Black,
                };
                dots[I] = El;
                canvas1.Children.Add(El);
            }
            //Create the edges of the cube
            for (int I = 0; I < 32; I++)
            {
                L = new Line()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                lines[I] = L;
                canvas1.Children.Add(L);
            }
            //Create the Rotation Matrices
            //RotationX = new Matrix(4);
            //RotationY = new Matrix(4);
            //RotationZ = new Matrix(4);
            //RotationXW = new Matrix(4);
            //RotationYW = new Matrix(4);
            //RotationZW = new Matrix(4);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
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
            Matrix[] Rotated = new Matrix[16];
            double Zfactor;
            double Wfactor;
            //Create the Rotation Matrices
            RotationX = Matrix.FromArray(new double[,]
                                         {{1, 0, 0, 0},
                                         {0, Math.Cos(AngleX), -Math.Sin(AngleX), 0},
                                         {0, Math.Sin(AngleX), Math.Cos(AngleX), 0},
                                         {0, 0, 0, 1}});
            RotationY = Matrix.FromArray(new double[,]
                                         {{Math.Cos(AngleY), 0, Math.Sin(AngleY), 0},
                                         {0, 1, 0, 0},
                                         {-Math.Sin(AngleY), 0, Math.Cos(AngleY), 0},
                                         { 0, 0, 0, 1}});
            RotationZ = Matrix.FromArray(new double[,]
                                         {{Math.Cos(AngleZ), -Math.Sin(AngleZ), 0, 0},
                                         {Math.Sin(AngleZ), Math.Cos(AngleZ), 0, 0},
                                         {0, 0, 1, 0},
                                         {0, 0, 0, 1}});
            RotationXW = Matrix.FromArray(new double[,]
                                         {{Math.Cos(AngleW), 0, 0, -Math.Sin(AngleW)},
                                         {0, 1, 0, 0},
                                         {0, 0, 1, 0},
                                         {Math.Sin(AngleW), 0, 0, Math.Cos(AngleW)}});
            RotationYW = Matrix.FromArray(new double[,]
                                         {{1, 0, 0, 0},
                                         {0, Math.Cos(AngleW), 0, -Math.Sin(AngleW)},
                                         {0, 0, 1, 0},
                                         {0, Math.Sin(AngleW), 0, Math.Cos(AngleW)}});
            RotationZW = Matrix.FromArray(new double[,]
                                         {{1, 0, 0, 0},
                                         {0, 1, 0, 0},
                                         {0, 0, Math.Cos(AngleW), -Math.Sin(AngleW)},
                                         {0, 0, Math.Sin(AngleW), Math.Cos(AngleW)}});

            //Rotate and project the points on the Canvas
            for (int I = 0; I < points.Length; I++)
            {
                //*** No Rotation
                Rotated[I] = points[I];
                //*** Possible Rotations
                if (CBX.IsChecked == true) { Rotated[I] = RotationX * Rotated[I]; }
                if (CBY.IsChecked == true) { Rotated[I] = RotationY * Rotated[I]; }
                if (CBZ.IsChecked == true) { Rotated[I] = RotationZ * Rotated[I]; }
                if (CBXW.IsChecked == true) { Rotated[I] = RotationXW * Rotated[I]; }
                if (CBYW.IsChecked == true) { Rotated[I] = RotationYW * Rotated[I]; }
                if (CBZW.IsChecked == true) { Rotated[I] = RotationZW * Rotated[I]; }

                //*** Project the rotated 4D points into 3D.
                if (Perspective)
                {
                    Wfactor = 1 / (2 - Rotated[I].GetValue(3, 0));
                    ProjectionMatrix3D = Matrix.FromArray(new double[,] { { Wfactor, 0, 0, 0 }, { 0, Wfactor, 0, 0 }, { 0, 0, Wfactor, 0 } });
                }
                else
                {
                    ProjectionMatrix3D = Matrix.FromArray(new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 } });
                }
                Projected3D[I] = ProjectionMatrix3D * Rotated[I];
                //*** Project the 3D projected points into 2D.
                if (Perspective)
                {
                    Zfactor = 1 / (2 - Projected3D[I].GetValue(2, 0) / 5);
                    ProjectionMatrix2D = Matrix.FromArray(new double[,] { { Zfactor, 0, 0 }, { 0, Zfactor, 0 } });
                }
                else
                {
                    ProjectionMatrix2D = Matrix.FromArray(new double[,] { { 1, 0, 0 }, { 0, 1, 0 } });
                }
                Projected2D[I] = ProjectionMatrix2D * Projected3D[I];
                Projected2D[I].MultiplyScalar(EdgeLength);
            }
            //Draw the cube
            Draw();
            //Update the angles
            AngleX += DeltaAngleX;
            AngleY += DeltaAngleY;
            AngleZ += DeltaAngleZ;
            AngleW += DeltaAngleW;
        }

        private void Draw()
        {
            //Draw the corner points
            for (int I = 0; I < PointsNr; I++)
            {
                dots[I].SetValue(Canvas.LeftProperty, Center.X + Projected2D[I].GetValue(0, 0) - dotSize / 2);
                dots[I].SetValue(Canvas.TopProperty, Center.Y + Projected2D[I].GetValue(1, 0) - dotSize / 2);
            }
            //Draw the Edges
            int diff;
            int index = 0;
            for (int I = 0; I < PointsNr; I++)
            {
                for (int J = I + 1; J < PointsNr; J++)
                {
                    diff = 0;
                    for (int K = 0; K < 4; K++)
                    {
                        if (points[I].GetValue(K,0) != points[J].GetValue(K,0)) { diff += 1; }
                    }
                    if (diff == 1)
                    {
                        lines[index].X1 = Center.X + Projected2D[I].GetValue(0, 0);
                        lines[index].Y1 = Center.Y + Projected2D[I].GetValue(1, 0);
                        lines[index].X2 = Center.X + Projected2D[J].GetValue(0, 0);
                        lines[index].Y2 = Center.Y + Projected2D[J].GetValue(1, 0);
                        index++;
                    }
                }
            }
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SelectRotation(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                CBNone.IsChecked = false;
            }
        }

        private void NoRotation(object sender, RoutedEventArgs e)
        {
            if (CBNone.IsChecked == true)
            {
                CBX.IsChecked = false;
                CBY.IsChecked = false;
                CBZ.IsChecked = false;
                CBXW.IsChecked = false;
                CBYW.IsChecked = false;
                CBZW.IsChecked = false;
            }
        }
    }
}
