using System;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Input;

namespace VoxelEngine.Input
{
    public static class Input
    {
        private static KeyboardState _currentState, _lastState;
        private static MouseState _currentMouseState, _lastMouseState, _curRelMouseState, _lastRelMouseState;

        public static void OnUpdateFrame(FrameEventArgs e)
        {
            _lastState = _currentState;
            _currentState = Keyboard.GetState();

            _lastMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetCursorState();

            _lastRelMouseState = _curRelMouseState;
            _curRelMouseState = Mouse.GetState();
        }

        public static bool GetKey(Key key)
        {
            return _currentState[key];
        }

        public static bool GetKeyDown(Key key)
        {
            return _currentState[key] && !_lastState[key];
        }

        public static bool GetKeyUp(Key key)
        {
            return !_currentState[key] && _lastState[key];
        }

        public static bool GetMouseButton(MouseButton button)
        {
            return _currentMouseState[button];
        }

        public static bool GetMouseButtonDown(MouseButton button)
        {
            return _currentMouseState[button] && !_lastMouseState[button];
        }

        public static bool GetMouseButtonUp(MouseButton button)
        {
            return !_currentMouseState[button] && _lastMouseState[button];
        }

        public static Vector2 GetMousePosition()
        {
            return new Vector2(_currentMouseState.X, _currentMouseState.Y);
        }

        public static Vector2 GetMouseDelta()
        {
            return new Vector2(_curRelMouseState.X - _lastRelMouseState.X, -1 * (_curRelMouseState.Y - _curRelMouseState.Y));
        }

        public static bool IsMouseInRect(int x, int y, int width, int height)
        {
            var mPos = GetMousePosition();
            var xEnd = x + width;
            var yEnd = y + width;
            if (mPos.X > x && mPos.X < xEnd && mPos.Y > y && mPos.Y < yEnd) return true;
            return false;
        }

        public static bool IsMouseInRect(Rectangle rect)
        {
            return IsMouseInRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static int GetMouseScroll()
        {
            return (int)_currentMouseState.Scroll.Y;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetCapture(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool ReleaseCapture();
    }
}