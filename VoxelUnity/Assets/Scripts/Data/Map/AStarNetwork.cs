using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class AStarNetwork
    {
        public readonly Node[,,] NodeGrid;

        public AStarNetwork(int width, int height, int depth)
        {
            NodeGrid = new Node[width, height, depth];
        }

        public void RemoveNode(Node node)
        {
            node.Disconnect();
            NodeGrid[(int) node.Position.x, (int) node.Position.y, (int) node.Position.z] = null;
        }

        public void AddNode(Node node)
        {
            NodeGrid[(int) node.Position.x, (int) node.Position.y, (int) node.Position.z] = node;
            ConnectNode(node);
        }

        private void ConnectNode(Node node)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    for (var dz = -1; dz <= 1; dz++)
                    {
                        var nodeTo = GetNode((int) node.Position.x + dx, (int) node.Position.y + dy, (int) node.Position.z + dz);
                        if (nodeTo != null)
                        {
                            ConnectNodes(node, nodeTo);
                        }
                    }
                }
            }
        }

        private void ConnectNodes(Node node, Node target)
        {
            var cost = (int)(node.Position.y - target.Position.y) == 0
                   ? (node.Position - target.Position).magnitude
                   : (int)Mathf.Abs(node.Position.y - target.Position.y - 1) <= 0.01f
                       ? 1.5f
                       : 5 * Mathf.Abs(node.Position.y - target.Position.y);
            node.Neighbours[target] = cost;
            target.Neighbours[node] = cost;
        }

        private Node GetNode(int x, int y, int z)
        {
            if (x > 0 && x < NodeGrid.GetLength(0) &&
                y > 0 && y < NodeGrid.GetLength(1) &&
                z > 0 && z < NodeGrid.GetLength(2))
                return NodeGrid[x, y, z];
            return null;
        }

        public List<Node> UpdateChunkNodes(List<Node> oldNodes, List<Node> newNodes)
        {
            if(newNodes.Count > 0) Debug.Log("Updating Chunk. Node Count (" + newNodes.Count + "/" + oldNodes.Count +")");
            foreach (var node in oldNodes.Where(o => !newNodes.Contains(o)).ToList())
            {
                RemoveNode(node);
                oldNodes.Remove(node);
            }
            foreach (var node in newNodes.Where(n => !oldNodes.Contains(n)))
            {
                AddNode(node);
                oldNodes.Add(node);
            }
            return oldNodes;
        }
    }
}
