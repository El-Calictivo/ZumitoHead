using Payosky.Architecture;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.Inventory;

namespace ZumitoGame
{
    public class ZumitoGameController : GameController<ZumitoGameController>
    {
        protected override void Initialize()
        {
            ServiceLocator.Add(new GameEntityService());
            ServiceLocator.Add(new InventoryService());
        }

        protected override void Dispose()
        {
            ServiceLocator.Dispose();
        }
    }
}