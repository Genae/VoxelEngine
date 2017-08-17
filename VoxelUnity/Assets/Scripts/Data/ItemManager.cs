using System.Collections.Generic;
using Assets.Scripts.Data.Importer;
using Assets.Scripts.MultiblockHandling;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class ItemManager
    {
        private static Dictionary<string, ItemType> _items;
        public static void DropItem(Vector3 pos, ItemType item)
        {
            MultiblockLoader.LoadMultiblock(item.Model, pos - Vector3.one/2);
        }

        public static ItemType GetItemType(string name)
        {
            if (_items == null)
                _items = LoadItems();
            return _items[name];
        }

        private static Dictionary<string, ItemType> LoadItems()
        {
            _items = new Dictionary<string, ItemType>();
            var configs = ConfigImporter.GetConfig<ItemType[]>("Items");
            foreach (var config in configs)
            {
                foreach (var itemType in config)
                {
                    _items[itemType.Name] = itemType;
                }
            }
            return _items;
        }
    }

    public class ItemType
    {
        public string Name;
        public int StackSize;
        public string Model;
    }
}