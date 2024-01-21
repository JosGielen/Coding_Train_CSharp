using Spire.Doc;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Threading;

namespace TF_IDF
{
    public partial class MainWindow : Window
    {
        private delegate void StatusDelegate(double v);
        private List<DocumentWords> DocWords;
        private Document doc = new Document();
        private List<string> TotalList;
        private List<int> DocumentCounts;
        private string Status = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ReadDirectory()
        {
            string[] words;
            DocumentWords dw;
            int TotalFiles = 0;
            int DocFiles = 0;
            int DocxFiles = 0;
            int ErrorFiles = 0;
            int OtherFiles = 0;
            double Progress = 0.0;
            ClosedXML.Excel.XLWorkbook my_Workbook;
            ClosedXML.Excel.IXLWorksheet my_Worksheet;
            FolderBrowserDialog fldDialog = new FolderBrowserDialog();
            fldDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fldDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DocWords = new List<DocumentWords>();
                TotalList = new List<string>();
                DocumentCounts = new List<int>();
                Status = "Scanning directory: " + fldDialog.SelectedPath;
                Progress = 0;
                Dispatcher.Invoke(DispatcherPriority.SystemIdle, new StatusDelegate(UpdateStatus), 100 * Progress); //progress = 0 - 1
                foreach (string f in GetFiles(fldDialog.SelectedPath))
                {
                    TotalFiles += 1;
                    if (Path.GetExtension(f) == ".doc" | Path.GetExtension(f) == ".docx")
                    {
                        dw = new DocumentWords(f);
                        DocWords.Add(dw);
                    }
                }
                //Get the word counts for each document
                for (int I = 0; I < DocWords.Count(); I++)
                {
                    Status = "Reading :" + DocWords[I].FileName;
                    Progress = I / (3 * DocWords.Count());
                    Dispatcher.Invoke(DispatcherPriority.SystemIdle, new StatusDelegate(UpdateStatus), 100 * Progress); //progress = 0 - 1
                    try
                    {
                        if (Path.GetExtension(DocWords[I].FileName) == ".doc")
                        {
                            doc.LoadFromFile(DocWords[I].FileName, FileFormat.Doc);
                            DocFiles += 1;
                        }
                        else if (Path.GetExtension(DocWords[I].FileName) == ".docx")
                        {
                            doc.LoadFromFile(DocWords[I].FileName, FileFormat.Docx);
                            DocxFiles += 1;
                        }
                        else
                        {
                            OtherFiles += 1;
                            continue;
                        }
                        Char[] chars = [' ', '\n', '\r'];
                        words = doc.GetText().ToLower().Split(chars);
                        chars = ['.', '(', ')', ':', ',', ';', '!', ' '];
                        for (int J = 0; J < words.Count(); J++)
                        {
                            words[J] = words[J].Trim(chars);
                            if (words[J] != "")
                            {
                                if (DocWords[I].HasToken(words[J]))
                                {
                                    DocWords[I].IncreaseCount(words[J]);
                                }
                                else
                                {
                                    DocWords[I].Addtoken(words[J]);
                                }
                                if (!TotalList.Contains(words[J]))
                                {
                                    TotalList.Add(words[J]);
                                    DocumentCounts.Add(0);
                                }
                            }
                        }
                    }
                    catch
                    {
                        ErrorFiles += 1;
                        //Do nothing
                    }
                }
                Status = "Processing Data...";
                Progress = 0.3333;
                Dispatcher.Invoke(DispatcherPriority.SystemIdle, new StatusDelegate(UpdateStatus), 100 * Progress); //progress = 0 - 1
                //Get the document count for each word in TotalList
                for (int I = 0; I < TotalList.Count(); I++)
                {
                    for (int J = 0; J < DocWords.Count(); J++)
                    {
                        if (DocWords[J].HasToken(TotalList[I]))
                        {
                            DocumentCounts[I] += 1;
                        }
                    }
                }
                //For each document: Set the DocCount of each word,
                //then sort the words in that document by TF_IDF value
                int index = 0;
                for (int I = 0; I < DocWords.Count(); I++)
                {
                    Progress = 0.333 + I / (3 * DocWords.Count);
                    Dispatcher.Invoke(DispatcherPriority.SystemIdle, new StatusDelegate(UpdateStatus), 100 * Progress); //progress = 0 - 1
                    for (int J = 0; J < DocWords[I].Tokens.Count(); J++)
                    {
                        index = TotalList.IndexOf(DocWords[I].Tokens[J]);
                        DocWords[I].SetDocCount(J, DocumentCounts[index]);
                    }
                    DocWords[I].CalculateTF_IDF(DocWords.Count());
                    DocWords[I].SortByTF_IDF();
                }

                //Repeat TF-IDF for the 15 best tokens of each document
                TotalList.Clear();
                DocumentCounts.Clear();
                for (int I = 0; I < DocWords.Count(); I++)
                {
                    DocWords[I].RemainBest(15);
                    DocWords[I].CalculateTF_IDF(DocWords.Count());
                    DocWords[I].SortByTF_IDF();
                }

                //For each document: Write the Filename and the 8 words
                //with the highest TF-IDF to a row in an Excel file
                SaveFileDialog SFD = new SaveFileDialog();
                SFD.InitialDirectory = Environment.CurrentDirectory;
                SFD.Filter = "Excel File (*.xlsx)|*.xlsx";
                SFD.FilterIndex = 1;
                SFD.RestoreDirectory = true;
                if (SFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        Status = "Writing results to " + Path.GetFileName(SFD.FileName);
                        Progress = 0.666;
                        Dispatcher.Invoke(DispatcherPriority.SystemIdle, new StatusDelegate(UpdateStatus), 100 * Progress); //progress = 0 - 1
                        my_Workbook = new ClosedXML.Excel.XLWorkbook();
                        my_Worksheet = my_Workbook.AddWorksheet("Sheet1");
                        my_Worksheet.Cell(2, 2).Value = "Analysis Documents Keywords";
                        my_Worksheet.Cell(2, 2).Style.Font.FontSize = 18.0;
                        for (int I = 0; I < DocWords.Count(); I++)
                        {
                            Progress = 0.666 + I / (3 * DocWords.Count());
                            Dispatcher.Invoke(DispatcherPriority.SystemIdle, new StatusDelegate(UpdateStatus), 100 * Progress); //progress = 0 - 1
                            my_Worksheet.Cell(I + 5, 1).Value = Path.GetFileName(DocWords[I].FileName);
                            for (int J = 5; J <= 15; J++)
                            {
                                if (DocWords[I].Tokens.Count() > J)
                                {
                                    my_Worksheet.Cell(I + 5, J + 2).Value = DocWords[I].Tokens[J];
                                }
                            }
                        }
                        my_Workbook.SaveAs(SFD.FileName);
                        my_Workbook.Dispose();
                    }
                    catch (Exception Ex)
                    {
                        System.Windows.MessageBox.Show("Cannot save the TF_IDF data. Original error: " + Ex.Message, "TF_IDF error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                UpdateStatus(100);
                txtOut.Text += "\n\nTotal Files found = " + TotalFiles.ToString();
                txtOut.Text += "\nDoc Files processed = " + DocFiles.ToString();
                txtOut.Text += "\nDocx Files processed = " + DocxFiles.ToString();
                txtOut.Text += "\nInvalid Format Files = " + ErrorFiles.ToString();
                txtOut.Text += "\nother Files skipped = " + OtherFiles.ToString() + "\n";
            }
        }

        private List<string> GetFiles(string dir)
        {
            List<string> result = new List<string>();
            foreach (string f in Directory.GetFiles(dir))
            {
                if (Path.GetFileNameWithoutExtension(f).Length > 0)
                {
                    if (Path.GetFileNameWithoutExtension(f)[0] != '~') result.Add(f);
                }
            }
            foreach (string d in Directory.GetDirectories(dir))
            {
                result.AddRange(GetFiles(d));
            }
            return result;
        }

        private void UpdateStatus(double v)
        {
            PBStatus.Value = v;
            TxtStatus.Text = string.Format("{0:0.00}", v) + "%";
            txtFileName.Text = Status;
        }

        private void MnuExit_Click(Object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuStart_Click(Object sender, RoutedEventArgs e)
        {
            ReadDirectory();
        }
    }
}