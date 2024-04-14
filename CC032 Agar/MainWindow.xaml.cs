using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Agar
{
    public partial class MainWindow : Window
    {
        private Blob MainBlob;
        private List<Blob> Blobs;
        private List<Hunter> Hunters;
        private Ellipse border;
        private double borderDiameter;
        private Point Center;
        private double borderTop;
        private double borderLeft;
        private bool mouseDown = false;
        private List<Color> my_Colors;
        private double My_Score;
        private double my_Speed;
        private long counter;
        private bool GameOn;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ColorPalette my_palette = new ColorPalette(Environment.CurrentDirectory + "\\AgentLife.cpl");
            my_Colors = my_palette.GetColors(101);
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void Init()
        {
            canvas1.Children.Clear();
            My_Score = 0;
            my_Speed = 1.5;
            counter = 0;
            borderDiameter = 3 * canvas1.ActualHeight;
            //Draw the Petridish border
            border = new Ellipse()
            {
                Width = borderDiameter,
                Height = borderDiameter,
                Stroke = Brushes.Black,
                StrokeThickness = 15.0,
                Fill = Brushes.Beige
            };
            borderLeft = - borderDiameter / 2 + canvas1.ActualWidth / 2;
            borderTop = - borderDiameter / 3;
            border.SetValue(Canvas.LeftProperty, borderLeft);
            border.SetValue(Canvas.TopProperty, borderTop);
            canvas1.Children.Add(border);
            //Create the player Blob at the center
            Center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            MainBlob = new Blob(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2, 32, my_Colors[100]);
            MainBlob.Energy = 100;
            MainBlob.Draw(canvas1);
            //Create the food Blobs inside the Petridish border
            Blob b;
            double x, y, dist;
            Blobs = new List<Blob>();
            for (int i = 0; i < 120; i++)
            {
                do
                {
                    x = canvas1.ActualHeight * (3 * Rnd.NextDouble() - 1);
                    y = canvas1.ActualHeight * (3 * Rnd.NextDouble() - 1);
                    dist = (x - Center.X) * (x - Center.X) + (y - Center.Y) * (y - Center.Y);
                } while (Math.Sqrt(dist) > 1.5 * canvas1.ActualHeight - 50);
                b = new Blob(x, y, 12, Colors.Green);
                Blobs.Add(b);
                b.Draw(canvas1);
            }
            //Create the Hunters
            Hunter h;
            Hunters = new List<Hunter>();
            for (int i = 0; i < 15; i++)
            {
                do
                {
                    x = canvas1.ActualHeight * (3 * Rnd.NextDouble() - 1);
                    y = canvas1.ActualHeight * (3 * Rnd.NextDouble() - 1);
                    dist = (x - Center.X) * (x - Center.X) + (y - Center.Y) * (y - Center.Y);
                } while (Math.Sqrt(dist) > 1.5 * canvas1.ActualHeight - 50 || Math.Sqrt(dist) < 300);
                h = new Hunter(x, y, 40, Colors.Crimson);
                Hunters.Add(h);
                h.Draw(canvas1);
                GameOn = true;
            }
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (GameOn == false) return;
            if (mouseDown && MainBlob.Energy > 0)
            {
                counter++;
                if (counter % 1000 == 0) { my_Speed += 0.05; }
                //Keep the MainBlob centered and move the world with the mouse
                Vector v = Mouse.GetPosition(canvas1) - MainBlob.Location;
                v.Normalize();
                v = 3.0 * my_Speed * v;
                //Restrict the Mainblob inside the Petridish
                Point newCenter = Center - v;
                Point canvasCenter = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
                if ((newCenter - canvasCenter).Length < 1.5 * canvas1.ActualHeight - MainBlob.Radius - 10)
                {
                    Center = newCenter;
                    for (int i = 0; i < Blobs.Count; i++)
                    {
                        Blobs[i].Location -= v;
                    }
                    for (int i = 0; i < Hunters.Count; i++)
                    {
                        Hunters[i].Location -= v;
                    }
                    borderLeft -= v.X;
                    borderTop -= v.Y;
                    border.SetValue(Canvas.LeftProperty, borderLeft);
                    border.SetValue(Canvas.TopProperty, borderTop);
                }
            }
            //Move the hunters
            for (int i = Hunters.Count - 1; i >= 0; i--)
            {
                if ((MainBlob.Location - Hunters[i].Location).Length < 600)
                {
                    Vector vh = MainBlob.Location - Hunters[i].Location;
                    vh.Normalize();
                    vh = my_Speed * vh;
                    Hunters[i].Location += vh;
                }
                else
                {
                    //Move the hunter in a random direction
                    Hunters[i].Location += 1.5 * my_Speed * Hunters[i].Dir;
                    if ((Hunters[i].Location - Center).Length > borderDiameter / 2 - 100)
                    {
                        Hunters[i].Location -= 2 * my_Speed * Hunters[i].Dir;
                        Hunters[i].Dir = new Vector(2 * Rnd.NextDouble() - 1, 2 * Rnd.NextDouble() - 1);
                    }
                }
            }
            //Check collision with a hunter
            for (int i = Hunters.Count - 1; i >= 0; i--)
            {
                if ((MainBlob.Location - Hunters[i].Location).Length < MainBlob.Radius + Hunters[i].Radius - 10)
                {
                    //Game Over due to killed by hunter
                    MainBlob.Color = Colors.Black;
                    MainBlob.Energy = 0;
                    GameOn = false;
                    TextBox txt = new TextBox()
                    {
                        Width = canvas1.ActualWidth,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Text = "Game Over. Score = " + My_Score.ToString(),
                        FontSize = 48,
                        FontWeight = FontWeights.Bold,
                        Background = Brushes.Yellow
                    };
                    txt.SetValue(Canvas.LeftProperty, 0.0);
                    txt.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 80);
                    canvas1.Children.Add(txt);
                    return;
                }
            }
            //Reduce the Mainblob Energy and show this by color change
            if (MainBlob.Energy > 0)
            {
                MainBlob.Energy -= 0.1;
                MainBlob.Color = my_Colors[(int)MainBlob.Energy];
            }
            else  //Game Over due to starvation
            {
                MainBlob.Color = Colors.Black;
                GameOn = false;
                TextBox txt = new TextBox()
                {
                    Width = canvas1.ActualWidth,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Text = "Game Over. Score = " + My_Score.ToString(),
                    FontSize = 48,
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.Yellow
                };
                txt.SetValue(Canvas.LeftProperty, 0.0);
                txt.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 80);
                canvas1.Children.Add(txt);
                return;
            }
            //Check for food collision
            double x, y, dist;
            for (int i = Blobs.Count - 1; i >= 0; i--)
            {
                if ((MainBlob.Location - Blobs[i].Location).Length < MainBlob.Radius + Blobs[i].Radius)
                {
                    //Eat the food Blob
                    MainBlob.Radius = Math.Sqrt(MainBlob.Radius * MainBlob.Radius + 0.5 * Blobs[i].Radius * Blobs[i].Radius);
                    My_Score += 5;
                    if (MainBlob.Energy < 95)
                    {
                        MainBlob.Energy += 5;
                        MainBlob.Color = my_Colors[(int)MainBlob.Energy];
                    }
                    do
                    {
                        x = canvas1.ActualHeight * (3 * Rnd.NextDouble() - 1);
                        y = canvas1.ActualHeight * (3 * Rnd.NextDouble() - 1);
                        dist = (x - Center.X) * (x - Center.X) + (y - Center.Y) * (y - Center.Y);
                    } while (Math.Sqrt(dist) > 1.5 * canvas1.ActualHeight - 50);

                    Blobs[i].Location = new Point(x, y);
                }
            }
        }

        private void MnuNew_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }
    }
}