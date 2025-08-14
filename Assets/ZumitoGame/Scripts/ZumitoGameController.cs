using Payosky.Architecture;
using Payosky.Architecture.SceneManager;
using Payosky.Architecture.Services;

namespace ZumitoGame
{
    public class ZumitoGameController : GameController<ZumitoGameController>
    {
        protected override void Initialize()
        {
            ServiceLocator.Add(new SceneManagerService());
        }

        protected override void Dispose()
        {
            ServiceLocator.Dispose();
        }
    }
}