using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.VoxelEngine.Containers;
using Assets.Scripts.VoxelEngine.Containers.Chunks;
using Assets.Scripts.VoxelEngine.Materials;
using UnityEngine;

namespace Assets.Scripts.VoxelEngine.Renderers
{
    public class MeshData
    {
        public Vector3[] Vertices;
        public Dictionary<int, int[]> Triangles;
        public Vector3[] Normals;
        public Vector2[][] Uvs;
        public ushort[][][,] Planes;

        public MeshData(Vector3[] verticies, Dictionary<int, int[]> triangles, Vector3[] normals, Vector2[][] uvs, ushort[][][,] planes)
        {
            Vertices = verticies;
            Triangles = triangles;
            Normals = normals;
            Uvs = uvs;
            Planes = planes;
        }
    }
    public class GreedyMeshing
    {
        public static MeshData CreateMesh(IVoxelContainer container, Dictionary<ChunkSide, Chunk> neighbours, MaterialCollection materialCollection, int? slice, out List<Vector3> upVoxels)
        {
            //create planes
            if (neighbours == null)
            {
                neighbours = new Dictionary<ChunkSide, Chunk>
                {
                    {ChunkSide.Nx, null},
                    {ChunkSide.Px, null},
                    {ChunkSide.Nz, null},
                    {ChunkSide.Pz, null},
                    {ChunkSide.Ny, null},
                    {ChunkSide.Py, null},
                };
            }
            var size = container.GetSize();
            var planes = InitializePlanes(container, neighbours, materialCollection, slice, out upVoxels);

            //Planes to Rects
            var rects = new List<Rect>[6][];
            if(slice == null)
            {
                for (var side = 0; side < 6; side++)
                {
                    rects[side] = new List<Rect>[side < 2 ? size.x : (side < 4 ? size.z : size.y)];
                    for (var depth = 0; depth < rects[side].Length; depth++)
                    {
                        rects[side][depth] = CreateRectsForPlane(planes[side][depth]);
                    }
                }
            }
            else
            {
                rects[4] = new List<Rect>[size.y];
                rects[4][slice.Value] = CreateRectsForPlane(planes[4][slice.Value]);
            }

            //Rects to Mesh
            var verticesL = new List<Vector3>();
            var trianglesL = new Dictionary<int, List<int>>();
            var normalsL = new List<Vector3>();
            var uvsL = new[]
            {
                new List<Vector2>(),
                new List<Vector2>(),
                new List<Vector2>()
            };
            if(slice == null)
            {
                for (var side = 0; side < 6; side++)
                {
                    for (var depth = 0; depth < rects[side].Length; depth++)
                    {
                        AddRectsToMesh(side, depth, rects[side][depth], materialCollection, ref verticesL, ref trianglesL, ref normalsL, ref uvsL);
                    }
                }
            }
            else
            {
                AddRectsToMesh(4, slice.Value, rects[4][slice.Value], materialCollection, ref verticesL, ref trianglesL, ref normalsL, ref uvsL);
            }

            var meshData = new MeshData(verticesL.ToArray(), trianglesL.ToDictionary(v => v.Key, v => v.Value.ToArray()), normalsL.ToArray(), uvsL.Select(uv => uv.ToArray()).ToArray(), planes);
            return meshData;
        }

