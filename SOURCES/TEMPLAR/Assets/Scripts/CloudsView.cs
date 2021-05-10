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

        private float _initSpeed;
        private float _minX;
        private float _maxX;

        private System.Collections.IEnumerator _setSpeedMultiplierCoroutine;

        public void SetSpeedMultiplier(float value, float transitionDur, RSLib.Maths.Curve curve = RSLib.Maths.Curve.InOutSine)
        {
            if (_setSpeedMultiplierCoroutine != null)
                StopCoroutine(_setSpeedMultiplierCoroutine);

            StartCoroutine(_setSpeedMultiplierCoroutine = SetSpeedMultiplierCoroutine(value, transitionDur, curve));
        }

        public void ResetSpeedMultiplier(float transitionDur, RSLib.Maths.Curve curve = RSLib.Maths.Curve.Linear)
        {
            SetSpeedMultiplier(_initSpeed, transitionDur, curve);
        }

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

        private System.Collections.IEnumerator SetSpeedMultiplierCoroutine(float value, float transitionDur, RSLib.Maths.Curve curve = RSLib.Maths.Curve.InOutSine)
        {
            float initSpeed = _speedMult;

            for (float t = 0f; t < 1f; t += Time.deltaTime / transitionDur)
            {
                _speedMult = Mathf.Lerp(initSpeed, value, RSLib.Maths.Easing.Ease(t, curve));
                yield return null;
            }

            _speedMult = value;
        }

        private void Awake()
        {
            // Bounds should exceed the values the player camera can reach so that cloud transition can't be seen.
            _minX = _cloudsBounds.bounds.min.x;
            _maxX = _cloudsBounds.bounds.max.x;

            _initSpeed = _speedMult;
        }

        private void Update()
        {
            MoveClouds();
        }

#if UNITY_EDITOR
        [ContextMenu("Find Clouds in children")]
        private void FindCloudsInChildren()
        {
            _clouds = GetComponentsInChildren<SpriteRenderer>();
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
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
            RSLib.EditorUtilities.SceneManagerUtilities.SetCurrentSceneDirty();
        }
#endif
    }
}