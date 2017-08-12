using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Assets.Scripts.Data.Material;
using Assets.Scripts.Data.Multiblock;
using UnityEngine;

namespace Assets.Scripts.MultiblockHandling
{
    public static class MultiblockLoader
    {

        public static Multiblock LoadMultiblock(string filename)
        {
            //load vdata list from text file
            var list = LoadVDataListFromFile(Application.dataPath + "/Imported/", filename);

            //creating multiblock with vdata list
            var m = CreateMultiblock(list);
            m.transform.localScale = Vector3.one / 100; //10 is fraction value of importer, not very pretty atm
            m.transform.position = new Vector3(0, 0, 0);
            return m;
        }

        public static List<VData> LoadVDataListFromFile(string path, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<VData>));
            StreamReader reader = new StreamReader(path + filename);
            return (List<VData>)serializer.Deserialize(reader);
        }

        public static Multiblock CreateMultiblock(List<VData> list)
        {
            var dict = new Dictionary<VoxelMaterial, List<Vector3>>();

            foreach (var data in list)
            {
                var color = MaterialRegistry.Instance.GetColorIndex(data.Color);
                if (!dict.ContainsKey(color))
                    dict.Add(color, new List<Vector3>());
                dict[color].Add(data.VPos);
            }

            return Multiblock.InstantiateVoxels(new Vector3(-1, 0, 0), dict, "flowerpower");
        }
    }

}