        private static ushort[][][,] InitializePlanes(IVoxelContainer container, Dictionary<ChunkSide, Chunk> neigbours, MaterialCollection materialCollection, int? slice, out List<Vector3> upVoxels)
        {
            //initialize Plane Arrays
            var size = container.GetSize();
            upVoxels = new List<Vector3>();
            var planes = new ushort[6][][,];
            for (var side = 0; side < 6; side++)
            {
                planes[side] = new ushort[side < 2 ? size.x : (side < 4 ? size.z : size.y)][,];
                for (var depth = 0; depth < planes[side].Length; depth++)
                {
                    planes[side][depth] = new ushort[side < 2 ? size.y : size.x, side == 2 || side == 3 ? size.y : size.z];
                }
            }
            if(slice == null)
            {
                for (var x = 0; x < size.x; x++)
                {
                    for (var y = 0; y < size.y; y++)
                    {
                        for (var z = 0; z < size.z; z++)
                        {
                            SetPlaneValue(container, neigbours, materialCollection, false, x, y, z, planes, size, ref upVoxels);
                        }
                    }
                }
            }
            else
            {
                for (var x = 0; x < size.x; x++)
                {
                    for (var z = 0; z < size.z; z++)
                    {
                        SetPlaneValue(container, neigbours, materialCollection, true, x, slice.Value, z, planes, size, ref upVoxels);
                    }
                }
            }
            return planes;
        }

        private static void SetPlaneValue(IVoxelContainer container, Dictionary<ChunkSide, Chunk> neigbours, MaterialCollection materialCollection, bool slice, int x, int y, int z, ushort[][][,] planes, Vector3Int size, ref List<Vector3> upVoxels)
        {
            var idWithHeight = container.GetVoxelData(new Vector3Int(x, y, z));
            var id = (ushort)((ushort)(idWithHeight << 3) >> 3);
            var height = (ushort) (8 - (ushort) (idWithHeight >> 13));
            var material = materialCollection.GetById(id);
            if (id != 0)
            {
                if (!slice)
                {
                    if (x == size.x - 1 && IsTransparent(0, y, z, neigbours[ChunkSide.Px], materialCollection, material, height) || x != size.x - 1 && IsTransparent(x + 1, y, z, container, materialCollection, material, height)) //px
                    {
                        planes[0][x][y, z] = idWithHeight;
                    }
                    if (x == 0 && IsTransparent(size.x - 1, y, z, neigbours[ChunkSide.Nx], materialCollection, material, height) || x != 0 && IsTransparent(x - 1, y, z, container, materialCollection, material, height)) //nx
                    {
                        planes[1][x][y, z] = idWithHeight;
                    }
                    if (z == size.z - 1 && IsTransparent(x, y, 0, neigbours[ChunkSide.Pz], materialCollection, material, height) || z != size.z - 1 && IsTransparent(x, y, z + 1, container, materialCollection, material, height)) //pz
                    {
                        planes[2][z][x, y] = idWithHeight;
                    }
                    if (z == 0 && IsTransparent(x, y, size.z - 1, neigbours[ChunkSide.Nz], materialCollection, material, height) || z != 0 && IsTransparent(x, y, z - 1, container, materialCollection, material, height)) //nz
                    {
                        planes[3][z][x, y] = idWithHeight;
                    }
                    if (y == size.y - 1 && (slice || IsTransparent(x, 0, z, neigbours[ChunkSide.Py], materialCollection, material, height)) || y != size.y - 1 && (slice || IsTransparent(x, y + 1, z, container, materialCollection, material, height))) //py
                    {
                        if (y < size.y - 1 && container.GetVoxelData(new Vector3Int(x, y + 1, z)) == 0)
                            upVoxels.Add(new Vector3(x, y + 1, z));
                        planes[4][y][x, z] = idWithHeight;
                    }
                    if (y == 0 && IsTransparent(x, size.y - 1, z, neigbours[ChunkSide.Ny], materialCollection, material, height) || y != 0 && IsTransparent(x, y - 1, z, container, materialCollection, material, height)) //ny
                    {
                        planes[5][y][x, z] = idWithHeight;
                    }
                }
                if (y == size.y - 1 && (slice || IsTransparent(x, 0, z, neigbours[ChunkSide.Py], materialCollection, material, height)) || y != size.y - 1 && (slice || IsTransparent(x, y + 1, z, container, materialCollection, material, height))) //py
                {
                    if (y < size.y - 1 && container.GetVoxelData(new Vector3Int(x, y + 1, z)) == 0)
                        upVoxels.Add(new Vector3(x, y + 1, z));
                    planes[4][y][x, z] = idWithHeight;
                }

            }
            else
            {
                if (y == 0 && (neigbours[ChunkSide.Ny] == null || neigbours[ChunkSide.Ny].GetVoxelData(new Vector3Int(x, size.y - 1, z)) == 0))
                {
                    upVoxels.Add(new Vector3(x, y, z));
                }
            }
        }

