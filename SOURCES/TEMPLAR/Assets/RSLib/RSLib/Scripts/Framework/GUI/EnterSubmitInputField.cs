namespace RSLib.Framework.GUI
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(UnityEngine.UI.InputField))]
    public class EnterSubmitInputField : MonoBehaviour
    {
        [System.Serializable]
        public class SubmitEvent : UnityEvent<string>
        {
        }

        [SerializeField] private SubmitEvent _onSubmit = new SubmitEvent();

        private UnityEngine.UI.InputField _inputField;
        private bool _wasFocused;

        private void Awake()
        {
            _inputField = GetComponent<UnityEngine.UI.InputField>();
        }

        private void Update()
        {
            if (_wasFocused && _inputField.text != string.Empty && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
            {
                _onSubmit.Invoke(_inputField.text);
                _inputField.text = string.Empty;
                _inputField.Select();
                _inputField.ActivateInputField();
            }

            _wasFocused = _inputField.isFocused;
        }
    }
}