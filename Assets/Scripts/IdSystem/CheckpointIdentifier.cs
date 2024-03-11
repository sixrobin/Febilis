namespace Templar.Flags
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(fileName = "New Checkpoint Identifier", menuName = "Id/Identifier - Checkpoint")]
    public class CheckpointIdentifier : Identifier
    {
        private const string ID_PREFIX = "Checkpoint";

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
    [CustomEditor(typeof(CheckpointIdentifier))]
    public class CheckpointIdentifierEditor : IdentifierEditor
    {
    }
#endif
}