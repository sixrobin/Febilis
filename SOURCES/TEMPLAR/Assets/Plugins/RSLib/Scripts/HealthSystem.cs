namespace RSLib
{
    using RSLib.Maths;

    /// <summary>
    /// Class used to manage a health system. Every living unit can have an instance on this class and listen to the Killed event to be notified when dead.
    /// Methods of this class should be accessed by some methods implemented by an interface (something like ILivingUnit) so that there can be a clean
    /// way to handle additional conditions, heal or damage sources, etc.
    /// </summary>
    public class HealthSystem
    {
        public class HealthChangedEventArgs : System.EventArgs
        {
            public HealthChangedEventArgs(int previous, int current)
            {
                Previous = previous;
                Current = current;
            }

            public HealthChangedEventArgs(HealthChangedEventArgs template)
            {
                Previous = template.Previous;
                Current = template.Current;
            }

            public int Previous { get; private set; }
            public int Current { get; private set; }

            public bool IsLoss => Previous < Current;
        }


        public HealthSystem(int initHealth)
        {
            MaxHealth = initHealth;
            CurrentHealth = initHealth;
        }

        public delegate void HealthChangedEventHandler(HealthChangedEventArgs args);
        public delegate void KilledEventHandler();

        public event HealthChangedEventHandler HealthChanged;
        public event KilledEventHandler Killed;

        private int _currentHealth;
        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                int previousHealth = _currentHealth;
                _currentHealth = value.Clamp(0, MaxHealth);

                if (IsDead)
                    Killed?.Invoke();
                else if (previousHealth != _currentHealth)
                    HealthChanged?.Invoke(new HealthChangedEventArgs(previousHealth, _currentHealth));
            }
        }

        /// <summary>Current health percentage as a value from 0 to 1.</summary>
        public float HealthPercentage => (float)CurrentHealth / MaxHealth;

        public bool IsDead => CurrentHealth == 0;

        public int MaxHealth { get; private set; }

        /// <summary>Instantly changes the maximum health. Health is reduced if new maximum health is less than health value.</summary>
        /// <param name="newValue">New maximum health value.</param>
        /// <param name="increaseHealth">Does health also increase if new maximum health is higher than its previous value.</param>
        public void ChangeMaxHealth(int newValue, bool increaseHealth = true)
        {
            int previousMaxHealth = MaxHealth;
            MaxHealth = newValue;

            if (MaxHealth > previousMaxHealth && increaseHealth)
                CurrentHealth += MaxHealth - previousMaxHealth;
            else if (MaxHealth < CurrentHealth)
                CurrentHealth = MaxHealth;
        }

        /// <summary>Removes a given amount of health points.</summary>
        /// <param name="amount">Amount to remove.</param>
        public void Damage(int amount)
        {
            CurrentHealth -= amount;
        }

        /// <summary>Restores a given amount of health points.</summary>
        /// <param name="amount">Amount to restore.</param>
        /// <param name="ignoreIfDead">If true, heal is not applied if current health is equal to 0.</param>
        public void Heal(int amount, bool ignoreIfDead = true)
        {
            if (IsDead && ignoreIfDead)
                return;

            CurrentHealth += amount;
        }

        /// <summary>Sets health value to maximum health value.</summary>
        /// <param name="ignoreIfDead">If true, heal is not applied if current health is equal to 0.</param>
        public void HealFull(bool ignoreIfDead = true)
        {
            if (IsDead && ignoreIfDead)
                return;

            CurrentHealth = MaxHealth;
        }

        /// <summary>Sets health value to 0, and then kills the unit and triggers the Killed event.</summary>
        public void Kill()
        {
            if (IsDead)
            {
                UnityEngine.Debug.LogWarning("LivingUnit.Kill() WARNING: Can not kill an already dead unit, aborting.");
                return;
            }

            CurrentHealth = 0;
        }
    }
}