namespace RSLib.Jumble.DungeonGenerator
{
    using UnityEngine;

    public class DoorController : MonoBehaviour
    {
        [SerializeField] private DungeonGenerator.RoomType _openingDir = DungeonGenerator.RoomType.NA;
        [SerializeField] private DoorCrossTrigger _crossTrigger = null;
        [SerializeField] private Collider2D _collider = null;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private Sprite _openSprite = null;

        private Sprite _closeSprite = null;
        
        public static event System.Action<DoorController> DoorCrossTriggered = null;

        public DungeonGenerator.RoomType OpeningDir => _openingDir;

        public void Open()
        {
            _spriteRenderer.sprite = _openSprite;
            _collider.enabled = false;
            _crossTrigger.Enable();
        }

        public void Close()
        {
            _spriteRenderer.sprite = _closeSprite;
            _collider.enabled = true;
            _crossTrigger.Disable();
        }

        public void CrossDoor()
        {
            DoorCrossTriggered?.Invoke(this);
        }

        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsTrue(
                _openingDir == DungeonGenerator.RoomType.L
                || _openingDir == DungeonGenerator.RoomType.R
                || _openingDir == DungeonGenerator.RoomType.T
                || _openingDir == DungeonGenerator.RoomType.B,
                "Door can not have multiple openings!");

            _closeSprite = _spriteRenderer.sprite;
        }
    }
}