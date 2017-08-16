using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Multiblock.Trees
{
    public class Tree
    {
        public Vector3 Position;
        public Multiblock Multiblock;

        public Tree(TreeConfig config, Vector3 position)
        {
            Initialize(config, position);
        }

        private void Initialize(TreeConfig config, Vector3 position)
        {
            var voxels = new Dictionary<VoxelMaterial, List<Vector3>>();
            var data = GetRandomizedTreeValues(config);
            Position = position;
            var strainVoxels = GenerateStrain(data);
            voxels.Add(MaterialRegistry.Instance.GetMaterialFromName(config.StainMaterial), strainVoxels);

            var topOfStain = new Vector3(-data.TreeTopDia / 2f + (data.TreeStainDia) / 2f, data.TreeStainHeight, -data.TreeTopDia / 2f + (data.TreeStainDia) / 2f);
            var treeTopVoxels = GenerateTreeTop(topOfStain, data);
            voxels.Add(MaterialRegistry.Instance.GetMaterialFromName(config.LeafMaterial), treeTopVoxels);

            Multiblock = Multiblock.InstantiateVoxels(position, voxels, "Tree");
            Multiblock.EnableWind(1);
        }

        protected TreeData GetRandomizedTreeValues(TreeConfig config)
        {
            var stainDiaMod = (int)Random.Range(config.StainDiaMod[0], config.StainDiaMod[1]);
            var stainHeightMod = (int)Random.Range(config.StainHeightMod[0], config.StainHeightMod[1]);
            var topMod = (int)Random.Range(config.TreeTopMod[0], config.TreeTopMod[1]);

            return new TreeData
            {
                TreeTopDia = config.TreeTopDia + topMod,
                TreeTopHeight = config.TreeTopHeight + topMod,
                TreeStainDia = config.TreeStainDia + stainDiaMod,
                TreeStainHeight = config.TreeStainHeight + stainHeightMod,
            };
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
    }
}
