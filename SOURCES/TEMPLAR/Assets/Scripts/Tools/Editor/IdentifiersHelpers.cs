namespace Templar.Tools
{
    using System.Linq;
    using Templar.Flags;
    using UnityEngine;
    using UnityEditor;

    public class IdentifiersHelpers
    {
        private class IdCounter
        {
            public IdCounter(Identifier id)
            {
                Assets = new System.Collections.Generic.List<Identifier>() { id };
            }

            public System.Collections.Generic.List<Identifier> Assets { get; set; }
        }

        private const string IDENTIFIERS_ASSETS_ROOT_PATH = "Assets/Datafiles/Ids";

		[MenuItem("Tools/Identifiers/Check Identifiers Uniqueness")]
		private static void CheckIdentifiersUniqueness()
		{
            System.Collections.Generic.Dictionary<string, IdCounter> idsCounter
                = new System.Collections.Generic.Dictionary<string, IdCounter>();

            foreach (string subDirectory in RSLib.EditorUtilities.AssetDatabaseUtilities.GetSubFoldersRecursively(IDENTIFIERS_ASSETS_ROOT_PATH, true))
                foreach (Identifier id in RSLib.EditorUtilities.AssetDatabaseUtilities.GetAllAssetsAtFolderPath<Identifier>(subDirectory))
                    if (idsCounter.ContainsKey(id.Id))
                        idsCounter[id.Id].Assets.Add(id);
                    else
                        idsCounter.Add(id.Id, new IdCounter(id));

            if (idsCounter.Where(o => o.Value.Assets.Count > 1).Count() == 0)
            {
                Debug.Log($"No duplicate Id has been found in {IDENTIFIERS_ASSETS_ROOT_PATH}.");
                return;
            }

            foreach (System.Collections.Generic.KeyValuePair<string, IdCounter> idCounter in idsCounter.Where(o => o.Value.Assets.Count > 1))
            {
                Debug.LogWarning($"Id <b>{idCounter.Key}</b> has been found {idCounter.Value.Assets.Count} times in the following assets <i>(click on each log to select the asset)</i> :");
                idCounter.Value.Assets.ForEach(o => Debug.LogWarning($"- {o.name}", o));
            }
        }

        [MenuItem("Tools/Identifiers/Check Identifiers References")]
        private static void CheckIdentifiersReferences()
        {
            bool anyMissingReference = false;

            System.Collections.Generic.IEnumerable<MonoBehaviour> identifiableMonos = Object.FindObjectsOfType<MonoBehaviour>().Where(o => o is IIdentifiable);
            foreach (MonoBehaviour identifiableMono in identifiableMonos)
            {
                IIdentifiable identifiable = identifiableMono as IIdentifiable;
                if (identifiable.Identifier == null)
                {
                    anyMissingReference = true;
                    Debug.LogWarning($"{identifiableMono.GetType().Name} Identifiable on gameObject {identifiableMono.transform.name} Identifier reference is null.", identifiableMono.gameObject);
                }
            }

            if (!anyMissingReference)
                Debug.Log($"No Identifier reference is missing in the scene.");
        }
    }
}