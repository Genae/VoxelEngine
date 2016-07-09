﻿using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Algorithms.Pathfinding
{
    public class Path
    {
        public List<Node> Nodes;
        public float Length;

        public Path(List<Node> nodes, float length)
        {
            Nodes = nodes;
            Length = length;
        }

        public Node GetNode(int i)
        {
            if (i > Nodes.Count - 1)
            {
                return null;
            }
            return Nodes[i];
        }

        public void Visualize(Color color, int fromNode = -1)
        {
            for (int i = fromNode + 1; i < Nodes.Count - 1; i++)
            {
                if(fromNode == -1)
                    Debug.DrawLine(Nodes[i].Position, Nodes[i + 1].Position, color, 60000, true);
                else
                    Debug.DrawLine(Nodes[i].Position, Nodes[i + 1].Position, color);
                
            }
        }
    }

    public class PathNode
    {
        public Node GridNode;
        public PathNode Prev;
        public float GScore;

        public PathNode(Node node, PathNode prev, float cost)
        {
            GridNode = node;
            Prev = prev;
            if (prev == null)
            {
                GScore = 0;
            }
            else
            {
                GScore = prev.GScore + cost;
            }
        }

        public int GetCost(PathNode nodeTo)
        {
            return (int)(GScore + (nodeTo.GridNode.Position - GridNode.Position).magnitude);
        }

        public bool Equals(PathNode node)
        {
            return node.GridNode.Equals(GridNode);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            var pn = (PathNode) obj;
            return pn.GridNode.Equals(GridNode);
        }

        public override int GetHashCode()
        {
            return (GridNode != null ? GridNode.GetHashCode() : 0);
        }
    }
}