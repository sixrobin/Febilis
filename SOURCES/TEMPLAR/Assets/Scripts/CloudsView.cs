namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CloudsView : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _cloudsBounds = null;
        [SerializeField] private SpriteRenderer[] _clouds = null;

        [Space]
        [SerializeField] private Vector2 _cloudLengthMinMax = Vector2.zero;
        [SerializeField] private Vector2 _cloudSpeedMinMax = Vector2.zero;
        [SerializeField] private float _speedMult = 1f;

        private float _minX;
        private float _maxX;

        private void MoveClouds()
        {
            for (int i = _clouds.Length - 1; i >= 0; --i)
            {
                float speed = RSLib.Maths.Maths.Normalize(_clouds[i].size.x, _cloudLengthMinMax.x, _cloudLengthMinMax.y, _cloudSpeedMinMax.x, _cloudSpeedMinMax.y);
                speed *= _speedMult;

                _clouds[i].transform.Translate(Vector3.right * Time.deltaTime * speed);
                if (_clouds[i].bounds.min.x > _maxX)
                    _clouds[i].transform.position = _clouds[i].transform.position.WithX(_minX - _clouds[i].size.x * 0.5f);
            }
        }

        private void Awake()
        {
            // Bounds should exceed the values the player camera can reach so that cloud transition can't be seen.
            _minX = _cloudsBounds.bounds.min.x;
            _maxX = _cloudsBounds.bounds.max.x;
        }

        private void Update()
        {
            MoveClouds();
        }

        [ContextMenu("Find Clouds in children")]
        private void FindCloudsInChildren()
        {
            _clouds = GetComponentsInChildren<SpriteRenderer>();
        }

        [ContextMenu("Compute size range")]
        private void ComputeSizeRange()
        {
            float min = float.MaxValue;
            float max = float.MinValue;

            for (int i = _clouds.Length - 1; i >= 0; --i)
            {
                min = Mathf.Min(min, _clouds[i].size.x);
                max = Mathf.Max(min, _clouds[i].size.x);
            }

            _cloudLengthMinMax = new Vector2(min, max);
        }
    }
}