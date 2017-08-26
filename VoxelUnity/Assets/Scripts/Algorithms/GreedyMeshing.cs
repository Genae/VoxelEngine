using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Material;
using Assets.Scripts.EngineLayer.Voxels.Data;
using UnityEngine;

namespace Assets.Scripts.Algorithms
{
    public static class GreedyMeshing
    {
        public static void CreateMesh(out Vector3[] vertices, out Dictionary<int, int[]> triangles, out Vector3[] normals, out Vector2[] uvs, out List<Vector3> upVoxels, ContainerData container, int containerSize)
        {
            //Voxels to Planes
            var chunk = container as ChunkData;
            bool[][,] neighbourBorders;
            if (chunk != null)
            {
                neighbourBorders = chunk.GetNeighbourBorders();
            }
            else
            {
                neighbourBorders = new bool[6][,];
                for (var i = 0; i < 6; i++)
                {
                    neighbourBorders[i] = new bool[containerSize, containerSize];
                }
            }
            var planes = InitializePlanes(container, neighbourBorders, containerSize, out upVoxels);

            //Planes to Rects
            var rects = new Rect[6][][];
            for (var side = 0; side < 6; side++)
            {
                rects[side] = new Rect[containerSize][];
                for (var depth = 0; depth < containerSize; depth++)
                {
                    rects[side][depth] = CreateRectsForPlane(planes[side][depth]);
                }
            }

            //Rects to Mesh
            var verticesL = new List<Vector3>();
            var trianglesL = new Dictionary<int, List<int>>();
            var normalsL = new List<Vector3>();
            var uvsL = new List<Vector2>();
            for (var side = 0; side < 6; side++)
            {
                for (var depth = 0; depth < containerSize; depth++)
                {
                    AddRectsToMesh(side, depth, rects[side][depth], ref verticesL, ref trianglesL, ref normalsL, ref uvsL);
                }
            }

            vertices = verticesL.ToArray();
            triangles = trianglesL.ToDictionary(v => v.Key, v => v.Value.ToArray());
            normals = normalsL.ToArray();
            uvs = uvsL.ToArray();
        }

