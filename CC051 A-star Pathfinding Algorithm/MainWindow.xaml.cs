using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_Star_Pathfinder
{
    public partial class MainWindow : Window
    {
        private readonly int WaitTime = 50;
        private bool App_Started = false;
        private readonly int Rows = 40;
        private readonly int Cols = 40;
        private readonly int ConnChance = 25;
        private Node[,] Nodes;
        private Node goal;
        private Node current;
        private List<Connection> Connections;
        private double CellWidth;
        private double CellHeight;
        private readonly double NodeSize = 6;
        private readonly Random Rnd = new Random();
        private List<Node> OpenSet;
        private List<Node> ClosedSet;
        private List<Node> my_Path;
        private List<Line> DrawnPath;
        private List<Ellipse> DrawNodes;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Nodes = new Node[Cols + 1, Rows + 1];
            Init();
        }


        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!App_Started)
            {
                //Init();
                App_Started = true;
                Start();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App_Started = false;
            Environment.Exit(0);
        }

        private void Init()
        {
            CellWidth = Canvas1.ActualWidth / (Cols + 1);   //Leave half a cell width at the edges
            CellHeight = Canvas1.ActualHeight / (Rows + 1); //so devide by Cols + 1 and Rows + 1
                                                            //Initialize the Lists
            OpenSet = new List<Node>();
            ClosedSet = new List<Node>();
            DrawnPath = new List<Line>();
            DrawNodes = new List<Ellipse>();
            Connections = new List<Connection>();
            //Set the goal
            goal = new Node(Cols, Rows, CellWidth, CellHeight);
            //Make the Nodes
            for (int I = 0; I <= Cols; I++)
            {
                for (int J = 0; J <= Rows; J++)
                {
                    Nodes[I, J] = new Node(I, J, CellWidth, CellHeight);
                }
            }
            //Make random connections between Nodes
            for (int I = 0; I <= Cols; I++)
            {
                for (int J = 0; J <= Rows; J++)
                {
                    if (I > 0 & J > 0)
                    {
                        if (Rnd.Next(100) < ConnChance)
                        {
                            Connections.Add(new Connection(Nodes[I, J], Nodes[I - 1, J - 1]));
                        }
                    }
                    if (I > 0)
                    {
                        if (Rnd.Next(100) < ConnChance)
                        {
                            Connections.Add(new Connection(Nodes[I, J], Nodes[I - 1, J]));
                        }
                    }
                    if (I > 0 & J < Rows)
                    {
                        if (Rnd.Next(100) < ConnChance)
                        {
                            Connections.Add(new Connection(Nodes[I, J], Nodes[I - 1, J + 1]));
                        }
                    }
                    if (J > 0)
                    {
                        if (Rnd.Next(100) < ConnChance)
                        {
                            Connections.Add(new Connection(Nodes[I, J], Nodes[I, J - 1]));
                        }
                    }
                    if (J < Rows)
                    {
                        if (Rnd.Next(100) < ConnChance)
                        {
                            Connections.Add(new Connection(Nodes[I, J], Nodes[I, J + 1]));
                        }
                    }
                    if (I < Cols & J > 0)
                    {
                        if (Rnd.Next(100) < ConnChance)
                        {
                            Connections.Add(new Connection(Nodes[I, J], Nodes[I + 1, J - 1]));
                        }
                    }
                    if (I < Cols)
                    {
                        if (Rnd.Next(100) < ConnChance)
                        {
                            Connections.Add(new Connection(Nodes[I, J], Nodes[I + 1, J]));
                        }
                    }
                    if (I < Cols & J < Rows)
                    {
                        if (Rnd.Next(100) < ConnChance)
                        {
                            Connections.Add(new Connection(Nodes[I, J], Nodes[I + 1, J + 1]));
                        }
                    }
                }
            }
            //Connect the Start and End Nodes to their neighbors
            if (!ConnectionExist(Nodes[0, 0], Nodes[0, 1])) Connections.Add(new Connection(Nodes[0, 0], Nodes[0, 1]));
            if (!ConnectionExist(Nodes[0, 0], Nodes[1, 1]))  Connections.Add(new Connection(Nodes[0, 0], Nodes[1, 1]));
            if (!ConnectionExist(Nodes[0, 0], Nodes[1, 0])) Connections.Add(new Connection(Nodes[0, 0], Nodes[1, 0]));
            if (!ConnectionExist(Nodes[Cols, Rows], Nodes[Cols - 1, Rows])) Connections.Add(new Connection(Nodes[Cols, Rows], Nodes[Cols - 1, Rows]));
            if (!ConnectionExist(Nodes[Cols, Rows], Nodes[Cols - 1, Rows - 1])) Connections.Add(new Connection(Nodes[Cols, Rows], Nodes[Cols - 1, Rows - 1]));
            if (!ConnectionExist(Nodes[Cols, Rows], Nodes[Cols, Rows - 1])) Connections.Add(new Connection(Nodes[Cols, Rows], Nodes[Cols, Rows - 1]));

            //Draw the Nodes as small black circles
            Canvas1.Children.Clear();
            Ellipse ell;
            for (int I = 0; I <= Cols; I++)
            {
                for (int J = 0; J <= Rows; J++)
                {
                    ell = new Ellipse()
                    {
                        Width = NodeSize,
                        Height = NodeSize,
                        Stroke = Brushes.Black,
                        Fill = Brushes.Black,
                        StrokeThickness = 1
                    };
                    Canvas.SetLeft(ell, Nodes[I, J].Location.X + CellWidth / 2 - NodeSize / 2);
                    Canvas.SetTop(ell, Nodes[I, J].Location.Y + CellHeight / 2 - NodeSize / 2);
                    DrawNodes.Add(ell);
                    Canvas1.Children.Add(ell);
                }
            }

            //Draw the connections as black lines
            Line l;
            foreach (Connection conn in Connections)
            {
                l = new Line()
                {
                    X1 = conn.Node1.Location.X + CellWidth / 2,
                    Y1 = conn.Node1.Location.Y + CellHeight / 2,
                    X2 = conn.Node2.Location.X + CellWidth / 2,
                    Y2 = conn.Node2.Location.Y + CellHeight / 2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas1.Children.Add(l);
            }

            //Set the Start Node
            Nodes[0, 0].G = 0;
            Nodes[0, 0].F = Nodes[0, 0].Distance(goal);
            OpenSet.Add(Nodes[0, 0]);
        }

        private void Start()
        {
            Title = "Searching.";
            my_Path = new List<Node>();
            int index = 0;
            int MinIndex;
            double tempG;
            while (App_Started)
            {
                if (OpenSet.Count > 0)
                {
                    MinIndex = 0;
                    for (int I = 0; I < OpenSet.Count; I++)
                    {
                        if (OpenSet[I].F < OpenSet[MinIndex].F) MinIndex = I;
                    }
                    current = OpenSet[MinIndex];
                    if (current.Location == goal.Location)
                    {
                        //We found the goal!!
                        Title = "Goal was reached!!!";
                        //End the search
                        App_Started = false;
                    }
                    OpenSet.RemoveAt(MinIndex);
                    ClosedSet.Add(current);
                    //Color the current node Red
                    index = current.Col * (Rows + 1) + current.Row;
                    DrawNodes[index].Fill = Brushes.Red;
                    DrawNodes[index].Stroke = Brushes.Red;
                    foreach (Node n in NodeConnections(current))
                    {
                        if (!ClosedSet.Contains(n))
                        {
                            tempG = current.G + current.Distance(n);
                            if (OpenSet.Contains(n))
                            {
                                if (tempG < n.G)
                                {
                                    n.G = tempG;
                                    n.F = n.G + n.Distance(goal);
                                    n.Previous = current;
                                }
                            }
                            else
                            {
                                n.G = tempG;
                                n.F = n.G + n.Distance(goal);
                                n.Previous = current;
                                OpenSet.Add(n);
                                //Color this Neighbor green
                                index = n.Col * (Rows + 1) + n.Row;
                                DrawNodes[index].Fill = Brushes.Lime;
                                DrawNodes[index].Stroke = Brushes.Lime;
                            }
                        }
                    }
                }
                else
                {
                    //Unable to find the goal!
                    Title = "The Goal can not be reached!!!";
                    //End the search
                    App_Started = false;
                }
                Render();
                Dispatcher.Invoke(Wait, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }
        }

        private void Render()
        {
            Node temp;
            Line l;
            //Remove the previous path
            foreach (Line ln in DrawnPath)
            {
                Canvas1.Children.Remove(ln);
            }
            //Get the current Path
            my_Path.Clear();
            temp = current;
            my_Path.Add(temp);
            while (temp.Previous != null)
            {
                my_Path.Add(temp.Previous);
                temp = temp.Previous;
            }
            //Draw the current Path
            DrawnPath.Clear();
            if (my_Path.Count > 0)
            {
                for (int I = 0; I < my_Path.Count - 1; I++)
                {
                    l = new Line()
                    {
                        X1 = my_Path[I].Location.X + CellWidth / 2,
                        Y1 = my_Path[I].Location.Y + CellHeight / 2,
                        X2 = my_Path[I + 1].Location.X + CellWidth / 2,
                        Y2 = my_Path[I + 1].Location.Y + CellHeight / 2,
                        Stroke = Brushes.Blue,
                        StrokeThickness = 3
                    };
                    Canvas1.Children.Add(l);
                    DrawnPath.Add(l);
                }
            }
        }

        private bool ConnectionExist(Node node1, Node node2)
        {
            foreach (Connection conn in Connections)
            {
                if (conn.Node1.Location == node1.Location & conn.Node2.Location == node2.Location) return true;
            }
            return false;
        }

        private List<Node> NodeConnections(Node n)
        {
            List<Node> connNodes = new List<Node>();
            foreach (Connection conn in Connections)
            {
                if (conn.Node1.Location == n.Location)
                {
                    if (!connNodes.Contains(conn.Node2)) connNodes.Add(conn.Node2);
                }
                if (conn.Node2.Location == n.Location)
                {
                    if (!connNodes.Contains(conn.Node1)) connNodes.Add(conn.Node1);
                }
            }
            return connNodes;
        }

        private void Wait()
        {
            Thread.Sleep(WaitTime);
        }

    }
}
