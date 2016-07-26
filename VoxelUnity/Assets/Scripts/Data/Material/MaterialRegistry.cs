using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Data.Material
{
    public class MaterialRegistry : MonoBehaviour
    {
        private static readonly Dictionary<int, VoxelMaterial> VoxelMaterials = new Dictionary<int, VoxelMaterial>();
        private static readonly Dictionary<Color, VoxelMaterial> EntityMaterialsIndices = new Dictionary<Color, VoxelMaterial>();
        private static readonly Dictionary<string, VoxelMaterial> VoxelMaterialByName = new Dictionary<string, VoxelMaterial>();
        public UnityEngine.Material[] Materials;
        private Texture2D tex;
        public UnityEngine.Material EntityMaterial
        {
            get { return Materials[(int)MaterialTyp.Entity]; }
        }

        public UnityEngine.Material HighlightMaterial
        {
            get { return Materials[(int)MaterialTyp.Highlight]; }
        }

        public static int AtlasSize = 16;
        
        public static VoxelMaterial MaterialFromId(int typeId)
        {
            return VoxelMaterials[typeId];
        }

        public static int GetMaterialId(VoxelMaterial material)
        {
            return material.Id;
        }

        public static VoxelMaterial GetMaterialFromName(string name)
        {
            return VoxelMaterialByName[name];
        }

        void Start ()
        {
            CreateColorAtlas();
        }

        public VoxelMaterial GetColorIndex(Color color)
        {
            if (!EntityMaterialsIndices.ContainsKey(color))
            {
                AddColorToAtlas(color);
            }
            return EntityMaterialsIndices[color];
        }

        #region helper
        private void CreateColorAtlas()
        {
            foreach (MaterialTyp matTyp in Enum.GetValues(typeof(MaterialTyp)))
            {
                if (matTyp.Equals(MaterialTyp.Entity)) continue;
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

        private void AddColorToAtlas(Color color)
        {
            if (tex == null)
            {
                tex = new Texture2D(AtlasSize, AtlasSize, TextureFormat.ARGB32, false);
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.filterMode = FilterMode.Point;
            }
            var pos = EntityMaterialsIndices.Count+1;
            EntityMaterialsIndices[color]= Create("EntityMaterial", MaterialTyp.Entity, color);
            tex.SetPixel(pos / AtlasSize, pos % AtlasSize, color);
            tex.Apply();
            EntityMaterial.mainTexture = tex;
        }

        // ReSharper disable once InconsistentNaming
        private static Color rgb(int r, int g, int b)
        {
            return new Color(r/255f, g/255f, b/255f);
        }

        private static readonly Dictionary<MaterialTyp, int> _counterTyp = new Dictionary<MaterialTyp, int>();
        private static VoxelMaterial Create(string name, MaterialTyp typ, Color c)
        {
            if (!_counterTyp.ContainsKey(typ))
            {
                _counterTyp[typ] = 0;
            }
            var vm = new VoxelMaterial(_counterTyp[typ]++, typ, c, VoxelMaterials.Values.Count);
            VoxelMaterials[vm.Id] = vm;
            VoxelMaterialByName[name] = vm;
            return vm;
        }
        #endregion
        
        #region MaterialDefinition

        public static readonly VoxelMaterial Air = Create("Air", MaterialTyp.Default, Color.white);
        public static readonly VoxelMaterial Stone = Create("Stone", MaterialTyp.Default, rgb(120, 120, 120));
        public static readonly VoxelMaterial Dirt = Create("Dirt", MaterialTyp.Default, rgb(160, 82, 45));
        public static readonly VoxelMaterial Grass = Create("Grass", MaterialTyp.Default, rgb(50, 205, 50));
        public static readonly VoxelMaterial OakWood = Create("OakWood", MaterialTyp.Default, rgb(60, 30, 17));
        public static readonly VoxelMaterial OakLeaves = Create("OakLeaves", MaterialTyp.Default, rgb(35, 144, 35));
        public static readonly VoxelMaterial BirchWood = Create("BirchWood", MaterialTyp.Default, rgb(234, 231, 214));
        public static readonly VoxelMaterial BirchLeaves = Create("BirchLeaves", MaterialTyp.Default, rgb(160, 185, 125));
        public static readonly VoxelMaterial Copper = Create("Copper", MaterialTyp.Metallic, rgb(184, 115, 51));
        public static readonly VoxelMaterial Iron = Create("Iron", MaterialTyp.Metallic, rgb(123, 123, 123));
        public static readonly VoxelMaterial Gold = Create("Gold", MaterialTyp.Metallic, rgb(255, 215, 0));
        public static readonly VoxelMaterial Coal = Create("Coal", MaterialTyp.Default, rgb(0, 0, 0));

        #endregion

    }
}
