using UnityEngine;

namespace Payosky.CoreMechanics.PlayerController
{
    public interface IPlayerRendererController : IPlayerControllerComponent
    {
        public void PlayAnimation(string animationID);
        public void SetAnimationSpeed(float speed);
        public void HandleVelocityDirection(Vector3 flipVector);
    }
}