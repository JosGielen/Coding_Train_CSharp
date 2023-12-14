using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PerlinFlowField
{
    public partial class Settings : Window
    {
        private Window MyMain;
        private string my_Favoritesfile;
        private List<Favorite> FavoriteSettings;
        public double CellSize = 0.0;
        public int ParticleCount = 0;
        public double MaxForce = 0.0;
        public double Speed = 0.0;
        public double XYChange = 0.0;
        public double ZChange = 0.0;
        public byte TrailLength = 0;
        public bool RandomSpawn = false;
        public bool UseColor = false;

        public Settings(Window main)
        {
            InitializeComponent();
            MyMain = main;
            //Load Favorites from file
            my_Favoritesfile = Environment.CurrentDirectory + "\\Favorites.txt";
            FavoriteSettings = new List<Favorite>();
            try
            {
                LoadFavorites(my_Favoritesfile);
            }
            catch
            {
                //No ini file or wrong data format.
            }
        }

        public void Update()
        {
            TxtCellSize.Text = CellSize.ToString();
            TxtCount.Text = ParticleCount.ToString();
            TxtForce.Text = MaxForce.ToString();
            TxtSpeed.Text = Speed.ToString();
            TxtXYChange.Text = XYChange.ToString();
            TxtZChange.Text = ZChange.ToString();
            TxtTrailLength.Text = TrailLength.ToString();
            CBRndSpawn.IsChecked = RandomSpawn;
            CBUseColor.IsChecked = UseColor;
        }

        private void LoadFavorites(string filename)
        {
            StreamReader sr;
            Favorite fav;
            sr = new StreamReader(filename);
            while (!sr.EndOfStream)
            {
                fav = new Favorite();
                fav.Name = sr.ReadLine();
                fav.CellSize = double.Parse(sr.ReadLine());
                fav.ParticleCount = int.Parse(sr.ReadLine());
                fav.MaxForce = double.Parse(sr.ReadLine());
                fav.Speed = double.Parse(sr.ReadLine());
                fav.XYChange = double.Parse(sr.ReadLine());
                fav.ZChange = double.Parse(sr.ReadLine());
                fav.TrailLength = byte.Parse(sr.ReadLine());
                fav.RandomSpawn = bool.Parse(sr.ReadLine());
                fav.UseColor = bool.Parse(sr.ReadLine());
                FavoriteSettings.Add(fav);
            }
            if (sr != null)
            {
                sr.Close();
            }
            for (int I = 0; I < FavoriteSettings.Count; I++)
            {
                CmbFavorites.Items.Add(FavoriteSettings[I].Name);
            }
            if (CmbFavorites.Items.Count > 0)
            {
                CmbFavorites.SelectedIndex = 0;
                SetFavorite(0);
            }
        }

        private void SetFavorite(int index)
        {
            if (FavoriteSettings.Count > index)
            {
                CellSize = FavoriteSettings[index].CellSize;
                ParticleCount = FavoriteSettings[index].ParticleCount;
                MaxForce = FavoriteSettings[index].MaxForce;
                Speed = FavoriteSettings[index].Speed;
                XYChange = FavoriteSettings[index].XYChange;
                ZChange = FavoriteSettings[index].ZChange;
                TrailLength = FavoriteSettings[index].TrailLength;
                RandomSpawn = FavoriteSettings[index].RandomSpawn;
                UseColor = FavoriteSettings[index].UseColor;
                Update();
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
                CellSize = double.Parse(TxtCellSize.Text);
                ParticleCount = int.Parse(TxtCount.Text);
                MaxForce = double.Parse(TxtForce.Text);
                Speed = double.Parse(TxtSpeed.Text);
                XYChange = double.Parse(TxtXYChange.Text);
                ZChange = double.Parse(TxtZChange.Text);
                TrailLength = byte.Parse(TxtTrailLength.Text);
                RandomSpawn = CBRndSpawn.IsChecked.Value;
                UseColor = CBUseColor.IsChecked.Value;
                ((MainWindow)MyMain).Start();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnCellSizeUP_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtCellSize.Text);
            dummy += 1;
            TxtCellSize.Text = dummy.ToString();
        }

        private void BtnCellSizeDown_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtCellSize.Text);
            dummy -= 1;
            if (dummy < 5) dummy = 5;
            TxtCellSize.Text = dummy.ToString();
        }

        private void BtnCountUP_Click(object sender, RoutedEventArgs e)
        {
            int dummy = int.Parse(TxtCount.Text);
            dummy += 100;
            TxtCount.Text = dummy.ToString();
        }

        private void BtnCountDown_Click(object sender, RoutedEventArgs e)
        {
            int dummy = int.Parse(TxtCount.Text);
            dummy -= 100;
            if (dummy < 100) dummy = 100;
            TxtCount.Text = dummy.ToString();
        }

        private void BtnForceUP_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtForce.Text);
            dummy += 0.01;
            TxtForce.Text = dummy.ToString();
        }

        private void BtnForceDown_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtForce.Text);
            dummy -= 0.01;
            TxtForce.Text = dummy.ToString();
        }

        private void BtnSpeedUP_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtSpeed.Text);
            dummy += 0.05;
            TxtSpeed.Text = dummy.ToString();
        }

        private void BtnSpeedDown_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtSpeed.Text);
            dummy -= 0.05;
            TxtSpeed.Text = dummy.ToString();
        }

        private void BtnXYChangeUP_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtXYChange.Text);
            dummy += 0.01;
            TxtXYChange.Text = dummy.ToString();
        }

        private void BtnXYChangeDown_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtXYChange.Text);
            dummy -= 0.01;
            TxtXYChange.Text = dummy.ToString();
        }

        private void BtnZChangeUP_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtZChange.Text);
            dummy += 0.0002;
            TxtZChange.Text = dummy.ToString();
        }

        private void BtnZChangeDown_Click(object sender, RoutedEventArgs e)
        {
            double dummy = double.Parse(TxtZChange.Text);
            dummy -= 0.0002;
            TxtZChange.Text = dummy.ToString();
        }

        private void BtnTrailLengthUP_Click(object sender, RoutedEventArgs e)
        {
            int dummy = int.Parse(TxtTrailLength.Text);
            dummy += 5;
            if (dummy > 255) dummy = 255;
            TxtTrailLength.Text = dummy.ToString();
        }

        private void BtnTrailLengthDown_Click(object sender, RoutedEventArgs e)
        {
            int dummy = int.Parse(TxtTrailLength.Text);
            dummy -= 5;
            if (dummy < 5) dummy = 5;
            TxtTrailLength.Text = dummy.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = null;
            FavoriteNameForm frm = new FavoriteNameForm();
            if (frm.ShowDialog() == true)
            {
                //Make the new Favorite
                Favorite fav = new Favorite();
                fav.Name = frm.FavoriteName;
                fav.CellSize = double.Parse(TxtCellSize.Text);
                fav.ParticleCount = int.Parse(TxtCount.Text);
                fav.MaxForce = double.Parse(TxtForce.Text);
                fav.Speed = double.Parse(TxtSpeed.Text);
                fav.XYChange = double.Parse(TxtXYChange.Text);
                fav.ZChange = double.Parse(TxtZChange.Text);
                fav.TrailLength = byte.Parse(TxtTrailLength.Text);
                fav.RandomSpawn = CBRndSpawn.IsChecked.Value;
                fav.UseColor = CBUseColor.IsChecked.Value;
                FavoriteSettings.Add(fav);
                //Set the new Favorite name in the combobox
                CmbFavorites.Items.Add(fav.Name);
                CmbFavorites.SelectedIndex = CmbFavorites.Items.Count - 1;
                //Write all the Favorites to the Favorites file
                sw = new StreamWriter(my_Favoritesfile);
                for (int I = 0; I < FavoriteSettings.Count; I++)
                {
                    sw.WriteLine(FavoriteSettings[I].Name);
                    sw.WriteLine(FavoriteSettings[I].CellSize.ToString());
                    sw.WriteLine(FavoriteSettings[I].ParticleCount.ToString());
                    sw.WriteLine(FavoriteSettings[I].MaxForce.ToString());
                    sw.WriteLine(FavoriteSettings[I].Speed.ToString());
                    sw.WriteLine(FavoriteSettings[I].XYChange.ToString());
                    sw.WriteLine(FavoriteSettings[I].ZChange.ToString());
                    sw.WriteLine(FavoriteSettings[I].TrailLength.ToString());
                    sw.WriteLine(FavoriteSettings[I].RandomSpawn.ToString());
                    sw.WriteLine(FavoriteSettings[I].UseColor.ToString());
                }
                if ((sw != null))
                {
                    sw.Close();
                }
            }
        }

        private void CmbFavorites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            SetFavorite(CmbFavorites.SelectedIndex);
        }
    }

    public struct Favorite
    {
        public string Name;
        public double CellSize;
        public int ParticleCount;
        public double MaxForce;
        public double Speed;
        public double XYChange;
        public double ZChange;
        public byte TrailLength;
        public bool RandomSpawn;
        public bool UseColor;
    }
}
