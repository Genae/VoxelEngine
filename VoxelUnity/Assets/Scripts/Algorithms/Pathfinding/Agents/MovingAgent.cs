using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Pathfinding.Graphs;
using Algorithms.Pathfinding.Pathfinder;

namespace Algorithms.Pathfinding.Agents
{
    public class MovingAgent
    {
        public Path FollowPath(Path path, VoxelGraph graph)
        {
            if (path.IsT0 || path.Nodes == null)
            {
                return path;
            }
            var lastSuper = (SuperNode)path.Nodes.Last();
            var pos = 0;
            var curNode = path.Start;
            var visited = new List<Node>();
            float length = 0;
            while (!curNode.SuperNodes.ContainsKey(lastSuper))
            {
                if(visited.Contains(curNode))
                    throw new Exception("Loop detected");
                visited.Add(curNode);
                for (var i = 1; i >= 0; i--)
                {
                    if (curNode.SuperNodes.ContainsKey((SuperNode)path.Nodes[pos + i]))
                    {
                        var nextStep = curNode.SuperNodes[(SuperNode)path.Nodes[pos + i]];
                        curNode = nextStep.To;
                        length += nextStep.Length - (nextStep.To != null ? (nextStep.To.SuperNodes[(SuperNode)path.Nodes[pos + i]] != null ? nextStep.To.SuperNodes[(SuperNode)path.Nodes[pos + i]].Length : 0) : 0);
                        pos += i;
                        break;
                    }                    
                }
                    
            }
            var finalPath = Path.Calculate(graph, curNode.Position, path.Targets.Select(t => t.Position).ToList());
            finalPath.Thread.Join();
            visited.AddRange(finalPath.Nodes);
            var walkedPath = new Path(path.Start, path.Targets, graph.GetPathRegistry())
            {
                Nodes = visited,
                Length = length + finalPath.Length
            };
            finalPath.Dispose();
            return walkedPath;
        }
    }
}
