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

namespace Assets.Scripts.UI
{
    public class UITDScene : View
    {
        public void ClickStart()
        {
            var size = 129;
            while (Map.Instance.CreateMap(null, null).MoveNext()) ;
            var markers = new List<Transform>();
            for (var i = 0; i < PlaceRuneTool.MarkerParent.transform.childCount; i++)
                markers.Add(PlaceRuneTool.MarkerParent.transform.GetChild(i));
            var minX = (int)markers.Min(m => m.position.x) - 10;
            var minY = (int)markers.Min(m => m.position.z) - 10;
            var maxX = (int)markers.Max(m => m.position.x) + 10;
            var maxY = (int)markers.Max(m => m.position.z) + 10;

            var ds = new DiamondSquare(0.01f, size, size);
            var height = ds.Generate(new System.Random());


            var grass = MaterialRegistry.Instance.GetMaterialFromName("Grass");
            var dirt = MaterialRegistry.Instance.GetMaterialFromName("Dirt");
            for (var x = Mathf.Max(minX, 0); x < Mathf.Min(maxX, size); x++)
            {
                for (var y = Mathf.Max(minY, 0); y < Mathf.Min(maxY, size); y++)
                {
                    for(var h = 0; h <= (int)((height[x,y]*3) - 0.01f); h++)
                        World.At(x, h, y).SetVoxel(grass);
                }
            }

            markers = markers.OrderBy(m => m.gameObject.name).ToList<Transform>();
            
            var list = Bezier.GetBezierPoints(markers.Select(m => m.position).ToList(), 10f);
            for (var i = 1; i < list.Count; i++)
            {
                ResourceManager.DrawCapsule(list[i - 1], list[i], 3f, dirt, grass);
            }
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
