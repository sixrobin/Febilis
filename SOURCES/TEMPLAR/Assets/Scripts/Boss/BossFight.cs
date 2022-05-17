namespace Templar.Boss
{
    using RSLib.Maths;
    using Templar.Unit;
    using UnityEngine;

    public class BossFight : MonoBehaviour, Flags.IIdentifiable
    {
        public class BossFightEventArgs : System.EventArgs
        {
            public BossFightEventArgs(BossFight bossFight)
            {
                BossFight = bossFight;
            }

            public BossFight BossFight { get; }
        }

        public class BossFightOverEventArgs : BossFightEventArgs
        {
            public BossFightOverEventArgs(BossFight bossFight, bool victory, bool onLoad) : base(bossFight)
            {
                Victory = victory;
                OnLoad = onLoad;
            }

            public bool Victory { get; }
            public bool OnLoad { get; }
        }

        [SerializeField] private Flags.BossIdentifier _bossIdentifier = null;
        [SerializeField] private Datas.BossIntroDatas _bossIntroDatas = null;
        [SerializeField] private Unit.Enemy.EnemyController[] _fightBosses = null;
        [SerializeField] private bool _forcePlayerDetection = true;

        [SerializeField] private UnityEngine.Events.UnityEvent _onFightWon = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onFightLost = null;

        [Header("FEEDBACK")]
        [SerializeField, Min(0f)] private float _fightWonFreezeFrameDelay = 0f;
        [SerializeField, Min(0f)] private float _fightWonFreezeFrameDuration = 1f;

        [Header("DEBUG")]
        [SerializeField] private bool _skipIntroCutscene = false;
        
        private int _bossesToKillLeft = -1;

        public delegate void BossFightEventHandler(BossFightEventArgs args);
        public delegate void BossFightOverEventHandler(BossFightOverEventArgs args);

        public static event BossFightEventHandler BossFightStarted;
        public static event BossFightOverEventHandler BossFightOver;

        public Unit.Enemy.EnemyController[] FightBosses => _fightBosses;
        public Datas.BossIntroDatas BossIntroDatas => _bossIntroDatas;

        public Flags.IIdentifier Identifier => _bossIdentifier;

        private void OnBossKilled(UnitHealthController.UnitKilledEventArgs args)
        {
            _bossesToKillLeft--;
            if (_bossesToKillLeft == 0)
                OnFightWon(args);
        }

        private void OnPlayerKilled(UnitHealthController.UnitKilledEventArgs args)
        {
            OnFightLost();
        }

        public void TriggerFight()
        {
            void ForcePlayerDetection()
            {
                if (_forcePlayerDetection)
                    for (int i = FightBosses.Length - 1; i >= 0; --i)
                        FightBosses[i].ForcePlayerDetection();
            }
            
            CProLogger.Log(this, $"Triggering boss fight {Identifier.Id}.", gameObject);

            _bossesToKillLeft = FightBosses.Length;

            for (int i = FightBosses.Length - 1; i >= 0; --i)
            {
                FightBosses[i].HealthCtrl.UnitKilled += OnBossKilled;
                FightBosses[i].EnemyHealthCtrl.WorldSpaceHealthBar.Disabled = true; // Bosses should not display world UI health.
            }

            Manager.GameManager.PlayerCtrl.HealthCtrl.UnitKilled += OnPlayerKilled;

            if (!_skipIntroCutscene)
            {
                Manager.GameManager.PlayerCtrl.RollCtrl.Interrupt();
                Manager.GameManager.PlayerCtrl.AttackCtrl.CancelAttack();

                StartCoroutine(FocusCameraOnBossCoroutine(ForcePlayerDetection));
                if (_bossIntroDatas.DisallowInputs)
                    StartCoroutine(DisallowInputsCoroutine());
            }
            else
            {
                ForcePlayerDetection();
            }
            
            BossFightStarted?.Invoke(new BossFightEventArgs(this));
        }

        public void OnFightWon(UnitHealthController.UnitKilledEventArgs args)
        {
            CProLogger.Log(this, $"Boss fight {Identifier.Id} won.");

            Manager.FlagsManager.Register(this);

            BossFightOver?.Invoke(new BossFightOverEventArgs(this, true, false));
            _onFightWon?.Invoke();

            System.Collections.Generic.List<UnitView> unitsViews = new System.Collections.Generic.List<UnitView>(2)
            {
                args.SourceUnit.UnitView,
                Manager.GameManager.PlayerCtrl.PlayerView
            };
            
            StencilManager.ShowStencils(unitsViews, _fightWonFreezeFrameDelay);
            
            Templar.Manager.FreezeFrameManager.FreezeFrame(_fightWonFreezeFrameDelay,
                                                           _fightWonFreezeFrameDuration,
                                                           overrideCurrFreeze: true,
                                                           callback: StencilManager.HideStencils);
        }

        public void OnFightLost()
        {
            CProLogger.Log(this, $"Boss fight {Identifier.Id} lost.");
            
            BossFightOver?.Invoke(new BossFightOverEventArgs(this, false, false));
            _onFightLost?.Invoke();
        }

        private System.Collections.IEnumerator DisallowInputsCoroutine()
        {
            Manager.GameManager.PlayerCtrl.AllowInputs(false);
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_bossIntroDatas.TotalDuration);
            Manager.GameManager.PlayerCtrl.AllowInputs(true);
        }

        private System.Collections.IEnumerator FocusCameraOnBossCoroutine(System.Action callback = null)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_bossIntroDatas.CameraFocusDelay);

            Vector3 initPosition = Manager.GameManager.CameraCtrl.transform.position;
            Vector3 focusPosition = RSLib.Helpers.ComputeAveragePosition(FightBosses) + _bossIntroDatas.CameraFocusPositionOffset;
            GameObject bossesAveragePivot = new GameObject("Boss Camera Focus Position");
            bossesAveragePivot.transform.position = initPosition;
            
            if (_bossIntroDatas.CameraFocusBoss)
                Manager.GameManager.CameraCtrl.SetOverrideTarget(bossesAveragePivot.transform);

            for (float t = 0f; t < 1f; t += Time.deltaTime / _bossIntroDatas.CameraInDuration)
            {
                bossesAveragePivot.transform.position = Vector3.Lerp(initPosition, focusPosition, t.Ease(_bossIntroDatas.CameraInCurve));
                yield return null;
            }

            bossesAveragePivot.transform.position = focusPosition;
            
            for (int i = FightBosses.Length - 1; i >= 0; --i)
                FightBosses[i].EnemyView.PlayRoarAnimation();
                
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_bossIntroDatas.CameraFocusedDuration);

            for (float t = 0f; t < 1f; t += Time.deltaTime / _bossIntroDatas.CameraOutDuration)
            {
                bossesAveragePivot.transform.position = Vector3.Lerp(focusPosition, initPosition, t.Ease(_bossIntroDatas.CameraOutCurve));
                yield return null;
            }
        
            bossesAveragePivot.transform.position = initPosition;
            Manager.GameManager.CameraCtrl.ResetOverrideTarget();
            Destroy(bossesAveragePivot);
            
            callback?.Invoke();
        }

        private void Start()
        {
            for (int i = FightBosses.Length - 1; i >= 0; --i)
                FightBosses[i].IsBossUnit = true;
            
            // Boss fight already won.
            if (Manager.FlagsManager.Check(this))
            {
                _bossesToKillLeft = 0;
                GetComponent<BoxCollider2D>().enabled = false;
                for (int i = FightBosses.Length - 1; i >= 0; --i)
                    FightBosses[i].gameObject.SetActive(false);
                
                BossFightOver?.Invoke(new BossFightOverEventArgs(this, true, true));
                _onFightWon?.Invoke();
            }
            
            RSLib.Debug.Console.DebugConsole.OverrideCommand("BossWin", "Instantly wins current boss fight.", () =>
            {
                if (_bossesToKillLeft > 0)
                    for (int i = FightBosses.Length - 1; i >= 0; --i)
                        if (!FightBosses[i].IsDead)
                            FightBosses[i].HealthCtrl.DebugDamageInfinite();
            });
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Cheap check to ensure fight cannot be triggered multiple times due to collision issue.
            if (_bossesToKillLeft > -1)
                return;

            if (collision.gameObject.TryGetComponent<Unit.Player.PlayerController>(out _))
            {
                GetComponent<BoxCollider2D>().enabled = false;
                TriggerFight();
            }
        }
    }
}