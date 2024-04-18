using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Asteroids
{
    public partial class MainWindow : Window
    {
        private Ship my_Ship;
        private int ShipEnergy;
        private int ShipLife;
        private double ShipSpeed = 0.2;
        private double ShipAngleStep = 8.0;
        private List<Asteroid> Asteroids;
        private int AsteroidCount = 10;
        private double AsteroidSpeed = 1.2;
        private List<Bullet> ShipBullets;
        private double BulletSpeed = 5.0;
        private List<Explosion> Explosions;
        private int My_Score;
        private Random Rnd = new Random();

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
            Explosions = new List<Explosion>();
            ShipBullets = new List<Bullet>();
            //Create a Ship
            Vector ShipPos = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2 + 200);
            ShipEnergy = 100;
            ShipLife = 10;
            my_Ship = new Ship(ShipPos, ShipLife, Environment.CurrentDirectory + "\\Ship.jpg");
            my_Ship.Draw(canvas1);
            My_Score = 0;
            //Create the Asteroids
            Asteroid ast;
            Asteroids = new List<Asteroid>();
            Vector pos;
            Vector dir;
            for (int i = 0; i < AsteroidCount; i++)
            {
                do
                {
                    pos = new Vector(Rnd.NextDouble() * canvas1.ActualWidth, Rnd.NextDouble() * canvas1.ActualHeight);
                } while(pos.X < 60 || pos.X + 60 > canvas1.ActualWidth || pos.Y < 60 || pos.Y + 60 > canvas1.ActualHeight || (pos - ShipPos).Length < 100);
                dir = new Vector(Rnd.NextDouble() - 0.5, Rnd.NextDouble() - 0.5);
                ast = new Asteroid(pos, 2 * dir, 20 * Rnd.NextDouble() + 40, AsteroidSpeed);
                ast.Draw(canvas1);
                Asteroids.Add(ast);
            }
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
            //Check for Bullet Hits on Asteroids
            for (int j = 0; j < Asteroids.Count; j++)
            {
                for (int i = ShipBullets.Count - 1; i >= 0; i--)
                {
                    if (ShipBullets[i].Hit(Asteroids[j]))
                    {
                        Explosion ex = new Explosion(ShipBullets[i].Pos, false);
                        Explosions.Add(ex);
                        ex.Draw(canvas1);
                        ShipBullets[i].Erase(canvas1);
                        ShipBullets.RemoveAt(i);
                        Asteroids[j].Life--;
                    }
                }
                if (Asteroids[j].Life <= 0)
                {
                    Explosion ex = new Explosion(Asteroids[j].Position, true);
                    Explosions.Add(ex);
                    ex.Draw(canvas1);
                    Asteroids[j].Erase(canvas1);
                    List<Asteroid> newAsteroids = Asteroids[j].Split();
                    if (newAsteroids != null)
                    {
                        newAsteroids[0].Draw(canvas1);
                        newAsteroids[1].Draw(canvas1);
                        Asteroids.AddRange(newAsteroids);
                    }
                    Asteroids.RemoveAt(j);
                    My_Score += 10;
                    Title = "Asteroids score = " + My_Score.ToString();
                    //Create a new Asteroid when needed
                    if (Asteroids.Count < 10)
                    {
                        Asteroid ast;
                        Vector pos;
                        Vector dir;
                        AsteroidSpeed += 0.2;
                        do
                        {
                            pos = new Vector(Rnd.NextDouble() * canvas1.ActualWidth, Rnd.NextDouble() * canvas1.ActualHeight);
                        } while (pos.X < 60 || pos.X + 60 > canvas1.ActualWidth || pos.Y < 60 || pos.Y + 60 > canvas1.ActualHeight || (pos - my_Ship.Position).Length < 100);
                        dir = new Vector(Rnd.NextDouble() - 0.5, Rnd.NextDouble() - 0.5);
                        ast = new Asteroid(pos, 2 * dir, 20 * Rnd.NextDouble() + 40, AsteroidSpeed);
                        ast.Draw(canvas1);
                        Asteroids.Add(ast);
                    }
                }
            }
            //Update the Asteroids
            {
                for (int i = 0; i < Asteroids.Count; i++)
                {
                    Asteroids[i].Update();
                    //Loop Asteroids over the edges
                    if (Asteroids[i].Position.X < -Asteroids[i].Radius -10)
                    {
                        Asteroids[i].Position = new Vector(canvas1.ActualWidth + Asteroids[i].Radius, Asteroids[i].Position.Y);
                    }
                    if (Asteroids[i].Position.Y < -Asteroids[i].Radius - 10)
                    {
                        Asteroids[i].Position = new Vector(Asteroids[i].Position.X, canvas1.ActualHeight + Asteroids[i].Radius);
                    }
                    if (Asteroids[i].Position.X > canvas1.ActualWidth + Asteroids[i].Radius + 10)
                    {
                        Asteroids[i].Position = new Vector(-Asteroids[i].Radius, Asteroids[i].Position.Y);
                    }
                    if (Asteroids[i].Position.Y > canvas1.ActualHeight + Asteroids[i].Radius + 10)
                    {
                        Asteroids[i].Position = new Vector(Asteroids[i].Position.X, -Asteroids[i].Radius);
                    }
                }
            }
            //Check for Ship to Asteroid collision
            for (int i = 0; i < Asteroids.Count; i++)
            {
                if (ShipLife >= 0 && (Asteroids[i].Position - my_Ship.Position).Length < 0.9 * my_Ship.Shield[9].ActualWidth)
                {
                    Vector v = Asteroids[i].Position - my_Ship.Position;
                    v.Normalize();
                    v *= my_Ship.Shield[9].ActualWidth / 2;
                    Explosion ex = new Explosion(my_Ship.Position + v, false);
                    Explosions.Add(ex);
                    ex.Draw(canvas1);
                    ShipLife -= 1;
                    //Push the ship away from the Asteroid
                    v *= -1;
                    my_Ship.Position += v;
                    //Update Ship Shield
                    for (int k = 9; k >= 0; k--)
                    {
                        if (ShipLife <= k && canvas1.Children.Contains(my_Ship.Shield[k])) canvas1.Children.Remove(my_Ship.Shield[k]);
                    }
                    if (ShipLife < 0)
                    {
                        //Game Over
                        ex = new Explosion(my_Ship.Position, true);
                        Explosions.Add(ex);
                        ex.Draw(canvas1);
                        my_Ship.Erase(canvas1);
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
            //Update the Ship
            my_Ship.Update();
            //Loop the Ship over the edges
            double maxDist = my_Ship.Shield[9].ActualWidth;
            if (my_Ship.Position.X < -maxDist - 10)
            {
                my_Ship.Position = new Vector(canvas1.ActualWidth + maxDist, my_Ship.Position.Y);
            }
            if (my_Ship.Position.Y < -maxDist - 10)
            {
                my_Ship.Position = new Vector(my_Ship.Position.X, canvas1.ActualHeight + maxDist);
            }
            if (my_Ship.Position.X > canvas1.ActualWidth + maxDist + 10)
            {
                my_Ship.Position = new Vector(-maxDist, my_Ship.Position.Y);
            }
            if (my_Ship.Position.Y > canvas1.ActualHeight + maxDist + 10)
            {
                my_Ship.Position = new Vector(my_Ship.Position.X, -maxDist);
            }

            //Reload the Ship energy
            if (ShipEnergy < 50)
            {
                ShipEnergy++;
            }
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
            switch (e.Key)
            {
                case Key.Up:
                    my_Ship.Accelerate(ShipSpeed);
                    break;
                case Key.Down:
                    my_Ship.Accelerate(-1 * ShipSpeed);
                    break;
                case Key.Left:
                    my_Ship.Rotate(-1 * ShipAngleStep);
                    break;
                case Key.Right:
                    my_Ship.Rotate(ShipAngleStep);
                    break;
                case Key.Space:
                    //Fire
                    if (ShipEnergy > 10 && ShipLife >= 0)
                    {
                        Bullet b = new Bullet(my_Ship.Position, BulletSpeed * my_Ship.Direction);
                        ShipBullets.Add(b);
                        b.Draw(canvas1);
                        ShipEnergy -= 10;
                    }
                    break;
            }
        }
    }
}