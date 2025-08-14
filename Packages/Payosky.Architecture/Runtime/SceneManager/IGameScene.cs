namespace Payosky.Architecture.SceneManager
{
    public interface IGameScene
    {
        public string GetName();

        public void OnLoaded();

        public void OnUnloaded();
    }
}