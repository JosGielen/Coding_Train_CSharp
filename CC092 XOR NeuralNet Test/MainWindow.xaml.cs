using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NeuralNet_Test
{
    public partial class MainWindow : Window
    {
        private readonly Random Rnd = new Random();
        private bool AppLoaded = false;
        private Settings mySetter;
        private readonly int inputNr = 2;
        private int hiddenNr = 4;
        private readonly int outputNr = 1;
        private double lr = 0.1;
        private readonly bool Normdist = false;
        private int TrainingCount = 30000;
        private int res = 100;
        private NeuralNet nn;
        private readonly TrainingData[] trainData = new TrainingData[4];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mySetter = new Settings(this, res, lr, hiddenNr, TrainingCount);
            nn = new NeuralNet(inputNr, hiddenNr, outputNr, lr, Normdist);
            trainData[0] = new TrainingData(0.01, 0.01, 0.01);
            trainData[1] = new TrainingData(0.99, 0.01, 0.99);
            trainData[2] = new TrainingData(0.01, 0.99, 0.99);
            trainData[3] = new TrainingData(0.99, 0.99, 0.01);
            mySetter.Left = Left + Width - 17;
            mySetter.Top = Top;
            mySetter.Show();
            Train(TrainingCount);
            AppLoaded = true;
        }

        public void Reset(int Size, double LearnRate, int Nodes, int Count)
        {
            //Make a new NeuralNet
            res = Size;
            lr = LearnRate;
            hiddenNr = Nodes;
            TrainingCount = Count;
            nn = new NeuralNet(inputNr, hiddenNr, outputNr, lr, Normdist);
            Train(TrainingCount);
        }

        public void Train(int Count)
        {
            int index;
            TrainingCount = Count;
            //Train the Neural net with random TrainingData
            for (int I = 0; I < TrainingCount; I++)
            {
                index = Rnd.Next(4);
                nn.Train(trainData[index].data, trainData[index].targets);
            }
            Draw(res);
        }

        private void Draw(int Size)
        {
            Rectangle rect;
            res = Size;
            byte grey;
            double[] inp = new double[inputNr];
            double[] outp;
            Canvas1.Children.Clear();
            for (int I = 0; I < res; I++)
            {
                for (int J = 0; J < res; J++)
                {
                    inp[0] = I / (double)(res - 1);
                    inp[1] = J / (double)(res - 1);
                    outp = nn.Query(inp);
                    grey = (byte)(255 * outp[0]);
                    rect = new Rectangle()
                    {
                        Width = Canvas1.ActualWidth / res + 1,
                        Height = Canvas1.ActualHeight / res + 1,
                        Fill = new SolidColorBrush(Color.FromRgb(grey, grey, grey))
                    };
                    rect.SetValue(Canvas.LeftProperty, I * Canvas1.ActualWidth / res);
                    rect.SetValue(Canvas.TopProperty, J * Canvas1.ActualHeight / res);
                    Canvas1.Children.Add(rect);
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppLoaded)
            {
                mySetter.Left = Left + Width - 17;
                mySetter.Top = Top;
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (AppLoaded)
            {
                mySetter.Left = Left + Width - 17;
                mySetter.Top = Top;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
