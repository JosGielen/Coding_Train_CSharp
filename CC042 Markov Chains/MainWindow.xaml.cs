using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Markov_Chains
{
    public partial class MainWindow : Window
    {
        private int order = 4;
        private List<string> nGrams;
        private List<string> possibilities;
        private int MaxLength = 300;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\Rainbow.txt");
            string Txt = sr.ReadToEnd();
            string gram;
            int index;
            nGrams = new List<string>();
            possibilities = new List<string>();
            for (int I = 0; I < Txt.Length - order; I++)
            {
                gram = Txt.Substring(I, order);
                if (!nGrams.Contains(gram))
                {
                    nGrams.Add(gram);
                    possibilities.Add(Txt.Substring(I + order, 1));
                }
                else
                {
                    index = nGrams.IndexOf(gram);
                    possibilities[index] += Txt.Substring(I + order, 1);
                }
            }
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            int index;
            int nextIndex;
            string result = nGrams[0];
            string current = nGrams[0];
            for (int I = 0; I < MaxLength; I++)
            {
                index = nGrams.IndexOf(current);
                nextIndex = Rnd.Next(possibilities[index].Length);
                result += possibilities[index].Substring(nextIndex, 1);
                current = result.Substring(result.Length - order, order);
            }
            Text1.Text = result;
        }
    }
}
