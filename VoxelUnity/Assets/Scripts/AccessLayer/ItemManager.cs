using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.EngineLayer.Objects;
using UnityEngine;

namespace Assets.Scripts.AccessLayer
{
    public class ItemManager
    {
        private static Dictionary<string, ItemType> _items;
        private static readonly List<Inventory> Inventories = new List<Inventory>();
        public static void DropItem(Vector3 pos, ItemType item)
        {
            GetModel(pos, item);
        }

        public static GameObject GetModel(Vector3 pos, ItemType item)
        {
            return MultiblockLoader.LoadMultiblock(item.Model, pos - Vector3.one / 2).gameObject;
        }
        public static void ActivateObject(GameObject obj, ItemType item)
        {
            obj.AddComponent<Item>();
            if (item.Inventory != null)
            {
                var inv = obj.AddComponent<Inventory>();
                inv.SetSlotCount(item.Inventory.SlotAmount);
                Inventories.Add(inv);
            }
        }

        public static ItemType GetItemType(string name)
        {
            if (_items == null)
                _items = LoadItems();
            return _items[name];
        }

        public static List<Inventory> GetInventoriesFor(ItemType items)
        {
            return Inventories.Where(i => i.GetSpaceForItem(items) > 0).ToList();
        }

        private static Dictionary<string, ItemType> LoadItems()
        {
            _items = new Dictionary<string, ItemType>();
            var configs = ConfigImporter.GetAllConfigs<ItemType[]>("Items");
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

    public class Item : MonoBehaviour
    {
        public Vector3I GetPosition()
        {
            return transform.parent.transform.position + Vector3.one/2f;
        }
    }

    public class ItemType
    {
        public string Name;
        public int StackSize;
        public string Model;
        public string Connector;
        public InventoryConfig Inventory;
        public Recipe Recipe;
        public CraftingStationConfig CraftingStationConfig;
    }

    public class CraftingStationConfig
    {
        public string[] CraftingTypes;
    }

    public class Recipe
    {
        public Dictionary<string, int> Ingredients;
        public Dictionary<string, int> JobRequirements;
        public string CraftingType;
        public int Experience;
    }

    public class InventoryConfig
    {
        public int SlotAmount;
    }
}