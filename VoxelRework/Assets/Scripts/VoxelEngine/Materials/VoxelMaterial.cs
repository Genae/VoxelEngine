using UnityEngine;

namespace Assets.Scripts.VoxelEngine.Materials
{
    [CreateAssetMenu(fileName = "VoxelMaterial")]
    public class VoxelMaterial : ScriptableObject
    {
        public Material Material;
        public Color Color;
        public Texture2D Texture;
        public bool Transparent;
        private bool? _fluid;
        private string _name;

        public bool Fluid => _fluid ?? (_fluid = this is FluidVoxelMaterial).Value;
        
        public string Name => _name;

        void OnEnable()
        {
            _name = name.ToLower();
        }

    }

    [CreateAssetMenu(fileName = "FluidVoxelMaterial")]
    public class FluidVoxelMaterial : VoxelMaterial
    {
        public float Viscosity;
    }
}