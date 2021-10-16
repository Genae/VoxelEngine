using System.Linq;
using AccessLayer;
using UnityEngine;

namespace EngineLayer.Objects
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

        public int InsertItems(ObjectType @object, int amount)
        {
            foreach (var filled in InventorySlots.Where(sl => sl.ObjectType != null && @object.Equals(sl.ObjectType))
                .OrderByDescending(sl => sl.Amount).ToArray())
            {
                amount = InsertIntoSlot(@object, amount, filled);
                if (amount == 0)
                    return 0;
            }
            foreach (var inventorySlot in InventorySlots.Where(slot => slot.ObjectType == null))
            {
                amount = InsertIntoSlot(@object, amount, inventorySlot);
                if (amount == 0)
                    return 0;
            }
            return amount;
        }

        private int InsertIntoSlot(ObjectType @object, int amount, InventorySlot slot)
        {
            var free = @object.StackSize - slot.Amount;
            var insert = Mathf.Min(free, amount);
            slot.Amount += insert;
            slot.ObjectType = @object;
            amount -= insert;
            return amount;
        }

        public int GetSpaceForItem(ObjectType @object)
        {
            var emptySlots = InventorySlots.Count(slot => slot.ObjectType == null);
            var freeSpace = emptySlots * @object.StackSize;
            foreach (var filled in InventorySlots.Where(slot => @object.Equals(slot.ObjectType)))
            {
                freeSpace += @object.StackSize - filled.Amount;
            }
            return freeSpace;
        }

        public int GetItemCount(ObjectType @object)
        {
            return InventorySlots.Where(sl => @object.Equals(sl.ObjectType)).Sum(sl => sl.Amount);
        }

        public bool IsEmpty()
        {
            return InventorySlots.Any(s => s.ObjectType != null);
        }

        public ObjectType First()
        {
            return InventorySlots.First(s => s.ObjectType != null).ObjectType;
        }

        public void TransferItemTo(ObjectType @object, Inventory inventory)
        {
            if (GetItemCount(@object) == 0)
                return;
            var rest = inventory.InsertItems(@object, 1);
            if (rest == 0)
                RemoveItem(@object);
        }

        private void RemoveItem(ObjectType @object)
        {
            var slot = InventorySlots.Where(sl => sl.ObjectType != null && @object.Equals(sl.ObjectType))
                .OrderBy(sl => sl.Amount).First();
            slot.Amount--;
            if (slot.Amount == 0)
                slot.ObjectType = null;
        }
    }

    public class InventorySlot
    {
        public ObjectType ObjectType;
        public int Amount;
    }
}
