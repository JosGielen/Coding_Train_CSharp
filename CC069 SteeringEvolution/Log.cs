using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SteeringEvolution
{
    class Log
    {
        private string my_FileName;
        private List<String> my_LogItems;
        private long my_Size;
        private long my_SizeLimit;
        private bool my_LogStarted;
        private bool my_UseTimeStamp;

        public Log()
        {
            my_FileName = Environment.CurrentDirectory + "\\Log-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + ".log";
            my_LogItems = new List<string>();
            my_Size = 0;
            my_SizeLimit = 1000;
            my_LogStarted = true;
            my_UseTimeStamp = false;
        }

        public Log(bool started, bool useTimestamp)
        {
            my_FileName = Environment.CurrentDirectory + "\\Log-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + ".log";
            my_LogItems = new List<string>();
            my_Size = 0;
            my_SizeLimit = 1000;
            my_LogStarted = started;
            my_UseTimeStamp = useTimestamp;
        }

        public Log(string file, int memmoryLimit, bool started, bool useTimestamp)
        {
            my_FileName = file;
            my_LogItems = new List<string>();
            my_Size = 0;
            my_SizeLimit = memmoryLimit;
            my_LogStarted = started;
            my_UseTimeStamp = useTimestamp;
        }

        public bool UseTimeStamp
        {
            get { return my_UseTimeStamp; }
            set { my_UseTimeStamp = value; }
        }

        public int LogItemCount
        {
            get { return my_LogItems.Count; }
        }

        public void AddItem(string item)
        {
            if (my_LogStarted)
            {
                my_LogItems.Add(item);
                my_Size += 2 * item.Length;
                if (my_Size >= my_SizeLimit)
                {
                    if (SaveToFile())
                    {
                        my_LogItems.Clear();
                        my_Size = 0;
                    }
                }
            }
            else
            {
                Debug.Print(item);
            }
        }

        public void Flush()
        {
            if (my_LogItems.Count > 0) SaveToFile();
        }

        private bool SaveToFile()
        {
            //Append the log data to the file (creates the file if it does not exist)
            StreamWriter myStream = null;
            try
            {
                myStream = new StreamWriter(my_FileName, true);
                if ((myStream != null))
                {
                    for (int I = 0; I < my_LogItems.Count; I++)
                    {
                        myStream.WriteLine(my_LogItems[I]);
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Cannot save the Log data. Original error: " + Ex.Message, "Log Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                if ((myStream != null))
                {
                    myStream.Close();
                }
            }
            return true;
        }

        public void StartLog()
        {
            my_LogStarted = true;
        }

        public void StopLog()
        {
            my_LogStarted = false;
            if (my_LogItems.Count > 0) SaveToFile();
        }

        public void ClearLog(bool deleteFile)
        {
            if (deleteFile)
            {
                if (File.Exists(my_FileName)) File.Delete(my_FileName);
            }
            my_LogItems.Clear();
            my_Size = 0;
        }

        public void Close()
        {
            if (my_Size > 0) SaveToFile();
         }
    }
}
