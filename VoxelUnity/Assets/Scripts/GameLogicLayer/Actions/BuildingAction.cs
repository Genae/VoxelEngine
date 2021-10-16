using AccessLayer.Jobs;
using AccessLayer.Tools;
using AccessLayer.Worlds;
using EngineLayer.Voxels.Material;
using UnityEngine;

namespace GameLogicLayer.Actions
{
   
    public class BuildingAction : SolveJobAction
    {
        public BuildingAction() : base("hasBuilt", "Building", 1.5f)
        { }
    }

    public class BuildingJob : PositionedJob
    {
        private readonly VoxelMaterial _material;

        public BuildingJob(Vector3 position, VoxelMaterial material) : base(position, 1.1f, new Color(material.Color.r, material.Color.g, material.Color.b, 0.5f), Overlay.Building, "Building")
        {
            _material = material;
            RemainingTime = 1f;
        }

        protected override void SolveInternal(GameObject actor)
        {
            World.At(Position).SetVoxel(_material);
        }
    }
}
