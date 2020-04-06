using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Assets;
using GlmNet;

namespace _3D_Perlin_Terrain
{
    public abstract class GLGeometry
    {
        protected OpenGL my_OpenGL;
        protected int my_VertexCount;
        protected Vector3D[] my_Vertices;
        protected Vector3D[] my_Normals;
        protected Color[] my_Colors;
        protected int[] my_Indices;
        protected Vector[] my_TextureCoords;
        protected VertexBufferArray my_VertexBufferArray;
        private VertexBuffer vertexBuffer;
        private VertexBuffer normalBuffer;
        private VertexBuffer colorBuffer;
        private VertexBuffer textureBuffer;
        private IndexBuffer indexBuffer;
        protected mat4 my_ModelMatrix = mat4.identity();
        protected mat4 my_ViewMatrix = mat4.identity();
        protected mat3 my_NormalMatrix = mat3.identity();
        private vec3 my_Position = new vec3(0, 0, 0);
        private vec3 my_Direction = new vec3(0, 0, 0);
        private double my_Speed = 0.0;
        private vec3 my_RotationAxis = new vec3(0, 1, 0);
        private float my_RotationAngle = 0;
        private float my_RotationSpeed = 0;
        private Vector3D my_InitialRotationAxis = new Vector3D(0, 0, 0);
        //Material
        protected bool my_UseMaterial = true;
        private Color my_DiffuseMaterial;
        private Color my_AmbientMaterial;
        private Color my_SpecularMaterial;
        private float my_Shininess;
        //Texture
        protected bool my_UseTexture = false;
        protected string my_TextureFile = "";
        protected Texture my_Texture;
        //VertexColors
        private string my_PaletteFile;
        private bool my_UseVertexColors = false;
        protected List<Color> my_VertexColors;
        private double my_VertexColorIntensity = 0.8;
        //Drawing Mode
        private DrawMode my_DrawMode = DrawMode.Fill;
        protected BeginMode GLBeginMode = BeginMode.Triangles;
        private float my_LineWidth = 1.0F;
        protected float my_PointSize = 1.0F;
        protected double my_TextureScaleX = 1.0;
        protected double my_TextureScaleY = 1.0;

        #region "Properties"

        public Vector3D Position
        {
            get { return new Vector3D(my_Position.x, my_Position.y, my_Position.z); }
            set { my_Position = new vec3((float)value.X, (float)value.Y, (float)value.Z); }
        }

        public Vector3D Direction
        {
            get { return new Vector3D(my_Direction.x, my_Direction.y, my_Direction.z); }
            set { my_Direction = new vec3((float)value.X, (float)value.Y, (float)value.Z); }
        }

        public double Speed
        {
            get { return my_Speed; }
            set { my_Speed = value; }
        }

        public Vector3D InitialRotationAxis
        {
            get { return my_InitialRotationAxis; }
            set { my_InitialRotationAxis = value; }
        }

        public double RotationAngle
        {
            get { return my_RotationAngle; }
            set { my_RotationAngle = (float)value; }
        }

        public double RotationSpeed
        {
            get { return my_RotationSpeed; }
            set { my_RotationSpeed = (float)value; }
        }

        public Vector3D RotationAxis
        {
            get { return new Vector3D(my_RotationAxis.x, my_RotationAxis.y, my_RotationAxis.z); }
            set { my_RotationAxis = new vec3((float)value.X, (float)value.Y, (float)value.Z); }
        }

        public Vector3D[] Vertices
        {
            get { return my_Vertices; }
        }

        public Vector3D[] Normals
        {
            get { return my_Normals; }
        }

        public Color[] VertexColors
        {
            get { return my_Colors; }
        }

        public int[] Indices
        {
            get { return my_Indices; }
        }

        public Vector[] TextureCoordinates
        {
            get { return my_TextureCoords; }
        }

        public Color DiffuseMaterial
        {
            get { return my_DiffuseMaterial; }
            set { my_DiffuseMaterial = value; }
        }

