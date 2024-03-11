using System.Windows;
using System.Windows.Input;

namespace WinLife
{
    public partial class CoordinateDialog : Window
    {
        private int Xtot;
        private int Ytot;
        private Point pt = new Point(0, 0);

        public CoordinateDialog(int Xtotal, int Ytotal)
        {
            Xtot = Xtotal;
            Ytot = Ytotal;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtXtotal.Text = "/" + Xtot.ToString();
            TxtYtotal.Text = "/" + Ytot.ToString();
            TxtX.Focus();
        }

        private void TxtX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.D0) e.Handled = true;
            if (e.Key > Key.D9 & e.Key < Key.NumPad0) e.Handled = true;
            if (e.Key > Key.NumPad9) e.Handled = true;
        }

        private void TxtY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.D0) e.Handled = true;
            if (e.Key > Key.D9 & e.Key < Key.NumPad0) e.Handled = true;
            if (e.Key > Key.NumPad9) e.Handled = true;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pt.X = int.Parse(TxtX.Text);
                pt.Y = int.Parse(TxtY.Text);
            }
            catch
            {
                MessageBox.Show("Invalid Coordinates!", "Coordinate Dialog Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                DialogResult = false;
                Close();
                return;
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public Point Getcoordinate()
        {
            return pt;
        }
    }
}
