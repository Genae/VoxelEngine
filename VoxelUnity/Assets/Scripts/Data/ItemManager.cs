using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using Assets.Scripts.Data.Importer;
using Assets.Scripts.Logic;
using Assets.Scripts.MultiblockHandling;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class ItemManager
    {
        private static Dictionary<string, ItemType> _items;
        private static readonly List<Inventory> _inventories = new List<Inventory>();
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
            var i = obj.AddComponent<Item>();
            if (item.Inventory != null)
            {
                var inv = obj.AddComponent<Inventory>();
                inv.SetSlotCount(item.Inventory.SlotAmount);
                _inventories.Add(inv);
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
            return _inventories.Where(i => i.GetSpaceForItem(items) > 0).ToList();
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
        public InventoryConfig Inventory;
        public Recipe Recipe;
    }

    public class Recipe
    {
        public Dictionary<string, int> Ingredients;
        public Dictionary<string, int> JobRequirements;
        public CraftingType Type;
        public int Experience;
    }

    public class InventoryConfig
    {
        public int SlotAmount;
    }

    public enum CraftingType
    {
        Metal,
        Wood,
        Stone
    }
}