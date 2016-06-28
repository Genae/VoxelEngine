using System.Collections.Generic;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Data.Material;
using UnityEngine;

namespace Assets.Scripts.Data.Multiblock
{
    public class TreeManager
    {
        public List<Tree> TreeList;
        
        public TreeManager()
        {
            TreeList = new List<Tree>();
        }

        public void GenerateTree(Vector3 position, MapData mapData)
        {
            var treeData = GetRandomizedTreeValues();
            var tree = new Tree(position);

            var strainVoxels = GenerateStrain(position, mapData, treeData);
            tree.AddVoxelListToMultiblock(strainVoxels, MaterialRegistry.Wood);

            var topOfStain = new Vector3(position.x - treeData.TreeTopDia / 2f + (treeData.TreeStainDia) / 2f, position.y + treeData.TreeStainHeight, position.z - treeData.TreeTopDia / 2f + (treeData.TreeStainDia) / 2f);
            var treeTopVoxels = GenerateTreeTop(topOfStain, mapData, treeData);
            tree.AddVoxelListToMultiblock(treeTopVoxels, MaterialRegistry.Leaves);

            tree.InstantiateVoxels(mapData);
            TreeList.Add(tree);
        }

        private List<Vector3> GenerateStrain(Vector3 pos, MapData mapData, TreeData treeData)
        {
            var list = new List<Vector3>();

            for(var i = 0; i < treeData.TreeStainHeight; i++)
            {
                for(var x = 0; x <= treeData.TreeStainDia; x++)
                {
                    for (var z = 0; z <= treeData.TreeStainDia; z++)
                    {
                        if (IsInBounds(mapData, (int)pos.x + x, (int)pos.y + i, (int)pos.z + z)) list.Add(new Vector3(pos.x + x, pos.y + i, pos.z + z)); ;
                    }
                }
            }

            return list;
        }

        private List<Vector3> GenerateTreeTop(Vector3 pos, MapData mapData, TreeData treeData)
        {
            var list = new List<Vector3>();

            for (var i = 0; i <= treeData.TreeTopHeight; i++)
            {
                for (var x = 0; x <= treeData.TreeTopDia; x++)
                {
                    for (var z = 0; z <= treeData.TreeTopDia; z++)
                    {
                        if (IsInBounds(mapData, (int)pos.x + x, (int)pos.y + i, (int)pos.z + z))
                        {
                            if (x == 0 && z == 0 || x == treeData.TreeTopDia && z == 0 || z == treeData.TreeTopDia && x == 0 || x == treeData.TreeTopDia && z == treeData.TreeTopDia) continue;
                            if (i == 0 && x == 0 || i == treeData.TreeTopHeight && x == 0 || i == 0 && z == 0 || i == treeData.TreeTopHeight && z == 0) continue;
                            if (i == 0 && x == treeData.TreeTopDia || i == treeData.TreeTopHeight && x == treeData.TreeTopDia || i == 0 && z == treeData.TreeTopDia || i == treeData.TreeTopHeight && z == treeData.TreeTopDia) continue;
                            list.Add(new Vector3(pos.x + x, pos.y + i, pos.z + z));
                        }
                    }
                }
            }

            return list;
        }

        private TreeData GetRandomizedTreeValues()
        {
            //TODO improve
            var stainDiaMod = (int)Random.Range(-2f, 2f);
            var stainHeightMod = (int)Random.Range(-2f, 5f);
            var topMod = (int)Random.Range(-2f, 5f);

            var treeData = GetDefaultTreeValues();
            treeData.TreeStainDia += stainDiaMod;
            treeData.TreeStainHeight += stainHeightMod;
            treeData.TreeTopHeight += topMod;
            treeData.TreeTopDia += topMod;
            return treeData;
        }

        private TreeData GetDefaultTreeValues()
        {
            return new TreeData
            {
                TreeTopDia = 11,
                TreeTopHeight = 8,
                TreeStainDia = 3,
                TreeStainHeight = 7,
            };
        }


        private bool IsInBounds(MapData mapData, int x, int y, int z)
        {
            return x > 0 && y > 0 && z > 0 && x < mapData.Size * Chunk.ChunkSize && y < mapData.Height * Chunk.ChunkSize && z < mapData.Size * Chunk.ChunkSize;
        }
    }

    struct TreeData
    {
        public int TreeTopDia;
        public int TreeTopHeight;
        public int TreeStainDia;
        public int TreeStainHeight;
    }
}

