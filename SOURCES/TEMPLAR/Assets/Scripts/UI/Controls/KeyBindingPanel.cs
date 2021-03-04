using RSLib.Framework.InputSystem;
using UnityEngine;

public class KeyBindingPanel : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _actionName = null;
    [SerializeField] private TMPro.TextMeshProUGUI _btnName = null;
    [SerializeField] private TMPro.TextMeshProUGUI _altBtnName = null;

    [SerializeField] private UnityEngine.UI.Button _baseBtnButton = null;
    [SerializeField] private UnityEngine.UI.Button _altBtnButton = null;

    private (KeyCode btn, KeyCode altBtn) _btns;

    public InputAction Action { get; private set; }

    public UnityEngine.UI.Button BaseBtnButton => _baseBtnButton;
    public UnityEngine.UI.Button AltBtnButton => _altBtnButton;

    public void Init(InputAction action, (KeyCode btn, KeyCode alt) btns)
    {
        Action = action;
        _btns = btns;

        _actionName.text = action.ToString();
        _btnName.text = _btns.btn.ToString();
        _altBtnName.text = _btns.altBtn.ToString();

        Show();
    }

    public void OverrideKey(KeyCode btn, bool alt)
    {
        if (alt)
            _btns.altBtn = btn;
        else
            _btns.btn = btn;

        _btnName.text = _btns.btn.ToString();
        _altBtnName.text = _btns.altBtn.ToString();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}