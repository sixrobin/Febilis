namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class BreakingBridge : MonoBehaviour, IIdentifiable
    {
        [Header("IDENTIFIER")]
        [SerializeField] private Identifier _identifier = null;

        [Header("REFS")]
        [SerializeField] private RSLib.Physics2DEventReceiver _breakTrigger = null;
        [SerializeField] private Collider2D _collider2D = null;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private Sprite _brokenSprite = null;

        [Header("FEEDBACK")]
        [SerializeField] private Vector2 _trauma = Vector2.one;
        [SerializeField] private Rigidbody2D[] _brokenParts = null;
        [SerializeField] private Vector2 _brokenPartsRandomForce = Vector2.zero;
        [SerializeField] private Vector2 _brokenPartsRandomTorque = Vector2.zero;
        [SerializeField] private BoxCollider2D _particlesSpawnArea = null;
        [SerializeField] private GameObject _particlesPrefab = null;
        [SerializeField] private int _particlesCount = 5;

        private Sprite _baseSprite;
        private Vector3[] _initBrokenPartsPositions;

        public IIdentifier Identifier => _identifier;

        private void OnBreakTriggerEnter(Collider2D collider)
        {
            Manager.FlagsManager.AddGeneric(Identifier);

            ToggleBrokenBridge();
            PlayBreakFeedback();
        }

        private void ToggleBrokenBridge()
        {
            _breakTrigger.gameObject.SetActive(false);
            _collider2D.enabled = false;
            _spriteRenderer.sprite = _brokenSprite;
        }

        private void PlayBreakFeedback()
        {
            Manager.GameManager.CameraCtrl.GetShake(Camera.CameraShake.ID_BIG).AddTrauma(_trauma);

            for (int i = _brokenParts.Length - 1; i >= 0; --i)
            {
                _brokenParts[i].gameObject.SetActive(true);
                _brokenParts[i].AddForce(Random.insideUnitCircle * _brokenPartsRandomForce.Random(), ForceMode2D.Impulse);
                _brokenParts[i].AddTorque(_brokenPartsRandomTorque.Random());
            }

            for (int i = 0; i < _particlesCount; ++i)
            {
                Transform particle = RSLib.Framework.Pooling.Pool.Get(_particlesPrefab).transform;
                particle.position = _particlesSpawnArea.RandomPointInside();
                particle.localEulerAngles = new Vector3(0f, 0f, Random.Range(0, 4) * 90f);
            }
        }

        private void Start()
        {
            _breakTrigger.TriggerEntered += OnBreakTriggerEnter;

            _baseSprite = _spriteRenderer.sprite;

            _initBrokenPartsPositions = new Vector3[_brokenParts.Length];
            for (int i = _brokenParts.Length - 1; i >= 0; --i)
            {
                _initBrokenPartsPositions[i] = _brokenParts[i].transform.position;
            }

            if (Manager.FlagsManager.CheckGeneric(Identifier))
                ToggleBrokenBridge();
        }

        private void OnDestroy()
        {
            _breakTrigger.TriggerEntered -= OnBreakTriggerEnter;
        }

        public void DebugDestroyBridge()
        {
            OnBreakTriggerEnter(null);
        }

        public void DebugResetBridge()
        {
            _breakTrigger.gameObject.SetActive(true);
            _collider2D.enabled = true;
            _spriteRenderer.sprite = _baseSprite;

            for (int i = _brokenParts.Length - 1; i >= 0; --i)
            {
                _brokenParts[i].NullifyMovement();
                _brokenParts[i].gameObject.SetActive(false);
                _brokenParts[i].transform.position = _initBrokenPartsPositions[i];
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BreakingBridge))]
    public class BreakingBridgeEditor : RSLib.EditorUtilities.ButtonProviderEditor<BreakingBridge>
    {
        protected override void DrawButtons()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                DrawButton("Destroy Bridge", Obj.DebugDestroyBridge);
                DrawButton("Restore Bridge", Obj.DebugResetBridge);
            }
        }
    }
#endif
}