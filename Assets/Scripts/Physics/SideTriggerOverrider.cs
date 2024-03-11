namespace Templar.Physics
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class SideTriggerOverrider : MonoBehaviour
    {
        [SerializeField] private bool _above = false;
        [SerializeField] private bool _below = false;
        [SerializeField] private bool _left = false;
        [SerializeField] private bool _right = false;

        private System.Collections.Generic.Dictionary<CollisionsController.CollisionOrigin, bool> _triggerSidesStates;

        public bool IsSideSetAsTrigger(CollisionsController.CollisionOrigin side)
        {
            UnityEngine.Assertions.Assert.IsFalse(side == CollisionsController.CollisionOrigin.NONE, "Trying to check if side of origin None is set as trigger.");
            return _triggerSidesStates[side];
        }

        private void Awake()
        {
            _triggerSidesStates = new System.Collections.Generic.Dictionary<CollisionsController.CollisionOrigin, bool>(
                new RSLib.Framework.Comparers.EnumComparer<CollisionsController.CollisionOrigin>())
            {
                { CollisionsController.CollisionOrigin.ABOVE, _above },
                { CollisionsController.CollisionOrigin.BELOW, _below },
                { CollisionsController.CollisionOrigin.LEFT, _left },
                { CollisionsController.CollisionOrigin.RIGHT, _right }
            };
        }
    }
}