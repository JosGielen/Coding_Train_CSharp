using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Phyllotaxis
{
    public partial class MainWindow : Window
    {
        private bool Rendering;
        private double a;
        private int n;
        private int c;
        private Ellipse el;
        private List<Color> my_Colors;
        private int colorSize = 250;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPalette(Environment.CurrentDirectory + "\\Rainbow.cpl", colorSize);
            StartRender();
        }

        private void StartRender()
        {
            //Some initial code
            a = 137.5; //Angle of the pattern
            n = 0;
            c = 5;
            canvas1.Children.Clear();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            Rendering = true;
        }

        private void StopRender()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            Rendering = false;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double angle = (n * a) * Math.PI / 180;
            double radius = c * Math.Sqrt(n);
            double X = radius * Math.Cos(angle);
            double Y = radius * Math.Sin(angle);
            int dist = (int)Math.Floor(Math.Sqrt(X * X + Y * Y));
            int index = (colorSize - dist) % colorSize;
            if (dist > colorSize)
            {
                StopRender();
                return;
            }
            el = new Ellipse()
            {
                Stroke = new SolidColorBrush(my_Colors[index]),
                StrokeThickness = 1,
                Fill = new SolidColorBrush(my_Colors[index]),
                Width = 8,
                Height = 8
            };
            el.SetValue(Canvas.TopProperty, Y + canvas1.ActualHeight / 2);
            el.SetValue(Canvas.LeftProperty, X + canvas1.ActualWidth / 2);
            canvas1.Children.Add(el);
            n += 1;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Rendering)
            {
                StopRender();
            }
            else
            {
                StartRender();
           }
        }

        private void LoadPalette(string file, int size)
        {
            string Line;
            string[] txtparts;
            byte r;
            byte g;
            byte b;
            List<Color> TempColors = new List<Color>();
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(file);
                sr.ReadLine(); //Skip the number of color data in the palette file.
                while (!sr.EndOfStream)
                {
                    Line = sr.ReadLine();
                    txtparts = Line.Split(';');
                    r = byte.Parse(txtparts[0]);
                    g = byte.Parse(txtparts[1]);
                    b = byte.Parse(txtparts[2]);
                    TempColors.Add(Color.FromRgb(r, g, b));
                }
                my_Colors = new List<Color>();
                if (TempColors.Count != size)
                {
                    int diff = TempColors.Count - size;
                    double fraction = TempColors.Count / (double)diff;
                    for (int I = 0; I < TempColors.Count; I++)
                    {
                        if (I % fraction < 1)
                        {
                            if (TempColors.Count > size)
                            {
                                //Skip some colors
                                continue;
                            }
                            else
                            {
                                //Duplicate some colors
                                my_Colors.Add(TempColors[I]);
                            }
                        }
                        my_Colors.Add(TempColors[I]);
                    }
                }
                else
                {
                    my_Colors = TempColors;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot load the palette. Original error: " + ex.Message, "Lorenz Attractor error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }
    }
}