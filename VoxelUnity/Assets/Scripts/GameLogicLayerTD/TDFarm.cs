using Assets.Scripts.AccessLayer;
using Assets.Scripts.AccessLayer.Farming;
using Assets.Scripts.AccessLayer.Worlds;
using Assets.Scripts.GameLogicLayer.Actions;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class TDFarm
    {
        private Vector3 _position;

        public TDFarm(Vector3 position)
        {
            _position = position;
            var farm = new GameObject("Farm").AddComponent<Farm>();
            farm.transform.parent = GameObject.Find("Map").transform;
            farm.CropType = CropManager.Instance.GetCropByName("Wheat");

            var height = FlattenTerrain(position, 9);
            for (var i = -3; i <= 3; i++)
            {
                for (var j = -3; j <= 3; j++)
                {
                    var pos = new Vector3(position.x + i, height, position.z + j);
                    JobController.Instance.AddJob(new CreateSoilJob(pos));
                    farm.AddFarmblock(pos);
                }
            }
        }

        private float FlattenTerrain(Vector3 position, int size)
        {
            
            var heights = new float[size, size];
            var sum = 0f;

            for (var i = -size / 2; i <= size / 2; i++)
            {
                for (var j = -size / 2; j <= size / 2; j++)
                {
                    var pos = new Vector3(position.x + i, 0, position.z + j);
                    while (!World.At(pos + Vector3.up).IsAir())
                        pos += Vector3.up;

                    heights[i + size / 2, j + size / 2] = pos.y;
                    sum += heights[i + size / 2, j + size / 2];
                }
            }

            var height = sum / (size*size);

            for (var i = -size / 2; i <= size / 2; i++)
            {
                for (var j = -size / 2; j <= size / 2; j++)
                {
                    for (var h = 0; h < 10; h++)
                    {
                        var pos = new Vector3(position.x + i, h, position.z + j);
                        if(h <= height && World.At(pos).IsAir())
                            World.At(pos).SetVoxel("Grass");
                        if (h > height && !World.At(pos).IsAir())
                            World.At(pos).SetVoxel("Air");
                    }
                }
            }

            return height;
        }
    }
}
