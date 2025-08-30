namespace Payosky.Architecture.SceneManager
{
    public abstract class BaseGameScene<T> where T : IGameScene
    {
        public string Name => typeof(T).Name;
    }
}