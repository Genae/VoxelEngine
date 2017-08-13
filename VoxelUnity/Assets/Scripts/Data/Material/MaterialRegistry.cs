using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Importer;
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
        {
            LoadMaterialCollection();
        }

        public void Preload()
        {
            GetMaterials();
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
                var texture = new Texture2D(AtlasSize, AtlasSize, TextureFormat.ARGB32, false);
                var typ = matTyp;
                foreach (var material in VoxelMaterials.Values.Where(vm => vm.MaterialId.Equals(typ)))
                {
                    texture.SetPixel(material.AtlasPosition / AtlasSize, material.AtlasPosition % AtlasSize, material.Color);
                }
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Point;
                texture.Apply();
                Materials[(int)typ].mainTexture = texture;
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

        void LoadMaterialCollection()
        {
            var mats = new MaterialCollection();
            mats.Load("Materials", this);
        }
        #endregion
    }

    public class MaterialCollection
    {
        public void Load(string path, MaterialRegistry registry)
        {
            var materials = ConfigImporter.GetConfig<MaterialJson[]>(path);
            foreach (var material in materials)
            {
                foreach (var dyn in material)
                {
                    registry.Create(dyn.Name, dyn.MaterialTyp, dyn.TrueColor);
                }
            }
        }
    }

    public class MaterialJson
    {
        public string Name;
        public int[] Color;
        public string Type;

        public MaterialTyp MaterialTyp
        {
            get { return (MaterialTyp)Enum.Parse(typeof(MaterialTyp), Type); }
        }

        public Color TrueColor
        {
            get { return rgb(Color[0], Color[1], Color[2]); }
        }

        // ReSharper disable once InconsistentNaming

        private static Color rgb(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }
    }
}
