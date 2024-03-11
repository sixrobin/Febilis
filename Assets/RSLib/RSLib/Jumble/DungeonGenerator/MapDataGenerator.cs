namespace RSLib.Jumble.DungeonGenerator
{
    using System.Linq;

    public class MapDataGenerator
    {
        private const byte INIT_W = 2;
        private const byte INIT_H = 2;

        private System.Random _rnd = new System.Random();

        private RoomType[,] _roomsTypes;
        private GenerationDirection _dir;

        private int _x;
        private int _y;
        private (int x, int y) _start;
        private (int x, int y) _end;

        private enum GenerationDirection : byte
        {
            NA = 255,
            LEFT = 0,
            RIGHT = 1,
            DOWN = 2
        }

        public int Seed { get; private set; }

        private bool BorderingLeft => _x == 0;
        private bool BorderingTop => _y == 0;
        private bool BorderingRight => _x == _w - 1;
        private bool BorderingBottom => _y == _h - 1;

        private int _w;
        private int _h;

        public MapDataGenerator(int seed, int lvl)
        {
            Seed = seed;
            if (Seed == 0)
                Seed = new System.Random().Next(int.MaxValue);

            _rnd = new System.Random(Seed);

            _w = INIT_W + lvl;
            _h = INIT_H + lvl;
        }

        public MapDataGenerator(int seed, int w, int h)
        {
            Seed = seed;
            if (Seed == 0)
                Seed = new System.Random().Next(int.MaxValue);

            _rnd = new System.Random(Seed);

            _w = w > INIT_W ? w : INIT_W;
            _h = h > INIT_H ? h : INIT_H;
        }

        public MapDataGenerator(int seed, UnityEngine.Vector2Int size)
        {
            Seed = seed;
            if (Seed == 0)
                Seed = new System.Random().Next(int.MaxValue);

            _rnd = new System.Random(Seed);

            _w = size.x > INIT_W ? size.x : INIT_W;
            _h = size.y > INIT_H ? size.y : INIT_H;
        }

        public MapData ComputeMapData()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            _roomsTypes = new RoomType[_w, _h];

            GenerateMainPath();
            FillBlankRooms();
            CloseDeadEnds();
            CloseBorders();

            if (ComputeRoomsLinkedToMainPath().Count != _w * _h)
                UnityEngine.Debug.LogWarning("Not all rooms are connected.");

            Room[,] rooms = new Room[_w, _h];
            for (int x = _w - 1; x >= 0; --x)
                for (int y = _h - 1; y >= 0; --y)
                    rooms[x, y] = new Room(_roomsTypes[x, y]);

            sw.Stop();
            UnityEngine.Debug.Log($"Map generated in {sw.ElapsedMilliseconds} ms.");

            return new MapData(rooms, _start, _end);
        }

        private bool AreCoordinatesValid(int x, int y)
        {
            return x >= 0 && x < _w && y >= 0 && y < _h;
        }

        private void GenerateMainPath()
        {
            _x = _rnd.Next(_w);
            _roomsTypes[_x, _y] = RoomType.LR;
            _dir = (GenerationDirection)_rnd.Next(0, 3);
            _start = (_x, _y);

            int iterations = 0;

            while (!(_dir == GenerationDirection.DOWN && BorderingBottom) && iterations++ < 10000)
            {
                switch (_dir)
                {
                    case GenerationDirection.LEFT:
                    {
                        if (BorderingLeft)
                        {
                            _dir = GenerationDirection.DOWN;
                            goto case GenerationDirection.DOWN;
                        }

                        _x--;
                        _roomsTypes[_x, _y] = RoomTypeUtilities.GetRandomRoomType(RoomType.R);
                        _roomsTypes[_x + 1, _y] = _roomsTypes[_x + 1, _y].OpenSide(RoomType.L);
                        _dir = (GenerationDirection)(_rnd.Next(1, 3) == 2 ? 2 : 0);
                        break;
                    }

                    case GenerationDirection.RIGHT:
                    {
                        if (BorderingRight)
                        {
                            _dir = GenerationDirection.DOWN;
                            goto case GenerationDirection.DOWN;
                        }

                        _x++;
                        _roomsTypes[_x, _y] = RoomTypeUtilities.GetRandomRoomType(RoomType.L);
                        _roomsTypes[_x - 1, _y] = _roomsTypes[_x - 1, _y].OpenSide(RoomType.R);
                        _dir = (GenerationDirection)_rnd.Next(1, 3);
                        break;
                    }

                    case GenerationDirection.DOWN:
                    {
                        if (BorderingBottom)
                            continue;

                        _y++;
                        _roomsTypes[_x, _y] = RoomTypeUtilities.GetRandomRoomType(RoomType.T);
                        _roomsTypes[_x, _y - 1] = _roomsTypes[_x, _y - 1].OpenSide(RoomType.B);
                        _dir = (GenerationDirection)_rnd.Next(0, 3);
                        break;
                    }

                    case GenerationDirection.NA:
                    default:
                        UnityEngine.Debug.LogError($"Unhandled direction {_dir}!");
                        break;
                }
            }

            UnityEngine.Assertions.Assert.IsTrue(iterations < 10000, "Map generation got stuck in an infinite loop.");

            _end = (_x, _y);
        }

        private void FillBlankRooms()
        {
            System.Collections.Generic.List<(int x, int y)> fillRooms = new System.Collections.Generic.List<(int, int)>();

            for (int x = _w - 1; x >= 0; --x)
            {
                for (int y = _h - 1; y >= 0; --y)
                {
                    if (_roomsTypes[x, y] == RoomType.NA)
                    {
                        _roomsTypes[x, y] = RoomTypeUtilities.GetRandomFillingRoomType();
                        fillRooms.Add((x, y));
                    }
                }
            }

            for (int i = fillRooms.Count - 1; i >= 0; --i)
            {
                foreach (RoomType r in RoomTypeUtilities.s_singleOpenings)
                {
                    if (!_roomsTypes[fillRooms[i].x, fillRooms[i].y].HasOpening(r))
                        continue;

                    int openedToX = r == RoomType.L ? (fillRooms[i].x - 1) : r == RoomType.R ? (fillRooms[i].x + 1) : fillRooms[i].x;
                    int openedToY = r == RoomType.T ? (fillRooms[i].y - 1) : r == RoomType.B ? (fillRooms[i].y + 1) : fillRooms[i].y;

                    if (!AreCoordinatesValid(openedToX, openedToY) || _roomsTypes[openedToX, openedToY] == RoomType.NA)
                        continue;

                    RoomType opp = r.GetOppositeDoor();
                    if (!_roomsTypes[openedToX, openedToY].HasOpening(opp))
                        _roomsTypes[openedToX, openedToY] = _roomsTypes[openedToX, openedToY].OpenSide(opp);
                }
            }
        }

        private void CloseDeadEnds()
        {
            for (int x = _w - 1; x >= 0; --x)
            {
                for (int y = _h - 1; y >= 0; --y)
                {
                    foreach (RoomType r in RoomTypeUtilities.s_singleOpenings)
                    {
                        if (_roomsTypes[x, y].HasOpening(r))
                            continue;

                        int closedToX = r == RoomType.L ? (x - 1) : r == RoomType.R ? (x + 1) : x;
                        int closedToY = r == RoomType.T ? (y - 1) : r == RoomType.B ? (y + 1) : y;

                        if (!AreCoordinatesValid(closedToX, closedToY))
                            continue;

                        RoomType opp = r.GetOppositeDoor();
                        if (_roomsTypes[closedToX, closedToY].HasOpening(opp))
                            _roomsTypes[closedToX, closedToY] = _roomsTypes[closedToX, closedToY].CloseSide(opp);
                    }
                }
            }
        }

        private void CloseBorders()
        {
            for (int x = _w - 1; x >= 0; --x)
            {
                _roomsTypes[x, 0] = _roomsTypes[x, 0].CloseSide(RoomType.T);
                _roomsTypes[x, _h - 1] = _roomsTypes[x, _h - 1].CloseSide(RoomType.B);
            }

            for (int y = _h - 1; y >= 0; --y)
            {
                _roomsTypes[0, y] = _roomsTypes[0, y].CloseSide(RoomType.L);
                _roomsTypes[_w - 1, y] = _roomsTypes[_w - 1, y].CloseSide(RoomType.R);
            }
        }

        private System.Collections.Generic.List<(int, int)> ComputeRoomsLinkedToMainPath()
        {
            System.Collections.Generic.List<(int, int)> linkedRooms = new System.Collections.Generic.List<(int, int)>();
            System.Collections.Generic.Queue<(int, int)> roomsToCheck = new System.Collections.Generic.Queue<(int, int)>();
            linkedRooms.Add(_start);
            roomsToCheck.Enqueue(_start);

            while (roomsToCheck.Count > 0)
            {
                (int x, int y) = roomsToCheck.Dequeue();

                if (AreCoordinatesValid(x + 1, y) && _roomsTypes[x + 1, y].HasOpening(RoomType.L)
                    && linkedRooms.Count(o => o.Item1 == x + 1 && o.Item2 == y) == 0)
                {
                    roomsToCheck.Enqueue((x + 1, y));
                    linkedRooms.Add((x + 1, y));
                }

                if (AreCoordinatesValid(x - 1, y) && _roomsTypes[x - 1, y].HasOpening(RoomType.R)
                    && linkedRooms.Count(o => o.Item1 == x - 1 && o.Item2 == y) == 0)
                {
                    roomsToCheck.Enqueue((x - 1, y));
                    linkedRooms.Add((x - 1, y));
                }

                if (AreCoordinatesValid(x, y + 1) && _roomsTypes[x, y + 1].HasOpening(RoomType.T)
                    && linkedRooms.Count(o => o.Item1 == x && o.Item2 == y + 1) == 0)
                {
                    roomsToCheck.Enqueue((x, y + 1));
                    linkedRooms.Add((x, y + 1));
                }

                if (AreCoordinatesValid(x, y - 1) && _roomsTypes[x, y - 1].HasOpening(RoomType.B)
                    && linkedRooms.Count(o => o.Item1 == x && o.Item2 == y - 1) == 0)
                {
                    roomsToCheck.Enqueue((x, y - 1));
                    linkedRooms.Add((x, y - 1));
                }
            }

            return linkedRooms;
        }
    }
}