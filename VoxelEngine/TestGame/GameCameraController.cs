using System;
using OpenTK;
using OpenTK.Input;
using VoxelEngine;
using VoxelEngine.Camera;

namespace TestGame
{
    //should be in the game, not in the engine!
    public class GameCameraController
    {
        public Camera3D Camera;
        private Vector2 _curMousePosition, _pastMousePosition, _mouseDelta, _mouseSpeed;
        private float _cameraSpeed = 5, _rotationSpeed = 0.01f;

        public GameCameraController()
        {
            Camera = new Camera3D();
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            Camera.OnRenderFrame(e);
        }

        public void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard[Key.W])
            {
                Camera.CameraPos += Camera.Forward * _cameraSpeed * (float)e.Time;
            }

            if (keyboard[Key.S])
            {
                Camera.CameraPos += Camera.Backward * _cameraSpeed * (float)e.Time;
            }

            if (keyboard[Key.A])
            {
                Camera.CameraPos += Camera.Left * _cameraSpeed * (float)e.Time;
            }

            if (keyboard[Key.D])
            {
                Camera.CameraPos += Camera.Right * _cameraSpeed * (float)e.Time;
            }
            if (keyboard[Key.Space])
            {
                Camera.CameraPos += Camera.Up * _cameraSpeed * (float)e.Time;
            }

            if (keyboard[Key.C])
            {
                Camera.CameraPos += Camera.Down * _cameraSpeed * (float)e.Time;
            }


            var mouse = Mouse.GetState();
            _curMousePosition.X = mouse.X;
            _curMousePosition.Y = mouse.Y;

            _mouseDelta.X = _curMousePosition.X - _pastMousePosition.X;
            _mouseDelta.Y = -1 * (_curMousePosition.Y - _pastMousePosition.Y);
            Mouse.SetPosition(Engine.Instance.ScreenSize.X/2 + Engine.Instance.ScreenPos.X, Engine.Instance.ScreenSize.Y/2 + Engine.Instance.ScreenPos.Y);

            if (mouse[MouseButton.Right])
            {
                Console.WriteLine(_mouseDelta);
            }
            
            _pastMousePosition = _curMousePosition;

            Camera.Facing += _mouseDelta[0] * _rotationSpeed;
            Camera.Pitch += _mouseDelta[1] * _rotationSpeed;


        }
    }
}

