using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Algorithms.Pathfinding
{
    public class NodeBuilder
    {
        public static List<Node> BuildAStarNetwork(ChunkData data)
        {
            var dictionary = new Dictionary<int, Dictionary<int, Dictionary<int, Node>>>();
            for (int x = 0; x < Chunk.ChunkSize; x++)
            {
                dictionary[x] = new Dictionary<int, Dictionary<int, Node>>();
                for (int y = 0; y < Chunk.ChunkSize; y++)
                {
                    for (int z = 0; z < Chunk.ChunkSize; z++)
                    {
                        if (!dictionary[x].ContainsKey(z))
                            dictionary[x][z] = new Dictionary<int, Node>();
                        if (CheckPosition(data, x, y, z))
                        {
                            dictionary[x][z][dictionary[x][z].Count] = new Node(x, y, z);
                        }
                    }
                }
            }
            var nodes = new List<Node>();
            for (int x = 0; x < Chunk.ChunkSize; x++)
            {
                for (int z = 0; z < Chunk.ChunkSize; z++)
                {
                    foreach (var node in dictionary[x][z].Values)
                    {
                        var nodefrom = node;
                        if (x > 0 && z < Chunk.ChunkSize - 1)
                        {
                            foreach(var nodeto in dictionary[x - 1][z + 1].Values.Where(n => Mathf.Abs(n.Position.y - nodefrom.Position.y) < 1))
                            {
                                Connect(node, nodeto);
                            }
                        }
                        if (x < Chunk.ChunkSize - 1 && z < Chunk.ChunkSize - 1)
                        {
                            foreach (var nodeto in dictionary[x + 1][z + 1].Values.Where(n => Mathf.Abs(n.Position.y - nodefrom.Position.y) < 1))
                            {
                                Connect(node, nodeto);
                            }
                        }
                        if (x < Chunk.ChunkSize - 1)
                        {
                            foreach (var nodeTo in dictionary[x + 1][z])
                            {
                                CheckNodeConnection(node, nodeTo.Value);
                            }
                        }
                        if (z < Chunk.ChunkSize - 1)
                        {
                            foreach (var nodeTo in dictionary[x][z + 1])
                            {
                                CheckNodeConnection(node, nodeTo.Value);
                            }
                        }
                        if (node.Neighbours.Count > 0)
                        {
                            nodes.Add(node);
                        }
                    }
                    
                }
            }
            return nodes;
        }

        private static void Connect(Node node, Node target)
        {
            var cost = (int)(node.Position.y - target.Position.y) == 0
                ? (node.Position - target.Position).magnitude
                : (int)Mathf.Abs(node.Position.y - target.Position.y) == 1
                    ? 1.5f
                    : 5*Mathf.Abs(node.Position.y - target.Position.y);
            node.Neighbours[target] = cost;
            target.Neighbours[node] = cost;
        }

        private static void CheckNodeConnection(Node from, Node target)
        {
            if (Mathf.Abs(from.Position.y - target.Position.y) <= 1)
            {
                Connect(from, target);
            }
        }

        private static bool CheckPosition(ChunkData data, int x, int y, int z)
        {
            if (GetVoxelTyp(data, x, y, z).Equals(MaterialRegistry.Air) && !GetVoxelTyp(data, x, y-1, z).Equals(MaterialRegistry.Air))
            {
                return true;
            }
            return false;
        }

        private static VoxelMaterial GetVoxelTyp(ChunkData cd, int x, int y, int z)
        {
            var c = new Vector3((x + Chunk.ChunkSize * 10)/Chunk.ChunkSize - 10, (y + Chunk.ChunkSize * 10) /Chunk.ChunkSize - 10, (z + Chunk.ChunkSize * 10) /Chunk.ChunkSize - 10);
            var chunk = GetChunkRelative(cd, (int)c.x, (int)c.y, (int)c.z);
            if (chunk == null)
            {
                return MaterialRegistry.Air;
            }
            return chunk.GetVoxelType((int) (x - c.x*Chunk.ChunkSize), (int) (y - c.y*Chunk.ChunkSize), (int) (z - c.z*Chunk.ChunkSize));
        }

        private static ChunkData GetChunkRelative(ChunkData start, int x, int y, int z)
        {
            if (start == null)
                return null;
            if (x < 0)
            {
                return GetChunkRelative(start.NeighbourData[0], x + 1, y, z);
            }
            if (x > 0)
            {
                return GetChunkRelative(start.NeighbourData[1], x - 1, y, z);
            }
            if (y < 0)
            {
                return GetChunkRelative(start.NeighbourData[2], x, y + 1, z);
            }
            if (y > 0)
            {
                return GetChunkRelative(start.NeighbourData[3], x, y - 1, z);
            }
            if (z < 0)
            {
                return GetChunkRelative(start.NeighbourData[2], x, y, z + 1);
            }
            if (z > 0)
            {
                return GetChunkRelative(start.NeighbourData[3], x, y, z - 1);
            }
            return start;
        }
    }
}
