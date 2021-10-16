using System.Collections.Generic;
using AccessLayer;
using AccessLayer.Tools;
using AccessLayer.Worlds;
using GameLogicLayer.Actions;
using UnityEngine;

namespace GameLogicLayer.Tools
{
    public class DeleteTool : AreaTool
    {
        protected override void StartAction(IEnumerable<Vector3> voxels)
        {
            foreach (var vox in voxels)
            {
                ClearVoxelAtPosition(vox);
            }
        }

        protected override Color GetPreviewColor()
        {
            return new Color(1f, 0, 0, 0.5f);
        }

        private void ClearVoxelAtPosition(Vector3 pos)
        {
            if (JobController.Instance.HasJob(pos, "Mining"))
                return;
            
            if (World.At(pos).IsAir())
                return;
            JobController.Instance.AddJob(new MiningJob(pos));
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(true, true, false);
        }
    }
}
