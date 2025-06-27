using System.Collections.Generic;

namespace Payosky.CoreMechanics.Inventory
{
    internal class PlayerInventory
    {
        private readonly Dictionary<string, InventoryItemStack> _items = new();

        public void Add(IInventoryItem item)
        {
            var id = item.GetID();
            if (!_items.TryAdd(id, new InventoryItemStack { Item = item, Quantity = 1 }))
            {
                _items[id].Quantity++;
            }
        }

        public void RemoveStack(IInventoryItem item)
        {
            _items.Remove(item.GetID());
        }
    }
}