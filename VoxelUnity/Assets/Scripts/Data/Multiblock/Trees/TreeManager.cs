using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using UnityEngine;

namespace Assets.Scripts.Data.Multiblock.Trees
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
    }

    public struct TreeData
    {
        public int TreeTopDia;
        public int TreeTopHeight;
        public int TreeStainDia;
        public int TreeStainHeight;
    }
}

