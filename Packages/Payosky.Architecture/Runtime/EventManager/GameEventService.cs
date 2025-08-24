using System;
using System.Collections.Generic;
using Payosky.Architecture.Services;

namespace Payosky.Architecture.EventManager
{
    public class GameEventService : IGameService
    {
        private readonly Dictionary<Type, Delegate> _eventTable = new();

        public void Register<T>(Action<T> callback) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (_eventTable.TryGetValue(eventType, out var existingDelegate))
            {
                _eventTable[eventType] = Delegate.Combine(existingDelegate, callback);
            }
            else
            {
                _eventTable[eventType] = callback;
            }
        }

        public void Unregister<T>(Action<T> callback) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (_eventTable.TryGetValue(eventType, out var existingDelegate))
            {
                var newDelegate = Delegate.Remove(existingDelegate, callback);
                if (newDelegate == null)
                {
                    _eventTable.Remove(eventType);
                }
                else
                {
                    _eventTable[eventType] = newDelegate;
                }
            }
        }

        public void Dispatch<T>(T gameEvent) where T : IGameEvent
        {
            var eventType = typeof(T);
            if (_eventTable.TryGetValue(eventType, out var del))
            {
                if (del is Action<T> callback)
                {
                    callback.Invoke(gameEvent);
                }
            }
        }

        public void ClearAll()
        {
            _eventTable.Clear();
        }
    }
}