        public Color AmbientMaterial
        {
            get { return my_AmbientMaterial; }
            set { my_AmbientMaterial = value; }
        }

        public Color SpecularMaterial
        {
            get { return my_SpecularMaterial; }
            set { my_SpecularMaterial = value; }
        }

        public double Shininess
        {
            get { return my_Shininess ; }
            set { my_Shininess  = (float)value; }
        }

        public string TextureFile
        {
            get { return my_TextureFile; }
            set 
            {
                my_TextureFile = value;
                if (my_OpenGL != null & my_UseTexture & my_TextureFile != "")
                {
                    try
                    {
                        my_Texture.Create(my_OpenGL, my_TextureFile);
                        my_Texture.Name = my_TextureFile;
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("Unable to create Texture from File:" + my_TextureFile, "GLGeometry error", MessageBoxButton.OK, MessageBoxImage.Error);
                        my_Texture = new Texture();
                    }
                }
                else
                {
                    my_Texture = new Texture();
                }
            }
        }

        public bool UseTexture
        {
            get { return my_UseTexture; }
            set
            {
                my_UseTexture = value;
                if (my_OpenGL != null & my_UseTexture & my_TextureFile != "")
                {
                    try
                    {
                        my_Texture.Create(my_OpenGL, my_TextureFile);
                        my_Texture.Name = my_TextureFile;
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("Unable to create Texture from File:" + my_TextureFile, "GLGeometry error", MessageBoxButton.OK, MessageBoxImage.Error);
                        my_Texture = new Texture();
                    }
                }
                else
                {
                    my_Texture = new Texture();
                }
            }
        }

        public DrawMode DrawMode
        {
            get { return my_DrawMode; }
            set { my_DrawMode = value; }
        }

        public double LineWidth
        {
            get { return my_LineWidth; }
            set { my_LineWidth = (float)value; }
        }

        public double PointSize
        {
            get { return my_PointSize; }
            set { my_PointSize = (float)value; }
        }

        public double TextureScaleX
        {
            get { return my_TextureScaleX ; }
            set { my_TextureScaleX = (float)value; }
        }

        public double TextureScaleY
        {
            get { return my_TextureScaleY; }
            set { my_TextureScaleY = (float)value; }
        }

        public int VertexCount
        {
            get { return my_VertexCount; }
        }

        public double VertexColorIntensity
        {
            get { return my_VertexColorIntensity; }
            set { my_VertexColorIntensity = value; }
        }

        public bool UseVertexColors
        {
            get { return my_UseVertexColors; }
            set { my_UseVertexColors = value; }
        }

        public string PaletteFile
        {
            get { return my_PaletteFile; }
        }

        public bool UseMaterial
        {
            get { return my_UseMaterial; }
            set { my_UseMaterial = value; }
        }
        #endregion 


        public void GenerateGeometry(GLScene scene)
        {
            my_OpenGL = scene.GL;
            my_Texture = new Texture() { Name = "" };
            my_VertexBufferArray = new VertexBufferArray();
            my_VertexBufferArray.Create(my_OpenGL);
            my_VertexBufferArray.Bind(my_OpenGL);
            CreateVertexBuffer(GLScene.positionAttribute);
            CreateNormalBuffer(GLScene.normalAttribute);
            CreateColorBuffer(GLScene.colorAttribute);
            CreateTextureCoordBuffer(GLScene.textureAttribute);
            CreateIndexBuffer();
            my_VertexBufferArray.Unbind(my_OpenGL);
            if (my_OpenGL != null & my_UseTexture & my_TextureFile != "")
            {
                try
                {
                    my_Texture.Create(my_OpenGL, my_TextureFile);
                    my_Texture.Name = my_TextureFile;
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Unable to create Texture from File:" + my_TextureFile, "GeometryGL error", MessageBoxButton.OK, MessageBoxImage.Error);
                    my_Texture = new Texture();
                }
            }
        }

        public void Update()
        {
            //Move the geometry object.
            my_Position += (float)my_Speed * my_Direction;
            //Rotate the geometry object.
            my_RotationAngle += my_RotationSpeed;
        }

