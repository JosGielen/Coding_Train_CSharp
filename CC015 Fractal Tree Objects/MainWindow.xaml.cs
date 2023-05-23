using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fractal_TreeObjects
{
    public partial class MainWindow : Window
    {
        private List<Branch> Tree;
        private List<Ellipse> leafs;
        private double Len = 160;
        private double Angle;
        private bool App_Loaded = false;
        private string ResultFileName = "output\\FractalTree.gif";
        private int frameNumber = 0;
        private bool Recording = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App_Loaded = true;
            if (CbRecord.IsChecked.Value)
            {
                Recording = true;
            }
            else
            {
                Recording = false;
            }
            Init();
        }

        private void Init()
        {
            canvas1.Children.Clear();
            Angle = SldAngle.Value;
            Tree = new List<Branch>();
            leafs = new List<Ellipse>();
            Point StartPt = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight);
            Point EndPt = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight - Len);
            Branch Root = new Branch(StartPt, EndPt);
            Root.Show(canvas1);
            Tree.Add(Root);
            frameNumber = 0;
            canvas1.UpdateLayout();
            //Save the window as a jpeg image
            if (Recording) SaveImage(canvas1);
        }

        private void SldAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!App_Loaded) return;
            Angle = SldAngle.Value;
            Init();
        }

        private void BtnGrow_Click(object sender, RoutedEventArgs e)
        {
            List<Branch> newbranches = new List<Branch>();
            double leafSize;
            Ellipse leaf;
            //Branch the existing branches
            for (int I = 0; I < Tree.Count; I++)
            {
                newbranches.AddRange(Tree[I].BranchOut(Angle));
            }
            if (newbranches.Count > 0)
            {
                //Remove the previous leaves
                for (int I = 0; I < leafs.Count; I++)
                {
                    canvas1.Children.Remove(leafs[I]);
                }
                leafs = new List<Ellipse>();
                //Draw the new branches
                for (int I = 0; I < newbranches.Count; I++)
                {
                    newbranches[I].Show(canvas1);
                    Tree.Add(newbranches[I]);
                    //Draw leaves at the end of the new branches
                    leafSize = newbranches[I].Length;
                    if (leafSize < 6) leafSize = 6;
                    leaf = new Ellipse()
                    {
                        Stroke = Brushes.Green,
                        StrokeThickness = 1.0,
                        Fill = Brushes.Green,
                        Width = leafSize,
                        Height = leafSize
                    };
                    leaf.SetValue(Canvas.LeftProperty, newbranches[I].EndPt.X - leafSize / 2);
                    leaf.SetValue(Canvas.TopProperty, newbranches[I].EndPt.Y - leafSize / 2);
                    canvas1.Children.Add(leaf);
                    leafs.Add(leaf);
                }
                canvas1.UpdateLayout();
                //Save the window as a png image
                if (Recording) SaveImage(canvas1);
            }
            else
            {
                //The Tree is fully grown
                if (Recording)
                {
                    //Convert the png images into an animated Gif.
                    MakeGif(1);
                    Recording = false;
                }
            }
        }

        private void SaveImage(FrameworkElement Element )
        {
            DirectoryInfo dirInfo = Directory.CreateDirectory(Environment.CurrentDirectory + "\\output");
            string dir = dirInfo.FullName;
            string fileName = dir + "\\Image-" + frameNumber.ToString("0000") + ".png";
            BitmapEncoder MyEncoder = new PngBitmapEncoder();
            RenderTargetBitmap renderbmp = new RenderTargetBitmap((int)Element.ActualWidth, (int)Element.ActualHeight, 96, 96, PixelFormats.Default);
            renderbmp.Render(Element);
            try
            {
                MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp));
                // Create a FileStream to write the image to the file.
                FileStream sw = new FileStream(fileName, FileMode.Create);
                MyEncoder.Save(sw);
                frameNumber += 1;
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The Image could not be saved.", "NoiseGifLoop error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MakeGif(int frameRate)
        {
            //Create an animated Gif with ffmpeg.exe
            string prog = Environment.CurrentDirectory + "\\ffmpeg.exe";
            string args = " -framerate " + frameRate.ToString() + " -i output\\Image-%4d.png " + ResultFileName;
            Process p = Process.Start(prog, args);
            p.WaitForExit();
            //Delete the image files
            foreach (string f in Directory.GetFiles(Environment.CurrentDirectory + "\\output"))
            {
                if (System.IO.Path.GetExtension(f) == ".png")
                {
                    File.Delete(f);
                }
            }
        }

        private void CbRecord_Click(object sender, RoutedEventArgs e)
        {
            if (CbRecord.IsChecked.Value)
            {
                Recording = true;
            }
            else
            {
                Recording = false;
            }
        }
    }
}
