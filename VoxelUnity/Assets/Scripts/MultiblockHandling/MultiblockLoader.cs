using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Multiblock;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.MultiblockHandling
{
    public static class MultiblockLoader
    {

        public static Multiblock LoadMultiblock(string filename)
        {
            //load vdata list from text file
            var list = LoadVDataListFromFile("Imported/", filename);

            //creating multiblock with vdata list
            var m = CreateMultiblock(list);
            m.transform.localScale = Vector3.one / 100; //10 is fraction value of importer, not very pretty atm
            m.transform.position = new Vector3(0, 0, 0);
            return m;
        }

        public static List<VData> LoadVDataListFromFile(string path, string filename)
        {
            var files = Resources.Load<TextAsset>(path + filename);
            string jsonstring = files.text;
            return JsonConvert.DeserializeObject<VData[]>(jsonstring).ToList();
        }

        public static Multiblock CreateMultiblock(List<VData> list)
        {
            var dict = new Dictionary<VoxelMaterial, List<Vector3>>();

            foreach (var data in list)
            {
                var color = MaterialRegistry.Instance.GetColorIndex(new Color(data.Color.X, data.Color.Y, data.Color.Z));
                if (!dict.ContainsKey(color))
                    dict.Add(color, new List<Vector3>());
                dict[color].Add(new Vector3(data.VPos.X, data.VPos.Y, data.VPos.Z));
            }

            return Multiblock.InstantiateVoxels(new Vector3(-1, 0, 0), dict, "flowerpower");
        }
    }

}
