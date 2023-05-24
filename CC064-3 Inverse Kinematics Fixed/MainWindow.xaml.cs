using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Inverse_Kinematics_Fixed
{
    public partial class MainWindow : Window
    {
        private List<Flagellum> flags;
        private int SegmentCount = 30;
        private double FlagellumLength = 400;
        private int FlagellaCount = 1;
        private Vector MousePos;

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
                loc = new Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight - 20);
                fl = new Flagellum(loc, SegmentCount, FlagellumLength, Brushes.Lime);
                fl.Show(canvas1);
                flags.Add(fl);
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
}

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            for (int I = 0; I < flags.Count; I++)
            {
                flags[I].Follow(MousePos);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            MousePos = (Vector)e.GetPosition(canvas1);
        }
    }
}
