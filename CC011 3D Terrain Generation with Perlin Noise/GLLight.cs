using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using SharpGL;
using SharpGL.Shaders;

namespace _3D_Perlin_Terrain
{
    public class GLLight
    {
        private float my_Index;
        private bool my_SwitchedOn;
        private readonly LightType my_Type;
        private Vector3D my_Position;
        private Vector3D my_Direction;
        private float my_CutOff;
        private float my_OuterCutOff;
        private float my_Constant;
        private float my_Linear;
        private float my_Quadratic;
        private Color my_Ambient;
        private Color my_Diffuse;
        private Color my_Specular;
        
        public GLLight(LightType type)
        {
            my_Index = 0.0F;
            my_SwitchedOn = true;
            my_Type = type;
            my_Position = new Vector3D(0, 0, 0);
            my_Direction = new Vector3D(0, 0, 0);
            my_CutOff = 0.0F;
            my_OuterCutOff = 0.0F;
            my_Constant = 1.0F;
            my_Linear = 1.0F;
            my_Quadratic = 1.0F;
            my_Ambient = Colors.Black;
            my_Diffuse = Colors.Black;
            my_Specular = Colors.Black;
        }

        #region "Properties"

        public float Index
        {
            get { return my_Index; }
            set { my_Index = value; }
        }

        public LightType Type
        {
            get { return my_Type; }
        }

        public Vector3D Position
        {
            get { return my_Position; }
            set { my_Position = value; }
        }

        public Vector3D Direction
        {
            get { return my_Direction; }
            set { my_Direction = value; }
        }

        public double CutOff
        {
            get { return my_CutOff; }
            set { my_CutOff = (float)value; }
        }

        public double OuterCutOff
        {
            get { return my_OuterCutOff; }
            set { my_OuterCutOff = (float)value; }
        }

        public double Constant
        {
            get { return my_Constant ; }
            set { my_Constant = (float)value; }
        }

        public double Linear
        {
            get { return my_Linear; }
            set { my_Linear = (float)value; }
        }

        public double Quadratic
        {
            get { return my_Quadratic; }
            set { my_Quadratic = (float)value; }
        }

        public Color Ambient
        {
            get { return my_Ambient; }
            set { my_Ambient = value; }
        }

        public Color Diffuse
        {
            get { return my_Diffuse; }
            set { my_Diffuse = value; }
        }

        public Color Specular
        {
            get { return my_Specular; }
            set { my_Specular = value; }
        }

        public bool SwitchedOn
        {
            get { return my_SwitchedOn; }
            set { my_SwitchedOn = value; }
        }

        #endregion 

        public void SetLightData(OpenGL gl, GLScene scene, ShaderProgram shader)
        {
            if (my_SwitchedOn)
            {
                shader.SetUniform1(gl, "lights[" + my_Index.ToString() + "].type", (float)my_Type);
                shader.SetUniform3(gl, "lights[" + my_Index.ToString() + "].position", (float)my_Position.X, (float)my_Position.Y, (float)my_Position.Z);
                shader.SetUniform3(gl, "lights[" + my_Index.ToString() + "].direction", (float)my_Direction.X, (float)my_Direction.Y, (float)my_Direction.Z);
                shader.SetUniform1(gl, "lights[" + my_Index.ToString() + "].cutOff", (float)Math.Cos(my_CutOff * Math.PI / 180));
                shader.SetUniform1(gl, "lights[" + my_Index.ToString() + "].outerCutOff", (float)Math.Cos(my_OuterCutOff * Math.PI / 180));
                shader.SetUniform1(gl, "lights[" + my_Index.ToString() + "].constant", my_Constant);
                shader.SetUniform1(gl, "lights[" + my_Index.ToString() + "].linear", my_Linear);
                shader.SetUniform1(gl, "lights[" + my_Index.ToString() + "].quadratic", my_Quadratic);
                shader.SetUniform3(gl, "lights[" + my_Index.ToString() + "].ambient", my_Ambient.ScR, my_Ambient.ScG, my_Ambient.ScB);
                shader.SetUniform3(gl, "lights[" + my_Index.ToString() + "].diffuse", my_Diffuse.ScR, my_Diffuse.ScG, my_Diffuse.ScB);
                shader.SetUniform3(gl, "lights[" + my_Index.ToString() + "].specular", my_Specular.ScR, my_Specular.ScG, my_Specular.ScB);
            }
            else
            {
                shader.SetUniform1(gl, "lights[" + my_Index.ToString() + "].type", 0.0F);
            }
        }
    }
}

public enum LightType
{
    /// <summary>
    /// The socket is Empty
    /// </summary>
    NoLight = 0,
    /// <summary>
    /// e.g. A beam of sunlight
    /// </summary>
    DirectionalLight = 1,
    /// <summary>
    /// e.g. A light bulb
    /// </summary>
    PointLight = 2,
    /// <summary>
    /// e.g. A halogen spot
    /// </summary>
    SpotLight = 3
 }
