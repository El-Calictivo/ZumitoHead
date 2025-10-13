using Cysharp.Threading.Tasks;

namespace Payosky.CoreMechanics.PlayerController
{
    /// <summary>
    /// Defines the contract for a player's state within a player controller system.
    /// </summary>
    public interface IPlayerState
    {
        /// <summary>
        /// Called to initiate the state logic asynchronously when the state is entered.
        /// </summary>
        /// <returns>
        /// A UniTask representing the asynchronous operation of starting the state's functionality.
        /// </returns>
        UniTask Start();

        /// <summary>
        /// Executes logic that needs to run each frame while the player's state remains
        /// active. This method defines the behavior specific to the current state
        /// during the game's update loop, typically handling real-time input or dynamic
        /// adjustments.
        /// </summary>
        void Update()
        {
        }

        /// <summary>
        /// Handles physics-based updates for the player state. This method is called
        /// on a fixed interval, aligned with the physics update cycle of the engine.
        /// </summary>
        void FixedUpdate()
        {
        }

        /// <summary>
        /// Handles the logic for exiting the player's current state.
        /// This method should clean up resources and finalize state-specific operations.
        /// </summary>
        /// <returns>A UniTask that completes when the exit operations are finished.</returns>
        UniTask Exit();
    }
}