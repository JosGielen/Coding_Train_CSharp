using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inverse_Kinematics_Multiple
{

    public partial class MainWindow : Window
    {
        private List<Flagellum> flags;
        private int SegmentCount = 20;
        private double FlagellumLength = 300;
        private int FlagellaCount = 2;
        private Vector MousePos;
        private Vector BallPos;
        private Vector BallDir;
        private double BallSpeed;
        private double BallSize;
        private Vector Gravity;
        private Ellipse Ball;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            flags = new List<Flagellum>();
            Flagellum fl;
            Vector loc;
            for (int I = 0; I < FlagellaCount; I++)
            {
                loc = new Vector(I / (FlagellaCount - 1) * (canvas1.ActualWidth - 40) + 20, canvas1.ActualHeight - 5);
                fl = new Flagellum(loc, SegmentCount, FlagellumLength, Brushes.Yellow);
                fl.Show(canvas1);
                flags.Add(fl);
            }
            BallPos = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 3);
            BallDir = new Vector(2, Rnd.NextDouble());
            BallSpeed = 3.0;
            BallSize = 30.0;
            Gravity = new Vector(0, 0.03);
            Ball = new Ellipse()
            {
                Width = BallSize,
                Height = BallSize,
                Stroke = Brushes.Red,
                StrokeThickness = 1.0,
                Fill = Brushes.Red
            };
            canvas1.Children.Add(Ball);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            for (int I = 0; I < flags.Count; I++)
            {
                flags[I].Follow(BallPos);
            }
            Ball.SetValue(Canvas.LeftProperty, BallPos.X - BallSize / 2);
            Ball.SetValue(Canvas.TopProperty, BallPos.Y - BallSize / 2);
            BallDir += Gravity;
            BallPos += BallSpeed * BallDir;
            if (BallPos.X < 0)
            {
                BallPos.X = 0;
                BallDir.X = -1 * BallDir.X;
            }
            if (BallPos.X > canvas1.ActualWidth)
            {
                BallPos.X = canvas1.ActualWidth;
                BallDir.X = -1 * BallDir.X;
            }
            if (BallPos.Y < 0)
            {
                BallPos.Y = 0;
                BallDir.Y = -1 * BallDir.Y;
            }
            if (BallPos.Y > canvas1.ActualHeight)
            {
                BallPos.Y = canvas1.ActualHeight;
                BallDir.Y = -1 * BallDir.Y;
            }
        }
    }
}
