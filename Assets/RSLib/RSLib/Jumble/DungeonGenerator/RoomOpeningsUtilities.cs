namespace RSLib.Jumble.DungeonGenerator
{
    using System.Linq;

    public static class RoomTypeUtilities
    {
        private static System.Random s_rnd = new System.Random();

        private static RoomType[] s_allTypes;
        private static RoomType[] AllTypes
        {
            get
            {
                if (s_allTypes == null)
                    s_allTypes = System.Enum.GetValues(typeof(RoomType)) as RoomType[];

                return s_allTypes;
            }
        }

        public static readonly RoomType[] s_singleOpenings = new RoomType[]
        {
            RoomType.L,
            RoomType.R,
            RoomType.T,
            RoomType.B
        };

        public static readonly System.Collections.Generic.Dictionary<RoomType, RoomType> s_oppositeDoors
            = new System.Collections.Generic.Dictionary<RoomType, RoomType>()
        {
            { RoomType.L, RoomType.R },
            { RoomType.R, RoomType.L },
            { RoomType.T, RoomType.B },
            { RoomType.B, RoomType.T }
        };

        public static readonly System.Collections.Generic.Dictionary<RoomType, UnityEngine.Vector3> s_vectorsByOpening
            = new System.Collections.Generic.Dictionary<RoomType, UnityEngine.Vector3>()
        {
            { RoomType.L, UnityEngine.Vector3.left },
            { RoomType.R, UnityEngine.Vector3.right },
            { RoomType.T, UnityEngine.Vector3.up },
            { RoomType.B, UnityEngine.Vector3.down }
        };

        /// <summary>
        /// Rooms that can be used to fill blank rooms after main path generation.
        /// This array can be tweaked to have different more random map generation results.
        /// </summary>
        public static readonly RoomType[] s_fillRoomsDoors = new RoomType[]
        {
            RoomType.LR,
            RoomType.TB
        };

        public static RoomType GetRandomFillingRoomType()
        {
            return s_fillRoomsDoors[s_rnd.Next(s_fillRoomsDoors.Length)];
        }

        public static RoomType GetRandomRoomType(RoomType requiredOpening)
        {
            System.Collections.Generic.IEnumerable<RoomType> potentialRooms = AllTypes.Where(o => (o & requiredOpening) == requiredOpening);
            return potentialRooms.ElementAt(s_rnd.Next(potentialRooms.Count()));
        }

        public static bool HasOpening(this RoomType r, RoomType opening)
        {
            return (r & opening) == opening;
        }

        public static RoomType OpenSide(this RoomType r, RoomType sideToOpen)
        {
            return r | sideToOpen;
        }

        public static RoomType CloseSide(this RoomType r, RoomType sideToClose)
        {
            return (r | sideToClose) ^ sideToClose;
        }

        public static RoomType GetOppositeDoor(this RoomType r)
        {
            UnityEngine.Assertions.Assert.IsTrue(s_oppositeDoors.ContainsKey(r), "Trying to get opposite opening of a multiple openings room type.");
            return s_oppositeDoors[r];
        }
    }
}