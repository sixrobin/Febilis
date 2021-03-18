namespace RSLib.Framework.InputSystem
{
    using UnityEngine;

    /// <summary>
    /// Scriptable object meant to create a complete input mapping from the editor.
    /// Can be used for default input mapping or testing.
    /// </summary>
    [CreateAssetMenu(fileName = "New Input Map", menuName = "RSLib/Input Map")]
    public class InputMapDatas : ScriptableObject
    {
        [System.Serializable]
        public class KeyBinding
        {
            [SerializeField] private string _actionId = string.Empty;
            [SerializeField] private KeyCode _btn = KeyCode.None;
            [SerializeField] private KeyCode _altBtn = KeyCode.None;

            public string ActionId => _actionId;
            public (KeyCode btn, KeyCode altBtn) KeyCodes => (_btn, _altBtn);
        }

        [SerializeField] private KeyBinding[] _bindings = null;

        public KeyBinding[] Bindings => _bindings;
    }
}