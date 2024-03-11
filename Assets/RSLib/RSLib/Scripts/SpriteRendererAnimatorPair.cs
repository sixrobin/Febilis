namespace RSLib
{
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    /// <summary>
    /// Used to reference both a SpriteRenderer and an Animator, as both are often together.
    /// Can also avoid doing two loops on both components, but instead do only one on this component.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SpriteRendererAnimatorPair : MonoBehaviour
    {
        #if ODIN_INSPECTOR
        [FoldoutGroup("Refs")]
        #endif
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Refs")]
        #endif
        [SerializeField] private Animator _animator = null;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Animator Animator => _animator;

        private void GetMissingComponents()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_animator == null)
                _animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            GetMissingComponents();

            if (SpriteRenderer == null)
                UnityEngine.Debug.LogWarning($"SpriteRenderer is missing on {GetType().Name} attached to {transform.name}.", gameObject);

            if (Animator == null)
                UnityEngine.Debug.LogWarning($"Animator is missing on {GetType().Name} attached to {transform.name}.", gameObject);
        }

        private void Reset()
        {
            GetMissingComponents();
        }
    }
}