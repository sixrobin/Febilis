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
        /// Key is the action id (representing a button and NOT an axis).
        /// Value is a tuple containing both valid KeyCodes for the action.
        /// </summary>
        private System.Collections.Generic.Dictionary<string, (KeyCode btn, KeyCode altBtn)> _map
            = new System.Collections.Generic.Dictionary<string, (KeyCode btn, KeyCode altBtn)>();

        public System.Collections.Generic.Dictionary<string, (KeyCode btn, KeyCode altBtn)> MapCopy =>
            new System.Collections.Generic.Dictionary<string, (KeyCode btn, KeyCode altBtn)>(_map);

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

        /// <summary>Clears the map dictionary.</summary>
        public void Clear()
        {
            _map.Clear();
        }

        /// <summary> Generates map using ScriptableObject datas as template.</summary>
        /// <param name="mapDatas">Template datas.</param>
        public void GenerateMap(InputMapDatas mapDatas)
        {
            Clear();

            for (int i = mapDatas.Bindings.Length - 1; i >= 0; --i)
                CreateAction(mapDatas.Bindings[i].ActionId, mapDatas.Bindings[i].KeyCodes);

            InputManager.Instance.Log($"Generated {_map.Count} input bindings.");
        }

        /// <summary>Deserializes saved datas of an input map.</summary>
        /// <param name="container">Saved map XContainer.</param>
        /// <returns>True if deserialization has been done successfully, else false.</returns>
        public bool Deserialize(XContainer container)
        {
            XElement inputMapElement = container.Element("InputMap");
            foreach (XElement keyBindingElement in inputMapElement.Elements())
            {
                string actionId = keyBindingElement.Name.LocalName;

                XAttribute btnAttribute = keyBindingElement.Attribute("Btn");
                if (!System.Enum.TryParse(btnAttribute.Value, out KeyCode btnKeyCode))
                {
                    InputManager.Instance.LogError($"Could not parse {btnAttribute.Value} to a valid UnityEngine.KeyCode. Restoring default input mapping.");
                    return false;
                }

                XAttribute altBtnAttribute = keyBindingElement.Attribute("AltBtn");
                if (!System.Enum.TryParse(altBtnAttribute.Value, out KeyCode altBtnKeyCode))
                {
                    InputManager.Instance.LogError($"Could not parse {altBtnAttribute.Value} to a valid UnityEngine.KeyCode. Restoring default input mapping.");
                    return false;
                }

                CreateAction(actionId, (btnKeyCode, altBtnKeyCode));
            }

            return true;
        }

        public bool HasAction(string actionId)
        {
            return _map.ContainsKey(actionId);
        }

        /// <summary>Creates an action, added to the map dictionary.</summary>
        /// <param name="actionId">Action Id type to create.</param>
        /// <param name="btns">Both action KeyCodes.</param>
        public void CreateAction(string actionId, (KeyCode btn, KeyCode altBtn) btns)
        {
            UnityEngine.Assertions.Assert.IsFalse(HasAction(actionId), $"Trying to create an already know action Id {actionId}.");
            _map.Add(actionId, btns);
        }

        /// <summary>Gets both valid KeyCodes for a given action.</summary>
        /// <param name="actionId">Action Id to get KeyCodes of.</param>
        /// <returns>Tuple containing KeyCodes.</returns>
        public (KeyCode btn, KeyCode altBtn) GetActionKeyCodes(string actionId)
        {
            UnityEngine.Assertions.Assert.IsTrue(HasAction(actionId), $"Looking for keyCodes of unknown action Id {actionId}.");
            return _map[actionId];
        }

        /// <summary>Overrides a KeyCode for a given action.</summary>
        /// <param name="actionId">Action Id to override KeyCode of.</param>
        /// <param name="keyCode">KeyCode to set.</param>
        /// <param name="alt">Set the base button or the alternate one.</param>
        public void SetActionButton(string actionId, KeyCode keyCode, bool alt = false)
        {
            UnityEngine.Assertions.Assert.IsTrue(HasAction(actionId), $"Trying to set keyCode of unknown action Id {actionId}.");

            (KeyCode btn, KeyCode altBtn) newBinding = _map[actionId];

            if (alt)
                newBinding.altBtn = keyCode;
            else
                newBinding.btn = keyCode;

            _map[actionId] = newBinding;
        }
    }
}