using Microsoft.Win32;
using RubiksCube.Kociemba;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace RubiksCube
{
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate(int t);
        private Cube3D Cube;   //A Cube3D contains 27 Cubelets
        private CubeMap Map;   //A CubeMap contains 54 CubeletFaces
        private QuarterRotation myRotation;
        private QuarterRotation nextRotation;
        private bool ReverseMoves;
        private double My_RotationSpeed = 2;
        private string[] Solutionmoves;
        private int SolutionIndex;
        private string CubeName = ""; //Path + Filename + extension 
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\Assets\\Kociemba\\Tables\\flip"))
            {
                string facelets = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB";
                FaceCube fc = new FaceCube(facelets);
                CubieCube cc = fc.toCubieCube();
                CoordCubeBuildTables c = new CoordCubeBuildTables(cc, true);
            }
            Init();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void Init()
        {
            canvas1.Children.Clear();
            Scene1.Geometries.Clear();
            Map = new CubeMap(canvas1);
            Cube = new Cube3D(Scene1, Map)
            {
                RotationSpeed = My_RotationSpeed
            };
            Scene1.Camera.Position = new Vector3D(90, 60, 90);
        }

        #region "Window Events"

        private void Canvas1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point Pt = e.GetPosition(canvas1);
            Map.ToggleColor(Pt);
            Cube.UpdateColors();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (myRotation != QuarterRotation.NONE) { return; }
            switch (e.Key)
            {
                case Key.U:
                    if (Keyboard.IsKeyDown(Key.RightShift))
                    {
                        myRotation = QuarterRotation.UPCCW;
                    }
                    else
                    {
                        myRotation = QuarterRotation.UPCW;
                    }
                    break;
                case Key.D:
                    if (Keyboard.IsKeyDown(Key.RightShift))
                    {
                        myRotation = QuarterRotation.DWNCCW;
                    }
                    else
                    {
                        myRotation = QuarterRotation.DWNCW;
                    }
                    break;
                case Key.L:
                    if (Keyboard.IsKeyDown(Key.RightShift))
                    {
                        myRotation = QuarterRotation.LFTCCW;
                    }
                    else
                    {
                        myRotation = QuarterRotation.LFTCW;
                    }
                    break;
                case Key.R:
                    if (Keyboard.IsKeyDown(Key.RightShift))
                    {
                        myRotation = QuarterRotation.RGTCCW;
                    }
                    else
                    {
                        myRotation = QuarterRotation.RGTCW;
                    }
                    break;
                case Key.F:
                    if (Keyboard.IsKeyDown(Key.RightShift))
                    {
                        myRotation = QuarterRotation.FRTCCW;
                    }
                    else
                    {
                        myRotation = QuarterRotation.FRTCW;
                    }
                    break;
                case Key.B:
                    if (Keyboard.IsKeyDown(Key.RightShift))
                    {
                        myRotation = QuarterRotation.BCKCCW;
                    }
                    else
                    {
                        myRotation = QuarterRotation.BCKCW;
                    }
                    break;
            }
            if (myRotation != QuarterRotation.NONE) { Cube.Scrable.Add(myRotation); }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) //Prevent the reset of the 3D Camera position to the center.
            {
                ReverseMoves = true;
                e.Handled= true;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsLoaded) { return; }
            List<int> faceColors = new List<int>();
            for (int i = 0; i < Map.CubeletFaces.Count; i++)
            {
                faceColors.Add(Map.CubeletFaces[i].FaceColorNumber);
            }
            Map.Resize(canvas1, faceColors);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion

        #region "Menu Events"

        private void mnuLoad_Click(object sender, RoutedEventArgs e)
        {
            StreamReader myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Multiselect = false;
            openFileDialog1.DefaultExt = ".*";
            openFileDialog1.Filter = "Rubiks Cube files (*.cube)|*.cube|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog().Value)
            {
                try
                {
                    myStream = new StreamReader(openFileDialog1.OpenFile());
                    if (myStream != null)
                    {
                        //Lees de data in de file
                        string FaceletColors = myStream.ReadLine();
                        for (int i = 0; i < Map.CubeletFaces.Count; i++)
                        {
                            Map.CubeletFaces[i].FaceColorNumber = int.Parse(FaceletColors.Substring(i,1));
                        }
                        Cube.UpdateColors();
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Cannot read file from disk. Original error: " + Ex.Message);
                }
                finally
                {
                    // Check this again, since we need to make sure we didn't throw an exception on open.
                    if ((myStream != null))
                    {
                        myStream.Close();
                    }
                }
            }
        }

        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            if (CubeName == "")
            {
                //Request filename via SaveAs
                mnuSaveAs_Click(sender, e);
            }
            else
            {
                SaveFile();
            }
        }

        private void mnuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            saveFileDialog1.Filter = "Rubiks Cube files (*.cube)|*.cube|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                CubeName = saveFileDialog1.FileName;
                SaveFile();
            }
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mnuScramble_Click(object sender, RoutedEventArgs e)
        {
            int move;
            //Perform 50 random rotations
            for (int i = 0; i < 50; i++)
            {
                move = Rnd.Next(6);
                switch (move)
                {
                    case 0:
                        Cube.Scrable.Add(QuarterRotation.UPCW);
                        break;
                    case 1:
                        Cube.Scrable.Add(QuarterRotation.RGTCW);
                        break;
                    case 2:
                        Cube.Scrable.Add(QuarterRotation.FRTCW);
                        break;
                    case 3:
                        Cube.Scrable.Add(QuarterRotation.BCKCW);
                        break;
                    case 4:
                        Cube.Scrable.Add(QuarterRotation.LFTCW);
                        break;
                    case 5:
                        Cube.Scrable.Add(QuarterRotation.DWNCW);
                        break;
                }
            }
            ReverseMoves = true;
        }

        private void mnuReset_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        #endregion

        #region "Rotation Buttons"

        private void BtnBackCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.BCKCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnUpCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.UPCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnUpCCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.UPCCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnBackCCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.BCKCCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnLeftCCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.LFTCCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnLeftCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.LFTCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnRightCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.RGTCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnRightCCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.RGTCCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnFrontCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.FRTCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnDownCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.DWNCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnDownCCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.DWNCCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        private void BtnFrontCCW_Click(object sender, RoutedEventArgs e)
        {
            if (myRotation == QuarterRotation.NONE)
            {
                myRotation = QuarterRotation.FRTCCW;
                Cube.Scrable.Add(myRotation);
            }
        }

        #endregion

        #region "Utilities"

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!IsLoaded) { return; }
            if (ReverseMoves)
            {
                if (Cube.Rotation == QuarterRotation.NONE)
                {
                    myRotation = Reverse(Cube.GetLastRotation());
                    if (myRotation == QuarterRotation.NONE)
                    {
                        ReverseMoves = false;
                        return;
                    }
                }
            }
            //Apply the rotation
            Cube.Rotate(myRotation);
            if (Cube.Rotation == QuarterRotation.NONE) //Cube Rotation stopped
            {
                myRotation = QuarterRotation.NONE;
            }
            //Render the scene.
            Scene1.Render();
        }

        private QuarterRotation Reverse(QuarterRotation rot)
        {
            switch (rot)
            {
                case QuarterRotation.UPCW:
                    return QuarterRotation.UPCCW;
                case QuarterRotation.UPCCW:
                    return QuarterRotation.UPCW;
                case QuarterRotation.DWNCW:
                    return QuarterRotation.DWNCCW;
                case QuarterRotation.DWNCCW:
                    return QuarterRotation.DWNCW;
                case QuarterRotation.LFTCW:
                    return QuarterRotation.LFTCCW;
                case QuarterRotation.LFTCCW:
                    return QuarterRotation.LFTCW;
                case QuarterRotation.RGTCW:
                    return QuarterRotation.RGTCCW;
                case QuarterRotation.RGTCCW:
                    return QuarterRotation.RGTCW;
                case QuarterRotation.FRTCW:
                    return QuarterRotation.FRTCCW;
                case QuarterRotation.FRTCCW:
                    return QuarterRotation.FRTCW;
                case QuarterRotation.BCKCW:
                    return QuarterRotation.BCKCCW;
                case QuarterRotation.BCKCCW:
                    return QuarterRotation.BCKCW;
            }
            return QuarterRotation.NONE;
        }

        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            //Get the Kociemba notation of the cube
            string searchString = "";
            string info = "";
            for (int i = 0; i < Map.CubeletFaces.Count; i++)
            {
                switch (Map.CubeletFaces[i].FaceColorNumber)
                {
                    case 0:
                        searchString += "U";
                        break;
                    case 1:
                        searchString += "D";
                        break;
                    case 2:
                        searchString += "F";
                        break;
                    case 3:
                        searchString += "B";
                        break;
                    case 4:
                        searchString += "L";
                        break;
                    case 5:
                        searchString += "R";
                        break;
                }
            }
            string solution = Search.solution(searchString, out info);
            txtSolution.Text = solution;
            MessageBox.Show(info, "Kociemba info");
            if (solution.Length > 0)
            {
                Solutionmoves = txtSolution.Text.Split(" ");
                SolutionIndex = 0;
                txtCurrent.Text = Solutionmoves[SolutionIndex];
            }
        }

        private void btnUnscramble_Click(object sender, RoutedEventArgs e)
        {
            if (nextRotation != QuarterRotation.NONE)
            {
                myRotation = nextRotation;
                nextRotation = QuarterRotation.NONE;
                SolutionIndex++;
                txtCurrent.Text = Solutionmoves[SolutionIndex];
                return;
            }
            if (SolutionIndex < Solutionmoves.Length)
            {
                switch (Solutionmoves[SolutionIndex])
                {
                    case "U":
                        myRotation = QuarterRotation.UPCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "D":
                        myRotation = QuarterRotation.DWNCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "F":
                        myRotation = QuarterRotation.FRTCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "B":
                        myRotation = QuarterRotation.BCKCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "L":
                        myRotation = QuarterRotation.LFTCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "R":
                        myRotation = QuarterRotation.RGTCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "U'":
                        myRotation = QuarterRotation.UPCCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "D'":
                        myRotation = QuarterRotation.DWNCCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "F'":
                        myRotation = QuarterRotation.FRTCCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "B'":
                        myRotation = QuarterRotation.BCKCCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "L'":
                        myRotation = QuarterRotation.LFTCCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "R'":
                        myRotation = QuarterRotation.RGTCCW;
                        nextRotation = QuarterRotation.NONE;
                        SolutionIndex++;
                        break;
                    case "U2":
                        myRotation = QuarterRotation.UPCW;
                        nextRotation = QuarterRotation.UPCW;
                        break;
                    case "D2":
                        myRotation = QuarterRotation.DWNCW;
                        nextRotation = QuarterRotation.DWNCW;
                        break;
                    case "F2":
                        myRotation = QuarterRotation.FRTCW;
                        nextRotation = QuarterRotation.FRTCW;
                        break;
                    case "B2":
                        myRotation = QuarterRotation.BCKCW;
                        nextRotation = QuarterRotation.BCKCW;
                        break;
                    case "L2":
                        myRotation = QuarterRotation.LFTCW;
                        nextRotation = QuarterRotation.LFTCW;
                        break;
                    case "R2":
                        myRotation = QuarterRotation.RGTCW;
                        nextRotation = QuarterRotation.RGTCW;
                        break;
                }
                txtCurrent.Text = Solutionmoves[SolutionIndex];
            }
        }

        private void SaveFile()
        {
            //Write the data to the File
            StreamWriter outfile = null;
            try
            {
                outfile = new StreamWriter(CubeName);
                if (outfile != null)
                {
                    string faceColors = "";
                    for (int i = 0; i < Map.CubeletFaces.Count; i++)
                    {
                        faceColors += Map.CubeletFaces[i].FaceColorNumber.ToString();
                    }
                    //Schrijf de Map CubeletFaces weg
                    outfile.WriteLine(faceColors);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot write file to disk. Original error: " + ex.Message);
            }
            finally
            {
                if (outfile != null)
                {
                    outfile.Close();
                }
            }
        }

        #endregion

    }

    public enum QuarterRotation
    {
        NONE = 0,
        UPCW = 1,
        UPCCW = 2,
        DWNCW = 3,
        DWNCCW = 4,
        LFTCW = 5,
        LFTCCW = 6,
        RGTCW = 7,
        RGTCCW = 8,
        FRTCCW = 9,
        FRTCW = 10,
        BCKCW = 11,
        BCKCCW = 12,
    }
}
