namespace RSLib.Jumble.FPSController
{
    using UnityEngine;

    /// <summary>
    /// Main class for the FPS controller. Controls the more generic things of the controller.
    /// </summary>
    public class FPSMaster : MonoBehaviour
    {
        [SerializeField] private FPSControllableComponent[] _allComponents = null;

        /// <summary>
        /// Enables all the controllable components of the FPS controller.
        /// </summary>
        public void EnableAll()
        {
            for (int i = _allComponents.Length - 1; i >= 0; --i)
                _allComponents[i].Controllable = true;
        }

        /// <summary>
        /// Disables all the controllable components of the FPS controller.
        /// </summary>
        public void DisableAll()
        {
            for (int i = _allComponents.Length - 1; i >= 0; --i)
                _allComponents[i].Controllable = false;
        }
    }
}