﻿using UnityEngine;

public class EnemyHealthController : UnitHealthController
{
    [SerializeField] private WorldSpaceHealthBar _healthBar = null;

    public override HitLayer HitLayer => HitLayer.Enemy;

    public override void Init()
    {
        base.Init();
        _healthBar.HealthCtrl = this;
    }

    protected override void OnHealthChanged(RSLib.HealthSystem.HealthChangedEventArgs args)
    {
        base.OnHealthChanged(args);
        _healthBar?.UpdateHealth();
    }

    protected override void OnKilled()
    {
        base.OnKilled();
        _healthBar?.Display(false);
    }
}