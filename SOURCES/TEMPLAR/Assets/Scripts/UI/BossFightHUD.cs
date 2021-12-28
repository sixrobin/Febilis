namespace Templar.UI
{
    using System;
    using UnityEngine;

    public class BossFightHUD : MonoBehaviour
    {
        private Boss.BossFight _currBossFight;

        private void OnBossFightStarted(Boss.BossFight.BossFightEventArgs args)
        {
            _currBossFight = args.BossFight;

            Debug.LogError("Showing boss name.");
            Debug.LogError("Enabling bosses health bars.");

            for (int i = _currBossFight.FightBosses.Length - 1; i >= 0; --i)
                _currBossFight.FightBosses[i].HealthCtrl.UnitKilled += OnBossKilled;
        }

        private void OnBossKilled(Unit.UnitHealthController.UnitKilledEventArgs args)
        {
            Debug.LogError("Boss killed, removing its health bar.");
        }

        private void OnBossFightOver(Boss.BossFight.BossFightOverEventArgs args)
        {
            if (args.Victory)
            {
                Debug.LogError("Showing boss victory screen.");
            }

            for (int i = _currBossFight.FightBosses.Length - 1; i >= 0; --i)
                _currBossFight.FightBosses[i].HealthCtrl.UnitKilled -= OnBossKilled;

            _currBossFight = null;
        }

        private void Awake()
        {
            Boss.BossFight.BossFightStarted += OnBossFightStarted;
            Boss.BossFight.BossFightOver += OnBossFightOver;
        }

        private void OnDestroy()
        {
            Boss.BossFight.BossFightStarted -= OnBossFightStarted;
            Boss.BossFight.BossFightOver -= OnBossFightOver;
        
            if (_currBossFight != null)
                for (int i = _currBossFight.FightBosses.Length - 1; i >= 0; --i)
                    _currBossFight.FightBosses[i].HealthCtrl.UnitKilled -= OnBossKilled;
        }
    }
}