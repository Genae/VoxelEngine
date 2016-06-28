using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Multiblock
{
    public class Multiblock
    {
        private List<Vector3> _voxelPosList;

        public Multiblock(List<Vector3> voxelPosList)
        {
            _voxelPosList = voxelPosList;
        }

        public List<Vector3> GetVoxelPositions()
        {
            return _voxelPosList;
        }

        public void AddVoxelListToMultiblock(List<Vector3> list)
        {
             _voxelPosList.AddRange(list);
        }
    }
}

