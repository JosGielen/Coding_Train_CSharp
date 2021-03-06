﻿using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace _3D_Knots
{
    //For beta = 0 To pi
    //r = a1 + a2 * sin(a3 * beta)
    //theta = b1 * beta
    //phi = c1 * pi * sin(c2 * beta)

    //a1 = 0.8   b1 = 2.0    c1 = 0.6
    //a2 = 1.6               c2 = 12.0
    //a3 = 6.0
    //====================================================================
    class Knot4Geometry : GLGeometry
    {
        private double my_Size;
        private int my_Steps;
        private double my_Diameter;
        private int my_Slices;
        private Vector3D[] centerPoints;
        // Knot parameters
        public double a1 = 0.8;
        public double a2 = 1.6;
        public double a3 = 6.0;
        public double a4 = 0.0;
        public double a5 = 0.0;
        public double a6 = 0.0;
        public double a7 = 0.0;
        public double a8 = 0.0;
        public double b1 = 2.0;
        public double b2 = 0.0;
        public double b3 = 0.0;
        public double b4 = 0.0;
        public double b5 = 0.0;
        public double b6 = 0.0;
        public double b7 = 0.0;
        public double b8 = 0.0;
        public double c1 = 0.6;
        public double c2 = 12.0;
        public double c3 = 0.0;
        public double c4 = 0.0;
        public double c5 = 0.0;
        public double c6 = 0.0;
        public double c7 = 0.0;
        public double c8 = 0.0;

        public Knot4Geometry(double size, int steps, double diameter, int slices)
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
            double beta;
            double theta;
            double phi;
            double r;
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
                beta = I * Math.PI / my_Steps;
                r = my_Size * (a1 + a2 * Math.Sin(a3 * beta));
                theta = b1 * beta;
                phi = c1 * Math.PI * Math.Sin(c2 * beta);
                x = r * Math.Cos(phi) * Math.Cos(theta);
                y = r * Math.Cos(phi) * Math.Sin(theta);
                z = r * Math.Sin(phi);
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

