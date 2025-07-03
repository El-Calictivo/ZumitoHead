using Payosky.Platformer;
using System;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.Inventory;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace ZumitoGame.PowerUps
{
    [Serializable]
    public struct AlfredoJump : IInventoryItem
    {
        public const string ID = "ZumitoGame.PowerUps.AlfredoJump";

        [SerializeReference] [SubclassSelector]
        private JumpSubsystem alfredoJumpSubsystem;

        private JumpSubsystem _previousJumpSubsystem;

        public string GetID()
        {
            return ID;
        }

        public void OnStored(string ownerID)
        {
            if (!ServiceLocator.TryGet(out GameEntityService gameEntityService) || !gameEntityService.TryGet(ownerID, out PlatformerPlayerController playerController)) return;
            _previousJumpSubsystem = playerController.MovementController.JumpSubsystem;
            playerController.MovementController.UnRegisterJumpSubsystem();
            playerController.MovementController.RegisterJumpSubsystem(alfredoJumpSubsystem);
        }

        public void OnRemoved(string ownerID)
        {
            if (!ServiceLocator.TryGet(out GameEntityService gameEntityService) || !gameEntityService.TryGet(ownerID, out PlatformerPlayerController playerController)) return;
            if (playerController.MovementController.JumpSubsystem.GetType() != alfredoJumpSubsystem.GetType()) return;
            playerController.MovementController.UnRegisterJumpSubsystem();
            playerController.MovementController.RegisterJumpSubsystem(_previousJumpSubsystem ?? new PlatformerDefaultJumpSubsystem());
            _previousJumpSubsystem = null;
        }
    }
}