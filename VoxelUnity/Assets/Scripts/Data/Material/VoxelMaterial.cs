using UnityEngine;

namespace Assets.Scripts.Data.Material
{
    public class VoxelMaterial
    {
        public readonly int AtlasPosition;
        public readonly MaterialTyp MaterialId;
        public readonly int Id;
        public readonly Color Color;
        public readonly Drop[] Drops;

        public VoxelMaterial(int atlasPosition, MaterialTyp materialId, Color color, int id, Drop[] drops)
        {
            AtlasPosition = atlasPosition;
            MaterialId = materialId;
            Color = color;
            Id = id;
            Drops = drops;
        }

        public int GetMaterialId()
        {
            return (int) MaterialId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var mat = (VoxelMaterial) obj;
            return mat.Id == Id;
        }

        protected bool Equals(VoxelMaterial other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }

    public enum MaterialTyp
    {
        Default = 0,
        Metallic = 1,
        Entity = 2,
        Highlight = 3
    }
}
