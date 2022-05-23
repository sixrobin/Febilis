namespace Templar.Database
{
    using System.Linq;
    using System.Xml.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class EnemyDatabase : RSLib.Framework.SingletonConsolePro<EnemyDatabase>, IDatabase
    {
        [SerializeField] private TextAsset[] _enemiesDatas = null;
        [SerializeField] private AnimationClip[] _animationClips = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private string clipsAssetsRootPath = "Assets/Animations/Enemy";
#endif

        public static System.Collections.Generic.Dictionary<string, Datas.Unit.Enemy.EnemyDatas> EnemiesDatas { get; private set; }
        public static System.Collections.Generic.Dictionary<string, AnimationClip> AnimationClips { get; private set; }

        void IDatabase.Load()
        {
            DeserializeEnemyDatas();
            GenerateClipsDictionary();
        }

        System.Collections.Generic.IEnumerable<IDatabase> RSLib.Framework.ITopologicalSortedItem<IDatabase>.GetDependencies()
        {
            return null;
        }

        private void DeserializeEnemyDatas()
        {
            EnemiesDatas = new System.Collections.Generic.Dictionary<string, Datas.Unit.Enemy.EnemyDatas>();

            System.Collections.Generic.List<XElement> allFilesElements = new System.Collections.Generic.List<XElement>();
            
            // Gather all documents main element in a list.
            for (int i = _enemiesDatas.Length - 1; i >= 0; --i)
            {
                XDocument enemiesDatasDoc = XDocument.Parse(_enemiesDatas[i].text, LoadOptions.SetBaseUri);
                allFilesElements.Add(enemiesDatasDoc.Element("EnemiesDatas"));
            }

            for (int i = allFilesElements.Count - 1; i >= 0; --i)
            {
                foreach (XElement enemyElement in allFilesElements[i].Elements("EnemyDatas"))
                {
                    Datas.Unit.Enemy.EnemyDatas enemyDatas = new Datas.Unit.Enemy.EnemyDatas(enemyElement);
                    EnemiesDatas.Add(enemyDatas.Id, enemyDatas);
                }
            }

            Log($"Deserialized {EnemiesDatas.Count} enemies datas.");
        }

        private void GenerateClipsDictionary()
        {
            AnimationClips = new System.Collections.Generic.Dictionary<string, AnimationClip>();

            for (int i = _animationClips.Length - 1; i >= 0; --i)
            {
                UnityEngine.Assertions.Assert.IsFalse(
                    AnimationClips.ContainsKey(_animationClips[i].name),
                    $"Clip \"{_animationClips[i].name}\" has already been registered in {GetType().Name}.");

                AnimationClips.Add(_animationClips[i].name, _animationClips[i]);
            }

            Log($"Generated {AnimationClips.Count} animation clips dictionary entries.");
        }

#if UNITY_EDITOR
        public void GetAnimationClipsFromAssets()
        {
            Instance.Log($"Getting animation clips in subfolders of path {clipsAssetsRootPath}...", true);

            System.Collections.Generic.List<AnimationClip> clips = new System.Collections.Generic.List<AnimationClip>();

            foreach (string subDirectory in RSLib.EditorUtilities.AssetDatabaseUtilities.GetSubFoldersRecursively(clipsAssetsRootPath))
                foreach (AnimationClip clip in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<AnimationClip>(subDirectory))
                    clips.Add(clip);

            Instance.Log($"Found {clips.Count} clip(s).", true);
            _animationClips = clips.OrderBy(o => o.name).ToArray();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnemyDatabase))]
    public class EnemyDatabaseEditor : RSLib.EditorUtilities.ButtonProviderEditor<EnemyDatabase>
    {
        protected override void DrawButtons()
        {
            DrawButton("Get Animation Clips from Assets", Obj.GetAnimationClipsFromAssets);
        }
    }
#endif
}