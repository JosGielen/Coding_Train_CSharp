using GlmNet;
using JG_GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace RubiksCube
{

    internal class Cubelet : GLGeometry
    {
        private double my_Size;
        private Vector3D my_Pos;
        private readonly Color[] c = new Color[6];  //The color of each Cubelet face.
        private int[] my_FaceNumbers = new int[3]; //The index numbers of the cubeletfaces that determine the colors of the cubelet.
        private string my_ColorNumbers;
        private List<Color> sideColors; //The color of each vertex.
        private Color hiddenColor;
        private readonly double Centerdistance;

        public Cubelet(Vector3D position, double size, double spacing, int[] cubeletfacenumbers)
        {
            my_Pos = position;
            my_Size = size;
            hiddenColor = Color.FromRgb(130, 130, 130);
            my_FaceNumbers[0] = cubeletfacenumbers[0];
            my_FaceNumbers[1] = cubeletfacenumbers[1];
            my_FaceNumbers[2] = cubeletfacenumbers[2];
            sideColors = new List<Color>();
            Centerdistance = size + spacing;
            my_VertexCount = 24;
            //Set the position of the cubelet
            Position = new Vector3D(Centerdistance * my_Pos.X, Centerdistance * my_Pos.Y, Centerdistance * my_Pos.Z);
            //Set the default color of each Cubelet face
            //The colors of the cubelet are set according to the cubeletfaces colors in SetColors(...)
            AmbientMaterial = Color.FromRgb(0, 0, 0);
            DiffuseMaterial = Color.FromRgb(0, 0, 0);
            SpecularMaterial = Color.FromRgb(255, 255, 255);
            Shininess = 60.0;
            VertexColorIntensity = 0.8;
            c[0] = hiddenColor;  //Up
            c[1] = hiddenColor;  //Down
            c[2] = hiddenColor;  //Front
            c[3] = hiddenColor;  //Back
            c[4] = hiddenColor;  //Left
            c[5] = hiddenColor;  //Right
        }

        public string ColorNumbers
        {
            get { return my_ColorNumbers; }
        }

        public Vector3D CubeletPosition
        {
            get { return my_Pos; }
        }

        public void ResetPosition()
        {
            Position = new Vector3D(Centerdistance * my_Pos.X, Centerdistance * my_Pos.Y, Centerdistance * my_Pos.Z);
            RotationAxis = new Vector3D(0, 0, 0);
        }

        public void SetColors(List<CubeletFace> cubeletfaces)
        {
            my_ColorNumbers = "";
            if (my_Pos.Y == 1)
            {
                c[0] = cubeletfaces[my_FaceNumbers[1]].FaceColor;
                my_ColorNumbers += cubeletfaces[my_FaceNumbers[1]].FaceColorNumber.ToString();
            }
            else
            {
                c[0] = hiddenColor;
            }
            if (my_Pos.Y == -1)
            {
                c[1] = cubeletfaces[my_FaceNumbers[1]].FaceColor;
                my_ColorNumbers += cubeletfaces[my_FaceNumbers[1]].FaceColorNumber.ToString();
            }
            else
            {
                c[1] = hiddenColor;
            }
            if (my_Pos.Z == 1)
            {
                c[2] = cubeletfaces[my_FaceNumbers[2]].FaceColor;
                my_ColorNumbers += cubeletfaces[my_FaceNumbers[2]].FaceColorNumber.ToString();
            }
            else
            {
                c[2] = hiddenColor;
            }
            if (my_Pos.Z == -1)
            {
                c[3] = cubeletfaces[my_FaceNumbers[2]].FaceColor;
                my_ColorNumbers += cubeletfaces[my_FaceNumbers[2]].FaceColorNumber.ToString();
            }
            else
            {
                c[3] = hiddenColor;
            }
            if (my_Pos.X == 1)
            {
                c[5] = cubeletfaces[my_FaceNumbers[0]].FaceColor;
                my_ColorNumbers += cubeletfaces[my_FaceNumbers[0]].FaceColorNumber.ToString();
            }
            else
            {
                c[5] = hiddenColor;
            }
            if (my_Pos.X == -1)
            {
                c[4] = cubeletfaces[my_FaceNumbers[0]].FaceColor;
                my_ColorNumbers += cubeletfaces[my_FaceNumbers[0]].FaceColorNumber.ToString();
            }
            else
            {
                c[4] = hiddenColor;
            }
            //Set the color of each vertex of the cubelet
            sideColors.Clear();
            for (int I = 0; I <= 5; I++) //6 faces 
            {
                for (int J = 0; J <= 3; J++) //4 vertices per face 
                {
                    sideColors.Add(c[I]);
                }
            }

            UseVertexColors = true;
            my_VertexColors = sideColors;
        }

        #region "GLGeometry Overrides"

        public override Vector3D GetVertexLayout()
        {
            return new Vector3D(4, 0, 6);
        }

        protected override void CreateVertices()
        {
            my_vertices = new Vector3D[24];
            //Calculate the vertex positions
            double x = 0.5 * my_Size;
            double y = 0.5 * my_Size;
            double z = 0.5 * my_Size;
            //UP surface
            my_vertices[0] = new Vector3D(x, y, z);
            my_vertices[1] = new Vector3D(x, y, -z);
            my_vertices[2] = new Vector3D(-x, y, -z);
            my_vertices[3] = new Vector3D(-x, y, z);
            //DOWN surface
            my_vertices[4] = new Vector3D(-x, -y, z);
            my_vertices[5] = new Vector3D(-x, -y, -z);
            my_vertices[6] = new Vector3D(x, -y, -z);
            my_vertices[7] = new Vector3D(x, -y, z);
            //FRONT surface
            my_vertices[8] = new Vector3D(x, y, z);
            my_vertices[9] = new Vector3D(-x, y, z);
            my_vertices[10] = new Vector3D(-x, -y, z);
            my_vertices[11] = new Vector3D(x, -y, z);
            //BACK surface
            my_vertices[12] = new Vector3D(x, y, -z);
            my_vertices[13] = new Vector3D(x, -y, -z);
            my_vertices[14] = new Vector3D(-x, -y, -z);
            my_vertices[15] = new Vector3D(-x, y, -z);
            //LEFT surface
            my_vertices[16] = new Vector3D(-x, y, -z);
            my_vertices[17] = new Vector3D(-x, -y, -z);
            my_vertices[18] = new Vector3D(-x, -y, z);
            my_vertices[19] = new Vector3D(-x, y, z);
            //RIGHT surface
            my_vertices[20] = new Vector3D(x, y, z);
            my_vertices[21] = new Vector3D(x, -y, z);
            my_vertices[22] = new Vector3D(x, -y, -z);
            my_vertices[23] = new Vector3D(x, y, -z);
        }

        protected override void CreateNormals()
        {
            my_normals = new Vector3D[24];
            //Calculate the normals for each vertex position
            //UP surface
            my_normals[0] = new Vector3D(0, 1, 0);
            my_normals[1] = new Vector3D(0, 1, 0);
            my_normals[2] = new Vector3D(0, 1, 0);
            my_normals[3] = new Vector3D(0, 1, 0);
            //DOWN surface
            my_normals[4] = new Vector3D(0, -1, 0);
            my_normals[5] = new Vector3D(0, -1, 0);
            my_normals[6] = new Vector3D(0, -1, 0);
            my_normals[7] = new Vector3D(0, -1, 0);
            //FRONT surface
            my_normals[8] = new Vector3D(0, 0, 1);
            my_normals[9] = new Vector3D(0, 0, 1);
            my_normals[10] = new Vector3D(0, 0, 1);
            my_normals[11] = new Vector3D(0, 0, 1);
            //BACK surface
            my_normals[12] = new Vector3D(0, 0, -1);
            my_normals[13] = new Vector3D(0, 0, -1);
            my_normals[14] = new Vector3D(0, 0, -1);
            my_normals[15] = new Vector3D(0, 0, -1);
            //LEFT surface
            my_normals[16] = new Vector3D(-1, 0, 0);
            my_normals[17] = new Vector3D(-1, 0, 0);
            my_normals[18] = new Vector3D(-1, 0, 0);
            my_normals[19] = new Vector3D(-1, 0, 0);
            //RIGHT surface
            my_normals[20] = new Vector3D(1, 0, 0);
            my_normals[21] = new Vector3D(1, 0, 0);
            my_normals[22] = new Vector3D(1, 0, 0);
            my_normals[23] = new Vector3D(1, 0, 0);
        }

        protected override void CreateIndices()
        {
            my_indices = new int[36];
            //Calculate the Indices for each box surface
            //UP surface
            my_indices[0] = 0;
            my_indices[1] = 1;
            my_indices[2] = 2;
            my_indices[3] = 2;
            my_indices[4] = 3;
            my_indices[5] = 0;
            //DOWN surface
            my_indices[6] = 4;
            my_indices[7] = 5;
            my_indices[8] = 6;
            my_indices[9] = 6;
            my_indices[10] = 7;
            my_indices[11] = 4;
            //FRONT surface
            my_indices[12] = 8;
            my_indices[13] = 9;
            my_indices[14] = 10;
            my_indices[15] = 10;
            my_indices[16] = 11;
            my_indices[17] = 8;
            //BACK surface
            my_indices[18] = 12;
            my_indices[19] = 13;
            my_indices[20] = 14;
            my_indices[21] = 14;
            my_indices[22] = 15;
            my_indices[23] = 12;
            //LEFT surface
            my_indices[24] = 16;
            my_indices[25] = 17;
            my_indices[26] = 18;
            my_indices[27] = 18;
            my_indices[28] = 19;
            my_indices[29] = 16;
            //RIGHT surface
            my_indices[30] = 20;
            my_indices[31] = 21;
            my_indices[32] = 22;
            my_indices[33] = 22;
            my_indices[34] = 23;
            my_indices[35] = 20;
        }

        protected override void CreateTexCoordinates()
        {
            my_textureCoords = new Vector[24];
            //Calculate the Texture Coordinates for each vertex position
            //UP surface
            my_textureCoords[0] = new Vector(my_TextureScaleX, 0);
            my_textureCoords[1] = new Vector(my_TextureScaleX, my_TextureScaleY);
            my_textureCoords[2] = new Vector(0, my_TextureScaleY);
            my_textureCoords[3] = new Vector(0, 0);
            //DOWN surface
            my_textureCoords[4] = new Vector(0, my_TextureScaleY);
            my_textureCoords[5] = new Vector(0, 0);
            my_textureCoords[6] = new Vector(my_TextureScaleX, 0);
            my_textureCoords[7] = new Vector(my_TextureScaleX, my_TextureScaleY);
            //FRONT surface
            my_textureCoords[8] = new Vector(my_TextureScaleX, my_TextureScaleY);
            my_textureCoords[9] = new Vector(0, my_TextureScaleY);
            my_textureCoords[10] = new Vector(0, 0);
            my_textureCoords[11] = new Vector(my_TextureScaleX, 0);
            //BACK surface
            my_textureCoords[12] = new Vector(0, my_TextureScaleY);
            my_textureCoords[13] = new Vector(0, 0);
            my_textureCoords[14] = new Vector(my_TextureScaleX, 0);
            my_textureCoords[15] = new Vector(my_TextureScaleX, my_TextureScaleY);
            //LEFT surface
            my_textureCoords[16] = new Vector(0, my_TextureScaleY);
            my_textureCoords[17] = new Vector(0, 0);
            my_textureCoords[18] = new Vector(my_TextureScaleX, 0);
            my_textureCoords[19] = new Vector(my_TextureScaleX, my_TextureScaleY);
            //RIGHT surface
            my_textureCoords[20] = new Vector(0, my_TextureScaleY);
            my_textureCoords[21] = new Vector(0, 0);
            my_textureCoords[22] = new Vector(my_TextureScaleX, 0);
            my_textureCoords[23] = new Vector(my_TextureScaleX, my_TextureScaleY);
        }

        #endregion




    }
}
