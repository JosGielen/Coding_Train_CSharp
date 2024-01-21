namespace TF_IDF
{

    public class DocumentWords
    {
        private string my_FileName;
        private List<string> my_Tokens;
        private List<int> my_Counts;
        private List<int> my_DocCounts;
        private List<double> my_TF_IDF;

        public DocumentWords(string file)
        {
            my_FileName = file;
            my_Tokens = new List<string>();
            my_Counts = new List<int>();
            my_DocCounts = new List<int>();
            my_TF_IDF = new List<double>();
        }

        public string FileName
        {
            get { return my_FileName; }
        }

        public List<string> Tokens
        {
            get { return my_Tokens; }
        }

        public void Addtoken(string t)
        {
            my_Tokens.Add(t);
            my_Counts.Add(1);
            my_DocCounts.Add(0);
            my_TF_IDF.Add(0);
        }

        public void IncreaseCount(string t)
        {
            my_Counts[my_Tokens.IndexOf(t)] += 1;
        }

        public void SetDocCount(int I, int number)
        {
            my_DocCounts[I] = number;
        }

        public bool HasToken(string t)
        {
            return my_Tokens.Contains(t);
        }

        public void SortByCounts()
        {
            for (int I = 0; I < my_Tokens.Count(); I++)
            {
                for (int J = I + 1; J < my_Tokens.Count(); J++)
                {
                    if (my_Counts[I] < my_Counts[J]) Swap(I, J);
                }
            }
        }

        public void CalculateTF_IDF(int totalDocs)
        {
            int totalWords = 0;
            for (int I = 0; I < my_Tokens.Count(); I++)
            {
                totalWords += my_Counts[I];
            }
            for (int I = 0; I < my_Tokens.Count(); I++)
            {
                my_TF_IDF[I] = (my_Counts[I] / totalWords) * Math.Log10(totalDocs / my_DocCounts[I]);
            }
        }

        public void SortByTF_IDF()
        {
            for (int I = 0; I < my_Tokens.Count(); I++)
            {
                for (int J = I + 1; J < my_Tokens.Count(); J++)

                {
                    if (my_TF_IDF[I] < my_TF_IDF[J]) Swap(I, J);
                }
            }
        }

        public void RemainBest(int num)
        {
            if (my_Tokens.Count > num)
            {
                my_Tokens.RemoveRange(num, my_Tokens.Count - num);
                my_DocCounts.RemoveRange(num, my_DocCounts.Count - num);
            }
            my_Counts.Clear();
            my_TF_IDF.Clear();
            for (int I = 0; I < num; I++)
            {
                my_Counts.Add(1);
                my_TF_IDF.Add(0);
            }
        }

        private void Swap(int I, int J)
        {
            string dummyToken = my_Tokens[I];
            int dummyCount = my_Counts[I];
            int dummyDocCount = my_DocCounts[I];
            double dummyTF_IDF = my_TF_IDF[I];
            my_Tokens[I] = my_Tokens[J];
            my_Counts[I] = my_Counts[J];
            my_DocCounts[I] = my_DocCounts[J];
            my_TF_IDF[I] = my_TF_IDF[J];
            my_Tokens[J] = dummyToken;
            my_Counts[J] = dummyCount;
            my_DocCounts[J] = dummyDocCount;
            my_TF_IDF[J] = dummyTF_IDF;
        }
    }
}
