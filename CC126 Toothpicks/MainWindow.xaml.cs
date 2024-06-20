using System.Windows;
using System.Windows.Media;

namespace Toothpick_Pattern
{
    public partial class MainWindow : Window
    {
        private double W;
        private int my_Length = 50;
        private List<Toothpick> toothpicks = new List<Toothpick>();
        private List<Toothpick> newToothpicks = new List<Toothpick>();
        private int frameCount;
        private List<Color> my_Colors;
        private bool Started = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            W = canvas1.ActualWidth;
            //Load the colors
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            my_Colors = pal.GetColors(32);
        }

        private void Init()
        {
            canvas1.Children.Clear();
            toothpicks = new List<Toothpick>();
            newToothpicks = new List<Toothpick>();
            Toothpick first = new Toothpick((int)canvas1.ActualWidth / 2, (int)canvas1.ActualHeight / 2, my_Length, false, my_Colors[0]);
            newToothpicks.Add(first);
            toothpicks.Add(first);
            first.Draw(canvas1, 2.0);
            frameCount = 0;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            int min = int.MaxValue;
            int max = int.MinValue;
            double scaleFactor = 1.0;
            for (int i = 0; i < newToothpicks.Count; i++)
            {
                if (newToothpicks[i].End1.X < min) { min = (int)newToothpicks[i].End1.X; }
                if (newToothpicks[i].End2.X > max) { max = (int)newToothpicks[i].End2.X; }
            }
            if (0.8 * W < max - min) { scaleFactor = 0.8 * W / (max - min); }

            for (int i = 0; i < toothpicks.Count; i++)
            {
                toothpicks[i].CheckEnds(toothpicks);
            }
            newToothpicks.Clear();
            for (int i = 0; i < toothpicks.Count; i++)
            {
                if (toothpicks[i].End1Free)
                {
                    newToothpicks.Add(new Toothpick((int)toothpicks[i].End1.X, (int)toothpicks[i].End1.Y, my_Length, !toothpicks[i].Horizontal, my_Colors[frameCount % my_Colors.Count]));
                }
                if (toothpicks[i].End2Free)
                {
                    newToothpicks.Add(new Toothpick((int)toothpicks[i].End2.X, (int)toothpicks[i].End2.Y, my_Length, !toothpicks[i].Horizontal, my_Colors[frameCount % my_Colors.Count]));
                }
            }
            for (int i = 0; i < newToothpicks.Count; i++)
            {
                newToothpicks[i].Draw(canvas1, 1.0 / scaleFactor);
            }
            toothpicks.AddRange(newToothpicks);
            canvas1.RenderTransform = new ScaleTransform(scaleFactor, scaleFactor, canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            frameCount++;
            if (frameCount % 20 == 0)
            {
                canvas1.Children.Clear();
                for (int i = 0; i < toothpicks.Count; i++)
                {
                    toothpicks[i].Draw(canvas1, 1.0 / scaleFactor);
                }
            }
            Thread.Sleep(100);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Started)
            {
                Started = true;
                BtnStart.Content = "STOP";
                Init();
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                Started = false;
                BtnStart.Content = "START";
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

    }
}