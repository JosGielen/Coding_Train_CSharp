using System.Windows;

namespace NeuralNet_Test
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private MainWindow my_Parent;
        private int GridSize;
        private double LearningRate;
        private int HiddenNodes;
        private int TrainCount;

        public Settings(MainWindow parent, int size, double LearnRate, int Nodes, int count)
        {
            InitializeComponent();
            my_Parent = parent;
            GridSize = size;
            LearningRate = LearnRate;
            HiddenNodes = Nodes;
            TrainCount = count;
            TxtGridSize.Text = GridSize.ToString();
            TxtLR.Text = LearningRate.ToString();
            TxtHidden.Text = HiddenNodes.ToString();
            TxtTrain.Text = TrainCount.ToString();
        }

        private void BtnTrain_Click(object sender, RoutedEventArgs e)
        {
            TrainCount = int.Parse(TxtTrain.Text);
            my_Parent.Train(TrainCount);
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            GridSize = int.Parse(TxtGridSize.Text);
            LearningRate = double.Parse(TxtLR.Text);
            HiddenNodes = int.Parse(TxtHidden.Text);
            my_Parent.Reset(GridSize, LearningRate, HiddenNodes, TrainCount);
        }
    }
}
