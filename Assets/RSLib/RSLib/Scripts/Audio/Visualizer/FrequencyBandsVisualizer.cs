namespace RSLib.Audio.Visualizer
{
    using UnityEngine;

    public class FrequencyBandsVisualizer : MonoBehaviour
    {
        [SerializeField] private AudioSpectrumSampler _audioSpectrumSampler = null;
        [SerializeField] private bool _useBuffers = true;
        [SerializeField] private GameObject _frequencyBandCube = null;
        [SerializeField, Min(0f)] private float _spacing = 10f;
        [SerializeField, Min(0f)] private float _minimumHeight = 2f;
        [SerializeField, Min(0f)] private float _maximumHeight = 50f;
        [SerializeField, Min(0f)] private float _cubeWidth = 10f;

        private Transform[] _frequencyBandCubes;

        private System.Collections.IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            _frequencyBandCubes = new Transform[_audioSpectrumSampler.FrequencyBands.Length];

            for (int i = 0; i < _frequencyBandCubes.Length; ++i)
            {
                Transform samplerCubeInstance = Instantiate(_frequencyBandCube, transform).transform;
                _frequencyBandCubes[i] = samplerCubeInstance;
            }
        }

        private void Update()
        {
            if (_frequencyBandCubes == null)
                return;

            for (int i = 0; i < _frequencyBandCubes.Length; ++i)
            {
                Transform cube = _frequencyBandCubes[i];

                Vector3 position = transform.position + Vector3.right * (i * _spacing - (_frequencyBandCubes.Length / 2f) * (_spacing % 2 == 0 ? (_spacing - 1) : _spacing));

                cube.transform.position = position;
                cube.localScale = new Vector3(_cubeWidth,
                    (_useBuffers ? _audioSpectrumSampler.FrequencyBandsBuffers[i] : _audioSpectrumSampler.FrequencyBands[i]) * _maximumHeight + _minimumHeight,
                    _cubeWidth);
            }
        }
    }
}