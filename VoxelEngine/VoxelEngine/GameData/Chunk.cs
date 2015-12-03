using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using VoxelEngine.Shaders.DirectionalDiffuse;

namespace VoxelEngine.GameData
{
    public class Chunk : Mesh
    {
        public const int ChunkSize = 16;
        public Voxel[,,] Voxels;

        public Chunk(Vector3 pos):base(ChunkSize, pos)
        {
            Shader = new DirectionalDiffuse();
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
        }

        protected override void Load()
        {
            OnChunkUpdated();
        }

        public void OnChunkUpdated()
        {
            ushort[] triangles;
            float[] vertecies;
            float[] colors;
            float[] normals;
            CreateCubes(out vertecies, out triangles, out colors, out normals);

            CreateMesh(vertecies, triangles, colors, normals);
        }

        private void CreateCubes(out float[] arrayBuffer, out ushort[] arrayElementBuffer, out float[] color, out float[] normal)
        {
            var vertices = new List<float>();
            var triangles = new List<ushort>();
            var colors = new List<float>();
            var normals = new List<float>();

            var planes = new int[6, ChunkSize, ChunkSize, ChunkSize];
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        var voxel = Voxels[x, y, z];
                        if (voxel != null && voxel.IsActive)
                        {
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
                RunGreedyMeshing(planes, i, vertices, triangles, colors, normals);
            }
            arrayBuffer = vertices.ToArray();
            arrayElementBuffer = triangles.ToArray();
            Length = arrayElementBuffer.Length;
            color = colors.ToArray();
            normal = normals.ToArray();
        }

        private void RunGreedyMeshing(int[,,,] planes, int o, List<float> vertices, List<ushort> triangles, List<float> colors, List<float> normals)
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
                                        AddRect(curRectangle, o == 1, curType, vertices, triangles, colors, normals, new Vector3(1f,0,0));
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
                                        AddRect(curRectangle, o == 1, vox, vertices, triangles, colors, normals, new Vector3(1f, 0f, 0));
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
                                        AddRect(curRectangle, o == 2, curType, vertices, triangles, colors, normals, new Vector3(0, 1f, 0f));
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
                                        AddRect(curRectangle, o == 2, vox, vertices, triangles, colors, normals, new Vector3(0f, 1f, 0));
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
                                        AddRect(curRectangle, o == 5, curType, vertices, triangles, colors, normals, new Vector3(0, 0, 1f));
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
                                        AddRect(curRectangle, o == 5, vox, vertices, triangles, colors, normals, new Vector3(0, 0, 1f));
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

        private void AddRect(Rect curRectangle, bool front, int curType, List<float> vertices, List<ushort> triangles, List<float> colors, List<float> normals, Vector3 normal)
        {
            var offset = vertices.Count/3;
            vertices.AddRange(new[]
            {
                (curRectangle.WorldA.X + Pos.X*ChunkSize)*Scale, (curRectangle.WorldA.Y + Pos.Y*ChunkSize)*Scale, (curRectangle.WorldA.Z + Pos.Z*ChunkSize)*Scale, // vertex[0]
			    (curRectangle.WorldB.X + Pos.X*ChunkSize)*Scale, (curRectangle.WorldB.Y + Pos.Y*ChunkSize)*Scale, (curRectangle.WorldB.Z + Pos.Z*ChunkSize)*Scale, // vertex[1]
			    (curRectangle.WorldC.X + Pos.X*ChunkSize)*Scale, (curRectangle.WorldC.Y + Pos.Y*ChunkSize)*Scale, (curRectangle.WorldC.Z + Pos.Z*ChunkSize)*Scale, // vertex[2]
			    (curRectangle.WorldD.X + Pos.X*ChunkSize)*Scale, (curRectangle.WorldD.Y + Pos.Y*ChunkSize)*Scale, (curRectangle.WorldD.Z + Pos.Z*ChunkSize)*Scale, // vertex[3]
            });
            colors.AddRange(GetColor(Color.DarkGreen));
            var norm = normal*(front ? 1 : -1);
            normals.AddRange(new[]
            {
                norm.X, norm.Y, norm.Z,
                norm.X, norm.Y, norm.Z,
                norm.X, norm.Y, norm.Z,
                norm.X, norm.Y, norm.Z
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

        private float[] GetColor(Color c)
        {
            return new[]
            {
                c.R/255f, c.G/255f, c.B/255f, c.A/255f,
                c.R/255f, c.G/255f, c.B/255f, c.A/255f,
                c.R/255f, c.G/255f, c.B/255f, c.A/255f,
                c.R/255f, c.G/255f, c.B/255f, c.A/255f
            };
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
