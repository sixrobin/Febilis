namespace Templar.Flags
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    [CreateAssetMenu(fileName = "New Secret Wall Identifier", menuName = "Id/Identifier - Secret Wall")]
    public class SecretWallIdentifier : Identifier
    {
        private const string ID_PREFIX = "SecretWall";

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
    [CustomEditor(typeof(SecretWallIdentifier))]
    public class SecretWallIdentifierEditor : IdentifierEditor
    {
    }
#endif
}