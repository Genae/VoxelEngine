using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace VoxelEngine.GameData
{
    public class Chunk
    {
        public const int ChunkSize = 32;
        public Voxel[,,] Voxels;

        //drawing
        int _mVertexBuffer;
        int _mIndexBuffer;
        int _mColorBuffer;
        int _length;

        public Chunk()
        {
            Voxels = new Voxel[ChunkSize, ChunkSize, ChunkSize];
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        Voxels[x,y,z]  = new Voxel();
                    }
                }
            }
            /*Voxels[0,0,0]= new Voxel();
            Voxels[1, 0, 0] = new Voxel();
            Voxels[0, 1, 0] = new Voxel();
            Voxels[1, 1, 0] = new Voxel();*/
            OnChunkUpdated();
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _mColorBuffer);
            GL.ColorPointer(4, ColorPointerType.Float, 0, 0);
            GL.EnableClientState(ArrayCap.ColorArray);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _mVertexBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _mIndexBuffer);
            GL.DrawElements(BeginMode.Triangles, _length, DrawElementsType.UnsignedShort, 0);

            GL.Disable(EnableCap.VertexArray);
            GL.Disable(EnableCap.ColorArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        
        public void OnChunkUpdated()
        {
            ushort[] arrayElementBuffer;
            float[] arrayBuffer;
            float[] colors;
            CreateCubes(out arrayBuffer, out arrayElementBuffer, out colors);

            GL.GenBuffers(1, out _mVertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _mVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(BlittableValueType.StrideOf(arrayBuffer) * arrayBuffer.Length), arrayBuffer, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out _mIndexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _mIndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(ushort) * arrayElementBuffer.Length), arrayElementBuffer, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out _mColorBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _mColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(BlittableValueType.StrideOf(colors) * colors.Length), colors, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private void CreateCubes(out float[] arrayBuffer, out ushort[] arrayElementBuffer, out float[] color)
        {
            var vertices = new List<float>();
            var triangles = new List<ushort>();
            var colors = new List<float>();
            ushort offset = 0;
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        var voxel = Voxels[x, y, z];
                        if (voxel != null && voxel.IsActive)
                        {
                            vertices.AddRange(new []
                            {
                                -0.5f + x,  0.5f + y,  0.5f + z, // vertex[0]
			                     0.5f + x,  0.5f + y,  0.5f + z, // vertex[1]
			                     0.5f + x, -0.5f + y,  0.5f + z, // vertex[2]
			                    -0.5f + x, -0.5f + y,  0.5f + z, // vertex[3]
			                    -0.5f + x,  0.5f + y, -0.5f + z, // vertex[4]
			                     0.5f + x,  0.5f + y, -0.5f + z, // vertex[5]
			                     0.5f + x, -0.5f + y, -0.5f + z, // vertex[6]
			                    -0.5f + x, -0.5f + y, -0.5f + z  // vertex[7]
                            });
                            colors.AddRange(new []
                            {
                                1.0f, 0.0f, 0.0f, 1.0f,
                                0.0f, 1.0f, 0.0f, 1.0f,
                                0.0f, 0.0f, 1.0f, 1.0f,
                                0.0f, 1.0f, 1.0f, 1.0f,
                                1.0f, 0.0f, 0.0f, 1.0f,
                                0.0f, 1.0f, 0.0f, 1.0f,
                                0.0f, 0.0f, 1.0f, 1.0f,
                                0.0f, 1.0f, 1.0f, 1.0f,
                            });
                            triangles.AddRange(new []
                            {
                                (ushort)(1 + offset), (ushort)(0 + offset), (ushort)(2 + offset), // front
			                    (ushort)(3 + offset), (ushort)(2 + offset), (ushort)(0 + offset),
                                (ushort)(6 + offset), (ushort)(4 + offset), (ushort)(5 + offset), // back
			                    (ushort)(4 + offset), (ushort)(6 + offset), (ushort)(7 + offset),
                                (ushort)(4 + offset), (ushort)(7 + offset), (ushort)(0 + offset), // left
			                    (ushort)(7 + offset), (ushort)(3 + offset), (ushort)(0 + offset),
                                (ushort)(1 + offset), (ushort)(2 + offset), (ushort)(5 + offset), //right
			                    (ushort)(2 + offset), (ushort)(6 + offset), (ushort)(5 + offset),
                                (ushort)(0 + offset), (ushort)(1 + offset), (ushort)(5 + offset), // top
			                    (ushort)(0 + offset), (ushort)(5 + offset), (ushort)(4 + offset),
                                (ushort)(2 + offset), (ushort)(3 + offset), (ushort)(6 + offset), // bottom
			                    (ushort)(3 + offset), (ushort)(7 + offset), (ushort)(6 + offset),
                            });
                            offset += 8;
                        }
                    }
                }
            }
            arrayBuffer = vertices.ToArray();
            arrayElementBuffer = triangles.ToArray();
            _length = arrayElementBuffer.Length;
            color = colors.ToArray();
        }
    }
    struct Vertex
    {
        //public byte R, G, B, A;
        public float X, Y, Z;
        
        public Vertex(byte r, byte g, byte b, byte a, float x, float y, float z)
        {
            /*R = r;
            G = g;
            B = b;
            A = a;*/
            X = x;
            Y = y;
            Z = z;
        }
    }
}
