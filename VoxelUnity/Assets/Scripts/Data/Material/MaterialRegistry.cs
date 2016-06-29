using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Data.Material
{
    public class MaterialRegistry : MonoBehaviour
    {
        private static readonly Dictionary<int, VoxelMaterial> VoxelMaterials = new Dictionary<int, VoxelMaterial>();
        public UnityEngine.Material[] Materials;
        public static int AtlasSize = 16;
        
        public static VoxelMaterial MaterialFromId(int typeId)
        {
            return VoxelMaterials[typeId];
        }

        public static int GetMaterialId(VoxelMaterial material)
        {
            return material.Id;
        }

        void Start ()
        {
            CreateColorAtlas();
        }

        #region helper
        private void CreateColorAtlas()
        {
            foreach (MaterialTyp matTyp in Enum.GetValues(typeof(MaterialTyp)))
            {
                var tex = new Texture2D(AtlasSize, AtlasSize, TextureFormat.ARGB32, false);
                var typ = matTyp;
                foreach (var material in VoxelMaterials.Values.Where(vm => vm.MaterialId.Equals(typ)))
                {
                    tex.SetPixel(material.AtlasPosition / AtlasSize, material.AtlasPosition % AtlasSize, material.Color);
                }
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.filterMode = FilterMode.Point;
                tex.Apply();
                Materials[(int)typ].mainTexture = tex;
            }
        }

        // ReSharper disable once InconsistentNaming
        private static Color rgb(int r, int g, int b)
        {
            return new Color(r/255f, g/255f, b/255f);
        }

        private static readonly Dictionary<MaterialTyp, int> _counterTyp = new Dictionary<MaterialTyp, int>();
        private static VoxelMaterial Create(MaterialTyp typ, Color c)
        {
            if (!_counterTyp.ContainsKey(typ))
            {
                _counterTyp[typ] = 0;
            }
            var vm = new VoxelMaterial(_counterTyp[typ]++, typ, c, VoxelMaterials.Values.Count);
            VoxelMaterials[vm.Id] = vm;
            return vm;
        }
        #endregion
        
        #region MaterialDefinition

        public static readonly VoxelMaterial Air = Create(MaterialTyp.Default, Color.white);
        public static readonly VoxelMaterial Stone = Create(MaterialTyp.Default, rgb(120, 120, 120));
        public static readonly VoxelMaterial Dirt = Create(MaterialTyp.Default, rgb(160, 82, 45));
        public static readonly VoxelMaterial Grass = Create(MaterialTyp.Default, rgb(50, 205, 50));
        public static readonly VoxelMaterial OakWood = Create(MaterialTyp.Default, rgb(60, 30, 17));
        public static readonly VoxelMaterial OakLeaves = Create(MaterialTyp.Default, rgb(35, 144, 35));
        public static readonly VoxelMaterial BirchWood = Create(MaterialTyp.Default, rgb(234, 231, 214));
        public static readonly VoxelMaterial BirchLeaves = Create(MaterialTyp.Default, rgb(160, 185, 125));
        public static readonly VoxelMaterial Copper = Create(MaterialTyp.Metallic, rgb(184, 115, 51));
        public static readonly VoxelMaterial Iron = Create(MaterialTyp.Metallic, rgb(123, 123, 123));
        public static readonly VoxelMaterial Gold = Create(MaterialTyp.Metallic, rgb(255, 215, 0));

        #endregion

    }
}
