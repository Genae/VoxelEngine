using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Algorithms.Pathfinding
{
    public class AStar
    {
        public static Path GetPath(MapData map, Vector3 from, Vector3 to)
        {
            var start = DateTime.Now;
            var closeSet = new HashSet<Vector3>();
            var openSet = new PriorityQueue<PathNode>();
            var nodeFrom = GetNode(from, map);
            var nodeTo = GetNode(to, map);
            if (nodeTo == null || nodeFrom == null)
            {
                Debug.Log("Could not find start or end Node.");
                return null;
            }
            openSet.Enqueue(nodeFrom, nodeFrom.GetCost(nodeTo));

            while (!openSet.IsEmpty())
            {
                var curNode = openSet.Dequeue();
                if (curNode.Equals(nodeTo))
                {
                    var path = ReconstructPath(curNode);
                    Debug.Log("Found path between " + nodeFrom.GridNode.Position + " and " + nodeTo.GridNode.Position + " of length: " + path.Length + " in " + (DateTime.Now-start).TotalMilliseconds + "ms.");
                    return path;
                }
                closeSet.Add(curNode.GridNode.Position);
                foreach (var neighbour in curNode.GridNode.Neighbours)
                {
                    if (closeSet.Contains(neighbour.Key.Position))
                        continue;
                    var node = GetPathNode(neighbour.Key, curNode, neighbour.Value);
                    if (!openSet.Contains(node))
                    {
                        openSet.Enqueue(node, node.GetCost(nodeTo));
                    }
                }
            }
            Debug.Log("Couldn't find path between " + nodeFrom.GridNode.Position + " and " + nodeTo.GridNode.Position + " in " + (DateTime.Now - start).TotalMilliseconds + "ms.");
            return null;
        }

        private static Path ReconstructPath(PathNode node)
        {
            var length = node.GScore;
            var nodes = new List<Node>();
            while (node != null)
            {
                nodes.Add(node.GridNode);
                node = node.Prev;
            }
            nodes.Reverse();
            return new Path(nodes, length);
        }

        private static PathNode GetNode(Vector3 position, MapData map)
        {
            var cx = (int)position.x / Chunk.ChunkSize;
            var cy = (int)position.y / Chunk.ChunkSize;
            var cz = (int)position.z / Chunk.ChunkSize;
            return GetPathNode(map.Chunks[cx, cy, cz].AStar.Nodes.OrderBy(n => (n.Position - position).magnitude).FirstOrDefault(), null, 0);
        }

        private static PathNode GetPathNode(Node node, PathNode prev, float cost)
        {
            return new PathNode(node, prev, cost);
        }
    }
}
