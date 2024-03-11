namespace RSLib.Framework.InputSystem
{
    using System.Linq;
    using System.Xml.Linq;
    using UnityEngine;

    /// <summary>
    /// Contains an input map information.
    /// </summary>
    public class InputMap
    {
        /// <summary>
        /// Key is the action id (representing a button and NOT an axis).
        /// Value is a KeyBinding class containing inputs and other action related data.
        /// </summary>
        private System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding> _map = new System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding>();
        
        public System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding> MapCopy => new System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding>(_map);

        public bool UseAltButtons { get; private set; }
        
        public InputMap()
        {
        }

        public InputMap(InputMapDatas mapData)
        {
            GenerateMap(mapData);
            UseAltButtons = mapData.UseAltButtons;
        }

        public InputMap(XContainer container)
        {
            Deserialize(container);
        }

        public InputMap(InputMap inputMap)
        {
            _map = inputMap._map;
            UseAltButtons = inputMap.UseAltButtons;
        }

        public InputMap(System.Collections.Generic.Dictionary<string, InputMapDatas.KeyBinding> map, bool useAltButtons)
        {
            _map = map;
            UseAltButtons = useAltButtons;
        }

        /// <summary>
        /// Clears the map dictionary.
        /// </summary>
        public void Clear()
        {
            _map.Clear();
        }

        /// <summary>
        /// Generates map using ScriptableObject data as template.
        /// </summary>
        /// <param name="mapData">Template data.</param>
        public void GenerateMap(InputMapDatas mapData)
        {
            Clear();

            for (int i = 0; i < mapData.Bindings.Length; ++i)
            {
                InputMapDatas.KeyBinding keyBinding = new InputMapDatas.KeyBinding(
                    mapData.Bindings[i].ActionId,
                    mapData.Bindings[i].KeyCodes,
                    mapData.Bindings[i].UserAssignable);

                CreateAction(mapData.Bindings[i].ActionId, keyBinding);
            }

            InputManager.Instance.Log($"Generated {_map.Count} input bindings.", InputManager.Instance.gameObject);
        }

        /// <summary>
        /// Deserializes saved data of an input map.
        /// </summary>
        /// <param name="container">Saved map XContainer.</param>
        /// <returns>True if deserialization has been done successfully, else false.</returns>
        public void Deserialize(XContainer container)
        {
            InputMap defaultMap = InputManager.GetDefaultMapCopy();

            XElement inputMapElement = container.Element(InputManager.INPUT_MAP_ELEMENT_NAME);
            if (inputMapElement == null)
            {
                InputManager.Instance.LogError($"Error while deserializing {nameof(InputMap)}, could not find XElement with name {InputManager.INPUT_MAP_ELEMENT_NAME}.", InputManager.Instance.gameObject);
                return;
            }

            XAttribute useAltButtonsAttribute = inputMapElement.Attribute("UseAltButtons");
            if (useAltButtonsAttribute != null)
            {
                if (bool.TryParse(useAltButtonsAttribute.Value, out bool useAltButtons))
                {
                    UseAltButtons = useAltButtons;
                }
                else
                {
                    InputManager.Instance.LogWarning($"Could not parse {useAltButtonsAttribute.Value} to a valid bool. Setting it to false.", InputManager.Instance.gameObject);
                    UseAltButtons = false;
                }
            }
            else
            {
                UseAltButtons = false;
            }
            
            foreach (XElement keyBindingElement in inputMapElement.Elements())
            {
                string actionId = keyBindingElement.Name.LocalName;

                XAttribute btnAttribute = keyBindingElement.Attribute(InputManager.BTN_ATTRIBUTE_NAME);

                if (btnAttribute == null)
                {
                    InputManager.Instance.LogError($"Could not get {InputManager.BTN_ATTRIBUTE_NAME} attribute for action {actionId}. Restoring default input mapping.", InputManager.Instance.gameObject);
                    return;
                }
                
                if (!System.Enum.TryParse(btnAttribute.Value, out KeyCode btnKeyCode))
                {
                    InputManager.Instance.LogError($"Could not parse {btnAttribute.Value} to a valid UnityEngine.KeyCode for action {actionId}. Restoring default input mapping.", InputManager.Instance.gameObject);
                    return;
                }

                XAttribute altBtnAttribute = keyBindingElement.Attribute(InputManager.ALT_ATTRIBUTE_NAME);

                if (altBtnAttribute == null)
                {
                    InputManager.Instance.LogError($"Could not get {InputManager.ALT_ATTRIBUTE_NAME} attribute for action {actionId}. Restoring default input mapping.", InputManager.Instance.gameObject);
                    return;
                }
                
                if (!System.Enum.TryParse(altBtnAttribute.Value, out KeyCode altBtnKeyCode))
                {
                    InputManager.Instance.LogError($"Could not parse {altBtnAttribute.Value} to a valid UnityEngine.KeyCode for action {actionId}. Restoring default input mapping.", InputManager.Instance.gameObject);
                    return;
                }

                CreateAction(actionId, new InputMapDatas.KeyBinding(actionId, (btnKeyCode, altBtnKeyCode), defaultMap.GetActionBinding(actionId).UserAssignable));
            }
        }

        /// <summary>
        /// Used to sort input map after potentially generating missing inputs, because the user removed them from data files
        /// or because he's loading an older version of the application. Generated inputs are added at the end of the map, so we want
        /// to sort them based on the default map data.
        /// </summary>
        /// <param name="mapData">Map data to get the actions order from.</param>
        /// <returns>True if sorting has been done successfully, else false.</returns>
        public bool SortActionsBasedOnData(InputMapDatas mapData)
        {
            try
            {
                System.Collections.Generic.List<string> bindingsList = mapData.Bindings.ToList().Select(o => o.ActionId).ToList();
                _map = _map.OrderBy(o => bindingsList.IndexOf(o.Key)).ToDictionary(k => k.Key, v => v.Value);
            }
            catch (System.Exception e)
            {
                InputManager.Instance.LogError($"Error while sorting actions map based on map data : {e.Message}.", InputManager.Instance.gameObject);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if map contains a given action.
        /// </summary>
        /// <param name="actionId">Action Id to look for.</param>
        /// <returns>True if map contains the action, else false.</returns>
        public bool HasAction(string actionId)
        {
            return _map.ContainsKey(actionId);
        }

        /// <summary>
        /// Creates an action, added to the map dictionary.
        /// </summary>
        /// <param name="actionId">Action Id type to create.</param>
        /// <param name="keyBinding">Both action KeyCodes.</param>
        public void CreateAction(string actionId, InputMapDatas.KeyBinding keyBinding)
        {
            UnityEngine.Assertions.Assert.IsFalse(HasAction(actionId), $"Trying to create an already know action Id {actionId}.");
            _map.Add(actionId, keyBinding);
        }

        /// <summary>
        /// Gets both valid KeyCodes for a given action.
        /// </summary>
        /// <param name="actionId">Action Id to get KeyCodes of.</param>
        /// <returns>Tuple containing KeyCodes.</returns>
        public InputMapDatas.KeyBinding GetActionBinding(string actionId)
        {
            UnityEngine.Assertions.Assert.IsTrue(HasAction(actionId), $"Looking for keyCodes of unknown action Id {actionId}.");
            return _map[actionId];
        }

        /// <summary>
        /// Overrides a KeyCode for a given action and resets other actions that were using the same KeyCode.
        /// </summary>
        /// <param name="actionId">Action Id to override KeyCode of.</param>
        /// <param name="keyCode">KeyCode to set.</param>
        /// <param name="alt">Set the base button or the alternate one.</param>
        public void SetActionButton(string actionId, KeyCode keyCode, bool alt)
        {
            UnityEngine.Assertions.Assert.IsTrue(HasAction(actionId), $"Trying to set keyCode of unknown action Id {actionId}.");

            InputMapDatas.KeyBinding newBinding = _map[actionId];

            if (alt)
                newBinding.SetAltButton(keyCode);
            else
                newBinding.SetButton(keyCode);

            string[] keys = _map.Keys.ToArray();
            for (int i = keys.Length - 1; i >= 0; --i)
            {
                if (keys[i] == actionId)
                    continue;

                if (_map[keys[i]].KeyCodes.btn == keyCode)
                    _map[keys[i]].SetKeyCodes(KeyCode.None, _map[keys[i]].KeyCodes.altBtn);

                if (_map[keys[i]].KeyCodes.altBtn == keyCode)
                    _map[keys[i]].SetKeyCodes(_map[keys[i]].KeyCodes.btn, KeyCode.None);
            }

            _map[actionId] = newBinding;
        }
    }
}