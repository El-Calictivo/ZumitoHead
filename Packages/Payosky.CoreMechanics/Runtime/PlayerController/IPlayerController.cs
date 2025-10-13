using System;
using Cysharp.Threading.Tasks;
using Payosky.CoreMechanics.Damage;
using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Payosky.CoreMechanics.PlayerController
{
    public interface IPlayerController : IRespawnable, IGameEntity, IDamageable
    {
        //Properties
        IPlayerState CurrentState { get; }
        IInputActionCollection2 InputActions { get; set; }
        Animator Animator { get; }
        Renderer Renderer { get; }
        IPlayerMovementController MovementController { get; set; }
        IPlayerRendererController RendererController { get; set; }

        //Events
        event Action OnEditorSelected;

        UniTask SetState(IPlayerState playerState);
    }
}