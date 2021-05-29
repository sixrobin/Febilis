namespace Templar.Manager
{
    public class CurrencyManager : RSLib.Framework.ConsoleProSingleton<CurrencyManager>
    {
        public delegate void CurrencyChangedEventHandler(long previous, long current);
        public event CurrencyChangedEventHandler CurrencyChanged;

        private static long s_currency;
        public static long Currency
        {
            get => s_currency;
            private set
            {
                Instance.CurrencyChanged?.Invoke(s_currency, value);
                s_currency = value;
            }
        }

        public static void LoadCurrency(long value)
        {
            // Not using the property to avoid triggering change event.
            s_currency = value;
        }

        public static void GetCurrency(long value)
        {
            Currency += value;
        }

        public static void SpendCurrency(long value)
        {
            UnityEngine.Assertions.Assert.IsTrue(Currency >= value, $"Trying to spend {value} currency with only {Currency} left.");
            Currency -= value;
        }

        protected override void Awake()
        {
            base.Awake();

            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("GoldSet", "Sets the gold value.", (value) =>
            {
                if (value < 0)
                    RSLib.Debug.Console.DebugConsole.LogExternalError("Cannot set gold value to a negative value, clamping to 0.");

                Currency = UnityEngine.Mathf.Max(0, value);
            }));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("GoldAdd", "Adds value to owned gold.", (value) => GetCurrency(value)));
            RSLib.Debug.Console.DebugConsole.OverrideCommand(new RSLib.Debug.Console.Command<int>("GoldRemove", "Removes value to owned gold.", (value) => SpendCurrency(value)));
        }
    }
}