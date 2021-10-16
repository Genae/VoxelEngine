using System.Collections.Generic;
using AccessLayer;
using AccessLayer.Material;
using AccessLayer.Tools;
using AccessLayer.Worlds;
using GameLogicLayer.Actions;
using UnityEngine;

namespace GameLogicLayer.Tools
{
    public class AddBlocksTool : AreaTool {
        
        public int BlockMaterialId = 1;

        protected override Color GetPreviewColor()
        {
            var c = MaterialRegistry.Instance.MaterialFromId(BlockMaterialId).Color;
            return new Color(c.r, c.g, c.b, 0.5f);
        }

        protected override void StartAction(IEnumerable<Vector3> voxels)
        {
            foreach (var vox in voxels)
            {
                ClearVoxelAtPosition(vox);
            }
        }

        private void ClearVoxelAtPosition(Vector3 pos)
        {
            if (JobController.Instance.HasJob(pos, "Building"))
                return;

            if (!World.At(pos).IsAir())
                return;
            JobController.Instance.AddJob(new BuildingJob(pos, MaterialRegistry.Instance.MaterialFromId(BlockMaterialId)));
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(true, true, false);
        }
    }
}
