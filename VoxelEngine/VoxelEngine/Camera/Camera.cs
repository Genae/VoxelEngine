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
        private float _cameraSpeed = 5, _facing, _pitch, _rotationSpeed = 0.01f;
        private Vector3 _cameraPos, _cameraForward;
        private Vector2 _curMousePosition, _pastMousePosition, _mouseDelta, _mouseSpeed;

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
            Vector3 lookatPoint = new Vector3((float)Math.Cos(_facing), _pitch, (float)Math.Sin(_facing));
            cameraMatrix = Matrix4.LookAt(_cameraPos, _cameraPos  + lookatPoint, Vector3.UnitY);
            
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

            if (keyboard[Key.S])
            {
                _cameraPos += new Vector3(0f, 0f, -_cameraSpeed * (float)e.Time);
            }

            if (keyboard[Key.A])
            {
                _cameraPos += new Vector3(_cameraSpeed * (float)e.Time, 0f, 0f);
            }

            if (keyboard[Key.D])
            {
                _cameraPos += new Vector3(-_cameraSpeed * (float)e.Time, 0f, 0f);
            }
            if (keyboard[Key.Space])
            {
                _cameraPos += new Vector3(0f, _cameraSpeed * (float)e.Time, 0f);
            }

            if (keyboard[Key.C])
            {
                _cameraPos += new Vector3(0f, -_cameraSpeed * (float)e.Time, 0f);
            }


            var mouse = Mouse.GetState();
            _curMousePosition.X = mouse.X;
            _curMousePosition.Y = mouse.Y;

            _mouseDelta.X = _curMousePosition.X - _pastMousePosition.X;
            _mouseDelta.Y = -1 * (_curMousePosition.Y - _pastMousePosition.Y);

            if (mouse[MouseButton.Right])
            {
                Console.WriteLine(_mouseDelta);
            }

            /*_mouseSpeed.X += _mouseDelta.X;
            _mouseSpeed.Y += _mouseDelta.Y;
            */
            _pastMousePosition = _curMousePosition;

            _facing += _mouseDelta[0] * _rotationSpeed;
            _pitch += _mouseDelta[1] * _rotationSpeed;
            

        }
    }
}
