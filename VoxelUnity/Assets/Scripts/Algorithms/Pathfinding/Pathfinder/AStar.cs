using System.Collections.Generic;
using Algorithms.Pathfinding.Graphs;

namespace Algorithms.Pathfinding.Pathfinder
{
    public class AStar
    {
        public static Path GetPath(Node from, Node to, Path path)
        {
            return GetPath(new Dictionary<Node, float> {{from, 0f}}, new List<Node> {to}, path);
        }

        public static Path GetPath(Dictionary<Node, float> from, Node to, Path path)
        {
            return GetPath(from, new List<Node> { to }, path);
        }
        public static Path GetPath(Node from, List<Node> to, Path path)
        {
            return GetPath(new Dictionary<Node, float> { { from, 0f } }, to, path);
        }


        public static Path GetPath(Dictionary<Node, float> from, List<Node> to, Path path)
        {
            var openSet = new Utils.PriorityQueue<VisitedNode>();
            var pathNodeMap = new Dictionary<Node, VisitedNode>();
            var nodesTo = new HashSet<VisitedNode>();
            foreach (var node in to)
            {
                nodesTo.Add(new VisitedNode(node, null, 0));
            }
            if (nodesTo.Count == 0)
                return null;
            foreach (var f in from)
            {
                var nodeFrom = new VisitedNode(f.Key, null, f.Value);
                openSet.Enqueue(nodeFrom, nodeFrom.GetCost(nodesTo));
            }
            while (!openSet.IsEmpty())
            {
                var curNode = openSet.Dequeue();
                if (nodesTo.Contains(curNode))
                {
                    path = ReconstructPath(curNode, path);
                    //Debug.Log("Found path between " + nodeFrom.GridNode.Position + " and " + nodeTo.GridNode.Position + " of length: " + path.Length + " in " + (DateTime.Now-start).TotalMilliseconds + "ms.");
                    return path;
                }
                curNode.Status = NodeStatus.Closed;
                foreach (var neighbour in curNode.GridNode.GetNeighbours())
                {
                    if (pathNodeMap.ContainsKey(neighbour.To))
                    {
                        var pathNode = pathNodeMap[neighbour.To];
                        if (pathNode.Status == NodeStatus.Closed)
                            continue;
                        var node = new VisitedNode(neighbour.To, curNode, neighbour.Length);
                        if (openSet.Update(pathNode, pathNode.GetCost(nodesTo), node, node.GetCost(nodesTo)))
                            pathNodeMap[neighbour.To] = node;
                    }
                    else
                    {
                        var node = new VisitedNode(neighbour.To, curNode, neighbour.Length);
                        openSet.Enqueue(node, node.GetCost(nodesTo));
                        pathNodeMap[neighbour.To] = node;
                    }
                }
            }
            //Debug.Log("Couldn't find path between " + nodeFrom.GridNode + " and " + nodeTo.GridNode + " in " + (DateTime.Now - start).TotalMilliseconds + "ms.");
            return path;
        }
        
        private static Path ReconstructPath(VisitedNode node, Path path)
        {
            var length = node.GScore;
            var nodes = new List<Node>();
            while (node != null)
            {
                nodes.Add(node.GridNode);
                node = node.Prev;
            }
            nodes.Reverse();
            path.Nodes = nodes;
            path.Length = length;
            return path;
        }
        
    }

    public enum NodeStatus
    {
        None,
        Opened,
        Closed
    }
}
