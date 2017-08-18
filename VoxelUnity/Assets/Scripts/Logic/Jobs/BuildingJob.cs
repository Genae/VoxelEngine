using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic.Tools;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public class BuildingJob : PositionedJob
    {
        private readonly VoxelMaterial _material;

        public override JobType GetJobType()
        {
            return JobType.Building;
        }

        public BuildingJob(Vector3 position, VoxelMaterial material) : base(position, 1.1f, new Color(material.Color.r, material.Color.g, material.Color.b, 0.5f), Overlay.Building)
        {
            _material = material;
            RemainingTime = 1f;
        }

        protected override void SolveInternal(GameObject actor)
        {
            Map.Instance.MapData.Chunks[(int)Position.x/16, (int)Position.y/16, (int)Position.z/16]
                .SetVoxelType((int)Position.x%16, (int)Position.y%16, (int)Position.z%16, _material);
        }
    }
}
