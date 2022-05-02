namespace Templar.UI.Boss
{
    using RSLib.Extensions;
    using UnityEngine;

    public class BossFightHUD : HUDElement
    {
        private const string BOSS_NAME_APPEAR = "Appear";

        [Header("HEALTH BARS")]
        [SerializeField] private BossHealthView _bossHealthViewPrefab = null;
        [SerializeField] private RectTransform _bossHealthViewsLayout = null;

        [Header("BOSS NAME")]
        [SerializeField] private GameObject _bossNameContainer = null;
        [SerializeField] private Animator _bossNameAnimator = null;
        [SerializeField] private TMPro.TextMeshProUGUI[] _bossNameTexts = null; // Multiple text to include potential shadow.

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
            StartCoroutine(DisplayBossHUDCoroutine());
        }

        private void OnBossFightOver(Templar.Boss.BossFight.BossFightOverEventArgs args)
        {
            for (int i = 0; i < _bossesViewsPool.Count; ++i)
                _bossesViewsPool[i].Display(false);

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

            // Hide pool excess.
            for (; i < _bossesViewsPool.Count; ++i)
                _bossesViewsPool[i].Display(false);

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_bossHealthViewsLayout);
        }

        private System.Collections.IEnumerator DisplayBossHUDCoroutine()
        {
            for (int i = _bossNameTexts.Length - 1; i >= 0; --i)
                _bossNameTexts[i].text = _currBossFight.Identifier.Id;

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currBossFight.BossIntroDatas.BossNameAppearanceDelay);
            
            _bossNameContainer.SetActive(true);
            _bossNameAnimator.SetTrigger(BOSS_NAME_APPEAR);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_currBossFight.BossIntroDatas.BossNameDuration);

            _bossNameContainer.SetActive(false);
        
            InitBossesHealthViews();
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