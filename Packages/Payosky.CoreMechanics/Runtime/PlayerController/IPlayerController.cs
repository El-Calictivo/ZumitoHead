using System;
using Payosky.CoreMechanics.Damage;
using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.Runtime;
using UnityEngine;

namespace Payosky.CoreMechanics.PlayerController
{
    public interface IPlayerController : IRespawnable, IGameEntity, IDamageable
    {
        //Properties
        Animator Animator { get; }
        IPlayerMovementController MovementController { get; set; }
        IPlayerRendererController RendererController { get; set; }

        //Events
        event Action OnEditorSelected;
    }
}