namespace Templar.Flags
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Lock Identifier", menuName = "Id/Identifier - Lock")]
    public class LockIdentifier : Identifier
    {
        private const string ID_PREFIX = "Lock";

        public override string Id
        {
            get
            {
                if (string.IsNullOrEmpty(BaseId) && !UseNumbering)
                    return ID_ERROR;

                string id = ID_PREFIX;

                if (!string.IsNullOrEmpty(BaseId))
                    id += "_" + BaseId;

                if (UseNumbering)
                    id += "_" + Number;

                return id;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LockIdentifier))]
    public class LockIdentifierEditor : IdentifierEditor
    {
    }
#endif
}