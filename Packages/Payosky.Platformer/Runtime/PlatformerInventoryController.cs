using System;
using Payosky.PlayerController;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    [RequireComponent(typeof(PlatformerPlayerController))]
    public class PlatformerInventoryController : MonoBehaviour, IPlayerInventoryController
    {
        [SerializeReference] [SubclassSelector]
        private IStorableItem[] items = Array.Empty<IStorableItem>();
    }
}