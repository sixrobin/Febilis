namespace Templar.UI.Boss
{
    using RSLib.Extensions;
    using UnityEngine;

    public class BossFightHUD : HUDElement
    {
        [SerializeField] private BossHealthView _bossHealthViewPrefab = null;
        [SerializeField] private RectTransform _bossHealthViewsLayout = null;

        private Templar.Boss.BossFight _currBossFight;

        private System.Collections.Generic.List<BossHealthView> _bossesViewsPool = new System.Collections.Generic.List<BossHealthView>();

        protected override bool CanBeDisplayed()
        {
            return base.CanBeDisplayed() && _currBossFight != null;
        }

        private void OnBossFightStarted(Templar.Boss.BossFight.BossFightEventArgs args)
        {
            _currBossFight = args.BossFight;

            Display(true);

            InitBossesHealthViews();
            StartCoroutine(DisplayBossName());
        }

        private void OnBossFightOver(Templar.Boss.BossFight.BossFightOverEventArgs args)
        {
            for (int i = 0; i < _bossesViewsPool.Count; ++i)
                _bossesViewsPool[i].Display(false);

            if (args.Victory)
            {
                Debug.LogError("Showing boss victory screen.");
            }

            _currBossFight = null;
        }

        private void InitBossesHealthViews()
        {
            int i = 0;

            for (; i < _currBossFight.FightBosses.Length; ++i)
            {
                if (i >= _bossesViewsPool.Count)
                    _bossesViewsPool.Add(Instantiate(_bossHealthViewPrefab, _bossHealthViewsLayout));

                _bossesViewsPool[i].Init(_currBossFight.FightBosses[i]);
                _bossesViewsPool[i].Display(true);
            }

            // Hide pool excedent.
            for (; i < _bossesViewsPool.Count; ++i)
                _bossesViewsPool[i].Display(false);

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_bossHealthViewsLayout);
        }

        private System.Collections.IEnumerator DisplayBossName()
        {
            Debug.LogError("Showing boss name.");
            yield break;
        }

        protected override void Awake()
        {
            base.Awake();

            Templar.Boss.BossFight.BossFightStarted += OnBossFightStarted;
            Templar.Boss.BossFight.BossFightOver += OnBossFightOver;

            _bossHealthViewsLayout.DestroyChildren();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Templar.Boss.BossFight.BossFightStarted -= OnBossFightStarted;
            Templar.Boss.BossFight.BossFightOver -= OnBossFightOver;
        }
    }
}