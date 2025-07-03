using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using UnityEngine;

namespace Payosky.CoreMechanics.Inventory
{
    public class ItemZoneDebugger : MonoBehaviour
    {
        [SerializeReference] [SubclassSelector]
        private IInventoryItem item;

        private void OnTriggerEnter(Collider other)
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.attachedRigidbody.TryGetComponent(out IGameEntity entity) && ServiceLocator.TryGet(out InventoryService inventoryService))
            {
                inventoryService.AddItem(entity, item);
            }
        }

        private void OnTriggerExit(Collider other)
        {
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.attachedRigidbody.TryGetComponent(out IGameEntity entity) && ServiceLocator.TryGet(out InventoryService inventoryService))
            {
                inventoryService.RemoveItemStack(entity, item);
            }
        }
    }
}