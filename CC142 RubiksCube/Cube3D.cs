using JG_GL;
using System.Windows.Media.Media3D;

namespace RubiksCube
{
    internal class Cube3D
    {
        private GLScene my_Scene;
        private CubeMap my_Map;
        private List<CubeletGeometry> my_Cubelets;
        private readonly double CubeletSize;
        private readonly double CubeletSpacing = 1.5;
        private QuarterRotation my_Rotation;
        private double my_RotationSpeed;
        private List<QuarterRotation> my_Scrable;
        private List<CubeletGeometry> SelectedCubelets;
        private double my_TotalAngle;

        public Cube3D(GLScene scene, CubeMap map)
        {
            CubeletGeometry cubie;
            Vector3D pos;
            my_Scene = scene;
            my_Map = map;
            my_Cubelets = new List<CubeletGeometry>();
            my_Scrable = new List<QuarterRotation>();
            my_RotationSpeed = 0.1;
            my_TotalAngle = 0.0;
            CubeletSize = scene.ActualWidth / 25;
            int[] CubeletFaceNumbers = new int[3];
            //The FaceNumbers link the CubeletFaces to the X (or -X), Y (or -Y) and Z (or -Z) face of each Cubelet
            //Each Cubelet has 1, 2 or 3 CubeletFaces (-1 = hidden face);
            int[] FaceNumbers = {
                42, 33, 53, 43, 30, -1, 44, 27, 24, 39, -1, 50, 40, -1, -1, 41, -1,
                21, 36, 0, 47, 37, 3, -1, 38, 6, 18, -1, 34, 52, -1, 31, -1,
                -1, 28, 25, -1, -1, 49, -1, -1, -1, -1, -1, 22, -1, 1, 46, -1, 4,
                -1, -1, 7, 19, 17, 35, 51, 16, 32, -1, 15, 29, 26, 14, -1,
                48, 13, -1, -1, 12, -1, 23, 11, 2, 45, 10, 5, -1, 9, 8, 20};
            int counter = 0;
            //Make the Cubelets
            for (int X = -1; X <= 1; X++)
            {
                for (int Y = -1; Y <= 1; Y++)
                {
                    for (int Z = -1; Z <= 1; Z++)
                    {
                        pos = new Vector3D(X, Y, Z);
                        CubeletFaceNumbers[0] = FaceNumbers[3 * counter];
                        CubeletFaceNumbers[1] = FaceNumbers[3 * counter + 1];
                        CubeletFaceNumbers[2] = FaceNumbers[3 * counter + 2];
                        cubie = new CubeletGeometry(pos, CubeletSize, CubeletSpacing, CubeletFaceNumbers);
                        cubie.SetColors(map.CubeletFaces);
                        my_Scene.AddGeometry(cubie);
                        my_Cubelets.Add(cubie);
                        counter += 1;
                    }
                }
            }
        }

        #region "Parameters"

        public double RotationSpeed
        {
            get { return my_RotationSpeed; }
            set { my_RotationSpeed = value; }
        }

        public QuarterRotation Rotation
        {
            get { return my_Rotation; }
            set { my_Rotation = value; }
        }

        public List<CubeletGeometry> Cubelets
        {
            get { return my_Cubelets; }
        }


        public List<QuarterRotation> Scrable
        {
            get { return my_Scrable; }
        }

        #endregion

        #region "Rotation"

        public void Rotate(QuarterRotation rot)
        {
            my_Rotation = rot;
            switch (my_Rotation)
            {
                case QuarterRotation.UPCW:
                    RotateUp(true);
                    break;
                case QuarterRotation.UPCCW:
                    RotateUp(false);
                    break;
                case QuarterRotation.DWNCW:
                    RotateDown(true);
                    break;
                case QuarterRotation.DWNCCW:
                    RotateDown(false);
                    break;
                case QuarterRotation.LFTCW:
                    RotateLeft(true);
                    break;
                case QuarterRotation.LFTCCW:
                    RotateLeft(false);
                    break;
                case QuarterRotation.RGTCW:
                    RotateRight(true);
                    break;
                case QuarterRotation.RGTCCW:
                    RotateRight(false);
                    break;
                case QuarterRotation.FRTCW:
                    RotateFront(true);
                    break;
                case QuarterRotation.FRTCCW:
                    RotateFront(false);
                    break;
                case QuarterRotation.BCKCW:
                    RotateBack(true);
                    break;
                case QuarterRotation.BCKCCW:
                    RotateBack(false);
                    break;
            }
        }

