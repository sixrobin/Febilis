namespace Templar.Unit.Player
{
    using RSLib.Framework.InputSystem;
    using UnityEngine;

    public class PlayerInputController
    {
        public const string HORIZONTAL_KEYBOARD = "Horizontal";
        public const string HORIZONTAL_CONTROLLER = "HorizontalLeftStick";
        public const string VERTICAL_KEYBOARD = "Vertical";
        public const string VERTICAL_CONTROLLER = "VerticalLeftStick";
        public const string JUMP = "Jump";
        public const string ROLL = "Roll";
        public const string ATTACK = "Attack";
        public const string INTERACT = "Interact";
        public const string HEAL = "Heal";

        private MonoBehaviour _coroutinesExecuter;

        private System.Collections.Generic.Dictionary<ButtonCategory, InputGetterHandler> _inputGetters;
        private System.Collections.Generic.Dictionary<ButtonCategory, System.Collections.IEnumerator> _inputStoreCoroutines;
        private System.Collections.Generic.Dictionary<ButtonCategory, float> _inputDelaysByCategory;
        private ButtonCategory _delayedInputs = ButtonCategory.NONE;

        public PlayerInputController(Datas.Unit.Player.PlayerInputDatas inputDatas, MonoBehaviour coroutinesExecuter)
        {
            InputDatas = inputDatas;
            _coroutinesExecuter = coroutinesExecuter;

            Init();
        }

        public delegate bool InputGetterHandler();

        public enum ButtonCategory
        {
            NONE = 0,
            JUMP = 1,
            ROLL = 2,
            ATTACK = 4,
            INTERACT = 8,
            HEAL = 16,
            ANY = JUMP | ROLL | ATTACK | INTERACT | HEAL
        }

        public Datas.Unit.Player.PlayerInputDatas InputDatas { get; private set; }

        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }

        public float CurrentHorizontalDir => Mathf.Sign(Horizontal);
        public float CurrentVerticalDir => Mathf.Sign(Vertical);

        public bool CheckInput(ButtonCategory btnCategory)
        {
            return btnCategory == ButtonCategory.ANY ? _delayedInputs != ButtonCategory.NONE : _delayedInputs.HasFlag(btnCategory);
        }

        public bool CheckJumpInput()
        {
            return InputManager.GetInput(JUMP);
        }

        public bool CheckJumpInputUp()
        {
            return InputManager.GetInputUp(JUMP);
        }

        public void Update()
        {
            Horizontal = Input.GetAxisRaw(HORIZONTAL_KEYBOARD);
            float leftStickHorizontal = Input.GetAxis(HORIZONTAL_CONTROLLER);
            if (leftStickHorizontal * leftStickHorizontal > Manager.SettingsManager.AxisDeadZone.ValueSqr)
                Horizontal = leftStickHorizontal;

            Vertical = Input.GetAxisRaw(VERTICAL_KEYBOARD);
            float leftStickVertical = Input.GetAxis(VERTICAL_CONTROLLER);
            if (leftStickVertical * leftStickVertical > Manager.SettingsManager.AxisDeadZone.ValueSqr)
                Vertical = leftStickVertical;

            foreach (System.Collections.Generic.KeyValuePair<ButtonCategory, InputGetterHandler> input in _inputGetters)
            {
                if (input.Value())
                {
                    ResetDelayedInput(input.Key);
                    _delayedInputs |= input.Key;
                    _inputStoreCoroutines[input.Key] = StoreInputCoroutine(input.Key);
                    _coroutinesExecuter.StartCoroutine(_inputStoreCoroutines[input.Key]);
                }
            }
        }

        public void Reset()
        {
            Horizontal = 0f;
            Vertical = 0f;

            _delayedInputs &= ~ButtonCategory.HEAL;
        }

        public void ResetDelayedInput(ButtonCategory btnCategory)
        {
            if (_inputStoreCoroutines.TryGetValue(btnCategory, out System.Collections.IEnumerator storeCoroutine)
                && storeCoroutine != null)
            {
                _coroutinesExecuter.StopCoroutine(storeCoroutine);
                storeCoroutine = null;
            }

            _delayedInputs &= ~btnCategory;
        }

        private void Init()
        {
            _inputGetters = new System.Collections.Generic.Dictionary<ButtonCategory, InputGetterHandler>(
                new RSLib.Framework.Comparers.EnumComparer<ButtonCategory>())
                {
                    { ButtonCategory.JUMP, () => InputManager.GetInputDown(JUMP) },
                    { ButtonCategory.ROLL, () => InputManager.GetInputDown(ROLL) },
                    { ButtonCategory.ATTACK, () => InputManager.GetInputDown(ATTACK) },
                    { ButtonCategory.INTERACT, () => InputManager.GetInputDown(INTERACT) },
                    { ButtonCategory.HEAL, () => InputManager.GetInputDown(HEAL) }
                };

            _inputDelaysByCategory = new System.Collections.Generic.Dictionary<ButtonCategory, float>(
                new RSLib.Framework.Comparers.EnumComparer<ButtonCategory>())
                {
                    { ButtonCategory.JUMP, InputDatas.JumpInputDelay },
                    { ButtonCategory.ROLL, InputDatas.RollInputDelay },
                    { ButtonCategory.ATTACK, InputDatas.AttackInputDelay }
                };

            _inputStoreCoroutines = new System.Collections.Generic.Dictionary<ButtonCategory, System.Collections.IEnumerator>(
                new RSLib.Framework.Comparers.EnumComparer<ButtonCategory>())
                {
                    { ButtonCategory.JUMP, null },
                    { ButtonCategory.ROLL, null },
                    { ButtonCategory.ATTACK, null }
                };
        }

        private System.Collections.IEnumerator StoreInputCoroutine(ButtonCategory btnCategory)
        {
            if (!_inputDelaysByCategory.ContainsKey(btnCategory))
                yield break;

            _delayedInputs &= btnCategory;
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_inputDelaysByCategory[btnCategory]);
            _delayedInputs &= ~btnCategory;
        }
    }
}