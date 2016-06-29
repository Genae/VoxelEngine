using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Multiblock.Trees
{
    public class Oak : Tree
    {
        public Oak(Vector3 position) : base(position)
        {
        }

        protected override TreeData GetRandomizedTreeValues()
        {
            var stainDiaMod = (int)Random.Range(-2f, 2f);
            var stainHeightMod = (int)Random.Range(-2f, 5f);
            var topMod = (int)Random.Range(-2f, 5f);
            
            return new TreeData
            {
                TreeTopDia = 11 + topMod,
                TreeTopHeight = 8 + topMod,
                TreeStainDia = 3 + stainDiaMod,
                TreeStainHeight = 7 + stainHeightMod,
            };
        }

        protected override VoxelMaterial GetStainMaterial()
        {
            return MaterialRegistry.OakWood;
        }

        protected override VoxelMaterial GetLeafMaterial()
        {
            return MaterialRegistry.OakLeaves;
        }
    }
}
