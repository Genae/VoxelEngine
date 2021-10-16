using System.Collections.Generic;
using System.Linq;
using Algorithms.Pathfinding.Pathfinder;

namespace Algorithms.Pathfinding.Graphs
{
    public class PathRegistry
    {
        public HashSet<Path> ActivePaths = new HashSet<Path>();

        public void RemovedNode(Node node, VoxelGraph graph)
        {
            foreach (var activePath in ActivePaths.ToArray())
            {
                if (activePath.Nodes != null && activePath.Nodes.Contains(node))
                    activePath.Recalculate(node, graph);
            }
        }
    }
}
