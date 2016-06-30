using System.Collections.Generic;
using Assets.Scripts.Algorithms.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class AStarNetwork
    {
        List<Node> Nodes;

        public void RefreshNetwork(ChunkData chunk)
        {
            Nodes = NodeBuilder.BuildAStarNetwork(chunk);
            /*foreach (var node in Nodes)
            {
                node.Visualize(chunk.Position);
            }*/
        }
    }

    public class Node
    {
        public Dictionary<Node, float> Neighbours;
        public Vector3 Position;

        public Node(int x, int y, int z)
        {
            Position = new Vector3(x, y, z);
            Neighbours = new Dictionary<Node, float>();
        }

        public void Visualize(Vector3 pos)
        {
            foreach (var neighbour in Neighbours.Keys)
            {
                Debug.DrawLine(Position + pos, neighbour.Position + pos, Neighbours[neighbour] <= 1f ? Color.blue : Neighbours[neighbour] < 1.5f ? Color.yellow : Neighbours[neighbour] <= 1.5f ? Color.red : Color.magenta, 6000, true);
            }
        }
    }
}
