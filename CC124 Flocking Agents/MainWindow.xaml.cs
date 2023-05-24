using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Flocking_Agents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Agent> flock;
        private int population = 500;
        private double viewdistance = 40.0;
        private double maxForce = 0.1;
        private double maxSpeed = 1.5;
        private bool constantSpeed = true;
        private bool WrapEdges = true;
        private double separationStrength = 0.0;
        private double alignmentStrength = 0.0;
        private double cohesionStrength = 0.0;
        private double cursorStrength = 0.1;
        private Vector center;

        private Random rnd = new Random();
        private bool Started = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Agent ag;
            double x = 0.0;
            double y = 0.0;
            flock = new List<Agent>();
            for (int I = 0; I < population; I++)
            {
                x = (0.8 * rnd.NextDouble() + 0.1) * canvas1.ActualWidth;
                y = (0.8 * rnd.NextDouble() + 0.1) * canvas1.ActualHeight;
                ag = new Agent(new Point(x, y))
                {
                    CanRotate = true,
                    MaxSpeed = maxSpeed,
                    MaxForce = maxForce,
                    MoveToTarget = false,
                    Velocity = new Vector(4 * rnd.NextDouble() - 2, 4 * rnd.NextDouble() - 2),
                    UseConstantSpeed = constantSpeed,
                    Size = new Size(10.0, 5.0)
                };
                ag.Draw(canvas1);
                flock.Add(ag);
                center = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
                Ellipse El = new Ellipse()
                {
                    Width = 30,
                    Height = 30,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2.0,
                    Fill = Brushes.Red
                };
                El.SetValue(Canvas.LeftProperty, center.X - 15);
                El.SetValue(Canvas.TopProperty, center.Y - 15);
                canvas1.Children.Add(El);
            }
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            if (!Started) return;
            double x;
            double y;
            double dist;
            Vector Steering;
            int total;
            //Calculate the Steerings on each agent
            for (int I = 0; I < population; I++)
            {
                //Move away from the cursor
                dist = Distance(flock[I].Location, center);
                if (dist < 3 * viewdistance)
                {
                    flock[I].ApplyForce(cursorStrength * (flock[I].Location - center));
                }
                //Apply flocking behaviour
                Steering = new Vector();
                total = 0;
                for (int J = 0; J < population; J++)
                {
                    if (J != I)
                    {
                        dist = Distance(flock[J].Location, flock[I].Location);
                        if (dist < viewdistance)
                        {
                            //Separation
                            Steering += separationStrength * (flock[I].Location - flock[J].Location) / (dist * dist);
                            //Alignment
                            Steering += alignmentStrength * (flock[J].Velocity - flock[I].Velocity) / dist;
                            //Cohesion
                            Steering += cohesionStrength * (flock[J].Location - flock[I].Location) / dist;
                            total += 1;
                        }
                    }
                }
                if (total > 0)
                {
                    Steering = Steering / total;
                    flock[I].ApplyForce(Steering);
                }
            }
            //Update the agents
            for (int I = 0; I < population; I++)
            {
                if (WrapEdges) flock[I].Edges(canvas1);
                flock[I].Update();
            }
        }

        private double Distance(Vector v1, Vector v2)
        {
            return (v2 - v1).Length;
        }

        private void SldSep_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            separationStrength = SldSep.Value;
        }

        private void SldAlig_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            alignmentStrength = SldAlig.Value;
        }

        private void SldCoh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            cohesionStrength = SldCoh.Value;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Started)
            {
                Started = true;
                BtnStart.Content = "STOP";
                separationStrength = SldSep.Value;
                alignmentStrength = SldAlig.Value;
                cohesionStrength = SldCoh.Value;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                Started = false;
                BtnStart.Content = "START";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }
    }
}
