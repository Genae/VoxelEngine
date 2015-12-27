using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using VoxelEngine.GameData;

namespace VoxelEngine.Camera
{
    public class Camera3D : GameObject
    {
        //Relative Directions
        public Vector3 Left => -Right;
        public Vector3 Right => new Vector3(_cameraMatrix.M11, _cameraMatrix.M21, _cameraMatrix.M31);
        public Vector3 Up => new Vector3(_cameraMatrix.M12, _cameraMatrix.M22, _cameraMatrix.M32);
        public Vector3 Down => -Up;
        public Vector3 Forward => -Backward;
        public Vector3 Backward => new Vector3(_cameraMatrix.M13, _cameraMatrix.M23, _cameraMatrix.M33);
        
        //Position and Rotation
        private Vector3 _cameraPos;
        public Vector3 CameraPos
        {
            get { return _cameraPos; }
            set
            {
                if (_cameraPos != value)
                {
                    _cameraPos = value;
                    RecalculateMatrix();
                }
            }
        }
        private float _facing;
        public float Facing
        {
            get { return _facing; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_facing != value)
                {
                    _facing = value;
                    RecalculateMatrix();
                }
            }
        }
        private float _pitch;
        public float Pitch
        {
            get { return _pitch; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_pitch != value)
                {
                    _pitch = value;
                    RecalculateMatrix();
                }
            }
        }

        //Other Properties
        public Frustum Frustum { get; }

        //internal
        private Matrix4 _cameraMatrix;

        public Camera3D()
        {
            _cameraPos = new Vector3(0f, 0f, -5f);
            Frustum = new Frustum();
            Engine.Instance.Cameras.Add(this);
        }

        public override void Destroy()
        {
            base.Destroy();
            Engine.Instance.Cameras.Remove(this);
        }
        
        public void OnRenderFrame(FrameEventArgs e)
        {
            RecalculateMatrix();
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref _cameraMatrix);
        }

        private void RecalculateMatrix()
        {
            var newForward = new Vector3((float)Math.Cos(_facing), _pitch, (float)Math.Sin(_facing));
            _cameraMatrix = Matrix4.LookAt(_cameraPos, _cameraPos + newForward, Vector3.UnitY);
            //_cameraMatrix = Matrix4.LookAt(_cameraPos, Vector3.Zero, Vector3.UnitY);

            Matrix4 projection;
            GL.GetFloat(GetPName.ProjectionMatrix, out projection);
            Matrix4 modelview;
            GL.GetFloat(GetPName.ModelviewMatrix, out modelview);
            Frustum.CalculateFrustum(projection, modelview);
        }
    }
}
