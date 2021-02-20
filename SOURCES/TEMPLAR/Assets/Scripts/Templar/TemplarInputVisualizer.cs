using UnityEngine;

public class TemplarInputVisualizer : MonoBehaviour
{
    [SerializeField] private bool _jump = false;
    [SerializeField] private bool _roll = false;
    [SerializeField] private bool _attack = false;

    [SerializeField] private TemplarController _templarController = null;

    private void LateUpdate()
    {
        if (_templarController == null)
            return;

        _jump = _templarController.InputCtrl.CheckInput(TemplarInputController.ButtonCategory.JUMP);
        _roll = _templarController.InputCtrl.CheckInput(TemplarInputController.ButtonCategory.ROLL);
        _attack = _templarController.InputCtrl.CheckInput(TemplarInputController.ButtonCategory.ATTACK);
    }
}