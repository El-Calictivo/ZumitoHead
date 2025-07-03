using System.Collections.Generic;
using Payosky.Architecture.Services;
using UnityEngine;

namespace Payosky.CoreMechanics.GameEntitites
{
    public class GameEntityService : IGameService
    {
        private Dictionary<string, IGameEntity> _entities;

        public void Initialize()
        {
            _entities = new Dictionary<string, IGameEntity>();
        }

        public bool TryGet(string entityID, out IGameEntity entityController)
        {
            _entities.TryGetValue(entityID, out var controller);
            entityController = controller;
            return controller != null;
        }

        public bool TryGet<T>(string entityID, out T entityController) where T : MonoBehaviour, IGameEntity
        {
            _entities.TryGetValue(entityID, out var controller);
            entityController = controller as T;
            return entityController;
        }

        public void AddEntity<T>(T controller) where T : MonoBehaviour, IGameEntity
        {
            _entities.TryAdd(controller.GetID(), controller);
        }

        public void RemoveEntity<T>(T controller) where T : MonoBehaviour, IGameEntity
        {
            _entities.Remove(controller.GetID());
        }

        public void Dispose()
        {
            _entities?.Clear();
        }
    }
}