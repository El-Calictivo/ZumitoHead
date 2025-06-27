public interface IInventoryItem
{
    string GetID();
    void OnStored(string ownerID);
    void OnRemoved(string ownerID);
}