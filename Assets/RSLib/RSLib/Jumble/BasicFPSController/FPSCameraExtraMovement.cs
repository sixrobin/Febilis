namespace RSLib.Jumble.FPSController
{
    using UnityEngine;

    /// <summary>
    /// This class can be extended to implement extra movements for the FPS camera, like head bobbing or shaking.
    /// Camera should then call the ApplyMovement method in its LateUpdate method, after its basic movement.
    /// </summary>
    public abstract class FPSCameraExtraMovement : MonoBehaviour
    {
        /// <summary>
        /// Abstract method to implement to add an extra movement to the FPS camera (or any transform).
        /// </summary>
        public abstract void ApplyMovement();
    }
}