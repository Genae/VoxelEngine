using Assets.Scripts.Logic.Farming;
using Assets.Scripts.Logic.Tools;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public class HarvestCropJob : Job
    {
        private readonly FarmBlock _block;

        public override JobType GetJobType()
        {
            return JobType.PlantCrop;
        }

        public HarvestCropJob(FarmBlock block) : base(block.Position + Vector3.up, 1.1f, new Color(0.7f, 0.7f, 0f, 0.5f), Overlay.Farming)
        {
            _block = block;
            RemainingTime = 1f;
        }

        protected override void SolveInternal(GameObject actor)
        {
            _block.Stage = 0;
        }
    }
}
