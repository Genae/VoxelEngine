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
        public static List<VData> Imported;

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
            Imported = getVoxelData(zone);
            if (Imported.Count == 0) return;
            var m = CreateMultiblock(Imported);
            m.transform.localScale = Vector3.one / FractionValue;
            m.transform.position = new Vector3(0, 0, 0);
        }

        private Multiblock CreateMultiblock(List<VData> list)
        {
            var dict = new Dictionary<VoxelMaterial, List<Vector3>>();
            
            foreach (var data in list)
            {
                var color = Map.Instance.MaterialRegistry.GetColorIndex(data.Color);
                if (!dict.ContainsKey(color))
                dict.Add(color, new List<Vector3>());
                dict[color].Add(data.VPos);
            }

            return Multiblock.InstantiateVoxels(new Vector3(-1, 0, 0), dict);
        }

        private List<VData> getVoxelData(Transform zone)
        {
            var offset = 0.5f;
            var voxelPosList = new List<VData>();
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
                        Color color;
                        if (isVoxelInModel(pos, dirList, out color))
                        {
                            voxelPosList.Add(new VData(new Vector3(x, y, z), color));
                        }
                    }
                }
            }
            return voxelPosList;
        }

        private bool isVoxelInModel(Vector3 pos, List<Vector3> dirList, out Color color)
        {
            color = Color.magenta;
            foreach (var dir in dirList)
            {
                var hitsrev = Physics.RaycastAll(pos + dir/FractionValue, -dir, 1f / FractionValue);
                if(hitsrev.Count() > 0)
                {
                    var texCoord = hitsrev.First().textureCoord;
                    var mat = hitsrev.First().collider.gameObject.GetComponent<MeshRenderer>().material;
                    color = ((Texture2D)mat.mainTexture).GetPixelBilinear(texCoord.x, texCoord.y);
                    break;
                }
            }
            foreach (var dir in dirList)
            {
                var hits = Physics.RaycastAll(pos, dir, Mathf.Infinity);
                var hitsrev = Physics.RaycastAll(pos + dir * 100, -dir, 100f);
                if (hits.Where(h => h.collider.tag == "Import").Count() + hitsrev.Where(h => h.collider.tag == "Import").Count() % 2 == 0) return false;
            }
            return true;
        }
    }


    public struct VData
    {
        public VData(Vector3 vp, Color c)
        {
            VPos = vp;
            Color = c;
        }
        public Vector3 VPos;
        public Color Color;
    }
}

