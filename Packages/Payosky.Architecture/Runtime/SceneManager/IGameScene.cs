namespace Payosky.Architecture.SceneManager
{
    public interface IGameScene
    {
        public string Name { get; }

        public void OnLoaded();

        public void OnUnloaded();
    }
}