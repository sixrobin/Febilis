namespace Templar
{
    using UnityEngine;

    public class BreakingBridge : MonoBehaviour
    {
        [SerializeField] private Physics.Physics2DEventReceiver _breakTrigger = null;
        [SerializeField] private Collider2D _collider2D = null;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private Sprite _brokenSprite = null;

        [Header("FEEDBACK")]
        [SerializeField] private Vector2 _trauma = Vector2.one;

        private void OnBreakTriggerEnter(Collider2D collider)
        {
            _breakTrigger.gameObject.SetActive(false);
            _collider2D.enabled = false;
            _spriteRenderer.sprite = _brokenSprite;

            Manager.GameManager.CameraCtrl.GetShake(Camera.CameraShake.ID_BIG).AddTrauma(_trauma);
        }

        private void Awake()
        {
            _breakTrigger.OnTriggerEnter += OnBreakTriggerEnter;
        }

        private void OnDestroy()
        {
            _breakTrigger.OnTriggerEnter -= OnBreakTriggerEnter;
        }
    }
}