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
        public static Engine Instance;
        public GameCameraController Camera;
        public Map Map;
        private Matrix4 _matrixProjection;
        private int _timer, _counter;
        public static Vector2 ScreenSize;
        public static Vector2 ScreenPos;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            OnResize(e);
            OnMove(e);
            CursorVisible = false;
            // Load stuff
            Camera = new GameCameraController();
            Map = new Map(16, 4);

            //Settings
            VSync = VSyncMode.On;
            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            //light
            float[] mat_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] mat_shininess = { 50.0f };
            float[] light_position = { 1.0f, 1.0f, 1.0f, 0.0f };
            float[] light_ambient = { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] lmodel_ambient = { 0.7f, 0.2f, 0.2f, 1.0f };

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.ShadeModel(ShadingModel.Smooth);

            GL.Material(MaterialFace.Front, MaterialParameter.Specular, mat_specular);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, mat_shininess);
            GL.Light(LightName.Light0, LightParameter.Position, light_position);
            GL.Light(LightName.Light0, LightParameter.Ambient, light_ambient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, mat_specular);
            GL.LightModel(LightModelParameter.LightModelAmbient, lmodel_ambient);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            _matrixProjection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1f, 100f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _matrixProjection);
            ScreenSize = new Vector2(Width, Height);
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            ScreenPos = new Vector2(X, Y);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _counter++;
            _timer += (int)(1000*e.Time);
            if (_timer >= 1000)
            {
                Console.WriteLine(_counter);
                _timer = 0;
                _counter = 0;
            }
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Camera.OnRenderFrame(e);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); //PolygonMode important, MaterialFace.Front only renders front side?
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