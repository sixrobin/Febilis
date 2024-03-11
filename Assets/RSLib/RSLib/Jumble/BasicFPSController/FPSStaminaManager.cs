namespace RSLib.Jumble.FPSController
{
    using UnityEngine;

    /// <summary>
    /// Stamina manager of the FPS controller. Does not inherit from MonoBehaviour and has an Update
    /// method that should be call in some MonoBehaviour Update.
    /// Must be constructed to be initialized with the wanted settings.
    /// </summary>
    public class FPSStaminaManager
    {
        private float _fullDuration;
        private float _recoverDelay;
        private float _reloadDuration;

        private bool _recovering;
        private float _recoverTimer;

        private float _currentCharge;
        public float CurrentCharge
        {
            get => _currentCharge;
            private set => _currentCharge = Mathf.Clamp01(value);
        }

        public delegate void OutOfStaminaEventHandler();
        public event OutOfStaminaEventHandler OutOfStamina;

        public bool IsEmpty => CurrentCharge == 0f;

        public FPSStaminaManager()
        {
            SetSettings(5f, 2f, 15f);
            CurrentCharge = 1f;
        }

        public FPSStaminaManager(float fullDuration, float recoverDelay, float reloadDuration)
        {
            SetSettings(fullDuration, recoverDelay, reloadDuration);
            CurrentCharge = 1f;
        }

        /// <summary>
        /// Overrides the stamina settings at runtime.
        /// </summary>
        /// <param name="fullDuration">Full stamina charge duration.</param>
        /// <param name="recoverDelay">Delay to wait when stamina is empty.</param>
        /// <param name="reloadDuration">Full reload duration from empty to full.</param>
        public void SetSettings(float fullDuration, float recoverDelay, float reloadDuration)
        {
            _fullDuration = fullDuration;
            _recoverDelay = recoverDelay;
            _reloadDuration = reloadDuration;
        }

        /// <summary>
        /// Updates the stamina charge according to the stamina current state (reloading, empty, etc.).
        /// Must be called inside a MonoBehaviour Update method.
        /// </summary>
        /// <param name="consuming">Is the stamina being consumed.</param>
        public void Update(bool consuming)
        {
            if (_recovering)
            {
                _recoverTimer += Time.deltaTime;
                if (_recoverTimer > _recoverDelay)
                {
                    _recoverTimer = 0;
                    _recovering = false;
                }

                return;
            }

            if (consuming)
            {
                CurrentCharge -= Time.deltaTime / _fullDuration;
                if (IsEmpty)
                {
                    OutOfStamina?.Invoke();
                    _recovering = true;
                }
            }
            else
            {
                CurrentCharge += Time.deltaTime / _reloadDuration;
            }
        }
    }
}