using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using VoxelEngine.Camera;
using VoxelEngine.Shaders;

namespace VoxelEngine.GameData
{
    public abstract class Mesh : GameObject
    {
        //
        public Vector3 Pos;
        public float Scale = 0.1f;
        public float Size;
        public Shader Shader;

        //drawing
        protected int MVertexBuffer;
        protected int MIndexBuffer;
        protected int MColorBuffer;
        protected int MNormalBuffer;
        protected int Length;
        protected bool Loaded;
        protected bool Active;
        public bool Visible;

        protected Mesh(float size, Vector3 pos)
        {
            Size = size;
            Pos = pos;
            Engine.Instance.Meshes.Add(this);
        }

        protected Mesh(float size, Vector3 pos, bool autoDraw)
        {
            Size = size;
            Pos = pos;
            if(autoDraw)
                Engine.Instance.Meshes.Add(this);
        }

        public override void Destroy()
        {
            base.Destroy();
            Engine.Instance.Meshes.Remove(this);
        }
        
        public virtual void OnRenderFrame(FrameEventArgs e)
        {
            Render(true);
        }

        public void Render(bool ownShader)
        {
            if (Length == 0 || !Active || !Visible)
                return;
            if (!Loaded)
                Load();

            if(ownShader) Shader.Bind();
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MColorBuffer);
            GL.ColorPointer(4, ColorPointerType.Float, 0, 0);


            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MVertexBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);

            GL.EnableClientState(ArrayCap.NormalArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MNormalBuffer);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, 0);


            GL.BindBuffer(BufferTarget.ElementArrayBuffer, MIndexBuffer);
            GL.DrawElements(BeginMode.Triangles, Length, DrawElementsType.UnsignedShort, 0);

            GL.Disable(EnableCap.VertexArray);
            GL.Disable(EnableCap.ColorArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            if (ownShader) Shader.Unbind();
        }

        public void RenderLines(bool ownShader)
        {
            if (Length == 0 || !Active || !Visible)
                return;
            if (!Loaded)
                Load();

            if (ownShader) Shader.Bind();
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MColorBuffer);
            GL.ColorPointer(4, ColorPointerType.Float, 0, 0);


            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MVertexBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);

            GL.EnableClientState(ArrayCap.NormalArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MNormalBuffer);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, 0);


            GL.BindBuffer(BufferTarget.ElementArrayBuffer, MIndexBuffer);
            GL.DrawElements(BeginMode.Lines, Length, DrawElementsType.UnsignedShort, 0);

            GL.Disable(EnableCap.VertexArray);
            GL.Disable(EnableCap.ColorArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            if (ownShader) Shader.Unbind();
        }

        protected void CreateMesh(float[] vertecies, ushort[] triangles, float[] colors, float[] normals)
        {
            if (vertecies.Length == 0)
                return;
            Length = triangles.Length;
            GL.GenBuffers(1, out MVertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(BlittableValueType.StrideOf(vertecies) * vertecies.Length),
                vertecies, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out MIndexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, MIndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(ushort) * triangles.Length), triangles,
                BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out MColorBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(BlittableValueType.StrideOf(colors) * colors.Length), colors,
                BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out MNormalBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, MNormalBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(BlittableValueType.StrideOf(normals) * normals.Length), normals,
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            Loaded = true;
        }

        protected void CreateCube()
        {
            var x = Pos.X * Size;
            var y = Pos.Y * Size;
            var z = Pos.Z * Size;
            float[] vertecies = {
                (x - 0.5f) * Scale, (y - 0.5f) * Scale, (z - 0.5f) *Scale,
                (x - 0.5f) * Scale, (y - 0.5f + Size) * Scale, (z - 0.5f) *Scale,
                (x - 0.5f) * Scale, (y - 0.5f + Size) * Scale, (z - 0.5f + Size) *Scale,
                (x - 0.5f) * Scale, (y - 0.5f) * Scale, (z - 0.5f + Size) *Scale,
                (x - 0.5f + Size) * Scale, (y - 0.5f) * Scale, (z - 0.5f) * Scale,
                (x - 0.5f + Size) * Scale, (y - 0.5f + Size) * Scale, (z - 0.5f) * Scale,
                (x - 0.5f + Size) * Scale, (y - 0.5f + Size) * Scale, (z - 0.5f + Size) * Scale,
                (x - 0.5f + Size) * Scale, (y - 0.5f) * Scale, (z - 0.5f + Size) * Scale,
            };
            ushort[] lines =
            {
                0, 1,
                1, 2,
                2, 3,
                0, 4,
                0, 3,
                1, 5,
                3, 7,
                2, 6,
                6, 7,
                7, 4,
                4, 5,
                5, 6
            };
            float[] color = {
                1, 0, 0, 1,
                1, 0, 0, 1,
                1, 0, 0, 1,
                1, 0, 0, 1,
                1, 0, 0, 1,
                1, 0, 0, 1,
                1, 0, 0, 1,
                1, 0, 0, 1,
            };
            float[] normals = {
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
            };
            CreateMesh(vertecies, lines, color, normals);
        }

        public virtual void ApplyFrustum(Frustum frustum)
        {
            Visible = frustum.SphereVsFrustum(new Vector3(Pos.X + 0.5f*Size, Pos.Y + 0.5f*Size, Pos.Z + 0.5f*Size), Size);
        }

        public virtual void SetActive(bool a)
        {
            if (a == Active)
                return;
            Active = a;
            if (Active)
                Load();
            else
                Unload();
        }

        public void Unload()
        {
            if (!Loaded)
                return;
            GL.DeleteBuffers(1, ref MVertexBuffer);
            GL.DeleteBuffers(1, ref MIndexBuffer);
            GL.DeleteBuffers(1, ref MColorBuffer);
            GL.DeleteBuffers(1, ref MNormalBuffer);
        }

        protected abstract void Load();
    }
}