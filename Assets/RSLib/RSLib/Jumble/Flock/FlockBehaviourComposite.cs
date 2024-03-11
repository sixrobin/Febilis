namespace RSLib.Jumble.Flock
{
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// Composite behaviour makes the flock agents behave using a multiple weighted behaviours sequences.
    /// </summary>
    [CreateAssetMenu(fileName = "New Composite Flock Behaviour", menuName = "RSLib/Flock/Behaviour/Composite", order = -100)]
    public class FlockBehaviourComposite : FlockBehaviour
    {
        [System.Serializable]
        public struct BehaviourComponent
        {
            public FlockBehaviour Behaviour;
            public float Weight;
        }

        [SerializeField]
        private BehaviourComponent[] _behaviours = null;

        public override Vector2 ComputeVelocity(FlockAgent agent, List<Transform> context, Flock flock)
        {
            Vector2 velocity = Vector2.zero;

            for (int i = 0; i < _behaviours.Length; ++i)
            {
                float weight = _behaviours[i].Weight;

                Vector2 behaviourVelocity = _behaviours[i].Behaviour.ComputeVelocity(agent, context, flock);
                behaviourVelocity *= weight;

                if (behaviourVelocity == Vector2.zero)
                    continue;

                if (behaviourVelocity.sqrMagnitude > weight * weight)
                {
                    behaviourVelocity.Normalize();
                    behaviourVelocity *= weight;
                }

                velocity += behaviourVelocity;
            }

            return velocity;
        }

#if UNITY_EDITOR
        public void SortByWeight(bool ascending)
        {
            System.Array.Sort(_behaviours, (a, b) => a.Weight.CompareTo(b.Weight) * (ascending ? 1 : -1));
        }
#endif
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FlockBehaviourComposite.BehaviourComponent))]
    public class OptionalFloatPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative("Behaviour");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty behaviourProperty = property.FindPropertyRelative("Behaviour");
            SerializedProperty weightProperty = property.FindPropertyRelative("Weight");

            position.width -= 50;

            EditorGUI.PropertyField(position, behaviourProperty, label, true);

            position.x += position.width + 50;
            position.width = EditorGUI.GetPropertyHeight(weightProperty) + 26;
            position.height = EditorGUI.GetPropertyHeight(weightProperty);
            position.x -= position.width;

            EditorGUI.PropertyField(position, weightProperty, GUIContent.none);
        }
    }

    [CustomEditor(typeof(FlockBehaviourComposite))]
    public class FlockBehaviourCompositeEditor : Editor
    {
        private FlockBehaviourComposite _flockBehaviourComposite;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10f);

            if (GUILayout.Button("Sort by weight (ascending)"))
                _flockBehaviourComposite.SortByWeight(true);
            if (GUILayout.Button("Sort by weight (descending)"))
                _flockBehaviourComposite.SortByWeight(false);
        }

        private void OnEnable()
        {
            _flockBehaviourComposite = (FlockBehaviourComposite)target;
        }
    }
#endif
}