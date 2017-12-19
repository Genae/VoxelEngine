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

            var size = BuildEmptyMap(markers.Select(m => m.transform).ToList(), grass);
            
            CreatePath(markers.OfType<Raido>().ToList(), markers.OfType<Mannaz>().ToList(), dirt, grass);

            CreateFarms(markers.OfType<Jera>().ToList());

            CreateTowers(markers.OfType<Algiz>().ToList());

            CreateVillages(markers.OfType<Mannaz>().ToList());

            var wm = new GameObject("WaveManager").AddComponent<WaveManager>();
            wm.transform.parent = gameObject.transform;

            StartCoroutine(LoadGame(size));
        }

        private void CreateVillages(List<Mannaz> markers)
        {
            foreach (var m in markers)
            {
                Villages.Add(new TDVillage(m.gameObject));
            }
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

        private void CreatePath(List<Raido> markersPath, List<Mannaz> markersBase, VoxelMaterial dirt, VoxelMaterial grass)
        {
            var transforms = new List<Transform>();
            if(markersPath.Count > 0)
                transforms.AddRange(markersPath.OrderBy(m => m.Number).Select(m => m.transform));
            if(markersBase.Count > 0)
                transforms.AddRange(markersBase.Select(m => m.transform));
            if (transforms.Count < 2) return;

            var list = Bezier.GetBSplinePoints(transforms.Select(m => m.position).ToList(), 10f);
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
