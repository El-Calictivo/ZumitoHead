using Payosky.CoreMechanics.GameEntitites;

namespace Payosky.CoreMechanics.Inventory
{
    public interface IInventoryItem
    {
        string GetID();
        void OnStored(string ownerEntityID);
        void OnRemoved(string ownerEntityID);
    }
}