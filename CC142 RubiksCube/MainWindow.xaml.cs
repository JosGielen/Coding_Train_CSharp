using JG_GL;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;

namespace RubiksCube
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate(int t);
        private bool App_Initialized = false;
        private Cube3D Cube;   //A Cube3D contains 27 Cubelets
        private CubeMap Map;   //A CubeMap contains 54 CubeletFaces
        private QuarterRotation myRotation;
        private bool ReverseMoves;
        private double My_RotationSpeed = 2;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            App_Initialized = true;
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
            Scene1.Camera.Position = new Vector3D(70, 70, 100);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!App_Initialized) { return; }
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
            if (Cube.Rotation == QuarterRotation.NONE)
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

        #region "EventHandlers"

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
                case Key.Escape:
                    Init();
                    break;
                case Key.F1:
                    ReverseMoves = true;
                    break;

            }
            if (myRotation != QuarterRotation.NONE) { Cube.Scrable.Add(myRotation); }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }


        private void Normalize(List<double> list)
        {
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

        private void Wait(int waitTime)
        {
            Thread.Sleep(waitTime);
        }
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
