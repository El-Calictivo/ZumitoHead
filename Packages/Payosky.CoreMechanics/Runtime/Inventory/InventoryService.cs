using System.Collections.Generic;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;

namespace Payosky.CoreMechanics.Inventory
{
    public class InventoryService : IGameService
    {
        private Dictionary<IGameEntity, PlayerInventory> _playerInventories;

        public void Initialize()
        {
            _playerInventories = new Dictionary<IGameEntity, PlayerInventory>();
        }

        public void Dispose()
        {
            _playerInventories?.Clear();
        }

        public void AddItem(IGameEntity owner, IInventoryItem item)
        {
            var inventory = GetOrCreateInventory(owner);
            inventory.Add(item);
            item.OnStored(owner.GetID());
        }

        public void RemoveItemStack(IGameEntity owner, IInventoryItem item)
        {
            if (!_playerInventories.TryGetValue(owner, out var inventory)) return;
            inventory.RemoveStack(item);
            item.OnRemoved(owner.GetID());
        }

        private PlayerInventory GetOrCreateInventory(IGameEntity owner)
        {
            if (_playerInventories.TryGetValue(owner, out var inventory)) return inventory;

            inventory = new PlayerInventory();
            _playerInventories[owner] = inventory;

            return inventory;
        }
    }
}