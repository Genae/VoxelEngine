using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.VoxelEngine.Materials
{
    public class MaterialCollectionSettings
    {
        public const int AtlasSize = 8;
    }

    public class MaterialCollection
    {
        public List<LoadedVoxelMaterial> VoxelMaterials;
        public Dictionary<string, LoadedVoxelMaterial> VoxelMaterialIndex;
        public List<Atlas> Atlases;

        public MaterialCollection()
        {
            Atlases = new List<Atlas>();
            VoxelMaterials = new List<LoadedVoxelMaterial>
            {
                new LoadedVoxelMaterial(ScriptableObject.CreateInstance<VoxelMaterial>())
                {
                    AtlasPosition = 0,
                    Id = 0,
                    Transparent = true
                }
            };
            VoxelMaterialIndex = new Dictionary<string, LoadedVoxelMaterial> {{"air", VoxelMaterials.First()}};
            LoadAllMaterials();
        }

        public void LoadAllMaterials()
        {
            foreach (var mat in Resources.LoadAll<VoxelMaterial>(""))
            {
                LoadVoxelMaterial(mat);
            }
        }

        public void LoadVoxelMaterial(VoxelMaterial voxelMaterial)
        {
            if (VoxelMaterialIndex.ContainsKey(voxelMaterial.name.ToLower()))
                return;
            var loadedVoxelMaterial = new LoadedVoxelMaterial(voxelMaterial);
            if (GetAtlas(loadedVoxelMaterial).AddVoxelMaterial(loadedVoxelMaterial))
            {
                loadedVoxelMaterial.Id = (ushort)VoxelMaterials.Count;
                VoxelMaterials.Add(loadedVoxelMaterial);
                VoxelMaterialIndex[voxelMaterial.name.ToLower()] = loadedVoxelMaterial;
            }
        }

        public Atlas GetAtlas(LoadedVoxelMaterial loadedVoxelMaterial)
        {
            var atlas = Atlases.FirstOrDefault(a => a.Material.Equals(loadedVoxelMaterial.Material) && loadedVoxelMaterial.TextureSize.Equals(a.TextureSize));
            if (atlas == null)
            {
                atlas = new Atlas(loadedVoxelMaterial.Material, loadedVoxelMaterial.TextureSize);
                Atlases.Add(atlas);
            }
            return atlas;
        }

        public LoadedVoxelMaterial GetById(ushort voxelData)
        {
            return VoxelMaterials[voxelData];
        }

        public ushort GetId(VoxelMaterial voxelMaterial)
        {
            return GetId(voxelMaterial.name);
        }

        public ushort GetId(string name)
        {
            if (!VoxelMaterialIndex.ContainsKey(name.ToLower()))
                return 0;
            return VoxelMaterialIndex[name.ToLower()].Id;
        }

        internal void SetSlice(int slice)
        {
            foreach(var mat in Atlases.Select(a => a.Material))
            {
                mat.SetFloat("_Slice", slice + 0.4999f);
            }
        }
    }

    public class Atlas
    {
        public Material Material;
        public Vector2Int TextureSize;
        public ushort Count;
        public List<Color[]> Colors;

        public Atlas(Material material, Vector2Int textureSize)
        {
            Material = material;
            TextureSize = textureSize;
            Colors = new List<Color[]>();
        }

        public bool AddVoxelMaterial(LoadedVoxelMaterial loadedVoxelMaterial)
        {
            if (Colors.Contains(loadedVoxelMaterial.Color))
                return false;
            loadedVoxelMaterial.AtlasPosition = Count++;

            var texture = (Texture2D)loadedVoxelMaterial.Material.mainTexture;
            if(texture == null)
                texture = new Texture2D(MaterialCollectionSettings.AtlasSize * TextureSize.x, MaterialCollectionSettings.AtlasSize * TextureSize.y, TextureFormat.ARGB32, false);
            SetPixels(texture, loadedVoxelMaterial);
            
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            loadedVoxelMaterial.Material.mainTexture = texture;
            return true;
        }
        
        private void SetPixels(Texture2D texture, LoadedVoxelMaterial loadedVoxelMaterial)
        {
            texture.SetPixels((loadedVoxelMaterial.AtlasPosition / MaterialCollectionSettings.AtlasSize) * TextureSize.x, (loadedVoxelMaterial.AtlasPosition % MaterialCollectionSettings.AtlasSize) * TextureSize.y, TextureSize.x, TextureSize.y, loadedVoxelMaterial.Color);
            Colors.Add(loadedVoxelMaterial.Color);
        }
    }

    public class LoadedVoxelMaterial
    {
        private readonly VoxelMaterial _voxelMaterial;
        public ushort Id;
        public ushort AtlasPosition;

        public bool Transparent
        {
            get { return _voxelMaterial.Transparent; }
            set { _voxelMaterial.Transparent = value; }
        }

        public Color[] Color
        {
            get
            {
                if(_voxelMaterial.Texture == null)
                    return new[]{_voxelMaterial.Color};
                return _voxelMaterial.Texture.GetPixels();
            }
        }
        public Material Material
        {
            get { return _voxelMaterial.Material; }
            set { _voxelMaterial.Material = value; }
        }

        public Vector2Int TextureSize => _voxelMaterial.Texture == null ? Vector2Int.one : new Vector2Int(_voxelMaterial.Texture.width, _voxelMaterial.Texture.height);

        public LoadedVoxelMaterial(VoxelMaterial material)
        {
            _voxelMaterial = material;
        }
    }
}