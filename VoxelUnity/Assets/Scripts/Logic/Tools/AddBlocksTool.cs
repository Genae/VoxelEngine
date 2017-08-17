using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class AddBlocksTool : AreaTool {
        
        public int BlockMaterialId = 1;
        private JobController _jobController;

        void Awake()
        {
            _jobController = GameObject.Find("World").GetComponent<JobController>();
        }


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
            if (_jobController.HasJob(pos, JobType.Building))
                return;

            var type = Map.Instance.MapData.GetVoxelMaterial(pos);
            if (!type.Equals(MaterialRegistry.Instance.GetMaterialFromName("Air")))
                return;
            _jobController.AddJob(new BuildingJob(pos, MaterialRegistry.Instance.MaterialFromId(BlockMaterialId)));
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(true, true, false);
        }
    }
}
