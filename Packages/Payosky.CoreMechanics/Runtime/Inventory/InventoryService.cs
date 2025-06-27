using System.Collections.Generic;
using Payosky.Architecture.Services;

namespace Payosky.CoreMechanics.Inventory
{
    public class InventoryService : IGameService
    {
        private Dictionary<string, PlayerInventory> _playerInventories;

        public void Initialize()
        {
            _playerInventories = new Dictionary<string, PlayerInventory>();
        }

        public void Dispose()
        {
            _playerInventories?.Clear();
        }

        public void AddItem(string ownerID, IInventoryItem item)
        {
            var inventory = GetOrCreateInventory(ownerID);
            inventory.Add(item);
        }

        public void RemoveItemStack(string ownerID, IInventoryItem item)
        {
            if (_playerInventories.TryGetValue(ownerID, out var inventory))
            {
                inventory.RemoveStack(item);
                item.OnRemoved(ownerID);
            }
        }

        private PlayerInventory GetOrCreateInventory(string ownerID)
        {
            if (_playerInventories.TryGetValue(ownerID, out var inventory)) return inventory;

            inventory = new PlayerInventory();
            _playerInventories[ownerID] = inventory;

            return inventory;
        }
    }
}