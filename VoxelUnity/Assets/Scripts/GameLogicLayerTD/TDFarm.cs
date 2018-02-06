using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.AccessLayer.Farming;
using Assets.Scripts.AccessLayer.Worlds;
using Assets.Scripts.EngineLayer.Util;
using Assets.Scripts.EngineLayer.Voxels.Containers.Multiblock;
using Assets.Scripts.GameLogicLayerTD.Runes;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class TDFarm
    {
        private Farm _farm;
        public Jera Marker;
        private List<GameObject> _fence = new List<GameObject>();
        public int Income;

        public TDFarm(Jera marker)
        {
            Marker = marker;
            var position = marker.transform.position;
            var _size = 13;
            _farm = new GameObject("Farm").AddComponent<Farm>();
            _farm.transform.parent = GameObject.Find("Map").transform;
            var fehu = Marker.GetUpgradeRunes().OfType<Fehu>();
            var ingwaz = Marker.GetUpgradeRunes().OfType<Ingwaz>();
            var sowilo = Marker.GetUpgradeRunes().OfType<Sowilo>();
            var wunjo = Marker.GetUpgradeRunes().OfType<Wunjo>();
            var uruz = Marker.GetUpgradeRunes().OfType<Uruz>().ToList();
            var height = FlattenTerrain(position, _size + 4);
            if (fehu.Any())
            {
                var c = ObjectManager.PlaceItemOfType("Cow", position + Vector3.up * (height + 0.5f));
                var cowComp = c.AddComponent<Cow>();
                cowComp.IncomeOnCooldown = 2;
                c = ObjectManager.PlaceItemOfType("Cow", position + Vector3.up * (height + 0.5f) + new Vector3(4, 0, 1));
                c = ObjectManager.PlaceItemOfType("Cow", position + Vector3.up * (height + 0.5f) + new Vector3(2, 0, 3));
                c.transform.RotateAround(Vector3.up, 90);
                c = ObjectManager.PlaceItemOfType("Cow", position + Vector3.up * (height + 0.5f) + new Vector3(0, 0, 2));
                c.transform.RotateAround(Vector3.up, 90);
                c = ObjectManager.PlaceItemOfType("Cow", position + Vector3.up * (height + 0.5f) + new Vector3(-2, 0, 2));
                c.transform.RotateAround(Vector3.up, 180);
                c = ObjectManager.PlaceItemOfType("Cow", position + Vector3.up * (height + 0.5f) + new Vector3(-2, 0, -3));
                c.transform.RotateAround(Vector3.up, 180);
                c = ObjectManager.PlaceItemOfType("Cow", position + Vector3.up * (height + 0.5f) + new Vector3(0, 0, -4));
                c.transform.RotateAround(Vector3.up, 270);
                if (uruz.Any())
                {
                    c = ObjectManager.PlaceItemOfType("Ox", position + Vector3.up * (height + 0.5f) + new Vector3(3, 0, -4));
                    c.transform.RotateAround(Vector3.up, 270);
                    cowComp.IncomeOnCooldown += uruz.Count();
                }
            }
            else
            {
                if (sowilo.Any())
                {
                    _farm.CropType = CropManager.Instance.GetCropByName("Sunflower");
                    Income = 130;
                    if (wunjo.Any())
                    {
                        Income = (int)(Income * Mathf.Pow(1.3f, wunjo.Count()));
                    }
                }
                else if (ingwaz.Any())
                {
                    _farm.CropType = CropManager.Instance.GetCropByName("Wine");
                    Income = 300;
                }
                else
                {
                    _farm.CropType = CropManager.Instance.GetCropByName("Wheat");
                    Income = 100;
                }
                CreateSoil(position, height, _farm, _size);
                CreateCollider(position, height, _size + 2);
            }
            BuildFence(position, height, _size + 2);
        }

        private void CreateCollider(Vector3 position, float height, int size)
        {
            var colliderGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderGO.transform.position = new Vector3(position.x, 0, position.z) + Vector3.up*height;
            colliderGO.transform.parent = _farm.transform;
            colliderGO.transform.localScale = new Vector3(size, 6, size);
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
                        _fence.Add(fence);
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
            if (!_farm.FarmBlocks.TrueForAll(f => f.Stage == _farm.CropType.GrowStages.Count))
                return false;
            ResourceOverview.Instance.Gold.Value += Income;
            foreach (var farmblock in _farm.FarmBlocks)
            {
                farmblock.Stage = 1;
            }
            return true;
        }

        public static float FlattenTerrain(Vector3 position, int size)
        {
            if (size % 2 == 0) size++;
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

        public void Explode()
        {
            foreach (var gameObject in _fence)
            {
                foreach (var multiblock in gameObject.GetComponentsInChildren<Multiblock>())
                {
                    Exploder.Explode(multiblock);
                }
            }
            foreach (var childs in _farm.GetComponentsInChildren<Transform>())
            {
                Object.Destroy(childs.gameObject);
            }
        }
    }

    public class Cow : MonoBehaviour
    {
        public float Cooldown;
        public int IncomeOnCooldown;
        void Update()
        {
            Cooldown -= Time.deltaTime;
            if (Cooldown <= 0)
            {
                Cooldown = 1;
                ResourceOverview.Instance.Gold.Value += IncomeOnCooldown;
            }
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
