using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace _MandelBulb
{
    class MandelBulbGeometry : GLGeometry
    {
        private double my_Size;
        private double my_StepSize;
        private readonly int N;
        private readonly int my_MaxIter;
        private readonly double my_IsoLevel;
        private readonly double[,,] my_Values;
        private readonly List<Vector3D> LstVertices = new List<Vector3D>();
        private readonly List<Vector3D> LstNormals = new List<Vector3D>();

        /// <summary>
        /// Creates a 3D Mandelbulb.
        /// </summary>
        /// <param name="size">The relative size of the MandelBulb e.g. in Pixels</param>
        /// <param name="stepSize">The distance between individual calculated points of the MandelBulb.</param>
        /// <param name="power">The power factor in the equation Z = Z^N + C</param>
        /// <param name="MaxIteration">The number of times the equation is iterated.</param>
        /// <param name="Isolevel">The value used to define the MandelBulb surface.</param>
        public MandelBulbGeometry(double size, double stepSize, int power, int maxIteration, double isolevel)
        {
            my_Size = size;
            my_StepSize = stepSize;
            N = power;
            my_MaxIter = maxIteration;
            my_IsoLevel = isolevel;
            my_Values = new double[(int)(my_Size / my_StepSize) + 1, (int)(my_Size / my_StepSize) + 1, (int)(my_Size / my_StepSize) + 1];
        }

        #region "Properties"

        public double Size
        {
            get { return my_Size; }
            set { my_Size = value; }
        }

        public double StepSize
        {
            get { return my_StepSize; }
            set { my_StepSize = value; }
        }

        public int Power
        {
            get { return N; }
        }

        public int MaxIteration
        {
            get { return my_MaxIter; }
        }

        #endregion 

        public override void CreateVertices()
        {
            //STEP1: Create a Mandelbulb scalar field
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            int Iteration;
            double r, theta, phi;
            Vector3D Zeta;
            Vector3D C;
            double dist;
            for (double X = -my_Size / 2; X <= my_Size / 2; X += my_StepSize)
            {
                for (double Y = -my_Size / 2; Y <= my_Size / 2; Y += my_StepSize)
                {
                    for (double Z = -my_Size / 2; Z <= my_Size / 2; Z += my_StepSize)
                    {
                        //Calculate the MandelBulb between -1.1 and 1.1 to prevent very large values
                        Zeta = new Vector3D(0, 0, 0);
                        C = new Vector3D()
                        {
                            X = 2.2 * X / my_Size,
                            Y = 2.2 * Y / my_Size,
                            Z = 2.2 * Z / my_Size
                        };
                        Iteration = 0;
                        do
                        {
                            //convert Zeta to Spherical coordinates
                            r = Math.Sqrt(Zeta.X * Zeta.X + Zeta.Y * Zeta.Y + Zeta.Z * Zeta.Z);
                            theta = Math.Atan2(Math.Sqrt(Zeta.X * Zeta.X + Zeta.Y * Zeta.Y), Zeta.Z);
                            phi = Math.Atan2(Zeta.Y, Zeta.X);
                            //Calculate Zeta^N
                            Zeta.X = Math.Pow(r, N) * Math.Sin(theta * N) * Math.Cos(phi * N);
                            Zeta.Y = Math.Pow(r, N) * Math.Sin(theta * N) * Math.Sin(phi * N);
                            Zeta.Z = Math.Pow(r, N) * Math.Cos(theta * N);
                            Zeta += C;
                            dist = Math.Sqrt(Zeta.X * Zeta.X + Zeta.Y * Zeta.Y + Zeta.Z * Zeta.Z);
                            if (dist > my_IsoLevel) break;
                            Iteration++;
                        } while (Iteration <= my_MaxIter);
                        my_Values[(int)((X + my_Size / 2) / my_StepSize), (int)((Y + my_Size / 2) / my_StepSize), (int)((Z + my_Size / 2) / my_StepSize)] = dist;
                    }
                }
            }
            //STEP2: Use Marching cubes to determine the MandelBulb surface
            Vector3D[] Corners = new Vector3D[8];
            double[] Values = new double[8];
            MarchingCube cube;
            int Xindex, Yindex, Zindex;
            for (double X = -my_Size / 2; X < my_Size / 2; X += my_StepSize)
            {
                for (double Y = -my_Size / 2; Y < my_Size / 2; Y += my_StepSize)
                {
                    for (double Z = -my_Size / 2; Z < my_Size / 2; Z += my_StepSize)
                    {
                        Corners[0] = new Vector3D(X, Y + my_StepSize, Z);
                        Corners[1] = new Vector3D(X, Y + my_StepSize, Z + my_StepSize);
                        Corners[2] = new Vector3D(X + my_StepSize, Y + my_StepSize, Z + my_StepSize);
                        Corners[3] = new Vector3D(X + my_StepSize, Y + my_StepSize, Z);
                        Corners[4] = new Vector3D(X, Y, Z);
                        Corners[5] = new Vector3D(X, Y, Z + my_StepSize);
                        Corners[6] = new Vector3D(X + my_StepSize, Y, Z + my_StepSize);
                        Corners[7] = new Vector3D(X + my_StepSize, Y, Z);
                        Xindex = (int)((X + my_Size / 2) / my_StepSize);
                        Yindex = (int)((Y + my_Size / 2) / my_StepSize);
                        Zindex = (int)((Z + my_Size / 2) / my_StepSize);
                        Values[0] = my_Values[Xindex, Yindex + 1, Zindex];
                        Values[1] = my_Values[Xindex, Yindex + 1, Zindex + 1];
                        Values[2] = my_Values[Xindex + 1, Yindex + 1, Zindex + 1];
                        Values[3] = my_Values[Xindex + 1, Yindex + 1, Zindex];
                        Values[4] = my_Values[Xindex, Yindex, Zindex];
                        Values[5] = my_Values[Xindex, Yindex, Zindex + 1];
                        Values[6] = my_Values[Xindex + 1, Yindex, Zindex + 1];
                        Values[7] = my_Values[Xindex + 1, Yindex, Zindex];
                        cube = new MarchingCube(Corners, Values, my_IsoLevel);
                        for (int I=0; I < cube.Vertices.Length; I++)
                        {
                            LstVertices.Add(cube.Vertices[I]);
                            LstNormals.Add(cube.Normals[I]);
                        }
                    }
                }
            }
            //STEP3: Get the total number of Vertices
            my_VertexCount = LstVertices.Count;
            my_Vertices = new Vector3D[my_VertexCount];
            my_Vertices = LstVertices.ToArray();
            //Apply the initial rotation
            for (int I = 0; I < my_VertexCount; I++)
            {
                my_Vertices[I] = rm.Transform(my_Vertices[I]);
            }
        }

        public override void CreateNormals()
        {
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            my_Normals = new Vector3D[my_VertexCount];
            my_Normals = LstNormals.ToArray();
            //Apply the initial rotation
            for (int I = 0; I < my_Normals.Length; I++)
            {
                my_Normals[I] = rm.Transform(my_Normals[I]);
            }
        }

        public override void CreateIndices()
        {
            int indexCount = my_VertexCount;
            my_Indices = new int[indexCount];
            for (int I = 0; I < indexCount; I++)
            {
                my_Indices[I] = I;
            }
        }

        public override void CreateTextureCoordinates()
        {
            my_TextureCoords = new Vector[my_VertexCount];
            double dist;
            double min = double.MaxValue;
            double max = double.MinValue;
            //X Texture coordinate = fixed as 0.5
            //Y Texture coordinate depends on the distance to (0,0,0)
            for (int I = 0; I < my_TextureCoords.Length; I++)
            {
                dist = Math.Sqrt(my_Vertices[I].X * my_Vertices[I].X + my_Vertices[I].Y * my_Vertices[I].Y + my_Vertices[I].Z * my_Vertices[I].Z);
                if (dist < min) { min = dist; }
                if (dist > max) { max = dist; }
            }
            for (int I = 0; I < my_TextureCoords.Length; I++)
            {
                dist = Math.Sqrt(my_Vertices[I].X * my_Vertices[I].X + my_Vertices[I].Y * my_Vertices[I].Y + my_Vertices[I].Z * my_Vertices[I].Z);
                my_TextureCoords[I] = new Vector(0.5, (dist - min) / (max - min));
            }
        }

        /// <summary>
        /// Not needed for this MandelBulb program
        /// </summary>
        public override Vector3D GetVertexLayout()
        {
            return new Vector3D(0, 0, 0);
        }
    }
}
