using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using Quadtree;
using JG_Graphs;

namespace _2DCollisions
{
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate();
        private int WaitTime = 10;
        private bool Rendering = false;
        private List<Ball> balls;
        private int ballCount = 3000;
        private int minBallSize = 3;
        private int maxBallSize = 4;
        private QTree QT;
        private int[] Speeddist;
        BarSeries bser1;
        private Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            QT = new QTree(new Rect(0.0, 0.0, canvas1.ActualWidth, canvas1.ActualHeight),10);
            balls = new List<Ball>();
            Ball B;
            Vector Pos;
            double Angle;
            double Size;
            double Mass;
            double Speed;
            double Energy = 0.0;
            Vector Momentum = new Vector(0, 0);
            double TotalMass = 0;
            do
            {
                Size = (maxBallSize - minBallSize) * rnd.NextDouble() + minBallSize;
                Mass = Size / 4;
                Speed = rnd.NextDouble() + 1;
                Angle = 360 * rnd.NextDouble();
                Pos = new Vector((canvas1.ActualWidth - 2 * Size) * rnd.NextDouble() + Size, (canvas1.ActualHeight - 2 * Size) * rnd.NextDouble() + Size);
                bool IsOK = true;
                //Make sure no 2 balls overlap
                for (int I = 0; I < balls.Count; I++)
                {
                    if ((Pos - balls[I].Pos).Length <= Size + balls[I].Radius)
                    {
                        IsOK = false;
                        break;
                    }
                }
                if (IsOK)
                {
                    B = new Ball(Pos, Angle, Size, Mass, Speed);
                    B.Color = new SolidColorBrush(Color.FromRgb((byte)(60 * Size), (byte)(60 * Size), (byte)(60 * Size)));
                    balls.Add(B);
                    B.Show(canvas1);
                    TotalMass += Mass;
                    Momentum += Mass * B.Velocity;
                    Energy += 0.5 * Mass * Math.Pow(Speed, 2);
                }
                TxtEnergy.Text = Energy.ToString("F4");
            } while (balls.Count < ballCount);

            //Create the BarLineGraph
            //Add the X-axis tick labels 0 to 10 in 0.5 interval
            for (double I = 0; I < 6; I+=0.1)
            {
                graph1.AddXTickLabel(I.ToString("F2"));
            }
            //Create the Bar Series
            bser1 = new BarSeries(0);
            bser1.SeriesName = "Speed Distribution";
            bser1.ShowLine = false;
            graph1.BarDataSeries.Add(bser1);
            //Create the Line Series with the theoretical distribution
            LineSeries lser = new LineSeries(0);
            for (double I = 0; I < 6; I += 0.1)
            {
                lser.AddDataPoint(300 * I * Math.Exp(-I*I/2));
            }
            lser.SeriesName = "Boltzmann Distribution";
            lser.ShowLine = true;
            lser.LineThickness = 2.0;
            lser.LineColor = Brushes.Red;
            graph1.LineDataSeries.Add(lser);
            graph1.LegendFontsize = 12;
            graph1.LegendPosition = LegendPosition.Top;
            //Set the axis names and font size
            graph1.XAxis.AxisLabel = "Speed";
            graph1.XAxis.AxisLabelFontSize = 12;
            graph1.YAxis.AxisLabel = "Count";
            graph1.YAxis.AxisLabelFontSize = 12;
            graph1.XAxis.TicksAtData = true;
            graph1.ShowXGrid = true;
            graph1.ShowYGrid = true;
            //Draw a border around the graph
            graph1.BorderBrush = Brushes.Black;
            graph1.BorderThickness = new Thickness(1.0);
            graph1.Draw();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                Rendering = true;
                BtnStart.Content = "STOP";
                Render();
            }
            else
            {
                Rendering = false;
                BtnStart.Content = "START";
            }
        }

        private void Render()
        {
            WaitDelegate waitDel = new WaitDelegate(Wait);

            while (Rendering)
            {
                if (CbQTree.IsChecked == true)
                {
                    //Build the QTree
                    QT.Clear();
                    for (int I = 0; I < balls.Count(); I++)
                    {
                        QT.Insert(balls[I].Pos, I);
                    }
                    //Check for collisions with balls that are close by in the QuadTree
                    for (int I = 0; I < balls.Count(); I++)
                    {
                        balls[I].Update(CbBounce.IsChecked == true);
                        List<int> closeBallIndices = QT.QueryIndices(balls[I].Pos, 10);
                        for (int J = 0; J < closeBallIndices.Count; J++)
                        {
                            if (I != closeBallIndices[J]) { balls[I].Collide(balls[closeBallIndices[J]]); }
                        }
                    }
                }
                else
                {
                    //Check for collisions with all other balls
                    for (int I = 0; I < balls.Count(); I++)
                    {
                        balls[I].Update(CbBounce.IsChecked == true);
                        for (int J = I + 1; J < balls.Count; J++)
                        {
                            balls[I].Collide(balls[J]);
                        }
                    }
                }
                //Update the BarGraph
                Speeddist = new int[60];
                for (int I = 0; I < balls.Count(); I++)
                {
                    double barindex = balls[I].Speed / 0.1;
                    if (barindex < 60) { Speeddist[(int)barindex]++; }
                }
                //Update the Bar series
                bser1.ClearData();
                for (int I = 0; I < 60; I++)
                {
                    bser1.AddDataPoint(Speeddist[I]);
                }
                graph1.Draw();
                //Calculate the total kinetic energy
                double Energy = 0.0;
                for (int I = 0; I < balls.Count(); I++)
                {
                    Energy += 0.5 * balls[I].Mass * Math.Pow(balls[I].Velocity.Length, 2);
                }
                TxtEnergy.Text = Energy.ToString("F4");
                Dispatcher.Invoke(waitDel, DispatcherPriority.ApplicationIdle);
            }
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }
    }
}
