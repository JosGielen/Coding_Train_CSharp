using System.Windows;

namespace Logo
{
    public partial class Legend : Window
    {
        private readonly MainWindow myParent;

        public Legend(MainWindow parent)
        {
            InitializeComponent();
            myParent = parent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtLegend.Text = "Some Commands must be followed by a numeric value as indicated below.\n";
            TxtLegend.Text += "Spaces are ignored but will increase readability.\n\n";
            TxtLegend.Text += "setX value : Set the X-coordinate of the turtle to value.\n";
            TxtLegend.Text += "setY value : Set the Y-coordinate of the turtle to value.\n";
            TxtLegend.Text += "size value : Set the line thickness to value.\n";
            TxtLegend.Text += "home : Reset the Turtle to its start position.\n";
            TxtLegend.Text += "col [r,g,b] : Set the color of the turtle (r,g,b between 0 - 255)\n";
            TxtLegend.Text += "fd value : Moves the turtle value pixels Forward.\n";
            TxtLegend.Text += "bd value : Moves the turtle value pixels Back.\n";
            TxtLegend.Text += "rt value : Rotates the turtle value degrees Clockwise\n";
            TxtLegend.Text += "lt value : Rotates the turtle value degrees Counter Clockwise\n";
            TxtLegend.Text += "pu : Lift the pen up (the turtle does not draw when moved).\n";
            TxtLegend.Text += "pd : Lowers the pen down (the turtle draws a line when moved).\n";
            TxtLegend.Text += "repeat value [cmds] : Repeats the cmds value times.\n";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            myParent.UncheckMnuLegend();
            Hide();
        }
    }
}
