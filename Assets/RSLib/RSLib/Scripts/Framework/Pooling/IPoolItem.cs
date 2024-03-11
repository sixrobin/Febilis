namespace RSLib.Framework.Pooling
{
    /// <summary>
    /// Any class attached to a pooled gameObject that should have a specific behaviour when getting enabled
    /// from pool can implement this interface to receive the event message.
    /// </summary>
    public interface IPoolItem
    {
        void OnGetFromPool(params object[] args);
    }
}