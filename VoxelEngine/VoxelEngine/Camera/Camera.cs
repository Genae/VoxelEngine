using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace VoxelEngine.Camera
{
    public class Camera3D
    {
        public float CameraYRotation;

        private Matrix4 cameraMatrix;
        private float _cameraSpeed = 5;
        private Vector3 _cameraPos, _cameraForward;

        public Camera3D()
        {
            //cameraMatrix *= Matrix4.LookAt(0f, 1f, -5f, 0f, 0f, 0f, 0f, 1f, 0f);
            _cameraForward = Vector3.UnitZ;
            _cameraPos = new Vector3(0f, 0f, -5f);
        }
        
        public void OnRenderFrame(FrameEventArgs e)
        {
            //CameraYRotation = (CameraYRotation < 360f) ? (CameraYRotation + 0.1f * (float)e.Time) : 0f;
            //Matrix4.CreateRotationY(CameraYRotation, out cameraMatrix);
            cameraMatrix = Matrix4.LookAt(_cameraPos, _cameraPos + _cameraForward, Vector3.UnitY);
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref cameraMatrix);
        }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard[Key.W])
            {
                _cameraPos += new Vector3(0f, 0f, _cameraSpeed * (float)e.Time);
            }

            if (Keyboard.GetState()[Key.S])
            {
                _cameraPos += new Vector3(0f, 0f, -_cameraSpeed * (float)e.Time);
            }

            if (Keyboard.GetState()[Key.A])
            {
                _cameraPos += new Vector3(_cameraSpeed * (float)e.Time, 0f, 0f);
            }

            if (Keyboard.GetState()[Key.D])
            {
                _cameraPos += new Vector3(-_cameraSpeed * (float)e.Time, 0f, 0f);
            }
            if (Keyboard.GetState()[Key.Space])
            {
                _cameraPos += new Vector3(0f, _cameraSpeed * (float)e.Time, 0f);
            }

            if (Keyboard.GetState()[Key.C])
            {
                _cameraPos += new Vector3(0f, -_cameraSpeed * (float)e.Time, 0f);
            }


            /*
            var mouse = Mouse.GetState();
            _curMousePosition.X = mouse.X;
            _curMousePosition.Y = mouse.Y;

            _mouseDelta.X = _curMousePosition.X - _pastMousePosition.X;
            _mouseDelta.Y = -1 * (_curMousePosition.Y - _pastMousePosition.Y);

            if (mouse[MouseButton.Right])
            {
                Console.WriteLine(_mouseDelta);
            }

            _mouseSpeed.X += _mouseDelta.X;
            _mouseSpeed.Y += _mouseDelta.Y;

            _pastMousePosition = _curMousePosition;


            cameraMatrix = Matrix4.Mult(cameraMatrix, Matrix4.CreateRotationY(_mouseSpeed.X * (float)e.Time));
            cameraMatrix = Matrix4.Mult(cameraMatrix, Matrix4.CreateRotationX(_mouseSpeed.Y * (float)e.Time));*/
        }
    }
}
