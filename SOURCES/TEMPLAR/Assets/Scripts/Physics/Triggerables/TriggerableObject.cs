namespace Templar.Physics.Triggerables
{
    using RSLib.Extensions;
    using System.Linq;
    using Templar.Interaction.Checkpoint;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class TriggerableObject : MonoBehaviour, ICheckpointListener
    {
        public class TriggerEventArgs : System.EventArgs
        {
            public TriggerEventArgs(TriggerableObject source)
            {
                Source = source;
            }

            public TriggerableObject Source { get; private set; }
        }

        [SerializeField] private string _id = string.Empty;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private RSLib.Framework.OptionalAnimator _animator = new RSLib.Framework.OptionalAnimator();
        [SerializeField] private Sprite[] _triggerSpritesLoop = null;
        [SerializeField] private LayerMask _groundLayer = 0;

        [Header("LINKED INTERACTION")]
        [SerializeField] private Interaction.Interactable _linkedInteractable = null;

        [Header("CHAIN DESTRUCTION")]
        [Tooltip("Triggerables that are immediatly triggered with this one.")]
        [SerializeField] private TriggerableObject[] _children = null;

        protected Datas.TriggerableDatas _triggerableDatas;
        private Collider2D _collider2D;
        private Sprite _baseSprite;

        private int _currentSpriteIndex = -1;
        private int _triggersCounter;

        public delegate void TriggerableEventHandler(TriggerEventArgs args);

        public event TriggerableEventHandler Triggered;
        public event TriggerableEventHandler Reset;
        
        /// <summary>Shared dictionary allowing collision detections across the game to check if collider has a related destroyable.</summary>
        public static System.Collections.Generic.Dictionary<Collider2D, TriggerableObject> SharedTriggerablesByColliders { get; private set; }
            = new System.Collections.Generic.Dictionary<Collider2D, TriggerableObject>();

        public bool NotTriggerableAnymore => _triggerableDatas.MaxTriggersCount > -1 && _triggersCounter >= _triggerableDatas.MaxTriggersCount;

        void ICheckpointListener.OnCheckpointInteracted(CheckpointController checkpointCtrl)
        {
            ResetTriggerable();
        }

        public bool TryTrigger(TriggerableSourceType sourceType, bool force = false)
        {
            if (!force && !_triggerableDatas.IsSourceValid(sourceType))
                return false;

            Trigger(sourceType); // We may want to specify sourceType + direction someday, if we want more specific feedback.
            return true;
        }

        public void ResetTriggerable()
        {
            _triggersCounter = 0;
            _currentSpriteIndex = -1;
            _spriteRenderer.sprite = _baseSprite;
            _collider2D.enabled = true;

            Reset?.Invoke(new TriggerEventArgs(this));
        }

        private void OnLinkedInteractableInteracted(Interaction.Interactable.InteractionEventArgs args)
        {
            Trigger(TriggerableSourceType.NONE);
        }

        private void ApplyOnTriggerDatasCallbacks()
        {
            if (_triggerableDatas.LootDatas != null)
                Manager.LootManager.SpawnLoot(_triggerableDatas.LootDatas, transform.position.AddY(0.2f));

            if (_triggerableDatas.TraumaDatas != null)
                Manager.GameManager.CameraCtrl.ApplyShakeFromDatas(_triggerableDatas.TraumaDatas);

            if (!string.IsNullOrEmpty(_triggerableDatas.AnimatorTrigger))
            {
                UnityEngine.Assertions.Assert.IsTrue(
                    _animator.Enabled && _animator.Value != null,
                    $"{_id} triggerable trying to set animator trigger {_triggerableDatas.AnimatorTrigger} but animator is disabled or missing.");

                _animator.Value.SetTrigger(_triggerableDatas.AnimatorTrigger);
            }

            for (int i = _triggerableDatas.ToSpawnFromPool.Count - 1; i >= 0; --i)
            {
                GameObject vfxInstance = RSLib.Framework.Pooling.Pool.Get(_triggerableDatas.ToSpawnFromPool[i]);
                vfxInstance.transform.position = transform.position;
            }
        }

        private void Trigger(TriggerableSourceType sourceType)
        {
            if (NotTriggerableAnymore)
                return;

            ApplyOnTriggerDatasCallbacks();

            _triggersCounter++;
            _collider2D.enabled = !NotTriggerableAnymore;

            Triggered?.Invoke(new TriggerEventArgs(this));

            if (_triggerSpritesLoop != null && _triggerSpritesLoop.Length > 0)
            {
                RaycastHit2D groundHit = Physics2D.Raycast(transform.position.AddY(0.2f), Vector2.down, 0.3f, _groundLayer);
                _spriteRenderer.sprite = groundHit || !NotTriggerableAnymore
                    ? _triggerSpritesLoop[++_currentSpriteIndex % _triggerSpritesLoop.Length]
                    : null;
            }
            
            for (int i = _children.Length - 1; i >= 0; --i)
                _children[i].Trigger(sourceType);
        }

        private void Awake()
        {
            if (_linkedInteractable != null)
                _linkedInteractable.Interacted += OnLinkedInteractableInteracted;

            _triggerableDatas = Database.TriggerablesDatabase.TriggerablesDatas[_id];

            if (!TryGetComponent(out Collider2D collider2D))
                CProLogger.LogWarning(this, $"No Collider2D has been found on {transform.name}, it then won't be able to be triggered.", gameObject);

            _collider2D = collider2D;
            _baseSprite = _spriteRenderer.sprite;

            SharedTriggerablesByColliders.Add(_collider2D, this);

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command(
                "ResetTriggerables",
                "Reset all triggerable objects.",
                () => FindObjectsOfType<TriggerableObject>().ToList().ForEach(o => o.ResetTriggerable())));
        }

        private void OnDestroy()
        {
            if (_linkedInteractable != null)
                _linkedInteractable.Interacted -= OnLinkedInteractableInteracted;

            SharedTriggerablesByColliders.Remove(_collider2D);
        }

        public void GetChildrenTriggerables()
        {
            _children = GetComponentsInChildren<TriggerableObject>().Where(o => o.transform.parent == transform).ToArray();
#if UNITY_EDITOR
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
#endif
        }

        public void GetChildrenTriggerablesRecursive()
        {
            _children = GetComponentsInChildren<TriggerableObject>().Where(o => o.transform.parent == transform).ToArray();
            for (int i = _children.Length - 1; i >= 0; --i)
                _children[i].GetChildrenTriggerables();

#if UNITY_EDITOR
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TriggerableObject))]
    public class TriggerableObjectEditor : RSLib.EditorUtilities.ButtonProviderEditor<TriggerableObject>
    {
        protected override void DrawButtons()
        {
            DrawButton("Get Children Triggerables", Obj.GetChildrenTriggerables);
            DrawButton("Get Children Triggerables Recursive", Obj.GetChildrenTriggerablesRecursive);

            if (UnityEditor.EditorApplication.isPlaying)
                DrawButton("Reset Triggerable", Obj.ResetTriggerable);
        }
    }
#endif
}