namespace RSLib.Audio.Visualizer
{
    using UnityEngine;

    public class AudioSpectrumVisualizer : MonoBehaviour
    {
        [SerializeField] private AudioSpectrumSampler _audioSpectrumSampler = null;
        [SerializeField] private GameObject _samplerCube = null;
        [SerializeField, Min(0f)] private float _radius = 100f;
        [SerializeField, Min(0f)] private float _minimumHeight = 2f;
        [SerializeField, Min(0f)] private float _maximumHeight = 50f;
        [SerializeField, Min(0f)] private float _cubeWidth = 10f;

        private Transform[] _samplerCubes;
        
        private System.Collections.IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            _samplerCubes = new Transform[_audioSpectrumSampler.Samples.Length];
            
            for (int i = 0; i < _samplerCubes.Length; ++i)
            {
                Transform samplerCubeInstance = Instantiate(_samplerCube, transform).transform;
                _samplerCubes[i] = samplerCubeInstance;
            }
        }

        private void Update()
        {
            if (_samplerCubes == null)
                return;

            for (int i = 0; i < _samplerCubes.Length; ++i)
            {
                Transform cube = _samplerCubes[i];
                
                float theta = (i * 2 * Mathf.PI) / _samplerCubes.Length;
                float x = Mathf.Sin(theta);
                float z = Mathf.Cos(theta);
                
                Vector3 position = transform.position + new Vector3(x, 0f, z) * _radius;

                Transform cubeTransform = cube.transform;
                cubeTransform.position = position;
                cubeTransform.forward = position;
                cube.localScale = new Vector3(_cubeWidth, _audioSpectrumSampler.Samples[i] * _maximumHeight + _minimumHeight, _cubeWidth);
            }
        }
    }
}
