namespace RSLib.Framework.Events.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : Editor
    {
        private GameEvent _gameEvent;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Raise"))
                _gameEvent.Raise();
        }

        private void OnEnable()
        {
            _gameEvent = (GameEvent)target;
        }
    }
}
