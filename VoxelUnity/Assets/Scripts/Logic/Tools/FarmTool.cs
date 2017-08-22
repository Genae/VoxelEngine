using System.Collections.Generic;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Logic.Farming;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic.Tools
{
    public class FarmTool : AreaTool {
        
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
            if (JobController.Instance.HasJob(pos, JobType.CreateSoil))
                return false;
            
            var wpos = World.At(pos);
            if(!(wpos.GetMaterial().Equals(MaterialRegistry.Instance.GetMaterialFromName("Dirt")) || wpos.GetMaterial().Equals(MaterialRegistry.Instance.GetMaterialFromName("Grass"))))
                return false;
            if (!World.At(pos + Vector3.up).IsAir())
                return false;
            if (!World.At(pos + Vector3.up + Vector3.up).IsAir())
                return false;
            JobController.Instance.AddJob(new CreateSoilJob(pos));
            return true;
        }

        public override void SwapOverlays()
        {
            OverlayManager.SwapOverlays(true, true, true);
        }
    }
}
