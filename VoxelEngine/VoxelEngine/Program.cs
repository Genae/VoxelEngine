using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using VoxelEngine.Camera;
using VoxelEngine.GameData;

namespace VoxelEngine
{
    public class Engine : GameWindow
    {
        public Camera3D Camera;
        public Chunk Chunk;
        private Matrix4 _matrixProjection;

        [STAThread]
        public static void Main()
        {
            using (var game = new Engine())
            {
                // Run the game at 60 updates per second
                game.Run(60);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Load stuff
            Camera = new Camera3D();
            Chunk = new Chunk();

            //Settings
            VSync = VSyncMode.On;
            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            _matrixProjection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1f, 100f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _matrixProjection);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Camera.OnRenderFrame(e);
            Chunk.OnRenderFrame(e);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Keyboard[Key.Escape])
            {
                Exit();
            }
        }
    }
}