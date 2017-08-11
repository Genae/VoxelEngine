using NUnit.Framework.Constraints;
using UnityEngine;

namespace Assets.Scripts.Data.Material
{
    public class MaterialDefinition : MonoBehaviour
    {
        public static MaterialCollection All
        {
            get { return _all ?? (_all = LoadMaterialCollection()); }
            set { _all = value; }
        }

        public UnityEngine.Material[] Materials;
        private static MaterialCollection _all;

        // ReSharper disable once InconsistentNaming
        private static Color rgb(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }

        static MaterialCollection LoadMaterialCollection()
        {
            return new MaterialCollection
            {
                Air = Create("Air", MaterialTyp.Default, Color.white),
                Stone = Create("Stone", MaterialTyp.Default, rgb(120, 120, 120)),
                Dirt = Create("Dirt", MaterialTyp.Default, rgb(160, 82, 45)),
                Grass = Create("Grass", MaterialTyp.Default, rgb(50, 205, 50)),
                OakWood = Create("OakWood", MaterialTyp.Default, rgb(60, 30, 17)),
                OakLeaves = Create("OakLeaves", MaterialTyp.Default, rgb(35, 144, 35)),
                BirchWood = Create("BirchWood", MaterialTyp.Default, rgb(234, 231, 214)),
                BirchLeaves = Create("BirchLeaves", MaterialTyp.Default, rgb(160, 185, 125)),
                Copper = Create("Copper", MaterialTyp.Metallic, rgb(184, 115, 51)),
                Iron = Create("Iron", MaterialTyp.Metallic, rgb(123, 123, 123)),
                Gold = Create("Gold", MaterialTyp.Metallic, rgb(255, 215, 0)),
                Coal = Create("Coal", MaterialTyp.Default, rgb(0, 0, 0)),
            };
        }

        private static VoxelMaterial Create(string name, MaterialTyp type, Color color)
        {
            return MaterialRegistry.Instance.Create(name, type, color);
        }
    }

    public class MaterialCollection
    {
        public VoxelMaterial Air;
        public VoxelMaterial Stone;
        public VoxelMaterial Dirt;
        public VoxelMaterial Grass;
        public VoxelMaterial OakWood;
        public VoxelMaterial OakLeaves;
        public VoxelMaterial BirchWood;
        public VoxelMaterial BirchLeaves;
        public VoxelMaterial Copper;
        public VoxelMaterial Iron;
        public VoxelMaterial Gold;
        public VoxelMaterial Coal;
    }
}
