namespace RSLib.Editor
{
	using UnityEngine;
	using UnityEditor;

    public static class GameObjectsRenamerMenu
	{
		private const string SHORTCUT = "%&r";

		[MenuItem("GameObject/Rename Objects " + SHORTCUT, true)]
		private static bool CheckIfAtLeastOneObjectIsSelected()
		{
			return Selection.gameObjects.Length > 0;
		}

		[MenuItem("GameObject/Rename Objects " + SHORTCUT)]
		public static void RenameSelectedObjects()
		{
			GameObjectsRenamerEditor.LaunchRenamer();
		}
	}

    public sealed class GameObjectsRenamerEditor : EditorWindow
	{
		private const string UNDERSCORE = "_";

		private GameObject[] _selection;
		private string _prefix;
		private string _nameBody;
		private string _suffix;
        private bool _numbering;

		public static void LaunchRenamer()
		{
			GetWindow<GameObjectsRenamerEditor>("Rename Objects").Show();
		}

        private void RenameSelection()
		{
			if (_selection.Length == 0)
			{
				EditorUtility.DisplayDialog("Renamer warning", "You must select at least 1 object to rename !", "OK");
				return;
			}
			if (string.IsNullOrEmpty(_nameBody))
			{
				EditorUtility.DisplayDialog("Renamer warning", "You must provide at least the name to set !", "OK");
				return;
			}

			System.Array.Sort(_selection, (a, b) => a.name.CompareTo(b.name));

			for (int i = 0; i < _selection.Length; i++)
			{
				string newName = string.Empty;

				if (!string.IsNullOrEmpty(_prefix))
					newName += _prefix;

				newName += (newName.Length != 0 ? UNDERSCORE : string.Empty) + _nameBody;

				if (!string.IsNullOrEmpty(_suffix))
					newName += UNDERSCORE + _suffix;

				if (_numbering)
					newName += UNDERSCORE + i.ToString("000");

				_selection[i].name = newName;
			}
		}

        private void OnGUI()
		{
			_selection = Selection.gameObjects;

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			EditorGUILayout.BeginVertical();
			GUILayout.Space(10f);

			EditorGUILayout.LabelField("SELECTED : " + _selection.Length, EditorStyles.boldLabel);

			GUILayout.Space(10f);
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUILayout.Space(5f);

			_prefix = EditorGUILayout.TextField("Prefix : ", _prefix, EditorStyles.miniTextField, GUILayout.ExpandWidth(true));
			_nameBody = EditorGUILayout.TextField("Name : ", _nameBody, EditorStyles.miniTextField, GUILayout.ExpandWidth(true));
			_suffix = EditorGUILayout.TextField("Suffix : ", _suffix, EditorStyles.miniTextField, GUILayout.ExpandWidth(true));
			_numbering = EditorGUILayout.Toggle("Add numbering ?", _numbering);

			GUILayout.Space (5);
			EditorGUILayout.EndVertical ();
			GUILayout.Space (10);

			if (GUILayout.Button("Rename Selected GameObjects", GUILayout.Height(45f), GUILayout.ExpandWidth(true)))
				RenameSelection();

			EditorGUILayout.EndVertical();
			GUILayout.Space(10f);
			EditorGUILayout.EndHorizontal();

			Repaint();
		}
	}
}