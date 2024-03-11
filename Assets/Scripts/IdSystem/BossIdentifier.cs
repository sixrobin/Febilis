namespace Templar.Flags
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Boss Identifier", menuName = "Id/Identifier - Boss")]
    public class BossIdentifier : Identifier
    {
        private const string ID_PREFIX = "Boss";

        public override string Id
        {
            get
            {
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
    [CustomEditor(typeof(BossIdentifier))]
    public class BossIdentifierEditor : IdentifierEditor
    {
    }
#endif
}