namespace Templar.Database
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class ItemDatabase : RSLib.Framework.ConsoleProSingleton<ItemDatabase>, IDatabase
    {
        public const string ITEM_PREFIX = "Item_";

        [SerializeField] private Sprite[] _sprites = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private string clipsAssetsRootPath = "Assets/Textures/Item";
#endif

        public static System.Collections.Generic.Dictionary<string, Sprite> ItemsSprites { get; private set; }

        void IDatabase.Load()
        {
            GenerateSpritesDictionary();
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