using Payosky.CoreMechanics.Damage;

namespace Payosky.CoreMechanics.GameEntitites
{
    public interface IGameEntity : IDamageable
    {
        string GetID();
        void RegisterEntity();
        void UnregisterEntity();
    }
}