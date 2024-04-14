using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Space_Invaders
{
    public partial class MainWindow : Window
    {
        private Image ImgShip;
        private double ShipPosX;
        private double ShipPosY;
        private double ShipWidth;
        private double ShipHeight;
        private Point ShipCenter;
        private int ShipEnergy;
        private int ShipLife;
        Ellipse[] shield;
        private List<Invader> Invaders;
        private List<Bullet> ShipBullets;
        private List<Bullet> InvadBullets;
        private List<Explosion> Explosions;
        private int Level;
        private double InvSpeed;
        private int My_Score;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MnuNew_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void Init()
        {
            canvas1.Children.Clear();
            //Load the Ship image and place it in the start position
            BitmapImage bitmap;
            bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\ship.jpg"));
            ImgShip = new Image();
            ImgShip.Source = bitmap;
            ShipPosX = (canvas1.ActualWidth - bitmap.Width) / 2;
            ShipPosY = canvas1.ActualHeight - bitmap.Height - 15;
            ShipWidth = bitmap.Width;
            ShipHeight = bitmap.Height;
            ShipCenter = new Point(ShipPosX + ShipWidth / 2, ShipPosY + ShipHeight / 2);
            ShipEnergy = 0;
            ShipLife = 9;
            ImgShip.SetValue(Canvas.LeftProperty, ShipPosX);
            ImgShip.SetValue(Canvas.TopProperty, ShipPosY);
            canvas1.Children.Add(ImgShip);
            //Give the ship a shield
            shield = new Ellipse[10];
            for (int  i = 0; i < 10; i++)
            {
                shield[i] = new Ellipse()
                {
                    Width = 60 + 2 * i,
                    Height = 60 + 2 * i,
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 2.0
                };
                shield[i].SetValue(Canvas.LeftProperty, ShipCenter.X - (30 + i));
                shield[i].SetValue(Canvas.TopProperty, ShipCenter.Y - (30 + i));
                canvas1.Children.Add(shield[i]);
            }
            //Initialize the game
            My_Score = 0;
            ShipBullets = new List<Bullet>();
            InvadBullets = new List<Bullet>();
            Explosions = new List<Explosion>();
            Invaders = new List<Invader>();
            InvSpeed = 0.05;
            //Show Level 1
            Level = 1;
            LoadLevel(Level);
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            //Update the Explosions
            for (int i = 0; i < Explosions.Count; i++)
            {
                Explosions[i].Update();
            }
            //Remove dead explosions
            for (int i = Explosions.Count - 1; i >= 0; i--)
            {
                if (Explosions[i].status == false)
                {
                    Explosions[i].Erase(canvas1);
                    Explosions.RemoveAt(i);
                }
            }
            //Update the Bullets
            for (int i = 0; i < ShipBullets.Count; i++)
            {
                ShipBullets[i].Update();
            }
            for (int i = 0; i < InvadBullets.Count; i++)
            {
                InvadBullets[i].Update();
            }
            //Check for Bullet Hits on Invaders
            for (int j = 0; j < Invaders.Count; j++)
            {
                for (int i = ShipBullets.Count - 1; i >= 0; i--)
                {
                    if (ShipBullets[i].Hit(Invaders[j]))
                    {
                        Explosion ex = new Explosion(ShipBullets[i].X, ShipBullets[i].Y, false);
                        Explosions.Add(ex);
                        ex.Draw(canvas1);
                        ShipBullets[i].Erase(canvas1);
                        ShipBullets.RemoveAt(i);
                        Invaders[j].Life--;
                    }
                }
                if (Invaders[j].Life <= 0)
                {
                    Explosion ex = new Explosion(Invaders[j].left + Invaders[j].width / 2, Invaders[j].top + Invaders[j].height / 2, true);
                    Explosions.Add(ex);
                    ex.Draw(canvas1);
                    Invaders[j].Erase(canvas1);
                    Invaders.RemoveAt(j);
                    My_Score += 10;
                    Title = "Space Invaders score = " + My_Score.ToString();
                }
            }
            //Check for Bullet Hits on the Ship
            for (int i = InvadBullets.Count - 1; i >= 0; i--)
            {
                if (ShipLife >= 0 && (InvadBullets[i].X - ShipCenter.X) *(InvadBullets[i].X - ShipCenter.X) + (InvadBullets[i].Y - ShipCenter.Y) *(InvadBullets[i].Y - ShipCenter.Y) < shield[ShipLife].ActualWidth*shield[ShipLife].ActualWidth / 4)
                {
                    Explosion ex = new Explosion(InvadBullets[i].X, InvadBullets[i].Y, false);
                    Explosions.Add(ex);
                    ex.Draw(canvas1);
                    InvadBullets[i].Erase(canvas1);
                    InvadBullets.RemoveAt(i);
                    ShipLife -= 1;
                    //Update Ship Shield
                    for (int k = 9; k >= 0; k--)
                    {
                        if (ShipLife <= k && canvas1.Children.Contains(shield[k])) canvas1.Children.Remove(shield[k]);
                    }
                    if (ShipLife < 0)
                    {
                        //Game Over
                        ex = new Explosion(ShipCenter.X, ShipCenter.Y, true);
                        Explosions.Add(ex);
                        ex.Draw(canvas1);
                        canvas1.Children.Remove(ImgShip);
                        ShipPosX = 0;
                        ShipPosY = 0;
                        TextBox txt = new TextBox()
                        {
                            Width = canvas1.ActualWidth,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            Text = "Game Over. Score = " + My_Score.ToString(),
                            FontSize = 24,
                            FontWeight = FontWeights.Bold,
                            Background = Brushes.Yellow
                        };
                        txt.SetValue(Canvas.LeftProperty, 0.0);
                        txt.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 15);
                        canvas1.Children.Add(txt);
                    }
                }
            }
            //Check for Bullets off the map
            for (int i = ShipBullets.Count - 1; i >= 0; i--)
            {
                if (ShipBullets[i].Y <= 0)
                {
                    ShipBullets[i].Erase(canvas1);
                    ShipBullets.RemoveAt(i);
                }
            }
            for (int i = InvadBullets.Count - 1; i >= 0; i--)
            {
                if (InvadBullets[i].Y >= canvas1.ActualHeight)
                {
                    InvadBullets[i].Erase(canvas1);
                    InvadBullets.RemoveAt(i);
                }
            }
            //Update the Invaders
            for (int i = 0; i < Invaders.Count; i++)
            {
                Invaders[i].Update();
                if (Invaders[i].right >= canvas1.ActualWidth - 10) { Invader.dX = -0.5; }
                if (Invaders[i].left <= 10) { Invader.dX = 0.5; }
                Invaders[i].Update();
                if (Invaders[i].CanFire())
                {
                    Bullet b = new Bullet(Invaders[i].left + (Invaders[i].right - Invaders[i].left) / 2, Invaders[i].bottom, 2);
                    InvadBullets.Add(b);
                    b.Draw(canvas1);
                }
                if (Invaders[i].top >= canvas1.ActualHeight)
                {
                    Invaders[i].Erase(canvas1);
                    Invaders.RemoveAt(i);
                }
            }
            //Check for collision between invader and ship
            if (ShipLife > 0)
            {
                for (int i = 0; i < Invaders.Count; i++)
                {
                    if (Invaders[i].right > ShipPosX && Invaders[i].left < ShipPosX + ShipWidth && Invaders[i].bottom > ShipPosY)
                    {
                        //Game Over
                        Explosion ex = new Explosion(ShipCenter.X, ShipCenter.Y, true);
                        Explosions.Add(ex);
                        ex.Draw(canvas1);
                        //Remove any remaining Ship Shield
                        for (int k = 9; k >= 0; k--)
                        {
                            if (canvas1.Children.Contains(shield[k])) canvas1.Children.Remove(shield[k]);
                        }
                        canvas1.Children.Remove(ImgShip);
                        ShipLife = -1;
                        ShipPosX = 0;
                        ShipPosY = 0;
                        TextBox txt = new TextBox()
                        {
                            Width = canvas1.ActualWidth,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            Text = "Game Over. Score = " + My_Score.ToString(),
                            FontSize = 24,
                            FontWeight = FontWeights.Bold,
                            Background = Brushes.Yellow
                        };
                        txt.SetValue(Canvas.LeftProperty, 0.0);
                        txt.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 15);
                        canvas1.Children.Add(txt);
                    }
                }
            }
            //Check for End of Level
            if (ShipLife > 0 && !Invaders.Any())
            {
                Level++;
                if (Level > 4) { InvSpeed += 0.01; }
                LoadLevel(Level);
            }
            //Reload the Ship energy
            if (ShipEnergy < 50)
            {
                ShipEnergy++;
            }
        }

        private void LoadLevel(int lvl)
        {
            string ImgFile;
            Invader invad;
            if (lvl == 1) //Load the first level
            {
                //Load a row of Invader1 images and place them at the top of the canvas
                ImgFile = Environment.CurrentDirectory + "\\invader1.jpg";
                BitmapImage bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\invader1.jpg"));
                for (int I = 0; I < 8; I++)
                {
                    invad = new Invader(ImgFile, I * (bitmap.Width + 25) + 75, 30.0, InvSpeed, false);
                    invad.Draw(canvas1);
                    Invaders.Add(invad);
                }

            }
            else if (lvl == 2) //Load the second level
            {
                //Load a row of Invader2 images and place them at the top of the canvas
                ImgFile = Environment.CurrentDirectory + "\\invader2.jpg";
                BitmapImage bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\invader2.jpg"));
                for (int I = 0; I < 5; I++)
                {
                    invad = new Invader(ImgFile, I * (bitmap.Width + 70) + 65, 30.0, InvSpeed, true);
                    invad.Draw(canvas1);
                    Invaders.Add(invad);
                }
            }
            else if (lvl == 3) //Load the third level = level 1 + level 2
            {
                //Load a row of Invader1 images and place them at the top of the canvas
                ImgFile = Environment.CurrentDirectory + "\\invader1.jpg";
                BitmapImage bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\invader1.jpg"));
                for (int I = 0; I < 8; I++)
                {
                    invad = new Invader(ImgFile, I * (bitmap.Width + 25) + 75, 30.0, InvSpeed, false);
                    invad.Draw(canvas1);
                    Invaders.Add(invad);
                }
                //Load a row of Invader2 images and place them below row 1
                ImgFile = Environment.CurrentDirectory + "\\invader2.jpg";
                bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\invader2.jpg"));
                for (int I = 0; I < 5; I++)
                {
                    invad = new Invader(ImgFile, I * (bitmap.Width + 70) + 65, 110.0, InvSpeed, true);
                    invad.Draw(canvas1);
                    Invaders.Add(invad);
                }
            }
            else if (lvl > 3) //Load levels above lvl 3
            {
                //Load a row of Invader1 images and place them at the top of the canvas
                ImgFile = Environment.CurrentDirectory + "\\invader1.jpg";
                BitmapImage bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\invader1.jpg"));
                for (int I = 0; I < 8; I++)
                {
                    invad = new Invader(ImgFile, I * (bitmap.Width + 25) + 75, 30.0, InvSpeed, false);
                    invad.Draw(canvas1);
                    Invaders.Add(invad);
                }
                //Load a row of Invader2 images and place them below row 1
                ImgFile = Environment.CurrentDirectory + "\\invader2.jpg";
                bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\invader2.jpg"));
                for (int I = 0; I < 5; I++)
                {
                    invad = new Invader(ImgFile, I * (bitmap.Width + 70) + 65, 110.0, InvSpeed, true);
                    invad.Draw(canvas1);
                    Invaders.Add(invad);
                }
                //Load a row of Invader3 images and place them below row 2
                ImgFile = Environment.CurrentDirectory + "\\invader3.jpg";
                bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\invader3.jpg"));
                for (int I = 0; I < 7; I++)
                {
                    invad = new Invader(ImgFile, I * (bitmap.Width + 45) + 65, 180.0, InvSpeed, true);
                    invad.Draw(canvas1);
                    Invaders.Add(invad);
                }
            }
        }

        private void UpdateShip()
        {
            ShipCenter = new Point(ShipPosX + ShipWidth / 2, ShipPosY + ShipHeight / 2);
            for (int i = 0; i < 10; i++)
            {
                shield[i].SetValue(Canvas.LeftProperty, ShipCenter.X - (30 + i));
                shield[i].SetValue(Canvas.TopProperty, ShipCenter.Y - (30 + i));
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    ShipPosX -= 5;
                    UpdateShip();
                    break;
                case Key.Right:
                    ShipPosX += 5;
                    UpdateShip();
                    break;
                case Key.Space:
                    if (ShipEnergy > 10 && ShipLife >= 0)
                    {
                        Bullet b = new Bullet(ShipPosX + ShipWidth / 2, ShipPosY, -2);
                        ShipBullets.Add(b);
                        b.Draw(canvas1);
                        ShipEnergy -= 10;
                    }
                    break;
            }
            ImgShip.SetValue(Canvas.LeftProperty, ShipPosX);
        }
    }
}