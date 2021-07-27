namespace Templar.Boards
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class ScenesPassage : ScriptableObject, IBoardTransitionHandler
    {
        [SerializeField] private ScenesPassage _targetPassage = null;
        [SerializeField] private RSLib.Framework.DisabledString _targetSceneName = new RSLib.Framework.DisabledString(string.Empty); // Visualizer only.

        [SerializeField, HideInInspector] private ScenesPassagesHandler _passagesHandlerContainer;

        void IBoardTransitionHandler.OnBoardsTransitionBegan()
        {
        }

        void IBoardTransitionHandler.OnBoardsTransitionOver()
        {
        }

        public RSLib.Framework.SceneField GetTargetScene()
        {
            return _passagesHandlerContainer.Scene;
        }

        public void Init(ScenesPassagesHandler container, int subAssetIndex)
        {
            name = $"Passage {subAssetIndex}";
            SetContainer(container);
        }

        public ScenesPassage TargetPassage => _targetPassage;

        public void SetContainer(ScenesPassagesHandler container)
        {
            _passagesHandlerContainer = container;
        }

        public void Delete()
        {
            _passagesHandlerContainer.DeleteSubAsset(this);
        }

        private void UpdateTargetSceneName()
        {
            _targetSceneName = new RSLib.Framework.DisabledString(TargetPassage?._passagesHandlerContainer?.name ?? string.Empty);
        }

        private void Awake()
        {
            UpdateTargetSceneName();
        }

        private void OnValidate()
        {
            UpdateTargetSceneName();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScenesPassage))]
    public class ScenePassageEditor : RSLib.EditorUtilities.ButtonProviderEditor<ScenesPassage>
    {
        protected override void DrawButtons()
        {
            if (GUILayout.Button("Delete"))
                Obj.Delete();
        }
    }
#endif
}