namespace RSLib.Editor
{
	using UnityEngine;
	using UnityEditor;

	public static class GameObjectsGrouperMenu
	{
		private const string SHORTCUT = "%#q";

		[MenuItem("GameObject/Group Objects " + SHORTCUT, true)]
		private static bool CheckSelectionCount()
		{
			return Selection.gameObjects.Length > 1;
		}

		[MenuItem("GameObject/Group Objects " + SHORTCUT)]
		public static void LaunchObjectGrouper()
		{
			GameObjectsGrouperEditor.LaunchGrouper();
		}
	}

    public sealed class GameObjectsGrouperEditor : EditorWindow
	{
		private const string FULL_HIERARCHY_CHANGE = "full object hierarchy change";
		private const string CREATE_UNDO = "Create ";

		private GameObject[] _selection;
		private string _groupName;
		private bool _averagePos;
		private bool _yReset;

		public static void LaunchGrouper()
		{
			GetWindow<GameObjectsGrouperEditor>("Group selection").Show();
		}

		private Vector3 GetSelectionAveragePosition()
		{
			Vector3 averagePosition = Vector3.zero;
			foreach (GameObject child in _selection)
				averagePosition += child.transform.position;

			return averagePosition /= _selection.Length;
		}

        private void GroupSelection()
		{
			if (_selection.Length < 2)
			{
				EditorUtility.DisplayDialog("Grouper warning", "You need to select at least 2 objects to group !", "OK");
				return;
			}
			if (string.IsNullOrEmpty(_groupName))
			{
				EditorUtility.DisplayDialog("Grouper warning", "You must provide a name for the group !", "OK");
				return;
			}

			GameObject groupParent = new GameObject(_groupName);
			groupParent.transform.SetParent(_selection[0].transform.parent);
			groupParent.transform.SetSiblingIndex(_selection[0].transform.GetSiblingIndex());

			if (_averagePos)
            {
                Vector3 averagePos = GetSelectionAveragePosition();
				if (_yReset)
					averagePos.y = 0f;
	
				groupParent.transform.position = averagePos;
			}

			foreach (GameObject child in _selection)
			{
				Undo.RegisterFullObjectHierarchyUndo(child, FULL_HIERARCHY_CHANGE);
				child.transform.SetParent(groupParent.transform);
			}

			Undo.RegisterCreatedObjectUndo(groupParent, CREATE_UNDO + groupParent.name);
			Selection.activeObject = groupParent;
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

			EditorGUILayout.LabelField("Group Name", EditorStyles.boldLabel);
			_groupName = EditorGUILayout.TextField(_groupName);

			GUILayout.Space (10f);
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			_averagePos = EditorGUILayout.Toggle("Average position", _averagePos);
			EditorGUILayout.EndHorizontal();

			if (_averagePos)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10f);
				_yReset = EditorGUILayout.Toggle("Keep Y at 0", _yReset);
				EditorGUILayout.EndHorizontal();
			}

			GUILayout.Space(10f);

			if (GUILayout.Button("Group Selection", GUILayout.Height(45f), GUILayout.ExpandWidth(true)))
				GroupSelection();

			Repaint();
		}
	}
}