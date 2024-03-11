namespace RSLib.Editor
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// This class is used to allow game artists to select some gameObjects, and set all meshes' gameObjects in
	/// children as static or non-static, with an option to select if it includes inactive objects or not.
	/// </summary>
	public class MeshesStaticSetter : EditorWindow
	{
		private bool _includeInactive;

        [MenuItem("RSLib/Meshes Static Setter")]
        public static void ShowWindow()
        {
            GetWindow<MeshesStaticSetter>("Meshes static setter");
        }

        private GameObject[] GetObjectsToModify()
		{
			List<MeshRenderer> meshes = new List<MeshRenderer>();

			foreach (GameObject g in Selection.gameObjects)
			{
				MeshRenderer[] childrenMeshes = g.GetComponentsInChildren<MeshRenderer>(_includeInactive);
				meshes.AddRange(childrenMeshes);
			}

			GameObject[] meshesObjects = new GameObject[meshes.Count];
			for (int i = 0; i < meshesObjects.Length; ++i)
				meshesObjects[i] = meshes[i].gameObject;

			return meshesObjects;
		}

        private void OnGUI()
		{
			GUILayout.Label("Select gameObjects and choose an option.", EditorStyles.boldLabel);
			GUILayout.Space(10f);

			_includeInactive = EditorGUILayout.Toggle("Include inactive meshes", _includeInactive);

			GUILayout.Space(10f);

			if (GUILayout.Button("Set children meshes as static", GUILayout.Height(30f)))
			{
				if (Selection.gameObjects.Length == 0)
				{
					Debug.LogWarning("No object selected");
					return;
				}

				GameObject[] meshesObjects = GetObjectsToModify();
				foreach (GameObject meshObject in meshesObjects)
					meshObject.isStatic = true;

				Debug.Log($"Set <color=white>{meshesObjects.Length} meshe(s)</color> as <color=white><b>static</b></color>");
			}

			if (GUILayout.Button("Set children meshes as non-static", GUILayout.Height(30f)))
			{
				if (Selection.gameObjects.Length == 0)
				{
					Debug.LogWarning("No object selected");
					return;
				}

				GameObject[] meshesObjects = GetObjectsToModify();
				foreach (GameObject meshObject in meshesObjects)
					meshObject.isStatic = false;

				Debug.Log ($"Set <color=white>{meshesObjects.Length} meshe(s)</color> as <color=white><b>non-static</b></color>");
			}
		}
	}
}