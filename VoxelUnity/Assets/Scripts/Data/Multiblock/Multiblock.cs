﻿using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Multiblock
{
    public class Multiblock
    {
        private readonly Dictionary<VoxelMaterial, List<Vector3>> _voxels = new Dictionary<VoxelMaterial, List<Vector3>>();
        
        public void AddVoxelListToMultiblock(List<Vector3> list, VoxelMaterial type)
        {
            if (!_voxels.ContainsKey(type))
            {
                _voxels[type] = new List<Vector3>();
            }
            _voxels[type].AddRange(list);
        }
        
        public void InstantiateVoxels(MapData mapData)
        {
            foreach (var type in _voxels.Keys)
            {
                foreach (var v in _voxels[type])
                {
                    mapData.SetVoxel((int)v.x, (int)v.y, (int)v.z, true, type);
                }
            }
        }
    }
}

