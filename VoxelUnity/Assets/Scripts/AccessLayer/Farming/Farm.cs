using System;
using System.Collections.Generic;
using AccessLayer.Jobs;
using AccessLayer.Material;
using AccessLayer.Worlds;
using EngineLayer.Voxels.Containers.Multiblock;
using EngineLayer.Voxels.Material;
using GameLogicLayer.Actions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AccessLayer.Farming
{
    public class Farm : MonoBehaviour
    {
        public List<FarmBlock> FarmBlocks = new List<FarmBlock>();
        private static VoxelMaterial _soil;
        public CropType CropType;

        void Start()
        {
            if (_soil == null)
            {
                _soil = MaterialRegistry.Instance.GetMaterialFromName("Soil");
            }
        }
        void Update()
        {
            foreach (var farmBlock in FarmBlocks.ToArray())
            {
                if (!World.At(farmBlock.Position + Vector3.up).IsAir())
                {
                    World.At(farmBlock.Position).SetVoxel("Dirt");
                    continue;
                }
                if (!World.At(farmBlock.Position).GetMaterial().Equals(_soil))
                {
                    if (!JobController.Instance.HasJob(farmBlock.Position, "CreateSoil"))
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

        public FarmBlock AddFarmblock(Vector3 position)
        {
            var block = new FarmBlock(position, CropType, transform);
            FarmBlocks.Add(block);
            return block;
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
            get => _stage;
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
                if (!JobController.Instance.HasJob(Position + Vector3.up, "PlantCrop"))
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
                        if (!JobController.Instance.HasJob(Position + Vector3.up, "HarvestCrop"))
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