        public void Draw(ShaderProgram shader)
        {
            my_OpenGL.LineWidth(my_LineWidth);
            my_OpenGL.PointSize(my_PointSize);
            if (my_UseMaterial )
            {
                shader.SetUniform3(my_OpenGL, "material.diffuse", my_DiffuseMaterial.ScR, my_DiffuseMaterial.ScG, my_DiffuseMaterial.ScB);
                shader.SetUniform3(my_OpenGL, "material.ambient", my_AmbientMaterial.ScR, my_AmbientMaterial.ScG, my_AmbientMaterial.ScB);
            }
            else
            {
                shader.SetUniform3(my_OpenGL, "material.diffuse", 0.0F, 0.0F, 0.0F);
                shader.SetUniform3(my_OpenGL, "material.ambient", 0.0F, 0.0F, 0.0F);
            }
            shader.SetUniform3(my_OpenGL, "material.specular", my_SpecularMaterial.ScR, my_SpecularMaterial.ScG, my_SpecularMaterial.ScB);
            shader.SetUniform1(my_OpenGL, "material.shininess", my_Shininess);
            shader.SetUniformMatrix4(my_OpenGL, "Model", GetModelMatrix().to_array());
            shader.SetUniformMatrix3(my_OpenGL, "NormalMatrix", GetNormalMatrix().to_array());
            my_Texture.Bind(my_OpenGL);
            my_VertexBufferArray.Bind(my_OpenGL);
            switch (my_DrawMode)
            {
                case DrawMode.Fill:
                    my_OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
                    break;
                case DrawMode.Lines:
                    my_OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
                    break;
                case DrawMode.Points:
                    my_OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Points);
                    break;
            }
            switch (GLBeginMode)
            {
                case BeginMode.Triangles:
                    my_OpenGL.DrawElements(OpenGL.GL_TRIANGLES, my_Indices.Length, OpenGL.GL_UNSIGNED_INT, IntPtr.Zero);
                    break;
                case BeginMode.Lines:
                    my_OpenGL.DrawElements(OpenGL.GL_LINES, my_Indices.Length, OpenGL.GL_UNSIGNED_INT, IntPtr.Zero);
                    break;
                case BeginMode.Points:
                    my_OpenGL.DrawElements(OpenGL.GL_POINTS, my_Indices.Length, OpenGL.GL_UNSIGNED_INT, IntPtr.Zero);
                    break;
            }
            my_VertexBufferArray.Unbind(my_OpenGL);
        }

