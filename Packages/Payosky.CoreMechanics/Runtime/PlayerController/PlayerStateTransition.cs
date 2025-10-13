using System;

namespace Payosky.CoreMechanics.PlayerController
{
    public sealed class PlayerStateTransition
    {
        private readonly Func<bool> _validation;

        private readonly Action _transition;

        public PlayerStateTransition(Func<bool> validation, Action transition)
        {
            _validation = validation;
            _transition = transition;
        }

        public void Validate()
        {
            if (_validation?.Invoke() == true)
            {
                _transition?.Invoke();
            }
        }
    }
}