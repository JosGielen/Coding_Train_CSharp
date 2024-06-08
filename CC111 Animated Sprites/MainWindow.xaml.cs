using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Animated_Sprites
{
    public partial class MainWindow : Window
    {
        private List<Sprite> my_Sprites;
        private int[] lap;
        private int maxLaps = 4;
        private double FinishX;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            Line l;
            Sprite sp;
            Label lbl;
            canvas1.Children.Clear();
            my_Sprites = new List<Sprite>();
            for (int i = 0; i < 4; i++)
            {
                sp = new Sprite(Environment.CurrentDirectory + "\\Images\\Running Horse.jpg", 4, 3);
                sp.Speed = 2 * Rnd.NextDouble() + 6.0;
                sp.SetValue(Canvas.LeftProperty, 0.0);
                sp.SetValue(Canvas.TopProperty, i * (canvas1.ActualHeight) / 4 - 20);
                my_Sprites.Add(sp);
                canvas1.Children.Add(sp);
            }
            for (int i = 0; i < 5; i++)
            {
                l = new Line()
                {
                    X1 = 0,
                    Y1 = i * canvas1.ActualHeight / 4,
                    X2 = canvas1.ActualWidth,
                    Y2 = i * canvas1.ActualHeight / 4,
                    Stroke = Brushes.Brown,
                    StrokeThickness = 6.0
                };
                canvas1.Children.Add(l);
            }
            FinishX = 0.9 * canvas1.ActualWidth; 
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            lap = new int[4];
            BtnStart.IsEnabled = false;
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            double X;
            for (int i = 0; i < my_Sprites.Count; i++)
            {
                my_Sprites[i].Update();
                X = (double)my_Sprites[i].GetValue(Canvas.LeftProperty) + my_Sprites[i].Speed;
                if (X > canvas1.ActualWidth + my_Sprites[i].ActualWidth / 2 )
                {
                    X = -1 * my_Sprites[i].ActualWidth + 30;
                    lap[i]++;
                    my_Sprites[i].Speed = 2 * Rnd.NextDouble() + 6.0;
                }
                my_Sprites[i].SetValue(Canvas.LeftProperty, X);
                if (lap.Min() == maxLaps) 
                {
                    Line fin = new Line()
                    {
                        X1 = FinishX,
                        Y1 = 0,
                        X2 = FinishX,
                        Y2 = canvas1.ActualHeight,
                        Stroke = Brushes.Red,
                        StrokeThickness = 4.0
                    };
                    canvas1.Children.Add(fin);
                }
                if (lap[i] == maxLaps && X + my_Sprites[i].ActualWidth - 15 > FinishX)
                {
                    TextBox txt = new TextBox()
                    {
                        Width = canvas1.ActualWidth,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Text = "Horse number " + (i + 1).ToString() + " won the Race.",
                        FontSize = 24,
                        FontWeight = FontWeights.Bold,
                        Background = Brushes.Yellow
                    };
                    txt.SetValue(Canvas.LeftProperty, 0.0);
                    txt.SetValue(Canvas.TopProperty, canvas1.ActualHeight / 2 - 15);
                    canvas1.Children.Add(txt);
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                    BtnStart.IsEnabled = true;
                    return;
                }
            }
            Thread.Sleep(50);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}