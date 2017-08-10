using System.Collections.Generic;
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

            var o = 0;
            var i = 0;
            while (true)
            {
                if (o < oldNodes.Length && i < newNodes.Length && oldNodes[o] == newNodes[i])
                {
                    o++;
                    i++;
                }
                else if(o < oldNodes.Length && (i >= newNodes.Length || oldNodes[o].CompareTo(newNodes[i]) < 0))
                {
                    map.AStarNetwork.RemoveNode(oldNodes[o]);
                    o++;
                }
                else if(i < newNodes.Length && (o >= oldNodes.Length || oldNodes[o].CompareTo(newNodes[i]) > 0))
                {
                    map.AStarNetwork.AddNode(newNodes[i]);
                    i++;
                }
                else
                {
                    break;
                }
            }
        }
    }
    
}
