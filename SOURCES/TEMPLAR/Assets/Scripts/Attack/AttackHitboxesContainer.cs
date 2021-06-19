namespace Templar.Attack
{
    using RSLib.Extensions;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class AttackHitboxesContainer : MonoBehaviour
    {
        [SerializeField] private AttackHitbox[] _attackHitboxes = null;

        public AttackHitbox[] AttackHitboxes => _attackHitboxes;

        public void SetDirection(float dir)
        {
            transform.SetScaleX(Mathf.Sign(dir));
        }

        public void GetHitboxesInChildren()
        {
            _attackHitboxes = GetComponentsInChildren<AttackHitbox>();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AttackHitboxesContainer))]
    public class AttackHitboxesContainerEditor : RSLib.EditorUtilities.ButtonProviderEditor<AttackHitboxesContainer>
    {
        protected override void DrawButtons()
        {
            DrawButton("Get Hitboxes in Children", Obj.GetHitboxesInChildren);
        }
    }
#endif
}