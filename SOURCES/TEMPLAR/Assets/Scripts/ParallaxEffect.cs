namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;

    public class ParallaxEffect : MonoBehaviour
    {
        [SerializeField] private float _furthestLayerDist = 10f;
        [SerializeField] private float _furthestLayerFactor = 0.9f;
        [SerializeField] private Transform _camTransform = null;

        [SerializeField] private Transform[] _layers = null;

        [SerializeField] private Transform _test = null;

        private Vector3 _initCamPos;

        private Vector3[] _layersInitOffsets;
        private float[] _layersDepths;

        private Vector3 _testInitOffset;
        private float _testDepth;

        private float TravelDistX => _camTransform.position.x - _initCamPos.x;

        private void Awake()
        {
            _initCamPos = _camTransform.position;

            _layersInitOffsets = new Vector3[_layers.Length];
            _layersDepths = new float[_layers.Length];

            for (int i = _layers.Length - 1; i >= 0; --i)
            {
                _layersInitOffsets[i] = _initCamPos - _layers[i].position;
                _layersDepths[i] = _layers[i].position.z;
            }

            _testInitOffset = _initCamPos - _test.position;
            _testDepth = _test.position.z;

            float testDepthPercentage = RSLib.Maths.Maths.Normalize01(_testDepth, 0f, _furthestLayerDist);
            float testTravelFactor = testDepthPercentage * _furthestLayerFactor;
            Debug.Log(_testInitOffset.x * testTravelFactor);
            _test.AddPositionX(_testInitOffset.x * testTravelFactor);
            _testInitOffset = _initCamPos - _test.position;
        }

        private void Update()
        {
            for (int i = _layers.Length - 1; i >= 0; --i)
            {
                float depthPercentage = RSLib.Maths.Maths.Normalize01(_layersDepths[i], 0f, _furthestLayerDist);
                float travelFactor = depthPercentage * _furthestLayerFactor;

                _layers[i].SetPositionX(_initCamPos.x - _layersInitOffsets[i].x + TravelDistX * travelFactor);
            }

            float testDepthPercentage = RSLib.Maths.Maths.Normalize01(_testDepth, 0f, _furthestLayerDist);
            float testTravelFactor = testDepthPercentage * _furthestLayerFactor;
            _test.SetPositionX(_initCamPos.x - _testInitOffset.x + TravelDistX * testTravelFactor);
        }
    }
}