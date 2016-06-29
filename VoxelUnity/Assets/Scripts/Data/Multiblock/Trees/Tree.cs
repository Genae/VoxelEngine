using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Multiblock.Trees
{
    public abstract class Tree : Multiblock
    {
        public Vector3 Position;

        protected Tree(Vector3 position)
        {
            Initialize(position);
        }

        private void Initialize(Vector3 position)
        {
            var data = GetRandomizedTreeValues();
            Position = position;
            var strainVoxels = GenerateStrain(data);
            AddVoxelListToMultiblock(strainVoxels, GetStainMaterial());

            var topOfStain = new Vector3(-data.TreeTopDia / 2f + (data.TreeStainDia) / 2f, data.TreeStainHeight, -data.TreeTopDia / 2f + (data.TreeStainDia) / 2f);
            var treeTopVoxels = GenerateTreeTop(topOfStain, data);
            AddVoxelListToMultiblock(treeTopVoxels, GetLeafMaterial());

            InstantiateVoxels(position);
        }

        private List<Vector3> GenerateStrain(TreeData treeData)
        {
            var list = new List<Vector3>();

            for (var y = 0; y < treeData.TreeStainHeight; y++)
            {
                for (var x = 0; x <= treeData.TreeStainDia; x++)
                {
                    for (var z = 0; z <= treeData.TreeStainDia; z++)
                    {
                        list.Add(new Vector3(x, y, z));
                    }
                }
            }

            return list;
        }

        protected virtual List<Vector3> GenerateTreeTop(Vector3 offset, TreeData treeData)
        {
            var list = new List<Vector3>();

            for (var y = 0; y <= treeData.TreeTopHeight; y++)
            {
                for (var x = 0; x <= treeData.TreeTopDia; x++)
                {
                    for (var z = 0; z <= treeData.TreeTopDia; z++)
                    {
                        if (x == 0 && z == 0 || x == treeData.TreeTopDia && z == 0 || z == treeData.TreeTopDia && x == 0 || x == treeData.TreeTopDia && z == treeData.TreeTopDia) continue;
                        if (y == 0 && x == 0 || y == treeData.TreeTopHeight && x == 0 || y == 0 && z == 0 || y == treeData.TreeTopHeight && z == 0) continue;
                        if (y == 0 && x == treeData.TreeTopDia || y == treeData.TreeTopHeight && x == treeData.TreeTopDia || y == 0 && z == treeData.TreeTopDia || y == treeData.TreeTopHeight && z == treeData.TreeTopDia) continue;
                        list.Add(new Vector3(offset.x + x, offset.y + y, offset.z + z));
                    }
                }
            }
            return list;
        }
        
        protected abstract TreeData GetRandomizedTreeValues();
        protected abstract VoxelMaterial GetStainMaterial();
        protected abstract VoxelMaterial GetLeafMaterial();
    }
}
