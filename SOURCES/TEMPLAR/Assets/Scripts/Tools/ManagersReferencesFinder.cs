#if UNITY_EDITOR
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
			GetManagers().All(o =>
			{
				o.DebugFindAllReferences();
				OnManagerHandled(o);
				return true;
			});

			OnAllManagersHandled();
		}

		[MenuItem("Tools/Local Managers/Find Missing References")]
		public static void FindMissingManagersReferences()
		{
			GetManagers().All(o =>
			{
				o.DebugFindMissingReferences();
				OnManagerHandled(o);
				return true;
			});

			OnAllManagersHandled();
		}

		public static void FindAllManagerReferences(IManagerReferencesHandler managerReferencesHandler)
        {
			managerReferencesHandler.DebugFindAllReferences();
			OnManagerHandled(managerReferencesHandler);
			OnAllManagersHandled();
		}

		public static void FindMissingManagerReferences(IManagerReferencesHandler managerReferencesHandler)
		{
			managerReferencesHandler.DebugFindMissingReferences();
			OnManagerHandled(managerReferencesHandler);
			OnAllManagersHandled();
		}
		
		private static System.Collections.Generic.IEnumerable<IManagerReferencesHandler> GetManagers()
		{
			return RSLib.Helpers.FindInstancesOfType<IManagerReferencesHandler>();
		}

		private static void OnManagerHandled(IManagerReferencesHandler managerReferencesHandler)
        {
			// [TODO] This will apply everything possible, including values that we may not want to apply.
			// It's okay since it should not happen on objects such as Managers, but it would be better to allow both ways, apply all or not.

			PrefabUtility.ApplyPrefabInstance(managerReferencesHandler.PrefabInstanceRoot, InteractionMode.UserAction);
			Debug.LogWarning($"Applying {managerReferencesHandler.PrefabInstanceRoot.name} prefab instance", managerReferencesHandler.PrefabInstanceRoot);
		}

		private static void OnAllManagersHandled()
        {
			RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
		}
	}
}
#endif