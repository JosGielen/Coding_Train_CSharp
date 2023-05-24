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

namespace Slide_Puzzle
{
    /// <summary>
    /// Interaction logic for GridSettings.xaml
    /// </summary>
    public partial class GridSettings : Window
    {
        public int Rows = 4;
        public int Columns = 4;

        public GridSettings(int rows, int columns)
        {
            InitializeComponent();
            Rows = rows;
            Columns = columns;
            TxtRowNum.Text = rows.ToString();
            TxtColNum.Text = columns.ToString();
        }

        private void BtnColUP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Columns = int.Parse(TxtColNum.Text);
                Columns++;
                if (Columns > 6) Columns = 6;
                TxtColNum.Text = Columns.ToString();
            }
            catch
            {
                //Do nothing
            }
        }

        private void BtnColDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Columns = int.Parse(TxtColNum.Text);
                Columns--;
                if (Columns < 2) Columns = 2;
                TxtColNum.Text = Columns.ToString();
            }
            catch
            {
                //Do nothing
            }
        }

        private void BtnRowUP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Rows = int.Parse(TxtRowNum.Text);
                Rows++;
                if (Rows > 6) Rows = 6;
                TxtRowNum.Text = Rows.ToString();
            }
            catch
            {
                //Do nothing
            }
        }

        private void BtnRowDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Rows = int.Parse(TxtRowNum.Text);
                Rows--;
                if (Rows < 2) Rows = 2;
                TxtRowNum.Text = Rows.ToString();
            }
            catch
            {
                //Do nothing
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
