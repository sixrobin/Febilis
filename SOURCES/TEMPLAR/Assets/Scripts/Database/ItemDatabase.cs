namespace Templar.Database
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using System.Xml.Linq;
    using RSLib.Extensions;
#endif

    public class ItemDatabase : RSLib.Framework.ConsoleProSingleton<ItemDatabase>, IDatabase
    {
        public const string ITEM_PREFIX = "Item_";

        [SerializeField] private TextAsset _inventoryDatas = null;
        [SerializeField] private TextAsset[] _itemsDatas = null;
        [SerializeField] private Sprite[] _sprites = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private string clipsAssetsRootPath = "Assets/Textures/Item";
#endif

        public static System.Collections.Generic.Dictionary<string, Sprite> ItemsSprites { get; private set; }
        public static System.Collections.Generic.Dictionary<string, Datas.Item.ItemDatas> ItemsDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, int> NativeInventoryItems { get; private set; }

        void IDatabase.Load()
        {
            DeserializeItemsDatas();
            GenerateSpritesDictionary();
            DeserializeInventoryDatas();

            // [TODO] Check if all items have a sprite.
        }

        public static Sprite GetItemSprite(string id)
        {
            if (!ItemsSprites.TryGetValue($"{ITEM_PREFIX}{id}", out Sprite itemSprite))
            {
                Instance.LogWarning($"Item sprite not found in {Instance.GetType().Name} for Id {id}, returning null.");
                return null;
            }

            return itemSprite;
        }

        public static Sprite GetItemSprite(Item.Item item)
        {
            return GetItemSprite(item.Id);
        }

        private void GenerateSpritesDictionary()
        {
            ItemsSprites = new System.Collections.Generic.Dictionary<string, Sprite>();

            for (int i = _sprites.Length - 1; i >= 0; --i)
            {
                UnityEngine.Assertions.Assert.IsFalse(
                    ItemsSprites.ContainsKey(_sprites[i].name),
                    $"Sprite \"{_sprites[i].name}\" has already been registered in {GetType().Name}.");

                ItemsSprites.Add(_sprites[i].name, _sprites[i]);
            }

            Log($"Generated {ItemsSprites.Count} items sprites dictionary entries.");
        }

        private void DeserializeItemsDatas()
        {
            ItemsDatas = new System.Collections.Generic.Dictionary<string, Datas.Item.ItemDatas>();

            System.Collections.Generic.List<XElement> allFilesElements = new System.Collections.Generic.List<XElement>();

            // Gather all documents main element in a list.
            for (int i = _itemsDatas.Length - 1; i >= 0; --i)
            {
                XDocument itemsDatasDoc = XDocument.Parse(_itemsDatas[i].text, LoadOptions.SetBaseUri);
                allFilesElements.Add(itemsDatasDoc.Element("ItemsDatas"));
            }

            for (int i = allFilesElements.Count - 1; i >= 0; --i)
            {
                foreach (XElement itemElement in allFilesElements[i].Elements("ItemDatas"))
                {
                    Datas.Item.ItemDatas itemDatas = new Datas.Item.ItemDatas(itemElement);
                    ItemsDatas.Add(itemDatas.Id, itemDatas);
                }
            }

            Log($"Deserialized {ItemsDatas.Count} items datas.");
        }

        private void DeserializeInventoryDatas()
        {
            NativeInventoryItems = new System.Collections.Generic.Dictionary<string, int>();

            XDocument inventoryDatasDoc = XDocument.Parse(_inventoryDatas.text, LoadOptions.SetBaseUri);
            XElement inventoryElement = inventoryDatasDoc.Element("InventoryDatas");

            XElement nativeContentElement = inventoryElement.Element("NativeContent");

            foreach (XElement nativeItemElement in nativeContentElement.Elements("NativeItem"))
            {
                UnityEngine.Assertions.Assert.IsTrue(nativeItemElement.Attribute("Id") != null, $"NativeItem element needs an Id attribute.");
                string id = nativeItemElement.Attribute("Id").Value;

                UnityEngine.Assertions.Assert.IsTrue(nativeItemElement.Attribute("Quantity") != null, $"NativeItem element needs a Quantity attribute.");
                int quantity = nativeItemElement.Attribute("Quantity").ValueToInt();

                NativeInventoryItems.Add(id, quantity);
            }
            
            Log($"Deserialized {NativeInventoryItems.Count} native inventory items.");
        }

#if UNITY_EDITOR
        public void GetSpritesFromAssets()
        {
            Instance.Log($"Getting items sprites in subfolders of path {clipsAssetsRootPath}...", true);

            System.Collections.Generic.List<Sprite> sprites = new System.Collections.Generic.List<Sprite>();

            foreach (string subDirectory in RSLib.EditorUtilities.AssetDatabaseUtilities.GetSubFoldersRecursively(clipsAssetsRootPath))
                foreach (Sprite sprite in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<Sprite>(subDirectory))
                    sprites.Add(sprite);

            foreach (Sprite sprite in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<Sprite>(clipsAssetsRootPath))
                sprites.Add(sprite);

            Instance.Log($"Found {sprites.Count} sprite(s).", true);
            _sprites = sprites.OrderBy(o => o.name).ToArray();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDatabaseEditor : RSLib.EditorUtilities.ButtonProviderEditor<ItemDatabase>
    {
        protected override void DrawButtons()
        {
            DrawButton("Get Items Sprites from Assets", Obj.GetSpritesFromAssets);
        }
    }
#endif
}