namespace RSLib.Jumble.DungeonGenerator
{
    /// <summary>
    /// Class containing basic information about a room.
    /// Can be inherited to add specific data.
    /// </summary>
    public class Room
    {
        public Room(RoomType roomType)
        {
            RoomType = roomType;
        }

        public RoomType RoomType { get; }
    }
}