using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.ComponentModel;

namespace Earthquake_Data
{
    public partial class MainWindow : Window
    {
        private double Zoom = 1.0;
        private Ellipse El;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            //Draw a map of the Earth
            BitmapImage bitmap = new BitmapImage(new System.Uri(Environment.CurrentDirectory + "\\Earth.jpg"));
            canvas1.Background = new ImageBrush(bitmap);
            //Get the earthquake data
            string address = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_week.csv";
            WebClient client = new WebClient();
            StreamReader reader = new StreamReader(client.OpenRead(address));
            List<EQData> EarthQuakes = new List<EQData>();
            while (!reader.EndOfStream)
            {
                EarthQuakes.Add(new EQData(reader.ReadLine()));
            }
            for (int I = 1; I < EarthQuakes.Count; I++)
            {
                MarkEQ(EarthQuakes[I].Longitude, EarthQuakes[I].Latitude, EarthQuakes[I].Magnitude);
            }
            UpdateLayout();
        }

        private void MarkEQ(double Longitude, double Latitude, double Magnitude)
        {
            if (Magnitude < 0) Magnitude = 0;
            double size = 2 * (Magnitude + 1);
            double G = 255 - 25.5 * Magnitude;
            El = new Ellipse()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1.0,
                Width = size,
                Height = size,
                Fill = new SolidColorBrush(Color.FromRgb(255, (byte)G, 0))
            };
            El.SetValue(Canvas.LeftProperty, MercX(Longitude) - size / 2);
            El.SetValue(Canvas.TopProperty, MercY(Latitude) - size / 2);
            canvas1.Children.Add(El);
        }

        private double MercX(double longit)
        {
            return canvas1.ActualWidth / 720 * Math.Pow(2, Zoom) * (longit + 180) - 3;
        }

        private double MercY(double latit)
        {
            return canvas1.ActualHeight / (4 * Math.PI) * Math.Pow(2, Zoom) * (Math.PI - Math.Log(Math.Tan(Math.PI / 4 + latit * Math.PI / 360)));
        }

        private void Window_Closing(Object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }

    public class EQData
    {
        public double Longitude;
        public double Latitude;
        public double Magnitude;

        public EQData(string source)
        {
            string[] data = source.Split(',');
            if (data.Length > 4)
            {
                try
                {
                    Latitude = double.Parse(data[1]);
                    Longitude = double.Parse(data[2]);
                    Magnitude = double.Parse(data[4]);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
