using Payosky.Architecture;
using Payosky.Architecture.EventManager;
using Payosky.Architecture.SceneManager;
using Payosky.Architecture.Services;

namespace ZumitoGame
{
    public class ZumitoGameController : GameController<ZumitoGameController>
    {
        protected override void Initialize()
        {
            ServiceLocator.Add(new SceneManagerService());
            ServiceLocator.Add(new GameEventService());
        }

        protected override void Dispose()
        {
            ServiceLocator.Dispose();
        }
    }
}