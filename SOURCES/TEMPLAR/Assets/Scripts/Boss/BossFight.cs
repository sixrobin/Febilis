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
            public BossFightOverEventArgs(BossFight bossFight, bool victory) : base(bossFight)
            {
                Victory = victory;
            }

            public bool Victory { get; }
        }

        [SerializeField] private Flags.BossIdentifier _bossIdentifier = null;
        [SerializeField] private Datas.BossIntroDatas _bossIntroDatas = null;
        [SerializeField] private Unit.Enemy.EnemyController[] _fightBosses = null;

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
                OnFightWon();
        }

        private void OnPlayerKilled(UnitHealthController.UnitKilledEventArgs args)
        {
            OnFightLost();
        }

        public void TriggerFight()
        {
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

                StartCoroutine(FocusCameraOnBossCoroutine());
                if (_bossIntroDatas.DisallowInputs)
                    StartCoroutine(DisallowInputsCoroutine());
            }
            
            BossFightStarted?.Invoke(new BossFightEventArgs(this));
        }

        public void OnFightWon()
        {
            Debug.LogError($"Boss fight {Identifier.Id} won!");

            Manager.FlagsManager.Register(this);

            BossFightOver?.Invoke(new BossFightOverEventArgs(this, true));
        }

        public void OnFightLost()
        {
            Debug.LogError($"Boss fight {Identifier.Id} lost...");
            BossFightOver?.Invoke(new BossFightOverEventArgs(this, false));
        }

        private System.Collections.IEnumerator DisallowInputsCoroutine()
        {
            Manager.GameManager.PlayerCtrl.AllowInputs(false);
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_bossIntroDatas.TotalDuration);
            Manager.GameManager.PlayerCtrl.AllowInputs(true);
        }

        private System.Collections.IEnumerator FocusCameraOnBossCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_bossIntroDatas.CameraFocusDelay);

            Vector3 initPosition = Manager.GameManager.CameraCtrl.BaseTargetPosition;
            Vector3 focusPosition = RSLib.Helpers.ComputeAveragePosition(_fightBosses) + _bossIntroDatas.CameraFocusPositionOffset;
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

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_bossIntroDatas.CameraFocusedDuration);

            for (float t = 0f; t < 1f; t += Time.deltaTime / _bossIntroDatas.CameraOutDuration)
            {
                bossesAveragePivot.transform.position = Vector3.Lerp(focusPosition, initPosition, t.Ease(_bossIntroDatas.CameraOutCurve));
                yield return null;
            }
        
            bossesAveragePivot.transform.position = initPosition;
            Manager.GameManager.CameraCtrl.ResetOverrideTarget();
            Destroy(bossesAveragePivot);
        }

        private void Start()
        {
            if (Manager.FlagsManager.Check(this))
                Debug.LogError("[TODO] Boss fight already done, disabling it.", gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // [TMP] Did this to try out fight, but maybe this should be done another way.

            if (_bossesToKillLeft > -1) // [TMP] Condition to avoid triggering fight multiple times.
                return;

            if (collision.gameObject.TryGetComponent<Unit.Player.PlayerController>(out _))
            {
                GetComponent<BoxCollider2D>().enabled = false;
                TriggerFight();
            }
        }
    }
}