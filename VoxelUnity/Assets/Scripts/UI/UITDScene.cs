using System.Collections;
using MarkLight;
using Assets.Scripts.AccessLayer.Worlds;
using Assets.Scripts.AccessLayer.Material;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Algorithms.MapGeneration;
using Assets.Scripts.GameLogicLayer.Tools;
using Assets.Scripts.AccessLayer;
using Assets.Scripts.AccessLayer.Farming;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.EngineLayer.Voxels.Material;
using Assets.Scripts.GameLogicLayer.Actions;
using Assets.Scripts.GameLogicLayerTD;

namespace Assets.Scripts.UI
{
    public class UITDScene : View
    {
        public void ClickStart()
        {
            var markers = new List<Transform>();
            var grass = MaterialRegistry.Instance.GetMaterialFromName("Grass");
            var dirt = MaterialRegistry.Instance.GetMaterialFromName("Dirt");

            var size = BuildEmptyMap(markers, grass);

            CreatePath(markers.Where(m => m.gameObject.name.Contains("Path") || m.gameObject.name.Contains("Village")).ToList(), dirt, grass);

            CreateFarms(markers.Where(m => m.gameObject.name.Contains("Farm")).ToList());


            StartCoroutine(LoadGame(size));
            
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
                var farm = new TDFarm(m.position);
            }

        }

        private static void CreatePath(List<Transform> markers, VoxelMaterial dirt, VoxelMaterial grass)
        {
            if (markers.Count < 2) return;
            markers = markers.OrderBy(m => m.gameObject.name).ToList();

            var list = Bezier.GetBSplinePoints(markers.Select(m => m.position).ToList(), 10f);
            for (var i = 1; i < list.Count; i++)
            {
                ResourceManager.DrawCapsule(list[i - 1], list[i], 3f, dirt, grass);
            }
        }

        private static int[] BuildEmptyMap(List<Transform> markers, VoxelMaterial grass)
        {
            var size = 129;
            while (Map.Instance.CreateMap(null, null).MoveNext()) ;
            for (var i = 0; i < PlaceRuneTool.MarkerParent.transform.childCount; i++)
                markers.Add(PlaceRuneTool.MarkerParent.transform.GetChild(i));
            var minX = (int) markers.Min(m => m.position.x) - 10;
            var minY = (int) markers.Min(m => m.position.z) - 10;
            var maxX = (int) markers.Max(m => m.position.x) + 10;
            var maxY = (int) markers.Max(m => m.position.z) + 10;

            var ds = new DiamondSquare(0.01f, size, size);
            var height = ds.Generate(new System.Random());


            for (var x = Mathf.Max(minX, 0); x < Mathf.Min(maxX, size); x++)
            {
                for (var y = Mathf.Max(minY, 0); y < Mathf.Min(maxY, size); y++)
                {
                    for (var h = 0; h <= (int) ((height[x, y] * 3) - 0.01f); h++)
                        World.At(x, h, y).SetVoxel(grass);
                }
            }
            return new[] {maxX - minX, maxY - minY};
        }

        public void ClickClear()
        {
            foreach (Transform child in Map.Instance.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
