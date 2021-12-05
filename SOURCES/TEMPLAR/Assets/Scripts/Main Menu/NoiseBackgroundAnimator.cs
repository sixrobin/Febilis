namespace Templar.MainMenu
{
    using UnityEngine;

    public class NoiseBackgroundAnimator : MonoBehaviour
    {
        [SerializeField] private RSLib.Noise.NoiseMapGenerator _noiseMapGenerator = null;
        [SerializeField] private Vector2 _noiseOffsetSpeed = Vector2.zero;

        private void Update()
        {
            _noiseMapGenerator.AddOffset(_noiseOffsetSpeed * Time.deltaTime);
            _noiseMapGenerator.GenerateMap();
        }
    }
}