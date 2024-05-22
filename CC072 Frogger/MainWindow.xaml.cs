using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Frogger
{
    public partial class MainWindow : Window
    {
        private List<Lane> lanes;
        private double LaneHeight;
        private Frog frog;
        private int Lives;
        private List<Image> LiveImages;
        private bool[] SaveFrogs;
        private DateTime StartTime;
        private double Elapsedtime;
        private bool Initialized = false;
        private bool Started = false;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }


        private void Init()
        {
            canvas1.Children.Clear();
            Started = false;
            //Create the Lanes
            string[] lanetypes = { "SAVE", "CARS", "CARS", "CARS", "CARS", "SAVE", "LOGS", "LOGS", "TURTLES", "LOGS", "LOGS", "BUSHES", "SAVE" };
            Lane L;
            lanes = new List<Lane>();
            SaveFrogs = new bool[8];
            LaneHeight = canvas1.ActualHeight / lanetypes.Length;
            for (int i = 0; i < lanetypes.Length; i++)
            {
                L = new Lane(lanetypes[i], i, canvas1);
                lanes.Add(L);
            }
            //Set the Cars
            lanes[1].SetObstacles(3, 100, 2.0, "TRUCKS");
            lanes[2].SetObstacles(4, 50, 2.5, "CARS");
            lanes[3].SetObstacles(3, 50, -2.7, "CARS");
            lanes[4].SetObstacles(3, 100, -2.3, "TRUCKS");
            //Set the Logs
            lanes[6].SetObstacles(3, 120, 1.9, "LOG");
            lanes[7].SetObstacles(2, 250, -2.1, "LOG");
            lanes[9].SetObstacles(2, 190, -1.3, "LOG");
            lanes[10].SetObstacles(3, 120, 2.1, "LOG");
            //Set the Turtles
            lanes[8].SetObstacles(4, 50, 2.0, "TURTLES");
            //Set the destiny sprites
            lanes[11].SetObstacles(4, 0, 0, "BUSHES");

            //Draw the lanes and Obstacles
            for (int i = 0; i < lanes.Count-1; i++)
            {
                lanes[i].Draw();
            }
            Lives = 8;
            //Show life icons
            BitmapImage lifeImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\FrogLife.Gif"));
            ImgLife1.Source = lifeImage;
            ImgLife2.Source = lifeImage;
            ImgLife3.Source = lifeImage;
            ImgLife4.Source = lifeImage;
            ImgLife5.Source = lifeImage;
            ImgLife6.Source = lifeImage;
            ImgLife7.Source = lifeImage;
            ImgLife8.Source = lifeImage;
            LiveImages = [ImgLife1, ImgLife2, ImgLife3, ImgLife4, ImgLife5, ImgLife6, ImgLife7, ImgLife8];
            PbTime.Value = 20.0;
            Initialized = true;
        }


        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Initialized) { return; }
            mnuMain.IsEnabled = false;
            if (frog != null)
            {
                //Remove dead frogs
                if (frog.Alive == false) { frog.Erase(canvas1); }
            }
            //Create a new Frog
            if (Lives > 0)
            {
                LiveImages[Lives - 1].Source = null;
                Lives--;
                frog = new Frog(canvas1.ActualHeight / 13 - 16)
                {
                    Left = canvas1.ActualWidth / 2 - 25,
                    Top = 12 * canvas1.ActualHeight / 13 + 8,
                    LaneIndex = 0,
                    OnLog = false
                };
                frog.Draw(canvas1);
                StartTime = DateTime.Now;
                Started = true;
                Elapsedtime = 0;
            }
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            //Move the Obstacles in the Lanes
            for (int i = 0; i < lanes.Count; i++)
            {
                lanes[i].Update();
            }
            if (Started)
            {
                //Update the Frog position
                frog.Update();
                //Check if the Frog is safe
                if (lanes[frog.LaneIndex].LaneType.Equals("SAVE"))
                {
                    //The frog is fine.
                }
                else if (lanes[frog.LaneIndex].LaneType.Equals("BUSHES"))
                {
                    //Check if the frog lands on a lotus leaf, else bounce back.
                    int saveIndex = lanes[frog.LaneIndex].CenterCollision(frog);
                    if (saveIndex % 2 == 1 )
                    {
                        if (SaveFrogs[saveIndex] == false) //The frog jumped on an empty lotus leaf
                        {
                            SaveFrogs[saveIndex] = true;
                            string gameresult = "";
                            if (SaveFrogs[1] && SaveFrogs[3] && SaveFrogs[5] && SaveFrogs[7])
                            {
                                gameresult = "Congratulations you saved 4 frogs!";
                            }
                            else if (Lives == 0)
                            {
                                gameresult = "Game Over. You failed to save 4 frogs!";
                            }
                            if (!gameresult.Equals(""))
                            { 
                                TextBox txt = new TextBox()
                                {
                                    Width = canvas1.ActualWidth,
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    Text = gameresult,
                                    FontSize = 24,
                                    FontWeight = FontWeights.Bold,
                                    Background = Brushes.Yellow
                                };
                                txt.SetValue(Canvas.LeftProperty, 0.0);
                                txt.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 15);
                                canvas1.Children.Add(txt);
                                mnuMain.IsEnabled = true;
                                Initialized = false;
                            }
                            Started = false;
                            return;
                        }
                    }
                    //Bounce back
                    frog.Top += LaneHeight;
                    frog.LaneIndex--;
                    frog.OnLog = false;
                }
                else if (lanes[frog.LaneIndex].LaneType.Equals("CARS"))
                {
                    if (lanes[frog.LaneIndex].Collision(frog) >= 0)
                    {
                        FrogKill(); //The frog got squashed by a Car.
                        return;
                    }
                }
                else
                {
                    if (lanes[frog.LaneIndex].Collision(frog) >= 0)
                    {
                        //The frog jumped on a log or turtle
                        frog.OnLog = true;
                        frog.LogSpeed = lanes[frog.LaneIndex].LaneSpeed;
                    }
                    else
                    {
                        FrogKill(); //The frog drowned in the river
                        return;
                    }
                }
                //Update the Timer
                Elapsedtime = (DateTime.Now - StartTime).TotalSeconds;
                PbTime.Value = 20 - Elapsedtime;
                if (PbTime.Value >= 10)
                {
                    PbTime.Foreground = Brushes.Green;
                }
                else if (PbTime.Value > 5) 
                { 
                    PbTime.Foreground = Brushes.Orange; 
                }
                else
                {
                    PbTime.Foreground = Brushes.Red;
                }
                if (PbTime.Value <= 0)
                {
                    //The frog died of old age
                    FrogKill();
                }
            }
        }

        private void FrogKill()
        {
            BitmapImage death = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\FrogDeath.Gif"));
            frog.SetImage(death);
            frog.Alive = false;
            if (Lives == 0)
            {
                //Game Over
                TextBox txt = new TextBox()
                {
                    Width = canvas1.ActualWidth,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Text = "Game Over. You failed to save 4 frogs!",
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.Yellow
                };
                txt.SetValue(Canvas.LeftProperty, 0.0);
                txt.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 15);
                canvas1.Children.Add(txt);
                mnuMain.IsEnabled = true;
                Initialized = false;
            }
            Started = false;
        }

        private void MnuNew_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Started)
            {
                e.Handled = true;
                return;
            }
            switch (e.Key)
            {
                case Key.Up:
                    if (frog.LaneIndex < 12)
                    {
                        frog.Top -= LaneHeight;
                        frog.LaneIndex++;
                        frog.OnLog = false;
                        Debug.Print("Lane " + frog.LaneIndex.ToString() + " = " + lanes[frog.LaneIndex].LaneType);
                    }
                    break;
                case Key.Down:
                    if (frog.LaneIndex > 0)
                    {
                        frog.Top += LaneHeight;
                        frog.LaneIndex--;
                        frog.OnLog = false;
                    }
                    break;
                case Key.Left:
                    if (frog.Left > LaneHeight)
                    {
                        frog.Left -= LaneHeight;
                        frog.Right -= LaneHeight;
                    }
                    break;
                case Key.Right:
                    if (frog.Right < canvas1.ActualWidth - LaneHeight)
                    {
                        frog.Left += LaneHeight;
                        frog.Right += LaneHeight;
                    }
                    break;
            }
        }

    }
}