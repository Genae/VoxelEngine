using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using VoxelEngine.Shaders;
using VoxelEngine.Shaders.ShadowMap;

namespace VoxelEngine.Light
{
    public class DirectionalLight : LightSource
    {
        private Shader _shader;
        private int _bufferId, _dTexId;
        public DirectionalLight(Vector3 dir) : base(new Vector4(dir.X, dir.Y, dir.Z, 0.0f))
        {
            AllocateBuffers();
            _shader = new ShadowMap();
        }

        private void AllocateBuffers()
        {
            GL.GenTextures(1, out _dTexId);
            GL.BindTexture(TextureTarget.Texture2D, _dTexId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent16, 1024, 1024, 0, PixelFormat.DepthComponent, PixelType.Float, new IntPtr());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.GenFramebuffers(1, out _bufferId);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _bufferId);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, _dTexId, 0);

            GL.DrawBuffer(DrawBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("Som Ting Wong");
                return;
            }
        }

        protected override void GenerateShadowMap()
        {
            base.GenerateShadowMap();
            
            var depthProjectionMatrix = Matrix4.CreateOrthographic(20, 20, -10, 20);
            var depthViewMatrix = Matrix4.LookAt(Position.Xyz, Vector3.Zero, Vector3.UnitY);
            var depthModelMatrix = Matrix4.Identity;
            var depthMVP = depthProjectionMatrix*depthViewMatrix*depthModelMatrix;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _bufferId);
            GL.Viewport(0,0,1024,1024);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            _shader.Bind();
            _shader.SetVariable("depthMVP", depthMVP);
            foreach (var mesh in Engine.Instance.Meshes)
            {
                mesh.Render(false);
            }
            _shader.Unbind();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }
    }
}
