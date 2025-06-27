using UnityEngine;

namespace Payosky.Architecture
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public T Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this as T;
                Initialize();
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            Instance = null;
            Dispose();
        }

        protected abstract void Initialize();
        protected abstract void Dispose();
    }
}