        private void RotateUp(bool ClockWise)
        {
            double angle;
            SelectedCubelets = new List<CubeletGeometry>();
            //Select the cubelets at the top
            for (int I = 0; I < my_Cubelets.Count(); I++)
            {
                if (my_Cubelets[I].CubeletPosition.Y == 1)
                {
                    SelectedCubelets.Add(my_Cubelets[I]);
                }
            }
            //Rotate the selected cubelets around the cube Y-axis and around their own Y-axis
            AxisAngleRotation3D rot;
            RotateTransform3D rotT;
            if (ClockWise)
            {
                my_TotalAngle += my_RotationSpeed;
                angle = -1 * my_RotationSpeed;
            }
            else
            {
                my_TotalAngle -= my_RotationSpeed;
                angle = my_RotationSpeed;
            }
            rot = new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle);
            rotT = new RotateTransform3D(rot, new Point3D(0, CubeletSize + CubeletSpacing, 0));
            for (int I = 0; I < SelectedCubelets.Count(); I++)
            {
                SelectedCubelets[I].Position = rotT.Transform(SelectedCubelets[I].Position);
                //SelectedCubelets[I].Rotation = new Vector3D(0, my_TotalAngle, 0);
                SelectedCubelets[I].RotationAxis = new Vector3D(0, 1, 0);
                SelectedCubelets[I].RotationAngle = my_TotalAngle * Math.PI / 180;
            }
            //When the rotation is finished
            if (Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001)
            {
                my_Rotation = QuarterRotation.NONE;
                my_TotalAngle = 0.0;
                //Update the 2D CubeletFaces colors
                my_Map.RotateUP2D(ClockWise);
                //Reset the selected cubelets position and update the colors
                for (int I = 0; I < SelectedCubelets.Count(); I++)
                {
                    SelectedCubelets[I].ResetPosition();
                    SelectedCubelets[I].SetColors(my_Map.CubeletFaces);
                    SelectedCubelets[I].GenerateGeometry(my_Scene);
                }
            }
        }

