namespace RSLib.Jumble.DungeonGenerator
{
    using RSLib.Extensions;
    using UnityEngine;

    public class RoomsFactory : MonoBehaviour
    {
        private System.Collections.Generic.Dictionary<RoomType, RoomController[]> _roomsPrefabsByType;

        [SerializeField] private RoomController[] _L = null;
        [SerializeField] private RoomController[] _R = null;
        [SerializeField] private RoomController[] _T = null;
        [SerializeField] private RoomController[] _B = null;
        [SerializeField] private RoomController[] _LR = null;
        [SerializeField] private RoomController[] _TB = null;
        [SerializeField] private RoomController[] _RT = null;
        [SerializeField] private RoomController[] _RB = null;
        [SerializeField] private RoomController[] _LT = null;
        [SerializeField] private RoomController[] _LB = null;
        [SerializeField] private RoomController[] _LRT = null;
        [SerializeField] private RoomController[] _LRB = null;
        [SerializeField] private RoomController[] _LTB = null;
        [SerializeField] private RoomController[] _RTB = null;
        [SerializeField] private RoomController[] _LRTB = null;

        public RoomController GetRandomRoomByType(RoomType type)
        {
            return _roomsPrefabsByType.TryGetValue(type, out RoomController[] rooms) ? rooms.RandomElement() : null;
        }

        private void Awake()
        {
            _roomsPrefabsByType = new System.Collections.Generic.Dictionary<RoomType, RoomController[]>()
            {
                { RoomType.L, _L },
                { RoomType.R, _R },
                { RoomType.T, _T },
                { RoomType.B, _B },
                { RoomType.LR, _LR },
                { RoomType.TB, _TB },
                { RoomType.RB, _RB },
                { RoomType.RT, _RT },
                { RoomType.LB, _LB },
                { RoomType.LT, _LT },
                { RoomType.LRT, _LRT },
                { RoomType.LRB, _LRB },
                { RoomType.LTB, _LTB },
                { RoomType.RTB, _RTB },
                { RoomType.LRTB, _LRTB }
            };
        }
    }
}