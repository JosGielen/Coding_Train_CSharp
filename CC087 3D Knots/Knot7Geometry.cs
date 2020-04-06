using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace _3D_Knots
{
    //X = A1 * sin(A2 * mu) * cos(A3 * mu) + A4 * cos(A5 * mu) * sin(A6 * mu) + A7 * sin(A8 * mu)
    //Y = B1 * sin(B2 * mu) * sin(B3 * mu) + B4 * cos(B5 * mu) * cos(B6 * mu) + B7 * cos(B8 * mu)
    //Z = C1 * cos(C2 * mu)


    //A1 = -0.45     B1 = -0.45      C1 = 0.75 
    //A2 = 1.5       B2 = 1.5        C2 = 1.5
    //A3 = 1.0       B3 = 1.0 
    //A4 = -0.3      B4 = 0.3
    //A5 = 1.5       B5 = 1.5
    //A6 = 1.0       B6 = 1.0
    //A7 = -0.5      B7 = 0.5
    //A8 = 1.0       B8 = 1.0
    //====================================================================
    class Knot7Geometry : GLGeometry
    {
        private double my_Size;
        private int my_Steps;
        private double my_Diameter;
        private int my_Slices;
        // Knot parameters
        public double a1 = -0.45;
        public double a2 = 1.5;
        public double a3 = 1.0;
        public double a4 = -0.3;
        public double a5 = 1.5;
        public double a6 = 1.0;
        public double a7 = -0.5;
        public double a8 = 1.0;
        public double b1 = -0.45;
        public double b2 = 1.5;
        public double b3 = 1.0;
        public double b4 = 0.3;
        public double b5 = 1.5;
        public double b6 = 1.0;
        public double b7 = 0.5;
        public double b8 = 1.0;
        public double c1 = 0.75;
        public double c2 = 1.5;
        public double c3 = 0.0;
        public double c4 = 0.0;
        public double c5 = 0.0;
        public double c6 = 0.0;
        public double c7 = 0.0;
        public double c8 = 0.0;

        public Knot7Geometry(double size, int steps, double diameter, int slices)
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
            int count = 0;
            double ds = 1.0 / my_Steps ;
            double dt = 1.0 / my_Slices ;
            //Calculate the vertex positions
            for (double  s = 0; s <= 1 + ds /2; s += ds)
            {
                for (double t = 0; t <= 1 + dt/2; t += dt)
                {
                    my_Vertices[count] = EvaluateTrefoil(s,t);
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
            Matrix3D rm = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z);
            my_Normals = new Vector3D[my_VertexCount];
            int count = 0;
            double e = 0.01;
            double ds = 1.0 / my_Steps;
            double dt = 1.0 / my_Slices;
            //Calculate the normals for each vertex position
            for (double s = 0; s <= 1 + ds / 2; s += ds)
            {
                for (double t = 0; t <= 1 + dt / 2; t += dt)
                {
                    Vector3D p = EvaluateTrefoil(s, t);
                    Vector3D u = EvaluateTrefoil(s + e, t) - p;
                    Vector3D v = EvaluateTrefoil(s, t + e) - p;
                    my_Normals[count] = Vector3D.CrossProduct(u, v);
                    my_Normals[count].Normalize();
                    count += 1;
                }
            }
            //Apply the initial rotation
            for (int I = 0; I < my_Normals.Length ; I++)
            {
              my_Normals[I] = rm.Transform(my_Normals[I]);
            }
        }

        public override void CreateIndices()
        {
            int indexCount = 6 * (my_Steps+1) * (my_Slices+1);
            my_Indices = new int[indexCount];
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
            int count = 0;
            double ds = 1.0 / my_Steps;
            double dt = 1.0 / my_Slices;
            //Calculate the texture coordinates for each vertex position
            for (double s = 0; s <= 1 + ds / 2; s += ds)
            {
                for (double t = 0; t <= 1 + dt / 2; t += dt)
                {
                    my_TextureCoords[count] = new Vector(my_TextureScaleX * t, my_TextureScaleY * s);
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

        private Vector3D EvaluateTrefoil(double s, double t)
        {
            double a = b7;
            double b = b4;
            double c = c1 / 1.5;
            double d = my_Diameter;
            double mu = (1 - s) * 4 * Math.PI;
            double v = t * Math.PI * 2;
            double r = a + b * Math.Cos(1.5 * mu);
            double x = r * Math.Cos(mu);
            double y = r * Math.Sin(mu);
            double z = c * Math.Sin(1.5 * mu);
            Vector3D dv = new Vector3D()
            {
                X = a1 * Math.Sin(a2 * mu) * Math.Cos(a3 * mu) + a4 * Math.Cos(a5 * mu) * Math.Sin(a6 * mu) + a7 * Math.Sin(a8 * mu),
                Y = b1 * Math.Sin(b2 * mu) * Math.Sin(b3 * mu) + b4 * Math.Cos(b5 * mu) * Math.Cos(b6 * mu) + b7 * Math.Cos(b8 * mu),
                Z = c1 * Math.Cos(c2 * mu)
            };
            dv.Normalize();
            Vector3D qvn = new Vector3D(dv.Y, -dv.X, 0.0);
            qvn.Normalize();
            Vector3D ww = Vector3D.CrossProduct(dv, qvn);
            Vector3D range = new Vector3D()
            {
                X = my_Size * (x + d * (qvn.X * Math.Cos(v) + ww.X * Math.Sin(v))),
                Y = my_Size * (y + d * (qvn.Y * Math.Cos(v) + ww.Y * Math.Sin(v))),
                Z = my_Size * (z + d * ww.Z * Math.Sin(v))
            };
            return range;
        }
    }
}

