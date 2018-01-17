using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.AccessLayer.Material;
using Assets.Scripts.AccessLayer.Worlds;
using Assets.Scripts.Algorithms.MapGeneration;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using Assets.Scripts.EngineLayer.Voxels.Material;
using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class TDMap : MonoBehaviour
    {
        public static TDMap Instance;

        public List<TDTower> Towers = new List<TDTower>();
        public List<TDVillage> Villages = new List<TDVillage>();
        public List<TDFarm> Farms = new List<TDFarm>();
        public List<Vector3> Path = new List<Vector3>();

        void Awake()
        {
            Instance = this;
        }

        public void BuildMap()
        {
            var markers = RuneRegistry.Runes.ToList();
            var grass = MaterialRegistry.Instance.GetMaterialFromName("Grass");
            var dirt = MaterialRegistry.Instance.GetMaterialFromName("Dirt");

            var mapInfo = CampaignManager.Instance.GetMapInfo();
            var size = GetSize(RuneRegistry.Runes.OfType<Raido>().ToList());
            var path = mapInfo.GetPath(size);
            BuildEmptyMap(size, grass);
            
            CreatePath(path, dirt, grass);

            CreateFarms(markers.OfType<Jera>().ToList());

            CreateTowers(markers.OfType<Algiz>().ToList());

            CreateVillages(markers.OfType<Mannaz>().ToList());

            var wm = new GameObject("WaveManager").AddComponent<WaveManager>();
            wm.transform.parent = gameObject.transform;

            StartCoroutine(LoadGame(new []{size.Width, size.Heigth}));
        }


        private void CreateVillages(List<Mannaz> markers)
        {
            foreach (var m in markers)
            {
                Villages.Add(new TDVillage(m.gameObject));
            }
        }

        public static MapSize GetSize(List<Raido> markers)
        {
            var minX = (int)markers.Min(m => m.transform.position.x);
            var minZ = (int)markers.Min(m => m.transform.position.z);
            var maxX = (int)markers.Max(m => m.transform.position.x);
            var maxZ = (int)markers.Max(m => m.transform.position.z);
            return new MapSize(minX, minZ, maxX, maxZ);
        }

        private static void BuildEmptyMap(MapSize mapSize, VoxelMaterial grass)
        {
            var size = Map.Instance.Size;
            while (Map.Instance.CreateMap(null, null).MoveNext()) ;
            

            var ds = new DiamondSquare(0.01f, size, size);
            var height = ds.Generate(new System.Random());

            Debug.Log(mapSize.MinX + "/" + mapSize.MaxX + "   " + mapSize.MinZ + "/" + mapSize.MaxZ);

            for (var x = Mathf.Max(mapSize.MinX, 0); x < Mathf.Min(mapSize.MaxX, size); x++)
            {
                for (var z = Mathf.Max(mapSize.MinZ, 0); z < Mathf.Min(mapSize.MaxZ, size); z++)
                {
                    for (var h = 0; h <= (int)(height[x, z] * 3 - 0.01f); h++)
                       World.At(x, h, z).SetVoxel(grass);
                }
            }
        }
        
        private IEnumerator LoadGame(int[] size)
        {
            yield return null;
            var biomeConfig = ConfigImporter.GetAllConfigs<BiomeConfiguration>("World/Biomes").First();
            var treeManager = new TreeManager();
            yield return treeManager.GenerateTrees((int)(size[0] * size[1] * 0.003f), Map.Instance.MapData, null);
            AmbientManager.SpawnAmbientPlants(biomeConfig);
            //Map.Instance.transform.parent.transform.localScale = Vector3.one * 0.05f;
        }

        private void CreateFarms(List<Jera> markers)
        {
            foreach (var m in markers)
            {
                Farms.Add(new TDFarm(m));
            }

        }

        private void CreateTowers(List<Algiz> markers)
        {
            foreach (var m in markers)
            {
                Towers.Add(new TDTower(m.gameObject));
            }
        }

        private void CreatePath(List<Vector3> positions, VoxelMaterial dirt, VoxelMaterial grass)
        {
            if (positions.Count < 2)
                return;
            positions = FixCurves(positions);
            var list = Bezier.GetBSplinePoints(positions, 10f);
            for (var i = 1; i < list.Count; i++)
            {
                ResourceManager.DrawCapsule(list[i - 1], list[i], 3f, dirt, grass);
            }
            Path = BuildWalkablePath(list);
        }

        private List<Vector3> FixCurves(List<Vector3> positions)
        {
            var fixedList = new List<Vector3> {positions[0]};
            for (var i = 1; i < positions.Count - 1; i++)
            {
                fixedList.Add(positions[i] + (positions[i - 1] - positions[i]) * 0.3f);
                fixedList.Add(positions[i]);
                fixedList.Add(positions[i] + (positions[i + 1] - positions[i]) * 0.3f);
            }
            fixedList.Add(positions[positions.Count-1]);
            return fixedList;
        }

        private List<Vector3> BuildWalkablePath(List<Vector3> list)
        {
            return list.Select(p => p + Vector3.up*GetHeightAt(p)).ToList();
        }

        private float GetHeightAt(Vector3 vector3)
        {
            return World.At(vector3).GetHeight();
        }

        public void Clear()
        {
            Destroy(Map.Instance.gameObject.GetComponent<TDMap>());
            foreach (Transform child in Map.Instance.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public class MapSize
    {
        public int MinX, MinZ, MaxX, MaxZ, Width, Heigth;

        public MapSize(int minX, int minZ, int maxX, int maxZ)
        {
            MinX = minX;
            MinZ = minZ;
            MaxX = maxX;
            MaxZ = maxZ;
            Width = maxX - minX;
            Heigth = maxZ - minZ;
        }
    }
}
