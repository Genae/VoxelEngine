﻿using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Logic.Jobs
{
    public class MiningJob : Job
    {
        public override JobType GetJobType()
        {
            return JobType.Mining;
        }

        public MiningJob(Vector3 position) : base(position, 1.1f, new Color(1f, 0f, 0f, 0.5f))
        {
            RemainingTime = 0.1f;
        }

        protected override void SolveInternal()
        {
            Map.MapData.Chunks[(int)Position.x/16, (int)Position.y/16, (int)Position.z/16]
                .SetVoxelType((int)Position.x%16, (int)Position.y%16, (int)Position.z%16, MaterialRegistry.Air);
        }
    }
}