using GlmNet;
using System.Windows.Media.Media3D;

namespace _3D_Knots
{
    public abstract class GLCamera
    {
        protected CameraType my_Type;
        protected Vector3D my_Position;
        protected Vector3D my_TargetPosition;
        protected Vector3D my_ViewDirection;
        protected Vector3D my_UpDirection;
        protected double my_Yaw;
        protected double my_Pitch;
        protected double my_Distance;
        protected double my_MoveSpeed;

        #region "Properties"

        public CameraType Type
        {
            get { return my_Type; }
        }

        public Vector3D Position
        {
            get { return my_Position; }
            set 
            { 
                my_Position = value;
                UpdateDirection();
            }
        }

        public Vector3D TargetPosition
        {
            get { return my_TargetPosition; }
            set 
            {
                my_TargetPosition = value;
                UpdateDirection();
            }
        }

        public Vector3D UpDirection
        {
            get { return my_UpDirection; }
            set 
            {
                my_UpDirection = value;
                UpdateDirection();
            }
        }

        public float X
        {
            get { return (float)my_Position.X; }
        }

        public float Y
        {
            get { return (float)my_Position.Y; }
        }

        public float Z
        {
            get { return (float)my_Position.Z; }
        }

        public double MoveSpeed
        {
            get { return my_MoveSpeed; }
            set { my_MoveSpeed = value; }
        }

        #endregion

        public mat4 GetViewMatrix()
        {
            mat4 result = mat4.identity();
            Vector3D X;
            Vector3D Y;
            Vector3D Z;
            Z = -my_ViewDirection;
            Z.Normalize();
            Y = my_UpDirection;
            X = Vector3D.CrossProduct(Y, Z);
            Y = Vector3D.CrossProduct(Z, X);
            X.Normalize();
            Y.Normalize();
            result[0, 0] = (float)X.X;
            result[1, 0] = (float)X.Y;
            result[2, 0] = (float)X.Z;
            result[3, 0] = (float)Vector3D.DotProduct(-X, Position);
            result[0, 1] = (float)Y.X;
            result[1, 1] = (float)Y.Y;
            result[2, 1] = (float)Y.Z;
            result[3, 1] = (float)Vector3D.DotProduct(-Y, Position);
            result[0, 2] = (float)Z.X;
            result[1, 2] = (float)Z.Y;
            result[2, 2] = (float)Z.Z;
            result[3, 2] = (float)Vector3D.DotProduct(-Z, Position);
            result[0, 3] = 0.0F;
            result[1, 3] = 0.0F;
            result[2, 3] = 0.0F;
            result[3, 3] = 1.0F;
            return result;
        }
        
        public abstract void Vertical(double amount);
        public abstract void Horizontal(double amount);
        public abstract void Forward(double amount);
        public abstract void UpdatePosition();
        public abstract void UpdateDirection();
    }
}
    public enum CameraType
    {
        Fixed = 0,
        ParentControlled = 1,
        FreeFlying = 2,
        Trackball = 3
    }