        private static bool IsTransparent(int x, int y, int z, IVoxelContainer container, MaterialCollection matCol, LoadedVoxelMaterial mat, ushort height)
        {
            if (container == null)
                return true;
            var id = container.GetVoxelData(new Vector3Int(x, y, z));
            var myHeight = (ushort)(8 - (ushort)(id >> 13));
            id = (ushort) ((ushort) (id << 3) >> 3);
            if (height != myHeight)
                return true;
            if (id == mat.Id)
                return false;
            return id == 0 || matCol.GetById(id).Transparent;
        }

        public static List<Rect> CreateRectsForPlane(ushort[,] plane)
        {
            var rects = new List<Rect>();
            var visited = new bool[plane.GetLength(0), plane.GetLength(1)];
            Rect curRectangle = null;
            ushort curType = 0;


            var l1 = plane.GetLength(1);
            var l0 = plane.GetLength(0);
            for (var j = 0; j < l1; j++)
            {
                for (var i = 0; i < l0; i++)
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
                            curRectangle = new Rect(i, j, vox);
                            curType = vox;
                        }
                        else
                        {
                            curRectangle.Width++;
                        }
                    }
                    if (i == l1 - 1) // End because of Border
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
            return rects;
        }

        
        public static void SetVisited(Rect curRectangle, bool[,] visited)
        {
            for (var i = curRectangle.X; i < curRectangle.X + curRectangle.Width; i++)
            {
                for (var j = curRectangle.Y; j < curRectangle.Y + curRectangle.Height; j++)
                {
                    visited[i, j] = true;
                }
            }
        }

        internal static void ExpandVertically(Rect curRectangle, ushort curType, ushort[,] plane)
        {
            while (true)
            {
                if (curRectangle.Y + curRectangle.Height == plane.GetLength(1))
                    return; //reached bottom

                //check next line
                for (var i = curRectangle.X; i < curRectangle.X + curRectangle.Width; i++)
                {
                    if (curType != plane[i, curRectangle.Y + curRectangle.Height])
                        return; // found wrong type
                }
                curRectangle.Height++;
            }
        }

