using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Multiblock;
using Assets.Scripts.Logic.Jobs;
using Assets.Scripts.MultiblockHandling;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Logic.Farming
{
    public class Farm : MonoBehaviour
    {
        public List<FarmBlock> FarmBlocks = new List<FarmBlock>();
        private static VoxelMaterial _soil;
        private static VoxelMaterial _air;
        public CropType CropType;

        void Start()
        {
            if (_soil == null)
            {
                _soil = MaterialRegistry.Instance.GetMaterialFromName("Soil");
                _air = MaterialRegistry.Instance.GetMaterialFromName("Air");
            }
        }
        void Update()
        {
            foreach (var farmBlock in FarmBlocks.ToArray())
            {
                if (!Map.Instance.MapData.GetVoxelMaterial(farmBlock.Position + Vector3.up).Equals(_air) || !Map.Instance.MapData.GetVoxelMaterial(farmBlock.Position + Vector3.up).Equals(_air))
                {
                    Map.Instance.MapData.SetVoxel((int)farmBlock.Position.x, (int)farmBlock.Position.y,
                        (int)farmBlock.Position.z, true, MaterialRegistry.Instance.GetMaterialFromName("Dirt"));
                    continue;
                }
                if (!Map.Instance.MapData.GetVoxelMaterial(farmBlock.Position).Equals(_soil))
                {
                    if (!JobController.Instance.HasJob(farmBlock.Position, JobType.CreateSoil))
                    {
                        farmBlock.Dispose();
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

    public class FarmBlock : IDisposable
    {
        public Vector3 Position;
        public float TimeToGrow;

        public CropType Type;

        public Multiblock Crop;
        public PositionedJob _currentJob;
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
                Crop = MultiblockLoader.LoadMultiblock(Type.GrowStages[_stage], Position + new Vector3(-0.5f, 0.5f, -0.5f), _parent, 1);
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
                if (!JobController.Instance.HasJob(Position + Vector3.up, JobType.PlantCrop))
                {
                    _currentJob = new PlantCropJob(this);
                    JobController.Instance.AddJob(_currentJob);
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
                        if (!JobController.Instance.HasJob(Position + Vector3.up, JobType.HarvestCrop))
                        {
                            _currentJob = new HarvestCropJob(this);
                            JobController.Instance.AddJob(_currentJob);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (Crop != null)
                Object.Destroy(Crop.gameObject);
            if(_currentJob != null)
                _currentJob.Abort();
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
