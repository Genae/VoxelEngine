using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Multiblock
{
    public class Tree
    {
        public Multiblock Multiblock;
        public Vector3 Position;
        
        public Tree(List<Vector3> voxelPosList, Vector3 position)
        {
            Multiblock = new Multiblock(voxelPosList);
            Position = position;
        }
        
    }
}
