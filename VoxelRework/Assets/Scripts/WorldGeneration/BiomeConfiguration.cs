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
        public VoxelMaterial Fluid;

        private LoadedVoxelMaterial[] _layers;
        private LoadedVoxelMaterial _fill;
        private LoadedVoxelMaterial _fluid;

        public LoadedVoxelMaterial GetLayer(int layer)
        {
            if(layer >= _layers.Length)
                return _fill;
            return _layers[layer];
        }

        public void Init(MaterialCollection collection)
        {
            _fill = collection.GetMaterial(FillMaterial);
            _fluid = collection.GetMaterial(Fluid);
            var layers = new List<LoadedVoxelMaterial>();
            foreach (var layerConfig in Layers)
            {
                for (var i = 0; i < layerConfig.Count; i++)
                {
                    layers.Add(collection.GetMaterial(layerConfig.Material));
                }
            }
            _layers = layers.ToArray();
        }

        public LoadedVoxelMaterial GetFluid()
        {
            return _fluid;
        }
    }

    [Serializable]
    public class LayerConfiguration
    {
        public VoxelMaterial Material;
        public int Count;
    }
}