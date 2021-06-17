namespace Templar.Physics.Destroyables
{
    using RSLib.Extensions;
    using System.Linq;
    using Templar.Interaction.Checkpoint;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class DestroyableObject : MonoBehaviour, ICheckpointListener
    {
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private Sprite _destroyedSprite = null;
        [SerializeField] private LayerMask _groundLayer = 0;

        [Header("CHAIN DESTRUCTION")]
        [Tooltip("Destroyables that are immediatly destroyed with this one.")]
        [SerializeField] private DestroyableObject[] _children = null;

        protected Datas.DestroyableDatas _destroyableDatas;
        private Collider2D _collider2D;
        private Sprite _baseSprite;

        private bool _destroyed;

        /// <summary>Shared dictionary allowing collision detections across the game to check if collider has a related destroyable.</summary>
        public static System.Collections.Generic.Dictionary<Collider2D, DestroyableObject> SharedDestroyableObjectsByColliders { get; private set; }
            = new System.Collections.Generic.Dictionary<Collider2D, DestroyableObject>();

        void ICheckpointListener.OnCheckpointInteracted(CheckpointController checkpointCtrl)
        {
            ResetDestroyable();
        }

        public bool TryDestroy(DestroyableSourceType sourceType)
        {
            if (!_destroyableDatas.IsSourceValid(sourceType))
                return false;

            Destroy(sourceType); // We may want to specify sourceType + direction someday, if we want more specific feedback.
            return true;
        }

        private void Destroy(DestroyableSourceType sourceType)
        {
            if (_destroyed)
                return;

            if (_destroyableDatas.LootDatas != null)
                Manager.LootManager.SpawnLoot(_destroyableDatas.LootDatas, transform.position.AddY(0.2f));

            if (_destroyableDatas.TraumaDatas != null)
                FindObjectOfType<Templar.Camera.CameraController>().ApplyShakeFromDatas(_destroyableDatas.TraumaDatas); // [TMP] Find.

            for (int i = _destroyableDatas.ToSpawnFromPool.Count - 1; i >= 0; --i)
            {
                GameObject vfxInstance = RSLib.Framework.Pooling.Pool.Get(_destroyableDatas.ToSpawnFromPool[i]);
                vfxInstance.transform.position = transform.position;
            }

            // Only show destroyed particles if grounded.
            RaycastHit2D hit = Physics2D.Raycast(transform.position.AddY(0.2f), Vector2.down, 0.3f, _groundLayer);
            _spriteRenderer.sprite = hit ? _destroyedSprite : null;

            _collider2D.enabled = false;
            _destroyed = true;

            for (int i = _children.Length - 1; i >= 0; --i)
                _children[i].Destroy(sourceType);
        }

        private void ResetDestroyable()
        {
            _destroyed = false;
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

            SharedDestroyableObjectsByColliders.Add(_collider2D, this);

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command(
                "ResetDestroyables",
                "Reset all destroyable objects.",
                () => FindObjectsOfType<DestroyableObject>().ToList().ForEach(o => o.ResetDestroyable())));
        }

        private void OnDestroy()
        {
            SharedDestroyableObjectsByColliders.Remove(_collider2D);
        }

        public void GetChildrenDestroyables()
        {
            _children = GetComponentsInChildren<DestroyableObject>().Where(o => o.transform.parent == transform).ToArray();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
        }

        public void GetChildrenDestroyablesRecursive()
        {
            _children = GetComponentsInChildren<DestroyableObject>().Where(o => o.transform.parent == transform).ToArray();
            for (int i = _children.Length - 1; i >= 0; --i)
                _children[i].GetChildrenDestroyables();

            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DestroyableObject))]
    public class DestroyableObjectEditor : RSLib.EditorUtilities.ButtonProviderEditor<DestroyableObject>
    {
        protected override void DrawButtons()
        {
            DrawButton("Get Children Destroyables", Obj.GetChildrenDestroyables);
            DrawButton("Get Children Destroyables Recursive", Obj.GetChildrenDestroyablesRecursive);
        }
    }
#endif
}