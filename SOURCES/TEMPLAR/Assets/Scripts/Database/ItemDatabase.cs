namespace Templar.Database
{
    using RSLib.Extensions;
    using System.Linq;
    using System.Xml.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class ItemDatabase : RSLib.Framework.ConsoleProSingleton<ItemDatabase>, IDatabase
    {
        public const string ITEM_PREFIX = "Item_";
        public const string ITEM_TYPE_PREFIX = "ItemType_";

        [SerializeField] private TextAsset _inventoryDatas = null;
        [SerializeField] private TextAsset[] _itemsDatas = null;
        [SerializeField] private Sprite[] _sprites = null;
        [SerializeField] private Sprite[] _typesSprites = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private string spritesAssetsRootPath = "Assets/Textures/Item";
        [SerializeField] private string typesIconsSubFolderName = "Type Icons";
#endif

        public static System.Collections.Generic.Dictionary<string, Sprite> ItemsSprites { get; private set; }
        public static System.Collections.Generic.Dictionary<Item.ItemType, Sprite> ItemTypesSprites { get; private set; }
        public static System.Collections.Generic.Dictionary<string, Datas.Item.ItemDatas> ItemsDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, int> NativeInventoryItems { get; private set; }

        void IDatabase.Load()
        {
            DeserializeItemsDatas();
            GenerateSpritesDictionary();
            GenerateTypesSpritesDictionary();
            DeserializeInventoryDatas();

#if UNITY_EDITOR
            Scan();
#endif
        }

        System.Collections.Generic.IEnumerable<IDatabase> RSLib.Framework.ITopologicalSortedItem<IDatabase>.GetDependencies()
        {
            return null;
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
            return GetItemSprite(item.Datas.Id);
        }
        
        public static Sprite GetItemTypeSprite(Item.ItemType type)
        {
            if (!ItemTypesSprites.TryGetValue(type, out Sprite typeSprite))
            {
                Instance.LogWarning($"Item sprite not found in {Instance.GetType().Name} for ItemType {type.ToString()}, returning null.");
                return null;
            }

            return typeSprite;
        }

        public static Sprite GetItemTypeSprite(Item.Item item)
        {
            return GetItemTypeSprite(item.Datas.Type);
        }

        private void GenerateSpritesDictionary()
        {
            ItemsSprites = new System.Collections.Generic.Dictionary<string, Sprite>();

            for (int i = _sprites.Length - 1; i >= 0; --i)
            {
                UnityEngine.Assertions.Assert.IsFalse(
                    ItemsSprites.ContainsKey(_sprites[i].name),
                    $"Sprite \"{_sprites[i].name}\" has already been registered in {GetType().Name} items dictionary.");

                ItemsSprites.Add(_sprites[i].name, _sprites[i]);
            }

            Log($"Generated {ItemsSprites.Count} items sprites dictionary entries.");
        }

        private void GenerateTypesSpritesDictionary()
        {
            ItemTypesSprites = new System.Collections.Generic.Dictionary<Item.ItemType, Sprite>(new RSLib.Framework.Comparers.EnumComparer<Item.ItemType>());

            for (int i = _typesSprites.Length - 1; i >= 0; --i)
            {
                Item.ItemType enumValue = _typesSprites[i].name.Remove(0, ITEM_TYPE_PREFIX.Length).ToUpper().ToEnum<Item.ItemType>();

                UnityEngine.Assertions.Assert.IsFalse(
                    ItemTypesSprites.ContainsKey(enumValue),
                    $"Sprite Type \"{_typesSprites[i].name}\" has already been registered in {GetType().Name} item types dictionary.");

                ItemTypesSprites.Add(enumValue, _typesSprites[i]);
            }

            Log($"Generated {ItemTypesSprites.Count} item types sprites dictionary entries.");
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

        private void Scan()
        {
            System.Collections.Generic.List<string> spritesKeys = ItemsSprites.Keys.ToList();
            System.Collections.Generic.List<string> datasKeys = ItemsDatas.Keys.ToList();

            if (spritesKeys.Count != datasKeys.Count)
            {
                Instance.LogWarning($"{GetType().Name} has a different amount of items sprites ({spritesKeys.Count}) than items datas ({datasKeys.Count}).");
                return;
            }

            spritesKeys.Sort();
            datasKeys.Sort();

            for (int i = 0; i < spritesKeys.Count; ++i)
                if (spritesKeys[i] != $"{ITEM_PREFIX}{datasKeys[i]}")
                    Instance.LogWarning($"Difference found in {GetType().Name} when comparing sorted list of sprites and datas for keys {spritesKeys[i]} and {datasKeys[i]}.");
        }

#if UNITY_EDITOR
        public void GetSpritesFromAssets()
        {
            Instance.Log($"Getting item sprites in subfolders of path {spritesAssetsRootPath}...", true);

            System.Collections.Generic.List<Sprite> sprites = new System.Collections.Generic.List<Sprite>();

            foreach (string subDirectory in RSLib.EditorUtilities.AssetDatabaseUtilities.GetSubFoldersRecursively(spritesAssetsRootPath))
                foreach (Sprite sprite in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<Sprite>(subDirectory))
                    if (!sprite.name.StartsWith(ITEM_TYPE_PREFIX))
                        sprites.Add(sprite);

            foreach (Sprite sprite in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<Sprite>(spritesAssetsRootPath))
                if (!sprite.name.StartsWith(ITEM_TYPE_PREFIX))
                    sprites.Add(sprite);

            _sprites = sprites.OrderBy(o => o.name).ToArray();
            Instance.Log($"Found {sprites.Count} sprite(s).", true);


            Instance.Log($"Getting items types sprites in subfolders of path {spritesAssetsRootPath}/{typesIconsSubFolderName}...", true);

            System.Collections.Generic.List<Sprite> typesSprites = new System.Collections.Generic.List<Sprite>();
            string typeIconsPath = $"{spritesAssetsRootPath}/{typesIconsSubFolderName}";

            foreach (string subDirectory in RSLib.EditorUtilities.AssetDatabaseUtilities.GetSubFoldersRecursively(typeIconsPath))
                foreach (Sprite sprite in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<Sprite>(subDirectory))
                    typesSprites.Add(sprite);

            foreach (Sprite sprite in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<Sprite>(typeIconsPath))
                typesSprites.Add(sprite);

            _typesSprites = typesSprites.OrderBy(o => o.name).ToArray();
            Instance.Log($"Found {typesSprites.Count} types sprite(s).", true);

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