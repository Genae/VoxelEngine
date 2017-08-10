using System.Collections.Generic;
using Assets.Scripts.Algorithms.Pathfinding;
using Assets.Scripts.Algorithms.Pathfinding.Graphs;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class LocalAStarNetwork
    {
        public Grid3D<Vector3I> Nodes = new Grid3D<Vector3I>();

        public void RefreshNetwork(ChunkData chunk, List<Vector3> upVoxels)
        {
            var map = Map.Instance;
            var oldNodes = Nodes;
            var newNodes = NodeBuilder.BuildAStarNetwork(chunk, upVoxels);

            var maxX = Mathf.Max(oldNodes.GetSize().x, newNodes.GetSize().x);
            var maxY = Mathf.Max(oldNodes.GetSize().y, newNodes.GetSize().y);
            var maxZ = Mathf.Max(oldNodes.GetSize().z, newNodes.GetSize().z);

            var minX = Mathf.Min(oldNodes.GetSize().x, newNodes.GetSize().x);
            var minY = Mathf.Min(oldNodes.GetSize().y, newNodes.GetSize().y);
            var minZ = Mathf.Min(oldNodes.GetSize().z, newNodes.GetSize().z);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {
                        if(oldNodes[x, y, z] != null && newNodes[x, y, z] == null) 
                            map.AStarNetwork.RemoveNode(new Vector3I(x, y, z));

                        if (newNodes[x, y, z] != null && oldNodes[x, y, z] == null)
                            map.AStarNetwork.AddNode(x, y, z);
                    }
                }
            }
        }
    }
    
}
