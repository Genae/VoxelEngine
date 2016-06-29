using System.Collections.Generic;
using Assets.Scripts.Data.Multiblock.Trees;
using UnityEngine;
using Tree = Assets.Scripts.Data.Multiblock.Trees.Tree;

namespace Assets.Scripts.Data.Map
{
    public class TreeManager
    {
        public List<Tree> TreeList;
        
        public TreeManager()
        {
            TreeList = new List<Tree>();
        }

        public void GenerateTree(Vector3 position)
        {
            Tree tree = null;
            switch (Random.Range(0, 2))
            {
                case 0:
                    tree = new Oak(position);
                    break;
                case 1:
                    tree = new Birch(position);
                    break;
            }
            TreeList.Add(tree);
        }

        public void GenerateTrees(int amount, MapData map)
        {

            for (int i = 0; i < 100; i++)
            {
                var pos = new Vector3(Random.Range(0, map.Chunks.GetLength(0) * Chunk.ChunkSize), 1000, Random.Range(0, map.Chunks.GetLength(0) * Chunk.ChunkSize));
                RaycastHit hit;
                Physics.Raycast(new Ray(pos, Vector3.down), out hit, float.PositiveInfinity);
                if (hit.collider.tag.Equals("Chunk"))
                {
                    GenerateTree(hit.point);
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
}

