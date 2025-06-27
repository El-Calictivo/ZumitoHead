using System;
using UnityEngine;

namespace Payosky.CoreMechanics.Inventory
{
    public class ItemZoneDebugger : MonoBehaviour
    {
        [SerializeReference] [SubclassSelector]
        private IInventoryItem item;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other.gameObject.name);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log(other.gameObject.name);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log(other.gameObject.name);
        }
    }
}