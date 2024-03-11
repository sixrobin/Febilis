namespace RSLib.Jumble.DungeonGenerator
{
    using UnityEngine;

    public class RoomController : MonoBehaviour
    {
        [SerializeField] private DoorController[] _doorControllers = null;
        [SerializeField] private Transform[] _playerLRTBSpawns = null;
        [SerializeField] private GameObject _firstRoomMark = null;
        [SerializeField] private GameObject _exitTrigger = null;
        [SerializeField] private Transform _firstRoomSpawnPos = null;
        [SerializeField] private bool _playerLookRightOnStart = true;

        private DungeonGenerator.Room _room;
        private bool _discovered;

        private System.Collections.Generic.Dictionary<DungeonGenerator.RoomType, DoorController> _doorsByOpeningType
            = new System.Collections.Generic.Dictionary<DungeonGenerator.RoomType, DoorController>();

        private System.Collections.Generic.Dictionary<DungeonGenerator.RoomType, Transform> _spawnsByOpening;

        public event System.Action RoomDiscovered;
        public event System.Action RoomEntered;
        public event System.Action RoomLeft;

        public Vector3 FirstRoomSpawnPos => _firstRoomSpawnPos.position;

        public bool PlayerLookRightOnStart => _playerLookRightOnStart;

        public void Init(DungeonGenerator.Room room, bool isFirstRoom, bool isLastRoom)
        {
            _room = room;
            _firstRoomMark.SetActive(isFirstRoom);
            _exitTrigger.SetActive(isLastRoom);

            if (isFirstRoom)
                OpenDoors();

            transform.name = $"Room {_room.RoomType} {(isFirstRoom ? "- Start" : isLastRoom ? "- End" : "")}";
            Display(isFirstRoom);
        }

        public void Display(bool state)
        {
            gameObject.SetActive(state);

            if (state)
            {
                RoomEntered?.Invoke();

                if (!_discovered)
                {
                    _discovered = true;
                    RoomDiscovered?.Invoke();
                }
            }
            else
            {
                RoomLeft?.Invoke();
            }
        }

        public DoorController GetDoor(DungeonGenerator.RoomType opening)
        {
            UnityEngine.Assertions.Assert.IsTrue(_doorsByOpeningType.ContainsKey(opening), "Trying to get a door that does not exist.");
            return _doorsByOpeningType[opening];
        }

        public Transform GetSpawnByOpening(DungeonGenerator.RoomType opening)
        {
            UnityEngine.Assertions.Assert.IsTrue(_spawnsByOpening.ContainsKey(opening), "Trying to get a door spawn point that does not exist.");
            return _spawnsByOpening[opening];
        }

        public void OpenDoors()
        {
            for (int i = _doorControllers.Length - 1; i >= 0; --i)
                _doorControllers[i].Open();
        }

        public void CloseDoors()
        {
            for (int i = _doorControllers.Length - 1; i >= 0; --i)
                _doorControllers[i].Close();
        }

        private void Awake()
        {
            for (int i = _doorControllers.Length - 1; i >= 0; --i)
                _doorsByOpeningType.Add(_doorControllers[i].OpeningDir, _doorControllers[i]);

            _spawnsByOpening = new System.Collections.Generic.Dictionary<DungeonGenerator.RoomType, Transform>()
            {
                { DungeonGenerator.RoomType.L, _playerLRTBSpawns[0] },
                { DungeonGenerator.RoomType.R, _playerLRTBSpawns[1] },
                { DungeonGenerator.RoomType.T, _playerLRTBSpawns[2] },
                { DungeonGenerator.RoomType.B, _playerLRTBSpawns[3] }
            };
        }

#if UNITY_EDITOR
        [ContextMenu("Get Doors in Children")]
        private void DebugGetDoorsInChildren()
        {
            _doorControllers = GetComponentsInChildren<DoorController>();
        }
#endif
    }
}