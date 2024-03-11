namespace RSLib.AStar
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(AStarMeshGrid))]
    public class AStarMeshGridEditor : Editor
    {
        private const string CLASS_INFO = "Generates a grid of nodes every object can use to move through a path.\n" +
            "Doesn't take rotation into account, and needs to be baked before entering play mode.";

        private AStarMeshGrid _grid;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(CLASS_INFO, MessageType.Info);
            GUILayout.Space(5);

            base.OnInspectorGUI();

            GUILayout.Space(5);
            if (GUILayout.Button("Preview"))
            {
                _grid.Refresh();
                EditorUtility.SetDirty(_grid);
            }
        }   

        private void OnEnable()
        {
            _grid = (AStarMeshGrid)target;
        }
    }
}