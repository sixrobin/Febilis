namespace RSLib.Framework.InputSystem
{
    using System.Xml.Linq;
    using UnityEngine;

    /// <summary>
    /// Contains an input map informations.
    /// </summary>
    public class InputMap
    {
        /// <summary>
        /// Key is the action (representing a button and NOT an axis).
        /// Value is a tuple containing both valid KeyCodes for the action.
        /// </summary>
        private System.Collections.Generic.Dictionary<InputAction, (KeyCode btn, KeyCode altBtn)> _map
            = new System.Collections.Generic.Dictionary<InputAction, (KeyCode btn, KeyCode altBtn)>(new Comparers.EnumComparer<InputAction>());

        public System.Collections.Generic.Dictionary<InputAction, (KeyCode btn, KeyCode altBtn)> MapCopy =>
            new System.Collections.Generic.Dictionary<InputAction, (KeyCode btn, KeyCode altBtn)>(_map);

        public InputMap()
        {
        }

        public InputMap(InputMapDatas mapDatas)
        {
            GenerateMap(mapDatas);
        }

        public InputMap(XContainer container)
        {
            Deserialize(container);
        }

        /// <summary>
        /// Clears the map dictionary.
        /// </summary>
        public void Clear()
        {
            _map.Clear();
        }

        /// <summary>
        /// Generates map using ScriptableObject datas as template.
        /// </summary>
        /// <param name="mapDatas">Template datas.</param>
        public void GenerateMap(InputMapDatas mapDatas)
        {
            Clear();

            for (int i = mapDatas.Bindings.Length - 1; i >= 0; --i)
                CreateAction(mapDatas.Bindings[i].Action, mapDatas.Bindings[i].KeyCodes);

            InputManager.Instance.Log($"Generated {_map.Count} input bindings.");
        }

        /// <summary>
        /// Deserializes saved datas of an input map.
        /// </summary>
        /// <param name="container">Saved map XContainer.</param>
        /// <returns>True if deserialization has been done successfully, else false.</returns>
        public bool Deserialize(XContainer container)
        {
            XElement inputMapElement = container.Element("InputMap");
            foreach (XElement keyBindingElement in inputMapElement.Elements())
            {
                if (!System.Enum.TryParse(keyBindingElement.Name.LocalName, out InputAction action))
                {
                    InputManager.Instance.LogError(
                        $"Could not parse {keyBindingElement.Name.LocalName} to a valid InputAction. Restoring default input mapping.");
                    return false;
                }

                XElement btnElement = keyBindingElement.Element("Btn");
                if (!System.Enum.TryParse(btnElement.Value, out KeyCode btnKeyCode))
                {
                    InputManager.Instance.LogError(
                        $"Could not parse {btnElement.Value} to a valid UnityEngine.KeyCode. Restoring default input mapping.");
                    return false;
                }

                XElement altBtnElement = keyBindingElement.Element("AltBtn");
                if (!System.Enum.TryParse(altBtnElement.Value, out KeyCode altBtnKeyCode))
                {
                    InputManager.Instance.LogError(
                        $"Could not parse {altBtnElement.Value} to a valid UnityEngine.KeyCode. Restoring default input mapping.");
                    return false;
                }

                CreateAction(action, (btnKeyCode, altBtnKeyCode));
            }

            return true;
        }

        public bool HasAction(InputAction action)
        {
            return _map.ContainsKey(action);
        }

        /// <summary>
        /// Creates an action, added to the map dictionary.
        /// </summary>
        /// <param name="action">Action type to create.</param>
        /// <param name="btns">Both action KeyCodes.</param>
        public void CreateAction(InputAction action, (KeyCode btn, KeyCode altBtn) btns)
        {
            UnityEngine.Assertions.Assert.IsFalse(HasAction(action), $"Trying to create an already know InputAction {action.ToString()}.");
            _map.Add(action, btns);
        }

        /// <summary>
        /// Gets both valid KeyCodes for a given action.
        /// </summary>
        /// <param name="action">Action to get KeyCodes of.</param>
        /// <returns>Tuple containing KeyCodes.</returns>
        public (KeyCode btn, KeyCode altBtn) GetActionKeyCodes(InputAction action)
        {
            UnityEngine.Assertions.Assert.IsTrue(HasAction(action), $"Looking for keyCodes of unknown InputAction {action.ToString()}.");
            return _map[action];
        }

        /// <summary>
        /// Overrides a KeyCode for a given action.
        /// </summary>
        /// <param name="action">Action to override KeyCode of.</param>
        /// <param name="keyCode">KeyCode to set.</param>
        /// <param name="alt">Set the base button or the alternate one.</param>
        public void SetActionButton(InputAction action, KeyCode keyCode, bool alt = false)
        {
            UnityEngine.Assertions.Assert.IsTrue(HasAction(action), $"Trying to set keyCode of unknown InputAction {action.ToString()}.");

            (KeyCode btn, KeyCode altBtn) newBinding = _map[action];

            if (alt)
                newBinding.altBtn = keyCode;
            else
                newBinding.btn = keyCode;

            _map[action] = newBinding;
        }
    }
}