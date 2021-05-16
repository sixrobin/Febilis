namespace Templar
{
    using UnityEngine;

    public class DisableSelf : MonoBehaviour
    {
        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}