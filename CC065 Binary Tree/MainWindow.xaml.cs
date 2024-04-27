using System.Windows;

namespace Binary_Tree
{
    public partial class MainWindow : Window
    {
        private BinaryTree tree;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tree = new BinaryTree(canvas1.ActualWidth / 2, canvas1.ActualHeight - 30);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            double num = double.Parse(TxtNumber.Text);
            tree.Add(num, (5 * num).ToString(), canvas1);
        }
    }
}