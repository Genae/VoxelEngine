using Assets.Scripts.AccessLayer;
using Assets.Scripts.AccessLayer.Farming;
using Assets.Scripts.AccessLayer.Worlds;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class TDFarm
    {
        private Farm _farm;

        public TDFarm(Vector3 position)
        {
            var _size = 7;
            _farm = new GameObject("Farm").AddComponent<Farm>();
            _farm.transform.parent = GameObject.Find("Map").transform;
            _farm.CropType = CropManager.Instance.GetCropByName("Wheat");

            var height = FlattenTerrain(position, _size + 4);
            CreateSoil(position, height, _farm, _size);
            BuildFence(position, height, _size + 2);
            CreateCollider(position, height, _size + 2);
        }

        private void CreateCollider(Vector3 position, float height, int size)
        {
            var colliderGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderGO.transform.position = position + Vector3.up*height;
            colliderGO.transform.parent = _farm.transform;
            colliderGO.transform.localScale = new Vector3(size, 3, size);
            Object.Destroy(colliderGO.GetComponent<MeshRenderer>());
            colliderGO.AddComponent<Harvester>().Farm = this;
        }

        private void BuildFence(Vector3 position, float height, int size)
        {
            for (var i = -size / 2; i <= size / 2; i++)
            {
                for (var j = -size / 2; j <= size / 2; j++)
                {
                    if (i == -size / 2 || j == -size / 2 || i == size / 2 || j == size / 2)
                    {
                        var pos = new Vector3(position.x + i, height + 0.5f, position.z + j);
                        var fence = ObjectManager.PlaceItemOfType("Fence", pos);
                        fence.transform.parent = _farm.transform;
                    }
                }
            }
        }

        private static void CreateSoil(Vector3 position, float height, Farm farm, int size)
        {
            for (var i = -size / 2; i <= size / 2; i++)
            {
                for (var j = -size / 2; j <= size / 2; j++)
                {
                    var pos = new Vector3(position.x + i, height, position.z + j);
                    World.At(pos).SetVoxel("Soil");
                    farm.AddFarmblock(pos).Stage = 1;
                }
            }
        }

        public bool HarvestFarm()
        {
            if(_farm.FarmBlocks.TrueForAll(f => f.Stage == _farm.CropType.GrowStages.Count))
            foreach (var farmblock in _farm.FarmBlocks)
            {
                farmblock.Stage = 1;
            }
            return true;
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

            return (int)height;
        }
    }

    public class Harvester : MonoBehaviour
    {
        public TDFarm Farm;
        void OnMouseDown()
        {
            Farm.HarvestFarm();
        }
    }
}
