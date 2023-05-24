using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace _MandelBulb
{
    class MandelBulbGeometry2 : GLGeometry 
    {
        private double my_Size;
        private double my_StepSize;
        private int N;
        private int my_MaxIter;


        public MandelBulbGeometry2(double size, double stepSize, int power, int MaxIteration)
        {
            my_Size = size;
            my_StepSize = stepSize;
            N = power;
            my_MaxIter = MaxIteration; 
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

        #endregion 

        public override void CreateVertices()
        {
            //Create a Mandelbulb pointcloud
            my_VertexCount = (int)Math.Pow (my_Size / my_StepSize + 1,3);
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            my_Vertices = new Vector3D[my_VertexCount];
            int count = 0;
            bool atEdge;
            for (double X = -my_Size/2; X <= my_Size/2; X+= my_StepSize  )
            {
                for (double Z = -my_Size / 2; Z <= my_Size / 2; Z += my_StepSize)
                {
                    atEdge = false;
                    for (double Y = -my_Size / 2; Y <= 0; Y += my_StepSize)
                    {
                        if (IsMandelBulb(X,Y,Z))
                        {
                            atEdge = true;
                            my_Vertices[count] = new Vector3D(X, Y, Z);
                            count += 1;
                            break;
                        }
                        else
                        {
                            if (atEdge)
                            {
                                break;
                            }
                        }
                    }
                    atEdge = false;
                    for (double Y = my_Size / 2; Y >= 0 / 2; Y -= my_StepSize)
                    {
                        if (IsMandelBulb(X, Y, Z))
                        {
                            atEdge = true;
                            my_Vertices[count] = new Vector3D(X, Y, Z);
                            count += 1;
                            break;
                        }
                        else
                        {
                            if (atEdge)
                            {
                                break;
                            }
                        }
                    }
                }
            }
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
            Vector3D center = new Vector3D(0, 0, 0);
            for (int I = 0; I < my_VertexCount; I++)
            {
                my_Normals[I] = my_Vertices[I] - center;
            } 
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

            for (int I = 0; I < indexCount; I++ )
            {
                my_Indices[I] = I;
            }
        }

        public override void CreateTextureCoordinates() //Should be OK
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

        //Check if a point is in the MandelBulb
        private bool IsMandelBulb(double X, double Y, double Z)
        {
            int Iteration;
            double r, theta, phi;
            Vector3D Zeta;
            Vector3D C;
            bool InMandelBulb;
            //Formula: Zeta = Zeta^N + C
            Zeta = new Vector3D(0, 0, 0);
            C = new Vector3D()
            {
                X = 2 * X / my_Size,
                Y = 2 * Y / my_Size,
                Z = 2 * Z / my_Size
            };
            //Check if (X,Y,Z) is part of the Mandelbulb
            Iteration = 0;
            InMandelBulb = true;
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
                Iteration++;
                if (Math.Sqrt(Zeta.X * Zeta.X + Zeta.Y * Zeta.Y + Zeta.Z * Zeta.Z) > 4)
                {
                    InMandelBulb = false;
                    break;
                }
            } while (Iteration <= my_MaxIter);
            return InMandelBulb;
        }
    }
}
