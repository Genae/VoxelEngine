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
        int m_vertexBuffer = 0;
        int m_indexBuffer = 0;
        int length;

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
            OnChunkUpdated();
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_indexBuffer);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);

            GL.DrawElements(BeginMode.TriangleStrip, length, DrawElementsType.UnsignedShort, 0);

            GL.Disable(EnableCap.VertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        
        public void OnChunkUpdated()
        {
            ushort[] arrayElementBuffer;
            Vertex[] arrayBuffer;
            CreateCubes(out arrayBuffer, out arrayElementBuffer);

            GL.GenBuffers(1, out m_vertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBuffer);
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vertex));
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(size * arrayBuffer.Length), arrayBuffer, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out m_indexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_indexBuffer);

            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(ushort) * arrayElementBuffer.Length), arrayElementBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private void CreateCubes(out Vertex[] arrayBuffer, out ushort[] arrayElementBuffer)
        {
            var vertices = new List<Vertex>();
            var triangles = new List<ushort>();
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
                                new Vertex(255,0,0,255,-0.5f + x,  0.5f + y,  0.5f + z), // vertex[0]
			                    new Vertex(0,255,0,255, 0.5f + x,  0.5f + y,  0.5f + z), // vertex[1]
			                    new Vertex(0,0,255,255, 0.5f + x, -0.5f + y,  0.5f + z), // vertex[2]
			                    new Vertex(255,255,0,255,-0.5f + x, -0.5f + y,  0.5f + z), // vertex[3]
			                    new Vertex(255,0,255,255,-0.5f + x,  0.5f + y, -0.5f + z), // vertex[4]
			                    new Vertex(0,255,255,255, 0.5f + x,  0.5f + y, -0.5f + z), // vertex[5]
			                    new Vertex(255,255,255,255, 0.5f + x, -0.5f + y, -0.5f + z), // vertex[6]
			                    new Vertex(0,0,0,255,-0.5f + x, -0.5f + y, -0.5f + z)  // vertex[7]
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
            length = arrayElementBuffer.Length;
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