        public static Matrix3D CalculateRotationMatrix(double x , double y, double z)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), x));
            matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0) * matrix, y));
            matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1) * matrix, z));
            return matrix;
        }

        private void CreateVertexBuffer(uint vertexAttributeLocation)
        {
            CreateVertices();
            vertexBuffer = new VertexBuffer();
            vertexBuffer.Create(my_OpenGL);
            vertexBuffer.Bind(my_OpenGL);
            List<float> vData = new List<float>();
            foreach (Vector3D v in my_Vertices)
            {
                vData.Add((float)v.X);
                vData.Add((float)v.Y);
                vData.Add((float)v.Z);
            }
            vertexBuffer.SetData(my_OpenGL, vertexAttributeLocation, vData.ToArray(), false, 3);
        }

        private void CreateNormalBuffer(uint normalAttributeLocation)
        {
            CreateNormals();
            normalBuffer = new VertexBuffer();
            normalBuffer.Create(my_OpenGL);
            normalBuffer.Bind(my_OpenGL);
            List<float> nData = new List<float>();
            foreach (Vector3D n in my_Normals)
            {
                nData.Add((float)n.X);
                nData.Add((float)n.Y);
                nData.Add((float)n.Z);
            }
            normalBuffer.SetData(my_OpenGL, normalAttributeLocation, nData.ToArray(), false, 3);
        }

        private void CreateColorBuffer(uint colorAttributeLocation)
        {
            CreateColors();
            colorBuffer = new VertexBuffer();
            colorBuffer.Create(my_OpenGL);
            colorBuffer.Bind(my_OpenGL);
            List<float> cData = new List<float>();
            foreach (Color c in my_Colors)
            {
                cData.Add(c.ScR);
                cData.Add(c.ScG);
                cData.Add(c.ScB);
            }
            colorBuffer.SetData(my_OpenGL, colorAttributeLocation, cData.ToArray(), false, 3);
        }

        private void CreateTextureCoordBuffer(uint textureAttributeLocation)
        {
            CreateTextureCoordinates();
            textureBuffer = new VertexBuffer();
            textureBuffer.Create(my_OpenGL);
            textureBuffer.Bind(my_OpenGL);
            List<float> tData = new List<float>();
            foreach (Vector t in my_TextureCoords)
            {
                tData.Add((float)t.X);
                tData.Add((float)t.Y);
            }
            textureBuffer.SetData(my_OpenGL, textureAttributeLocation, tData.ToArray(), false, 2);
        }

        private void CreateIndexBuffer()
        {
            CreateIndices();
            indexBuffer = new IndexBuffer();
            indexBuffer.Create(my_OpenGL);
            indexBuffer.Bind(my_OpenGL);
            uint[] indis = new uint[my_Indices.Length];
            for (int I = 0; I < my_Indices.Length; I++)
            {
                indis[I] = (uint)my_Indices[I];
            }
            indexBuffer.SetData(my_OpenGL, indis);
        }

        protected virtual mat4 GetModelMatrix()
        {
            mat4 rotation;
            if (my_RotationAxis.x + my_RotationAxis.y + my_RotationAxis.z == 0)
            {
                rotation = mat4.identity();
            }
            else
            {
                rotation = glm.rotate(mat4.identity(), my_RotationAngle, my_RotationAxis);
            }
            mat4 translation = glm.translate(mat4.identity(), my_Position);
            my_ModelMatrix = rotation * translation;
            return my_ModelMatrix;
        }

        protected virtual mat3 GetNormalMatrix()
        {
            return GetModelMatrix().to_mat3();
        }

        /// <summary>
        /// Specify the color of each Vertex.
        /// <para>If number of colors in ColorList is less than the number of Vertices the colors will wrap.</para>
        /// </summary>
        /// <param name="palettefile">*.cpl file made by ColorGradient.exe</param>
        /// <param name="colorcount">Number of colors to use from the palette.</param>
        public void SetVertexColors(string palettefile, int colorcount)
        {
            my_PaletteFile = palettefile;
            if (colorcount > 0)
            {
                try
                {
                    ColorPalette pal = new ColorPalette(palettefile);
                    my_VertexColors = pal.GetColors(colorcount);
                    my_UseVertexColors = true;
                }
                catch (Exception)
                {
                    my_VertexColors = null;
                    my_UseVertexColors = false;
                }
            }
            else
            {
                my_VertexColors = null;
                my_UseVertexColors = false;
            }
        }

        protected virtual void CreateColors()
        {
            if (my_VertexCount < 1)
            {
                my_Colors = new Color[] { Colors.Black }; 
                return;
            }
            my_Colors = new Color[VertexCount];
            if (my_UseVertexColors & my_VertexColors != null)
            {
                int index;
                for (int I = 0; I < VertexCount; I++)
                {
                    index = I % my_VertexColors.Count;
                    my_Colors[I] = my_VertexColors[index] * (float)my_VertexColorIntensity;
                    my_Colors[I].A = 255;
                }
            }
            else
            {
                for (int I = 0; I < VertexCount; I++)
                {
                    my_Colors[I] = Colors.Black;
                    my_Colors[I].A = 255;
                }
            }
        }

        public abstract void CreateVertices();

        public abstract void CreateNormals();

        public abstract void CreateIndices();

        public abstract void CreateTextureCoordinates();

        public abstract Vector3D GetVertexLayout();

    }
}

public enum DrawMode
{
    Points = 0,
    Lines = 1,
    Fill = 2
}

public enum GLDrawMode
{
    Points = 0,
    Lines = 1,
    Triangles = 2
}
