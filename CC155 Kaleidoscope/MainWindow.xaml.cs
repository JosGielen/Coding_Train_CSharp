using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kaleidoscope
{
    public partial class MainWindow : Window
    {
        private int symmetry = 6;
        private Point center;
        private bool MyMouseDown = false;
        private Point pMouse;
        private List<Brush> myColors = new List<Brush>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            center = new Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2);
            ColorPalette pal = new ColorPalette(Environment.CurrentDirectory + "\\Rainbow.cpl");
            myColors = pal.GetColorBrushes((int)(canvas1.ActualWidth / 2) + 5);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyMouseDown = true;
            pMouse = e.GetPosition(canvas1);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (MyMouseDown)
            {
                Point MousePos = e.GetPosition(canvas1);
                double thickness = 0.0;
                double dist = Math.Sqrt(Math.Pow((MousePos.X - center.X), 2) + Math.Pow((MousePos.Y - center.Y), 2));
                if (dist > canvas1.ActualWidth / 2) dist = canvas1.ActualWidth / 2;
                thickness = 8 - 7 * dist / (canvas1.ActualWidth / 2);
                ScaleTransform st = new ScaleTransform(1.0, -1.0, center.X, center.Y);
                RotateTransform rt = new RotateTransform(360 / symmetry, center.X, center.Y);
                Point StartPt1 = pMouse;
                Point EndPt1 = MousePos;
                Point StartPt2 = pMouse;
                Point EndPt2 = MousePos;
                Line l;
                for (int I = 0; I < symmetry; I++)
                {
                    //Draw the line
                    l = new Line()
                    {
                        X1 = StartPt1.X,
                        Y1 = StartPt1.Y,
                        X2 = EndPt1.X,
                        Y2 = EndPt1.Y,
                        Stroke = myColors[(int)dist],
                        StrokeThickness = thickness
                    };
                    l.StrokeEndLineCap = PenLineCap.Round;
                    canvas1.Children.Add(l);
                    //Flip the line vertically
                    StartPt2 = st.Transform(StartPt1);
                    EndPt2 = st.Transform(EndPt1);
                    //Draw the flipped line
                    l = new Line()
                    {
                        X1 = StartPt2.X,
                        Y1 = StartPt2.Y,
                        X2 = EndPt2.X,
                        Y2 = EndPt2.Y,
                        Stroke = myColors[(int)dist],
                        StrokeThickness = thickness
                    };
                    l.StrokeEndLineCap = PenLineCap.Round;
                    canvas1.Children.Add(l);
                    //Rotate the points
                    StartPt1 = rt.Transform(StartPt1);
                    EndPt1 = rt.Transform(EndPt1);
                    StartPt2 = rt.Transform(StartPt2);
                    EndPt2 = rt.Transform(EndPt2);
                }
                pMouse = MousePos;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMouseDown = false;
        }

        #region "Menu"

        private void MnuSaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD;
            BitmapEncoder MyEncoder = new BmpBitmapEncoder(); //Default save as bitmap
            RenderTargetBitmap renderbmp;
            try
            {
                SFD = new SaveFileDialog()
                {
                    InitialDirectory = Environment.CurrentDirectory,
                    Filter = "Windows Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|TIFF (*.tiff)|*.tiff|PNG (*.png)|*.png",
                    FilterIndex = 1,
                    RestoreDirectory = true
                };
                if (SFD.ShowDialog() == true)
                {
                    switch (SFD.FilterIndex)
                    {
                        case 1:
                            MyEncoder = new BmpBitmapEncoder();
                            break;
                        case 2:
                            MyEncoder = new JpegBitmapEncoder();
                            break;
                        case 3:
                            MyEncoder = new GifBitmapEncoder();
                            break;
                        case 4:
                            MyEncoder = new TiffBitmapEncoder();
                            break;
                        case 5:
                            MyEncoder = new PngBitmapEncoder();
                            break;
                        default:
                        {
                            //Should not occur
                            return;
                        }
                    }
                    renderbmp = new RenderTargetBitmap((int)(canvas1.ActualWidth), (int)(canvas1.ActualHeight), 96, 96, PixelFormats.Default);
                    renderbmp.Render(canvas1);
                    MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp));
                    // Create a FileStream to write the image to the file.
                    FileStream sw = new FileStream(SFD.FileName, FileMode.Create);
                    MyEncoder.Save(sw);
                    sw.Close();
                }
            }
            catch
            {
                MessageBox.Show("The Image could not be saved.", "NoiseGifLoop error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MnuExit_Click(Object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuClear_Click(Object sender, RoutedEventArgs e)
        {
            canvas1.Children.Clear();
        }

        #endregion
    }
}
