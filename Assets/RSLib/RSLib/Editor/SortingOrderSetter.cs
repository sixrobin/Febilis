namespace RSLib.Editor
{
	using UnityEngine;
	using UnityEditor;
    using System.Linq;

    public static class SpriteOrderingDataSetterMenu
	{
		[MenuItem("RSLib/Set Sprites Ordering Data", true)]
		private static bool CheckIfAtLeastOneObjectIsSelected()
		{
			return Selection.gameObjects.Length > 0;
		}

		[MenuItem("RSLib/Set Sprites Ordering Data")]
		public static void SetSelectedObjectsSortingOrder()
		{
			SpriteOrderingDataSetterEditor.LaunchSetter();
		}
	}

	public sealed class SpriteOrderingDataSetterEditor : EditorWindow
	{
		private GameObject[] _selection;
		private int _order;
		private int _offset;
		private string _layerName;
		private bool _orderIncludeInactive;
		private bool _offsetIncludeInactive;
		private bool _layerIncludeInactive;

		public static void LaunchSetter()
		{
			GetWindow<SpriteOrderingDataSetterEditor>("Set Sprites Ordering Data").Show();
		}

		private static System.Collections.Generic.IEnumerable<SpriteRenderer> GetAllSpriteRenderersRecursively(Transform[] parents, bool includeInactive)
        {
			System.Collections.Generic.List<SpriteRenderer> spriteRenderers = new System.Collections.Generic.List<SpriteRenderer>();
			for (int i = 0; i < parents.Length; ++i)
				spriteRenderers.AddRange(parents[i].GetComponentsInChildren<SpriteRenderer>(includeInactive));

			return spriteRenderers;
		}

		private void SetSortingOrder(int order, bool includeInactive)
		{
			if (_selection.Length == 0)
			{
				EditorUtility.DisplayDialog("Order setter warning", "You must select at least 1 object to set order of it and its children !", "OK");
				return;
			}

            System.Collections.Generic.IEnumerable<SpriteRenderer> spriteRenderers = GetAllSpriteRenderersRecursively(_selection.Select(o => o.transform).ToArray(), includeInactive);
			foreach (SpriteRenderer spriteRenderer in spriteRenderers)
				spriteRenderer.sortingOrder = order;

			Debug.Log($"Set the order of {spriteRenderers.Count()} sprite renderer(s) to {order}.");
		}

		private void OffsetSortingOrder(int offset, bool includeInactive)
		{
			if (_selection.Length == 0)
			{
				EditorUtility.DisplayDialog("Order setter warning", "You must select at least 1 object to offset order of it and its children !", "OK");
				return;
			}

			System.Collections.Generic.IEnumerable<SpriteRenderer> spriteRenderers = GetAllSpriteRenderersRecursively(_selection.Select(o => o.transform).ToArray(), includeInactive);
			foreach (SpriteRenderer spriteRenderer in spriteRenderers)
				spriteRenderer.sortingOrder += offset;

			Debug.Log($"Offset the order of {spriteRenderers.Count()} sprite renderer(s) by {offset}.");
		}

		private void SetSortingLayer(string layerName, bool includeInactive)
		{
			if (_selection.Length == 0)
			{
				EditorUtility.DisplayDialog("Order setter warning", "You must select at least 1 object to set layer of it and its children !", "OK");
				return;
			}

			System.Collections.Generic.IEnumerable<SpriteRenderer> spriteRenderers = GetAllSpriteRenderersRecursively(_selection.Select(o => o.transform).ToArray(), includeInactive);
			foreach (SpriteRenderer spriteRenderer in spriteRenderers)
				spriteRenderer.sortingLayerName = layerName;

			Debug.Log($"Set the layer of {spriteRenderers.Count()} sprite renderer(s) to {layerName}.");
		}

		private void OnGUI()
		{
			_selection = Selection.gameObjects;

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			EditorGUILayout.BeginVertical();
			GUILayout.Space(10f);

			EditorGUILayout.LabelField($"Selected GameObject(s) : {_selection.Length}.", EditorStyles.boldLabel);


			// Set Order.

			GUILayout.Space(10f);
			
			EditorGUILayout.LabelField("SET ORDER", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUILayout.Space(5f);

			_order = EditorGUILayout.IntField("Order:", _order, EditorStyles.miniTextField, GUILayout.ExpandWidth(true));
			_orderIncludeInactive = EditorGUILayout.Toggle("Include inactive ?", _orderIncludeInactive);

			GUILayout.Space(5f);

			if (GUILayout.Button("Set Orders Recursively", GUILayout.Height(45f), GUILayout.ExpandWidth(true)))
				SetSortingOrder(_order, _orderIncludeInactive);

            GUILayout.Space(2f);
            EditorGUILayout.EndVertical();
            GUILayout.Space(10f);


			// Offset Order.

			GUILayout.Space(10f);

			EditorGUILayout.LabelField("OFFSET ORDER", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUILayout.Space(5f);

			_offset = EditorGUILayout.IntField("Offset (can be negative):", _offset, EditorStyles.miniTextField, GUILayout.ExpandWidth(true));
			_offsetIncludeInactive = EditorGUILayout.Toggle("Include inactive ?", _offsetIncludeInactive);

			GUILayout.Space(5f);

			if (GUILayout.Button("Offset Orders Recursively", GUILayout.Height(45f), GUILayout.ExpandWidth(true)))
				OffsetSortingOrder(_offset, _offsetIncludeInactive);

			GUILayout.Space(2f);
			EditorGUILayout.EndVertical();
			GUILayout.Space(10f);


			// Layer.

			GUILayout.Space(10f);

			EditorGUILayout.LabelField("LAYER", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUILayout.Space(5f);

            _layerName = EditorGUILayout.TextField("Layer Name:", _layerName, EditorStyles.miniTextField, GUILayout.ExpandWidth(true));
            _layerIncludeInactive = EditorGUILayout.Toggle("Include inactive ?", _layerIncludeInactive);

            GUILayout.Space(5f);

            if (GUILayout.Button("Set Layers Recursively", GUILayout.Height(45f), GUILayout.ExpandWidth(true)))
                SetSortingLayer(_layerName, _layerIncludeInactive);

            GUILayout.Space(2f);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            GUILayout.Space(10f);
            EditorGUILayout.EndHorizontal();

            Repaint();
		}
	}
}