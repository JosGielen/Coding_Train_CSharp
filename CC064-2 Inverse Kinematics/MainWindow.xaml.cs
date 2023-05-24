using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inverse_Kinematics
{
    public partial class MainWindow : Window
    {
        private Flagellum fl;
        private int SegmentCount = 30;
        private double FlagellumLength = 400;
        private Vector MousePos;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fl = new Flagellum(SegmentCount, FlagellumLength);
            fl.Show(canvas1);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            MousePos = (Vector)e.GetPosition(canvas1);
            fl.Follow(MousePos);
        }
    }
}
