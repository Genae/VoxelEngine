using System;
using OpenTK;
using OpenTK.Input;

namespace VoxelEngine.Camera
{
    //should be in the game, not in the engine!
    public class GameCameraController
    {
        private Camera3D _camera;
        private Vector2 _curMousePosition, _pastMousePosition, _mouseDelta, _mouseSpeed;
        private float _cameraSpeed = 5, _rotationSpeed = 0.01f;

        public GameCameraController()
        {
            _camera = new Camera3D();
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            _camera.OnRenderFrame(e);
        }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard[Key.W])
            {
                _camera.CameraPos += _camera.Forward * _cameraSpeed * (float)e.Time;
            }

            if (keyboard[Key.S])
            {
                _camera.CameraPos += _camera.Backward * _cameraSpeed * (float)e.Time;
            }

            if (keyboard[Key.A])
            {
                _camera.CameraPos += _camera.Left * _cameraSpeed * (float)e.Time;
            }

            if (keyboard[Key.D])
            {
                _camera.CameraPos += _camera.Right * _cameraSpeed * (float)e.Time;
            }
            if (keyboard[Key.Space])
            {
                _camera.CameraPos += _camera.Up * _cameraSpeed * (float)e.Time;
            }

            if (keyboard[Key.C])
            {
                _camera.CameraPos += _camera.Down * _cameraSpeed * (float)e.Time;
            }


            var mouse = Mouse.GetState();
            _curMousePosition.X = mouse.X;
            _curMousePosition.Y = mouse.Y;

            _mouseDelta.X = _curMousePosition.X - _pastMousePosition.X;
            _mouseDelta.Y = -1 * (_curMousePosition.Y - _pastMousePosition.Y);
            Mouse.SetPosition(Engine.ScreenSize.X/2 + Engine.ScreenPos.X, Engine.ScreenSize.Y/2 + Engine.ScreenPos.Y);

            if (mouse[MouseButton.Right])
            {
                Console.WriteLine(_mouseDelta);
            }

            /*_mouseSpeed.X += _mouseDelta.X;
            _mouseSpeed.Y += _mouseDelta.Y;
            */
            _pastMousePosition = _curMousePosition;

            _camera.Facing += _mouseDelta[0] * _rotationSpeed;
            _camera.Pitch += _mouseDelta[1] * _rotationSpeed;


        }
    }
}
