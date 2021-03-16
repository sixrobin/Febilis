namespace Templar
{
    using UnityEngine;

    public class DestroySelf : MonoBehaviour
    {
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}