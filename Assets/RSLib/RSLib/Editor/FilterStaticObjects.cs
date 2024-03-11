namespace RSLib.Editor
{
	using UnityEngine;
	using UnityEditor;

	sealed class FilterStaticObjectMenu
	{
		[MenuItem("RSLib/Filter Static Objects")]
		public static void RenameSelectedObjects()
		{
			FilterStaticObjectEditor.LaunchFilter();
		}
	}

	public sealed class FilterStaticObjectEditor : EditorWindow
	{
		private StaticEditorFlags _flag;
		private bool _include;

		public static void LaunchFilter()
		{
			GetWindow<FilterStaticObjectEditor>("Filter Static Objects").Show();
		}

        private void FilterSelection(bool include)
		{
            Object[] gameObjects = FindObjectsOfType(typeof(GameObject));
            GameObject[] gameObjectsArray = new GameObject[gameObjects.Length];
			int arrayPointer = 0;

			foreach (Object obj in gameObjects)
			{
				GameObject gameObject = (GameObject)obj;
				StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(gameObject);

				if (((flags & _flag) != 0) != include)
					continue;
				
				gameObjectsArray[arrayPointer] = gameObject;
				arrayPointer += 1;
			}

			Selection.objects = gameObjectsArray;
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			EditorGUILayout.BeginVertical();
			GUILayout.Space(10f);

			EditorGUILayout.LabelField("Flag to filter :", EditorStyles.boldLabel);
            System.Array options = System.Enum.GetValues(typeof(StaticEditorFlags));
            _flag = (StaticEditorFlags)EditorGUILayout.EnumPopup(_flag);

			_include = EditorGUILayout.Toggle("Include", _include);

			GUILayout.Space(10f);

			if (GUILayout.Button("Filter Selection", GUILayout.Height(45f), GUILayout.ExpandWidth(true)))
				FilterSelection(_include);

			EditorGUILayout.EndVertical();
			GUILayout.Space(10f);
			EditorGUILayout.EndHorizontal();

			Repaint();
		}
	}
}