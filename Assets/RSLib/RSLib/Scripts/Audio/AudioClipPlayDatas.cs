namespace RSLib.Audio
{
    using UnityEngine;

    [System.Serializable]
    public class AudioClipPlayDatas : System.IComparable
    {
        [SerializeField] private AudioClip _clip = null;
        [SerializeField] private Vector2 _volumeRandomRange = Vector2.one;
        [SerializeField, Range(0f, 1f)] private float _pitchVariation = 0f;

        public AudioClip Clip => _clip;
        public float RandomVolume => Random.Range(_volumeRandomRange.x, _volumeRandomRange.y);
        public float PitchVariation => Random.Range(-_pitchVariation, _pitchVariation);

        public int CompareTo(object obj)
        {
            return Clip.name.CompareTo(((AudioClipPlayDatas)obj).Clip.name);
        }
    }
}