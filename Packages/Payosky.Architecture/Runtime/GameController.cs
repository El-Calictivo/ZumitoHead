namespace Payosky.Architecture
{
    public abstract class GameController<T> : Singleton<T> where T : GameController<T>
    {
    }
}