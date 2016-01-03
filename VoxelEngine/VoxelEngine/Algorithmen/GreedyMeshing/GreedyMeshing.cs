using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using VoxelEngine.GameData;

namespace VoxelEngine.Algorithmen.GreedyMeshing
{
    public static class GreedyMeshing
    {
        public static void CreateMesh(out float[] vertices, out ushort[] triangles, out float[] colors, out float[] normals, Voxel[,,] voxels, bool[][,] borders, Vector3 pos, float scale)
        {
            //Voxels to Planes
            var planes = InitializePlanes(voxels, borders);

            //Planes to Rects
            var rects = new Rect[6][][];
            for (var side = 0; side < 6; side++)
            {
                rects[side] = new Rect[voxels.GetLength(0)][];
                for (var depth = 0; depth < voxels.GetLength(0); depth++)
                {
                    rects[side][depth] = CreateRectsForPlane(planes[side][depth]);
                }
            }

            //Rects to Mesh
            var verticesL = new List<float>();
            var trianglesL = new List<ushort>();
            var colorsL = new List<float>();
            var normalsL = new List<float>();
            for (var side = 0; side < 6; side++)
            {
                for (var depth = 0; depth < voxels.GetLength(0); depth++)
                {
                    AddRectsToMesh(side, depth, rects[side][depth], pos, scale, voxels.GetLength(0), ref verticesL, ref trianglesL, ref colorsL, ref normalsL);
                }
            }

            vertices = verticesL.ToArray();
            triangles = trianglesL.ToArray();
            colors = colorsL.ToArray();
            normals = normalsL.ToArray();
        }
        
        internal static int[][][,] InitializePlanes(Voxel[,,] voxels, bool[][,] borders)
        {
            var planes = new int[6][][,];
            for (var side = 0; side < 6; side++)
            {
                planes[side] = new int[voxels.GetLength(0)][,];
                for (var depth = 0; depth < voxels.GetLength(0); depth++)
                {
                    planes[side][depth] = new int[voxels.GetLength(0), voxels.GetLength(0)];
                }
            }
            for (var x = 0; x < voxels.GetLength(0); x++)
            {
                for (var y = 0; y < voxels.GetLength(0); y++)
                {
                    for (var z = 0; z < voxels.GetLength(0); z++)
                    {
                        var voxel = voxels[x, y, z];
                        if (voxel != null && voxel.IsActive)
                        {
                            if ((x == 0 && !borders[0][y, z]) || (x != 0 && !voxels[x - 1, y, z].IsActive)) //+x left
                            {
                                planes[0][x][y, z] = voxels[x, y, z].BlockType;
                            }
                            if ((x == voxels.GetLength(0) - 1 && !borders[1][y, z]) ||
                                (x != voxels.GetLength(0) - 1 && !voxels[x + 1, y, z].IsActive)) //-x right
                            {
                                planes[1][x][y, z] = voxels[x, y, z].BlockType;
                            }
                            if ((y == 0 && !borders[2][x, z]) || (y != 0 && !voxels[x, y - 1, z].IsActive)) //+y bottom
                            {
                                planes[2][y][x, z] = voxels[x, y, z].BlockType;
                            }
                            if ((y == voxels.GetLength(0) - 1 && !borders[3][x, z]) ||
                                (y != voxels.GetLength(0) - 1 && !voxels[x, y + 1, z].IsActive)) //-y top
                            {
                                planes[3][y][x, z] = voxels[x, y, z].BlockType;
                            }
                            if ((z == 0 && !borders[4][x, y]) || (z != 0 && !voxels[x, y, z - 1].IsActive)) //+z back
                            {
                                planes[4][z][x, y] = voxels[x, y, z].BlockType;
                            }
                            if ((z == voxels.GetLength(0) - 1 && !borders[5][x, y]) ||
                                (z != voxels.GetLength(0) - 1 && !voxels[x, y, z + 1].IsActive)) //-z front
                            {
                                planes[5][z][x, y] = voxels[x, y, z].BlockType;
                            }
                        }
                    }
                }
            }
            return planes;
        }

        internal static Rect[] CreateRectsForPlane(int[,] plane)
        {
            var rects = new List<Rect>();
            var visited = new bool[plane.GetLength(0), plane.GetLength(1)];
            Rect curRectangle = null;
            var curType = 0;


            for (var j = 0; j < plane.GetLength(1); j++)
            {
                for (var i = 0; i < plane.GetLength(0); i++)
                {
                    var vox = plane[i, j];
                    if (vox != curType || visited[i, j]) //End Rect because of current voxel
                    {
                        if (curRectangle != null)
                        {
                            ExpandVertically(curRectangle, curType, plane);
                            rects.Add(curRectangle);
                            SetVisited(curRectangle, visited);
                            curRectangle = null;
                        }
                        if (visited[i, j])
                            continue;
                    }
                    if (vox != 0) //Create new Rect if there is no
                    {
                        if (curRectangle == null)
                        {
                            curRectangle = new Rect(i, j);
                            curType = vox;
                        }
                        else
                        {
                            curRectangle.Width++;
                        }
                    }
                    if (i == plane.GetLength(1) - 1) // End because of Border
                    {
                        if (curRectangle != null)
                        {
                            ExpandVertically(curRectangle, curType, plane);
                            rects.Add(curRectangle);
                            SetVisited(curRectangle, visited);
                            curRectangle = null;
                        }
                    }
                }
            }
            return rects.ToArray();
        }

