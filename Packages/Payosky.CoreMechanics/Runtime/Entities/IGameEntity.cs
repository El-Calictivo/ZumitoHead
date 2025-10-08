using UnityEngine;

namespace Payosky.CoreMechanics.GameEntitites
{
    public interface IGameEntity
    {
        public string EntityID { get; }
        GameObject GameObject { get; }
        void RegisterEntity();
        void UnregisterEntity();
    }
}