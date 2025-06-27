using System;
using Payosky.CoreMechanics.Inventory;

namespace ZumitoGame.PowerUps
{
    [Serializable]
    public struct AlfredoJump : IInventoryItem
    {
        public const string ID = "ZumitoGame.PowerUps.AlfredoJump";

        public string GetID()
        {
            return ID;
        }

        public void OnStored(string ownerID)
        {
        }

        public void OnRemoved(string ownerID)
        {
        }
    }
}