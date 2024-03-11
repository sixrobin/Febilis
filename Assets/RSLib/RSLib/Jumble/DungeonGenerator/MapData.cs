namespace RSLib.Jumble.DungeonGenerator
{
    public class MapData
    {
        public MapData(Room[,] rooms, (int, int) start, (int, int) end)
        {
            Rooms = rooms;
            Size = (rooms.GetLength(0), rooms.GetLength(1));
            Start = start;
            End = end;
        }

        public Room this[(int x, int y) index] => Rooms[index.x, index.y];

        public Room[,] Rooms { get; private set; }

        public (int W, int H) Size { get; }
        public (int X, int Y) Start { get; }
        public (int X, int Y) End { get; }
    }
}