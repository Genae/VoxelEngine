using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace VoxelEngine.GameData
{
    public class Chunk
    {
        public const int ChunkSize = 16;
        public float scale = 0.1f;
        public Voxel[,,] Voxels;
        public Vector3 Pos;

        //drawing
        int _mVertexBuffer;
        int _mIndexBuffer;
        int _mColorBuffer;
        int _length;
        private bool _loaded;
        private bool _active;

        public Chunk(Vector3 pos)
        {
            Pos = pos;
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
            var rand = new Random(0);
            for (int i = 0; i < 70; i++)
            {
                var x = rand.Next(Voxels.GetLength(0));
                var y = rand.Next(Voxels.GetLength(1));
                var z = rand.Next(Voxels.GetLength(2));
                Voxels[x, y, z].BlockType = 2;
            }
        }

        public void OnRenderFrame(FrameEventArgs e)
        {
            if (_length == 0 || !_active)
                return;
            if(!_loaded)
                OnChunkUpdated();
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

            if (arrayBuffer.Length == 0)
                return;
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
            _loaded = true;
        }

        private void CreateCubes(out float[] arrayBuffer, out ushort[] arrayElementBuffer, out float[] color)
        {
            var vertices = new List<float>();
            var triangles = new List<ushort>();
            var colors = new List<float>();

            var planes = new int[6, ChunkSize, ChunkSize, ChunkSize];
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
                            #region old
                            /*vertices.AddRange(new []
                            {
                                (-0.5f + x + Pos.X*ChunkSize)*scale, ( 0.5f + y + Pos.Y*ChunkSize)*scale, ( 0.5f + z + Pos.Z*ChunkSize)*scale, // vertex[0]
			                    ( 0.5f + x + Pos.X*ChunkSize)*scale, ( 0.5f + y + Pos.Y*ChunkSize)*scale, ( 0.5f + z + Pos.Z*ChunkSize)*scale, // vertex[1]
			                    ( 0.5f + x + Pos.X*ChunkSize)*scale, (-0.5f + y + Pos.Y*ChunkSize)*scale, ( 0.5f + z + Pos.Z*ChunkSize)*scale, // vertex[2]
			                    (-0.5f + x + Pos.X*ChunkSize)*scale, (-0.5f + y + Pos.Y*ChunkSize)*scale, ( 0.5f + z + Pos.Z*ChunkSize)*scale, // vertex[3]
			                    (-0.5f + x + Pos.X*ChunkSize)*scale, ( 0.5f + y + Pos.Y*ChunkSize)*scale, (-0.5f + z + Pos.Z*ChunkSize)*scale, // vertex[4]
			                    ( 0.5f + x + Pos.X*ChunkSize)*scale, ( 0.5f + y + Pos.Y*ChunkSize)*scale, (-0.5f + z + Pos.Z*ChunkSize)*scale, // vertex[5]
			                    ( 0.5f + x + Pos.X*ChunkSize)*scale, (-0.5f + y + Pos.Y*ChunkSize)*scale, (-0.5f + z + Pos.Z*ChunkSize)*scale, // vertex[6]
			                    (-0.5f + x + Pos.X*ChunkSize)*scale, (-0.5f + y + Pos.Y*ChunkSize)*scale, (-0.5f + z + Pos.Z*ChunkSize)*scale  // vertex[7]
                            });
                            colors.AddRange(new []
                            {
                                1.0f, 0.0f, 0.0f, 1.0f,
                                0.0f, 1.0f, 0.0f, 1.0f,
                                0.0f, 0.0f, 1.0f, 1.0f,
                                1.0f, 1.0f, 1.0f, 1.0f,
                                1.0f, 0.0f, 0.0f, 1.0f,
                                0.0f, 1.0f, 0.0f, 1.0f,
                                0.0f, 0.0f, 0.0f, 1.0f,
                                0.0f, 1.0f, 1.0f, 1.0f,
                            });

                            if (z == 0 || !Voxels[x, y, z - 1].IsActive)
                            {
                                triangles.AddRange(new[]
                                {
                                    (ushort)(6 + offset), (ushort)(4 + offset), (ushort)(5 + offset), // back
			                        (ushort)(4 + offset), (ushort)(6 + offset), (ushort)(7 + offset)
                                });
                            }
                            if (z == ChunkSize-1 || !Voxels[x, y, z + 1].IsActive)
                            {
                                triangles.AddRange(new[]
                                {
                                    (ushort) (1 + offset), (ushort) (0 + offset), (ushort) (2 + offset), // front
                                    (ushort) (3 + offset), (ushort) (2 + offset), (ushort) (0 + offset)
                                    
                                });
                            }
                            if (x == 0 || !Voxels[x-1, y, z].IsActive)
                            {
                                triangles.AddRange(new[]
                                {
                                    (ushort)(4 + offset), (ushort)(7 + offset), (ushort)(0 + offset), // left
			                        (ushort)(7 + offset), (ushort)(3 + offset), (ushort)(0 + offset),
                                });
                            }
                            if (x == ChunkSize - 1 || !Voxels[x+1, y, z].IsActive)
                            {
                                triangles.AddRange(new[]
                                {
                                    (ushort)(1 + offset), (ushort)(2 + offset), (ushort)(5 + offset), //right
			                        (ushort)(2 + offset), (ushort)(6 + offset), (ushort)(5 + offset),
                                });
                            }
                            if (y == 0 || !Voxels[x, y-1, z].IsActive)
                            {
                                triangles.AddRange(new[]
                                {
                                    (ushort)(2 + offset), (ushort)(3 + offset), (ushort)(6 + offset), // bottom
			                    (ushort)(3 + offset), (ushort)(7 + offset), (ushort)(6 + offset),
                                });
                            }
                            if (y == ChunkSize - 1 || !Voxels[x, y + 1, z].IsActive)
                            {
                                triangles.AddRange(new[]
                                {
                                    (ushort)(0 + offset), (ushort)(1 + offset), (ushort)(5 + offset), // top
			                        (ushort)(0 + offset), (ushort)(5 + offset), (ushort)(4 + offset),
                                });
                            }*/
                            #endregion

                            if (x == 0 || !Voxels[x - 1, y, z].IsActive) //+x left
                            {
                                planes[0, x, y, z] = Voxels[x, y, z].BlockType;
                            }
                            if (x == ChunkSize - 1 || !Voxels[x + 1, y, z].IsActive) //-x right
                            {
                                planes[1, x, y, z] = Voxels[x, y, z].BlockType;
                            }
                            if (y == 0 || !Voxels[x, y - 1, z].IsActive) //+y bottom
                            {
                                planes[2, x, y, z] = Voxels[x, y, z].BlockType;
                            }
                            if (y == ChunkSize - 1 || !Voxels[x, y + 1, z].IsActive) //-y top
                            {
                                planes[3, x, y, z] = Voxels[x, y, z].BlockType;
                            }
                            if (z == 0 || !Voxels[x, y, z - 1].IsActive) //+z back
                            {
                                planes[4, x, y, z] = Voxels[x, y, z].BlockType;
                            }
                            if (z == ChunkSize - 1 || !Voxels[x, y, z + 1].IsActive) //-z front
                            {
                                planes[5, x, y, z] = Voxels[x, y, z].BlockType;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 6; i++)
            {
                RunGreedyMeshing(planes, i, vertices, triangles, colors);
            }
            arrayBuffer = vertices.ToArray();
            arrayElementBuffer = triangles.ToArray();
            _length = arrayElementBuffer.Length;
            color = colors.ToArray();
        }

        private void RunGreedyMeshing(int[,,,] planes, int o, List<float> vertices, List<ushort> triangles, List<float> colors)
        {
            Rect curRectangle = null;
            var curType = 0;
            switch (o)
            {
                //x Axis
                case 0:
                case 1:
                    for (int x = 0; x < ChunkSize; x++)
                    {
                        for (int y = 0; y < ChunkSize; y++)
                        {
                            for (int z = 0; z < ChunkSize; z++)
                            {
                                var vox = planes[o, x, y, z];
                                if (vox != curType)
                                {
                                    if (curRectangle != null)
                                    {
                                        AddRect(curRectangle, o == 1, curType, vertices, triangles, colors);
                                        curRectangle = null;
                                    }
                                }
                                if (vox != 0)
                                {
                                    if (curRectangle != null)
                                    {
                                        curRectangle.Width++;
                                        curRectangle.WorldB.Z++;
                                        curRectangle.WorldD.Z++;
                                    }
                                    else
                                    {
                                        curRectangle = new Rect(x, y);
                                        curRectangle.WorldA = new Vector3(x - 0.5f + o, y + 0.5f, z - 0.5f);
                                        curRectangle.WorldB = new Vector3(x - 0.5f + o, y + 0.5f, z + 0.5f);
                                        curRectangle.WorldC = new Vector3(x - 0.5f + o, y - 0.5f, z - 0.5f);
                                        curRectangle.WorldD = new Vector3(x - 0.5f + o, y - 0.5f, z + 0.5f);
                                    }
                                }
                                if (z == ChunkSize - 1)
                                {
                                    if (curRectangle != null)
                                    {
                                        AddRect(curRectangle, o == 1, curType, vertices, triangles, colors);
                                    }
                                    curRectangle = null;
                                }
                                curType = vox;
                            }
                        }
                    }
                    break;
                //y Axis
                case 2:
                case 3:
                    for (int y = 0; y < ChunkSize; y++)
                    {
                        for (int x = 0; x < ChunkSize; x++)
                        {
                            for (int z = 0; z < ChunkSize; z++)
                            {
                                var vox = planes[o, x, y, z];
                                if (vox != curType)
                                {
                                    if (curRectangle != null)
                                    {
                                        AddRect(curRectangle, o == 2, curType, vertices, triangles, colors);
                                        curRectangle = null;
                                    }
                                }
                                if (vox != 0)
                                {
                                    if (curRectangle != null)
                                    {
                                        curRectangle.Width++;
                                        curRectangle.WorldB.Z++;
                                        curRectangle.WorldD.Z++;
                                    }
                                    else
                                    {
                                        curRectangle = new Rect(x, y);
                                        curRectangle.WorldA = new Vector3(x + 0.5f, y - 0.5f + o - 2, z - 0.5f);
                                        curRectangle.WorldB = new Vector3(x + 0.5f, y - 0.5f + o - 2, z + 0.5f);
                                        curRectangle.WorldC = new Vector3(x - 0.5f, y - 0.5f + o - 2, z - 0.5f);
                                        curRectangle.WorldD = new Vector3(x - 0.5f, y - 0.5f + o - 2, z + 0.5f);
                                    }
                                }
                                if (z == ChunkSize - 1)
                                {
                                    if (curRectangle != null)
                                    {
                                        AddRect(curRectangle, o == 2, curType, vertices, triangles, colors);
                                    }
                                    curRectangle = null;
                                }
                                curType = vox;
                            }
                        }
                    }
                    break;
                //z-Axis
                case 4:
                case 5:
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        for (int y = 0; y < ChunkSize; y++)
                        {
                            for (int x = 0; x < ChunkSize; x++)
                            {
                                var vox = planes[o, x, y, z];
                                if (vox != curType)
                                {
                                    if (curRectangle != null)
                                    {
                                        AddRect(curRectangle, o == 5, curType, vertices, triangles, colors);
                                        curRectangle = null;
                                    }
                                }
                                if (vox != 0)
                                {
                                    if (curRectangle != null)
                                    {
                                        curRectangle.Width++;
                                        curRectangle.WorldA.X++;
                                        curRectangle.WorldB.X++;
                                    }
                                    else
                                    {
                                        curRectangle = new Rect(x, y);
                                        curRectangle.WorldA = new Vector3(x + 0.5f, y - 0.5f, z - 0.5f + o - 4);
                                        curRectangle.WorldB = new Vector3(x + 0.5f, y + 0.5f, z - 0.5f + o - 4);
                                        curRectangle.WorldC = new Vector3(x - 0.5f, y - 0.5f, z - 0.5f + o - 4);
                                        curRectangle.WorldD = new Vector3(x - 0.5f, y + 0.5f, z - 0.5f + o - 4);
                                    }
                                }
                                if (x == ChunkSize - 1)
                                {
                                    if (curRectangle != null)
                                    {
                                        AddRect(curRectangle, o == 5, curType, vertices, triangles, colors);
                                    }
                                    curRectangle = null;
                                }
                                curType = vox;
                            }
                        }
                    }
                    break;
            }
            
        }

        private void AddRect(Rect curRectangle, bool front, int curType, List<float> vertices, List<ushort> triangles, List<float> colors)
        {
            var offset = vertices.Count/3;
            vertices.AddRange(new[]
            {
                (curRectangle.WorldA.X + Pos.X*ChunkSize)*scale, (curRectangle.WorldA.Y + Pos.Y*ChunkSize)*scale, (curRectangle.WorldA.Z + Pos.Z*ChunkSize)*scale, // vertex[0]
			    (curRectangle.WorldB.X + Pos.X*ChunkSize)*scale, (curRectangle.WorldB.Y + Pos.Y*ChunkSize)*scale, (curRectangle.WorldB.Z + Pos.Z*ChunkSize)*scale, // vertex[1]
			    (curRectangle.WorldC.X + Pos.X*ChunkSize)*scale, (curRectangle.WorldC.Y + Pos.Y*ChunkSize)*scale, (curRectangle.WorldC.Z + Pos.Z*ChunkSize)*scale, // vertex[2]
			    (curRectangle.WorldD.X + Pos.X*ChunkSize)*scale, (curRectangle.WorldD.Y + Pos.Y*ChunkSize)*scale, (curRectangle.WorldD.Z + Pos.Z*ChunkSize)*scale, // vertex[3]
            });
            colors.AddRange(new[]
            {
                curType-1.0f, curType-1.0f, curType-1.0f, curType-1.0f,
                curType-1.0f, curType-1.0f, curType-1.0f, curType-1.0f,
                curType-1.0f, curType-1.0f, curType-1.0f, curType-1.0f,
                curType-1.0f, curType-1.0f, curType-1.0f, curType-1.0f,
            });
            if (front)
            {
                triangles.AddRange(new[]
                {
                    (ushort) (0 + offset), (ushort) (1 + offset), (ushort) (3 + offset),
                    (ushort) (0 + offset), (ushort) (3 + offset), (ushort) (2 + offset)
                });
            }
            else
            {
                triangles.AddRange(new[]
                {
                    (ushort) (1 + offset), (ushort) (0 + offset), (ushort) (3 + offset),
                    (ushort) (3 + offset), (ushort) (0 + offset), (ushort) (2 + offset)
                });
            }
            
        }

        public bool HasSolidBorder(int dir)
        {
            switch (dir)
            {
                case 1: //+x
                    for (var y = 0; y < ChunkSize; y++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            if (!Voxels[ChunkSize - 1, y, z].IsActive)
                                return false;
                        }
                    }
                    return true;
                case 2: //-x
                    for (var y = 0; y < ChunkSize; y++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            if (!Voxels[0, y, z].IsActive)
                                return false;
                        }
                    }
                    return true;
                case 3: //+y
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            if (!Voxels[x, ChunkSize - 1, z].IsActive)
                                return false;
                        }
                    }
                    return true;
                case 4: //-y
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var z = 0; z < ChunkSize; z++)
                        {
                            if (!Voxels[x, 0, z].IsActive)
                                return false;
                        }
                    }
                    return true;
                case 5: //+z
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var y = 0; y < ChunkSize; y++)
                        {
                            if (!Voxels[x, y, ChunkSize - 1].IsActive)
                                return false;
                        }
                    }
                    return true;
                case 6: //-z
                    for (var x = 0; x < ChunkSize; x++)
                    {
                        for (var y = 0; y < ChunkSize; y++)
                        {
                            if (!Voxels[x, y, 0].IsActive)
                                return false;
                        }
                    }
                    return true;
            }
            return false;
        }

        public void SetActive(bool a)
        {
            if (a == _active)
                return;
            _active = a;
            if(_active)
                OnChunkUpdated();
            else
                Unload();
        }

        public void Unload()
        {
            if(!_loaded)
                return;
            GL.DeleteBuffers(1, ref _mVertexBuffer);
            GL.DeleteBuffers(1, ref _mIndexBuffer);
            GL.DeleteBuffers(1, ref _mColorBuffer);
        }
    }

    class Rect
    {
        public int X, Y;
        public int Width, Height;
        public Vector3 WorldA, WorldB, WorldC, WorldD;
        
        public Rect(int x, int y)
        {
            X = x;
            Y = y;
            Width = 1;
            Height = 1;
        }
    }
}
