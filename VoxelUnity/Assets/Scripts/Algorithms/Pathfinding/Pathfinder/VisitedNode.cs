using System;
using System.Collections.Generic;
using Algorithms.Pathfinding.Graphs;

namespace Algorithms.Pathfinding.Pathfinder
{
    public class VisitedNode
    {
        public Node GridNode;
        public VisitedNode Prev;
        public float GScore;
        public NodeStatus Status = NodeStatus.Opened;

        public VisitedNode(Node node, VisitedNode prev, float cost)
        {
            GridNode = node;
            Prev = prev;
            if (prev == null)
            {
                GScore = cost;
            }
            else
            {
                GScore = prev.GScore + cost;
            }
        }

        public float GetCost(HashSet<VisitedNode> nodeTo)
        {
            var min = float.MaxValue;
            foreach (var visitedNode in nodeTo)
            {
                var a = visitedNode.GridNode.Position;
                var b = GridNode.Position;
                var x = a.x - b.x;
                var y = a.y - b.y;
                var z = a.z - b.z;
                var dist = GScore + (float)Math.Sqrt(x * x + y * y + z * z);
                if (dist < min)
                    min = dist;
            }
            return min;
        }

        public bool Equals(VisitedNode node)
        {
            return node.GridNode.Equals(GridNode);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            var pn = (VisitedNode)obj;
            return pn.GridNode.Equals(GridNode);
        }

        public override int GetHashCode()
        {
            return (GridNode != null ? GridNode.GetHashCode() : 0);
        }
    }
}