using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AccessLayer.Material;
using Assets.Scripts.EngineLayer.Voxels.Data;
using UnityEngine;

namespace Assets.Scripts.EngineLayer.Voxels.Containers
{
    public class VoxelEntity : VoxelContainer
    {
        public static VoxelEntity InstantiateVoxels(Vector3 position, Dictionary<Color, List<Vector3>> voxels)
        {
            var materialRegistry = MaterialRegistry.Instance;
            Vector3 zeroVec;
            var size = GetSize(out zeroVec, voxels);
            var data = new ContainerData(size, position + zeroVec);
            foreach (var type in voxels.Keys)
            {
                foreach (var v in voxels[type])
                {
                    data.SetVoxel((int)(v.x - zeroVec.x), (int)(v.y - zeroVec.y), (int)(v.z - zeroVec.z), materialRegistry.GetColorIndex(type));
                }
            }
            var container = CreateContainer<VoxelEntity>(position + zeroVec, data, materialRegistry.Materials, "RandomizedTree");
            return (VoxelEntity)container;
        }

        private static int GetSize(out Vector3 zeroVec, Dictionary<Color, List<Vector3>> voxels)
        {
            var first = voxels.First().Value.First();
            float minX = first.x, minY = first.y, minZ = first.z, maxX = first.x, maxY = first.y, maxZ = first.z;
            foreach (var type in voxels.Keys)
            {
                foreach (var v in voxels[type])
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
            return (int)Mathf.Max(width, height, depth) + 1;
        }
    }
}


