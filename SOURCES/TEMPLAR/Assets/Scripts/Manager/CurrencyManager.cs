namespace Templar.Manager
{
    public class CurrencyManager : RSLib.Framework.ConsoleProSingleton<CurrencyManager>
    {
        public delegate void CurrencyChangedEventHandler(ulong previous, ulong current);
        public event CurrencyChangedEventHandler CurrencyChanged;

        private static ulong s_currency;
        public static ulong Currency
        {
            get => s_currency;
            private set
            {
                Instance.CurrencyChanged?.Invoke(s_currency, value);
                s_currency = value;
            }
        }

        public static void GetCurrency(ulong value)
        {
            Currency += value;
        }

        public static void SpendCurrency(ulong value)
        {
            UnityEngine.Assertions.Assert.IsTrue(Currency >= value, $"Trying to spend {value} currency with only {Currency} left.");
            Currency -= value;
        }
    }
}