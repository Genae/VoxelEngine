using System;
using System.Collections.Generic;
using Assets.Scripts.VoxelEngine.Materials;
using UnityEngine;

namespace Assets.Scripts.WorldGeneration
{
    [CreateAssetMenu]
    public class BiomeConfiguration : ScriptableObject
    {
        public LayerConfiguration[] Layers;
        public VoxelMaterial FillMaterial;
        private string _fillMaterialName;
        private string[] _layerNames;

        public string GetLayer(int layer)
        {
            if(layer >= _layerNames.Length)
                return _fillMaterialName;
            return _layerNames[layer];
        }

        public void Init()
        {
            var layers = new List<string>();
            _fillMaterialName = FillMaterial.name;
            foreach (var layerConfig in Layers)
            {
                layerConfig.Init();
                for (var i = 0; i < layerConfig.Count; i++)
                {
                    layers.Add(layerConfig.MaterialName);
                }
            }
            _layerNames = layers.ToArray();
        }
    }

    [Serializable]
    public class LayerConfiguration
    {
        public VoxelMaterial Material;

        public int Count;
        public string MaterialName { get; private set; }
        public void Init()
        {
            MaterialName = Material.name;
        }
    }
}