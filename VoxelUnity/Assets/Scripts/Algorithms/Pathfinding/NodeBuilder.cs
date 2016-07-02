using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Algorithms.Pathfinding
{
    public class NodeBuilder
    {
        public static List<Node> BuildAStarNetwork(ChunkData data, List<Vector3> upVoxels)
        {
            var dictionary = CreateNodePositions(data, upVoxels);
            return ConnectNodes(dictionary);
        }

        private static Dictionary<int, Dictionary<int, List<Node>>> CreateNodePositions(ChunkData data, List<Vector3> upVoxels)
        {
            var dictionary = new Dictionary<int, Dictionary<int, List<Node>>>();
            foreach (var upVoxel in upVoxels)
            {
                var x = (int) upVoxel.x;
                var y = (int)upVoxel.y;
                var z = (int)upVoxel.z;
                if (!dictionary.ContainsKey(x))
                {
                    dictionary[x] = new Dictionary<int, List<Node>>();
                }
                if (!dictionary[x].ContainsKey(z))
                {
                    dictionary[x].Add(z, new List<Node>());
                }
                if (CheckPosition(data, x, y, z))
                {
                    dictionary[x][z].Add(new Node(x + (int)data.Position.x, y + (int)data.Position.y, z + (int)data.Position.z));
                }
            }
            return dictionary;
        }

        public static List<Node> ConnectNodes(Dictionary<int, Dictionary<int, List<Node>>> dictionary)
        {
            var nodes = new List<Node>();
            for (int x = 0; x < Chunk.ChunkSize; x++)
            {
                if (!dictionary.ContainsKey(x))
                    continue;
                for (int z = 0; z < Chunk.ChunkSize; z++)
                {
                    if (!dictionary[x].ContainsKey(z))
                        continue;
                    foreach (var node in dictionary[x][z])
                    {
                        var nodefrom = node;
                        if (x > 0 && dictionary.ContainsKey(x - 1) && z < dictionary[x - 1].Count - 1 && dictionary[x-1].ContainsKey(z + 1))
                        {
                            foreach (var nodeto in dictionary[x - 1][z + 1].Where(n => Mathf.Abs(n.Position.y - nodefrom.Position.y) < 1))
                            {
                                Connect(node, nodeto);
                            }
                        }
                        if (x < dictionary.Count - 1 && dictionary.ContainsKey(x + 1) && z < dictionary[x + 1].Count - 1 && dictionary[x+1].ContainsKey(z + 1))
                        {
                            foreach (var nodeto in dictionary[x + 1][z + 1].Where(n => Mathf.Abs(n.Position.y - nodefrom.Position.y) < 1))
                            {
                                Connect(node, nodeto);
                            }
                        }
                        if (x < dictionary.Count - 1 && dictionary.ContainsKey(x+1) && dictionary[x + 1].ContainsKey(z))
                        {
                            foreach (var nodeTo in dictionary[x + 1][z])
                            {
                                CheckNodeConnection(node, nodeTo);
                            }
                        }
                        if (z < dictionary[x].Count - 1 && dictionary[x].ContainsKey(z + 1))
                        {
                            foreach (var nodeTo in dictionary[x][z + 1])
                            {
                                CheckNodeConnection(node, nodeTo);
                            }
                        }
                        nodes.Add(node);
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
            if (!GetVoxelTyp(data, x, y-1, z).Equals(MaterialRegistry.Air))
            {
                for (var dX = -1; dX <= 1; dX++)
                {
                    for (var dY = 0; dY <= 4; dY++)
                    {
                        for (var dZ = -1; dZ <= 1; dZ++)
                        {
                            if (Mathf.Abs(dX) > dY || Mathf.Abs(dZ) > dY)
                                continue;
                            if (!GetVoxelTyp(data, x + dX, y + dY, z + dZ).Equals(MaterialRegistry.Air))
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
