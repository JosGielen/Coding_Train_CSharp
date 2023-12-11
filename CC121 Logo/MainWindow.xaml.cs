using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Logo
{
    public partial class MainWindow : Window
    {
        public delegate void WaitDelegate(int t);
        private readonly int WaitTime = 1;
        private LogoParser parser;
        private Turtle turtle;
        private string FileName;
        private string FilePath;
        private Legend leg;
        private bool App_Started = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parser = new LogoParser();
            turtle = new Turtle(new Point(Canvas1.ActualWidth / 2, Canvas1.ActualHeight / 2), Canvas1);
            App_Started = false;
            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\Default.txt");
            TxtInput.Text = sr.ReadToEnd();
            sr.Close();
        }

        private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Execute();
        }

        public void UncheckMnuLegend()
        {
            MnuLegend.IsChecked = false;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!App_Started)
            {
                App_Started = true;
                BtnStart.Content = "STOP";
                Execute();
            }
            else
            {
                App_Started = false;
                BtnStart.Content = "START";
            }
        }

        private void Execute()
        {
            Canvas1.Children.Clear();
            turtle.Reset();
            parser.Input = TxtInput.Text;
            parser.Parse();
            App_Started = true;
            foreach (LogoCommand cmd in parser.Commands)
            {
                turtle.ExecuteCmd(cmd);
                Dispatcher.Invoke(new WaitDelegate(Wait), DispatcherPriority.ApplicationIdle, WaitTime);
                if (!App_Started) break;
            }
            App_Started = false;
            BtnStart.Content = "START";
        }

        private void Wait(int t)
        {
            Thread.Sleep(t);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            StreamReader sr;
            if (FilePath == "")
            {
                openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            }
            else
            {
                openFileDialog1.InitialDirectory = FilePath;
            }
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == true)
            {
                FilePath = Path.GetDirectoryName(openFileDialog1.FileName);
                FileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                sr = new StreamReader(openFileDialog1.FileName);
                TxtInput.Text = sr.ReadLine();
                Canvas1.Children.Clear();
            }
        }

        private void MnuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveLogoText(FileName);
        }

        private void MnuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (SFD.ShowDialog() == true)
            {
                FileName = SFD.FileName;
                SaveLogoText(FileName);
            }
        }

        private void SaveLogoText(string filename)
        {
            StreamWriter myStream = null;
            try
            {
                myStream = new StreamWriter(filename);
                if (myStream != null)
                {
                    myStream.Write(TxtInput.Text);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Cannot save the Logo data. Original error: " + Ex.Message, "Logo error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (myStream != null)
                {
                    myStream.Close();
                }
            }
        }

        private void MnuClear_Click(object sender, RoutedEventArgs e)
        {
            App_Started = false;
            TxtInput.Text = "";
        }

        private void MnuCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TxtInput.Text);
        }

        private void MnuPaste_Click(object sender, RoutedEventArgs e)
        {
            TxtInput.Text = Clipboard.GetText();
        }

        private void MnuLegend_Click(object sender, RoutedEventArgs e)
        {
            if (MnuLegend.IsChecked)
            {
                if (leg == null)
                {
                    leg = new Legend(this);
                    leg.Show();
                }
                else
                {
                    if (leg != null)
                    {
                        leg.Hide();
                    }
                }
            }
        }
    }
}
