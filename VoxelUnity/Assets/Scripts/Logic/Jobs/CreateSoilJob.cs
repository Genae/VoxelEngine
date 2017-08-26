using Assets.Scripts.AccessLayer;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic.Tools;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public class CreateSoilJob : PositionedJob
    {
        public override JobType GetJobType()
        {
            return JobType.CreateSoil;
        }

        public CreateSoilJob(Vector3 position) : base(position, 1.1f, new Color(0f, 0.3f, 0.3f, 0.5f), Overlay.Farming)
        {
            RemainingTime = 1f;
        }

        protected override void SolveInternal(GameObject actor)
        {
            World.At(Position).SetVoxel(MaterialRegistry.Instance.GetMaterialFromName("Soil"));
        }
    }
}
