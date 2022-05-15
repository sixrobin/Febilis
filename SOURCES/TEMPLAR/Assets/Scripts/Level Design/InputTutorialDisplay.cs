namespace Templar
{
    using System.Xml.Linq;
    using UnityEngine;
    
    public class InputTutorialDisplay : MonoBehaviour
    {
        [SerializeField] private DisplayType _displayType = DisplayType.START;
        [SerializeField] private ValidationType _validationType = ValidationType.TRIGGER_ENTER;
        [SerializeField] private RSLib.Framework.OptionalBoxCollider2D _validatingTrigger = new RSLib.Framework.OptionalBoxCollider2D(null, false);
        
        [Header("LOCALIZATION")]
        [SerializeField] private TMPro.TextMeshProUGUI _text = null;
        [SerializeField] private string _localizationSuffix = string.Empty;
        [SerializeField] private string _actionId = string.Empty;
        
        private bool _validated;
        
        public enum ValidationType
        {
            TRIGGER_ENTER,
            MOVEMENT,
            ATTACK,
            ROLL,
            EFFECTOR,
            HEAL,
            INTERACTION,
            INVENTORY,
            JUMP
        }

        public enum DisplayType
        {
            START,
            DAMAGED,
            ITEM_PICKUP
        }
        
        public static System.Collections.Generic.HashSet<ValidationType> ValidatedTypes { get; } = new System.Collections.Generic.HashSet<ValidationType>();

        public static XElement Save()
        {
            XElement validatedInputsElement = new XElement("ValidatedInputs");

            Manager.SaveManager.Instance.Log($"Saving ValidatedInputs: {string.Join(", ", ValidatedTypes)}");

            foreach (ValidationType validatedInput in ValidatedTypes)
                validatedInputsElement.Add(new XElement(validatedInput.ToString()));

            return validatedInputsElement;
        }

        public static void Load(XElement validatedInputsElement)
        {
            foreach (XElement validatedInputElement in validatedInputsElement.Elements())
                if (System.Enum.TryParse(validatedInputElement.Name.LocalName, out ValidationType validationType))
                    ValidatedTypes.Add(validationType);
        }

        public void Display(bool show)
        {
            if (show && _validated)
                return;

            if (show)
                Localize();

            gameObject.SetActive(show);
        }
        
        private void RaiseValidationEvent(ValidationType validationType)
        {
            if (validationType == _validationType && !_validated)
                OnInputValidated();
        }

        private void OnInputValidated()
        {
            _validated = true;
            ValidatedTypes.Add(_validationType);
            Display(false);
        }
        
        private System.Collections.IEnumerator Init()
        {
            Unit.Player.PlayerController playerCtrl = Manager.GameManager.PlayerCtrl;
            yield return new WaitUntil(() => playerCtrl.Initialized);

            if (ValidatedTypes.Contains(_validationType))
            {
                OnInputValidated();
                yield break;
            }

            switch (_displayType)
            {
                case DisplayType.START:
                    Display(true);
                    break;
                
                case DisplayType.DAMAGED:
                    Display(false);
                    playerCtrl.HealthCtrl.UnitHealthChanged += (args) =>
                    {
                        if (args.IsLoss)
                            Display(true);
                    };
                    break;
                
                case DisplayType.ITEM_PICKUP:
                    Display(false);
                    UI.ItemPickupPanel.PickupPanelDisplayed += () => { Display(true); };
                    break;
            }
            
            switch (_validationType)
            {
                case ValidationType.TRIGGER_ENTER:
                case ValidationType.ROLL when _validatingTrigger.Enabled:
                case ValidationType.JUMP when _validatingTrigger.Enabled:
                    UnityEngine.Assertions.Assert.IsTrue(
                        _validatingTrigger.Enabled && _validatingTrigger.Value != null,
                        $"{nameof(InputTutorialDisplay)} validation type is set as {nameof(ValidationType.TRIGGER_ENTER)} but no collider is referenced and/or enabled!");

                    if (!_validatingTrigger.Value.TryGetComponent(out RSLib.Physics2DEventReceiver physics2DEventReceiver))
                    {
                        CProLogger.LogError(this, $"{nameof(InputTutorialDisplay)} collider has no {nameof(RSLib.Physics2DEventReceiver)}!");
                        yield break;
                    }
                    
                    physics2DEventReceiver.TriggerEntered += _ => RaiseValidationEvent(_validationType);
                    break;
                
                case ValidationType.ROLL when !_validatingTrigger.Enabled:
                    playerCtrl.RollCtrl.Rolled += () => RaiseValidationEvent(ValidationType.ROLL);
                    break;
                
                case ValidationType.JUMP when !_validatingTrigger.Enabled:
                    playerCtrl.JumpCtrl.Jumped += () => RaiseValidationEvent(ValidationType.JUMP);
                    break;
             
                case ValidationType.MOVEMENT:
                    playerCtrl.Moved += (newPosition) => RaiseValidationEvent(ValidationType.MOVEMENT);
                    break;
                
                case ValidationType.HEAL:
                    playerCtrl.HealthCtrl.UnitHealthChanged += (args) =>
                    {
                        if (!args.IsLoss)
                            RaiseValidationEvent(ValidationType.HEAL);
                    };
                    break;
                
                case ValidationType.INVENTORY:
                    Manager.GameManager.InventoryView.DisplayChanged += (displayed) =>
                    {
                        if (displayed)
                            RaiseValidationEvent(ValidationType.INVENTORY);
                    };
                    break;

                case ValidationType.ATTACK:
                    playerCtrl.AttackCtrl.AttackHitTriggered += () => RaiseValidationEvent(ValidationType.ATTACK);
                    break;
                
                case ValidationType.EFFECTOR:
                    playerCtrl.CollisionsCtrl.EffectorDown += (effector) => RaiseValidationEvent(ValidationType.EFFECTOR);
                    break;
                
                case ValidationType.INTERACTION:
                    playerCtrl.Interacter.Interacted += (interactable) => RaiseValidationEvent(ValidationType.INTERACTION);
                    break;
            }
            
            _text.enabled = true; // Hidden on awake, reenable on start.
        }

        private void Awake()
        {
            ValidatedTypes.Clear();
            _text.enabled = false; // Hide on awake, then enable on start.

            Localizer.LanguageChanged += Localize;
        }

        private void Localize()
        {
            if (!string.IsNullOrEmpty(_actionId))
            {
                System.Collections.Generic.Dictionary<string, RSLib.Framework.InputSystem.InputMapDatas.KeyBinding> inputMapCopy = RSLib.Framework.InputSystem.InputManager.GetMapCopy();
                _text.text = inputMapCopy.TryGetValue(_actionId, out RSLib.Framework.InputSystem.InputMapDatas.KeyBinding binding)
                    ? string.Format(Localizer.Get($"{Localization.InputTutorial.PREFIX}{_localizationSuffix}"), binding.KeyCodes.btn)
                    : Localizer.Get($"{Localization.InputTutorial.PREFIX}{_localizationSuffix}");
            }
            else
            {
                _text.text = Localizer.Get($"{Localization.InputTutorial.PREFIX}{_localizationSuffix}");
            }
        }

        private void Start()
        {
            StartCoroutine(Init());
        }

        private void OnDestroy()
        {
            Localizer.LanguageChanged -= Localize;
        }
    }
}
