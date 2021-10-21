namespace Templar.Flags
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Lever Identifier", menuName = "Id/Identifier - Lever")]
    public class LeverIdentifier : Identifier
    {
        private const string ID_PREFIX = "Lever";

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
    [CustomEditor(typeof(LeverIdentifier))]
    public class LeverIdentifierEditor : IdentifierEditor
    {
    }
#endif
}