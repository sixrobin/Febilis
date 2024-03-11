namespace Templar.Unit.Player
{
    using UnityEngine;

    public class PlayerInputVisualizer : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private PlayerController _playerCtrl = null;

        [Space]
        [SerializeField] private RSLib.Framework.DisabledBool _jump = new RSLib.Framework.DisabledBool(false);
        [SerializeField] private RSLib.Framework.DisabledBool _roll = new RSLib.Framework.DisabledBool(false);
        [SerializeField] private RSLib.Framework.DisabledBool _attack = new RSLib.Framework.DisabledBool(false);
        [SerializeField] private RSLib.Framework.DisabledBool _interaction = new RSLib.Framework.DisabledBool(false);
        [SerializeField] private RSLib.Framework.DisabledBool _heal = new RSLib.Framework.DisabledBool(false);

        private void LateUpdate()
        {
            if (_playerCtrl == null || _playerCtrl.InputCtrl == null)
                return;

            _jump = new RSLib.Framework.DisabledBool(_playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.JUMP));
            _roll = new RSLib.Framework.DisabledBool(_playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ROLL));
            _attack = new RSLib.Framework.DisabledBool(_playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ATTACK));
            _interaction = new RSLib.Framework.DisabledBool(_playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.INTERACT));
            _heal = new RSLib.Framework.DisabledBool(_playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.HEAL));
        }
#endif
    }
}