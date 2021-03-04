using RSLib.Framework.InputSystem;
using UnityEngine;

public class TemplarInputController
{
    public const string HORIZONTAL_KEYBOARD = "Horizontal";
    public const string HORIZONTAL_CONTROLLER = "HorizontalLeftStick";
    public const string JUMP = "Jump";
    public const string ROLL = "Roll";
    public const string ATTACK = "Attack";

    private TemplarInputDatas _inputDatas;
    private MonoBehaviour _coroutinesExecuter;

    public delegate bool InputGetterHandler();

    private System.Collections.Generic.Dictionary<ButtonCategory, InputGetterHandler> _inputGetters;
    private System.Collections.Generic.Dictionary<ButtonCategory, System.Collections.IEnumerator> _inputStoreCoroutines;
    private System.Collections.Generic.Dictionary<ButtonCategory, float> _inputDelaysByCategory;
    private ButtonCategory _delayedInputs = ButtonCategory.NONE;

    public TemplarInputController(TemplarInputDatas inputDatas, MonoBehaviour coroutinesExecuter)
    {
        _inputDatas = inputDatas;
        _coroutinesExecuter = coroutinesExecuter;
        Init();
    }

    public enum ButtonCategory
    {
        NONE = 0,
        JUMP = 1,
        ROLL = 2,
        ATTACK = 4,
        ANY = JUMP | ROLL | ATTACK
    }

    public float Horizontal { get; private set; }

    public float CurrentInputDir => Mathf.Sign(Horizontal);

    public bool CheckInput(ButtonCategory btnCategory)
    {
        return btnCategory == ButtonCategory.ANY
            ? _delayedInputs != ButtonCategory.NONE
            : (_delayedInputs & btnCategory) == btnCategory;
    }

    public void Update()
    {
        Horizontal = Input.GetAxisRaw(HORIZONTAL_KEYBOARD);
        float leftStickHorizontal = Input.GetAxis(HORIZONTAL_CONTROLLER);
        if (leftStickHorizontal * leftStickHorizontal > _inputDatas.LeftJoystickDeadZoneSqr)
            Horizontal = leftStickHorizontal;

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
    }

    public void ResetDelayedInput(ButtonCategory btnCategory)
    {
        if (_inputStoreCoroutines[btnCategory] != null)
        {
            _coroutinesExecuter.StopCoroutine(_inputStoreCoroutines[btnCategory]);
            _inputStoreCoroutines[btnCategory] = null;
        }

        _delayedInputs ^= btnCategory;
    }

    private void Init()
    {
        _inputGetters = new System.Collections.Generic.Dictionary<ButtonCategory, InputGetterHandler>(
             new RSLib.Framework.Comparers.EnumComparer<ButtonCategory>())
        {
            { ButtonCategory.JUMP, () => InputManager.GetInputDown(InputAction.JUMP) },
            { ButtonCategory.ROLL, () => InputManager.GetInputDown(InputAction.ROLL) },
            { ButtonCategory.ATTACK, () => InputManager.GetInputDown(InputAction.ATTACK) },

            //{ ButtonCategory.JUMP, () => Input.GetButtonDown(JUMP) },
            //{ ButtonCategory.ROLL, () => Input.GetButtonDown(ROLL) },
            //{ ButtonCategory.ATTACK, () => Input.GetButtonDown(ATTACK) }
        };

        _inputDelaysByCategory = new System.Collections.Generic.Dictionary<ButtonCategory, float>(
            new RSLib.Framework.Comparers.EnumComparer<ButtonCategory>())
        {
            { ButtonCategory.JUMP, _inputDatas.JumpInputDelay },
            { ButtonCategory.ROLL, _inputDatas.RollInputDelay },
            { ButtonCategory.ATTACK, _inputDatas.AttackInputDelay }
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
        UnityEngine.Assertions.Assert.IsTrue(_inputDelaysByCategory.ContainsKey(btnCategory), $"Storing {btnCategory} input with unknown delay.");
        yield return RSLib.Yield.SharedYields.WaitForSeconds(_inputDelaysByCategory[btnCategory]);
        _delayedInputs ^= btnCategory;
    }
}