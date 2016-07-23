using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Algorithms.Pathfinding
{
    public class NodeBuilder
    {
        public static List<Node> BuildAStarNetwork(ChunkData data, List<Vector3> upVoxels)
        {
            var map = GameObject.Find("Map").GetComponent<Map>();
            var nodes = CreateNodePositions(data, upVoxels, map.MapData);
            return nodes;
        }

        private static List<Node> CreateNodePositions(ChunkData data, List<Vector3> upVoxels, MapData map)
        {
            var nodes = new List<Node>();
            foreach (var upVoxel in upVoxels)
            {
                var x = (int)upVoxel.x + (int)data.Position.x;
                var y = (int)upVoxel.y + (int)data.Position.y;
                var z = (int)upVoxel.z + (int)data.Position.z;
                if (CheckPosition(map, x, y, z, 1, 2, 1))
                {
                    nodes.Add(new Node(x, y, z));
                }
            }
            return nodes;
        }
        

        private static bool CheckPosition(MapData map, int x, int y, int z, int width, int height, int depth)
        {
            if (IsWorldPosBlocked(map,  x, y-1, z))
            {
                for (var dX = -width/2; dX <= width/2; dX++)
                {
                    for (var dY = 0; dY <= height; dY++)
                    {
                        for (var dZ = -depth/2; dZ <= depth/2; dZ++)
                        {
                            if (Mathf.Abs(dX) > dY || Mathf.Abs(dZ) > dY)
                                continue;
                            if (IsWorldPosBlocked(map,  x + dX, y + dY, z + dZ))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private static bool IsWorldPosBlocked(MapData map, int x, int y, int z)
        {
            if (x < 0 || x >= Chunk.ChunkSize*map.Chunks.GetLength(0) ||
                y < 0 || y >= Chunk.ChunkSize*map.Chunks.GetLength(1) ||
                z < 0 || z >= Chunk.ChunkSize*map.Chunks.GetLength(2))
                return false;
            var chunk = map.Chunks[x / Chunk.ChunkSize, y / Chunk.ChunkSize, z / Chunk.ChunkSize];
            if (chunk == null)
            {
                return false;
            }
            return chunk.IsWorldPosBlocked(x, y, z);
        }
    }
}
