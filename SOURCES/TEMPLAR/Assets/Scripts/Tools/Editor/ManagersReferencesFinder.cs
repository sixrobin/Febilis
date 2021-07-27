namespace Templar.Tools
{
	using System.Linq;
	using UnityEngine;
	using UnityEditor;

	public sealed class ManagersReferencesFinder
	{
		[MenuItem("Tools/Local Managers/Find All References")]
		public static void FindAllManagersReferences()
		{
			Object.FindObjectsOfType<MonoBehaviour>()
				.OfType<IManagerReferencesHandler>()
				.ToList()
				.ForEach(o => o.DebugFindAllReferences());
		}

		[MenuItem("Tools/Local Managers/Find Missing References")]
		public static void FindMissingManagersReferences()
		{
			Object.FindObjectsOfType<MonoBehaviour>()
				.OfType<IManagerReferencesHandler>()
				.ToList()
				.ForEach(o => o.DebugFindMissingReferences());
		}
	}
}