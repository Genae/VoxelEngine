using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.EngineLayer.Voxels.Containers;
using Assets.Scripts.EngineLayer.Voxels.Containers.Chunks;
using Assets.Scripts.EngineLayer.Voxels.Data;
using Assets.Scripts.GameLogicLayer;
using Assets.Scripts.GameLogicLayer.Objects.TreeOfLife;
using Assets.Scripts.GameLogicLayer.Objects.Trees;
using UnityEngine;

namespace Assets.Scripts.AccessLayer
{
    public class TreeManager
    {
        public List<RandomizedTree> TreeList;
        public List<TreeConfig> TreeConfigList;
        
        public TreeManager()
        {
            TreeList = new List<RandomizedTree>();
            TreeConfigList = ConfigImporter.GetAllConfigs<TreeConfig>("World/Trees");
        }

        public void GenerateTree(Vector3 position)
        {
            var treeType = Random.Range(0, TreeConfigList.Count);
            TreeList.Add(new RandomizedTree(TreeConfigList[treeType], position));
        }

        public IEnumerator GenerateTrees(int amount, MapData map, GameLoader loader)
        {

            for (int i = 0; i < amount; i++)
            {
                loader.SetStatus("Spawning Trees", 0.85f + (i/(float)amount)*0.1f);
                var pos = new Vector3(Random.Range(0, map.Chunks.GetLength(0) * Chunk.ChunkSize), 1000, Random.Range(0, map.Chunks.GetLength(0) * Chunk.ChunkSize));
                RaycastHit hit;
                Physics.Raycast(new Ray(pos, Vector3.down), out hit, float.PositiveInfinity);
                if (hit.collider.tag.Equals("Chunk"))
                {
                    GenerateTree(hit.point);
                    yield return null;
                }
            }
        }

        public void BuildTreeOfLife()
        {
            var mapCenter = new Vector3(Map.Instance.MapData.Chunks.GetLength(0) * Chunk.ChunkSize / 2f, 1000, Map.Instance.MapData.Chunks.GetLength(2) * Chunk.ChunkSize / 2f);
            RaycastHit hit;
            Physics.Raycast(new Ray(mapCenter, Vector3.down), out hit, float.PositiveInfinity);
            if (hit.collider.tag.Equals("Chunk"))
            {
                var tree = new GameObject("TreeOfLife");
                var theRealTree = new GameObject("theRealTree");
                var comp = theRealTree.AddComponent<TreeOfLife>();
                theRealTree.transform.parent = tree.transform;
                tree.transform.position = hit.point;
                comp.GenerateTree();
            }
        }
    }

    public struct TreeData
    {
        public int TreeTopDia;
        public int TreeTopHeight;
        public int TreeStainDia;
        public int TreeStainHeight;
    }

    public class TreeConfig
    {
        public int TreeTopDia;
        public int TreeTopHeight;
        public int TreeStainDia;
        public int TreeStainHeight;
        public float[] StainDiaMod;
        public float[] StainHeightMod;
        public float[] TreeTopMod;
        public string LeafMaterial;
        public string StainMaterial;
    }
}
