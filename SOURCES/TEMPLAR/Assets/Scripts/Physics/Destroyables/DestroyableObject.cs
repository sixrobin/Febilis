namespace Templar.Physics.Destroyables
{
    using RSLib.Extensions;
    using Templar.Interaction.Checkpoint;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class DestroyableObject : MonoBehaviour, ICheckpointListener
    {
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private Sprite _destroyedSprite = null;
        [SerializeField] private LayerMask _groundLayer = 0;

        protected Datas.DestroyableDatas _destroyableDatas;
        private Collider2D _collider2D;
        private Sprite _baseSprite;

        public void OnCheckpointInteracted(CheckpointController checkpointCtrl)
        {
            ResetDestroyable();
        }

        public bool TryDestroy(DestroyableSourceType sourceType)
        {
            if (!_destroyableDatas.IsSourceValid(sourceType))
                return false;

            Destroy(sourceType);
            return true;
        }

        protected void Destroy(DestroyableSourceType sourceType)
        {
            if (_destroyableDatas.TraumaDatas != null)
                FindObjectOfType<Templar.Camera.CameraController>().ApplyShakeFromDatas(_destroyableDatas.TraumaDatas); // [TMP] Find.

            for (int i = _destroyableDatas.ToSpawnFromPool.Count - 1; i >= 0; --i)
            {
                GameObject vfxInstance = RSLib.Framework.Pool.Get(_destroyableDatas.ToSpawnFromPool[i]);
                vfxInstance.transform.position = transform.position;
            }

            // Only show destroyed particles if grounded.
            RaycastHit2D hit = Physics2D.Raycast(transform.position.AddY(0.2f), Vector2.down, 0.3f, _groundLayer);
            _spriteRenderer.sprite = hit ? _destroyedSprite : null;

            _collider2D.enabled = false;

            // [TODO] If some destroyables should be destroyed with this one, do it here.
            // Maybe register them and destroy them in LateUpdate only if they haven't already been destroyed in the same blow ?
        }

        private void ResetDestroyable()
        {
            _spriteRenderer.sprite = _baseSprite;
            _collider2D.enabled = true;
        }

        private void Awake()
        {
            _destroyableDatas = Database.DestroyablesDatabase.DestroyablesDatas[_id];

            if (!TryGetComponent(out Collider2D collider2D))
                CProLogger.LogWarning(this, $"No Collider2D has been found on {transform.name}, it then won't be able to be destroyed.", gameObject);

            _collider2D = collider2D;
            _baseSprite = _spriteRenderer.sprite;
        }
    }
}