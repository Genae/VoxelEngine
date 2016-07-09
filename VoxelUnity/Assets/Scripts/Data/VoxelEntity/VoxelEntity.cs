using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Data.VoxelEntity
{
    public class VoxelEntity : VoxelContainer
    {
        public static VoxelEntity InstantiateVoxels(Vector3 position, Dictionary<Color, List<Vector3>> voxels)
        {
            var materialRegistry = GameObject.Find("Map").GetComponent<MaterialRegistry>();
            Vector3 zeroVec;
            var size = GetSize(out zeroVec, voxels);
            var data = new ContainerData(size, position + zeroVec);
            foreach (var type in voxels.Keys)
            {
                foreach (var v in voxels[type])
                {
                    data.SetVoxel((int)(v.x - zeroVec.x), (int)(v.y - zeroVec.y), (int)(v.z - zeroVec.z), true, materialRegistry.GetColorIndex(type));
                }
            }
            var container = CreateContainer<VoxelEntity>(position + zeroVec, data, materialRegistry.Materials, "Tree");
            container.transform.parent = materialRegistry.transform;
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


