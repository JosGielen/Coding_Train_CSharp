using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace _3D_Perlin_Terrain
{
    class MeshGeometry : GLGeometry 
    {
        private int my_X_Size;
        private int my_Z_Size;
        private double my_CellSize;
        private double[,] my_Heights;


        public MeshGeometry()
        {
            my_X_Size = 100;
            my_Z_Size = 100;
            my_CellSize = 1.0;
            my_Heights = new double[100, 100];
            my_VertexCount = 10000;        
        }

        public MeshGeometry(int X_Size, int Z_Size, double cellSize)
        {
            my_X_Size = X_Size;
            my_Z_Size = Z_Size;
            my_CellSize = cellSize;
            my_Heights = new double[X_Size, Z_Size];
            my_VertexCount = X_Size * Z_Size;
        }

        #region "Properties"

        public int X_Size
        {
            get { return my_X_Size; }
            set { my_X_Size = value; }
        }

        public int Z_Size
        {
            get { return my_Z_Size; }
            set { my_Z_Size = value; }
        }

        public double CellSize
        {
            get { return my_CellSize; }
            set { my_CellSize = value; }
        }

        public double[,] Heights
        {
            get { return my_Heights; }
            set { my_Heights = value; }
        }

        #endregion 

        public override void CreateVertices()
        {
            my_VertexCount = my_X_Size * my_Z_Size;
            my_Vertices = new Vector3D[my_VertexCount];
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            //Calculate the vertex positions
            int count = 0;
            for (int X = 0; X < my_X_Size; X++)
            {
                for (int Z = 0; Z < my_Z_Size; Z++)
                {
                    my_Vertices[count] = new Vector3D(my_CellSize * (X - (my_X_Size - 1) / 2), my_Heights[X, Z], my_CellSize * (Z - (my_Z_Size - 1) / 2));
                    count++;
                }
            }
            //Apply the initial rotation
            for (int I = 0; I < my_Vertices.Length; I++)
            {
                my_Vertices[I] = rm.Transform(my_Vertices[I]);
            }
        }

        public override void CreateNormals()
        {
            my_Normals = new Vector3D[my_VertexCount];
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            //Calculate the vertex positions
            int count = 0;
            Vector3D V1;
            Vector3D V2;
            for (int X = 0; X < my_X_Size; X++)
            {
                for (int Z = 0; Z < my_Z_Size; Z++)
                {
                    if (X < my_X_Size - 1 & Z < my_Z_Size - 1)
                    {
                        V1 = my_Vertices[(X + 1) * my_Z_Size + Z] - my_Vertices[X * my_Z_Size + Z];
                        V2 = my_Vertices[X * my_Z_Size + Z + 1] - my_Vertices[X * my_Z_Size + Z];
                        my_Normals[count] = Vector3D.CrossProduct(V2, V1);
                    }
                    else if (X == my_X_Size - 1 )
                    {
                        my_Normals[count] = my_Normals[count - my_Z_Size + 1];
                    }
                    else if (Z == my_Z_Size - 1)
                    {
                        my_Normals[count] = my_Normals[count - 1];
                    }
                    count++;
                }
            }
            //Apply the initial rotation
            for (int I = 0; I < my_Normals.Length; I++)
            {
                my_Normals[I] = rm.Transform(my_Normals[I]);
            }
        }

        public override void CreateIndices()
        {
            int indexCount = 6 * (my_X_Size - 1) * (my_Z_Size - 1);
            my_Indices = new int[indexCount];
            int count = 0;
            int K1;
            int K2;
            //Triangles = (K1, K+1, K2) - (K2, K1+1, K2+1)
            for (int I = 0; I < my_X_Size - 1; I++)
            {
                K1 = I * my_Z_Size;
                K2 = (I + 1) * my_Z_Size;
                for (int J = 1; J < my_Z_Size - 1; J++)
                {
                    my_Indices[count] = K1;
                    count += 1;
                    my_Indices[count] = K2;
                    count += 1;
                    my_Indices[count] = K1 + 1;
                    count += 1;
                    my_Indices[count] = K1 + 1;
                    count += 1;
                    my_Indices[count] = K2;
                    count += 1;
                    my_Indices[count] = K2 + 1;
                    count += 1;
                    K1 += 1;
                    K2 += 1;
                }
            }
        }

        public override void CreateTextureCoordinates()
        {
            my_TextureCoords = new Vector[my_VertexCount];
            int count = 0;
            double maxHeight = double.MinValue;
            //Calculate the maximum height
            for (int I = 0; I < my_TextureCoords.Length; I++)
            {
                if (my_Vertices[I].Y > maxHeight) { maxHeight = my_Vertices[I].Y; }
            }
            if (maxHeight == 0) { maxHeight = 1; }
            for (int X = 0; X < my_X_Size; X++)
            {
                for (int Z = 0; Z < my_Z_Size; Z++)
                {
                    my_TextureCoords[count] = new Vector((X + Z) / (my_X_Size + my_Z_Size - 2), my_Heights[X, Z] / maxHeight);
                    count++;
                }
            }
        }

        public void SetVertexHeight(int X, int Z, double height)
        {
            my_Heights[X, Z] = height;
        }

        /// <summary>
        /// X = number of points along X-axis, Y = 1, Z = number of points along Z-axis.
        /// </summary>
        /// <returns></returns>
        public override Vector3D GetVertexLayout()
        {
            return new Vector3D(my_X_Size, 1, my_Z_Size);
        }
    }
}
