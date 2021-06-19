namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class ParallaxEffect : MonoBehaviour
    {
        [SerializeField] private float _furthestLayerDist = 10f;
        [SerializeField] private float _furthestLayerFactor = 0.9f;
        [SerializeField] private Transform _camTransform = null;
        [SerializeField] private bool _clampDepthNormalization = true;

        [Tooltip("Transforms that will be directly affected by parallax.")]
        [SerializeField] private Transform[] _layers = null;

        [Tooltip("Transforms whose children will be affected by parallax, but not the transforms themselves." +
            "Those children are \"prewarmed\" and will be placed on Start so that they will appear to the player at the" +
            "position they have originally in the scene view.")]
        [SerializeField] private Transform[] _individualsObjectsParents = null;
        
        private Vector3 _initCamPos;

        private Vector3[] _layersInitOffsets;
        private float[] _layersDepths;

        private Transform[] _individualTransforms;
        private Vector3[] _individualTransformsOffsets;
        private float[] _individualTransformsDepths;

        private float TravelDistX => _camTransform.position.x - _initCamPos.x;

        private void PrewarmIndividualTransforms()
        {
            // Apply initial offset to individual objects.
            // This is done so that objects can be placed in the scene where they should be, and this loop will "prewarm" them
            // for the parallax system, so that the player will see them at the desired position.

            for (int i = _individualTransforms.Length - 1; i >= 0; --i)
            {
                float depthPercentage = _clampDepthNormalization
                    ? RSLib.Maths.Maths.Normalize01Clamped(_individualTransformsDepths[i], 0f, _furthestLayerDist)
                    : RSLib.Maths.Maths.Normalize01(_individualTransformsDepths[i], 0f, _furthestLayerDist);

                float travelFactor = depthPercentage * _furthestLayerFactor;
                _individualTransforms[i].AddPositionX(_individualTransformsOffsets[i].x * travelFactor);

                // Compute init offset again after prewarming.
                _individualTransformsOffsets[i] = _initCamPos - _individualTransforms[i].position;
            }
        }

        /// <summary>
        /// Positions a transform based on its depth and initial offset from camera.
        /// First, calculates the "travel factor", which is a normalization of the depth from the range [0,_furtherLayerDist] to 
        /// the range [0,1] that we multiply by _furthestLayerFactor.
        /// Second, we position the transform to its initial position relative to the camera initial position, and we add the
        /// total distance travelled by the camera multiplied by the previously evaluated travel factor.
        /// Transforms that have no depth have a factor of 0, meaning they won't follow the camera, while transforms at maximum depth
        /// have a factor of 1, making they completely follow the camera travel.
        /// </summary>
        /// <param name="t">Transform to position.</param>
        /// <param name="initOffset">Initial offset of the tranform for camera.</param>
        /// <param name="depth">Depth (position.z) of the object.</param>
        private void Position(Transform t, Vector3 initOffset, float depth)
        {
            float travelFactor = _clampDepthNormalization
                ? RSLib.Maths.Maths.Normalize01Clamped(depth, 0f, _furthestLayerDist) * _furthestLayerFactor
                : RSLib.Maths.Maths.Normalize01(depth, 0f, _furthestLayerDist) * _furthestLayerFactor;

            t.SetPositionX(_initCamPos.x - initOffset.x + TravelDistX * travelFactor);
        }

        private void Start()
        {
            _initCamPos = _camTransform.position;

            int i;

            // Arrays lengths initialization.

            _layersInitOffsets = new Vector3[_layers.Length];
            _layersDepths = new float[_layers.Length];

            int individualObjectsCount = 0;
            for (i = _individualsObjectsParents.Length - 1; i >= 0; --i)
                individualObjectsCount += _individualsObjectsParents[i].childCount;

            _individualTransforms = new Transform[individualObjectsCount];
            _individualTransformsOffsets = new Vector3[individualObjectsCount];
            _individualTransformsDepths = new float[individualObjectsCount];

            // Arrays contents initialization.

            for (i = _layers.Length - 1; i >= 0; --i)
            {
                _layersInitOffsets[i] = _initCamPos - _layers[i].position;
                _layersDepths[i] = _layers[i].position.z;
            }

            i = 0;
            for (int j = _individualsObjectsParents.Length - 1; j >= 0; --j)
            {
                for (int k = _individualsObjectsParents[j].childCount - 1; k >= 0; --k, ++i)
                {
                    _individualTransforms[i] = _individualsObjectsParents[j].GetChild(k);
                    _individualTransformsOffsets[i] = _initCamPos - _individualsObjectsParents[j].GetChild(k).position;
                    _individualTransformsDepths[i] = _individualsObjectsParents[j].GetChild(k).position.z;
                }
            }

            PrewarmIndividualTransforms();
        }

        private void Update()
        {
            for (int i = _layers.Length - 1; i >= 0; --i)
                Position(_layers[i], _layersInitOffsets[i], _layersDepths[i]);

            for (int i = _individualTransforms.Length - 1; i >= 0; --i)
                Position(_individualTransforms[i], _individualTransformsOffsets[i], _individualTransformsDepths[i]);
        }

        public void DebugCheckDuplicates()
        {
            System.Collections.Generic.Dictionary<Transform, int> transformsCounters = new System.Collections.Generic.Dictionary<Transform, int>();

            for (int i = _layers.Length - 1; i >= 0; --i)
            {
                if (!transformsCounters.ContainsKey(_layers[i]))
                    transformsCounters.Add(_layers[i], 0);

                transformsCounters[_layers[i]]++;
            }

            for (int i = _individualsObjectsParents.Length - 1; i >= 0; --i)
            {
                for (int j = _individualsObjectsParents[i].childCount - 1; j >= 0; --j)
                {
                    if (!transformsCounters.ContainsKey(_individualsObjectsParents[i].GetChild(j)))
                        transformsCounters.Add(_individualsObjectsParents[i].GetChild(j), 0);

                    transformsCounters[_individualsObjectsParents[i].GetChild(j)]++;
                }
            }

            bool anyDuplicataFound = false;
            foreach (System.Collections.Generic.KeyValuePair<Transform, int> counter in transformsCounters)
            {
                if (counter.Value > 1)
                {
                    CProLogger.Log(this, $"Duplicata found for {counter.Key.name} ({counter.Value} occurences).");
                    anyDuplicataFound = true;
                }
            }

            if (!anyDuplicataFound)
                CProLogger.Log(this, $"No duplicata found.");
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ParallaxEffect))]
    public class ParallaxEffectEditor : RSLib.EditorUtilities.ButtonProviderEditor<ParallaxEffect>
    {
        protected override void DrawButtons()
        {
            DrawButton("Check Duplicates", Obj.DebugCheckDuplicates);
        }
    }
#endif
}