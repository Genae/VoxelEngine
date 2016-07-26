using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data.Multiblock.Trees;
using UnityEngine;
using Assets.Scripts.Data.Importer;
using Tree = Assets.Scripts.Data.Multiblock.Trees.Tree;

namespace Assets.Scripts.Data.Map
{
    public class TreeManager
    {
        public List<Tree> TreeList;
        public List<TreeConfig> TreeConfigList;
        
        public TreeManager()
        {
            TreeList = new List<Tree>();
            TreeConfigList = ConfigImporter.GetConfig<TreeConfig>("Assets/Config/Trees");
        }

        public void GenerateTree(Vector3 position)
        {
            var treeType = Random.Range(0, TreeConfigList.Count);
            TreeList.Add(new Tree(TreeConfigList[treeType], position));
        }

        public IEnumerator GenerateTrees(int amount, MapData map)
        {

            for (int i = 0; i < amount; i++)
            {
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
