namespace RSLib.Framework.FSM
{
    public struct FSMTransitionComparer : System.Collections.Generic.IEqualityComparer<FSMTransition>
    {
        public bool Equals(FSMTransition x, FSMTransition y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(FSMTransition obj)
        {
            return obj.GetHashCode();
        }
    }
}