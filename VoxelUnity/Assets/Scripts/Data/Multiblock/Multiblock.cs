using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Multiblock
{
    public class Multiblock : VoxelContainer
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
        
        public void InstantiateVoxels(Vector3 position)
        {
            Vector3 zeroVec;
            var size = GetSize(out zeroVec);
            var data = new ContainerData(size);
            foreach (var type in _voxels.Keys)
            {
                foreach (var v in _voxels[type])
                {
                    data.SetVoxel((int)(v.x - zeroVec.x), (int)(v.y - zeroVec.y), (int)(v.z - zeroVec.z), true, type);
                }
            }
            CreateContainer(position+zeroVec, data, GameObject.Find("Map").GetComponent<Map.Map>(), "Tree");
        }

        private int GetSize(out Vector3 zeroVec)
        {
            var first = _voxels.First().Value.First();
            float minX = first.x, minY = first.y, minZ = first.z, maxX = first.x, maxY = first.y, maxZ = first.z;
            foreach (var type in _voxels.Keys)
            {
                foreach (var v in _voxels[type])
                {
                    if (minX > v.x)
                        minX = v.x;
                    if (minY > v.y)
                        minY = v.y;
                    if (minZ > v.z)
                        minZ = v.z;
                    if (maxX < v.x)
                        maxX = v.x;
                    if (maxY < v.y)
                        maxY = v.y;
                    if (maxZ < v.z)
                        maxZ = v.z;
                }
            }
            var width = maxX - minX;
            var height = maxY - minY;
            var depth = maxZ - minZ;
            zeroVec = new Vector3(minX, minY, minZ);
            return (int)Mathf.Max(width, height, depth)+1;
        }
    }
}

