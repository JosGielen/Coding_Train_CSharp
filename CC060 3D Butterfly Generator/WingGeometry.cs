using JG_GL;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Butterfly_Generator
{
    internal class WingGeometry : GLGeometry
    {
        private double my_Size;

        public WingGeometry(double size) 
        {
            my_Size = size;
            my_VertexCount = 100;
        }

        protected override void CreateVertices()
        {
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            my_vertices = new Vector3D[my_VertexCount];
            int count = 0;
            double CellSize = my_Size / 10;
            for (int Z = 0; Z < 10; Z++)
            {
                for (int X = 0; X < 10; X++)
                {
                    my_vertices[count] = new Vector3D(CellSize * X, 0, CellSize * Z);
                    count += 1;
                }
            }

            //Apply the initial rotation
            for (int I = 0; I < my_vertices.Length; I++)
            {
                my_vertices[I] = rm.Transform(my_vertices[I]);
            }
        }

        protected override void CreateNormals()
        {
            my_normals = new Vector3D[my_VertexCount];
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            //Calculate the normals for each vertex position
            for (int I = 0; I < my_normals.Length; I++)
            {
                my_normals[I] = rm.Transform(new Vector3D(0, 1, 0));
            }
        }

        protected override void CreateIndices()
        {
            int count = 0;
            int K1;
            int K2;
            my_indices = new int[486];
            //Calculate the Indices for each cell
            for (int I = 0; I < 10 - 1; I++)
            {
                K1 = I * 10;
                K2 = (I + 1) * 10;
                for (int J = 0; J < 10 - 1; J++)
                {
                    my_indices[count] = K1;
                    count += 1;
                    my_indices[count] = K2;
                    count += 1;
                    my_indices[count] = K1 + 1;
                    count += 1;
                    my_indices[count] = K1 + 1;
                    count += 1;
                    my_indices[count] = K2;
                    count += 1;
                    my_indices[count] = K2 + 1;
                    count += 1;
                    K1++;
                    K2++;
                }
            }
        }

        protected override void CreateTexCoordinates()
        {
            my_textureCoords = new Vector[my_VertexCount];
            int count = 0;
            //Calculate the Texture Coordinates for each vertex position
            for (int Z = 0; Z < 10; Z++)
            {
                for (int X = 0; X < 10; X++)
                {
                    my_textureCoords[count] = new Vector(X / 9.0, Z / 9.0);
                    count += 1;
                }
            }
        }

        public override Vector3D GetVertexLayout()
        {
            return new Vector3D(my_Size,0,my_Size);
        }
    }
}
