using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Multiblock;
using Assets.Scripts.Logic.Jobs;
using Assets.Scripts.MultiblockHandling;
using UnityEngine;

namespace Assets.Scripts.Logic.Farming
{
    public class Farm : MonoBehaviour
    {
        public List<FarmBlock> FarmBlocks = new List<FarmBlock>();
        private static VoxelMaterial _soil;
        public CropType CropType;

        void Start()
        {
            if (_soil == null)
                _soil = MaterialRegistry.Instance.GetMaterialFromName("Soil");
        }
        void Update()
        {
            var jobController = GameObject.Find("World").GetComponent<JobController>();
            foreach (var farmBlock in FarmBlocks.ToArray())
            {
                if (!Map.Instance.MapData.GetVoxelMaterial(farmBlock.Position).Equals(_soil))
                {
                    if (!jobController.HasJob(farmBlock.Position, JobType.CreateSoil))
                    {
                        Destroy(farmBlock.Crop.gameObject);
                        FarmBlocks.Remove(farmBlock);
                        if (FarmBlocks.Count == 0)
                            Destroy(this);
                    }
                }
                else
                {
                    farmBlock.Update(Time.deltaTime);
                }
            }
        }

        public void AddFarmblock(Vector3 position)
        {
            FarmBlocks.Add(new FarmBlock(position, CropType, transform));
        }
    }

    public class FarmBlock
    {
        public Vector3 Position;
        public float TimeToGrow;

        public CropType Type;

        public Multiblock Crop;
        private int _stage;
        private Transform _parent;

        public int Stage
        {
            get { return _stage; }
            set
            {
                _stage = value;
                TimeToGrow = Type.StageGrowTime;
                UpdateModel();
            }
        }

        private void UpdateModel()
        {
            if (Crop != null)
                Object.Destroy(Crop.gameObject);
            
            if(Stage != 0)
                Crop = MultiblockLoader.LoadMultiblock(Type.GrowStages[_stage], Position + new Vector3(-0.5f, 0.5f, -0.5f), _parent);
        }

        public FarmBlock(Vector3 position, CropType cropType, Transform parent)
        {
            Position = position;
            Type = cropType;
            _parent = parent;
        }

        public void Update(float dTime)
        {
            if (Stage == 0)
            {
                var jobController = GameObject.Find("World").GetComponent<JobController>();
                if (!jobController.HasJob(Position + Vector3.up, JobType.PlantCrop))
                {
                    jobController.AddJob(new PlantCropJob(this));
                }
            }
            else
            {
                TimeToGrow -= dTime;
                if (TimeToGrow <= 0 && Stage < Type.GrowStages.Count)
                {
                    Stage += 1;
                    if (Stage == Type.GrowStages.Count)
                    {
                        var jobController = GameObject.Find("World").GetComponent<JobController>();
                        if (!jobController.HasJob(Position + Vector3.up, JobType.HarvestCrop))
                        {
                            jobController.AddJob(new HarvestCropJob(this));
                        }
                    }
                }
            }
        }
    }

    public class CropType
    {
        public Dictionary<int, string> GrowStages = new Dictionary<int, string>();
        public float StageGrowTime;
        public string Name;

        public CropType(string cropConfigName, float cropConfigStageTime)
        {
            StageGrowTime = cropConfigStageTime;
            Name = cropConfigName;
        }
    }
}