        private void RotateDown(bool ClockWise)
        {
            double angle;
            SelectedCubelets = new List<CubeletGeometry>();
            //Select the cubelets at the bottom
            for (int I = 0; I < my_Cubelets.Count(); I++)
            {
                if (my_Cubelets[I].CubeletPosition.Y == -1)
                {
                    SelectedCubelets.Add(my_Cubelets[I]);
                }
            }
            //Rotate the selected cubelets around the cube Y-axis and around their own Y-axis
            AxisAngleRotation3D rot;
            RotateTransform3D rotT;
            if (ClockWise)
            {
                my_TotalAngle -= my_RotationSpeed;
                angle = my_RotationSpeed;
            }
            else
            {
                my_TotalAngle += my_RotationSpeed;
                angle = -1 * my_RotationSpeed;
            }
            rot = new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle);
            rotT = new RotateTransform3D(rot, new Point3D(0, -1 * (CubeletSize + CubeletSpacing), 0));
            for (int I = 0; I < SelectedCubelets.Count(); I++)
            {
                SelectedCubelets[I].Position = rotT.Transform(SelectedCubelets[I].Position);
                //SelectedCubelets[I].Rotation = new Vector3D(0, my_TotalAngle, 0);
                SelectedCubelets[I].RotationAxis = new Vector3D(0, 1, 0);
                SelectedCubelets[I].RotationAngle = my_TotalAngle * Math.PI / 180;

            }
            //When the rotation is done
            if (Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001)
            {
                my_Rotation = QuarterRotation.NONE;
                my_TotalAngle = 0.0;
                //Update the 2D CubeletFaces colors
                my_Map.RotateDOWN2D(ClockWise);
                //Reset the selected cubelets position and update the colors
                for (int I = 0; I < SelectedCubelets.Count(); I++)
                {
                    SelectedCubelets[I].ResetPosition();
                    SelectedCubelets[I].SetColors(my_Map.CubeletFaces);
                    SelectedCubelets[I].GenerateGeometry(my_Scene);
                }
            }
        }

        private void RotateLeft(bool ClockWise)
        {
            double angle;
            SelectedCubelets = new List<CubeletGeometry>();
            //Select the cubelets at the left side
            for (int I = 0; I < my_Cubelets.Count(); I++)
            {
                if (my_Cubelets[I].CubeletPosition.X == -1)
                {
                    SelectedCubelets.Add(my_Cubelets[I]);
                }
            }
            //Rotate the selected cubelets around the cube X-axis and around their own X-axis
            AxisAngleRotation3D rot;
            RotateTransform3D rotT;
            if (ClockWise)
            {
                my_TotalAngle -= my_RotationSpeed;
                angle = my_RotationSpeed;
            }
            else
            {
                my_TotalAngle += my_RotationSpeed;
                angle = -1 * my_RotationSpeed;
            }
            rot = new AxisAngleRotation3D(new Vector3D(1, 0, 0), angle);
            rotT = new RotateTransform3D(rot, new Point3D(-1 * (CubeletSize + CubeletSpacing), 0, 0));
            for (int I = 0; I < SelectedCubelets.Count(); I++)
            {
                SelectedCubelets[I].Position = rotT.Transform(SelectedCubelets[I].Position);
                //SelectedCubelets[I].Rotation = new Vector3D(my_TotalAngle, 0, 0);
                SelectedCubelets[I].RotationAxis = new Vector3D(1, 0, 0);
                SelectedCubelets[I].RotationAngle = my_TotalAngle * Math.PI / 180;

            }
            //When the rotation is done
            if (Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001)
            {
                my_Rotation = QuarterRotation.NONE;
                my_TotalAngle = 0.0;
                //Update the 2D CubeletFaces colors
                my_Map.RotateLEFT2D(ClockWise);
                //Reset the selected cubelets position and update the colors
                for (int I = 0; I < SelectedCubelets.Count(); I++)
                {
                    SelectedCubelets[I].ResetPosition();
                    SelectedCubelets[I].SetColors(my_Map.CubeletFaces);
                    SelectedCubelets[I].GenerateGeometry(my_Scene);
                }
            }
        }

        private void RotateRight(bool ClockWise)
        {
            double angle;
            SelectedCubelets = new List<CubeletGeometry>();
            //Select the cubelets at the left side
            for (int I = 0; I < my_Cubelets.Count(); I++)
            {
                if (my_Cubelets[I].CubeletPosition.X == 1)
                {
                    SelectedCubelets.Add(my_Cubelets[I]);
                }
            }
            //Rotate the selected cubelets around the cube X-axis and around their own X-axis
            AxisAngleRotation3D rot;
            RotateTransform3D rotT;
            if (ClockWise)
            {
                my_TotalAngle += my_RotationSpeed;
                angle = -1 * my_RotationSpeed;
            }
            else
            {
                my_TotalAngle -= my_RotationSpeed;
                angle = my_RotationSpeed;
            }
            rot = new AxisAngleRotation3D(new Vector3D(1, 0, 0), angle);
            rotT = new RotateTransform3D(rot, new Point3D(CubeletSize + CubeletSpacing, 0, 0));
            for (int I = 0; I < SelectedCubelets.Count(); I++)
            {
                SelectedCubelets[I].Position = rotT.Transform(SelectedCubelets[I].Position);
                //SelectedCubelets[I].Rotation = new Vector3D(my_TotalAngle, 0, 0);
                SelectedCubelets[I].RotationAxis = new Vector3D(1, 0, 0);
                SelectedCubelets[I].RotationAngle = my_TotalAngle * Math.PI / 180;

            }
            //When the rotation is done
            if (Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001)
            {
                my_Rotation = QuarterRotation.NONE;
                my_TotalAngle = 0.0;
                //Update the 2D CubeletFaces colors
                my_Map.RotateRIGHT2D(ClockWise);
                //Reset the selected cubelets position and update the colors
                for (int I = 0; I < SelectedCubelets.Count(); I++)
                {
                    SelectedCubelets[I].ResetPosition();
                    SelectedCubelets[I].SetColors(my_Map.CubeletFaces);
                    SelectedCubelets[I].GenerateGeometry(my_Scene);
                }
            }
        }

        private void RotateFront(bool ClockWise)
        {
            double angle;
            SelectedCubelets = new List<CubeletGeometry>();
            //Select the cubelets at the left side
            for (int I = 0; I < my_Cubelets.Count; I++)
            {
                if (my_Cubelets[I].CubeletPosition.Z == 1)
                {
                    SelectedCubelets.Add(my_Cubelets[I]);
                }
            }
            //Rotate the selected cubelets around the cube Z-axis and around their own Z-axis
            AxisAngleRotation3D rot;
            RotateTransform3D rotT;
            if (ClockWise)
            {
                my_TotalAngle += my_RotationSpeed;
                angle = -1 * my_RotationSpeed;
            }
            else
            {
                my_TotalAngle -= my_RotationSpeed;
                angle = my_RotationSpeed;
            }
            rot = new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle);
            rotT = new RotateTransform3D(rot, new Point3D(0, 0, CubeletSize + CubeletSpacing));
            for (int I = 0; I < SelectedCubelets.Count(); I++)
            {
                SelectedCubelets[I].Position = rotT.Transform(SelectedCubelets[I].Position);
                //SelectedCubelets[I].Rotation = new Vector3D(0, 0, my_TotalAngle);
                SelectedCubelets[I].RotationAxis = new Vector3D(0, 0, 1);
                SelectedCubelets[I].RotationAngle = my_TotalAngle * Math.PI / 180;

            }
            //When the rotation is done
            if (Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001)
            {
                my_Rotation = QuarterRotation.NONE;
                my_TotalAngle = 0.0;
                //Update the 2D CubeletFaces colors
                my_Map.RotateFRONT2D(ClockWise);
                //Reset the selected cubelets position and update the colors
                for (int I = 0; I < SelectedCubelets.Count(); I++)
                {
                    SelectedCubelets[I].ResetPosition();
                    SelectedCubelets[I].SetColors(my_Map.CubeletFaces);
                    SelectedCubelets[I].GenerateGeometry(my_Scene);
                }
                return;
            }
        }

        private void RotateBack(bool ClockWise)
        {
            double angle;
            SelectedCubelets = new List<CubeletGeometry>();
            //Select the cubelets at the left side
            for (int I = 0; I < my_Cubelets.Count(); I++)
            {
                if (my_Cubelets[I].CubeletPosition.Z == -1)
                {
                    SelectedCubelets.Add(my_Cubelets[I]);
                }
            }
            //Rotate the selected cubelets around the cube Z-axis and around their own Z-axis
            AxisAngleRotation3D rot;
            RotateTransform3D rotT;
            if (ClockWise)
            {
                my_TotalAngle -= my_RotationSpeed;
                angle = my_RotationSpeed;
            }
            else
            {
                my_TotalAngle += my_RotationSpeed;
                angle = -1 * my_RotationSpeed;
            }
            rot = new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle);
            rotT = new RotateTransform3D(rot, new Point3D(0, 0, -1 * (CubeletSize + CubeletSpacing)));
            for (int I = 0; I < SelectedCubelets.Count(); I++)
            {
                SelectedCubelets[I].Position = rotT.Transform(SelectedCubelets[I].Position);
                //SelectedCubelets[I].Rotation = new Vector3D(0, 0, my_TotalAngle);
                SelectedCubelets[I].RotationAxis = new Vector3D(0, 0, 1);
                SelectedCubelets[I].RotationAngle = my_TotalAngle * Math.PI / 180;
            }
            //When the rotation is done
            if (Math.Abs(Math.Abs(my_TotalAngle) - 90) < 0.001)
            {
                my_Rotation = QuarterRotation.NONE;
                my_TotalAngle = 0.0;
                //Update the 2D CubeletFaces colors
                my_Map.RotateBACK2D(ClockWise);
                //Reset the selected cubelets position and update the colors
                for (int I = 0; I < SelectedCubelets.Count(); I++)
                {
                    SelectedCubelets[I].ResetPosition();
                    SelectedCubelets[I].SetColors(my_Map.CubeletFaces);
                    SelectedCubelets[I].GenerateGeometry(my_Scene);
                }
                return;
            }
        }

        #endregion

        public void UpdateColors()
        {
            for (int I = 0; I < my_Cubelets.Count(); I++)
            {
                my_Cubelets[I].SetColors(my_Map.CubeletFaces);
                my_Cubelets[I].GenerateGeometry(my_Scene);
            }
        }

        public QuarterRotation GetLastRotation()
        {
            QuarterRotation result = QuarterRotation.NONE;
            if (my_Scrable.Count > 0)
            {
                result = Scrable.Last();
                my_Scrable.RemoveAt(my_Scrable.Count - 1);
            }
            return result;
        }
    }
}
