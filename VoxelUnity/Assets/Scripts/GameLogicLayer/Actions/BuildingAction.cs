using System.Threading;
using Assets.Scripts.AccessLayer.Jobs;
using Assets.Scripts.AccessLayer.Tools;
using Assets.Scripts.AccessLayer.Worlds;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Actions
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
