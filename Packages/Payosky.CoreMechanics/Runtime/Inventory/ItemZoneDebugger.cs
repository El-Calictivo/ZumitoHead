using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using UnityEngine;
using UnityEngine.Serialization;

namespace Payosky.CoreMechanics.Inventory
{
    public class ItemZoneDebugger : MonoBehaviour
    {
        [SerializeReference] [SubclassSelector]
        private IInventoryItem item;

        [SerializeField]
        private bool removeOnExit;

        [SerializeField]
        private bool deactivateOnEnter;

        private void OnTriggerEnter(Collider other)
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.attachedRigidbody.TryGetComponent(out IGameEntity entity) && ServiceLocator.TryGet(out InventoryService inventoryService))
            {
                inventoryService.AddItem(entity, item);
                gameObject.SetActive(!deactivateOnEnter);
            }
        }

        private void OnTriggerExit(Collider other)
        {
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (removeOnExit && other.attachedRigidbody.TryGetComponent(out IGameEntity entity) && ServiceLocator.TryGet(out InventoryService inventoryService))
            {
                inventoryService.RemoveItemStack(entity, item);
            }
        }
    }
}