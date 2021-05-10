namespace Templar.Datas.Unit.Player
{
    using UnityEngine;

    public partial class PlayerDatabase : RSLib.Framework.ConsoleProSingleton<PlayerDatabase>
    {
        [SerializeField] private AnimationClip[] _animationClips = null;

#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private string clipsAssetsRootPath = "Assets/Animations/Templar";
#endif

        public static System.Collections.Generic.Dictionary<string, AnimationClip> AnimationClips { get; private set; }

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

            foreach (AnimationClip clip in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<AnimationClip>(clipsAssetsRootPath))
                clips.Add(clip);

            Instance.Log($"Found {clips.Count} clip(s).");
            _animationClips = clips.ToArray();

            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            GenerateClipsDictionary();
        }
    }
}