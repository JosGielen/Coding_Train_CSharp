using System.Windows;

namespace PerlinFlowField
{
    /// <summary>
    /// Interaction logic for FavoriteNameForm.xaml
    /// </summary>
    public partial class FavoriteNameForm : Window
    {
        private string my_Name = "";

        public FavoriteNameForm()
        {
            InitializeComponent();
        }

        public string FavoriteName
        {
            get { return my_Name; }
            set { my_Name = value; }
        }

        private void BtnOK_Click(Object sender, RoutedEventArgs e)
        {
            my_Name = TxtName.Text;
            DialogResult = true;
        }

        private void BtnCancel_Click(Object sender, RoutedEventArgs e)
        {
            my_Name = "";
            DialogResult = false;
        }
    }
}
