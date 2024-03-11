namespace RSLib.Jumble.DungeonGenerator
{
    using UnityEngine;

    [RequireComponent(typeof(CircleCollider2D))]
    public class DoorCrossTrigger : MonoBehaviour
    {
        [SerializeField] private DoorController _doorController = null;
        [SerializeField] private Collider2D _collider = null;

        public void Enable()
        {
            _collider.enabled = true;
        }

        public void Disable()
        {
            _collider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _doorController.CrossDoor();
            Disable();
        }
    }
}