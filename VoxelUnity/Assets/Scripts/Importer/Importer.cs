using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data.Multiblock;

namespace Assets.Scripts.Importer
{
    public class Importer : MonoBehaviour
    {
        public int FractionValue = 10;
        

        void Start()
        {
            Import(transform.FindChild("Zone"));
        }

        private void Import(Transform zone)
        {
            var list = getVoxelPositions(zone);
        }

        private List<Vector3> getVoxelPositions(Transform zone)
        {
            var offset = FractionValue / 2;
            var voxelPosList = new List<Vector3>();
            var dirList = new List<Vector3>();
            dirList.Add(new Vector3(1, 0, 0));
            dirList.Add(new Vector3(-1, 0, 0));
            dirList.Add(new Vector3(0, 1, 0));
            dirList.Add(new Vector3(0, -1, 0));
            dirList.Add(new Vector3(0, 0, 1));
            dirList.Add(new Vector3(0, 0, -1));
            
            for (var x = zone.position.x; x < (int)zone.transform.localScale.x * FractionValue; x++)
            {
                for (var y = zone.position.y; y < (int)zone.transform.localScale.y * FractionValue; y++)
                {
                    for (var z = zone.position.z; z < (int)zone.transform.localScale.z * FractionValue; z++)
                    {
                        var pos = new Vector3((x + offset) / FractionValue, (y + offset) / FractionValue, (z + offset) / FractionValue);
                        if (isVoxelInModel(pos, dirList)) voxelPosList.Add(new Vector3(x / FractionValue, y / FractionValue, z / FractionValue));
                    }
                }
            }
            return voxelPosList;
        }

        private bool isVoxelInModel(Vector3 pos, List<Vector3> dirList)
        {
            var dirHitCount = 0;
            foreach (var dir in dirList)
            {
                var planeHitCount = 0;
                var hits = Physics.RaycastAll(pos, dir, Mathf.Infinity);
                Debug.DrawRay(pos, dir, Color.red, 100f);
                Debug.Log(hits.Length);
                if (hits.Length == 0) return false;
                for(int i = 0; i < hits.Length; i++)
                {
                    if(hits[0].collider != null)
                    {
                        planeHitCount++;
                    }
                    Debug.Log(planeHitCount);
                    if (planeHitCount % 2 == 0) return false;
                }
                dirHitCount++;
            }
            if (dirHitCount == 6) return true;
            return false;
        }
        
    }
}

