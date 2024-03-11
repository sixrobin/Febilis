namespace RSLib.Editor
{
	using UnityEngine;
	using UnityEditor;
    using Extensions;

    public static class LayerRecursiveSetterMenu
	{
		[MenuItem("RSLib/Layer Recursive Setter", true)]
		private static bool CheckSelectionCount()
		{
			return Selection.gameObjects.Length > 0;
		}

		[MenuItem("RSLib/Layer Recursive Setter")]
		public static void LaunchLayerSetter()
		{
			LayerRecursiveSetterEditor.LaunchSetter();
		}
	}

    public sealed class LayerRecursiveSetterEditor : EditorWindow
	{
		private GameObject[] _selection;
		private string _layerName;

		public static void LaunchSetter()
		{
			EditorWindow window = GetWindow<LayerRecursiveSetterEditor>("Set objet's children layer");
			window.Show();
		}

		private void OnGUI()
		{
			_selection = Selection.gameObjects;

			EditorGUILayout.LabelField("Layer Name", EditorStyles.boldLabel);
			_layerName = EditorGUILayout.TextField(_layerName);

			if (GUILayout.Button("Set objet's children layer recursively", GUILayout.Height(45f), GUILayout.ExpandWidth(true)))
                foreach (GameObject selected in _selection)
					selected.SetChildrenLayers(LayerMask.NameToLayer(_layerName));

			Repaint();
		}
	}
}