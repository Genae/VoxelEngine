using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data.Map;

namespace Assets.Scripts.Multiblock
{
    public class TreeManager
    {
        public List<Tree> TreeList;

        private int _treeTopDia = 11, _treeTopHeight = 8;
        private int _treeStainDia = 3, _treeStainHeight = 7;

        public TreeManager()
        {
            TreeList = new List<Tree>();
        }

        public void GenerateTree(Vector3 position, MapData mapData)
        {
            var strainVoxels = generateStrain(position);
            InstantiateVoxels(mapData, strainVoxels, 4);

            var treeTopVoxels = generateTreeTop(new Vector3(position.x - _treeTopDia/2 + (_treeStainDia - 1)/2, position.y + _treeStainHeight, position.z - _treeTopDia / 2 +(_treeStainDia - 1) / 2));
            InstantiateVoxels(mapData, treeTopVoxels, 5);

            var tree = new Tree(strainVoxels, position);
            tree.Multiblock.AddVoxelListToMultiblock(treeTopVoxels);
            TreeList.Add(tree);
        }

        private List<Vector3> generateStrain(Vector3 pos)
        {
            var list = new List<Vector3>();

            for(int i = 0; i < _treeStainHeight; i++)
            {
                for(int x = 0; x <= _treeStainDia; x++)
                {
                    for (int z = 0; z <= _treeStainDia; z++)
                    {
                        list.Add(new Vector3(pos.x + x, pos.y + i, pos.z + z));
                    }
                }
            }

            return list;
        }

        private List<Vector3> generateTreeTop(Vector3 pos)
        {
            var list = new List<Vector3>();

            for (int i = 0; i <= _treeTopHeight; i++)
            {
                for (int x = 0; x <= _treeTopDia; x++)
                {
                    for (int z = 0; z <= _treeTopDia; z++)
                    {
                        if (x == 0 && z == 0|| x == _treeTopDia && z == 0 ||z == _treeTopDia && x == 0 || x == _treeTopDia && z == _treeTopDia) continue;
                        if (i == 0 && x == 0 || i == _treeTopHeight && x == 0 || i == 0 && z == 0 || i == _treeTopHeight && z == 0 ) continue;
                        if (i == 0 && x == _treeTopDia || i == _treeTopHeight && x == _treeTopDia || i == 0 && z == _treeTopDia || i == _treeTopHeight && z == _treeTopDia) continue;
                        list.Add(new Vector3(pos.x + x, pos.y + i, pos.z + z));
                    }
                }
            }

            return list;
        }

        private void InstantiateVoxels(MapData mapData, List<Vector3> voxelList, int blockType)
        {
            foreach(var v in voxelList)
            {
                mapData.SetVoxel((int)v.x, (int)v.y, (int)v.z, new VoxelData(true, blockType));
            }
        }

    }
}

