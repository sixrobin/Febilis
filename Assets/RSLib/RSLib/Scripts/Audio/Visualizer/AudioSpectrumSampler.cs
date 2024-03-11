namespace RSLib.Audio.Visualizer
{
    using UnityEngine;

    public class AudioSpectrumSampler : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource = null;
        [SerializeField] private SamplesCount _samplesCount = SamplesCount._512;
        [SerializeField, Range(2, 8)] private int _frequencyBandsCount = 8;

        private float[] _frequencyBandsBuffersDecrease;
        private float[] _frequencyBandsHighest;

        public enum SamplesCount
        {
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096,
            _8192 = 8192,
        }

        public float[] Samples { get; private set; }
        public float[] FrequencyBands { get; private set; }
        public float[] FrequencyBandsBuffers { get; private set; }
        public float[] AudioBands { get; private set; }
        public float[] AudioBandsBuffers { get; private set; }

        private void GetAudioSourceSpectrum()
        {
            _audioSource.GetSpectrumData(Samples, 0, FFTWindow.Blackman);
        }

        private void ComputeFrequencyBands()
        {
            int count = 0;

            for (int i = 0; i < FrequencyBands.Length; ++i)
            {
                int samplesCount = (int)Mathf.Pow(2, i) * 2;

                if (i == FrequencyBands.Length - 1)
                    samplesCount += 2;

                float samplesAverage = 0f;
                for (int j = 0; j < samplesCount; ++j)
                {
                    if (count > Samples.Length - 1)
                        break;

                    samplesAverage += Samples[count] * (count + 1);
                    count++;
                }

                samplesAverage /= count;
                FrequencyBands[i] = samplesAverage * 10f;
            }
        }

        private void ComputeFrequencyBandsBuffers()
        {
            for (int i = 0; i < FrequencyBandsBuffers.Length; ++i)
            {
                if (FrequencyBands[i] > FrequencyBandsBuffers[i])
                {
                    FrequencyBandsBuffers[i] = FrequencyBands[i];
                    _frequencyBandsBuffersDecrease[i] = 1 / 200f;
                }
                else if (FrequencyBands[i] < FrequencyBandsBuffers[i])
                {
                    FrequencyBandsBuffers[i] -= _frequencyBandsBuffersDecrease[i];
                    _frequencyBandsBuffersDecrease[i] *= 1.2f;
                }
            }
        }

        private void ComputeAudioBands()
        {
            for (int i = 0; i < AudioBands.Length; ++i)
            {
                if (FrequencyBands[i] > _frequencyBandsHighest[i])
                    _frequencyBandsHighest[i] = FrequencyBands[i];

                AudioBands[i] = FrequencyBands[i] / _frequencyBandsHighest[i];
                AudioBandsBuffers[i] = FrequencyBandsBuffers[i] / _frequencyBandsHighest[i];
            }
        }

        private void Awake()
        {
            Samples = new float[(int)_samplesCount];
            FrequencyBands = new float[_frequencyBandsCount];
            FrequencyBandsBuffers = new float[_frequencyBandsCount];
            AudioBands = new float[_frequencyBandsCount];
            AudioBandsBuffers = new float[_frequencyBandsCount];

            _frequencyBandsBuffersDecrease = new float[_frequencyBandsCount];
            _frequencyBandsHighest = new float[_frequencyBandsCount];
        }

        private void Update()
        {
            GetAudioSourceSpectrum();
            ComputeFrequencyBands();
            ComputeFrequencyBandsBuffers();
            ComputeAudioBands();
        }
    }
}