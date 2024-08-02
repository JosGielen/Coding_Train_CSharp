using JG_GL;
using Self_Avoiding_Walk;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3D_Self_Avoiding_Walk
{
    public partial class MainWindow : Window
    {
        //3D PolyLine
        private PolyLineGeometry my_Polyline;
        private List<Vector3D> points = new List<Vector3D>();
        //Camera positioning
        private Vector3D CamStartPos = new Vector3D(0.0, 5.0, 100.0);
        private Vector3D CamStartTarget = new Vector3D(0.0, 0.0, 0.0);
        private Vector3D CamUpDir = new Vector3D(0.0, 1.0, 0.0);
        //Walker setup
        private Cell[,,] cells;
        private List<Cell> cellList;
        private int stepSize = 5;
        private int GridSize = 20;
        private bool Solved;
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            my_Polyline = new PolyLineGeometry()
            {
                Position = new Vector3D(0, 0, 0),
                InitialRotationAxis = new Vector3D(0, 0, 0),
                RotationAxis = new Vector3D(0, 1, 0),
                RotationSpeed = 0.001
            };
            scene1.ShowAxes = false;
            scene1.ShowGrid = false;
            scene1.Camera.Position = CamStartPos;
            scene1.Camera.TargetPosition = CamStartTarget;
            scene1.Camera.UpDirection = CamUpDir;
            scene1.ViewDistance = 300;
            my_Polyline.AllowScaling = false;
            my_Polyline.Closed = false;
            points.Add(new Vector3D(0, 0, 0));
            my_Polyline.Points = points;
            my_Polyline.SetVertexColors(Environment.CurrentDirectory + "\\Rainbow continuous.cpl", 128);
            scene1.AddGeometry(my_Polyline);
            cells = new Cell[GridSize, GridSize, GridSize];
            cellList = new List<Cell>();
            Solved = false;
            //Create all cells unused
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    for (int k = 0; k < GridSize; k++)
                    {
                        cells[i, j, k] = new Cell(i, j, k, GridSize);
                    }
                }
            }
            //Check the available neightbours
            List<int> free;
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    for (int k = 0; k < GridSize; k++)
                    {
                        free = cells[i, j, k].FreeNeighbours(cells);
                    }
                }
            }
            //Start in the middle
            cellList.Add(cells[GridSize / 2, GridSize / 2, GridSize / 2]);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (Solved) { return; }
            Cell last = cellList.Last();
            last.Used = true;
            Cell next = null;
            List<int> FreeDirs = last.FreeNeighbours(cells);
            if (FreeDirs.Count > 0)
            {
                int free = FreeDirs[Rnd.Next(FreeDirs.Count)];
                if (free == 0) //Left
                {
                    next = cells[last.X - 1, last.Y, last.Z];
                }
                if (free == 1) //Right
                {
                    next = cells[last.X + 1, last.Y, last.Z];
                }
                if (free == 2) //Top
                {
                    next = cells[last.X, last.Y - 1, last.Z];
                }
                if (free == 3) //Bottom
                {
                    next = cells[last.X, last.Y + 1, last.Z];
                }
                if (free == 4) //Front
                {
                    next = cells[last.X, last.Y, last.Z - 1];
                }
                if (free == 5) //Back
                {
                    next = cells[last.X, last.Y, last.Z + 1];
                }
                if (next != null)
                {
                    Vector3D v = new Vector3D((next.X - GridSize / 2) * stepSize, (next.Y - GridSize / 2) * stepSize, (next.Z - GridSize / 2) * stepSize);
                    points.Add(v);
                    cellList.Add(next);
                    last.SetTried(free);
                    next.Used = true;
                    //Check solved
                    bool AllUsed = true;
                    for (int i = 0; i < GridSize; i++)
                    {
                        for (int j = 0; j < GridSize; j++)
                        {
                            for (int k = 0; k < GridSize; k++)
                            {
                                if (!cells[i, j, k].Used)
                                {
                                    AllUsed = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (AllUsed == true)
                    {
                        Solved = true;
                        CompositionTarget.Rendering -= CompositionTarget_Rendering;
                    }
                }
            }
            else
            {
                cellList.Last().Used = false;
                cellList.Last().UnTried();
                cellList.RemoveAt(cellList.Count - 1);
                points.RemoveAt(points.Count - 1);
            }
            //Redraw the Polyline
            my_Polyline.Points = points;
            my_Polyline.GenerateGeometry(scene1);
            my_Polyline.Update();

            scene1.Render();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}