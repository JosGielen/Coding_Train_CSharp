using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Flappy_Bird_with_NeuralNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate();
        private int WaitTime = 10;
        private bool App_Started = false;
        private Gate g;
        private List<Gate> Gates;
        private int Gatenumber = 4;
        private double GateWidth = 40.0;
        private double Gatespace = 150.0;
        private double GateSpeed = 0.8;
        private double upSpeed = 2.0;
        private double downSpeed = 0.02;
        private int Birdsnumber = 300;  //Number of birds in each generation ;
        private Bird[] Birds;
        private double BirdSize = 25.0;
        private double BirdMutationRate = 5;    //Percent chance that a weight in the Bird NeuralNet changes
        private double BirdMutationFactor = 5;  //Percentage that a weight in the Bird NeuralNet can change (+ or -)
        private double BirdX;
        private Bird BestBird;
        //private int GapReduceTime = 1000;      //Number of frames between each speed increase
        //private double GapRedution = 5;    //Percentage of speed increase
        private Random Rnd = new Random();

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
            //Make the gates on the right side of the screen
            for (int I = 0; I < Gatenumber; I++)
            {
                g = new Gate((Canvas1.ActualWidth + GateWidth) * (0.5 + I / (double)Gatenumber), GateWidth, Gatespace, Canvas1);
                //DEBUG CODE : Always start with the same first gate
                if (I == 0)
                {
                    g.GateTop = (0.8 * Canvas1.ActualHeight - Gatespace) * 0.5 + 0.1 * Canvas1.ActualHeight; //between 0.1*Height and (0.9*Height - space);
                }
                //END DEBUG CODE
                g.Draw();
                Gates.Add(g);
            }

        }

        private void MakeInitialGeneration()
        {
            //Make the initial Generation
            Birds = new Bird[Birdsnumber];
            for (int I = 0; I < Birdsnumber; I++)
            {
                Birds[I] = new Bird(BirdX, Canvas1.ActualHeight / 2, BirdSize, Canvas1);
                Birds[I].UpSpeed = upSpeed;
                Birds[I].SetBrain(new NeuralNet(5, 8, 2, 0.3, false));
                Birds[I].Draw();
            }
        }

        private void Start()
        {
            int Index = 0;
            double Mindistance = 0.0;
            int LiveBirdCounter = 0;
            App_Started = true;
            while (App_Started)
            {
                Render();
                //Check if a Gate has reached the Birds or if the birds are inside a gate.
                Index = -1;
                for (int I = 0; I < Gatenumber; I++)
                {
                    if (Gates[I].X <= BirdX + BirdSize / 2 && Gates[I].X + GateWidth > BirdX - BirdSize / 2)
                    {
                        Index = I;
                        break;
                    }
                }
                if (Index >= 0) //Gates[Index] has reached the Birds
                {
                    //Check bird collision with gates[Index]
                    for (int I = 0; I < Birdsnumber; I++)
                    {
                        Birds[I].CheckCollision(Gates[Index]);
                    }
                }
                //Get the closest Gate in front of the Birds
                Index = -1;
                Mindistance = double.MaxValue;
                for (int J = 0; J < Gatenumber; J++)
                {
                    if (Gates[J].X + GateWidth > BirdX + BirdSize / 2) //Gate is in front of the birds
                    {
                        if (Math.Abs(Gates[J].X - BirdX) < Mindistance)
                        {
                            Mindistance = Math.Abs(Gates[J].X - BirdX);
                            Index = J;
                        }
                    }
                }
                //All Live Birds can think about their next move to pass Gates[Index]
                LiveBirdCounter = 0;
                for (int I = 0; I < Birdsnumber; I++)
                {
                    if (Birds[I].Alive)
                    {
                        Birds[I].Think(Gates[Index]);
                        LiveBirdCounter += 1;
                    }
                }
                Title = LiveBirdCounter.ToString() + " Birds still alive.";
                //If all birds died: Make the next Generation;
                if (LiveBirdCounter == 0)
                {
                    Init();
                    //Get the fitness of each bird as the normalized score
                    double sumScore = 0.0;
                    Bird[] OldBirds = new Bird[Birdsnumber];
                    for (int I = 0; I < Birdsnumber; I++)
                    {
                        sumScore += (Birds[I].Score);
                    }
                    for (int I = 0; I < Birdsnumber; I++)
                    {
                        Birds[I].Fitness = Birds[I].Score / sumScore;
                    }
                    //Get the best bird
                    double maxScore = 0.0;
                    for (int I = 0; I < Birdsnumber; I++)
                    {
                        if (Birds[I].Score > maxScore)
                        {
                            maxScore = Birds[I].Score;
                            BestBird = Birds[I].copy();
                        }
                    }

                    //DEBUG CODE
                    //Make all Bestbirds and mutate some of them
                    //for (int I = 0; I < Birdsnumber; I++)
                    //{
                    //    Birds[I] = BestBird.copy();
                    //    if (Rnd.NextDouble() < 0.1)
                    //    {
                    //        Birds[I].Brain.Mutate(BirdMutationRate, BirdMutationFactor);
                    //    }
                    //    Birds[I].Draw();
                    //}
                    //END DEBUG CODE

                    ////Copy all the birds into a selection pool (= Oldbirds)
                    for (int I = 0; I < Birdsnumber; I++)
                    {
                        OldBirds[I] = Birds[I].copy();
                        OldBirds[I].Fitness = Birds[I].Fitness;
                    }
                    //Pick random Oldbirds to form the next Generation  (Genetic Algorithm where the pickchance = fitness);
                    double r;
                    for (int I = 0; I < Birdsnumber; I++)
                    {
                        r = Rnd.NextDouble();
                        Index = 0;
                        while (r > 0.0)
                        {
                            r = r - OldBirds[Index].Fitness;
                            Index += 1;
                        }
                        Index -= 1;
                        Birds[I] = OldBirds[Index].copy();
                        Birds[I].Brain.Mutate(BirdMutationRate, BirdMutationFactor);
                        Birds[I].Draw();
                    }
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
            for (int I = 0; I < Birdsnumber; I++)
            {
                if (Birds[I].Alive)
                {
                    Birds[I].Update(downSpeed);
                    Birds[I].Score += 1;
                }
                else
                {
                    Birds[I].Remove();
                }
            }
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }

        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            App_Started = false;
            Environment.Exit(1);
        }

        private void MnuLoad_Click(Object sender, RoutedEventArgs e)
        {
            //Show an OpenFile dialog
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.InitialDirectory = Environment.CurrentDirectory;
            OFD.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            OFD.FilterIndex = 1;
            OFD.RestoreDirectory = true;
            if (OFD.ShowDialog() == true)
            {
                Init();
                BestBird = new Bird(BirdX, Canvas1.ActualHeight / 2, BirdSize, Canvas1);
                BestBird.UpSpeed = upSpeed;
                BestBird.LoadNN(OFD.FileName);
            }
            for (int I = 0; I < Birdsnumber; I++)
            {
                Birds[I] = BestBird.copy();
                if (Rnd.Next(100) > BirdMutationRate)
                {
                    Birds[I].Brain.Mutate(BirdMutationRate, BirdMutationFactor);
                    Birds[I].Draw();
                }
                Start();
            }
        }

        private void MnuSave_Click(Object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.InitialDirectory = Environment.CurrentDirectory;
            SFD.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            SFD.FilterIndex = 1;
            SFD.RestoreDirectory = true;
            if (SFD.ShowDialog() == true)
            {
                BestBird.SaveNN(SFD.FileName);
            }
        }

        private void MnuExit_Click(Object sender, RoutedEventArgs e)
        {
            App_Started = false;
            Environment.Exit(1);
        }

        private void BtnStart_Click(Object sender, RoutedEventArgs e)
        {
            if (!App_Started)
            {
                BtnStart.Content = "STOP";
                Init();
                MakeInitialGeneration();
                Start();
            }
            else
            {
                BtnStart.Content = "START";
                Halt();
            }
        }

        private void MnuSaveCurrent_Click(Object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.InitialDirectory = Environment.CurrentDirectory;
            SFD.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            SFD.FilterIndex = 1;
            SFD.RestoreDirectory = true;
            if (SFD.ShowDialog() == true)
            {
                for (int I = 0; I < Birdsnumber; I++)
                {
                    if (Birds[I].Alive)
                    {
                        Birds[I].SaveNN(SFD.FileName);
                        break;
                    }
                }
            }
        }
    }
}
