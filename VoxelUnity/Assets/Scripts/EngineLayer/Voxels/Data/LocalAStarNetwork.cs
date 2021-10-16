using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Pathfinding;
using Algorithms.Pathfinding.Utils;
using EngineLayer.Voxels.Containers;
using UnityEngine;

namespace EngineLayer.Voxels.Data
{
    public class LocalAStarNetwork
    {
        private Vector3I[] Nodes = Array.Empty<Vector3I>();

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
