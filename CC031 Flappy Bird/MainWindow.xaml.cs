using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Flappy_Bird
{
    public partial class MainWindow : Window
    {
        private int WaitTime = 10;
        private bool App_Started = false;
        private bool App_Initialized = false;
        private Gate g;
        private List<Gate> Gates;
        private int Gatenumber = 4;
        private double GateWidth = 40.0;
        private double Gatespace = 100.0;
        private double GateSpeed = 0.8;
        private double upSpeed = 2.0;
        private double downSpeed = 0.02;
        private Bird b;
        private double BirdSize = 25.0;
        private double BirdX;
        private int Framecounter = 0;
        private int IncreaseTime = 1000;    //Number of frames between each speed increase ;
        private double SpeedIncrease = 5;   //Percentage of speed increase ;
        private int Score = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            Gates = new List<Gate>();
            Canvas1.Children.Clear();
            BirdX = Canvas1.ActualWidth / 10;
            GateSpeed = 0.8;
            upSpeed = 1;
            downSpeed = 0.02;
            //Make the border and background
            Rectangle r = new Rectangle()
            {
                Width = Canvas1.ActualWidth - 15,
                Height = Canvas1.ActualHeight - 30,
                Fill = Brushes.Black
            };
            r.SetValue(Canvas.TopProperty, 15.0);
            r.SetValue(Canvas.LeftProperty, 0.0);
            Canvas1.Children.Add(r);
            //Make the gates to the right of the screen
            for (int I = 0; I < Gatenumber; I++)
            {
                g = new Gate((Canvas1.ActualWidth + GateWidth) * (0.5 + I / (double)Gatenumber), GateWidth, Gatespace, Canvas1);
                g.Draw();
                Gates.Add(g);
            }
            //Make the bird
            b = new Bird(BirdX, Canvas1.ActualHeight / 2, BirdSize, Canvas1);
            b.Draw();
            Score = 0;
            App_Initialized = true;
        }

        private void Start()
        {
            int Index;
            App_Started = true;
            if (!App_Initialized) Init();
            Canvas1.Focus();
            while (App_Started)
            {
                Render();
                //Check bird collision when a gate reaches the bird(s) or when the bird(s) is/are inside the gate)
                Index = -1;
                for (int I = 0; I < Gatenumber; I++)
                {
                    if (Gates[I].X <= BirdX + BirdSize / 2 & Gates[I].X + GateWidth > BirdX - BirdSize / 2)
                    {
                        Index = I;
                        break;
                    }
                }
                if (Index >= 0) b.CheckCollision(Gates[Index]);
                if (!b.Alive)
                {
                    App_Initialized = false;
                    BtnStart.Content = "START";
                    App_Started = false;
                    MessageBox.Show("   Game Over!   \tYour Score = " + Score.ToString(), "Flappy Bird", MessageBoxButton.OK);
                }
                Dispatcher.Invoke(Wait, DispatcherPriority.ApplicationIdle);
            }
        }

        private void Halt()
        {
            App_Started = false;
        }

        private void Render()
        {
            for (int I = 0; I < Gates.Count; I++)
            {
                Gates[I].Update(GateSpeed);
                if (Gates[I].X < -GateWidth)
                {
                    Gates[I].Reset();
                }
            }
            b.Update(downSpeed);
            Framecounter += 1;
            if (Framecounter == IncreaseTime)
            {
                Framecounter = 0;
                GateSpeed *= (100 + SpeedIncrease) / 100;
                downSpeed *= (100 + SpeedIncrease) / 100;
                upSpeed *= (100 + SpeedIncrease) / 100;
            }
            Score += 1;
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }


        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!App_Started)
            {
                BtnStart.Content = "STOP";
                Start();
            }
            else
            {
                Halt();
                BtnStart.Content = "START";
            }
        }

        private void Window_Closing(Object sender, CancelEventArgs e)
        {
            App_Started = false;
            Environment.Exit(0);
        }

        private void Window_KeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                b.Flap(upSpeed);
            }
            e.Handled = true;
        }

    }
}
