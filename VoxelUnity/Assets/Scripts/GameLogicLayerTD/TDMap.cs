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
using Assets.Scripts.GameLogicLayer.Tools;
using Assets.Scripts.GameLogicLayerTD.Runes;
using UnityEngine;

namespace Assets.Scripts.GameLogicLayerTD
{
    public class TDMap : MonoBehaviour
    {
        public static TDMap Instance;
        public List<TDTower> Towers = new List<TDTower>();
        public List<TDFarm> Farms = new List<TDFarm>();
        public List<Vector3> Path = new List<Vector3>();

        void Awake()
        {
            Instance = this;
        }
        public void BuildMap()
        {
            var markers = FindObjectsOfType<Rune>().Select(r => r.transform).ToList();
            var grass = MaterialRegistry.Instance.GetMaterialFromName("Grass");
            var dirt = MaterialRegistry.Instance.GetMaterialFromName("Dirt");

            var size = BuildEmptyMap(markers, grass);

            CreatePath(markers.Where(m => m.gameObject.name.Contains("Path") || m.gameObject.name.Contains("Village")).ToList(), dirt, grass);

            CreateFarms(markers.Where(m => m.gameObject.name.Contains("Farm")).ToList());

            CreateTowers(markers.Where(m => m.gameObject.GetComponentInChildren<Algiz>() != null).ToList());

            var wm = new GameObject("WaveManager").AddComponent<WaveManager>();
            wm.transform.parent = gameObject.transform;

            StartCoroutine(LoadGame(size));
        }
        
        private static int[] BuildEmptyMap(List<Transform> markers, VoxelMaterial grass)
        {
            var size = 129;
            while (Map.Instance.CreateMap(null, null).MoveNext()) ;
            var minX = (int)markers.Min(m => m.position.x) - 10;
            var minY = (int)markers.Min(m => m.position.z) - 10;
            var maxX = (int)markers.Max(m => m.position.x) + 10;
            var maxY = (int)markers.Max(m => m.position.z) + 10;

            var ds = new DiamondSquare(0.01f, size, size);
            var height = ds.Generate(new System.Random());


            for (var x = Mathf.Max(minX, 0); x < Mathf.Min(maxX, size); x++)
            {
                for (var y = Mathf.Max(minY, 0); y < Mathf.Min(maxY, size); y++)
                {
                    for (var h = 0; h <= (int)((height[x, y] * 3) - 0.01f); h++)
                       World.At(x, h, y).SetVoxel(grass);
                }
            }
            return new[] { maxX - minX, maxY - minY };
        }
        
        private IEnumerator LoadGame(int[] size)
        {
            yield return null;
            var biomeConfig = ConfigImporter.GetAllConfigs<BiomeConfiguration>("World/Biomes").First();
            var treeManager = new TreeManager();
            yield return treeManager.GenerateTrees((int)(size[0] * size[1] * 0.003f), Map.Instance.MapData, null);
            AmbientManager.SpawnAmbientPlants(biomeConfig);
        }

        private void CreateFarms(List<Transform> markers)
        {
            foreach (var m in markers)
            {
                Farms.Add(new TDFarm(m.position));
            }

        }

        private void CreateTowers(List<Transform> markers)
        {
            foreach (var m in markers)
            {
                Towers.Add(new TDTower(m.gameObject));
            }
        }

        private void CreatePath(List<Transform> markers, VoxelMaterial dirt, VoxelMaterial grass)
        {
            if (markers.Count < 2) return;
            markers = markers.OrderBy(m => m.gameObject.name).ToList();

            var list = Bezier.GetBSplinePoints(markers.Select(m => m.position).ToList(), 10f);
            Path = BuildWalkablePath(list);
            for (var i = 1; i < list.Count; i++)
            {
                ResourceManager.DrawCapsule(list[i - 1], list[i], 3f, dirt, grass);
            }
        }

        private List<Vector3> BuildWalkablePath(List<Vector3> list)
        {
            return list.Select(p => p +Vector3.up*3).ToList();
        }
    }
}
