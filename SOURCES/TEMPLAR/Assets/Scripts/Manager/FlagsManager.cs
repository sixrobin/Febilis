namespace Templar.Manager
{
    using System.Xml.Linq;

    public partial class FlagsManager : RSLib.Framework.ConsoleProSingleton<FlagsManager>
    {
        private System.Collections.Generic.List<string> _generics = new System.Collections.Generic.List<string>();
        private System.Collections.Generic.List<string> _pickedUpItems = new System.Collections.Generic.List<string>();
        private System.Collections.Generic.List<string> _openChests = new System.Collections.Generic.List<string>();
        private System.Collections.Generic.List<string> _discoveredBoards = new System.Collections.Generic.List<string>();


        public static bool CheckGeneric(IIdentifier identifier)
        {
            return CheckGeneric(identifier.Id);
        }
        public static bool CheckGeneric(string id)
        {
            return Instance._generics.Contains(id);
        }

        public static void AddGeneric(IIdentifier identifier)
        {
            AddGeneric(identifier.Id);
        }
        public static void AddGeneric(string id)
        {
            UnityEngine.Assertions.Assert.IsFalse(CheckGeneric(id), $"Adding already generic Id {id} to the generics list.");
            Instance._generics.Add(id);
        }


        public static bool CheckPickedUpItem(IIdentifier identifier)
        {
            return CheckPickedUpItem(identifier.Id);
        }
        public static bool CheckPickedUpItem(string id)
        {
            return Instance._pickedUpItems.Contains(id);
        }

        public static void AddPickedUpItem(IIdentifier identifier)
        {
            AddPickedUpItem(identifier.Id);
        }
        public static void AddPickedUpItem(string id)
        {
            UnityEngine.Assertions.Assert.IsFalse(CheckPickedUpItem(id), $"Adding already picked up item Id {id} to the picked up list.");
            Instance._pickedUpItems.Add(id);
        }


        public static bool CheckOpenChest(IIdentifier identifier)
        {
            return CheckOpenChest(identifier.Id);
        }
        public static bool CheckOpenChest(string id)
        {
            return Instance._openChests.Contains(id);
        }

        public static void AddOpenChest(IIdentifier identifier)
        {
            AddOpenChest(identifier.Id);
        }
        public static void AddOpenChest(string id)
        {
            UnityEngine.Assertions.Assert.IsFalse(CheckOpenChest(id), $"Adding already open chest Id {id} to the open list.");
            Instance._openChests.Add(id);
        }


        public static bool CheckDiscoveredBoard(IIdentifier identifier)
        {
            return CheckDiscoveredBoard(identifier.Id);
        }
        public static bool CheckDiscoveredBoard(string id)
        {
            return Instance._discoveredBoards.Contains(id);
        }

        public static void AddDiscoveredBoard(IIdentifier identifier)
        {
            AddDiscoveredBoard(identifier.Id);
        }
        public static void AddDiscoveredBoard(string id)
        {
            // Backtracking is allowed so some boards will already be discovered.
            if (!CheckDiscoveredBoard(id))
                Instance._discoveredBoards.Add(id);
        }
    }

    public partial class FlagsManager : RSLib.Framework.ConsoleProSingleton<FlagsManager>
    {
        public static void Load(XElement flagsElement)
        {
            XElement genericsElement = flagsElement.Element("Generics");
            if (genericsElement != null)
                foreach (XElement genericElement in genericsElement.Elements("GenericId"))
                    AddGeneric(genericElement.Value);

            XElement pickedUpItemsElement = flagsElement.Element("PickedUpItems");
            if (pickedUpItemsElement != null)
                foreach (XElement pickedUpItemElement in pickedUpItemsElement.Elements("ItemId"))
                    AddPickedUpItem(pickedUpItemElement.Value);

            XElement openChestsElement = flagsElement.Element("OpenChests");
            if (openChestsElement != null)
                foreach (XElement openChestElement in openChestsElement.Elements("ChestId"))
                    AddOpenChest(openChestElement.Value);

            XElement discoveredBoardsElement = flagsElement.Element("DiscoveredBoards");
            if (discoveredBoardsElement != null)
                foreach (XElement discoveredBoardElement in discoveredBoardsElement.Elements("BoardId"))
                    AddDiscoveredBoard(discoveredBoardElement.Value);
        }

        public static XElement Save()
        {
            XElement flagsElement = new XElement("Flags");

            if (Instance._generics.Count > 0)
            {
                //// [TODO] Check if this works or returns the value without changing the ref.
                //// And factorize the sort method when factorizing the ids lists.
                //Instance._generics.Sort();

                XElement genericsElement = new XElement("Generics");
                Instance._generics.ForEach(o => genericsElement.Add(new XElement("GenericId", o)));
                flagsElement.Add(genericsElement);
            }

            if (Instance._pickedUpItems.Count > 0)
            {
                XElement pickedUpItemsElement = new XElement("PickedUpItems");
                Instance._pickedUpItems.ForEach(o => pickedUpItemsElement.Add(new XElement("ItemId", o)));
                flagsElement.Add(pickedUpItemsElement);
            }

            if (Instance._openChests.Count > 0)
            {
                XElement openChestsElement = new XElement("OpenChests");
                Instance._openChests.ForEach(o => openChestsElement.Add(new XElement("ChestId", o)));
                flagsElement.Add(openChestsElement);
            }

            if (Instance._discoveredBoards.Count > 0)
            {
                XElement discoveredBoardsElementElement = new XElement("DiscoveredBoards");
                Instance._discoveredBoards.ForEach(o => discoveredBoardsElementElement.Add(new XElement("BoardId", o)));
                flagsElement.Add(discoveredBoardsElementElement);
            }

            return flagsElement;
        }
    }
}