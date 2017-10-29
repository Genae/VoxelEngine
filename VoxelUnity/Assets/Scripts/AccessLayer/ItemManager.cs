using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AccessLayer.Worlds;
using Assets.Scripts.Algorithms.Pathfinding.Utils;
using Assets.Scripts.EngineLayer;
using Assets.Scripts.EngineLayer.Objects;
using Assets.Scripts.EngineLayer.Voxels.Containers.Multiblock;
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
            var chunk = World.At(obj.transform.position).GetChunkData();
            chunk.RegisterSmallMultiblock(obj.GetComponentInChildren<Multiblock>(), (Vector3I)(obj.transform.position - chunk.Position));
            if (item.Inventory != null)
            {
                var inv = obj.AddComponent<Inventory>();
                inv.SetSlotCount(item.Inventory.SlotAmount);
                Inventories.Add(inv);
            }
            if (!string.IsNullOrEmpty(item.Connection.Model))
            {
                var ctn = obj.AddComponent<ConnectToNeighbours>();
                ctn.ItemType = item;
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

        public static object PlaceItemOfType(string itemTypeName, Vector3 pos)
        {
            var itemType = GetItemType(itemTypeName);
            var obj = new GameObject(itemTypeName);
            obj.transform.position = pos;
            var model = GetModel(pos, itemType);
            model.transform.parent = obj.transform;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localPosition = Vector3.zero;
            ActivateObject(obj, itemType);
            return obj;
        }
    }

    public class ConnectToNeighbours : MonoBehaviour
    {
        public ItemType ItemType;
        void Start()
        {
            for (var i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        if (World.At((Vector3I)(transform.position) + Vector3.right).GetObject() == null)
                            continue;
                        break;
                    case 1:
                        if (World.At((Vector3I)(transform.position) + Vector3.back).GetObject() == null)
                            continue;
                        break;
                    case 2:
                        if (World.At((Vector3I)(transform.position) + Vector3.left).GetObject() == null)
                            continue;
                        break;
                    case 3:
                        if (World.At((Vector3I)(transform.position) + Vector3.forward).GetObject() == null)
                            continue;
                        break;
                }

                var rotate = new GameObject("Connection " + i);
                rotate.transform.parent = transform;
                rotate.transform.localPosition = Vector3.zero;
                var connector = MultiblockLoader.LoadMultiblock(ItemType.Connection.Model, transform.position).gameObject;
                connector.transform.parent = rotate.transform;
                connector.transform.localPosition = ItemType.Connection.Offset;
                rotate.transform.RotateAround(transform.position + new Vector3(0.05f, 0, 0.05f), Vector3.up, 90 * i);
            }
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
        public Connection Connection;
        public InventoryConfig Inventory;
        public Recipe Recipe;
        public CraftingStationConfig CraftingStationConfig;
    }

    public class Connection
    {
        public string Model;
        public Vector3 Offset;
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