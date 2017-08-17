using System.Linq;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class Inventory : MonoBehaviour
    {
        public InventorySlot[] InventorySlots;
        public int SlotCount = 5;

        void Start()
        {
            InventorySlots = new InventorySlot[SlotCount];
            for(var i = 0; i < InventorySlots.Length; i++)
                InventorySlots[i] = new InventorySlot();
            
        }

        public int InsertItems(ItemType item, int amount)
        {
            foreach (var filled in InventorySlots.Where(sl => sl.ItemType != null && item.Equals(sl.ItemType)).OrderByDescending(sl => sl.Amount).ToArray())
            {
                amount = InsertIntoSlot(item, amount, filled);
                if (amount == 0)
                    return 0;
            }
            foreach (var inventorySlot in InventorySlots.Where(slot => slot.ItemType == null))
            {
                amount = InsertIntoSlot(item, amount, inventorySlot);
                if (amount == 0)
                    return 0;
            }
            return amount;
        }

        private int InsertIntoSlot(ItemType item, int amount, InventorySlot slot)
        {
            var free = item.StackSize - slot.Amount;
            var insert = Mathf.Min(free, amount);
            slot.Amount += insert;
            slot.ItemType = item;
            amount -= insert;
            return amount;
        }

        public int GetSpaceForItem(ItemType item)
        {
            var emptySlots = InventorySlots.Count(slot => slot.ItemType == null);
            var freeSpace = emptySlots * item.StackSize;
            foreach (var filled in InventorySlots.Where(slot => item.Equals(slot.ItemType)))
            {
                freeSpace += item.StackSize - filled.Amount;
            }
            return freeSpace;
        }
    }

    public class InventorySlot
    {
        public ItemType ItemType;
        public int Amount;
    }
}