        private static void AddRectsToMesh(int side, int depth, List<Rect> rects, MaterialCollection materialCollection, ref List<Vector3> vertices, ref Dictionary<int, List<int>> triangles, ref List<Vector3> normals, ref List<Vector2>[] uvs)
        {
            foreach (var rect in rects)
            {
                var offset = vertices.Count;
                Vector3 vertA = Vector3.zero, vertB = Vector3.zero, vertC = Vector3.zero, vertD = Vector3.zero, norm = Vector3.zero;
                var blockHeight = 0f;
                if (rect.BlockHeight != 0)
                {
                    blockHeight = (8 - rect.BlockHeight) / 8f;
                }
                if (side == 5)
                    blockHeight = 0;
                switch (side)
                {
                    case 0:
                    case 1:
                        vertA = new Vector3(depth + 0.5f - side, rect.X - 0.5f + rect.Width - blockHeight, rect.Y - 0.5f);
                        vertB = new Vector3(depth + 0.5f - side, rect.X - 0.5f + rect.Width - blockHeight, rect.Y - 0.5f + rect.Height);
                        vertC = new Vector3(depth + 0.5f - side, rect.X - 0.5f, rect.Y - 0.5f);
                        vertD = new Vector3(depth + 0.5f - side, rect.X - 0.5f, rect.Y - 0.5f + rect.Height);
                        norm = new Vector3(side % 2 != 0 ? -1 : 1, 0, 0);
                        break;
                    case 2:
                    case 3:
                        vertA = new Vector3(rect.X - 0.5f + rect.Width, rect.Y - 0.5f, depth + 0.5f - side % 2);
                        vertB = new Vector3(rect.X - 0.5f + rect.Width, rect.Y - 0.5f + rect.Height - blockHeight, depth + 0.5f - side % 2);
                        vertC = new Vector3(rect.X - 0.5f, rect.Y - 0.5f, depth + 0.5f - side % 2);
                        vertD = new Vector3(rect.X - 0.5f, rect.Y - 0.5f + rect.Height - blockHeight, depth + 0.5f - side % 2);
                        norm = new Vector3(0, 0, side % 2 != 0 ? -1 : 1);
                        break;
                    case 4:
                    case 5:
                        vertA = new Vector3(rect.X - 0.5f, depth + 0.5f - side % 2 - blockHeight, rect.Y - 0.5f + rect.Height);
                        vertB = new Vector3(rect.X - 0.5f + rect.Width, depth + 0.5f - side % 2 - blockHeight, rect.Y - 0.5f + rect.Height);
                        vertC = new Vector3(rect.X - 0.5f, depth + 0.5f - side % 2 - blockHeight, rect.Y - 0.5f);
                        vertD = new Vector3(rect.X - 0.5f + rect.Width, depth + 0.5f - side % 2 - blockHeight, rect.Y - 0.5f);
                        norm = new Vector3(0, side % 2 != 0 ? -1 : 1, 0);
                        break;
                }
                vertices.AddRange(new[]
                {
                    new Vector3(vertA.x, vertA.y, vertA.z),
                    new Vector3(vertB.x, vertB.y, vertB.z),
                    new Vector3(vertC.x, vertC.y, vertC.z),
                    new Vector3(vertD.x, vertD.y, vertD.z)
                });

                normals.AddRange(new[]
                {
                    new Vector3(norm.x, norm.y, norm.z),
                    new Vector3(norm.x, norm.y, norm.z),
                    new Vector3(norm.x, norm.y, norm.z),
                    new Vector3(norm.x, norm.y, norm.z)
                });

                if (!triangles.ContainsKey(rect.Type))
                    triangles[rect.Type] = new List<int>();

                if (side % 2 == 0)
                {
                    triangles[rect.Type].AddRange(new[]
                    {
                        0 + offset, 1 + offset, 3 + offset,
                        0 + offset, 3 + offset, 2 + offset
                    });
                }
                else
                {
                    triangles[rect.Type].AddRange(new[]
                    {
                        1 + offset, 0 + offset, 3 + offset,
                        3 + offset, 0 + offset, 2 + offset
                    });
                }

                // ReSharper disable once PossibleLossOfFraction
                var material = materialCollection.GetById(rect.Type);
                var atlasPos = new Vector2((int)(material.AtlasPosition / MaterialCollectionSettings.AtlasSize), material.AtlasPosition % MaterialCollectionSettings.AtlasSize);
                var size = new Vector2(rect.Width, rect.Height);
                if(side >= 4)
                    size = new Vector2(rect.Height, rect.Width);
                uvs[0].AddRange(new[] { new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) });
                uvs[1].AddRange(new[] { atlasPos, atlasPos, atlasPos, atlasPos });
                uvs[2].AddRange(new[] { size, size, size, size });
            }
        }

    }

    public class Rect
    {
        public int X, Y;
        public int Width, Height;
        public ushort Type;
        public ushort BlockHeight;

        public Rect(int x, int y, ushort type)
        {
            X = x;
            Y = y;
            Width = 1;
            Height = 1;
            Type = (ushort)((ushort)(type << 3) >> 3);
            BlockHeight = (ushort)(8 - (ushort) (type >> 13));
        }

        protected bool Equals(Rect other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height && Equals(Type, other.Type);
        }

        public override string ToString()
        {
            return "Point: (" + X + "/" + Y + "), " + Width + "x" + Height;
        }
    }
}