        internal static void SetVisited(Rect curRectangle, bool[,] visited)
        {
            for (var i = curRectangle.X; i < curRectangle.X + curRectangle.Width; i++)
            {
                for (var j = curRectangle.Y; j < curRectangle.Y + curRectangle.Height; j++)
                {
                    visited[i, j] = true;
                }
            }
        }

        internal static void ExpandVertically(Rect curRectangle, int curType, int[,] plane)
        {
            while (true)
            {
                if (curRectangle.Y + curRectangle.Height == plane.GetLength(1))
                    return; //reached bottom

                //check next line
                for (var i = curRectangle.X; i < curRectangle.X + curRectangle.Width; i++)
                {
                    if (plane[i, curRectangle.Y + curRectangle.Height] != curType)
                        return; // found wrong type
                }
                curRectangle.Height++;
            }
        }


        internal static void AddRectsToMesh(int side, int depth, Rect[] rects, Vector3 pos, float scale, float chunksize, ref List<float> vertices, ref List<ushort> triangles, ref List<float> colors, ref List<float> normals)
        {
            foreach (var rect in rects)
            {
                var offset = vertices.Count / 3;
                Vector3 vertA = Vector3.Zero, vertB = Vector3.Zero, vertC = Vector3.Zero, vertD = Vector3.Zero, norm = Vector3.Zero;
                switch (side)
                {
                    case 0:
                    case 1:
                        vertA = new Vector3(depth - 0.5f + side - 0, rect.X - 0.5f + rect.Width, rect.Y - 0.5f);
                        vertB = new Vector3(depth - 0.5f + side - 0, rect.X - 0.5f + rect.Width, rect.Y - 0.5f + rect.Height);
                        vertC = new Vector3(depth - 0.5f + side - 0, rect.X - 0.5f, rect.Y - 0.5f);
                        vertD = new Vector3(depth - 0.5f + side - 0, rect.X - 0.5f, rect.Y - 0.5f + rect.Height);
                        norm = new Vector3(side % 2 == 0 ? 1 : -1, 0, 0);
                        break;
                    case 2:
                    case 3:
                        vertA = new Vector3(rect.X - 0.5f + rect.Width, depth - 0.5f + side - 2, rect.Y - 0.5f);
                        vertB = new Vector3(rect.X - 0.5f + rect.Width, depth - 0.5f + side - 2, rect.Y - 0.5f + rect.Height);
                        vertC = new Vector3(rect.X - 0.5f, depth - 0.5f + side - 2, rect.Y - 0.5f);
                        vertD = new Vector3(rect.X - 0.5f, depth - 0.5f + side - 2, rect.Y - 0.5f + rect.Height);
                        norm = new Vector3(0, side % 2 == 0 ? 1 : -1, 0);
                        break;
                    case 4:
                    case 5:
                        vertA = new Vector3(rect.X - 0.5f + rect.Width, rect.Y - 0.5f, depth - 0.5f + side - 4);
                        vertB = new Vector3(rect.X - 0.5f + rect.Width, rect.Y - 0.5f + rect.Height, depth - 0.5f + side - 4);
                        vertC = new Vector3(rect.X - 0.5f, rect.Y - 0.5f, depth - 0.5f + side - 4);
                        vertD = new Vector3(rect.X - 0.5f, rect.Y - 0.5f + rect.Height, depth - 0.5f + side - 4);
                        norm = new Vector3(0, 0, side % 2 == 0 ? 1 : -1);
                        break;
                }

                vertices.AddRange(new []
                {
                    (vertA.X + pos.X*chunksize) * scale, (vertA.Y + pos.Y*chunksize) * scale, (vertA.Z + pos.Z*chunksize) * scale,
                    (vertB.X + pos.X*chunksize) * scale, (vertB.Y + pos.Y*chunksize) * scale, (vertB.Z + pos.Z*chunksize) * scale,
                    (vertC.X + pos.X*chunksize) * scale, (vertC.Y + pos.Y*chunksize) * scale, (vertC.Z + pos.Z*chunksize) * scale,
                    (vertD.X + pos.X*chunksize) * scale, (vertD.Y + pos.Y*chunksize) * scale, (vertD.Z + pos.Z*chunksize) * scale
                });

                normals.AddRange(new []
                {
                    norm.X, norm.X, norm.Z,
                    norm.X, norm.X, norm.Z,
                    norm.X, norm.X, norm.Z,
                    norm.X, norm.X, norm.Z,
                });

                if (side % 2 == 0)
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
                colors.AddRange(GetColor(Color.DarkGreen));
            }
        }

        private static float[] GetColor(Color c)
        {
            return new[]
            {
                c.R/255f, c.G/255f, c.B/255f, c.A/255f,
                c.R/255f, c.G/255f, c.B/255f, c.A/255f,
                c.R/255f, c.G/255f, c.B/255f, c.A/255f,
                c.R/255f, c.G/255f, c.B/255f, c.A/255f
            };
        }

    }


    class Rect
    {
        public int X, Y;
        public int Width, Height;

        public Rect(int x, int y)
        {
            X = x;
            Y = y;
            Width = 1;
            Height = 1;
        }

        public override bool Equals(object obj)
        {
            var re = (Rect) obj;
            return re.X == X && re.Y == Y && re.Width == Width && re.Height == Height;
        }

        public override string ToString()
        {
            return "Point: (" + X + "/" + Y + "), " + Width + "x" + Height;
        }
    }
}
