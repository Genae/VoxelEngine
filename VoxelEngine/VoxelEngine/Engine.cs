using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using Awesomium.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using VoxelEngine.Camera;
using VoxelEngine.GameData;
using VoxelEngine.GUI;
using VoxelEngine.Light;
using VoxelEngine.Shaders;
using VoxelEngine.Physics;
using FrameEventArgs = OpenTK.FrameEventArgs;

namespace VoxelEngine
{
    public class Engine : GameWindow
    {
        public static Engine Instance;
        private Matrix4 _matrixProjection;
        private int _timer, _counter;
        private bool _wireframe;
        private bool _chunks;
        public Vector2 ScreenSize;
        public Vector2 ScreenPos;

        public List<Camera3D> Cameras = new List<Camera3D>();
        public List<Mesh> Meshes = new List<Mesh>();
        public List<Shader> Shaders = new List<Shader>();
        public List<LightSource> Lights = new List<LightSource>();
        public List<Collider> Collider = new List<Collider>();

        public List<UiElement> UIElements = new List<UiElement>();

        public Thread WebThread;
        public static object Lock = new object();

        #region OnStuff
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            OnFocusedChanged(e);
            OnResize(e);
            OnMove(e);
            //CursorVisible = false;

            //Settings
            VSync = VSyncMode.On;
            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            
            //light
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            WebThread.Abort();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            _matrixProjection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1f, 100f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _matrixProjection);
            ScreenSize = new Vector2(Width, Height);
            Input.Input.UpdateInputFocus(this);
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            ScreenPos = new Vector2(X, Y);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            CountFrames(e);
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (_wireframe)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); //PolygonMode important, MaterialFace.Front only renders front side?
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill); //PolygonMode important, MaterialFace.Front only renders front side?
            }
            /*foreach (var lightSource in Lights)
            {
                lightSource.OnRenderFrame(e);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            }*/

            Cameras[0].OnRenderFrame(e);
            foreach (var shader in Shaders)
            {
                shader.OnRenderFrame(e);
            }
            foreach (var mesh in Meshes)
            {
                mesh.ApplyFrustum(Cameras[0].Frustum);
                mesh.OnRenderFrame(e);
            }
            if (_chunks)
            {
                GL.Disable(EnableCap.Lighting);
                GL.LineWidth(4);
                foreach (var mesh in Meshes)
                {
                    if (!(mesh is Chunk))
                        continue;
                    ((Chunk)mesh).ChunkBorders.ApplyFrustum(Cameras[0].Frustum);
                    ((Chunk)mesh).ChunkBorders.OnRenderFrame(e);
                }
                GL.LineWidth(1);
                GL.Enable(EnableCap.Lighting);
            }

            SetRenderUI(true);
            foreach (var uiElement in UIElements)
            {
                uiElement.OnRenderFrame(e);
            }
            SetRenderUI(false);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            Input.Input.OnUpdateFrame(e);

            foreach (var uiElement in UIElements)
            {
                uiElement.OnUpdateFrame(e);
            }

            //Listen to KeyEvents
            if (Keyboard[Key.Escape])
            {
                Exit();
            }
            if (Input.Input.GetKeyDown(Key.F11))
            {
                ToggleFullscreen();
            }
            if (Input.Input.GetKeyDown(Key.F12))
            {
                ToggleWireframe();
            }
            if (Input.Input.GetKeyDown(Key.F10))
            {
                ToggleChunks();
            }
        }

        private void ToggleChunks()
        {
            _chunks = !_chunks;
        }

        private void ToggleWireframe()
        {
            _wireframe = !_wireframe;
        }

        private void ToggleFullscreen()
        {
            if (WindowState != WindowState.Fullscreen)
            {
                WindowBorder = WindowBorder.Hidden;
                WindowState = WindowState.Fullscreen;
                Input.Input.UpdateInputFocus(this);
            }
            else
            {
                WindowBorder = WindowBorder.Resizable;
                WindowState = WindowState.Normal;
                Input.Input.UpdateInputFocus(this);
            }
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            base.OnFocusedChanged(e);
            Input.Input.UpdateInputFocus(this);
        }
        #endregion

        #region Helpers
        private void SetRenderUI(bool ui)
        {
            if (ui)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix();
                GL.LoadIdentity();


                GL.Ortho(0, Width, 0, Height, -1, 1);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                GL.LoadIdentity();
                GL.PushAttrib(AttribMask.DepthBufferBit | AttribMask.LightingBit);

                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.CullFace);
                GL.Disable(EnableCap.Lighting);

                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            }
            else
            {
                GL.PopAttrib();
                GL.MatrixMode(MatrixMode.Projection);
                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PopMatrix();
                GL.Disable(EnableCap.Blend);
            }
        }

        private void CountFrames(FrameEventArgs e)
        {
            _counter++;
            _timer += (int)(1000 * e.Time);
            if (_timer >= 1000)
            {
                Console.WriteLine((int)(_counter * (1000f / _timer)));
                _timer = 0;
                _counter = 0;
            }
        }

        private void RunWebCore()
        {
            WebCore.Initialize(new WebConfig()
            {
                LogPath = Environment.CurrentDirectory + "/awesomium.log",
                LogLevel = LogLevel.Verbose,
            });
            WebCore.CreateWebSession(new WebPreferences() { CustomCSS = "::-webkit-scrollbar { visibility: hidden; }" });
            WebCore.Run();
        }

        public void EnsureWebCore()
        {
            lock (Lock)
            {
                if (!WebCore.IsInitialized)
                {
                    WebThread = new Thread(RunWebCore);
                    WebThread.Start();
                }
            }
        }
        #endregion

    }
}