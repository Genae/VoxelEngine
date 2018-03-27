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
        private VoxelMaterial[] _layers;

        public VoxelMaterial GetLayer(int layer)
        {
            if(layer >= _layers.Length)
                return FillMaterial;
            return _layers[layer];
        }

        public void Init()
        {
            var layers = new List<VoxelMaterial>();
            foreach (var layerConfig in Layers)
            {
                for (var i = 0; i < layerConfig.Count; i++)
                {
                    layers.Add(layerConfig.Material);
                }
            }
            _layers = layers.ToArray();
        }
    }

    [Serializable]
    public class LayerConfiguration
    {
        public VoxelMaterial Material;
        public int Count;
    }
}