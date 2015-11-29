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
        public Map Map;
        private Matrix4 _matrixProjection;
        private int timer;

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
            Map = new Map(1);

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
            timer += (int)(1000*e.Time);
            if (timer >= 1000)
            {
                Console.WriteLine((int)(1 / e.Time));
                timer = 0;
            }
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Camera.OnRenderFrame(e);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill); //PolygonMode important, MaterialFace.Front only renders front side?
            Map.OnRenderFrame(e);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Keyboard[Key.Escape])
            {
                Exit();
            }

            Camera.OnUpdateFrame(e);
            
        }
    }
}