using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Data.Material
{
    public class MaterialRegistry
    {
        public static MaterialRegistry Instance
        {
            get { return _instance ?? (_instance = new MaterialRegistry()); }
            set { _instance = value; }
        }
        private static MaterialRegistry _instance;

        private readonly Dictionary<int, VoxelMaterial> VoxelMaterials = new Dictionary<int, VoxelMaterial>();
        private readonly Dictionary<Color, VoxelMaterial> EntityMaterialsIndices = new Dictionary<Color, VoxelMaterial>();
        private readonly Dictionary<string, VoxelMaterial> VoxelMaterialByName = new Dictionary<string, VoxelMaterial>();

        public UnityEngine.Material[] Materials
        {
            get { return _materials ?? GetMaterials(); }
            set { _materials = value; }
        }

        private UnityEngine.Material[] GetMaterials()
        {
            _materials = Object.FindObjectOfType<MaterialDefinition>().Materials;
            var allMats = MaterialDefinition.All;
            CreateColorAtlas();
            return _materials;
        }

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
        
        public VoxelMaterial MaterialFromId(int typeId)
        {
            return VoxelMaterials[typeId];
        }

        public int GetMaterialId(VoxelMaterial material)
        {
            return material.Id;
        }

        public VoxelMaterial GetMaterialFromName(string name)
        {
            return VoxelMaterialByName[name];
        }

        private MaterialRegistry()
        {}

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
            var pos = EntityMaterialsIndices.Count;
            EntityMaterialsIndices[color]= Create("EntityMaterial", MaterialTyp.Entity, color);
            tex.SetPixel(pos / AtlasSize, pos % AtlasSize, color);
            tex.Apply();
            EntityMaterial.mainTexture = tex;
        }
        
        private readonly Dictionary<MaterialTyp, int> _counterTyp = new Dictionary<MaterialTyp, int>();
        private UnityEngine.Material[] _materials;

        protected internal VoxelMaterial Create(string name, MaterialTyp typ, Color c)
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
    }
}
