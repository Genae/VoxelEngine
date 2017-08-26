using Assets.Scripts.AccessLayer.Jobs;
using Assets.Scripts.AccessLayer.Material;
using Assets.Scripts.AccessLayer.Tools;
using Assets.Scripts.AccessLayer.Worlds;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayer.Actions
{
    public class CreateSoilAction : SolveJobAction
    {
        public CreateSoilAction() : base("hasCreatedSoil", "CreateSoil", 1.5f)
        { }
    }

    public class CreateSoilJob : PositionedJob
    {
        public CreateSoilJob(Vector3 position) : base(position, 1.1f, new Color(0f, 0.3f, 0.3f, 0.5f), Overlay.Farming, "CreateSoil")
        {
            RemainingTime = 1f;
        }

        protected override void SolveInternal(GameObject actor)
        {
            World.At(Position).SetVoxel(MaterialRegistry.Instance.GetMaterialFromName("Soil"));
        }
    }
}
