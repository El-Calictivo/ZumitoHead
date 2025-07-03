using UnityEngine;

namespace Payosky.CoreMechanics.GameEntitites
{
    public interface IGameEntity
    {
        string GetID();
        void RegisterEntity();
        void UnregisterEntity();
    }
}