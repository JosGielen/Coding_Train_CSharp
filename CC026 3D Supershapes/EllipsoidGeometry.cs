using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace _3D_Supershape
{
    class EllipsoidGeometry : GLGeometry 
    {
        private double my_Size;
        private int my_Steps;
        // Knot parameters
        public double a1 = 1.0;
        public double b1 = 1.0;
        public double m1 = 7.0; 
        public double n11 = 0.2;
        public double n21 = 1.7;
        public double n31 = 1.7;
        public double a2 = 1.0;
        public double b2 = 1.0;
        public double m2 = 7.0;
        public double n12 = 0.2;
        public double n22 = 1.7;
        public double n32 = 1.7;


        public EllipsoidGeometry(double size, int steps)
        {
            my_Size = size;
            my_Steps = steps;
            my_VertexCount = (my_Steps + 1) * (my_Steps + 1);
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

        #endregion 

        public override void CreateVertices()
        {
            my_VertexCount = (my_Steps + 1) * (my_Steps + 1);
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            my_Vertices = new Vector3D[my_VertexCount];
            int count = 0;
            double r1;
            double r2;
            double theta;
            double phi;
            double x;
            double y;
            double z;
            //Calculate the knot center coordinates
            for (int I = 0; I <= my_Steps; I++)
            {
                phi = I * Math.PI / my_Steps - Math.PI / 2;          //-PI/2 to PI/2
                r2 = SuperShapeRadius(phi, a2, b2, m2, n12, n22, n32);
                for (int J = 0; J <= my_Steps; J++)
                {
                    theta = 2 * J * Math.PI / my_Steps - Math.PI;    //PI to PI
                r1 = SuperShapeRadius(theta, a1, b1, m1, n11, n21, n31);
                    x = my_Size * r1 * Math.Cos(theta) * r2 * Math.Cos(phi);
                    y = my_Size * r1 * Math.Sin(theta) * r2 * Math.Cos(phi);
                    z = my_Size * r2 * Math.Sin(phi);
                    my_Vertices[count] = new Vector3D(x, y, z);
                    count += 1;
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
            my_VertexCount = (my_Steps + 1) * (my_Steps + 1);
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            my_Normals = new Vector3D[my_VertexCount];
            int count = 0;
            const double e = 0.01;
            double r1;
            double r2;
            double theta;
            double phi;
            double x;
            double y;
            double z;
            //Calculate the knot center coordinates
            for (int I = 0; I <= my_Steps; I++)
            {
                for (int J = 0; J <= my_Steps; J++)
                {
                    phi = I * Math.PI / my_Steps - Math.PI / 2;
                    r2 = SuperShapeRadius(phi, a2, b2, m2, n12, n22, n32);
                    theta = 2 * J * Math.PI / my_Steps - Math.PI;
                    r1 = SuperShapeRadius(theta, a1, b1, m1, n11, n21, n31);
                    x = my_Size * r1 * Math.Cos(theta) * r2 * Math.Cos(phi);
                    y = my_Size * r1 * Math.Sin(theta) * r2 * Math.Cos(phi);
                    z = my_Size * r2 * Math.Sin(phi);
                    Vector3D p = new Vector3D(x, y, z);

                    phi = (I + e) * Math.PI / my_Steps - Math.PI / 2;
                    r2 = SuperShapeRadius(phi, a2, b2, m2, n12, n22, n32);
                    theta = 2 * J * Math.PI / my_Steps - Math.PI;
                    r1 = SuperShapeRadius(theta, a1, b1, m1, n11, n21, n31);
                    x = my_Size * r1 * Math.Cos(theta) * r2 * Math.Cos(phi);
                    y = my_Size * r1 * Math.Sin(theta) * r2 * Math.Cos(phi);
                    z = my_Size * r2 * Math.Sin(phi);
                    Vector3D u = new Vector3D(x, y, z) - p;

                    phi = I * Math.PI / my_Steps - Math.PI / 2;
                    r2 = SuperShapeRadius(phi, a2, b2, m2, n12, n22, n32);
                    theta = 2 * (J + e) * Math.PI / my_Steps - Math.PI;
                    r1 = SuperShapeRadius(theta, a1, b1, m1, n11, n21, n31);
                    x = my_Size * r1 * Math.Cos(theta) * r2 * Math.Cos(phi);
                    y = my_Size * r1 * Math.Sin(theta) * r2 * Math.Cos(phi);
                    z = my_Size * r2 * Math.Sin(phi);
                    Vector3D v = new Vector3D(x, y, z) - p;
                    my_Normals[count] = Vector3D.CrossProduct(v, u);
                    my_Normals[count].Normalize();
                    count += 1;
                }
            }
            //Apply the initial rotation
            for (int I = 0; I < my_Normals.Length; I++)
            {
                my_Normals[I] = rm.Transform(my_Normals[I]);
            }
        }

        private double SuperShapeRadius(double angle, double a, double b, double m, double n1, double n2, double n3)
        {
            double T1 = Math.Abs(Math.Cos(m * angle / 4) / a);
            T1 = Math.Pow(T1, n2);
            double T2 = Math.Abs(Math.Sin(m * angle / 4) / b);
            T2 = Math.Pow(T2, n3);
            return Math.Pow((T1 + T2), (-1 / n1));
        }

        public override void CreateIndices()
        {
            int indexCount = 6 * (my_Steps * (my_Steps + 1));
            my_Indices = new int[indexCount];
            int count = 0;
            int K1;
            int K2;
            //Triangles = (K1, K+1, K2) - (K2, K1+1, K2+1)
            K1 = 0;
            K2 = my_Steps + 1;
            for (int I = 0; I < my_Steps -1; I++)
            {
                for (int J = 1; J <= my_Steps; J++)
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
                K1 += 1;
                K2 += 1;
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
        /// X = number of vertices per stack, Y = number of stacks, Z = 0
        /// </summary>
        /// <returns></returns>
        public override Vector3D GetVertexLayout()
        {
            return new Vector3D(my_Steps + 1, my_Steps, 0);
        }
    }
}
