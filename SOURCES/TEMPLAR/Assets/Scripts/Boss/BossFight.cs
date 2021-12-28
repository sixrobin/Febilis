namespace Templar.Boss
{
    using Templar.Unit;
    using UnityEngine;

    public class BossFight : MonoBehaviour
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

        [SerializeField] private Unit.Enemy.BossController[] _fightBosses = null;

        public delegate void BossFightEventHandler(BossFightEventArgs args);
        public delegate void BossFightOverEventHandler(BossFightOverEventArgs args);

        public static event BossFightEventHandler BossFightStarted;
        public static event BossFightOverEventHandler BossFightOver;

        public Unit.Enemy.BossController[] FightBosses => _fightBosses;

        private int _bossesToKillLeft;

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

        // [TODO] Call this from trigger enter or enemy detection.
        public void TriggerFight()
        {
            _bossesToKillLeft = FightBosses.Length;

            for (int i = FightBosses.Length - 1; i >= 0; --i)
                FightBosses[i].HealthCtrl.UnitKilled += OnBossKilled;

            Manager.GameManager.PlayerCtrl.HealthCtrl.UnitKilled += OnPlayerKilled;

            BossFightStarted?.Invoke(new BossFightEventArgs(this));
        }

        public void OnFightWon()
        {
            Debug.LogError("Boss fight won!");
            BossFightOver?.Invoke(new BossFightOverEventArgs(this, true));
        }

        public void OnFightLost()
        {
            Debug.LogError("Boss fight lost...");
            BossFightOver?.Invoke(new BossFightOverEventArgs(this, false));
        }
    }
}