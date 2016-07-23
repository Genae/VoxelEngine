using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class LocalAStarNetwork
    {
        public List<Node> Nodes = new List<Node>();

        public void RefreshNetwork(ChunkData chunk, List<Vector3> upVoxels)
        {
            var map = GameObject.Find("Map").GetComponent<Map>();
            var oldNodes = Nodes;
            var newNodes = NodeBuilder.BuildAStarNetwork(chunk, upVoxels);
            Nodes = map.AStarNetwork.UpdateChunkNodes(oldNodes, newNodes);
        }

        public void Visualize()
        {
            foreach (var node in Nodes)
            {
                node.Visualize();
            }
        }
    }

    public class Node
    {
        public Dictionary<Node, float> Neighbours;
        public Vector3 Position;

        public Node(int x, int y, int z)
        {
            Position = new Vector3(x, y-0.5f, z);
            Neighbours = new Dictionary<Node, float>();
        }

        public void Visualize()
        {
            foreach (var neighbour in Neighbours.Keys)
            {
                //if(Neighbours[neighbour] <= 1f)
                    Debug.DrawLine(Position, neighbour.Position, Neighbours[neighbour] <= 1f ? Color.blue : Neighbours[neighbour] < 1.5f ? Color.yellow : Neighbours[neighbour] <= 1.5f ? Color.red : Color.magenta, 6000, true);
            }
        }

        public void Disconnect()
        {
            foreach (var neighbour in Neighbours.Keys.ToArray())
            {
                neighbour.Neighbours.Remove(this);
            }
            Neighbours.Clear();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            var n = (Node) obj;
            return n.Position.Equals(Position);
        }

        protected bool Equals(Node other)
        {
            return Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}
