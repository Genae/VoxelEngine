using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Editor;
using UnityEngine;

namespace Assets.Scripts.Data.Material
{
    [Serializable]
    public class DictionaryMats : SerializableDictionary<MaterialTyp, UnityEngine.Material> { }

    public class MaterialRegistry : MonoBehaviour
    {
        private static readonly Dictionary<int, VoxelMaterial> VoxelMaterials = new Dictionary<int, VoxelMaterial>();
        public DictionaryMats Materials;
        public static int AtlasSize = 16;

        public UnityEngine.Material[] GetMaterials()
        {
            return Materials.Values.ToArray();
        }

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
            var tex = new Texture2D(AtlasSize, AtlasSize, TextureFormat.ARGB32, false);
            var i = 0;
            foreach (MaterialTyp matTyp in Enum.GetValues(typeof(MaterialTyp)))
            {
                var typ = matTyp;
                foreach (var material in VoxelMaterials.Values.Where(vm => vm.MaterialId.Equals(typ)))
                {
                    tex.SetPixel(material.AtlasPosition / AtlasSize, material.AtlasPosition % AtlasSize, material.Color);
                }
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.filterMode = FilterMode.Point;
                tex.Apply();
                Materials[typ].mainTexture = tex;
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
        public static readonly VoxelMaterial Dirt = Create(MaterialTyp.Default, rgb(161, 109, 37));
        public static readonly VoxelMaterial Grass = Create(MaterialTyp.Default, rgb(62, 131, 51));
        public static readonly VoxelMaterial Wood = Create(MaterialTyp.Default, rgb(142, 99, 56));
        public static readonly VoxelMaterial Leaves = Create(MaterialTyp.Default, rgb(125, 173, 70));

        #endregion

    }
}
