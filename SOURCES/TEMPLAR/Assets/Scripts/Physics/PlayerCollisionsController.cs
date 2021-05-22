namespace Templar.Physics
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

        protected override void TryDestroy(Destroyables.IDestroyableObject destroyable)
        {
            if (_playerCtrl.RollCtrl.IsRolling && destroyable.ValidSourcesTypes.HasFlag(Destroyables.DestroyableSourceType.ROLL))
            {
                destroyable.Destroy(Destroyables.DestroyableSourceType.ROLL);
                return;
            }

            if (_playerCtrl.IsFalling && destroyable.ValidSourcesTypes.HasFlag(Destroyables.DestroyableSourceType.FALL))
                destroyable.Destroy(Destroyables.DestroyableSourceType.FALL);
        }
    }
}