using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Purple_Rain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Drop> Rain;
        private int DropCount = 500;
        private Random rnd = new Random();
        private bool Rendering = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Drop d;
            Rain = new List<Drop>();
            for (int I = 0; I < DropCount; I++)
            {
                d = new Drop(canvas1.ActualWidth * rnd.NextDouble(), canvas1.ActualHeight * rnd.NextDouble());
                d.Show(canvas1);
                Rain.Add(d);
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            for (int I = 0; I < Rain.Count; I++)
            {
                Rain[I].Update();
            }
        }

        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}