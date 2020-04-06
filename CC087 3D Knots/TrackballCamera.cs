using System;
using System.Windows.Media.Media3D;

namespace _3D_Knots
{
    class TrackballCamera : GLCamera
    {
        /// <summary>
        /// A Trackball Camera rotates around a targetposition and always looks at that position
        /// </summary>
        public TrackballCamera()
        {
            my_Type = CameraType.Trackball;
            my_Position = new Vector3D(0, 0, 1);
            my_TargetPosition = new Vector3D(0, 0, 0);
            my_ViewDirection = my_TargetPosition - my_Position;
            my_Distance = 1.0;
            my_UpDirection = new Vector3D(0, 1, 0);
            my_Yaw = Math.PI;
            my_Pitch = 0.0;
            my_MoveSpeed = 1.0;        
        }

        /// <summary>
        /// A Trackball Camera rotates around a targetposition and always looks at that position
        /// </summary>
        /// <param name="position">The initial Camera position</param>
        /// <param name="targetPosition">The Target position</param>
        /// <param name="up">The UP direction of the Camera</param>
        public TrackballCamera(Vector3D position, Vector3D targetPosition, Vector3D up)
        {
            my_Type = CameraType.Trackball;
            my_Position = position;
            my_TargetPosition = targetPosition;
            my_UpDirection = up;
            my_MoveSpeed = 1.0;
            UpdateDirection();
        }

        #region "Properties"

        protected double DistanceToTarget
        {
            get { return my_Distance; }
            set 
            {
                my_Distance = value;
                UpdatePosition();
            }
        }

        #endregion

        public override void Vertical(double amount)
        {
            my_Pitch -= amount * my_MoveSpeed * Math.PI / 180;
            if (my_Pitch < -0.49 * Math.PI) { my_Pitch = -0.49 * Math.PI; }
            if (my_Pitch > 0.49 * Math.PI) { my_Pitch = 0.49 * Math.PI; }
            UpdatePosition();
        }

        public override void Horizontal(double amount)
        {
            my_Yaw += amount * my_MoveSpeed * Math.PI / 180;
            UpdatePosition();
        }

        public override void Forward(double amount)
        {
            my_Distance += amount * my_MoveSpeed;
            UpdatePosition();        
        }
        
        public override void UpdateDirection()
        {
            my_ViewDirection = my_TargetPosition - my_Position;
            my_Distance = my_ViewDirection.Length;
            my_ViewDirection.Normalize();
            my_Yaw = Vector3D.AngleBetween(new Vector3D(my_ViewDirection.X, 0, my_ViewDirection.Z), new Vector3D(0, 0, -1)) * Math.PI / 180;
            my_Pitch = Vector3D.AngleBetween(my_ViewDirection, new Vector3D(my_ViewDirection.X, 0, my_ViewDirection.Z)) * Math.PI / 180;
        }

        public override void UpdatePosition()
        {
            my_Position = new Vector3D()
            {
                X = -my_Distance * Math.Sin(my_Yaw) * Math.Cos(my_Pitch) + my_TargetPosition.X,
                Y = my_Distance * Math.Sin(my_Pitch) + my_TargetPosition.Y,
                Z = my_Distance * Math.Cos(my_Yaw) * Math.Cos(my_Pitch) + my_TargetPosition.Z
            };
            my_ViewDirection = my_TargetPosition - my_Position;
        }
    }
}
