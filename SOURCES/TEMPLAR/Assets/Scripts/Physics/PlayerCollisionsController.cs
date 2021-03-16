namespace Templar.Physics
{
    using UnityEngine;

    public class PlayerCollisionsController : CollisionsController
    {
        private Unit.Player.PlayerController _templarCtrl;
        private LayerMask _rollCollisionMask;

        public PlayerCollisionsController(BoxCollider2D boxCollider2D, LayerMask baseCollisionMask, LayerMask rollCollisionMask, Unit.Player.PlayerController templarCtrl)
            : base(boxCollider2D, baseCollisionMask)
        {
            _templarCtrl = templarCtrl;
            _rollCollisionMask = rollCollisionMask;
        }

        public override LayerMask ComputeCollisionMask()
        {
            return _templarCtrl.RollCtrl.IsRolling ? _rollCollisionMask : base.ComputeCollisionMask();
        }
    }
}