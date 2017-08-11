using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class LocalAStarNetwork
    {
        public Vector3I[] Nodes = new Vector3I[0];

        public void RefreshNetwork(ChunkData chunk, List<Vector3> upVoxels)
        {
            var map = Map.Instance;
            var oldNodes = Nodes;
            var newNodes = NodeBuilder.BuildAStarNetwork(chunk, upVoxels).ToArray();
            
            var addNodes = newNodes.Except(oldNodes).ToList();
            var removeNodes = oldNodes.Except(newNodes).ToList();

            foreach (var node in removeNodes)
            {
                map.AStarNetwork.RemoveNode(node);
            }
            foreach (var node in addNodes)
            {
                map.AStarNetwork.AddNode(node);
            }
            Nodes = newNodes;
        }
    }
    
}
