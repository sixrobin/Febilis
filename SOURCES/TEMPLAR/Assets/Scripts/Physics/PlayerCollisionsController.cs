﻿namespace Templar.Physics
{
    using UnityEngine;

    public class PlayerCollisionsController : CollisionsController
    {
        private Unit.Player.PlayerController _playerCtrl;
        private LayerMask _rollCollisionMask;

        public PlayerCollisionsController(BoxCollider2D boxCollider2D, LayerMask baseCollisionMask, LayerMask rollCollisionMask, Unit.Player.PlayerController templarCtrl)
            : base(boxCollider2D, baseCollisionMask)
        {
            _playerCtrl = templarCtrl;
            _rollCollisionMask = rollCollisionMask;
        }

        public override LayerMask ComputeCollisionMask()
        {
            return _playerCtrl.RollCtrl.IsRolling ? _rollCollisionMask : base.ComputeCollisionMask();
        }

        protected override bool TryDestroy(Destroyables.DestroyableObject destroyable)
        {
            bool success = false;

            if (_playerCtrl.RollCtrl.IsRolling)
                success = destroyable.TryDestroy(Destroyables.DestroyableSourceType.ROLL);

            if (!success && _playerCtrl.WasFallingLastFrame)
                success = destroyable.TryDestroy(Destroyables.DestroyableSourceType.FALL);

            return success;
        }
    }
}