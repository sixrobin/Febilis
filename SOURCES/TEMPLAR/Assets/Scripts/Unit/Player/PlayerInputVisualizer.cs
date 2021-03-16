namespace Templar.Unit.Player
{
    using UnityEngine;

    public class PlayerInputVisualizer : MonoBehaviour
    {
        [SerializeField] private bool _jump = false;
        [SerializeField] private bool _roll = false;
        [SerializeField] private bool _attack = false;

        [SerializeField] private PlayerController _playerCtrl = null;

        private void LateUpdate()
        {
            if (_playerCtrl == null)
                return;

            _jump = _playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.JUMP);
            _roll = _playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ROLL);
            _attack = _playerCtrl.InputCtrl.CheckInput(PlayerInputController.ButtonCategory.ATTACK);
        }
    }
}