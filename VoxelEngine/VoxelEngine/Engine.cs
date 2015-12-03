﻿using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using VoxelEngine.Camera;
using VoxelEngine.GameData;
using VoxelEngine.GUI;
using VoxelEngine.Light;
using VoxelEngine.Shaders;

namespace VoxelEngine
{
    public class Engine : GameWindow
    {
        public static Engine Instance;
        private Matrix4 _matrixProjection;
        private int _timer, _counter;
        public Vector2 ScreenSize;
        public Vector2 ScreenPos;

        public List<Camera3D> Cameras = new List<Camera3D>();
        public List<Mesh> Meshes = new List<Mesh>();
        public List<Shader> Shaders = new List<Shader>();
        public List<LightSource> Lights = new List<LightSource>();

        private AwsomUI ui;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ui = new AwsomUI();
            OnResize(e);
            OnMove(e);
            //CursorVisible = false;

            //Settings
            VSync = VSyncMode.On;
            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); //PolygonMode important, MaterialFace.Front only renders front side?

            //light
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
            CountFrames(e);
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            Cameras[0].OnRenderFrame(e);
            
            foreach (var lightSource in Lights)
            {
                lightSource.OnRenderFrame(e);
            }
            foreach (var shader in Shaders)
            {
                shader.OnRenderFrame(e);
            }
            foreach (var mesh in Meshes)
            {
                mesh.ApplyFrustum(Cameras[0].Frustum);
                mesh.OnRenderFrame(e);
            }


            SetRenderUI(true);
            ui.OnRenderFrame(e);
            SetRenderUI(false);

            SwapBuffers();
        }

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
            _timer += (int) (1000*e.Time);
            if (_timer >= 1000)
            {
                Console.WriteLine((int)(_counter*(1000f/_timer)));
                ui.SetFPS((int)(_counter * (1000f / _timer)));
                _timer = 0;
                _counter = 0;
            }
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