using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Xml;
using System.Media;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TextBox> CelList;
        private Sudoku MainSudoku;
        private Sudoku OpgaveSudoku;
        private int SingleDigitCells;
        private int SingleDigitRows;
        private int SingleDigitColumns;
        private int SingleDigitBlocks;
        private List<int> AnalyzeResult;
        private string fileName;
        private bool ProcesTextChange;

        public MainWindow()
        {
            InitializeComponent();
            CelList = new List<TextBox>();
            MainSudoku = new Sudoku();
            OpgaveSudoku = new Sudoku();
            fileName = "";
            ProcesTextChange = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox source = new TextBox();
            //Add all textboxes to CelList and add handlers to them
            for (int I = 0; I <= 80; I++)
            {
                source = (TextBox)FindName("Cel" + I.ToString());
                source.GotFocus += CelFocus;
                source.PreviewKeyDown += CelKeyPressed;
                source.TextChanged += CelTextChanged;
                CelList.Add((TextBox)FindName("Cel" + I.ToString()));
                source.FontSize = 28;
                source.HorizontalContentAlignment = HorizontalAlignment.Center;
                source.VerticalContentAlignment = VerticalAlignment.Top;
            }
            Title = "SudokuSolver";
            CelList[0].Focus();
        }

        #region "Menu"

        private void MnuNew_Click(object sender, RoutedEventArgs e)
        {
            //clear Mainsudoku and OpgaveSudoku
            MainSudoku.Clear();
            OpgaveSudoku.Clear();
            UpdateUI(MainSudoku);
            Title = "SudokuSolver";
            CelList[0].Focus();
            UpdateList(0);
        }

        private void MnuOpen_Click(object sender, RoutedEventArgs e)
        {
            //Ask for a filename and read the file in MainSudoku and in OpgaveSudoku
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.InitialDirectory = Environment.CurrentDirectory;
            OFD.Filter = "Sudoku (*.sud)|*.sud";
            OFD.FilterIndex = 1;
            OFD.RestoreDirectory = true;
            if ((bool)OFD.ShowDialog())
            {
                fileName = OFD.FileName;
                MainSudoku.Load(fileName);
                OpgaveSudoku.Load(fileName);
                UpdateUI(MainSudoku);
                Title = "SudokuSolver: " + Path.GetFileName(fileName);
                CelList[0].Focus();
                UpdateList(0);
            }
        }

        private void MnuSave_Click(object sender, RoutedEventArgs e)
        {
            //check if MainSudoku has a filename
            if (fileName == "" | !File.Exists(fileName))
            {
                MnuSaveAs_Click(sender, e);
            }
            else
            {
                MainSudoku.Save(fileName);
            }
        }

        private void MnuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.InitialDirectory = Environment.CurrentDirectory;
            SFD.Filter = "Sudoku (*.sud)|*.sud";
            SFD.FilterIndex = 1;
            SFD.RestoreDirectory = true;
            if ((bool)SFD.ShowDialog())
            {
                fileName = SFD.FileName;
                MainSudoku.Save(fileName);
                Title = "SudokuSolver: " + Path.GetFileName(fileName);
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            StackPanel PrintPanel = new StackPanel();
            double pageWidth;
            double pageHeight;
            double sudokuWidth = SudokuBorder.ActualWidth;
            double sudokuHeight = SudokuBorder.ActualHeight;
            double leftMargin;
            double topMargin;
            double rightMargin;
            double bottomMargin;
            string My_Xaml;
            StringReader stringReader;
            XmlReader MyReader;
            Border SudokuCopy;
            printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
            if (printDlg.ShowDialog() == true)
            {
                //Get the paper size
                pageWidth = printDlg.PrintableAreaWidth;
                pageHeight = printDlg.PrintableAreaHeight;
                //Center the sudoku on the page
                leftMargin = (pageWidth - sudokuWidth) / 2 - 20;
                rightMargin = (pageWidth - sudokuWidth) / 2 - 20;
                topMargin = 80;
                bottomMargin = 50;
                PrintPanel.Margin = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin);
                //Copy the SudokuBorder
                My_Xaml = XamlWriter.Save(SudokuBorder);
                stringReader = new StringReader(My_Xaml);
                MyReader = XmlReader.Create(stringReader);
                SudokuCopy = (Border)XamlReader.Load(MyReader);
                //Place the sudoku copy in the PrintPanel
                PrintPanel.Children.Add(SudokuCopy);
                //Set the Sudoku print size the same as the actual screen size.
                SudokuBorder.Width = sudokuWidth;
                SudokuBorder.Height = sudokuHeight;
                //Measure and arrange the PrintPanel to fit the page
                PrintPanel.Measure(new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight));
                PrintPanel.Arrange(new Rect(new Point(0, 0), PrintPanel.DesiredSize));
                //Print the PrintPanel
                printDlg.PrintVisual(PrintPanel, "SudokuSolver Print");
            }
        }

        private void MnuClear_Click(object sender, RoutedEventArgs e)
        {
            //clear MainSudoku but not OpgaveSudoku
            MainSudoku.Clear();
            UpdateUI(MainSudoku);
            CelList[0].Focus();
            UpdateList(0);
        }

        private void MnuStore_Click(object sender, RoutedEventArgs e)
        {
            //copy Mainsudoku naar OpgaveSudoku
            OpgaveSudoku = MainSudoku.copy();
        }

        private void MnuRecall_Click(object sender, RoutedEventArgs e)
        {
            //copy OpgaveSudoku to MainSudoku
            MainSudoku = OpgaveSudoku.copy();
            UpdateUI(MainSudoku);
            CelList[0].Focus();
            UpdateList(0);
        }

        private void MnuCheck_Click(object sender, RoutedEventArgs e)
        {
            //Check if the values are OK
            Sudoku backup = MainSudoku.copy();
            int Aantal;
            do
            {
                Aantal = SingleScan(backup);
            } while (Aantal > 0);
            if (Aantal == 0)
            {
                MessageBox.Show("All values are valid.", "SudokuSolver Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("The Sudoku is not correct!", "SudokuSolver Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void MnuAnalyze_Click(object sender, RoutedEventArgs e)
        {
            Sudoku backup = MainSudoku.copy();
            string Result = "";
            for (int I = 0; I <= 80; I++)
            {
                if (backup.GetCel(I).Number > 0) backup.GetCel(I).Fixed = true;
            }
            //Check the sudoku with a single scan
            int Aantal = SingleScan(backup);
            if (Aantal == -1)
            {
                MessageBox.Show("The Sudoku is not correct!", "SudokuSolver Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                for (int I = 0; I < AnalyzeResult.Count; I++)
                {
                    CelList[AnalyzeResult[I]].Background = Brushes.LightGoldenrodYellow;
                }
                Result += SingleDigitCells.ToString() + " Cells can have only 1 value.\n";
                Result += SingleDigitRows.ToString() + " Rows have a value that can only be placed in 1 cell.\n";
                Result += SingleDigitColumns.ToString() + " Columns have a value that can only be placed in 1 cell.\n";
                Result += SingleDigitBlocks.ToString() + " Blocks have a value that can only be placed in 1 cell.\n\n";
                Result += "The Scan result cells have been highlighted.";
                if (MessageBox.Show(Result, "Sudoku Analysis Result", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    for (int I = 0; I < AnalyzeResult.Count; I++)
                    {
                        CelList[AnalyzeResult[I]].Background = Brushes.White;
                    }
                }
            }
        }

        private void MnuSingleScan_Click(object sender, RoutedEventArgs e)
        {
            string Result = "";
            for (int I = 0; I <= 80; I++)
            {
                if (MainSudoku.GetCel(I).Number > 0) MainSudoku.GetCel(I).Fixed = true;
            }
            //Check the sudoku with a single scan
            int Aantal = SingleScan(MainSudoku);
            ProcesTextChange = false;
            for (int I = 0; I <= 80; I++)
            {
                if (MainSudoku.GetCel(I).Number != 0)
                {
                    CelList[I].Text = MainSudoku.GetCel(I).Number.ToString();
                }
                else
                {
                    CelList[I].Text = "";
                }
            }
            ProcesTextChange = true;
            if (Aantal == -1)
            {
                MessageBox.Show("The Sudoku is not correct!", "SudokuSolver Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                Result += Aantal.ToString() + " Cells have been filled.\n";
                MessageBox.Show(Result, "Sudoku SingleScan Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MnuSolve_Click(object sender, RoutedEventArgs e)
        {
            for (int I = 0; I <= 80; I++)
            {
                if (MainSudoku.GetCel(I).Number > 0) MainSudoku.GetCel(I).Fixed = true;
            }
            if (Solve(MainSudoku, -1))
            {
                UpdateUI(MainSudoku);
                CelList[0].Focus();
                UpdateList(0);
            }
            else
            {
                MessageBox.Show("The sudoku can not be solved.", "SudokuSolver Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion

        #region "Solver"

        private int SingleScan(Sudoku s)
        {
            List<int> aantal = new List<int>();
            //int teller = 0;  //Number of cells filled during this scan ;
            bool isfixed;
            AnalyzeResult = new List<int>();
            SingleDigitCells = 0;
            SingleDigitRows = 0;
            SingleDigitColumns = 0;
            SingleDigitBlocks = 0;
            //Step1 : Check for empty cells with only 1 possible value
            s.UpdateValues();
            for (int I = 0; I <= 80; I++)
            {
                if (s.GetCel(I).Number == 0)
                {
                    if (s.GetCel(I).TotalAllowed() == 0)
                    {
                        return -1;
                    }
                    else if (s.GetCel(I).TotalAllowed() == 1)
                    {
                        s.GetCel(I).Number = s.GetCel(I).GetAllowedValues()[0];
                        s.UpdateValues();
                        AnalyzeResult.Add(I);
                        SingleDigitCells += 1;
                    }
                }
            }
            //Step2 : Check if a value can only be placed in 1 empty cel of a row, column or block
            for (int N = 1; N <= 9; N++)
            {
                for (int row = 1; row <= 9; row++)
                {
                    aantal.Clear();
                    isfixed = false;
                    for (int I = 9 * (row - 1); I <= 9 * row - 1; I++)
                    {
                        if (s.GetCel(I).Number == N)
                        {
                            isfixed = true;
                            continue;
                        }
                        if (s.GetCel(I).Number == 0)
                        {
                            if (s.GetCel(I).GetIsAllowed(N)) aantal.Add(I);
                        }
                    }
                    if (!isfixed)
                    {
                        if (aantal.Count == 0)
                        {
                            return -1;
                        }
                        else if (aantal.Count == 1)
                        {
                            s.GetCel(aantal[0]).Number = N;
                            s.UpdateValues();
                            AnalyzeResult.Add(aantal[0]);
                            SingleDigitRows += 1;
                        }
                    }
                }
                for (int col = 1; col <= 9; col++)
                {
                    aantal.Clear();
                    isfixed = false;
                    for (int I = col - 1; I <= 72 + col - 1; I += 9)
                    {
                        if (s.GetCel(I).Number == N)
                        {
                            isfixed = true;
                            continue;
                        }
                        if (s.GetCel(I).Number == 0)
                        {
                            if (s.GetCel(I).GetIsAllowed(N)) aantal.Add(I);
                        }
                    }
                    if (!isfixed)
                    {
                        if (aantal.Count == 0)
                        {
                            return -1;
                        }
                        else if (aantal.Count == 1)
                        {
                            s.GetCel(aantal[0]).Number = N;
                            s.UpdateValues();
                            AnalyzeResult.Add(aantal[0]);
                            SingleDigitColumns += 1;
                        }
                    }
                }
                for (int block = 1; block <= 9; block++)
                {
                    aantal.Clear();
                    isfixed = false;
                    for (int I = 0; I <= 80; I++)
                    {
                        if (s.GetCel(I).Block == block)
                        {
                            if (s.GetCel(I).Number == N)
                            {
                                isfixed = true;
                                continue;
                            }
                            if (s.GetCel(I).Number == 0)
                            {
                                if (s.GetCel(I).GetIsAllowed(N)) aantal.Add(I);
                            }
                        }
                    }
                    if (!isfixed)
                    {
                        if (aantal.Count == 0)
                        {
                            return -1;
                        }
                        else if (aantal.Count == 1)
                        {
                            s.GetCel(aantal[0]).Number = N;
                            s.UpdateValues();
                            AnalyzeResult.Add(aantal[0]);
                            SingleDigitBlocks += 1;
                        }
                    }
                }
            }
            return SingleDigitCells + SingleDigitRows + SingleDigitColumns + SingleDigitBlocks; //teller
        }

        private void TrySingleScanSolve(Sudoku s)
        {
            //Try solving the Sudoku with repeated single scans
            int Aantal;
            do
            {
                Aantal = SingleScan(s);
            } while (Aantal > 0);
        }

        private bool Solve(Sudoku s, int TestCel)
        {
            Sudoku backup = s.copy();
            //Use single scans untill no more changes are possible
            TrySingleScanSolve(backup);
            if (backup.TotalFilled() == 81)
            {
                MainSudoku = backup.copy();
                return true;
            }
            else
            {
                //No solution found with single scans so use Trial and Error
                while (true)
                {
                    //Find the next empty TestCel
                    TestCel += 1;
                    if (TestCel > 80) return false; //No valid Testcel found
                    if (backup.GetCel(TestCel).Number == 0)
                    {
                        //Try all available values in this empty TestCel
                        foreach (int trialValue in backup.GetCel(TestCel).GetAllowedValues())
                        {
                            backup.GetCel(TestCel).Number = trialValue;
                            if (Solve(backup, TestCel)) return true;
                        }
                        return false;  //None of the available values gives a solution
                    }
                };
            }
        }

        #endregion

        #region "User Interface"

        private void CelFocus(object sender, RoutedEventArgs e)
        {
            //Update the list of available values for the selected Cel.
            TextBox source;
            int tag;
            if (e.OriginalSource.GetType() == typeof(TextBox))
            {
                source = (TextBox)e.OriginalSource;
                tag = int.Parse(source.Tag.ToString());
                UpdateList(tag);
            }
        }

        private void UpdateList(int index)
        {
            LstBeschikbaar.Items.Clear();
            foreach (int n in MainSudoku.GetCel(index).GetAllowedValues())
            {
                LstBeschikbaar.Items.Add(n);
            }
        }

        private void CelKeyPressed(object sender, KeyEventArgs e)
        {
            //Process special keys (direction, delete, ...)
            string k = e.Key.ToString();
            int tag;
            TextBox source;
            if (e.OriginalSource.GetType() == typeof(TextBox))
            {
                source = (TextBox)e.OriginalSource;
                tag = int.Parse(source.Tag.ToString());
                switch (k)
                {
                    case "Delete":
                    {
                        if (!MainSudoku.GetCel(tag).Fixed)
                        {
                            if (source.SelectedText == source.Text)
                            {
                                MainSudoku.GetCel(tag).Number = 0;
                                source.Text = "";
                            }
                        }
                        else
                        {
                            e.Handled = true;
                            return;
                        }
                        break;
                    }
                    case "Back":
                    {
                        if (!MainSudoku.GetCel(tag).Fixed)
                        {
                            MainSudoku.GetCel(tag).Number = 0;
                            source.Text = "";
                        }
                        else
                        {
                            e.Handled = true;
                            return;
                        }
                        break;
                    }
                    case "Left":
                    {
                        if (tag % 9 == 0)
                        {
                            CelList[tag + 8].Focus();
                        }
                        else
                        {
                            CelList[tag - 1].Focus();
                        }
                        e.Handled = true;
                        break;
                    }
                    case "Right":
                    {
                        if ((tag + 1) % 9 == 0)
                        {
                            CelList[tag - 8].Focus();
                        }
                        else
                        {
                            CelList[tag + 1].Focus();
                        }
                        e.Handled = true;
                        break;
                    }
                    case "Up":
                    {
                        if (tag < 9)
                        {
                            CelList[72 + tag].Focus();
                        }
                        else
                        {
                            CelList[tag - 9].Focus();
                        }
                        e.Handled = true;
                        break;
                    }
                    case "Down":
                    {
                        if (tag > 71)
                        {
                            CelList[tag - 72].Focus();
                        }
                        else
                        {
                            CelList[tag + 9].Focus();
                        }
                        e.Handled = true;
                        break;
                    }
                }
                if (source.Text != "")
                {
                    e.Handled = true;
                }
            }
        }

        private void CelTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox source;
            int tag;
            int TestValue;
            if (MainSudoku == null) return;
            if (ProcesTextChange == false) return;
            if (e.OriginalSource.GetType() == typeof(TextBox))
            {
                source = (TextBox)e.OriginalSource;
                tag = int.Parse(source.Tag.ToString());
                if (int.TryParse(source.Text, out TestValue))
                {
                    int[] allowedCount = MainSudoku.GetCel(tag).GetAllowedValues();
                    bool hasTestValue = false;
                    //check if the int[] allowedCount contains TestValue
                    for (int I = 0; I < allowedCount.Length; I++)
                    {
                        if (allowedCount[I] == TestValue)
                        {
                            hasTestValue = true;
                            break;
                        }
                    }
                    if (hasTestValue)
                    {
                        MainSudoku.GetCel(tag).Number = TestValue;
                        if (MainSudoku.TotalFilled() == 81)
                        {
                            MessageBox.Show("Congratulations, You solved the Sudoku!", "SudokuSolver info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    else
                    {
                        SystemSounds.Beep.Play();
                        MainSudoku.GetCel(tag).Number = 0;
                        source.Text = "";
                    }
                }
                else
                {
                    MainSudoku.GetCel(tag).Number = 0;
                    source.Text = "";
                }
                MainSudoku.UpdateValues();
            }
        }

        private void UpdateUI(Sudoku s)
        {
            ProcesTextChange = false;
            for (int I = 0; I <= 80; I++)
            {
                if (s.GetCel(I).Number != 0)
                {
                    CelList[I].Text = s.GetCel(I).Number.ToString();
                }
                else
                {
                    CelList[I].Text = "";
                }
            }
            for (int I = 0; I <= 80; I++)
            {
                if (s.GetCel(I).Given)
                {
                    CelList[I].Background = Brushes.LightBlue;
                }
                else
                {
                    CelList[I].Background = Brushes.White;
                }
            }
            ProcesTextChange = true;
        }

        #endregion
    }
}
