namespace RSLib.Jumble.FPSController
{
    using UnityEngine;

    /// <summary>
    /// Some example of a gameObject interactable by the FPS controller.
    /// </summary>
    public class FPSInteractableTest : FPSInteraction
    {
        public override void Focus()
        {
            base.Focus();
            transform.localScale = Vector3.one;
        }

        public override void Unfocus()
        {
            base.Unfocus();
            transform.localScale = Vector3.one * 0.7f;
        }

        public override void Interact()
        {
            base.Interact();
            FPSCameraShake.Instance.AddTrauma(0.25f);
        }
    }
}