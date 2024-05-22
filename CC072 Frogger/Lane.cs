using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frogger
{
    internal class Lane
    {
        private Canvas my_Canvas;
        private Rectangle my_Rect;
        private Brush my_Color;
        private string my_Type;
        private int my_Index;
        private double my_Width;
        private double my_Height;
        private double my_Top;
        private List<Obstacle> Obstacles;
        private double my_Spacing;
        private double my_Speed;
        private Random Rnd = new Random();

        public Lane(string type, int index, Canvas canv)
        {
            my_Type = type;
            my_Index = index + 1;
            my_Canvas = canv;
            my_Width = canv.ActualWidth;
            my_Height = canv.ActualHeight / 13;
            my_Top = canv.ActualHeight - my_Index * my_Height;
            if (my_Type.Equals("SAVE")) { my_Color = Brushes.DarkGreen; }
            if (my_Type.Equals("CARS")) { my_Color = Brushes.Black; }
            if (my_Type.Equals("LOGS")) { my_Color = Brushes.Blue; }
            if (my_Type.Equals("TURTLES")) { my_Color = Brushes.Blue; }
        }

        public void SetObstacles(int count, int width, double speed, string imagetype)
        {
            Obstacle ob;
            List<BitmapImage> bmps = new List<BitmapImage>();
            int index;
            Obstacles = new List<Obstacle>();
            my_Spacing = (my_Canvas.ActualWidth - count * width)/(count - 1);
            my_Speed = speed;
            if (imagetype.Equals("TRUCKS"))
            {
                if (my_Speed > 0)
                {
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Truck1.Gif")));
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Truck2.Gif")));
                }
                else
                {
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Truck1Left.Gif")));
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Truck2Left.Gif")));
                }
            }
            if (imagetype.Equals("CARS"))
            {
                if (my_Speed > 0)
                {
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Car1.Gif")));
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Car2.Gif")));
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Car3.Gif")));
                }
                else
                {
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Car1Left.Gif")));
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Car2Left.Gif")));
                    bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Car3Left.Gif")));
                }
            }
            if (imagetype.Equals("LOG"))
            {
                bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Log1.Gif")));
            }
            if (imagetype.Equals("TURTLES"))
            {
                bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\Turtle1.Gif")));
            }
            if (imagetype.Equals("BUSHES"))
            {
                //Make the Bushes Lanes double high and add 4 Lotus leaves
                bmps.Add(new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\FrogNest.jpg")));
                BitmapImage background = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\sprites\\NoFrogNest.jpg"));
                my_Spacing = background.PixelWidth;
                my_Top = my_Top - my_Height - 8;
                my_Height = 2 * my_Height + 17;
                my_Speed = 0;
                double w = bmps[0].PixelWidth;
                double h = bmps[0].PixelHeight;
                width = (int)w;
                //Fill the lane with Bush background
                for (int i = 0; i < 9; i++)
                {
                    ob = new Obstacle(i * w, my_Top + 8, w, h + 8, 0, 0, background);
                    Obstacles.Add(ob);
                }
            }

            for (double i = 0.5; i < count; i+= 1)
            {
                index = Rnd.Next(bmps.Count);
                ob = new Obstacle((width + my_Spacing) * i, my_Top + 8, width, my_Height - 16, my_Spacing, speed, bmps[index]);
                Obstacles.Add(ob);
            }
        }

        public string LaneType
        {
            get { return my_Type; }
        }

        public double LaneSpeed
        {
            get { return my_Speed; }
        }

        public void Draw()
        {
            //Draw the Lane background
            my_Rect = new Rectangle()
            {
                Width = my_Width,
                Height = my_Height,
                Fill = my_Color
            };
            my_Rect.SetValue(Canvas.LeftProperty, 0.0);
            my_Rect.SetValue(Canvas.TopProperty, my_Top);
            my_Canvas.Children.Add(my_Rect);
            //Draw street markings
            Line L;
            if (my_Index > 1 && my_Index < 5)
            {
                L = new Line()
                {
                    X1 = 0,
                    Y1 = my_Top,
                    X2 = my_Canvas.ActualWidth,
                    Y2 = my_Top,
                    Stroke = Brushes.White,
                    StrokeThickness = 2.0,
                    StrokeDashArray = new DoubleCollection() { 20.0, 20.0 }
                };
                my_Canvas.Children.Add(L);
            }
            //Draw the Obstacles
            if (Obstacles != null)
            {
                for (int i = 0; i < Obstacles.Count; i++)
                {
                    Obstacles[i].Draw(my_Canvas);
                }
            }
        }

        public void Update()
        {
            if (Obstacles != null)
            {
                for (int i = 0; i < Obstacles.Count; i++)
                {
                    Obstacles[i].Update();
                }
            }
        }

        public int Collision(Frog frog)
        {
            if (my_Type.Equals("SAVE")) return -1;
            for (int i = 0; i < Obstacles.Count; i++)
            {
                if (Obstacles[i].Collision(frog)) return i;
            }
            return -1;
        }

        public int CenterCollision(Frog frog) //Used for BUSHES Lane
        {
            if (!my_Type.Equals("BUSHES")) return -1;
            for (int i = 0; i < Obstacles.Count; i++)
            {
                if (Obstacles[i].CenterCollision(frog)) return i;
            }
            return -1;
        }
    }
}
