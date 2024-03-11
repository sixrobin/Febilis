namespace RSLib.Jumble.DungeonGenerator
{
    using UnityEngine;

    /// <summary>
    /// Object that actually instantiates the dungeon in the game.
    /// Should probably have methods to get the right world coordinates, and some event to notify the generation has been done.
    /// </summary>
    public class RoomsGenerator : MonoBehaviour
    {
        public event System.Action<MapData> MapGenerated;

        private MapData _data;

        [SerializeField] private int _w = 2;
        [SerializeField] private int _h = 7;
        [SerializeField] private int _seed = 0;
        [SerializeField] private RoomsFactory _roomsFactory = null;
        [SerializeField] private GameObject _firstRoomMark = null;
        [SerializeField] private GameObject _lastRoomMark = null;

        private Vector3 RoomWorldCoordinates(int x, int y)
        {
            return new Vector3(x * 17, -y * 9);
        }

        private void GenerateMap()
        {
            MapDataGenerator generator = new MapDataGenerator(_seed, _w, _h);
            _data = generator.ComputeMapData();
            _seed = generator.Seed;
            SpawnRooms();
            MapGenerated?.Invoke(_data);
        }

        private void SpawnRooms()
        {
            for (int x = 0; x < _data.Size.W; ++x)
            {
                for (int y = 0; y < _data.Size.H; ++y)
                {
                    if (_data.Rooms[x, y].RoomType == RoomType.NA)
                    {
                        Debug.LogError("MapGeneratorGO ERROR: Trying to instantiate a room of type RoomType.NA!");
                        continue;
                    }

                    Instantiate(_roomsFactory.GetRandomRoomByType(_data.Rooms[x, y].RoomType), RoomWorldCoordinates(x, y), Quaternion.identity);

                    if (x == _data.Start.X && y == _data.Start.Y)
                        Instantiate(_firstRoomMark, RoomWorldCoordinates(x, y), _firstRoomMark.transform.rotation);
                    else if (x == _data.End.X && y == _data.End.Y)
                        Instantiate(_lastRoomMark, RoomWorldCoordinates(x, y), _lastRoomMark.transform.rotation);
                }
            }
        }

        private void Start()
        {
            GenerateMap();
        }
    }
}