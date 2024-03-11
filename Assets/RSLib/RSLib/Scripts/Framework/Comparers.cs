namespace RSLib.Framework.Comparers
{
    using System.Collections.Generic;

    public class EnumComparer<T> : IEqualityComparer<T> where T : System.Enum
    {
        public bool Equals(T x, T y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    public class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }

        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public class IntComparer : IEqualityComparer<int>
    {
        bool IEqualityComparer<int>.Equals(int x, int y)
        {
            return x == y;
        }

        int IEqualityComparer<int>.GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
    }

    public class ReversedComparer<T> : IComparer<T> where T : System.IComparable<T>
    {
        public int Compare(T a, T b)
        {
            return b.CompareTo(a);
        }
    }
}
