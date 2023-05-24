using System;
using System.Windows.Media.Media3D;

namespace _MandelBulb
{
    class FixedCamera : GLCamera
    {
        private Vector3D my_StartPosition;
        private Vector3D my_StartTargetPosition;
        private Vector3D my_startUpDirection;

        /// <summary>
        /// A Fixed camera can not be changed at all.
        /// </summary>
        /// <param name="position">The camera will always stay at this position.</param>
        /// <param name="targetPosition">The camera will always look at this target position.</param>
        /// <param name="up">The Up direction determines the roll angle of the camera. It can not be changed.</param>
        public FixedCamera(Vector3D position, Vector3D targetPosition, Vector3D up)
        {
            my_Type = CameraType.Fixed;
            my_StartPosition = position;
            my_StartTargetPosition = targetPosition;
            my_startUpDirection = up;
            my_MoveSpeed = 0.0;
            my_Position = my_StartPosition;
            my_TargetPosition = my_StartTargetPosition;
            my_UpDirection = my_startUpDirection;
            my_ViewDirection = my_StartTargetPosition - my_StartPosition;
            my_Distance = my_ViewDirection.Length;
            my_ViewDirection.Normalize();
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

        /// <summary>
        /// Not implemented for a Fixed Camera.
        /// </summary>
        /// <param name="amount"></param>
        public override void Vertical(double amount)
        {
            throw new NotImplementedException("A fixed Camera can not be modified.");
        }

        /// <summary>
        /// Not implemented for a Fixed Camera.
        /// </summary>
        /// <param name="amount"></param>
        public override void Horizontal(double amount)
        {
            throw new NotImplementedException("A fixed Camera can not be modified.");
        }

        /// <summary>
        /// Not implemented for a Fixed Camera.
        /// </summary>
        /// <param name="amount"></param>
        public override void Forward(double amount)
        {
            throw new NotImplementedException("A fixed Camera can not be modified.");
        }

        public override void UpdateDirection()
        {
            my_MoveSpeed = 0.0;
            my_Position = my_StartPosition;
            my_TargetPosition = my_StartTargetPosition;
            my_UpDirection = my_startUpDirection;
            my_ViewDirection = my_StartTargetPosition - my_StartPosition;
            my_Distance = my_ViewDirection.Length;
            my_ViewDirection.Normalize();
        }

        public override void UpdatePosition()
        {
            my_MoveSpeed = 0.0;
            my_Position = my_StartPosition;
            my_TargetPosition = my_StartTargetPosition;
            my_UpDirection = my_startUpDirection;
            my_ViewDirection = my_StartTargetPosition - my_StartPosition;
            my_Distance = my_ViewDirection.Length;
            my_ViewDirection.Normalize();
        }
    }
}
