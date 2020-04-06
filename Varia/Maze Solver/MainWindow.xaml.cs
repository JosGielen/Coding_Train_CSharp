using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace A_Star_Maze_Solver
{
    public partial class MainWindow : Window
    {
        private int WaitTime = 0;
        private readonly double Size = 30.0;
        private int Rows = 0;
        private int Cols = 0;
        private Cell[,] Grid;
        private Cell CurrentCell;
        private Cell NextCell;
        private List<Cell> CellStack;
        private Node[,] Nodes;
        private Node goal;
        private Node current;
        private List<Connection> Connections;
        private List<Node> OpenSet;
        private List<Node> ClosedSet;
        private List<Node> my_Path;
        private List<Node> DrawnPath;
        private readonly Random Rnd = new Random();
        private readonly bool AllowRandomRemoval = true;
        private readonly int OpenChance = 10; //%chance of random wall removal
        private bool Building;
        private bool Searching;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Rows = (int)Math.Floor(canvas1.ActualHeight / Size);
            Cols = (int)Math.Floor(canvas1.ActualWidth / Size);
            Grid = new Cell[Rows, Cols];
            Nodes = new Node[Rows, Cols];
            CellStack = new List<Cell>();
            for(int I = 0; I < Rows; I++)
            {
                for (int J = 0; J < Cols; J++)
                {
                    Grid[I, J] = new Cell(I, J, Size);
                    Grid[I, J].Draw(canvas1);
                }
            }
            CurrentCell = Grid[0, 0];
            CurrentCell.IsCurrent = true;
            CellStack.Add(CurrentCell);
            Building = true;
            Searching = false;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            Wait();
            if (Building)  //Generate the Maze
            {
                CurrentCell.IsVisited = true;
                if (AllowRandomRemoval)
                { 
                    if (100 * Rnd.NextDouble() < OpenChance)
                    {
                        NextCell = GetUnvisitedNeighbour(CurrentCell);
                        if (NextCell != null)
                        {
                            RemoveWalls(CurrentCell, NextCell);
                        }
                    }
                }
                NextCell = GetUnvisitedNeighbour(CurrentCell);
                if (NextCell != null)
                {
                    RemoveWalls(CurrentCell, NextCell);
                    CurrentCell.IsCurrent = false;
                    CurrentCell = NextCell;
                    CellStack.Add(CurrentCell);
                }
                else
                {
                    if (CellStack.Count > 0)
                    {
                        CurrentCell.IsCurrent = false;
                        CurrentCell = CellStack.Last();
                        CellStack.RemoveAt(CellStack.Count - 1);
                    }
                    else
                    {
                        Building = false;
                        AStarInit();
                        Searching = true;
                    }
                }
                CurrentCell.IsCurrent = true;
            }
            else if (Searching)  //Find a path through the Maze
            {
                WaitTime = 100;
                my_Path = new List<Node>();
                int MinIndex;
                double tempG;
                if (OpenSet.Count > 0)
                {
                    MinIndex = 0;
                    for (int I = 0; I < OpenSet.Count; I++)
                    {
                        if (OpenSet[I].F < OpenSet[MinIndex].F) MinIndex = I;
                    }
                    current = OpenSet[MinIndex];
                    if ((current.Location - goal.Location).Length < 1)
                    {
                        //We found the goal!!
                        Title = "Goal was reached!!!";
                        //End the search
                        Searching = false;
                    }
                    OpenSet.RemoveAt(MinIndex);
                    ClosedSet.Add(current);
                    //Color the current node Green
                    Grid[current.Row, current.Col].SetFill(Brushes.Blue);
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
                            }
                        }
                    }
                }
                else
                {
                    //Unable to find the goal!
                    Title = "The Goal can not be reached!!!";
                    //End the search
                    Searching = false;
                }
                Render();
            }
        }

        private Cell GetUnvisitedNeighbour(Cell c)
        {
            List<Cell> Neighbours = new List<Cell>();
            int index;
            if (c.Col > 0)
            {
                if (!Grid[c.Row, c.Col - 1].IsVisited) Neighbours.Add(Grid[c.Row, c.Col - 1]);
            }
            if (c.Col < Cols - 1)
            {
                if (!Grid[c.Row, c.Col + 1].IsVisited) Neighbours.Add(Grid[c.Row, c.Col + 1]);
            }
            if (c.Row > 0)
            {
                if (!Grid[c.Row - 1, c.Col].IsVisited) Neighbours.Add(Grid[c.Row - 1, c.Col]);
            }
            if (c.Row < Rows - 1)
            {
                if (!Grid[c.Row + 1, c.Col].IsVisited) Neighbours.Add(Grid[c.Row + 1, c.Col]);
            }
            if (Neighbours.Count == 0)
            {
                return null;
            }
            else
            {
                index = Rnd.Next(Neighbours.Count);
                return Neighbours[index];
            }
        }

        private void RemoveWalls(Cell cell1, Cell cell2)
        {
            if (cell1.Row > cell2.Row)
            {
                cell1.RemoveTopWall();
                cell2.RemoveBottomWall();
            }
            else if (cell1.Row < cell2.Row)
            {
                cell1.RemoveBottomWall();
                cell2.RemoveTopWall();
            }
            else if (cell1.Col < cell2.Col)
            {
                cell1.RemoveRightWall();
                cell2.RemoveLeftWall();
            }
            else if (cell1.Col > cell2.Col)
            {
                cell1.RemoveLeftWall();
                cell2.RemoveRightWall();
            }
        }

        private void AStarInit()
        {
            DrawnPath = new List<Node>();
            OpenSet = new List<Node>();
            ClosedSet = new List<Node>();
            Connections = new List<Connection>();
            //Convert Cells to Nodes 
            for (int I = 0; I < Rows; I++)
            {
                for (int J = 0; J < Cols; J++)
                {
                    Nodes[I, J] = new Node(I, J, Size, Size);
                }
            }
            //Make a Connection when there are no walls between 2 cells
            for (int I = 0; I < Rows; I++)
            {
                for (int J = 0; J < Cols; J++)
                {
                    if (I > 0)
                    {
                        if (!Grid[I, J].HasTopWall) Connections.Add(new Connection(Nodes[I, J], Nodes[I - 1, J]));
                    }
                    if (I < Rows - 1)
                    {
                        if (!Grid[I, J].HasBottomWall) Connections.Add(new Connection(Nodes[I, J], Nodes[I + 1, J]));
                    }
                    if (J > 0)
                    {
                        if (!Grid[I, J].HasLeftWall) Connections.Add(new Connection(Nodes[I, J], Nodes[I, J - 1]));
                    }
                    if (J < Cols - 1)
                    {
                        if (!Grid[I, J].HasRightWall) Connections.Add(new Connection(Nodes[I, J], Nodes[I, J + 1]));
                    }
                }
            }
            //Set the goal at the bottom Right cell
            goal = new Node(Rows - 1, Cols - 1, Size, Size);
            //Set the Start Node
            Nodes[0, 0].G = 0;
            Nodes[0, 0].F = Nodes[0, 0].Distance(goal);
            OpenSet.Add(Nodes[0, 0]);
        }

        private void Render()
        {
            Node temp;
            //Remove the previous path
            foreach (Node n in DrawnPath)
            {
                Grid[n.Row, n.Col].SetFill(Brushes.Black);
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
                for (int I = 0; I < my_Path.Count; I++)
                {
                    Grid[my_Path[I].Row, my_Path[I].Col].SetFill(Brushes.Blue);
                    DrawnPath.Add(my_Path[I]);
                }
            }
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
