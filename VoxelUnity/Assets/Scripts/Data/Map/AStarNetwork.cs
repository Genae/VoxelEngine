using System.Collections.Generic;
using System.Linq;
using System.Net;
using Assets.Scripts.Algorithms.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class AStarNetwork
    {
        public List<Node> Nodes = new List<Node>();
        public AStarNetwork[] NeighbourNetworks = new AStarNetwork[4];

        public void RefreshNetwork(ChunkData chunk, List<Vector3> upVoxels)
        {
            foreach (var node in Nodes)
            {
                node.Disconnect();
            }
            
            Nodes = NodeBuilder.BuildAStarNetwork(chunk, upVoxels);
        }

        public void Visualize()
        {
            foreach (var node in Nodes)
            {
                node.Visualize();
            }
        }

        public void ConnectNetworkToNeighbours(ChunkData chunk)
        {
            if (Nodes.Count == 0)
                return;
            var x = 0;
            for (int i = 0; i < 6; i++)
            {
                if (i == 3 || i == 4)
                    continue;
                if (NeighbourNetworks[x] == null && chunk.NeighbourData[i] != null)
                {
                    NeighbourNetworks[x] = chunk.NeighbourData[i].AStar;
                    NeighbourNetworks[x].ConnectToNeighbour(this, GetBorder(x), x % 2==0? x + 1: x - 1);
                }
                x++;
            }
        }

        private void ConnectToNeighbour(AStarNetwork neighbour, Dictionary<int, List<Node>> border1, int index)
        {
            NeighbourNetworks[index] = neighbour;
            var border2 = GetBorder(index);
            var dic = new Dictionary<int, Dictionary<int, List<Node>>>();
            dic[0] = border1;
            dic[1] = border2;
            NodeBuilder.ConnectNodes(dic);
        }

        private Dictionary<int, List<Node>> GetBorder(int side)
        {
            var dic = new Dictionary<int, List<Node>>();
            List<Node> nodes;
            switch (side)
            {
                case 0:
                    nodes = Nodes.Where(n => (int)n.Position.x%Chunk.ChunkSize == 0).ToList();
                    for (var z = 0; z < Chunk.ChunkSize; z++)
                    {
                        dic[z] = nodes.Where(n => (int)n.Position.z % Chunk.ChunkSize == z).ToList();
                    }
                    return dic;
                case 1:
                    nodes = Nodes.Where(n => (int)n.Position.x % Chunk.ChunkSize == Chunk.ChunkSize-1).ToList();
                    for (var z = 0; z < Chunk.ChunkSize; z++)
                    {
                        dic[z] = nodes.Where(n => (int)n.Position.z % Chunk.ChunkSize == z).ToList();
                    }
                    return dic;
                case 2:
                    nodes = Nodes.Where(n => (int)n.Position.z % Chunk.ChunkSize == 0).ToList();
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        dic[x] = nodes.Where(n => (int)n.Position.x % Chunk.ChunkSize == x).ToList();
                    }
                    return dic;
                case 3:
                    nodes = Nodes.Where(n => (int)n.Position.z % Chunk.ChunkSize == Chunk.ChunkSize - 1).ToList();
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        dic[x] = nodes.Where(n => (int)n.Position.x % Chunk.ChunkSize == x).ToList();
                    }
                    return dic;
            }
            return dic;
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
            foreach (var neighbour in Neighbours)
            {
                neighbour.Key.Neighbours.Remove(this);
            }
        }
    }
}
