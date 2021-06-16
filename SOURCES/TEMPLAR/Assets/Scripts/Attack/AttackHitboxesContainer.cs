namespace Templar.Attack
{
    using RSLib.Extensions;
    using UnityEngine;

    public class AttackHitboxesContainer : MonoBehaviour
    {
        [SerializeField] private AttackHitbox[] _attackHitboxes = null;

        public AttackHitbox[] AttackHitboxes => _attackHitboxes;

        public void SetDirection(float dir)
        {
            transform.SetScaleX(Mathf.Sign(dir));
        }

        // [TODO] Editor button.
        [ContextMenu("Get Hitboxes in Children")]
        private void GetHitboxesInChildren()
        {
            _attackHitboxes = GetComponentsInChildren<AttackHitbox>();
        }
    }
}