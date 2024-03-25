using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RayTracing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Camera my_Camera;
        private double CameraSpeed = 2.0;
        private double FrontWallDistance;
        private double BackWallDistance;
        private List<Wall> my_Walls;
        private bool my_MouseDown = false;
        private int RayCount;
        private List<Line> WallLines;
        private double FirstPersonViewScale = 30;
        private Random Rnd = new Random();
        //Maze Generator
        private bool Building;
        private bool AllowRandomRemoval = true;
        private int Size = 44;
        private int Rows = 0;
        private int Cols = 0;
        private Cell[,] Grid;
        private Cell CurrentCell;
        private Cell NextCell;
        private List<Cell> CellStack;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            RayCount = (int)(canvas2.ActualWidth);
            my_Walls = new List<Wall>();
            Wall w;
            Brush WallColor = Brushes.DarkKhaki;
            //Initialize the Maze generator used to make the walls
            Building = true;
            Rows = (int)(Math.Floor(canvas1.ActualHeight / Size));
            Cols = (int)(Math.Floor(canvas1.ActualWidth / Size));
            Grid = new Cell[Rows, Cols];
            CellStack = new List<Cell>();
            for (int I = 0; I < Rows; I++)
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
            //Make the outer walls
            w = new Wall(new Point(1, 1), new Point(Cols * Size - 1, 1), WallColor); //Top wall;
            my_Walls.Add(w);
            w = new Wall(new Point(1, 1), new Point(1, Rows * Size - 1), WallColor); //Left Wall;
            my_Walls.Add(w);
            w = new Wall(new Point(1, Rows * Size - 1), new Point(Cols * Size - 1, Rows * Size - 1), WallColor); //Bottom wall;
            my_Walls.Add(w);
            w = new Wall(new Point(Cols * Size - 1, 1), new Point(Cols * Size - 1, Rows * Size - 1), WallColor); //Right wall;
            my_Walls.Add(w);
            //Set the ground and sky in the First Person View
            Rectangle ground = new Rectangle()
            {
                Width = canvas2.ActualWidth,
                Height = canvas2.ActualHeight / 2,
                Fill = Brushes.LightBlue
            };
            ground.SetValue(Canvas.LeftProperty, 0.0);
            ground.SetValue(Canvas.TopProperty, 0.0);
            canvas2.Children.Add(ground);
            Rectangle Sky = new Rectangle()
            {
                Width = canvas2.ActualWidth,
                Height = canvas2.ActualHeight / 2,
                Fill = Brushes.Green
            };
            Sky.SetValue(Canvas.LeftProperty, 0.0);
            Sky.SetValue(Canvas.TopProperty, canvas2.ActualHeight / 2);
            canvas2.Children.Add(Sky);
            //Make the WallLines for the First Person View
            WallLines = new List<Line>();
            Line wl;
            for (int I = 0; I <= RayCount; I++)
            {
                wl = new Line()
                {
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 2.0,
                    X1 = I,
                    Y1 = 0.0,
                    X2 = I,
                    Y2 = canvas2.ActualHeight
                };
                WallLines.Add(wl);
                canvas2.Children.Add(wl);
            }
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Wall w;
            Brush wallColor = Brushes.White;
            //Generate the maze
            if (Building)
            {
                CurrentCell.IsVisited = true;
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
                        //THE MAZE IS FINISHED
                        Building = false;
                        canvas1.Children.Clear();
                        //Make a camera
                        my_Camera = new Camera(new Point(Size / 2, Size / 2), 0.0, 45, RayCount);
                        my_Camera.Show(canvas1);
                        //Create the maze walls
                        for (int I = 0; I < Rows; I++)
                        {
                            for (int J = 0; J < Cols; J++)
                            {
                                if (Grid[I, J].HasTopWall)
                                {
                                    wallColor = new SolidColorBrush(Colors.Brown);
                                    w = new Wall(new Point(J * Size, I * Size), new Point((J + 1) * Size, I * Size), wallColor);
                                    my_Walls.Add(w);
                                }
                                if (Grid[I, J].HasLeftWall)
                                {
                                    wallColor = new SolidColorBrush(Colors.DarkGray);
                                    w = new Wall(new Point(J * Size, I * Size), new Point(J * Size, (I + 1) * Size), wallColor);
                                    my_Walls.Add(w);
                                }
                                if (Grid[I, J].HasBottomWall)
                                {
                                    wallColor = new SolidColorBrush(Colors.Brown);
                                    w = new Wall(new Point(J * Size, (I + 1) * Size), new Point((J + 1) * Size, (I + 1) * Size), wallColor);
                                    my_Walls.Add(w);
                                }
                                if (Grid[I, J].HasRightWall)
                                {
                                    wallColor = new SolidColorBrush(Colors.DarkGray);
                                    w = new Wall(new Point((J + 1) * Size, I * Size), new Point((J + 1) * Size, (I + 1) * Size), wallColor);
                                    my_Walls.Add(w);
                                }
                            }
                        }
                        //Show the walls
                        for (int I = 0; I < my_Walls.Count(); I++)
                        {
                            my_Walls[I].Show(canvas1);
                        }
                    }
                }
                CurrentCell.IsCurrent = true;
            }
            else
            {
                //For each Ray calculate the closest Wall intersect
                double dist;
                double mindist;
                Point intPt;
                Point closestPt;
                double WallLineHeight;
                for (int I = 0; I < my_Camera.Rays.Count(); I++)
                {
                    mindist = double.MaxValue;
                    closestPt = new Point(-1, -1);
                    for (int J = 0; J < my_Walls.Count(); J++)
                    {
                        intPt = my_Walls[J].Intersect(my_Camera.Rays[I]);
                        if (intPt.X >= 0 & intPt.Y >= 0)
                        {
                            dist = Math.Sqrt((my_Camera.Pos.X - intPt.X) * (my_Camera.Pos.X - intPt.X) + (my_Camera.Pos.Y - intPt.Y) * (my_Camera.Pos.Y - intPt.Y));
                            if (dist < mindist)
                            {
                                mindist = dist;
                                closestPt = intPt;
                                wallColor = my_Walls[J].WallColor;
                            }
                        }
                    }
                    //End the ray at the closest intersect point
                    my_Camera.Rays[I].X2 = closestPt.X;
                    my_Camera.Rays[I].Y2 = closestPt.Y;
                    //Set the wall height in the First Person View
                    double rayAngleOffset = Vector.AngleBetween(my_Camera.Rays[I].Dir, my_Camera.Dir) * Math.PI / 180;
                    WallLineHeight = FirstPersonViewScale * canvas2.ActualHeight / (mindist * Math.Abs(Math.Cos(rayAngleOffset)));
                    WallLines[I].Y1 = (canvas2.ActualHeight - WallLineHeight) / 2;
                    WallLines[I].Y2 = (canvas2.ActualHeight + WallLineHeight) / 2;
                    WallLines[I].Stroke = wallColor;
                }
                //Calculate the camera front to Wall distance
                FrontWallDistance = double.MaxValue;
                Point front = my_Camera.Pos + my_Camera.Size * my_Camera.Dir;
                for (int I = 0; I < my_Walls.Count(); I++)
                {
                    dist = (my_Walls[I].Intersect(my_Camera.Rays[(int)(RayCount / 2)]) - front).Length;
                    if (dist < FrontWallDistance) FrontWallDistance = dist;
                }
                //Calculate the camera back to Wall distance
                BackWallDistance = double.MaxValue;
                Point back = my_Camera.Pos - my_Camera.Size * my_Camera.Dir;
                Ray backray = new Ray(my_Camera.Pos, my_Camera.Angle + 180);
                for (int I = 0; I < my_Walls.Count(); I++)
                {
                    dist = (my_Walls[I].Intersect(backray) - back).Length;
                    if (dist < BackWallDistance) BackWallDistance = dist;
                }
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
            if (Neighbours.Count() == 0)
            {
                return null;
            }
            else
            {
                index = Rnd.Next(Neighbours.Count);
                return Neighbours[index];
            }
        }

        private void RemoveWalls(Cell Cell1, Cell Cell2)
        {
            if (Cell1.Row > Cell2.Row)
            {
                Cell1.RemoveTopWall();
                Cell2.RemoveBottomWall();
            }
            else if (Cell1.Row < Cell2.Row)
            {
                Cell1.RemoveBottomWall();
                Cell2.RemoveTopWall();
            }
            else if (Cell1.Col < Cell2.Col)
            {
                Cell1.RemoveRightWall();
                Cell2.RemoveLeftWall();
            }
            else if (Cell1.Col > Cell2.Col)
            {
                Cell1.RemoveLeftWall();
                Cell2.RemoveRightWall();
            }
            if (AllowRandomRemoval)
            {
                //Remove some walls at random to create more open space
                if (100 * Rnd.NextDouble() < 5)
                {
                    Cell1.RemoveRightWall();
                    Cell2.RemoveLeftWall();
                }
                if (100 * Rnd.NextDouble() < 5)
                {
                    Cell1.RemoveTopWall();
                    Cell2.RemoveBottomWall();
                }
            }

        }


        private void Window_KeyDown(Object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (FrontWallDistance > 1) my_Camera.Pos = my_Camera.Pos + CameraSpeed * my_Camera.Dir;
                    break;
                case Key.Down:
                    if (BackWallDistance > 1) my_Camera.Pos = my_Camera.Pos - CameraSpeed * my_Camera.Dir;
                    break;
                case Key.Left:
                    my_Camera.Angle -= CameraSpeed;
                    break;
                case Key.Right:
                    my_Camera.Angle += CameraSpeed;
                    break;
            }
            my_Camera.Update();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}