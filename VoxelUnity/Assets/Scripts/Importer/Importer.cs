using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data.Multiblock;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Map;
using System.Linq;

namespace Assets.Scripts.Importer
{
    public class Importer : MonoBehaviour
    {
        public int FractionValue = 10;
        private bool _once = false;

        void Update()
        {
            if (GameObject.Find("Map").GetComponent<Map>().IsDoneGenerating && !_once)
            {
                _once = true;
                Import(transform.GetChild(0));
            }
        }

        private void Import(Transform zone)
        {
            zone.tag = "Import";
            if(zone.GetComponent<MeshCollider>() == null)
            {
                zone.gameObject.AddComponent<MeshCollider>();
            }
            var list = getVoxelPositions(zone);
            if (list.Count == 0) return;
            var m = Multiblock.InstantiateVoxels(new Vector3(-1, 0, 0), new Dictionary<VoxelMaterial, List<Vector3>>() { { MaterialRegistry.Stone, list } });
            m.transform.localScale = Vector3.one / FractionValue;
            m.transform.position = new Vector3(0, 0, 0);
        }

        private List<Vector3> getVoxelPositions(Transform zone)
        {
            var offset = 0.5f;
            var voxelPosList = new List<Vector3>();
            var dirList = new List<Vector3>()
            {
                Vector3.left,
                Vector3.right,
                Vector3.up,
                Vector3.down,
                Vector3.forward,
                Vector3.back
            };
            var bounds = zone.GetComponent<MeshFilter>().mesh.bounds;
            Debug.Log(bounds.size.z);
            for (var x = 0; x < Mathf.Round(zone.transform.localScale.x * FractionValue * bounds.size.x + 0.5f); x++)
            {
                for (var y = 0; y < Mathf.Round(zone.transform.localScale.y * FractionValue * bounds.size.y + 0.5f); y++)
                {
                    for (var z = 0; z < Mathf.Round(zone.transform.localScale.z * FractionValue * bounds.size.z + 0.5f); z++)
                    {
                        var pos = new Vector3((x + offset) / FractionValue, (y + offset) / FractionValue, (z + offset) / FractionValue) + bounds.center - bounds.size/2;
                        if (isVoxelInModel(pos, dirList)) voxelPosList.Add(new Vector3(x, y, z));
                    }
                }
            }
            return voxelPosList;
        }

        private bool isVoxelInModel(Vector3 pos, List<Vector3> dirList)
        {
            foreach (var dir in dirList)
            {
                var hits = Physics.RaycastAll(pos, dir, Mathf.Infinity);
                var hitsrev = Physics.RaycastAll(pos + dir * 100, -dir, 100f);
                if (hits.Where(h => h.collider.tag == "Import").Count() + hitsrev.Where(h => h.collider.tag == "Import").Count() % 2 == 0) return false;
            }
            return true;
        }
    }
}

