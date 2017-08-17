using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic.Farming;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class FarmTool : AreaTool {

        private JobController _jobController;

        protected void Awake()
        {
            _jobController = GameObject.Find("World").GetComponent<JobController>();
        }

        protected override void StartAction(IEnumerable<Vector3> voxels)
        {
            Farm farm = null;
            foreach (var vox in voxels)
            {
                if (CreateFarmAtPosition(vox))
                {
                    if (farm == null)
                    {
                        farm = new GameObject("Farm").AddComponent<Farm>();
                        farm.transform.parent = GameObject.Find("Map").transform;
                        farm.CropType = CropManager.Instance.GetCropByName("Wheat");
                    }
                    farm.AddFarmblock(vox);
                }
            }
        }

        protected override Color GetPreviewColor()
        {
            return new Color(0f, 0.5f, 0.5f, 0.5f);
        }

        private bool CreateFarmAtPosition(Vector3 pos)
        {
            if (_jobController.HasJob(pos, JobType.CreateSoil))
                return false;
            
            var type = Map.Instance.MapData.GetVoxelMaterial(pos);
            var air = MaterialRegistry.Instance.GetMaterialFromName("Air");
            if(type.Equals(air) || !(type.Equals(MaterialRegistry.Instance.GetMaterialFromName("Dirt")) || type.Equals(MaterialRegistry.Instance.GetMaterialFromName("Grass"))))
                return false;
            type = Map.Instance.MapData.GetVoxelMaterial(pos + Vector3.up);
            if (!type.Equals(air))
                return false;
            type = Map.Instance.MapData.GetVoxelMaterial(pos + Vector3.up + Vector3.up);
            if (!type.Equals(air))
                return false;
            _jobController.AddJob(new CreateSoilJob(pos));
            return true;
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(true, true, true);
        }
    }
}
