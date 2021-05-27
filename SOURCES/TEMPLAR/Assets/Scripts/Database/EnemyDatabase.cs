namespace Templar.Database
{
    using System.Linq;
    using System.Xml.Linq;
    using UnityEngine;

    public partial class EnemyDatabase : RSLib.Framework.ConsoleProSingleton<EnemyDatabase>, IDatabase
    {
        [SerializeField] private TextAsset _enemiesDatas = null;
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

        private void DeserializeEnemyDatas()
        {
            XDocument enemiesDatasDoc = XDocument.Parse(_enemiesDatas.text, LoadOptions.SetBaseUri);
            EnemiesDatas = new System.Collections.Generic.Dictionary<string, Datas.Unit.Enemy.EnemyDatas>();

            XElement enemiesDatasElement = enemiesDatasDoc.Element("EnemiesDatas");
            foreach (XElement enemyDatasElement in enemiesDatasElement.Elements("EnemyDatas"))
            {
                Datas.Unit.Enemy.EnemyDatas enemyDatas = new Datas.Unit.Enemy.EnemyDatas(enemyDatasElement);
                EnemiesDatas.Add(enemyDatas.Id, enemyDatas);
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

            Log($"Generated {AnimationClips.Count} animation clips.");
        }

#if UNITY_EDITOR
        [ContextMenu("Get AnimationClips from Assets")]
        private void GetAnimationClipsFromAssets()
        {
            Instance.Log($"Getting animation clips in subfolders of path {clipsAssetsRootPath}...");

            System.Collections.Generic.List<AnimationClip> clips = new System.Collections.Generic.List<AnimationClip>();

            foreach (string subDirectory in RSLib.EditorUtilities.AssetDatabaseUtilities.GetSubFoldersRecursively(clipsAssetsRootPath))
                foreach (AnimationClip clip in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<AnimationClip>(subDirectory))
                    clips.Add(clip);

            Instance.Log($"Found {clips.Count} clip(s).");
            _animationClips = clips.OrderBy(o => o.name).ToArray();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
            RSLib.EditorUtilities.PrefabEditorUtilities.SetCurrentPrefabStageDirty();
        }
#endif
    }
}