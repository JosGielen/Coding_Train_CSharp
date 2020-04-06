using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace _3D_Knots
{
    //x = cos(mu) * (1 + cos(A1 * mu / A2) / 2.0);
    //y = sin(mu) * (1 + cos(B1 * mu / B2) / 2.0);
    //z = sin(C1 * mu / C2) / 2.0;

    //A1 = 4   B1 = 4    C1 = 4 
    //A2 = 3   B2 = 3    C2 = 3
    //Or
    //A1 = 7   B1 = 7    C1 = 7 
    //A2 = 4   B2 = 4    C2 = 4
    //Or
    //A1 = 11  B1 = 11   C1 = 11 
    //A2 = 6   B2 = 6    C2 = 6
    //====================================================================
    class Knot3Geometry : GLGeometry
    {
        private double my_Size;
        private int my_Steps;
        private double my_Diameter;
        private int my_Slices;
        private Vector3D[] centerPoints;
        // Knot parameters
        public double a1 = 7.0;
        public double a2 = 4.0;
        public double a3 = 0.0;
        public double a4 = 0.0;
        public double a5 = 0.0;
        public double a6 = 0.0;
        public double a7 = 0.0;
        public double a8 = 0.0;
        public double b1 = 7.0;
        public double b2 = 4.0;
        public double b3 = 0.0;
        public double b4 = 0.0;
        public double b5 = 0.0;
        public double b6 = 0.0;
        public double b7 = 0.0;
        public double b8 = 0.0;
        public double c1 = 7.0;
        public double c2 = 7.0;
        public double c3 = 0.0;
        public double c4 = 0.0;
        public double c5 = 0.0;
        public double c6 = 0.0;
        public double c7 = 0.0;
        public double c8 = 0.0;

        public Knot3Geometry(double size, int steps, double diameter, int slices)
        {
            my_Size = size;
            my_Steps = steps;
            my_Diameter = diameter;
            my_Slices = slices;
            my_VertexCount = (my_Steps + 1) * (my_Slices + 1);
        }

        #region "Properties"

        public double Size
        {
            get { return my_Size; }
            set { my_Size = value; }
        }

        public int Steps
        {
            get { return my_Steps; }
            set { my_Steps = value; }
        }

        public double Diameter
        {
            get { return my_Diameter; }
            set { my_Diameter = value; }
        }

        public int Slices
        {
            get { return my_Slices; }
            set { my_Slices = value; }
        }

        #endregion 

        public void SetParameters(Settings settingForm)
        {
            a1 = settingForm.A1;
            a2 = settingForm.A2;
            a3 = settingForm.A3;
            a4 = settingForm.A4;
            a5 = settingForm.A5;
            a6 = settingForm.A6;
            a7 = settingForm.A7;
            a8 = settingForm.A8;
            b1 = settingForm.B1;
            b2 = settingForm.B2;
            b3 = settingForm.B3;
            b4 = settingForm.B4;
            b5 = settingForm.B5;
            b6 = settingForm.B6;
            b7 = settingForm.B7;
            b8 = settingForm.B8;
            c1 = settingForm.C1;
            c2 = settingForm.C2;
            c3 = settingForm.C3;
            c4 = settingForm.C4;
            c5 = settingForm.C5;
            c6 = settingForm.C6;
            c7 = settingForm.C7;
            c8 = settingForm.C8;
        }


        public override void CreateVertices()
        {
            my_VertexCount = (my_Steps + 1) * (my_Slices + 1);
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            my_Vertices = new Vector3D[my_VertexCount];
            centerPoints = new Vector3D[my_Steps + 1];
            int count = 0;
            double mu;
            double x;
            double y;
            double z;
            Vector3D V;
            Vector3D V1 = new Vector3D();
            AxisAngleRotation3D Aar;
            RotateTransform3D rotT;
            //Calculate the knot center coordinates
            for (int I = 0; I <= my_Steps; I++)
            {
                mu = I * 2.0 * Math.PI * a2 / my_Steps;
                x = my_Size * (Math.Cos(mu) * (1 + Math.Cos(a1 * mu / a2) / 2.0));
                y = my_Size * (Math.Sin(mu) * (1 + Math.Cos(b1 * mu / b2) / 2.0));
                z = my_Size * (Math.Sin(c1 * mu / c2) / 2.0);
                centerPoints[I] = new Vector3D(x, y, z);
            }
            //Calculate the vertex positions at my_Diameter / 2 around the knot center coordinates 
            for (int I = 0; I < my_Steps; I++)
            {
                V = centerPoints[I + 1] - centerPoints[I];
                Aar = new AxisAngleRotation3D(V, 360.0 / my_Slices);
                rotT = new RotateTransform3D(Aar);
                if (V.X != 0 | V.Z != 0)
                {
                    V1 = Vector3D.CrossProduct(V, new Vector3D(0, 1, 0));
                }
                else if (V.Y != 0)
                {
                    V1 = Vector3D.CrossProduct(V, new Vector3D(0, 0, 1));
                }
                V1.Normalize();
                V1 = (my_Diameter / 2.0) * V1;
                for (int J = 0; J <= my_Slices; J++)
                {
                    my_Vertices[count] = V1 + centerPoints[I];
                    V1 = rotT.Transform(V1);
                    count += 1;
                }
            }
            //Add vertices around the first knot coordinate again to close the knot.
            V = centerPoints[1] - centerPoints[0];
            Aar = new AxisAngleRotation3D(V, 360.0 / my_Slices);
            rotT = new RotateTransform3D(Aar);
            if (V.X != 0 | V.Z != 0)
            {
                V1 = Vector3D.CrossProduct(V, new Vector3D(0, 1, 0));
            }
            else if (V.Y != 0)
            {
                V1 = Vector3D.CrossProduct(V, new Vector3D(0, 0, 1));
            }
            V1.Normalize();
            V1 = (my_Diameter / 2.0) * V1;
            for (int J = 0; J <= my_Slices; J++)
            {
                my_Vertices[count] = V1 + centerPoints[0];
                V1 = rotT.Transform(V1);
                count += 1;
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
            int count = 0;
            //'Calculate the normals as vector3D from the knot center coordinates towards the vertices 
            for (int I = 0; I <= my_Steps; I++)
            {
                for (int J = 0; J <= my_Slices; J++)
                {
                    my_Normals[count] = my_Vertices[count] - rm.Transform(centerPoints[I]);
                    my_Normals[count].Normalize();
                    count += 1;
                }
            }
        }

        public override void CreateIndices()
        {
            int indexCount = 6 * my_Steps * my_Slices - 1;
            my_Indices = new int[indexCount + 1];
            int count = 0;
            int K1;
            int K2;
            //Triangles = (K1, K+1, K2) - (K2, K1+1, K2+1)
            K1 = 0;
            K2 = my_Slices + 1;
            for (int I = 0; I < my_Steps; I++)
            {
                for (int J = 0; J < my_Slices; J++)
                {
                    my_Indices[count] = K1;
                    count += 1;
                    my_Indices[count] = K1 + 1;
                    count += 1;
                    my_Indices[count] = K2;
                    count += 1;
                    my_Indices[count] = K2;
                    count += 1;
                    my_Indices[count] = K1 + 1;
                    count += 1;
                    my_Indices[count] = K2 + 1;
                    count += 1;
                    K1 += 1;
                    K2 += 1;
                }
                K1 += 1;
                K2 += 1;
            }
        }

        public override void CreateTextureCoordinates()
        {
            my_TextureCoords = new Vector[my_VertexCount];
            int count = 0;
            //Calculate the texture coordinates for each vertex position
            for (int I = 0; I <= my_Steps; I++)
            {
                for (int J = 0; J <= my_Slices; J++)
                {
                    my_TextureCoords[count] = new Vector(my_TextureScaleX * J / my_Slices, my_TextureScaleY * I / my_Steps);
                    count += 1;
                }
            }
        }

        /// <summary>
        /// X = number of vertices per stack, Y = number of stacks, Z = 0
        /// </summary>
        /// <returns></returns>
        public override Vector3D GetVertexLayout()
        {
            return new Vector3D(my_Slices + 1, my_Steps, 0);
        }
    }
}
