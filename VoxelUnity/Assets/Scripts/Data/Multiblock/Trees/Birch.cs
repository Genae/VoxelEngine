using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Multiblock.Trees
{
    public class Birch : Tree
    {
        public Birch(Vector3 position):base(position)
        {
            
        }

        protected override TreeData GetRandomizedTreeValues()
        {
            var stainDiaMod = (int)Random.Range(-1f, 1f);
            var stainHeightMod = (int)Random.Range(-2f, 5f);
            var topMod = (int)Random.Range(-2f, 5f);

            return new TreeData
            {
                TreeTopDia = 8 + topMod,
                TreeTopHeight = 8 + topMod,
                TreeStainDia = 2 + stainDiaMod,
                TreeStainHeight = 9 + stainHeightMod,
            };
        }
        
        protected override VoxelMaterial GetStainMaterial()
        {
            return MaterialRegistry.BirchWood;
        }

        protected override VoxelMaterial GetLeafMaterial()
        {
            return MaterialRegistry.BirchLeaves;
        }
    }
}