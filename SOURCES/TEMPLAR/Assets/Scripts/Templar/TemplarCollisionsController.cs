using UnityEngine;

public class TemplarCollisionsController : CollisionsController
{
    private TemplarController _templarCtrl;
    private LayerMask _rollCollisionMask;

    public TemplarCollisionsController(BoxCollider2D boxCollider2D, LayerMask baseCollisionMask, LayerMask rollCollisionMask, TemplarController templarCtrl)
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