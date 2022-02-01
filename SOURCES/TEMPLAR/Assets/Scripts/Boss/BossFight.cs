namespace Templar.Boss
{
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

            StartCoroutine(BossIntroCoroutine());
            if (_bossIntroDatas.DisallowInputs)
                StartCoroutine(DisallowInputsCoroutine());

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

        private System.Collections.IEnumerator BossIntroCoroutine()
        {
            Vector2 bossesAveragePosition = RSLib.Helpers.ComputeAveragePosition(_fightBosses);
            GameObject bossesAveragePivot = new GameObject("Boss Camera Focus Position");
            bossesAveragePivot.transform.position = bossesAveragePosition;

            if (_bossIntroDatas.CameraFocusBoss)
                Manager.GameManager.CameraCtrl.SetOverrideTarget(bossesAveragePivot.transform);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_bossIntroDatas.TotalDuration);

            Manager.GameManager.CameraCtrl.ResetOverrideTarget();
            Destroy(bossesAveragePivot);
        }

        private void Start()
        {
            if (Manager.FlagsManager.Check(this))
                Debug.LogError("Boss fight already done, disabling it.", gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // [TMP] Did this to try out fight, but maybe this should be done another way.

            if (_bossesToKillLeft > -1) // [TMP] Condition to avoid triggering fight mutliple times.
                return;

            if (collision.gameObject.TryGetComponent<Unit.Player.PlayerController>(out _))
            {
                GetComponent<BoxCollider2D>().enabled = false;
                TriggerFight();
            }
        }
    }
}