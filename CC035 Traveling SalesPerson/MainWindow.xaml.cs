using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TSP_Compare
{
    public partial class MainWindow : Window
    {
        //General data
        private Random Rnd = new Random();
        private int locCount;
        private Point[] locations;
        private DateTime StartTime;
        private bool App_Loaded = false;
        private bool Rendering = false;
        private bool initialised = false;
        //Genetic data
        private int popCount;
        private List<int[]> population;
        private int MutationRate = 20;
        private int bestGeneration;
        private int GenerationCounter;
        private List<double> fitness;
        private int[] indices1;
        private int[] bestOrder;
        private Polyline path1;
        private Polyline bestPath1;
        private double bestDistance1;
        //Lexicographic data
        private int Counter2;
        private int[] indices2;
        private Polyline path2;
        private Polyline bestPath2;
        private double bestDistance2;
        private long TotalPermutations;
        private string findTime = "";
        private bool finished;
        //Random solver data
        private int Counter3;
        private Polyline path3;
        private Polyline bestPath3;
        private double bestDistance3;
        private int[] indices3;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Create the locations
            locCount = 10;
            popCount = 2000;
            TxtLocationCount.Text = locCount.ToString();
            Init();
            App_Loaded = true;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Rendering)
            {
                StartRender();
            }
            else
            {
                StopRender();
            }
        }

        private void StartRender()
        {
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            BtnStart.Content = "STOP";
            StartTime = DateTime.Now;
            Rendering = true;
        }

        private void StopRender()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            BtnStart.Content = "START";
            Rendering = false;
        }

        private void TxtLocationCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!App_Loaded) return;
            locCount = int.Parse(TxtLocationCount.Text);
            initialised = false;
        }

        private void Init()
        {
            locCount = int.Parse(TxtLocationCount.Text);
            locations = new Point[locCount];
            indices1 = new int[locCount];
            indices2 = new int[locCount];
            indices3 = new int[locCount];
            for (int I = 0; I < locCount; I++)
            {
                indices1[I] = I;
            }
            for (int I = 0; I < locCount; I++)
            {
                locations[I] = new Point(5 + (canvas1.ActualWidth - 10) * Rnd.NextDouble(), 5 + (canvas1.ActualHeight - 10) * Rnd.NextDouble());
            }
            canvas1.Children.Clear();
            canvas2.Children.Clear();
            canvas3.Children.Clear();
            //1 : GENETIC START
            //Initialize Genetic data
            Ellipse E1;
            for (int I = 0; I < locCount; I++)
            {
                E1 = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.0
                };
                E1.SetValue(Canvas.TopProperty, locations[I].Y - 3);
                E1.SetValue(Canvas.LeftProperty, locations[I].X - 3);
                canvas1.Children.Add(E1);
            }
            population = new List<int[]>();
            fitness = new List<double>();
            bestDistance1 = double.MaxValue;
            path1 = new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            bestPath1 = new Polyline()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            //Make the first generation as random sequences
            for (int I = 0; I < popCount; I++)
            {
                for (int J = 0; J < indices1.Length; J++)
                {
                    indices1[J] = J;
                }
                int a;
                int b;
                for (int J = 0; J < 100; J++)
                {
                    a = Rnd.Next(locCount);
                    b = Rnd.Next(locCount);
                    SwapIndices(indices1, a, b);
                }
                population.Add(indices1);
                fitness.Add(0.0);
            }
            //Draw the first generation
            for (int I = 0; I < locations.Count(); I++)
            {
                bestPath1.Points.Add(locations[I]);
                path1.Points.Add(locations[I]);
            }
            canvas1.Children.Add(bestPath1);
            canvas1.Children.Add(path1);
            GenerationCounter = 1;
            bestGeneration = 1;

            //2 : LEXICOGRAPHIC START
            //Initialize Lexicographic data
            for (int J = 0; J < indices2.Length; J++)
            {
                indices2[J] = J;
            }
            for (int I = 0; I < locCount; I++)
            {
                E1 = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.0
                };
                E1.SetValue(Canvas.TopProperty, locations[I].Y - 3);
                E1.SetValue(Canvas.LeftProperty, locations[I].X - 3);
                canvas2.Children.Add(E1);
            }
            if (locCount <= 20) TotalPermutations = fact(locCount);
            bestDistance2 = double.MaxValue;
            finished = false;
            Counter2 = 0;
            path2 = new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            bestPath2 = new Polyline()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            //Draw the first generation
            for (int I = 0; I < locations.Count(); I++)
            {
                path2.Points.Add(locations[I]);
                bestPath2.Points.Add(locations[I]);
            }
            canvas2.Children.Add(bestPath2);
            canvas2.Children.Add(path2);

            //3 : RANDOM START
            //Initialize random data
            for (int J = 0; J < indices3.Length; J++)
            {
                indices3[J] = J;
            }
            for (int I = 0; I < locCount; I++)
            {
                E1 = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.0
                };
                E1.SetValue(Canvas.TopProperty, locations[I].Y - 3);
                E1.SetValue(Canvas.LeftProperty, locations[I].X - 3);
                canvas3.Children.Add(E1);
            }
            bestDistance3 = double.MaxValue;
            Counter3 = 0;
            path3 = new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            bestPath3 = new Polyline()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            //Draw the first generation
            for (int I = 0; I < locations.Count(); I++)
            {
                path3.Points.Add(locations[I]);
                bestPath3.Points.Add(locations[I]);
            }
            canvas3.Children.Add(bestPath3);
            canvas3.Children.Add(path3);
            initialised = true;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!Rendering) return;
            //1 : GENETIC ALGORITHM
            //Evaluate every member of the current population and give it a Fitness value.
            int[] currentBestOrder1 = population[0];
            double currentbestdist1 = double.MaxValue;
            for (int I = 0; I < popCount; I++)
            {
                double dist1 = TotalDistance(population[I]);
                fitness[I] = dist1;
                //Find the best of the current population
                if (dist1 < currentbestdist1)
                {
                    currentbestdist1 = dist1;
                    currentBestOrder1 = population[I];
                }
                //Find the All-time best
                if (dist1 < bestDistance1)
                {
                    bestDistance1 = dist1;
                    bestOrder = population[I];
                    bestGeneration = GenerationCounter;
                    TxtResult1.Text = Math.Round(Math.Sqrt(bestDistance1), 2).ToString() + " found in generation " + GenerationCounter.ToString() + " after " + Math.Round((DateTime.Now - StartTime).TotalSeconds, 2).ToString() + " seconds.";
                    //Show the all-time best path
                    bestPath1.Points.Clear();
                    for (int J = 0; J < locations.Count(); J++)
                    {
                        bestPath1.Points.Add(locations[bestOrder[J]]);
                    }
                }
            }
            Normalize(fitness);
            //Show the best of the current population
            path1.Points.Clear();
            for (int J = 0; J < locations.Count(); J++)
            {
                path1.Points.Add(locations[currentBestOrder1[J]]);
            }
            //Make the next generation
            List<int[]> newPopulation = new List<int[]>();
            if (GenerationCounter - bestGeneration > 100)
            {
                //Stuck at local minimum
                newPopulation = Swapsequence(bestOrder);
                bestGeneration = GenerationCounter;
            }
            else
            {
                int[] newOrder;
                int[] orderA;
                int[] orderB;
                for (int I = 0; I < population.Count(); I++)
                {
                    orderA = bestOrder;
                    orderB = PickOne();
                    newOrder = CrossOver(orderA, orderB);
                    Mutate(newOrder, MutationRate);
                    newPopulation.Add(newOrder);
                }
            }
            population = newPopulation;
            GenerationCounter += 1;

            //2: LEXICOGRAPHIC ALGORITHM
            if (!finished)
            {
                int[] currentBestOrder2 = indices2;
                double currentbestdist2 = double.MaxValue;
                for (int N = 0; N < popCount; N++)
                {
                    if (FindNextPermutation(indices2))
                    {
                        Counter2 += 1;
                        path2.Points.Clear();
                        for (int I = 0; I < locations.Count(); I++)
                        {
                            path2.Points.Add(locations[indices2[I]]);
                        }
                        double dist2 = TotalDistance(indices2);
                        //Find the best of the current loop
                        if (dist2 < currentbestdist2)
                        {
                            currentbestdist2 = dist2;
                            currentBestOrder2 = indices2;
                        }
                        //Find the All-time best
                        if (dist2 < bestDistance2)
                        {
                            bestDistance2 = dist2;
                            findTime = Math.Round(Math.Sqrt(bestDistance2), 2).ToString() + " found after " + Math.Round((DateTime.Now - StartTime).TotalSeconds, 2).ToString() + " seconds.";
                            bestPath2.Points.Clear();
                            for (int I = 0; I < locations.Count(); I++)
                            {
                                bestPath2.Points.Add(locations[indices2[I]]);
                            }
                        }
                    }
                    else
                    {
                        finished = true;
                    }
                }

                TxtResult2.Text = findTime;
                if (locCount <= 20) TxtResult2.Text += Math.Round(100.0 * Counter2 / TotalPermutations, 5).ToString() + "% completed. )";
                //Show the best of the current loop
                path2.Points.Clear();
                for (int J = 0; J < locations.Count(); J++)
                {
                    path2.Points.Add(locations[currentBestOrder2[J]]);
                }
            }
            else
            {
                TxtResult2.Text = findTime + "Finished. )";
                StopRender();
            }

            //3 : RANDOM ALGORITHM
            int[] currentBestOrder3 = indices3;
            double currentbestdist3 = double.MaxValue;
            int a;
            int b;
            for (int N = 0; N < popCount; N++)
            {
                Counter3 += 1;
                a = Rnd.Next(locations.Count());
                b = Rnd.Next(locations.Count());
                SwapIndices(indices3, a, b);
                double dist3 = TotalDistance(indices3);
                //Find the best of the current loop
                if (dist3 < currentbestdist3)
                {
                    currentbestdist3 = dist3;
                    currentBestOrder3 = indices3;
                }
                //Find the All-time best
                if (dist3 < bestDistance3)
                {
                    bestDistance3 = dist3;
                    bestPath3.Points.Clear();
                    for (int I = 0; I < locations.Count(); I++)
                    {
                        bestPath3.Points.Add(locations[indices3[I]]);
                    }
                    TxtResult3.Text = Math.Round(Math.Sqrt(bestDistance3), 2).ToString() + " found after " + Math.Round((DateTime.Now - StartTime).TotalSeconds, 2).ToString() + " seconds.";
                }
            }
            //Show the best of the current loop
            path3.Points.Clear();
            for (int I = 0; I < locations.Count(); I++)
            {
                path3.Points.Add(locations[currentBestOrder3[I]]);
            }
            Title = "Iteration " + GenerationCounter.ToString();
            initialised = false;
        }

        private void Normalize(List<double> list)
        {
            //scale the values reversed
            double min = list.Min();
            double max = list.Max();
            for (int I = 0; I < list.Count(); I++)
            {
                list[I] = 1 - ((list[I] - min + 1) / (max - min));
            }
            //Normalize the values
            double sum = 0.0;
            for (int I = 0; I < list.Count(); I++)
            {
                sum += list[I];
            }
            for (int I = 0; I < list.Count(); I++)
            {
                list[I] = list[I] / sum;
            }
        }

        private int[] PickOne()
        {
            int index = 0;
            double r = Rnd.NextDouble();
            while (r > 0)
            {
                r = r - fitness[index];
                index += 1;
            }
            index -= 1;
            return population[index];
        }

        private int[] CrossOver(int[] A, int[] B)
        {
            int startindex = Rnd.Next(A.Length);
            int endindex = Rnd.Next(startindex + 1, A.Length);
            int num = endindex - startindex;
            int[] neworder = new int[A.Length];
            for (int I = 0; I < neworder.Length; I++)
            {
                neworder[I] = -1;
            }
            Array.Copy(A, startindex, neworder, 0, num);
            for (int I = 0; I < B.Length; I++)
            {
                if (!neworder.Contains(B[I]))
                {
                    neworder[num] = B[I];
                    num += 1;
                }
            }
            return neworder;
        }

        private int[] Mutate(int[] order, double mutationRate)
        {
            for (int I = 0; I < order.Length; I++)
            {
                if (100 * Rnd.NextDouble() < mutationRate)
                {
                    int a = Rnd.Next(order.Count());
                    int b = Rnd.Next(order.Count());
                    int temp = order[a];
                    order[a] = order[b];
                    order[b] = temp;
                }
            }
            return order;
        }

        private List<int[]> Swapsequence(int[] best)
        {
            List<int[]> result = new List<int[]>();
            int[] neworder;
            //Keep 1 copy of the best
            result.Add(best);
            //Reverse parts of the best array with length = 1 then 2 then 3 ....;
            for (int N = 2; N < locations.Count(); N++)
            {
                for (int I = 0; I < locations.Count() - N; I++)
                {
                    neworder = best;
                    Array.Reverse(neworder, I, N);
                    if (result.Count < population.Count)
                    {
                        result.Add(neworder);
                    }
                    else
                    {
                        return result;
                    }
                }
            }
            //Swap elements of the best array at distance = 1 then 2 then 3 .... till swap first and last.;
            for (int N = 1; N < locations.Count(); N++)
            {
                for (int I = 0; I < locations.Count() - N; I++)
                {
                    neworder = best;
                    int temp = neworder[I];
                    neworder[I] = neworder[I + N];
                    neworder[I + N] = temp;
                    if (result.Count < population.Count)
                    {
                        result.Add(neworder);
                    }
                    else
                    {
                        return result;
                    }
                }
            }
            //Keep the rest of the population intact
            if (result.Count < population.Count)
            {
                for (int I = result.Count(); I < population.Count(); I++)
                {
                    result.Add(population[I]);
                }
            }
            return result;
        }

        private void SwapIndices(int[] ind, int a, int b)
        {
            int temp = ind[a];
            ind[a] = ind[b];
            ind[b] = temp;
        }

        private double TotalDistance(int[] ind)
        {
            double result = 0.0;
            for (int I = 0; I < locations.Count() - 1; I++)
            {
                result += (locations[ind[I]].X - locations[ind[I + 1]].X) * (locations[ind[I]].X - locations[ind[I + 1]].X) + (locations[ind[I]].Y - locations[ind[I + 1]].Y) * (locations[ind[I]].Y - locations[ind[I + 1]].Y);
            }
            return result;
        }

        private bool FindNextPermutation(int[] A)
        {
            int X = 0;
            int Y = 0;
            int dummy = 0;
            X = -1;
            //Find largest k with A(k) < A(k+1)
            for (int J = A.GetUpperBound(0) - 1; J >= 0; J--)
            {
                if (A[J] < A[J + 1])
                {
                    X = J;
                    break;
                }
            }
            if (X == -1)
            {
                //No more permutations available;
                return false;
            }
            //Find largest l with A(l) > A(k)
            for (int J = A.GetUpperBound(0); J > X; J--)
            {
                if (A[J] > A[X])
                {
                    Y = J;
                    break;
                }
            }
            //Swap A(k) and A(l)
            dummy = A[X];
            A[X] = A[Y];
            A[Y] = dummy;
            //Reverse array A(k+1) to A(N)
            Array.Reverse(A, X + 1, A.GetUpperBound(0) - X);
            return true;
        }

        private long fact(int n)
        {
            long result = 1;
            for (int I = 1; I <= n; I++)
            {
                result *= I;
            }
            return result;
        }
    }
}