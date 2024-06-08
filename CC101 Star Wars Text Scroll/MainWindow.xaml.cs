using JG_GL;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Star_Wars_Intro_Text
{
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate();
        private int WaitTime;
        private TextBlock Text1;
        private Image image1;
        private TextGeometry Text2;
        private double Text2Y;
        private DateTime StartTime;
        private int Step;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string Intro;
            using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\IntroText.txt"))
            {
                Intro = sr.ReadToEnd();
            }
            Text1 = new TextBlock()
            {
                Foreground = new SolidColorBrush(Color.FromRgb(101, 206, 208)),
                Background = Brushes.Black,
                FontSize = 48,
                Width = 660,
                Height = 150,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Text = "A long time ago in a galaxy far, far away...."
            };
            Text1.SetValue(Canvas.LeftProperty, (canvas1.ActualWidth - Text1.Width) / 2);
            Text1.SetValue(Canvas.TopProperty, (canvas1.ActualHeight - Text1.Height) / 2);
            canvas1.Children.Add(Text1);
            image1 = new Image()
            {
                Width = canvas1.ActualWidth,
                Height = canvas1.ActualHeight,
                Stretch = Stretch.Fill,
                Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Logo.jpg")),
            };
            image1.Visibility = Visibility.Hidden;
            canvas1.Children.Add(image1);
            UpdateLayout();

            //3D Set-up for the Text scroll
            Vector3D CamStart = new Vector3D(0, -10, 4);
            Vector3D CamTarget = new Vector3D(0, 5, 0);
            Vector3D CamUp = new Vector3D(0,1,0);
            scene1.Camera = new FixedCamera(CamStart, CamTarget, CamUp);
            scene1.ViewDistance= 30;
            Text2 = new TextGeometry(Intro)
            {
                Position = new Vector3D(0.5, -11, 0),
                FontSize = 42,
                Width = 9.0,
                ForeColor = Color.FromRgb(238,213,75)
            };
            scene1.AddGeometry(Text2);
            Step = 1;
            WaitTime = 5000;
            StartTime = DateTime.Now;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if ((DateTime.Now - StartTime).TotalMilliseconds < WaitTime)
            {
                Dispatcher.Invoke(new WaitDelegate(Wait), DispatcherPriority.ApplicationIdle);
                return;
            }
            //Wait before the next Step
            if (Step == 1)
            {
                Text1.Visibility = Visibility.Hidden;
                image1.Visibility = Visibility.Visible;
                if (image1.Width > 20)
                {
                    image1.Width *= 0.992;
                    image1.Height *= 0.992;
                    image1.SetValue(Canvas.LeftProperty, (canvas1.ActualWidth - image1.Width) / 2);
                    image1.SetValue(Canvas.TopProperty, (canvas1.ActualHeight - image1.Height) / 2);
                }
                else
                {
                    Step = 2;
                }
            }
            else if (Step == 2)
            {

                canvas1.Visibility = Visibility.Hidden;
                scene1.Visibility = Visibility.Visible;
                Text2.Position = new Vector3D(Text2.Position.X, Text2.Position.Y + 0.007, Text2.Position.Z);
                scene1.Render();
            }
        }

        private void Wait()
        {
            Thread.Sleep(20);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}