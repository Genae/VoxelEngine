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

        public static Vector2 GetMousePosition(bool uiMode = false)
        {
            var p = Engine.Instance.PointToClient(new Point(_currentMouseState.X, _currentMouseState.Y));
            if(uiMode)
                return new Vector2(p.X, Engine.Instance.ClientRectangle.Height - p.Y);
            return new Vector2(p.X, p.Y);
        }

        public static Vector2 GetMouseDelta()
        {
            return new Vector2(_curRelMouseState.X - _lastRelMouseState.X, -1 * (_curRelMouseState.Y - _curRelMouseState.Y));
        }

        public static bool IsMouseInRect(int x, int y, int width, int height, bool uiMode = false)
        {
            var mPos = GetMousePosition(uiMode);
            var xEnd = x + width;
            var yEnd = y + width;
            if (mPos.X > x && mPos.X < xEnd && mPos.Y > y && mPos.Y < yEnd) return true;
            return false;
        }

        public static bool IsMouseInRect(Rectangle rect, bool uiMode = false)
        {
            return IsMouseInRect(rect.X, rect.Y, rect.Width, rect.Height, uiMode);
        }

        public static int GetMouseScroll()
        {
            return (int)_currentMouseState.Scroll.Y;
        }

        public static void UpdateInputFocus(GameWindow window)
        {
            var pi = (window.WindowInfo.GetType()).GetProperty("WindowHandle");
            var hnd = ((IntPtr)pi.GetValue(window.WindowInfo, null));
            var location = new Point(0,0);
            ClientToScreen(hnd, ref location);
            if (window.Focused)
            {
                RECT windowRect = window.ClientRectangle;
                windowRect.Right += location.X;
                windowRect.Left += location.X;
                windowRect.Top += location.Y;
                windowRect.Bottom += location.Y;
                ClipCursor(ref windowRect);
            }
            else
            {
                var rect = new RECT();
                ClipCursor(ref rect);
            }
        }

        #region Imports
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr SetCapture(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool ClipCursor(ref RECT rcClip);

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public static implicit operator Rectangle(RECT rect)
            {
                return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
            public static implicit operator RECT(Rectangle rect)
            {
                return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }
        #endregion

    }
}