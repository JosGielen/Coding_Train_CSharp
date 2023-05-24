using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Forward_Kinematics
{
    public partial class MainWindow : Window
    {
        private List<Flagellum> flags;
        private int SegmentCount = 20;
        private double FlagellumLength = 400;
        private int FlagellaCount = 8;
        private double YOff = 0.0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e )
        {
            flags = new List<Flagellum>();
            Flagellum fl;
            Vector loc;
            for (int I = 0; I < FlagellaCount; I++)
            {
                loc = new Vector((I + 1) / (double)(FlagellaCount + 1) * canvas1.ActualWidth, canvas1.ActualHeight - 20);
                fl = new Flagellum(loc, -90, SegmentCount, FlagellumLength, Brushes.Lime);
                fl.Show(canvas1);
                flags.Add(fl);
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
}

        private void CompositionTarget_Rendering(object sender, EventArgs e )
        {
            for (int I = 0; I < flags.Count; I++)
            {
                flags[I].Update(YOff);
            }
            YOff += 0.01;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e )
        {
            Environment.Exit(0);
        }

    }
}
