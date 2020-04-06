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
using System.Windows.Shapes;

namespace _7SegmentDisplay
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private MainWindow my_Parent;
        public double Border;
        public double Thickness;
        public double Campher;
        public double Space;
        public bool HasCampher;
        public bool IsTilted;
        public Brush BackColor;
        public Brush SegmentColor;
        private List<Brush> My_Brushes;

        public Settings(MainWindow parent)
        {
            InitializeComponent();
            my_Parent = parent; 
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load the WPF default colors in the Color comboboxes.
            My_Brushes = new List<Brush>();
            BrushConverter bc = new BrushConverter();
            foreach (System.Reflection.PropertyInfo propinfo in typeof(Colors).GetProperties())
            {
                CmbSegmentColor.Items.Add(propinfo.Name);
                CmbBackgroundColor.Items.Add(propinfo.Name);
                My_Brushes.Add((Brush)bc.ConvertFromString(propinfo.Name));
            }
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            Border = double.Parse(TxtBorder.Text);
            Thickness = double.Parse(TxtThickness.Text);
            Campher = double.Parse(TxtCampher.Text);
            Space = double.Parse(TxtSpace.Text);
            HasCampher = CbHasCampher.IsChecked.Value;
            IsTilted = CbIsTilted.IsChecked.Value;
            BackColor = My_Brushes[CmbBackgroundColor.SelectedIndex];
            SegmentColor = My_Brushes[CmbSegmentColor.SelectedIndex];
            my_Parent.UpdateDisplays();
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            my_Parent.SetDefault();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