        public static VoxelMaterial[][][,] InitializePlanes(ContainerData chunk, bool[][,] borders, int containerSize, out List<Vector3> upVoxels)
        {
            upVoxels = new List<Vector3>();
            var planes = new VoxelMaterial[6][][,];
            for (var side = 0; side < 6; side++)
            {
                planes[side] = new VoxelMaterial[containerSize][,];
                for (var depth = 0; depth < containerSize; depth++)
                {
                    planes[side][depth] = new VoxelMaterial[containerSize, containerSize];
                }
            }
            for (var x = 0; x < containerSize; x++)
            {
                for (var y = 0; y < containerSize; y++)
                {
                    for (var z = 0; z < containerSize; z++)
                    {
                        if (chunk.GetVoxelActive(x, y, z))
                        {
                            if (x == 0 && !borders[0][y, z] || x != 0 && !chunk.GetVoxelActive(x - 1, y, z)) //+x left
                            {
                                planes[0][x][y, z] = chunk.GetVoxelType(x, y, z);
                            }
                            if (x == containerSize - 1 && !borders[1][y, z] || x != containerSize - 1 && !chunk.GetVoxelActive(x + 1, y, z)) //-x right
                            {
                                planes[1][x][y, z] = chunk.GetVoxelType(x, y, z);
                            }
                            if (y == 0 && !borders[2][x, z] || y != 0 && !chunk.GetVoxelActive(x, y - 1, z)) //+y bottom
                            {
                                planes[2][y][x, z] = chunk.GetVoxelType(x, y, z);
                            }
                            if (y == containerSize - 1 && !borders[3][x, z] || y != containerSize - 1 && !chunk.GetVoxelActive(x, y + 1, z)) //-y top
                            {
                                if (y < containerSize - 1)
                                    upVoxels.Add(new Vector3(x, y + 1, z));
                                planes[3][y][x, z] = chunk.GetVoxelType(x, y, z);
                            }
                            if (z == 0 && !borders[4][x, y] || z != 0 && !chunk.GetVoxelActive(x, y, z - 1)) //+z back
                            {
                                planes[4][z][x, y] = chunk.GetVoxelType(x, y, z);
                            }
                            if (z == containerSize - 1 && !borders[5][x, y] || z != containerSize - 1 && !chunk.GetVoxelActive(x, y, z + 1)) //-z front
                            {
                                planes[5][z][x, y] = chunk.GetVoxelType(x, y, z);
                            }
                        }
                        else
                        {
                            if (y == 0)
                            {
                                upVoxels.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                }
            }
            return planes;
        }

        public static Rect[] CreateRectsForPlane(VoxelMaterial[,] plane)
        {
            var rects = new List<Rect>();
            var visited = new bool[plane.GetLength(0), plane.GetLength(1)];
            Rect curRectangle = null;
            VoxelMaterial curType = null;


            for (var j = 0; j < plane.GetLength(1); j++)
            {
                for (var i = 0; i < plane.GetLength(0); i++)
                {
                    var vox = plane[i, j];
                    if (!Equals(vox, curType) || visited[i, j]) //End Rect because of current voxel
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
                    if (vox != null) //Create new Rect if there is no
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

        internal static void ExpandVertically(Rect curRectangle, VoxelMaterial curType, VoxelMaterial[,] plane)
        {
            while (true)
            {
                if (curRectangle.Y + curRectangle.Height == plane.GetLength(1))
                    return; //reached bottom

                //check next line
                for (var i = curRectangle.X; i < curRectangle.X + curRectangle.Width; i++)
                {
                    if (!Equals(plane[i, curRectangle.Y + curRectangle.Height], curType))
                        return; // found wrong type
                }
                curRectangle.Height++;
            }
        }
        
        private static void AddRectsToMesh(int side, int depth, Rect[] rects, ref List<Vector3> vertices, ref Dictionary<int, List<int>> triangles, ref List<Vector3> normals, ref List<Vector2> uvs)
        { 
            foreach (var rect in rects)
            {
                var offset = vertices.Count;
                Vector3 vertA = Vector3.zero, vertB = Vector3.zero, vertC = Vector3.zero, vertD = Vector3.zero, norm = Vector3.zero;
                switch (side)
                {
                    case 0:
                    case 1:
                        vertA = new Vector3(depth - 0.5f + side - 0, rect.X - 0.5f + rect.Width, rect.Y - 0.5f);
                        vertB = new Vector3(depth - 0.5f + side - 0, rect.X - 0.5f + rect.Width, rect.Y - 0.5f + rect.Height);
                        vertC = new Vector3(depth - 0.5f + side - 0, rect.X - 0.5f, rect.Y - 0.5f);
                        vertD = new Vector3(depth - 0.5f + side - 0, rect.X - 0.5f, rect.Y - 0.5f + rect.Height);
                        norm = new Vector3(side % 2 != 0 ? 1 : -1, 0, 0);
                        break;
                    case 2:
                    case 3:
                        vertA = new Vector3(rect.X - 0.5f + rect.Width, depth - 0.5f + side - 2, rect.Y - 0.5f);
                        vertB = new Vector3(rect.X - 0.5f + rect.Width, depth - 0.5f + side - 2, rect.Y - 0.5f + rect.Height);
                        vertC = new Vector3(rect.X - 0.5f, depth - 0.5f + side - 2, rect.Y - 0.5f);
                        vertD = new Vector3(rect.X - 0.5f, depth - 0.5f + side - 2, rect.Y - 0.5f + rect.Height);
                        norm = new Vector3(0, side % 2 != 0 ? 1 : -1, 0);
                        break;
                    case 4:
                    case 5:
                        vertA = new Vector3(rect.X - 0.5f + rect.Width, rect.Y - 0.5f, depth - 0.5f + side - 4);
                        vertB = new Vector3(rect.X - 0.5f + rect.Width, rect.Y - 0.5f + rect.Height, depth - 0.5f + side - 4);
                        vertC = new Vector3(rect.X - 0.5f, rect.Y - 0.5f, depth - 0.5f + side - 4);
                        vertD = new Vector3(rect.X - 0.5f, rect.Y - 0.5f + rect.Height, depth - 0.5f + side - 4);
                        norm = new Vector3(0, 0, side % 2 != 0 ? 1 : -1);
                        break;
                }

                vertices.AddRange(new []
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
                
                if(!triangles.ContainsKey(rect.Type.GetMaterialId()))
                    triangles[rect.Type.GetMaterialId()] = new List<int>();

                if (side == 5 || side == 2 || side == 1)
                {
                    triangles[rect.Type.GetMaterialId()].AddRange(new[]
                    {
                        0 + offset, 1 + offset, 3 + offset,
                        0 + offset, 3 + offset, 2 + offset
                    });
                }
                else
                {
                    triangles[rect.Type.GetMaterialId()].AddRange(new[]
                    {
                        1 + offset, 0 + offset, 3 + offset,
                        3 + offset, 0 + offset, 2 + offset
                    });
                }

                // ReSharper disable once PossibleLossOfFraction
                var uvcoord = new Vector2(rect.Type.AtlasPosition/MaterialRegistry.AtlasSize/(float) MaterialRegistry.AtlasSize, rect.Type.AtlasPosition%MaterialRegistry.AtlasSize/(float) MaterialRegistry.AtlasSize);
                uvs.AddRange(new []
                {
                    new Vector2(uvcoord.x + 0.1f/MaterialRegistry.AtlasSize, uvcoord.y+ 0.1f/MaterialRegistry.AtlasSize),
                    new Vector2(uvcoord.x + 0.1f/MaterialRegistry.AtlasSize, uvcoord.y+ 0.1f/MaterialRegistry.AtlasSize),
                    new Vector2(uvcoord.x + 0.1f/MaterialRegistry.AtlasSize, uvcoord.y+ 0.1f/MaterialRegistry.AtlasSize),
                    new Vector2(uvcoord.x + 0.1f/MaterialRegistry.AtlasSize, uvcoord.y+ 0.1f/MaterialRegistry.AtlasSize),
                });
            }
        }
    }


    public class Rect
    {
        public int X, Y;
        public int Width, Height;
        public VoxelMaterial Type;

        public Rect(int x, int y, VoxelMaterial type)
        {
            X = x;
            Y = y;
            Width = 1;
            Height = 1;
            Type = type;
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
