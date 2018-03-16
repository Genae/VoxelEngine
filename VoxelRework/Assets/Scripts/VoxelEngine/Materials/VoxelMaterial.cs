using UnityEngine;

namespace Assets.Scripts.VoxelEngine.Materials
{
    [CreateAssetMenu]
    public class VoxelMaterial : ScriptableObject
    {
        public Material Material;
        public Color Color;
        public Texture2D Texture;
        public bool Transparent;
    }
}