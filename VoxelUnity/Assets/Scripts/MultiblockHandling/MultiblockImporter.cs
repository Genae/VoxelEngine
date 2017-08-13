using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Data.Material;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.MultiblockHandling
{
    public class MultiblockImporter : MonoBehaviour
    {
        public int FractionValue = 10;
        public string FileName;
        public static List<VData> Imported;

        void Start()
        {
            Import(transform.GetChild(0), FileName ?? "flower.txt");
            var mb = MultiblockLoader.LoadMultiblock(FileName.Split('.')[0]);
            mb.transform.localScale = mb.transform.localScale * 10 / FractionValue;
        }

        //import wrapper function
        private void Import(Transform zone, string filename)
        {
            //defines imported area
            zone.tag = "Import";
            if(zone.GetComponent<MeshCollider>() == null)
            {
                //adding meshcollider for raycasting
                zone.gameObject.AddComponent<MeshCollider>();
            }
            //importing mesh to voxeldata
            Imported = GetVoxelData(zone);
            if (Imported.Count == 0) return;

            //dataPath is path to assets folder
            SaveVDataListToFile(Application.dataPath + "/Resources/Imported/", filename, Imported);
        }


        public void SaveVDataListToFile(string path, string filename, List<VData> list)
        {
            var jsonstring = JsonConvert.SerializeObject(list.ToArray());
            File.WriteAllText(path + filename, jsonstring);
        }

        private List<VData> GetVoxelData(Transform zone)
        {
            var offset = 0.5f;
            var voxelPosList = new List<VData>();
            var dirList = new List<Vector3>
            {
                Vector3.left,
                Vector3.right,
                Vector3.up,
                Vector3.down,
                Vector3.forward,
                Vector3.back
            };
            var bounds = zone.GetComponent<MeshFilter>().mesh.bounds;
            //Debug.Log(bounds.size.z);
            for (var x = 0; x < Mathf.Round(zone.transform.localScale.x * FractionValue * bounds.size.x + 0.5f); x++)
            {
                for (var y = 0; y < Mathf.Round(zone.transform.localScale.y * FractionValue * bounds.size.y + 0.5f); y++)
                {
                    for (var z = 0; z < Mathf.Round(zone.transform.localScale.z * FractionValue * bounds.size.z + 0.5f); z++)
                    {
                        var pos = new Vector3((x + offset) / FractionValue, (y + offset) / FractionValue, (z + offset) / FractionValue) + bounds.center - bounds.size/2;
                        Color color;
                        if (IsVoxelInModel(pos, dirList, out color))
                        {
                            color = MaterialRegistry.Instance.GetSimilarColor(color);
                            voxelPosList.Add(new VData(new DreierWecktor(x, y, z), new DreierWecktor((int)(color.r*256), (int)(color.g * 256), (int)(color.b * 256))));
                        }
                    }
                }
            }
            return voxelPosList;
        }

        private bool IsVoxelInModel(Vector3 pos, List<Vector3> dirList, out Color color)
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

    [Serializable]
    public class VData
    {
        public VData(DreierWecktor vp, DreierWecktor c)
        {
            VPos = vp;
            Color = c;
        }
        public DreierWecktor VPos;
        public DreierWecktor Color;
    }

    public class DreierWecktor
    {
        public float X;
        public float Y;
        public float Z;

        public DreierWecktor(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}

