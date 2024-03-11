namespace LDJAM50
{
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
    
    /// <summary>
    /// Used to randomize some SpriteRenderer values like sprite, flip and color.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererRandomizer : MonoBehaviour
    {
        #if !ODIN_INSPECTOR
        [Header("REFS")]
        #endif
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Refs")]
        #endif
        [SerializeField] private SpriteRenderer _spriteRenderer = null;

        #if !ODIN_INSPECTOR
        [Header("SPRITE (Leave sprites array empty not to randomize sprite)")]
        #endif
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Refs")]
        [InfoBox("Leave sprites array empty not to randomize sprite.")]
        #endif
        [SerializeField] private Sprite[] _sprites = null;
        
        #if !ODIN_INSPECTOR
        [Header("FLIP")]
        #endif
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Flip")]
        #endif
        [SerializeField, Range(0f, 1f)] private float _flipXChance = 0.5f;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Flip")]
        #endif
        [SerializeField, Range(0f, 1f)] private float _flipYChance = 0.5f;

        #if !ODIN_INSPECTOR
        [Header("COLOR")]
        #endif
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Color")]
        #endif
        [SerializeField] private Color _colorA = Color.white;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Color")]
        #endif
        [SerializeField] private Color _colorB = Color.white;

        [ContextMenu("Randomize")]
        #if ODIN_INSPECTOR
        [Button("Randomize")]
        #endif
        public void Randomize()
        {
            _spriteRenderer.flipX = Random.value < _flipXChance;
            _spriteRenderer.flipY = Random.value < _flipYChance;
            _spriteRenderer.color = Color.Lerp(_colorA, _colorB, Random.value);

            if (_sprites.Length > 0)
            {
                _spriteRenderer.sprite = _sprites[Random.Range(0, _sprites.Length)];
            }            
        }
        
        private void Awake()
        {
            if (_spriteRenderer != null || TryGetComponent(out _spriteRenderer))
                Randomize();
        }

        private void Reset()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}