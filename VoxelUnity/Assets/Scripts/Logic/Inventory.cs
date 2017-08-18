using System.Linq;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class Inventory : MonoBehaviour
    {
        public InventorySlot[] InventorySlots;
        public int SlotCount { get; private set; }

        void Start()
        {
            if (SlotCount == 0)
                SetSlotCount(5);
        }

        public void SetSlotCount(int count)
        {
            SlotCount = count;
            InventorySlots = new InventorySlot[SlotCount];
            for (var i = 0; i < InventorySlots.Length; i++)
                InventorySlots[i] = new InventorySlot();
        }

        public int InsertItems(ItemType item, int amount)
        {
            foreach (var filled in InventorySlots.Where(sl => sl.ItemType != null && item.Equals(sl.ItemType))
                .OrderByDescending(sl => sl.Amount).ToArray())
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

        public int GetItemCount(ItemType item)
        {
            return InventorySlots.Where(sl => item.Equals(sl.ItemType)).Sum(sl => sl.Amount);
        }

        public bool IsEmpty()
        {
            return InventorySlots.Any(s => s.ItemType != null);
        }

        public ItemType First()
        {
            return InventorySlots.First(s => s.ItemType != null).ItemType;
        }

        public void TransferItemTo(ItemType item, Inventory inventory)
        {
            if (GetItemCount(item) == 0)
                return;
            var rest = inventory.InsertItems(item, 1);
            if (rest == 0)
                RemoveItem(item);
        }

        private void RemoveItem(ItemType item)
        {
            var slot = InventorySlots.Where(sl => sl.ItemType != null && item.Equals(sl.ItemType))
                .OrderBy(sl => sl.Amount).First();
            slot.Amount--;
            if (slot.Amount == 0)
                slot.ItemType = null;
        }
    }

    public class InventorySlot
    {
        public ItemType ItemType;
        public int Amount;
    }
}
